#include "pch.h"

#include <interface/powertoy_module_interface.h>

#include <common/interop/shared_constants.h>
#include <common/logger/logger.h>
#include <common/SettingsAPI/settings_helpers.h>
#include <common/SettingsAPI/settings_objects.h>
#include <common/utils/logger_helper.h>
#include <common/utils/resources.h>
#include <common/utils/winapi_error.h>

#include <ProjectsLib/trace.h>
#include <ProjectsLib/ProjectsData.h>

#include <shellapi.h>

#include "resource.h"
#include <common/utils/EventWaiter.h>

// Non-localizable
const std::wstring projectsLauncherPath = L"PowerToys.ProjectsLauncher.exe";
const std::wstring projectsSnapshotToolPath = L"PowerToys.ProjectsSnapshotTool.exe";
const std::wstring projectsEditorPath = L"PowerToys.ProjectsEditor.exe";

namespace
{
    const wchar_t JSON_KEY_PROPERTIES[] = L"properties";
    const wchar_t JSON_KEY_RUN_EDITOR_HOTKEY[] = L"hotkey";
    const wchar_t JSON_KEY_RUN_SNAPSHOT_TOOL_HOTKEY[] = L"run-snapshot-tool-hotkey";
    const wchar_t JSON_KEY_RUN_LAUNCHER_HOTKEY[] = L"run-launcher-hotkey";
    const wchar_t JSON_KEY_VALUE[] = L"value";
}

BOOL APIENTRY DllMain(HMODULE /*hModule*/, DWORD ul_reason_for_call, LPVOID /*lpReserved*/)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        Trace::RegisterProvider();
        break;

    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;

    case DLL_PROCESS_DETACH:
        Trace::UnregisterProvider();
        break;
    }
    return TRUE;
}

class ProjectsModuleInterface : public PowertoyModuleIface
{
public:
    EventWaiter m_toggleEditorEventWaiter;

    // Return the localized display name of the powertoy
    virtual PCWSTR get_name() override
    {
        return app_name.c_str();
    }

    // Return the non localized key of the powertoy, this will be cached by the runner
    virtual const wchar_t* get_key() override
    {
        return app_key.c_str();
    }

    virtual std::optional<HotkeyEx> GetHotkeyEx() override
    {
        return m_hotkey;
    }

    virtual void OnHotkeyEx() override
    {
        launch_editor();
    }

    // Return the configured status for the gpo policy for the module
    virtual powertoys_gpo::gpo_rule_configured_t gpo_policy_enabled_configuration() override
    {
        return powertoys_gpo::getConfiguredProjectsEnabledValue();
    }

    // Return JSON with the configuration options.
    // These are the settings shown on the settings page along with their current values.
    virtual bool get_config(_Out_ PWSTR buffer, _Out_ int* buffer_size) override
    {
        HINSTANCE hinstance = reinterpret_cast<HINSTANCE>(&__ImageBase);

        // Create a Settings object.
        PowerToysSettings::Settings settings(hinstance, get_name());
        settings.set_description(GET_RESOURCE_STRING(IDS_PROJECTS_SETTINGS_DESC));
        settings.set_overview_link(L"https://aka.ms/PowerToysOverview_AppLayouts");

        return settings.serialize_to_buffer(buffer, buffer_size);
    }

    // Passes JSON with the configuration settings for the powertoy.
    // This is called when the user hits Save on the settings page.
    virtual void set_config(PCWSTR config) override
    {
        try
        {
            // Parse the input JSON string.
            PowerToysSettings::PowerToyValues values = PowerToysSettings::PowerToyValues::from_json_string(config, get_key());

            parse_hotkeys(values);

            auto settingsObject = values.get_raw_json();
            values.save_to_settings_file();
        }
        catch (std::exception&)
        {
            // Improper JSON.
        }
    }

    // Signal from the Settings editor to call a custom action.
    // This can be used to spawn more complex editors.
    virtual void call_custom_action(const wchar_t* /*action*/) override
    {
        SetEvent(m_toggleEditorEvent);
    }

    // Enable the powertoy
    virtual void enable()
    {
        Logger::info("App Layouts enabling");
        Enable();
    }

    // Disable the powertoy
    virtual void disable()
    {
        Logger::info("App Layouts disabling");
        Disable(true);
    }

    // Returns if the powertoy is enabled
    virtual bool is_enabled() override
    {
        Logger::info("enabled = {}", m_enabled);
        return m_enabled;
    }

    // Destroy the powertoy and free memory
    virtual void destroy() override
    {
        Disable(false);

        if (m_toggleEditorEvent)
        {
            CloseHandle(m_toggleEditorEvent);
            m_toggleEditorEvent = nullptr;
        }

        delete this;
    }

    virtual void send_settings_telemetry() override
    {
    }

