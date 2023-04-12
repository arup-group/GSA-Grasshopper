using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace GsaGHTests {
  public class GrasshopperFixture : IDisposable {
    public static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "GSA 10.1");

    private object _core = null;
    private object _ghPlugin = null;
    private object DocIo { get; set; }
    private object Doc { get; set; }
    private bool _isDisposed;
    private static readonly string s_linkFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper", "Libraries");
    private const string LinkFileName = "GsaGhTests.ghlink";

    static GrasshopperFixture() {
      // This MUST be included in a static constructor to ensure that no Rhino DLLs
      // are loaded before the resolver is set up. Avoid creating other static functions
      // and members which may reference Rhino assemblies, as that may cause those
      // assemblies to be loaded before this is called.
      RhinoInside.Resolver.Initialize();
    }


    public GrasshopperFixture() {
      AddPluginToGh();

      LoadRefs();
      Assembly.LoadFile(InstallPath + "\\GsaAPI.dll");

      InitializeCore();

      OasysGH.Units.Utility.SetupUnitsDuringLoad(true);
    }

    public void LoadRefs() {
      const string name = "PATH";
      string pathvar = System.Environment.GetEnvironmentVariable(name);
      string value = pathvar + ";" + InstallPath + "\\";
      EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
      System.Environment.SetEnvironmentVariable(name, value, target);
    }

    public void AddPluginToGh() {
      Directory.CreateDirectory(s_linkFilePath);
      StreamWriter writer = File.CreateText(Path.Combine(s_linkFilePath, LinkFileName));
      writer.Write(Environment.CurrentDirectory);
      writer.Close();
    }

    protected virtual void Dispose(bool disposing) {
      if (_isDisposed)
        return;
      if (disposing) {
        Doc = null;
        DocIo = null;
        GhPlugin.CloseAllDocuments();
        _ghPlugin = null;
        Core.Dispose();
      }

      _isDisposed = true;
    }

    public void Dispose() {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
      File.Delete(Path.Combine(s_linkFilePath, LinkFileName));
    }

    public Rhino.Runtime.InProcess.RhinoCore Core {
      get {
        if (null == _core)
          InitializeCore();
        return _core as Rhino.Runtime.InProcess.RhinoCore;
      }
    }

    public Grasshopper.Plugin.GH_RhinoScriptInterface GhPlugin {
      get {
        if (null == _ghPlugin)
          InitializeGrasshopperPlugin();
        return _ghPlugin as Grasshopper.Plugin.GH_RhinoScriptInterface;
      }
    }

    private void InitializeCore() => _core = new Rhino.Runtime.InProcess.RhinoCore();

    private void InitializeGrasshopperPlugin() {
      if (null == _core)
        InitializeCore();
      // we do this in a seperate function to absolutely ensure that the core is initialized before we load the GH plugin,
      // which will happen automatically when we enter the function containing GH references
      InitializeGrasshopperPlugin2();
    }

    private void InitializeGrasshopperPlugin2() {
      _ghPlugin = Rhino.RhinoApp.GetPlugInObject("Grasshopper");
      var ghp = _ghPlugin as Grasshopper.Plugin.GH_RhinoScriptInterface;
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
