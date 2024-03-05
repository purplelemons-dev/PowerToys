﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Microsoft.FancyZonesEditor.UITests
{
    public static class Constants
    {
        public enum TemplateLayouts
        {
            Empty,
            Focus,
            Columns,
            Rows,
            Grid,
            PriorityGrid,
        }

        public static readonly Dictionary<TemplateLayouts, string> TemplateLayoutNames = new Dictionary<TemplateLayouts, string>()
        {
            { TemplateLayouts.Empty, "No layout" },
            { TemplateLayouts.Focus, "Focus" },
            { TemplateLayouts.Columns, "Columns" },
            { TemplateLayouts.Rows, "Rows" },
            { TemplateLayouts.Grid, "Grid" },
            { TemplateLayouts.PriorityGrid, "Priority Grid" },
        };

        public static readonly Dictionary<TemplateLayouts, string> TemplateLayoutTypes = new Dictionary<TemplateLayouts, string>()
        {
            { TemplateLayouts.Empty, "blank" },
            { TemplateLayouts.Focus, "focus" },
            { TemplateLayouts.Columns, "columns" },
            { TemplateLayouts.Rows, "rows" },
            { TemplateLayouts.Grid, "grid" },
            { TemplateLayouts.PriorityGrid, "priority-grid" },
        };

        public static readonly string CustomLayoutTypeString = "custom";

        public enum CustomLayoutType
        {
            Canvas,
            Grid,
        }

        public static readonly Dictionary<CustomLayoutType, string> CustomLayoutTypeNames = new Dictionary<CustomLayoutType, string>()
        {
            { CustomLayoutType.Canvas, "canvas" },
            { CustomLayoutType.Grid, "grid" },
        };
    }
}
