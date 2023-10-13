using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Grasshopper;
using Grasshopper.Kernel;
using GsaGH.Graphics.Menu;
using GsaGH.Helpers.GsaApi;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Helpers;
using Rhino;
using Utility = OasysGH.Utility;

namespace GsaGH {
  public class GsaGhInfo : GH_AssemblyInfo {
    public override string AuthorContact => Contact;
    public override string AuthorName => Company;
    public override string Description
      =>
        //Return a short string describing the purpose of this GHA library.
        "Official Oasys GSA Grasshopper Plugin" + Environment.NewLine + (isBeta ? disclaimer : string.Empty)
        + Environment.NewLine + "A licensed version of GSA 10.2." + MinGsaVersion + " or later installed in "
        + @"C:\Program Files\Oasys\GSA 10.1\ is required to use this plugin." + Environment.NewLine
        + "Contact oasys@arup.com to request a free trial version." + Environment.NewLine
        + TermsConditions + Environment.NewLine + Copyright;
    public override Bitmap Icon => Resources.GSALogo;
    public override Guid Id => guid;
    public override string Name => ProductName;
    public override string Version => isBeta ? Vers + "-beta" : Vers;
    internal const string Company = "Oasys";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Copyright = "Copyright © Oasys 1985 - 2023";
    internal const string PluginName = "GsaGH";
    internal const string ProductName = "GSA";
    internal static int MinGsaVersion = 0;
    internal const string TermsConditions
      = "Oasys terms and conditions apply. See https://www.oasys-software.com/terms-conditions for details. ";
    internal const string Vers = "0.9.99";
    internal static string disclaimer = $"{PluginName} is pre-release and under active development, " +
      $"including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. " +
      $"Future versions may contain breaking changes. Any files, results, or other types of output information created using " +
      $"{PluginName} should not be relied upon without thorough and independent checking. " +
      $"{PluginName} {Vers} requires {ProductName} 10.2.{MinGsaVersion} or higher installed.";
    internal static Guid guid = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    internal static bool isBeta = true;
  }

  public class AddReferencePriority : GH_AssemblyPriority {
    public static string PluginPath => pluginPath ?? (pluginPath = TryFindPluginPath("GSA.gha"));
    public static string InstallPath = InstallationFolder.GetPath;
    private static string pluginPath;

    public override GH_LoadingInstruction PriorityLoad() {
      if (TryFindPluginPath("GSA.gha") == string.Empty) {
        return GH_LoadingInstruction.Abort;
      }

      // ### Set system environment variables to allow user rights to read below dlls ###
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      string value = InstallPath + ";" + pathvar;
      EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);

      // ### Reference GSA API dlls ###
      string gsaVersion = string.Empty;
      if (!File.Exists(InstallPath + "\\GsaAPI.dll")) {
        var exception = new Exception("GsaGH requires GSA to be installed in " + InstallPath
          + ". Unable to find GsaAPI.dll. It looks like you haven't got GSA installed, or it may be installed in an unknown path. Please install or reinstall GSA in "
          + InstallPath + ", restart Rhino and load Grasshopper again.");
        var ghLoadingException = new GH_LoadingException("GSA: GsaAPI.dll loading", exception);
        Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
        PostHog.PluginLoaded(PluginInfo.Instance, exception.Message);
        return GH_LoadingInstruction.Abort;
      }

