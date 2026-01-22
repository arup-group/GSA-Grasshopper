using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

namespace DocsGeneratorCLI {
  public static class GsaGhDll {
    private static string PluginPath;
    private static GrasshopperFixture _grasshopperFixture;
    public static XmlDocument GsaGhXml { get; private set; }

    public static Assembly LoadGhAndPassAssembly(string projectName) {
      Console.WriteLine($"==> [{projectName}] Start loading...");

      Console.WriteLine("Loading Rhino/Grasshopper fixture");
      _grasshopperFixture = new GrasshopperFixture(projectName);

      PluginPath = GetPluginDirectory();
      string dllPath = TryFindDll(projectName) ?? TryBuildAndFindDll(projectName);

      if (dllPath == null) {
        Console.WriteLine($"Couldn't find {projectName}.dll");
        return null;
      }

      Assembly GsaGH = LoadAssembly(dllPath);
      LoadXmlIfExists(dllPath, projectName);

      Console.WriteLine($"Finished loading {projectName}");
      return GsaGH;
    }

    public static void Cleanup() {
      _grasshopperFixture?.Dispose();
      _grasshopperFixture = null;
    }

    // === Submethods ===

    private static string GetPluginDirectory() {
      return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
    }

    private static string TryFindDll(string gsaGhName) {
      string[] searchPaths = {
        Path.Combine(PluginPath, $"{gsaGhName}.dll"),
        Path.Combine(PluginPath, "..", gsaGhName, "bin", "Debug", $"{gsaGhName}.dll"),
        Path.Combine(PluginPath, "..", "..", gsaGhName, "bin", "Debug", $"{gsaGhName}.dll"),
      };

      foreach (string path in searchPaths) {
        Console.WriteLine($"Looking at path: {path}");
        string full = Path.GetFullPath(path);
        if (!File.Exists(full)) {
          Console.WriteLine($"Couldn't find {gsaGhName}.dll");
          continue;
        }

        Console.WriteLine($"Found DLL: {full}");
        return full;
      }

      return null;
    }

    private static string TryBuildAndFindDll(string GsaGhName) {
      string csprojPath = FindCsproj(GsaGhName);
      if (csprojPath == null) {
        Console.WriteLine($"Didn't find {GsaGhName}.csproj");
        return null;
      }

      Console.WriteLine($"Building project: {csprojPath}");
      BuildProject(csprojPath);

      string dllPath = Path.Combine(Path.GetDirectoryName(csprojPath), "bin", "Debug", $"{GsaGhName}.dll");
      return File.Exists(dllPath) ? dllPath : null;
    }

    private static string FindCsproj(string GsaGhName) {
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
        FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe"),
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
            Console.WriteLine("Build succeeded.");
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

    private static void LoadXmlIfExists(string dllPath, string GsaGhName) {
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
  }
}
