﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WorkspacesEditor.Utils
{
    public class MonitorHelper
    {
        private const int DpiAwarenessContextUnaware = -1;

        private Screen[] screens;

        [DllImport("user32.dll")]
        private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

        private void SaveDpiUnawareScreens()
        {
            SetThreadDpiAwarenessContext(DpiAwarenessContextUnaware);
            screens = Screen.AllScreens;
        }

        private Screen[] GetDpiUnawareScreenBounds()
        {
            Thread dpiUnawareThread = new Thread(new ThreadStart(SaveDpiUnawareScreens));
            dpiUnawareThread.Start();
            dpiUnawareThread.Join();

            return screens;
        }

        public static Screen[] GetDpiUnawareScreens()
        {
            MonitorHelper monitorHelper = new MonitorHelper();
            return monitorHelper.GetDpiUnawareScreenBounds();
        }

        internal static double GetScreenDpiFromScreen(Screen screen)
        {
            var point = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var monitor = NativeMethods.MonitorFromPoint(point, NativeMethods._MONITOR_DEFAULTTONEAREST);
            NativeMethods.GetDpiForMonitor(monitor, NativeMethods.DpiType.EFFECTIVE, out uint dpiX, out uint dpiY);
            return dpiX / 96.0;
        }
    }
}
