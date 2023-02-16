using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Helpers;

namespace GsaGH
{
  public class AddReferencePriority : GH_AssemblyPriority
  {
    public static string PluginPath;
    public static string InstallPath = Helpers.GsaAPI.InstallationFolder.GetPath;
    
    public override GH_LoadingInstruction PriorityLoad()
    {
      if (!TryFindPluginPath("GSA.gha"))
        return GH_LoadingInstruction.Abort;

      // ### Set system environment variables to allow user rights to read below dlls ###
      const string name = "PATH";
      string pathvar = System.Environment.GetEnvironmentVariable(name);
      var value = InstallPath + ";" + pathvar;
      var target = EnvironmentVariableTarget.Process;
      System.Environment.SetEnvironmentVariable(name, value, target);

      // ### Reference GSA API dlls ###
      string gsaVersion = "";
      // check if GSA is installed
      if (!File.Exists(InstallPath + "\\GsaAPI.dll"))
      {
        Exception exception = new Exception("GsaGH requires GSA to be installed in " + InstallPath + ". Unable to find GsaAPI.dll. It looks like you haven't got GSA installed, or it may be installed in an unknown path. Please install or reinstall GSA in " + InstallPath + ", restart Rhino and load Grasshopper again.");
        GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI.dll loading", exception);
        Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
        PostHog.PluginLoaded(PluginInfo.Instance, exception.Message);
        return GH_LoadingInstruction.Abort;
      }
      try
      {
        // Try load GSA
        Assembly GsaAPI = Assembly.LoadFile(InstallPath + "\\GsaAPI.dll");
        FileVersionInfo gsaVers = FileVersionInfo.GetVersionInfo(InstallPath + "\\GsaAPI.dll");
        gsaVersion = gsaVers.FileMajorPart + "." + gsaVers.FileMinorPart + "." + gsaVers.FileBuildPart;
        if (gsaVers.FileBuildPart < 63)
        {
          Exception exception = new Exception("Version " + GsaGH.GsaGHInfo.Vers + " of GSA-Grasshopper requires GSA 10.1.63 installed. Please upgrade GSA.");
          GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA Version Error: Upgrade required", exception);
          Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
          PostHog.PluginLoaded(PluginInfo.Instance, exception.Message);
          return GH_LoadingInstruction.Abort;
        }

      }
      catch (Exception e)
      {
        // check other plugins?
        string loadedPlugins = "";
        ReadOnlyCollection<GH_AssemblyInfo> plugins = Grasshopper.Instances.ComponentServer.Libraries;
        foreach (GH_AssemblyInfo plugin in plugins)
        {
          if (!plugin.IsCoreLibrary)
          {
            if (!plugin.Name.StartsWith("Kangaroo"))
            {
              loadedPlugins = loadedPlugins + "-" + plugin.Name + Environment.NewLine;
            }
          }
        }
        string message = e.Message
            + Environment.NewLine + Environment.NewLine +
            "This may be due to clash with other referenced dll files by one of these plugins that's already been loaded: "
            + Environment.NewLine + loadedPlugins
            + Environment.NewLine + "You may try disable the above plugins to solve the issue."
            + Environment.NewLine + "The plugin cannot be loaded.";
        Exception exception = new Exception(message);
        GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI.dll loading", exception);
        Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
        PostHog.PluginLoaded(PluginInfo.Instance, message);
        return GH_LoadingInstruction.Abort;
      }

      // ### Queue up Main menu loader ###
      Grasshopper.Instances.CanvasCreated += Graphics.Menu.MenuLoad.OnStartup;

      // ### Create Ribbon Category name and icon ###
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');
      Grasshopper.Instances.ComponentServer.AddCategoryIcon("GSA", Properties.Resources.GSALogo);

      // ### Setup OasysGH and shared Units ###
      Utility.InitialiseMainMenuAndDefaultUnits();

      PostHog.PluginLoaded(GsaGH.PluginInfo.Instance, gsaVersion);

      return GH_LoadingInstruction.Proceed;
    }