      try {
        Assembly.LoadFile(InstallPath + "\\GsaAPI.dll");
        var gsaVers = FileVersionInfo.GetVersionInfo(InstallPath + "\\GsaAPI.dll");
        gsaVersion = gsaVers.FileMajorPart + "." + gsaVers.FileMinorPart + "."
          + gsaVers.FileBuildPart;
        if (gsaVers.FileBuildPart < GsaGhInfo.MinGsaVersion) {
          var exception = new Exception("Version " + GsaGhInfo.Vers
            + " of GSA-Grasshopper requires GSA 10.2." + GsaGhInfo.MinGsaVersion + " installed. Please upgrade GSA.");
          var ghLoadingException
            = new GH_LoadingException("GSA Version Error: Upgrade required", exception);
          Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
          PostHog.PluginLoaded(PluginInfo.Instance, exception.Message);
          return GH_LoadingInstruction.Abort;
        }
      } catch (Exception e) {
        ReadOnlyCollection<GH_AssemblyInfo> plugins = Instances.ComponentServer.Libraries;
        string loadedPlugins = plugins.Where(plugin => !plugin.IsCoreLibrary)
         .Where(plugin => !plugin.Name.StartsWith("Kangaroo")).Aggregate(string.Empty,
            (current, plugin) => current + "-" + plugin.Name + Environment.NewLine);

        string message = e.Message + Environment.NewLine + Environment.NewLine
          + "This may be due to clash with other referenced dll files by one of these plugins that's already been loaded: "
          + Environment.NewLine + loadedPlugins + Environment.NewLine
          + "You may try disable the above plugins to solve the issue." + Environment.NewLine
          + "The plugin cannot be loaded.";
        var exception = new Exception(message);
        var ghLoadingException = new GH_LoadingException("GSA: GsaAPI.dll loading", exception);
        Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
        PostHog.PluginLoaded(PluginInfo.Instance, message);
        return GH_LoadingInstruction.Abort;
      }

      // this is a temporary fix for TDA
      // needs more investigation!
      if (Assembly.GetEntryAssembly() != null && !Assembly.GetEntryAssembly().FullName.Contains("compute.geometry")) {
        Assembly.LoadFile(pluginPath + @"\Microsoft.Data.Sqlite.dll");
      }

      Instances.CanvasCreated += MenuLoad.OnStartup;
      Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');
      Instances.ComponentServer.AddCategoryIcon("GSA", Resources.GSALogo);

      Utility.InitialiseMainMenuUnitsAndDependentPluginsCheck();

      PostHog.PluginLoaded(PluginInfo.Instance, gsaVersion);

      return GH_LoadingInstruction.Proceed;
    }

    private static string TryFindPluginPath(string keyword) {
      // initially look in %appdata% folder where package manager will store the plugin
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", RhinoApp.ExeVersion + ".0",
        GsaGhInfo.ProductName);

      if (File.Exists(Path.Combine(path, keyword))) {
        return Path.GetDirectoryName(path);
      }

      string sDir
        = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
          "Grasshopper", "Libraries");

      string[] files = Directory.GetFiles(sDir, keyword, SearchOption.AllDirectories);
      if (files.Length > 0) {
        path = files[0].Replace(keyword, string.Empty);
      }

      if (File.Exists(Path.Combine(path, keyword))) {
        return Path.GetDirectoryName(path);
      }

      foreach (GH_AssemblyFolderInfo pluginFolder in Folders.AssemblyFolders) {
        files = Directory.GetFiles(pluginFolder.Folder, keyword, SearchOption.AllDirectories);
        if (files.Length <= 0) {
          continue;
        }

        path = files[0].Replace(keyword, string.Empty);
        return Path.GetDirectoryName(path);
      }

      string message = "Error loading the file " + keyword
        + " from any Grasshopper plugin folders - check if the file exist." + Environment.NewLine
        + "The plugin cannot be loaded." + Environment.NewLine
        + "Folders (including subfolder) that was searched:" + Environment.NewLine + sDir;
      message = Folders.AssemblyFolders.Aggregate(message,
        (current, pluginFolder) => current + Environment.NewLine + pluginFolder.Folder);

      var exception = new Exception(message);
      var ghLoadingException
        = new GH_LoadingException(GsaGhInfo.ProductName + ": " + keyword + " loading failed",
          exception);
      Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
      PostHog.PluginLoaded(PluginInfo.Instance, message);
      return string.Empty;
    }
  }

  internal sealed class PluginInfo {
    public static OasysPluginInfo Instance => lazy.Value;
    private static readonly Lazy<OasysPluginInfo> lazy = new Lazy<OasysPluginInfo>(()
      => new OasysPluginInfo(GsaGhInfo.ProductName, GsaGhInfo.PluginName, GsaGhInfo.Vers,
        GsaGhInfo.isBeta, "phc_QjmqOoe8GqTMi3u88ynRR3WWvrJA9zAaqcQS1FDVnJD"));

    private PluginInfo() { }
  }
}
