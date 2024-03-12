﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using FileActionsMenu.Helpers;
using FileActionsMenu.Helpers.Telemetry;
using FileActionsMenu.Interfaces;
using FileActionsMenu.Ui.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RoutedEventArgs = Microsoft.UI.Xaml.RoutedEventArgs;

namespace PowerToys.FileActionsMenu.Plugins.PathCopy
{
    internal sealed class CopyDirectoryPathWSL : IActionAndRequestCheckedMenuItems
    {
        private string[]? _selectedItems;
        private CheckedMenuItemsDictionary? _checkedMenuItemsDictionary;

        public string[] SelectedItems { get => _selectedItems.GetOrArgumentNullException(); set => _selectedItems = value; }

        public string Title => ResourceHelper.GetResource("Path_Copy.DirectoryPathWSL.Title");

        public IAction.ItemType Type => IAction.ItemType.SingleItem;

        public IAction[]? SubMenuItems => null;

        public int Category => 0;

        public IconElement? Icon => null;

        public bool IsVisible => true;

        public CheckedMenuItemsDictionary CheckedMenuItemsDictionary { get => _checkedMenuItemsDictionary.GetOrArgumentNullException(); set => _checkedMenuItemsDictionary = value; }

        public Task Execute(object sender, RoutedEventArgs e)
        {
            bool resolveShortcut = CheckedMenuItemsDictionary["f2544fd5-13f7-4d52-b7b4-00a3c70923e6"].First(checkedMenuItems => ((ToggleMenuFlyoutItem)checkedMenuItems.Item1).IsChecked).Item2 is ResolveShortcut;
            if (SelectedItems[0].EndsWith(".lnk", StringComparison.InvariantCultureIgnoreCase) && resolveShortcut)
            {
                SelectedItems[0] = ShortcutHelper.GetFullPathFromShortcut(SelectedItems[0]);
            }

            string tmpPath = Directory.Exists(SelectedItems[0])
                ? Directory.GetParent(SelectedItems[0])?.FullName ?? string.Empty
                : Path.GetDirectoryName(SelectedItems[0]) ?? string.Empty;
            Clipboard.SetText("/mnt/" + tmpPath[0].ToString().ToLowerInvariant() + tmpPath[1..].Replace("\\", "/").Replace(":/", "/"));

            TelemetryHelper.LogEvent(new FileActionsMenuCopyDirectoryPathActionInvokedEvent() { ResolveShortcut = resolveShortcut, IsWSLMode = true }, SelectedItems);

            return Task.CompletedTask;
        }
    }
}
