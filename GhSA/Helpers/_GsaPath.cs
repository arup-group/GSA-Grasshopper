﻿using System;
using System.IO;

namespace GhSA.Util.Gsa
{
    /// <summary>
    /// GsaPath class holding the path to the folder containing the GSA installation.
    /// Will be modified to account for different GSA versions etc.
    /// 
    /// </summary>
    public class InstallationFolderPath
    {
        // File path to GSA folder
        public static string GetPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),"Oasys", "GSA 10.1"); }
        }

    }
}
