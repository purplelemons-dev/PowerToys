﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Projects.Data;
using static ProjectsEditor.Data.ProjectData;

namespace ProjectsEditor.Data
{
    public class ProjectData : ProjectsEditorData<ProjectWrapper>
    {
        public struct ApplicationWrapper
        {
            public struct WindowPositionWrapper
            {
                public int X { get; set; }

                public int Y { get; set; }

                public int Width { get; set; }

                public int Height { get; set; }
            }

            public string Application { get; set; }

            public string ApplicationPath { get; set; }

            public string Title { get; set; }

            public string PackageFullName { get; set; }

            public string CommandLineArguments { get; set; }

            public bool IsElevated { get; set; }

            public bool Minimized { get; set; }

            public bool Maximized { get; set; }

            public WindowPositionWrapper Position { get; set; }

            public int Monitor { get; set; }
        }

        public struct MonitorConfigurationWrapper
        {
            public struct MonitorRectWrapper
            {
                public int Top { get; set; }

                public int Left { get; set; }

                public int Width { get; set; }

                public int Height { get; set; }
            }

            public string Id { get; set; }

            public string InstanceId { get; set; }

            public int MonitorNumber { get; set; }

            public int Dpi { get; set; }

            public MonitorRectWrapper MonitorRectDpiAware { get; set; }

            public MonitorRectWrapper MonitorRectDpiUnaware { get; set; }
        }

        public struct ProjectWrapper
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public long CreationTime { get; set; }

            public long LastLaunchedTime { get; set; }

            public bool IsShortcutNeeded { get; set; }

            public List<MonitorConfigurationWrapper> MonitorConfiguration { get; set; }

            public List<ApplicationWrapper> Applications { get; set; }
        }
    }
}
