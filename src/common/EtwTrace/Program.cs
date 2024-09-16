// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Threading;
using System.Windows.Forms;
using ManagedCommon;
using Microsoft.PowerToys.Telemetry;

namespace EtwTrace
{
    internal static class Program
    {
        private static ManualResetEvent quitEvent = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            int runnerPid = -1;
            if (!int.TryParse(args[0], out runnerPid))
            {
                return;
            }

            ETWTrace eTWTrace = new ETWTrace();
            eTWTrace.Start();

            RunnerHelper.WaitForPowerToysRunner(runnerPid, () =>
            {
                eTWTrace.Dispose();
                quitEvent.Set();
            });

            quitEvent.WaitOne();
        }
    }
}
