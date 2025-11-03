using System;
using System.IO;
using System.Reflection;

using Grasshopper.Plugin;

using OasysGH.Units;

using Rhino;
using Rhino.Runtime.InProcess;

using RhinoInside;

using Xunit;

namespace GsaGHTests {
  public class GrasshopperFixture : IDisposable {
    public RhinoCore Core {
      get {
        if (null == _core) {
          InitializeCore();
        }

        return _core as RhinoCore;
      }
    }
    public GH_RhinoScriptInterface GhPlugin {
      get {
        if (null == _ghPlugin) {
          InitializeGrasshopperPlugin();
        }

        return _ghPlugin as GH_RhinoScriptInterface;
      }
    }
    public static string InstallPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "GSA 10.2");

    private object Doc { get; set; }
    private object DocIo { get; set; }
    private const string LinkFileName = "GsaGhTests.ghlink";
    private static readonly string linkFilePath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper",
      "Libraries");
    private object _core = null;
    private object _ghPlugin = null;
    private bool _isDisposed;

    static GrasshopperFixture() {
      // This MUST be included in a static constructor to ensure that no Rhino DLLs
      // are loaded before the resolver is set up. Avoid creating other static functions
      // and members which may reference Rhino assemblies, as that may cause those
      // assemblies to be loaded before this is called.
      Resolver.Initialize();
    }

    public GrasshopperFixture() {
      AddPluginToGh();

      LoadRefs();
      Assembly.LoadFile(InstallPath + "\\GsaAPI.dll");

      InitializeCore();

      OasysGH.Units.Utility.SetupUnitsDuringLoad();
    }

    public void AddPluginToGh() {
      Directory.CreateDirectory(linkFilePath);
      StreamWriter writer = File.CreateText(Path.Combine(linkFilePath, LinkFileName));
      writer.Write(Environment.CurrentDirectory);
      writer.Close();
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
      File.Delete(Path.Combine(linkFilePath, LinkFileName));
    }

    public void LoadRefs() {
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      string value = pathvar + ";" + InstallPath + "\\";
      EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);
    }

    protected virtual void Dispose(bool disposing) {
      if (_isDisposed) {
        return;
      }

      if (disposing) {
        Doc = null;
        DocIo = null;
        GhPlugin.CloseAllDocuments();
        _ghPlugin = null;
        Core.Dispose();
      }

      _isDisposed = true;
    }

    private void InitializeCore() {
      _core = new RhinoCore();
    }

    private void InitializeGrasshopperPlugin() {
      if (null == _core) {
        InitializeCore();
      }

      // we do this in a seperate function to absolutely ensure that the core is initialized before we load the GH plugin,
      // which will happen automatically when we enter the function containing GH references
      InitializeGrasshopperPlugin2();
    }

    private void InitializeGrasshopperPlugin2() {
      _ghPlugin = RhinoApp.GetPlugInObject("Grasshopper");
      var ghp = _ghPlugin as GH_RhinoScriptInterface;
      ghp?.RunHeadless();
    }
  }

  [CollectionDefinition("GrasshopperFixture collection")]
  public class GrasshopperCollection : ICollectionFixture<GrasshopperFixture> {
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
  }
}
