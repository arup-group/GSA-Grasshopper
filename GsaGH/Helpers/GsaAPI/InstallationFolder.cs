using System;
using System.IO;

namespace GsaGH.Helpers.GsaApi {
  /// <summary>
  ///   GsaPath class holding the path to the folder containing the GSA installation.
  ///   Will be modified to account for different GSA versions etc.
  /// </summary>
  public static class InstallationFolder {
    public static string GetPath
      => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys",
        "GSA 10.3");
  }
}