    private bool TryFindPluginPath(string keyword)
    {
      // ### Search for plugin path ###

      // initially look in %appdata% folder where package manager will store the plugin
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", Rhino.RhinoApp.ExeVersion + ".0", GsaGHInfo.ProductName);

      if (!File.Exists(Path.Combine(path, keyword))) // if no plugin file is found there continue search
      {
        // search grasshopper libraries folder
        string sDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper",
          "Libraries");

        string[] files = Directory.GetFiles(sDir, keyword, SearchOption.AllDirectories);
        if (files.Length > 0)
          path = files[0].Replace(keyword, string.Empty);

        if (!File.Exists(Path.Combine(path, keyword))) // if no plugin file is found there continue search
        {
          // look in all the other Grasshopper assembly (plugin) folders
          foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders)
          {
            files = Directory.GetFiles(pluginFolder.Folder, keyword, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
              path = files[0].Replace(keyword, string.Empty);
              PluginPath = Path.GetDirectoryName(path);
              return true;
            }
          }
          string message =
            "Error loading the file " + keyword + " from any Grasshopper plugin folders - check if the file exist."
            + Environment.NewLine + "The plugin cannot be loaded."
            + Environment.NewLine + "Folders (including subfolder) that was searched:"
            + Environment.NewLine + sDir;
          foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders)
            message += Environment.NewLine + pluginFolder.Folder;

          Exception exception = new Exception(message);
          GH_LoadingException gH_LoadingException = new GH_LoadingException(GsaGHInfo.ProductName + ": " + keyword + " loading failed", exception);
          Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
          PostHog.PluginLoaded(GsaGH.PluginInfo.Instance, message);
          return false;
        }
      }
      PluginPath = Path.GetDirectoryName(path);
      return true;
    }
  }

  internal sealed class PluginInfo
  {
    private static readonly Lazy<OasysPluginInfo> lazy =
      new Lazy<OasysPluginInfo>(() => new OasysPluginInfo(
        GsaGHInfo.ProductName,
        GsaGHInfo.PluginName,
        GsaGHInfo.Vers,
        GsaGHInfo.isBeta,
        "phc_alOp3OccDM3D18xJTWDoW44Y1cJvbEScm5LJSX8qnhs"
        ));

    public static OasysPluginInfo Instance { get { return lazy.Value; } }

    private PluginInfo() { }
  }

  

  public static class SolverRequiredDll
  {
    private static bool _loaded = false;
    private static bool _canAnalyse = false;
    internal static string loadedFromPath { get; private set; }
    public static bool IsCorrectVersionLoaded()
    {
      if (!_loaded || !_canAnalyse)
      {
        ProcessModuleCollection dlls = Process.GetCurrentProcess().Modules;
        foreach (ProcessModule module in dlls)
        {
          if (module.ModuleName == "libiomp5md.dll")
          {
            _loaded = true;
            string gsaVersion = FileVersionInfo.GetVersionInfo(AddReferencePriority.InstallPath + "\\libiomp5md.dll").FileVersion;
            if (FileVersionInfo.GetVersionInfo(module.FileName).FileVersion == gsaVersion)
              _canAnalyse = true;
            else
            {
              _canAnalyse = false;
              loadedFromPath = module.FileName;
            }
            break;
          }
        }
      }
      if (!_loaded)
        return true; // GsaAPI.dll will load the correct verison
      return _canAnalyse;
    }
  }


  public class GsaGHInfo : GH_AssemblyInfo
  {
    internal static Guid GUID = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    internal const string Company = "Oasys";
    internal const string Copyright = "Copyright © Oasys 1985 - 2023";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Vers = "0.9.46";
    internal static bool isBeta = true;
    internal static string Disclaimer = PluginName + " is pre-release and under active development, including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using " + PluginName + " should not be relied upon without thorough and independent checking. ";
    internal const string ProductName = "GSA";
    internal const string PluginName = "GsaGH";
    internal const string TermsConditions = "Oasys terms and conditions apply. See https://www.oasys-software.com/terms-conditions for details. ";

    public override string Name
    {
      get
      {
        return ProductName;
      }
    }
    public override System.Drawing.Bitmap Icon => Properties.Resources.GSALogo;
    public override string Description
    {
      get
      {
        //Return a short string describing the purpose of this GHA library.
        return "Official Oasys GSA Grasshopper Plugin" + Environment.NewLine
          + (isBeta ? Disclaimer : "")
        + Environment.NewLine + "A licensed version of GSA 10.1.63 or later installed in "
        + @"C:\Program Files\Oasys\GSA 10.1\ is required to use this plugin."
        + Environment.NewLine + "Contact oasys@arup.com to request a free trial version."
        + Environment.NewLine + TermsConditions
        + Environment.NewLine + Copyright;
      }
    }
    public override Guid Id
    {
      get
      {
        return GUID;
      }
    }

    public override string AuthorName
    {
      get
      {
        //Return a string identifying you or your company.
        return Company;
      }
    }
    public override string AuthorContact
    {
      get
      {
        //Return a string representing your preferred contact details.
        return Contact;
      }
    }

    public override string Version
    {
      get
      {
        if (isBeta)
          return Vers + "-beta";
        else
          return Vers;
      }
    }
  }
}
