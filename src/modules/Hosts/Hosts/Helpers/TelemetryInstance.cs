﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;

namespace Hosts.Helpers
{
    public class TelemetryInstance
    {
        public static EventSource TelemetrySource { get; set; }
    }
}
