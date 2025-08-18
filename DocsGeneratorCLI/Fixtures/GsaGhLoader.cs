using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace DocsGeneratorCLI {
  public class GsaGhDll {
    public static Assembly GsaGH;
    public static string PluginPath;
    public static XmlDocument GsaGhXml;

    public Assembly Load() {
      Console.WriteLine($"Start Loading GsaGH dll...");

      PluginPath = Assembly.GetExecutingAssembly().Location;
      PluginPath = Path.GetDirectoryName(PluginPath) ?? "";

      // Update environment PATH to include plugin path
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name) ?? "";
      string value = pathvar + ";" + PluginPath;
      Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);

      try {
        Console.WriteLine($"Loading Rhino/Grasshopper fixture");
        var grasshopper = new GrasshopperFixture();

        Console.WriteLine($"Loading GsaGH dll");
        GsaGH = Assembly.LoadFile(Path.Combine(PluginPath, "GsaGH.dll"));

        Console.WriteLine($"Loading GsaGH xml");
        GsaGhXml = new XmlDocument();
        GsaGhXml.Load(Path.Combine(PluginPath, "GsaGH.xml"));
      } catch (Exception e) {
        Console.WriteLine("Error loading GsaGH: " + e.Message);
      }

      Console.WriteLine($"Completed Loaded GsaGH dll");
      return GsaGH;
    }
  }
}
