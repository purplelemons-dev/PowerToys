#include "pch.h"
#include "ProjectsData.h"

#include <common/SettingsAPI/settings_helpers.h>

namespace NonLocalizable
{
    const inline wchar_t ModuleKey[] = L"Workspaces";
}

namespace ProjectsData
{
    std::wstring WorkspacesFile()
    {
        std::wstring settingsFolderPath = PTSettingsHelper::get_module_save_folder_location(NonLocalizable::ModuleKey);
        return settingsFolderPath + L"\\workspaces.json";
    }

    std::wstring TempWorkspacesFile()
    {
        std::wstring settingsFolderPath = PTSettingsHelper::get_module_save_folder_location(NonLocalizable::ModuleKey);
        return settingsFolderPath + L"\\temp-workspaces.json";
    }

    std::wstring LaunchWorkspacesFile()
    {
        std::wstring settingsFolderPath = PTSettingsHelper::get_module_save_folder_location(NonLocalizable::ModuleKey);
        return settingsFolderPath + L"\\launch-workspaces.json";
    }
    
    RECT Project::Application::Position::toRect() const noexcept
    {
        return RECT{ .left = x, .top = y, .right = x + width, .bottom = y + height };
    }

    namespace ProjectJSON
    {
        namespace ApplicationJSON
        {
            namespace PositionJSON
            {
                namespace NonLocalizable
                {
                    const static wchar_t* XAxisID = L"X";
                    const static wchar_t* YAxisID = L"Y";
                    const static wchar_t* WidthID = L"width";
                    const static wchar_t* HeightID = L"height";
                }

                json::JsonObject ToJson(const Project::Application::Position& position)
                {
                    json::JsonObject json{};
                    json.SetNamedValue(NonLocalizable::XAxisID, json::value(position.x));
                    json.SetNamedValue(NonLocalizable::YAxisID, json::value(position.y));
                    json.SetNamedValue(NonLocalizable::WidthID, json::value(position.width));
                    json.SetNamedValue(NonLocalizable::HeightID, json::value(position.height));
                    return json;
                }

                std::optional<Project::Application::Position> FromJson(const json::JsonObject& json)
                {
                    Project::Application::Position result;
                    try
                    {
                        result.x = static_cast<int>(json.GetNamedNumber(NonLocalizable::XAxisID, 0));
                        result.y = static_cast<int>(json.GetNamedNumber(NonLocalizable::YAxisID, 0));
                        result.width = static_cast<int>(json.GetNamedNumber(NonLocalizable::WidthID, 0));
                        result.height = static_cast<int>(json.GetNamedNumber(NonLocalizable::HeightID, 0));
                    }
                    catch (const winrt::hresult_error&)
                    {
                        return std::nullopt;
                    }

                    return result;
                }
            }

            namespace NonLocalizable
            {
                const static wchar_t* AppNameID = L"application";
                const static wchar_t* AppPathID = L"application-path";
                const static wchar_t* AppPackageFullNameID = L"package-full-name";
                const static wchar_t* AppTitleID = L"title";
                const static wchar_t* CommandLineArgsID = L"command-line-arguments";
                const static wchar_t* ElevatedID = L"is-elevated";
                const static wchar_t* CanLaunchElevatedID = L"can-launch-elevated";
                const static wchar_t* MinimizedID = L"minimized";
                const static wchar_t* MaximizedID = L"maximized";
                const static wchar_t* PositionID = L"position";
                const static wchar_t* MonitorID = L"monitor";
            }

            json::JsonObject ToJson(const Project::Application& data)
            {
                json::JsonObject json{};
                json.SetNamedValue(NonLocalizable::AppNameID, json::value(data.name));
                json.SetNamedValue(NonLocalizable::AppPathID, json::value(data.path));
                json.SetNamedValue(NonLocalizable::AppTitleID, json::value(data.title));
                json.SetNamedValue(NonLocalizable::AppPackageFullNameID, json::value(data.packageFullName));
                json.SetNamedValue(NonLocalizable::CommandLineArgsID, json::value(data.commandLineArgs));
                json.SetNamedValue(NonLocalizable::ElevatedID, json::value(data.isElevated));
                json.SetNamedValue(NonLocalizable::CanLaunchElevatedID, json::value(data.canLaunchElevated));
                json.SetNamedValue(NonLocalizable::MinimizedID, json::value(data.isMinimized));
                json.SetNamedValue(NonLocalizable::MaximizedID, json::value(data.isMaximized));
                json.SetNamedValue(NonLocalizable::PositionID, PositionJSON::ToJson(data.position));
                json.SetNamedValue(NonLocalizable::MonitorID, json::value(data.monitor));

                return json;
            }