    ProjectsModuleInterface()
    {
        app_name = GET_RESOURCE_STRING(IDS_PROJECTS_NAME);
        app_key = L"App Layouts";
        LoggerHelpers::init_logger(app_key, L"ModuleInterface", "App Layouts");
        init_settings();

        m_toggleEditorEvent = CreateDefaultEvent(CommonSharedConstants::PROJECTS_LAUNCH_EDITOR_EVENT);
        if (!m_toggleEditorEvent)
        {
            Logger::error(L"Failed to create launch editor event");
            auto message = get_last_error_message(GetLastError());
            if (message.has_value())
            {
                Logger::error(message.value());
            }
        }
        m_toggleEditorEventWaiter = EventWaiter(CommonSharedConstants::PROJECTS_LAUNCH_EDITOR_EVENT, [&](int err) {
            if (err == ERROR_SUCCESS)
            {
                Logger::trace(L"{} event was signaled", CommonSharedConstants::PROJECTS_LAUNCH_EDITOR_EVENT);
                launch_editor();
            }
        });
    }

private:
    void Enable()
    {
        Logger::info("Enable");
        m_enabled = true;

        Trace::Projects::Enable(true);

        unsigned long powertoys_pid = GetCurrentProcessId();
        std::wstring executable_args = L"";
        executable_args.append(std::to_wstring(powertoys_pid));
    }

    void SendCloseEvent()
    {
        auto exitEvent = CreateEventW(nullptr, false, false, CommonSharedConstants::PROJECTS_EXIT_EVENT);
        if (!exitEvent)
        {
            Logger::warn(L"Failed to create exitEvent. {}", get_last_error_or_default(GetLastError()));
        }
        else
        {
            Logger::trace(L"Signaled exitEvent");
            if (!SetEvent(exitEvent))
            {
                Logger::warn(L"Failed to signal exitEvent. {}", get_last_error_or_default(GetLastError()));
            }

            ResetEvent(exitEvent);
            CloseHandle(exitEvent);
        }
    }

    void Disable(bool const traceEvent)
    {
        Logger::info("Disable");
        m_enabled = false;
        if (traceEvent)
        {
            Trace::Projects::Enable(false);
        }

        if (m_toggleEditorEvent)
        {
            ResetEvent(m_toggleEditorEvent);
        }

        if (m_hProcess)
        {
            TerminateProcess(m_hProcess, 0);
            SendCloseEvent();
            m_hProcess = nullptr;
        }
    }

    // Load the settings file.
    void init_settings()
    {
        try
        {
            Logger::trace(L"Read settings {}", get_key());
            // Load and parse the settings file for this PowerToy.
            PowerToysSettings::PowerToyValues settings = PowerToysSettings::PowerToyValues::load_from_settings_file(get_key());
            parse_hotkeys(settings);
        }
        catch (std::exception&)
        {
            Logger::warn(L"An exception occurred while loading the settings file");
            // Error while loading from the settings file. Let default values stay as they are.
        }
    }

    void parse_hotkeys(PowerToysSettings::PowerToyValues& settings)
    {
        auto settingsObject = settings.get_raw_json();

        if (settingsObject.GetView().Size())
        {
            if (settingsObject.GetNamedObject(JSON_KEY_PROPERTIES).HasKey(JSON_KEY_RUN_EDITOR_HOTKEY))
            {
                auto jsonPropertiesObject = settingsObject.GetNamedObject(JSON_KEY_PROPERTIES).GetNamedObject(JSON_KEY_RUN_EDITOR_HOTKEY).GetNamedObject(JSON_KEY_VALUE);
                auto hotkey = PowerToysSettings::HotkeyObject::from_json(jsonPropertiesObject);
                m_hotkey = HotkeyEx();
                if (hotkey.win_pressed())
                {
                    m_hotkey.modifiersMask |= MOD_WIN;
                }

                if (hotkey.ctrl_pressed())
                {
                    m_hotkey.modifiersMask |= MOD_CONTROL;
                }

                if (hotkey.shift_pressed())
                {
                    m_hotkey.modifiersMask |= MOD_SHIFT;
                }

                if (hotkey.alt_pressed())
                {
                    m_hotkey.modifiersMask |= MOD_ALT;
                }

                m_hotkey.vkCode = static_cast<WORD>(hotkey.get_code());
            }
        }
    }

    void launch_editor()
    {
        Logger::trace(L"Starting App Layouts Editor");

        /*unsigned long powertoys_pid = GetCurrentProcessId();
        std::wstring executable_args = L"";
        executable_args.append(std::to_wstring(powertoys_pid));*/

        SHELLEXECUTEINFOW sei{ sizeof(sei) };
        sei.fMask = SEE_MASK_NOCLOSEPROCESS;
        sei.lpFile = L"PowerToys.ProjectsEditor.exe";
        sei.nShow = SW_SHOWNORMAL;
        //sei.lpParameters = executable_args.data();
        if (ShellExecuteExW(&sei))
        {
            Logger::trace("Successfully started the App Layouts Editor");
        }
        else
        {
            Logger::error(L"App Layouts Editor failed to start. {}", get_last_error_or_default(GetLastError()));
        }

        m_hProcess = sei.hProcess;
    }

    std::wstring app_name;
    //contains the non localized key of the powertoy
    std::wstring app_key;

    bool m_enabled = false;
    HANDLE m_hProcess = nullptr;

    // Handle to event used to invoke Projects Editor
    HANDLE m_toggleEditorEvent;

    // Hotkey to invoke the module
    HotkeyEx m_hotkey{
        .modifiersMask = MOD_SHIFT | MOD_WIN,
        .vkCode = 0x4F, // O key;
    };
};

extern "C" __declspec(dllexport) PowertoyModuleIface* __cdecl powertoy_create()
{
    return new ProjectsModuleInterface();
}
