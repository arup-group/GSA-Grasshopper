using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

namespace DocsGeneratorCLI {
  public class GsaGhDll {
#pragma warning disable S1104 // Fields should not have public accessibility
#pragma warning disable S2223 // Non-constant static fields should not be visible
    public static Assembly GsaGH;
    public static string PluginPath;
    public static XmlDocument GsaGhXml;
    public const string GsaGhName = "GsaGH";
#pragma warning restore S1104 // Fields should not have public accessibility
#pragma warning restore S2223 // Non-constant static fields should not be visible
#pragma warning disable S2696 // Instance members should not write to "static" fields
    public static Assembly Load() {
      Console.WriteLine($"==> [{GsaGhName}] Start loading...");

      Console.WriteLine("Loading Rhino/Grasshopper fixture");
#pragma warning disable S1481 // Unused local variables should be removed
      var grasshopper = new GrasshopperFixture("GsaGh");
#pragma warning restore S1481 // Unused local variables should be removed

      PluginPath = GetPluginDirectory();
      string dllPath = TryFindDll() ?? TryBuildAndFindDll();

      if (dllPath == null) {
        Console.WriteLine($"Couldn't find {GsaGhName}.dll");
        return null;
      }

      UpdateEnvironmentPath(Path.GetDirectoryName(dllPath));

      GsaGH = LoadAssembly(dllPath);
      LoadXmlIfExists(dllPath);

      Console.WriteLine($"Finished loading {GsaGhName}");
      return GsaGH;
    }

    // === Submethods ===

    private static string GetPluginDirectory() {
      return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
    }

    private static string TryFindDll() {
      string[] searchPaths = {
        Path.Combine(PluginPath, $"{GsaGhName}.dll"),
        Path.Combine(PluginPath, "..", GsaGhName, "bin", "Debug", $"{GsaGhName}.dll"),
        Path.Combine(PluginPath, "..", "..", GsaGhName, "bin", "Debug", $"{GsaGhName}.dll"),
      };

      foreach (string path in searchPaths) {
        string full = Path.GetFullPath(path);
        if (!File.Exists(full)) {
          continue;
        }

        Console.WriteLine($"Found DLL: {full}");
        return full;
      }

      return null;
    }

    private static string TryBuildAndFindDll() {
      string csprojPath = FindCsproj();
      if (csprojPath == null) {
        Console.WriteLine($"Didn't found {GsaGhName}.csproj");
        return null;
      }

      Console.WriteLine($"Building project: {csprojPath}");
      BuildProject(csprojPath);

      string dllPath = Path.Combine(Path.GetDirectoryName(csprojPath), "bin", "Debug", $"{GsaGhName}.dll");
      return File.Exists(dllPath) ? dllPath : null;
    }

    private static string FindCsproj() {
      string[] candidates = {
        Path.Combine(PluginPath, "..", GsaGhName, $"{GsaGhName}.csproj"),
        Path.Combine(PluginPath, "..", "..", GsaGhName, $"{GsaGhName}.csproj"),
      };

      foreach (string path in candidates) {
        string full = Path.GetFullPath(path);
        if (!File.Exists(full)) {
          continue;
        }

        return full;
      }

      return null;
    }

    private static void BuildProject(string csprojPath) {
      var startInfo = new ProcessStartInfo {
        FileName = "cmd.exe",
        Arguments = $"/c msbuild \"{csprojPath}\" /p:Configuration=Debug",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
      };

      try {
        using (var process = Process.Start(startInfo)) {
          string output = process.StandardOutput.ReadToEnd();
          string error = process.StandardError.ReadToEnd();
          process.WaitForExit();

          Console.WriteLine(output);

          if (process.ExitCode != 0) {
            Console.WriteLine("Build failed:");
            Console.WriteLine(error);
          } else {
            Console.WriteLine("Build succeded.");
          }
        }
      } catch (Exception ex) {
        Console.WriteLine("Fail to run msbuild: " + ex.Message);
      }
    }

    private static Assembly LoadAssembly(string dllPath) {
      try {
#pragma warning disable S3885 // "Assembly.Load" should be used
        return Assembly.LoadFrom(dllPath);
#pragma warning restore S3885 // "Assembly.Load" should be used
      } catch (Exception e) {
        Console.WriteLine($"Fail to load DLL: {e.Message}");
        throw;
      }
    }

    private static void LoadXmlIfExists(string dllPath) {
      string xmlPath = Path.Combine(Path.GetDirectoryName(dllPath), $"{GsaGhName}.xml");

      if (!File.Exists(xmlPath)) {
        Console.WriteLine($"{GsaGhName}.xml not found");
        return;
      }

      try {
        GsaGhXml = new XmlDocument();
        GsaGhXml.Load(xmlPath);
        Console.WriteLine($"Loaded{GsaGhName}.xml");
      } catch (Exception e) {
        Console.WriteLine($"Fail to load XML: {e.Message}");
      }
    }

    private static void UpdateEnvironmentPath(string path) {
      const string name = "PATH";
      string current = Environment.GetEnvironmentVariable(name) ?? "";

      if (!current.Contains(path)) {
        Environment.SetEnvironmentVariable(name, current + ";" + path);
      }
    }
#pragma warning restore S2696 // Instance members should not write to "static" fields
  }
}