            std::optional<Project::Application> FromJson(const json::JsonObject& json)
            {
                Project::Application result;
                try
                {
                    if (json.HasKey(NonLocalizable::AppNameID))
                    {
                        result.name = json.GetNamedString(NonLocalizable::AppNameID);
                    }

                    result.path = json.GetNamedString(NonLocalizable::AppPathID);
                    result.title = json.GetNamedString(NonLocalizable::AppTitleID);
                    if (json.HasKey(NonLocalizable::AppPackageFullNameID))
                    {
                        result.packageFullName = json.GetNamedString(NonLocalizable::AppPackageFullNameID);
                    }

                    result.commandLineArgs = json.GetNamedString(NonLocalizable::CommandLineArgsID);

                    if (json.HasKey(NonLocalizable::ElevatedID))
                    {
                        result.isElevated = json.GetNamedBoolean(NonLocalizable::ElevatedID);
                    }

                    if (json.HasKey(NonLocalizable::CanLaunchElevatedID))
                    {
                        result.canLaunchElevated = json.GetNamedBoolean(NonLocalizable::CanLaunchElevatedID);
                    }

                    result.isMaximized = json.GetNamedBoolean(NonLocalizable::MaximizedID);
                    result.isMinimized = json.GetNamedBoolean(NonLocalizable::MinimizedID);
                    result.monitor = static_cast<int>(json.GetNamedNumber(NonLocalizable::MonitorID));
                    if (json.HasKey(NonLocalizable::PositionID))
                    {
                        auto position = PositionJSON::FromJson(json.GetNamedObject(NonLocalizable::PositionID));
                        if (!position.has_value())
                        {
                            return std::nullopt;
                        }

                        result.position = position.value();
                    }
                }
                catch (const winrt::hresult_error&)
                {
                    return std::nullopt;
                }

                return result;
            }
        }

        namespace MonitorJSON
        {
            namespace MonitorRectJSON
            {
                namespace NonLocalizable
                {
                    const static wchar_t* TopID = L"top";
                    const static wchar_t* LeftID = L"left";
                    const static wchar_t* WidthID = L"width";
                    const static wchar_t* HeightID = L"height";
                }

                json::JsonObject ToJson(const Project::Monitor::MonitorRect& data)
                {
                    json::JsonObject json{};
                    json.SetNamedValue(NonLocalizable::TopID, json::value(data.top));
                    json.SetNamedValue(NonLocalizable::LeftID, json::value(data.left));
                    json.SetNamedValue(NonLocalizable::WidthID, json::value(data.width));
                    json.SetNamedValue(NonLocalizable::HeightID, json::value(data.height));

                    return json;
                }

                std::optional<Project::Monitor::MonitorRect> FromJson(const json::JsonObject& json)
                {
                    Project::Monitor::MonitorRect result;
                    try
                    {
                        result.top = static_cast<int>(json.GetNamedNumber(NonLocalizable::TopID));
                        result.left = static_cast<int>(json.GetNamedNumber(NonLocalizable::LeftID));
                        result.width = static_cast<int>(json.GetNamedNumber(NonLocalizable::WidthID));
                        result.height = static_cast<int>(json.GetNamedNumber(NonLocalizable::HeightID));
                    }
                    catch (const winrt::hresult_error&)
                    {
                        return std::nullopt;
                    }

                    return result;
                }
            }

            namespace NonLocalizable
            {
                const static wchar_t* MonitorID = L"id";
                const static wchar_t* InstanceID = L"instance-id";
                const static wchar_t* NumberID = L"monitor-number";
                const static wchar_t* DpiID = L"dpi";
                const static wchar_t* MonitorRectDpiAwareID = L"monitor-rect-dpi-aware";
                const static wchar_t* MonitorRectDpiUnawareID = L"monitor-rect-dpi-unaware";
            }

