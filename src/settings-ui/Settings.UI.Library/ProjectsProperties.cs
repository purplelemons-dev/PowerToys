﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.PowerToys.Settings.UI.Library
{
    public class ProjectsProperties
    {
        public enum SortByProperty
        {
            LastLaunched,
            Created,
            Name,
        }

        public static readonly HotkeySettings DefaultHotkeyValue = new HotkeySettings(true, false, false, true, 0x4F);

        public ProjectsProperties()
        {
            Hotkey = new KeyboardKeysProperty(DefaultHotkeyValue);
        }

        [JsonPropertyName("hotkey")]
        public KeyboardKeysProperty Hotkey { get; set; }

        [JsonPropertyName("sortby")]
        public SortByProperty SortBy { get; set; }

        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
