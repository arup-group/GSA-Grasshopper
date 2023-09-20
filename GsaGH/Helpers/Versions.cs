using System;
using System.Drawing;
using System.Reflection;
using Grasshopper;
using Grasshopper.Kernel;
using GsaGH.Graphics;

namespace GsaGH.Helpers {
  internal class Versions {
    internal static readonly Guid AdSecGuid = new Guid("f815c29a-e1eb-4ca6-9e56-0554777ff9c9");
    internal static readonly Guid ComposGuid = new Guid("c3884cdc-ac5b-4151-afc2-93590cef4f8f");
    internal static readonly Guid GsaGuid = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");

    internal static Version OasysGhVersion {
      get {
        if (_oasysGhVersion == null) {
          _oasysGhVersion = GetGsaGhOasyGhVersion();
        }
        
        return _oasysGhVersion;
      }
    }
    private static Version _oasysGhVersion = null;

    internal static void Check() {
      bool isAdSecOutdated = IsPluginOutdated(AdSecGuid);
      bool isComposOutdated = IsPluginOutdated(ComposGuid);
      if (!isAdSecOutdated && !isComposOutdated) {
        return;
      }

      string process = @"rhino://package/search?name=guid:";
      string text = "An update is available for ";
      string header = "Update ";
      Bitmap icon = Properties.Resources.OasysGHUpdate;

      switch (isAdSecOutdated, isComposOutdated) {
        case (true, false):
          text += "AdSec";
          process += AdSecGuid;
          header += "AdSecGH Plugin";
          icon = Properties.Resources.AdSecGHUpdate;
          break;

        case (false, true):
          text += "Compos";
          process += ComposGuid;
          header += "ComposGH Plugin";
          icon = Properties.Resources.ComposGHUpdate;
          break;

        default:
          text = "Updates are avaiable for AdSecGH and ComposGH";
          process = @"rhino://package/search?name=oasys";
          header += "Oasys Plugins";
          break;
      }

      text += ".\n\nClick OK to update now.";
      var updateComposBox = new UpdatePluginsBox(header, text, process, icon);
      updateComposBox.ShowDialog();
    }

    private static bool IsPluginOutdated(Guid guid) {
      GH_AssemblyInfo pluginInfo = Instances.ComponentServer.FindAssembly(guid);
      if (pluginInfo == null) {
        return false;
      }

      Version pluginOasysGhVersion = GetOasyGhVersion(pluginInfo.Location);
      return pluginOasysGhVersion.CompareTo(OasysGhVersion) < 0;
    }

    private static Version GetGsaGhOasyGhVersion() {
      GH_AssemblyInfo gsaGhInfo = Instances.ComponentServer.FindAssembly(GsaGhInfo.guid);
      return GetOasyGhVersion(gsaGhInfo.Location);
    }

    private static Version GetOasyGhVersion(string path) {
      var oasyGh = AssemblyName.GetAssemblyName(path + @"\OasysGH.dll");
      return oasyGh.Version;
    }
  }
}
