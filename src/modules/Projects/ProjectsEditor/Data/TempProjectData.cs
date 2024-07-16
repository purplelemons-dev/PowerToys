﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ProjectsEditor.Utils;

namespace ProjectsEditor.Data
{
    public class TempProjectData : ProjectData
    {
        public static string File
        {
            get
            {
                return FolderUtils.DataFolder() + "\\temp-project.json";
            }
        }

        public static void DeleteTempFile()
        {
            if (System.IO.File.Exists(File))
            {
                System.IO.File.Delete(File);
            }
        }
    }
}
