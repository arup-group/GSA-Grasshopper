using System;
using System.IO;
using System.Reflection;
using GsaGH.Parameters;
using Xunit;

namespace IntegrationTests
{
  public class GrasshopperFixture : IDisposable
  {
    public static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "GSA 10.1");

    private object _Core = null;
    private object _GHPlugin = null;
    private object _DocIO { get; set; }
    private object _Doc { get; set; }
    private bool _isDisposed;
    private static string _linkFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper", "Libraries");
    private static string _linkFileName = "IntegrationTests.ghlink";
    static GrasshopperFixture()
    {
      // This MUST be included in a static constructor to ensure that no Rhino DLLs
      // are loaded before the resolver is set up. Avoid creating other static functions
      // and members which may reference Rhino assemblies, as that may cause those
      // assemblies to be loaded before this is called.
      RhinoInside.Resolver.Initialize();
    }
    public GrasshopperFixture()
    {
      AddPluginToGH();

      LoadRefs();
      Assembly GsaAPI = Assembly.LoadFile(InstallPath + "\\GsaAPI.dll");
      Assembly SQLite = Assembly.LoadFile(InstallPath + "\\System.Data.SQLite.dll");

      InitializeCore();

      // setup headless units
      OasysGH.Units.Utility.SetupUnitsDuringLoad(true);
    }

    public void LoadRefs()
    {
      const string name = "PATH";
      string pathvar = System.Environment.GetEnvironmentVariable(name);
      var value = pathvar + ";" + InstallPath + "\\";
      var target = EnvironmentVariableTarget.Process;
      System.Environment.SetEnvironmentVariable(name, value, target);
    }

    public void AddPluginToGH()
    {
      Directory.CreateDirectory(_linkFilePath);
      StreamWriter writer = File.CreateText(Path.Combine(_linkFilePath, _linkFileName));
      writer.Write(Environment.CurrentDirectory);
      writer.Close();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_isDisposed) return;
      if (disposing)
      {
        _Doc = null;
        _DocIO = null;
        GHPlugin.CloseAllDocuments();
        _GHPlugin = null;
        Core.Dispose();
      }

      // TODO: free unmanaged resources (unmanaged objects) and override finalizer
      // TODO: set large fields to null
      _isDisposed = true;
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~GrasshopperFixture()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
      File.Delete(Path.Combine(_linkFilePath, _linkFileName));
    }

    public Rhino.Runtime.InProcess.RhinoCore Core
    {
      get
      {
        if (null == _Core) InitializeCore();
        return _Core as Rhino.Runtime.InProcess.RhinoCore;
      }
    }
    public Grasshopper.Plugin.GH_RhinoScriptInterface GHPlugin
    {
      get
      {
        if (null == _GHPlugin) InitializeGrasshopperPlugin();
        return _GHPlugin as Grasshopper.Plugin.GH_RhinoScriptInterface;
      }
    }

    public Grasshopper.Kernel.GH_DocumentIO DocIO
    {
      get
      {
        if (null == _DocIO) InitializeDocIO();
        return _DocIO as Grasshopper.Kernel.GH_DocumentIO;
      }
    }

    void InitializeCore()
    {
      _Core = new Rhino.Runtime.InProcess.RhinoCore();
    }

    void InitializeGrasshopperPlugin()
    {
      if (null == _Core) InitializeCore();
      // we do this in a seperate function to absolutely ensure that the core is initialized before we load the GH plugin,
      // which will happen automatically when we enter the function containing GH references
      InitializeGrasshopperPlugin2();
    }

    void InitializeGrasshopperPlugin2()
    {
      _GHPlugin = Rhino.RhinoApp.GetPlugInObject("Grasshopper");
      var ghp = _GHPlugin as Grasshopper.Plugin.GH_RhinoScriptInterface;
      ghp.RunHeadless();
    }

    void InitializeDocIO()
    {
      // we do this in a seperate function to absolutely ensure that the core is initialized before we load the GH plugin,
      // which will happen automatically when we enter the function containing GH references
      if (null == _GHPlugin) InitializeGrasshopperPlugin();
      InitializeDocIO2();
    }

    void InitializeDocIO2()
    {
      var docIO = new Grasshopper.Kernel.GH_DocumentIO();
      _DocIO = docIO;
    }
  }

  [CollectionDefinition("GrasshopperFixture collection")]
  public class GrasshopperCollection : ICollectionFixture<GrasshopperFixture>
  {
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
  }
}