            json::JsonObject ToJson(const Project::Monitor& data)
            {
                json::JsonObject json{};
                json.SetNamedValue(NonLocalizable::MonitorID, json::value(data.id));
                json.SetNamedValue(NonLocalizable::InstanceID, json::value(data.instanceId));
                json.SetNamedValue(NonLocalizable::NumberID, json::value(data.number));
                json.SetNamedValue(NonLocalizable::DpiID, json::value(data.dpi));
                json.SetNamedValue(NonLocalizable::MonitorRectDpiAwareID, MonitorRectJSON::ToJson(data.monitorRectDpiAware));
                json.SetNamedValue(NonLocalizable::MonitorRectDpiUnawareID, MonitorRectJSON::ToJson(data.monitorRectDpiUnaware));

                return json;
            }

            std::optional<Project::Monitor> FromJson(const json::JsonObject& json)
            {
                Project::Monitor result;
                try
                {
                    result.id = json.GetNamedString(NonLocalizable::MonitorID);
                    result.instanceId = json.GetNamedString(NonLocalizable::InstanceID);
                    result.number = static_cast<int>(json.GetNamedNumber(NonLocalizable::NumberID));
                    result.dpi = static_cast<int>(json.GetNamedNumber(NonLocalizable::DpiID));
                    auto rectDpiAware = MonitorRectJSON::FromJson(json.GetNamedObject(NonLocalizable::MonitorRectDpiAwareID));
                    if (!rectDpiAware.has_value())
                    {
                        return std::nullopt;
                    }

                    auto rectDpiUnaware = MonitorRectJSON::FromJson(json.GetNamedObject(NonLocalizable::MonitorRectDpiUnawareID));
                    if (!rectDpiUnaware.has_value())
                    {
                        return std::nullopt;
                    }

                    result.monitorRectDpiAware = rectDpiAware.value();
                    result.monitorRectDpiUnaware = rectDpiUnaware.value();
                }
                catch (const winrt::hresult_error&)
                {
                    return std::nullopt;
                }

                return result;
            }
        }

        namespace NonLocalizable
        {
            const static wchar_t* IdID = L"id";
            const static wchar_t* NameID = L"name";
            const static wchar_t* CreationTimeID = L"creation-time";
            const static wchar_t* LastLaunchedTimeID = L"last-launched-time";
            const static wchar_t* IsShortcutNeededID = L"is-shortcut-needed";
            const static wchar_t* MoveExistingWindowsID = L"move-existing-windows";
            const static wchar_t* MonitorConfigurationID = L"monitor-configuration";
            const static wchar_t* AppsID = L"applications";
        }

        json::JsonObject ToJson(const Project& data)
        {
            json::JsonObject json{};

            json::JsonArray appsArray{};
            for (const auto& app : data.apps)
            {
                appsArray.Append(ApplicationJSON::ToJson(app));
            }

            json::JsonArray monitorsArray{};
            for (const auto& monitor : data.monitors)
            {
                monitorsArray.Append(MonitorJSON::ToJson(monitor));
            }

            json.SetNamedValue(NonLocalizable::IdID, json::value(data.id));
            json.SetNamedValue(NonLocalizable::NameID, json::value(data.name));
            json.SetNamedValue(NonLocalizable::CreationTimeID, json::value(static_cast<long>(data.creationTime)));
            if (data.lastLaunchedTime.has_value())
            {
                json.SetNamedValue(NonLocalizable::LastLaunchedTimeID, json::value(static_cast<long>(data.lastLaunchedTime.value())));
            }
            json.SetNamedValue(NonLocalizable::IsShortcutNeededID, json::value(data.isShortcutNeeded));
            json.SetNamedValue(NonLocalizable::MoveExistingWindowsID, json::value(data.moveExistingWindows));
            json.SetNamedValue(NonLocalizable::MonitorConfigurationID, monitorsArray);
            json.SetNamedValue(NonLocalizable::AppsID, appsArray);
            return json;
        }

