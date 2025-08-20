using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

namespace DocsGeneratorCLI {
  public class GsaGhDll {
    public static Assembly GsaGH;
    public static string PluginPath;
    public static XmlDocument GsaGhXml;
    public const string GsaGhName = "GsaGH";

    public Assembly Load() {
      Console.WriteLine($"==> [{GsaGhName}] Start loading...");

      Console.WriteLine("Loading Rhino/Grasshopper fixture");
      var grasshopper = new GrasshopperFixture("GsaGh");

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

    private string GetPluginDirectory() {
      return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
    }

    private string TryFindDll() {
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

    private string TryBuildAndFindDll() {
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

    private string FindCsproj() {
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

    private void BuildProject(string csprojPath) {
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

    private Assembly LoadAssembly(string dllPath) {
      try {
        return Assembly.LoadFrom(dllPath);
      } catch (Exception e) {
        Console.WriteLine($"Fail to load DLL: {e.Message}");
        throw;
      }
    }

    private void LoadXmlIfExists(string dllPath) {
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

    private void UpdateEnvironmentPath(string path) {
      const string name = "PATH";
      string current = Environment.GetEnvironmentVariable(name) ?? "";

      if (!current.Contains(path)) {
        Environment.SetEnvironmentVariable(name, current + ";" + path);
      }
    }

  }
}
