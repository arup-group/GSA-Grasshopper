using System;
using System.IO;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using GsaGH.Helpers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GsaGH
{
  public class AddReferencePriority : GH_AssemblyPriority
  {
    public static string PluginPath;
    public static string InstallPath = Util.Gsa.InstallationFolderPath.GetPath;

    public override GH_LoadingInstruction PriorityLoad()
    {
      if (!TryFindPluginPath("GSA.gha"))
        return GH_LoadingInstruction.Abort;

      // ### Set system environment variables to allow user rights to read below dlls ###
      const string name = "PATH";
      string pathvar = System.Environment.GetEnvironmentVariable(name);
      var value = pathvar + ";" + InstallPath;
      var target = EnvironmentVariableTarget.Process;
      System.Environment.SetEnvironmentVariable(name, value, target);

      // ### Reference GSA API and SQLite dlls ###
      // set folder to latest GSA version.
      try
      {
        Assembly GsaAPI = Assembly.LoadFile(InstallPath + "\\GsaAPI.dll");

        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(InstallPath + "\\GsaAPI.dll");
        if (myFileVersionInfo.FileBuildPart < 60)
        {
          Exception exception = new Exception("Version " + GsaGH.GsaGHInfo.Vers + " of GSA-Grasshopper require GSA 10.1.60 installed. Please upgrade GSA.");
          GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA Version Error: Upgrade required", exception);
          Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
          PostHog.PluginLoaded(exception.Message);
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
              loadedPlugins = loadedPlugins + "-" + plugin.Name + System.Environment.NewLine;
            }
          }
        }
        string message = e.Message
            + System.Environment.NewLine + System.Environment.NewLine +
            "This may be due to clash with other referenced dll files by one of these plugins that's already been loaded: "
            + System.Environment.NewLine + loadedPlugins
            + System.Environment.NewLine + "You may try disable the above plugins to solve the issue."
            + System.Environment.NewLine + "The plugin cannot be loaded.";
        Exception exception = new Exception(message);
        GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI.dll loading", exception);
        Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
        PostHog.PluginLoaded(message);
        return GH_LoadingInstruction.Abort;
      }

      try
      {
        Assembly ass2 = Assembly.LoadFile(InstallPath + "\\System.Data.SQLite.dll");
      }
      catch (Exception e)
      {
        string loadedPlugins = "";
        ReadOnlyCollection<GH_AssemblyInfo> plugins = Grasshopper.Instances.ComponentServer.Libraries;
        foreach (GH_AssemblyInfo plugin in plugins)
        {
          if (!plugin.IsCoreLibrary)
          {
            if (!plugin.Name.StartsWith("Kangaroo"))
            {
              loadedPlugins = loadedPlugins + "-" + plugin.Name + System.Environment.NewLine;
            }
          }
        }
        string message = e.Message
            + System.Environment.NewLine + System.Environment.NewLine +
            "This may be due to clash with other referenced dll files by one of these plugins that's already been loaded: "
            + System.Environment.NewLine + loadedPlugins
            + System.Environment.NewLine + "You may try disable the above plugins to solve the issue."
            + System.Environment.NewLine + "The plugin cannot be loaded.";
        Exception exception = new Exception(message);
        GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: System.Data.SQLite.dll loading", exception);
        Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
        PostHog.PluginLoaded(message);
        return GH_LoadingInstruction.Abort;
      }

      // ### Queue up Main menu loader ###
      Grasshopper.Instances.CanvasCreated += UI.Menu.MenuLoad.OnStartup;

      // ### Create Ribbon Category name and icon ###
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');
      Grasshopper.Instances.ComponentServer.AddCategoryIcon("GSA", Properties.Resources.GSALogo);

      // ### Setup units ###
      Units.SetupUnitsDuringLoad();

      PostHog.PluginLoaded();

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
          PostHog.PluginLoaded(message);
          return false;
        }
      }
      PluginPath = Path.GetDirectoryName(path);
      return true;
    }
  }
  
  public class GsaGHInfo : GH_AssemblyInfo
  {
    internal static Guid GUID = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    internal const string Company = "Oasys";
    internal const string Copyright = "Copyright © Oasys 1985 - 2022";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Vers = "0.9.27";
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
    public override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GSALogo;
    public override string Description
    {
      get
      {
        //Return a short string describing the purpose of this GHA library.
        return "Official Oasys GSA Grasshopper Plugin" + System.Environment.NewLine
          + (isBeta ? Disclaimer : "")
        + System.Environment.NewLine + "A licensed version of GSA 10.1.60 or later installed in "
        + @"C:\Program Files\Oasys\GSA 10.1\ is required to use this plugin."
        + System.Environment.NewLine + "Contact oasys@arup.com to request a free trial version."
        + System.Environment.NewLine + TermsConditions
        + System.Environment.NewLine + Copyright;
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
    public string Icon_url
    {
      get
      {
        // TODO to be updated - not supported by yak currently
        return "https://raw.githubusercontent.com/arup-group/GSA-Grasshopper/master/GsaGH/Properties/Icons/icons/4x/GsaGhLogo%404x.png";
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
