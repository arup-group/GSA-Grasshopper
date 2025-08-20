using System;
using System.IO;
using System.Reflection;

using Grasshopper.Plugin;

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
      Resolver.Initialize();
    }

    public GrasshopperFixture(string fileName) {
      _linkFileName = $"{fileName}Tests.ghlink";
      AddPluginToGh();
      LoadRefs();
      Assembly.LoadFile(Path.Combine(GrasshopperInstallPath, "GsaAPI.dll"));
      InitializeCore();
      Utility.SetupUnitsDuringLoad();
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