        std::optional<Project> FromJson(const json::JsonObject& json)
        {
            Project result{};

            try
            {
                result.id = json.GetNamedString(NonLocalizable::IdID);
                result.name = json.GetNamedString(NonLocalizable::NameID);
                result.creationTime = static_cast<time_t>(json.GetNamedNumber(NonLocalizable::CreationTimeID));

                if (json.HasKey(NonLocalizable::LastLaunchedTimeID))
                {
                    result.lastLaunchedTime = static_cast<time_t>(json.GetNamedNumber(NonLocalizable::LastLaunchedTimeID));
                }

                if (json.HasKey(NonLocalizable::IsShortcutNeededID))
                {
                    result.isShortcutNeeded = json.GetNamedBoolean(NonLocalizable::IsShortcutNeededID);
                }
                
                if (json.HasKey(NonLocalizable::MoveExistingWindowsID))
                {
					result.moveExistingWindows = json.GetNamedBoolean(NonLocalizable::MoveExistingWindowsID);
				}

                auto appsArray = json.GetNamedArray(NonLocalizable::AppsID);
                for (uint32_t i = 0; i < appsArray.Size(); ++i)
                {
                    auto obj = ApplicationJSON::FromJson(appsArray.GetObjectAt(i));
                    if (!obj.has_value())
                    {
                        return std::nullopt;
                    }

                    result.apps.push_back(obj.value());
                }

                auto monitorsArray = json.GetNamedArray(NonLocalizable::MonitorConfigurationID);
                for (uint32_t i = 0; i < monitorsArray.Size(); ++i)
                {
                    auto obj = MonitorJSON::FromJson(monitorsArray.GetObjectAt(i));
                    if (!obj.has_value())
                    {
                        return std::nullopt;
                    }

                    result.monitors.push_back(obj.value());
                }
            }
            catch (const winrt::hresult_error&)
            {
                return std::nullopt;
            }

            return result;
        }
    }

    namespace ProjectsListJSON
    {
        namespace NonLocalizable
        {
            const static wchar_t* ProjectsID = L"projects";
        }

        json::JsonObject ToJson(const std::vector<Project>& data)
        {
            json::JsonObject json{};
            json::JsonArray projectsArray{};

            for (const auto& project : data)
            {
                projectsArray.Append(ProjectJSON::ToJson(project));
            }

            json.SetNamedValue(NonLocalizable::ProjectsID, projectsArray);
            return json;
        }

        std::optional<std::vector<Project>> FromJson(const json::JsonObject& json)
        {
            std::vector<Project> result{};

            try
            {
                auto array = json.GetNamedArray(NonLocalizable::ProjectsID);
                for (uint32_t i = 0; i < array.Size(); ++i)
                {
                    auto obj = ProjectJSON::FromJson(array.GetObjectAt(i));
                    if (obj.has_value())
                    {
                        result.push_back(obj.value());
                    }
                    else
                    {
                        return std::nullopt;
                    }
                }
            }
            catch (const winrt::hresult_error&)
            {
                return std::nullopt;
            }

            return result;
        }
    }

    namespace AppLaunchInfoJSON
    {
        namespace NonLocalizable
        {
            const static wchar_t* NameID = L"name";
            const static wchar_t* PathID = L"path";
            const static wchar_t* StateID = L"state";
        }

        json::JsonObject ToJson(const AppLaunchInfo& data)
        {
            json::JsonObject json{};
            json.SetNamedValue(NonLocalizable::NameID, json::value(data.name));
            json.SetNamedValue(NonLocalizable::PathID, json::value(data.path));
            json.SetNamedValue(NonLocalizable::StateID, json::value(data.state));
            return json;
        }
    }

    namespace AppLaunchInfoListJSON
    {
        namespace NonLocalizable
        {
            const static wchar_t* AppLaunchInfoID = L"appLaunchInfos";
        }

        json::JsonObject ToJson(const std::vector<AppLaunchInfo>& data)
        {
            json::JsonObject json{};
            json::JsonArray appLaunchInfoArray{};
            for (const auto& appLaunchInfo : data)
            {
                appLaunchInfoArray.Append(AppLaunchInfoJSON::ToJson(appLaunchInfo));
            }

            json.SetNamedValue(NonLocalizable::AppLaunchInfoID, appLaunchInfoArray);
            return json;
        }
    }

    namespace AppLaunchDataJSON
    {
        namespace NonLocalizable
        {
            const static wchar_t* AppsID = L"apps";
            const static wchar_t* ProcessID = L"processId";
        }

        json::JsonObject ToJson(const AppLaunchData& data)
        {
            json::JsonObject json{};
            json.SetNamedValue(NonLocalizable::AppsID, AppLaunchInfoListJSON::ToJson(data.appLaunchInfoList));
            json.SetNamedValue(NonLocalizable::ProcessID, json::value(data.launcherProcessID));
            return json;
        }
    }
}
