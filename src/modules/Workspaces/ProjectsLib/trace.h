#pragma once

#include <modules/interface/powertoy_module_interface.h>

#include <ProjectsLib/ProjectsData.h>
#include <workspaces-common/InvokePoint.h>

class Trace
{
public:
    static void RegisterProvider() noexcept;
    static void UnregisterProvider() noexcept;

    class Workspaces
    {
    public:
        static void Enable(bool enabled) noexcept;
        static void Launch(bool success,
                           const WorkspacesData::WorkspacesProject& project,
                           InvokePoint invokePoint,
                           double launchTimeSeconds,
                           bool setupIsDifferent,
                           const std::vector<std::pair<std::wstring, std::wstring>> errors) noexcept;
    };
};
