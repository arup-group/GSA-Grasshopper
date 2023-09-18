using System;
using System.Reflection;
using System.Xml;

namespace DocsGeneration.Helpers {
  public class GsaGhDll {
    public static Assembly GsaGH;
    public static string PluginPath;
    public static XmlDocument GsaGhXml;

    public Assembly Load() {
      Console.WriteLine($"Start Loading GsaGH dll...");
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
        Console.WriteLine($"Loading Rhino/Grasshopper fixture");
        var grasshopper = new GrasshopperFixture();
        Console.WriteLine($"Loading GsaGH dll");
        GsaGH = Assembly.LoadFile(PluginPath + "\\GsaGH.dll");
        GsaGhXml = new XmlDocument();
        Console.WriteLine($"Loading GsaGH xml");
        GsaGhXml.Load(PluginPath + "\\GsaGH.xml");
      } catch (Exception e) {
        string msg = e.Message;
      }

      Console.WriteLine($"Completed Loaded GsaGH dll");
      return GsaGH;
    }
  }
}
