using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GsaGhDocs.Helpers {
  public class GsaGhDll {
    public static Assembly GsaGH;
    public static string PluginPath;

    public Assembly Load() {
      // ## Get plugin assembly file location
      PluginPath = Assembly.GetExecutingAssembly().Location; // full path+name
      PluginPath = PluginPath.Replace("DocsGeneration.exe", "");

      // ### Set system environment variables to allow user rights to read above dll ###
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      string value = pathvar + ";" + PluginPath;
      EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);

      try {
        var grasshopper = new GrasshopperFixture();
        GsaGH = Assembly.LoadFile(PluginPath + "\\GsaGH.dll");
      } catch (Exception e) {
        string msg = e.Message;
      }
      return GsaGH;
    }
  }
}
