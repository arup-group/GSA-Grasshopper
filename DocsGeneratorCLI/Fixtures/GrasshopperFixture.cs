using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Grasshopper.Plugin;

using Microsoft.Win32;

using OasysGH.Units;

using Rhino;
using Rhino.Runtime.InProcess;

using RhinoInside;

namespace DocsGeneratorCLI {
  public class GrasshopperFixture : IDisposable {
    private RhinoCore _core;
    private GH_RhinoScriptInterface _ghPlugin;
    private bool _isDisposed;
    private readonly string _linkFileName;

    static GrasshopperFixture() {
      InitRhinoLibrary();
    }

    public GrasshopperFixture(string fileName) {
      InitRhinoLibrary();
      _linkFileName = $"{fileName}Tests.ghlink";
      AddPluginToGh();
      LoadRefs();
      Assembly.LoadFile(Path.Combine(GrasshopperInstallPath, "GsaAPI.dll"));
      InitializeCore();
      Utility.SetupUnitsDuringLoad();
    }

    private static void InitRhinoLibrary() {
      string rhinoSystemPath = GetRhinoSystemPath();
      string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";

      if (!currentPath.Split(';').Contains(rhinoSystemPath, StringComparer.OrdinalIgnoreCase)) {
        Environment.SetEnvironmentVariable("PATH", currentPath + ";" + rhinoSystemPath,
          EnvironmentVariableTarget.Process);
      }

      Resolver.Initialize();
    }

    private static string GetRhinoSystemPath() {
      string[] versions = {
        "7.0",
        "8.0",
      };

      foreach (string version in versions) {
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\McNeel\Rhinoceros\{version}\Install")) {
          if (key == null) {
            continue;
          }

          object installPath = key.GetValue("InstallPath");
          if (!(installPath is string path)) {
            continue;
          }

          string systemFolder = Path.Combine(path, "System");
          if (Directory.Exists(systemFolder)) {
            return systemFolder;
          }
        }
      }

      return null;
    }

    public void AddPluginToGh() {
      string linkFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Grasshopper", "Libraries");
      Directory.CreateDirectory(linkFilePath);

      string fullPath = Path.Combine(linkFilePath, _linkFileName);

      using (StreamWriter writer = File.CreateText(fullPath)) {
        writer.Write(Environment.CurrentDirectory);
      }
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);

      string linkFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Grasshopper", "Libraries");
      File.Delete(Path.Combine(linkFilePath, _linkFileName));
    }

    private void LoadRefs() {
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name) ?? "";
      string value = pathvar + ";" + GrasshopperInstallPath + "\\";
      Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
      Console.WriteLine("PATH ENV after loadrefs :");
      Console.WriteLine(Environment.GetEnvironmentVariable("PATH"));
    }

    private void Dispose(bool disposing) {
      if (_isDisposed) {
        return;
      }

      if (disposing) {
        _ghPlugin?.CloseAllDocuments();
        _ghPlugin = null;
        _core?.Dispose();
        _core = null;
      }

      _isDisposed = true;
    }

    private void InitializeCore() {
      _core = new RhinoCore();
    }

    private void InitializeGrasshopperPlugin() {
      if (_core == null) {
        InitializeCore();
      }

      _ghPlugin = RhinoApp.GetPlugInObject("Grasshopper") as GH_RhinoScriptInterface;
      _ghPlugin?.RunHeadless();
    }

    private const string GrasshopperInstallPath = @"C:\Program Files\Oasys\GSA 10.2";
  }
}
