using System;
using Grasshopper;
using Grasshopper.Kernel;

namespace GsaGH.Helpers {
  internal class Versions {
    internal static readonly Guid AdSecGuid = new Guid("f815c29a-e1eb-4ca6-9e56-0554777ff9c9");
    internal static readonly Guid ComposGuid = new Guid("c3884cdc-ac5b-4151-afc2-93590cef4f8f");

    internal const int AdSecMajorVersion = 0;
    internal const int AdSecMinorVersion = 9;
    internal const int AdSecBuildVersion = 22;

    internal const int ComposMajorVersion = 0;
    internal const int ComposMinorVersion = 9;
    internal const int ComposBuildVersion = 13;

    internal static bool _isAdSecOutDated;
    internal static bool _hasAdSecBeenChecked = false;

    internal static bool _isComposOutDated;
    internal static bool _hasComposBeenChecked = false;

    internal static bool CheckAdSecVersion() {
      if (_hasAdSecBeenChecked) {
        return _isAdSecOutDated;
      }

      GH_AssemblyInfo adSecInfo = Instances.ComponentServer.FindAssembly(AdSecGuid);
      _isComposOutDated = CheckVersion(adSecInfo, AdSecMajorVersion, AdSecMinorVersion, AdSecBuildVersion);

      return _isAdSecOutDated;
    }

    internal static bool CheckComposVersion() {
      if (_hasComposBeenChecked) {
        return _isComposOutDated;
      }

      GH_AssemblyInfo composInfo = Instances.ComponentServer.FindAssembly(ComposGuid);
      _isComposOutDated = CheckVersion(composInfo, ComposMajorVersion, ComposMinorVersion, ComposBuildVersion);

      return _isComposOutDated;
    }

    internal static bool CheckVersion(GH_AssemblyInfo info, int majorVersion, int minorVersion, int buildVersion) {
      if (info == null) {
        return false;
      }
      string[] installedVersion = info.Version.Split(new[] { "-beta", }, StringSplitOptions.None)[0].Split('.');
      if (int.TryParse(installedVersion[0], out int installedMajorVersion)) {
        if (installedMajorVersion < majorVersion) {
          return true;
        }
      }
      if (int.TryParse(installedVersion[1], out int installedMinorVersion)) {
        if (installedMinorVersion < minorVersion) {
          return true;
        }
      }
      if (int.TryParse(installedVersion[2], out int installedBuildVersion)) {
        if (installedBuildVersion < buildVersion) {
          return true;
        }
      }

      return false;
    }
  }
}
