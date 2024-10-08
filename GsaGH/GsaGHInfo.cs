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
using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Properties;

using OasysGH;

using Rhino;

using PostHog = OasysGH.Helpers.PostHog;
using Utility = OasysGH.Utility;

namespace GsaGH {
  public class GsaGhInfo : GH_AssemblyInfo {
    public override string AuthorContact => Contact;
    public override string AuthorName => Company;
    public override string Description
      =>
        //Return a short string describing the purpose of this GHA library.
        "Official Oasys GSA Grasshopper Plugin" + Environment.NewLine + (isBeta ? disclaimer : string.Empty)
        + Environment.NewLine + $"A licensed version of {MajorVer}.{MinorVer}." + PatchVersion
        + " or later installed in "
        + $@"C:\Program Files\Oasys\GSA {MajorVer}.{MinorVer}\ is required to use this plugin." + Environment.NewLine
        + "Contact oasys@arup.com to request a free trial version." + Environment.NewLine + TermsConditions
        + Environment.NewLine + Copyright;
    public override Bitmap Icon => Resources.GSALogo;
    public override Guid Id => guid;
    public override string Name => ProductName;
    public override string Version => isBeta ? Vers + "-beta" : Vers;
    internal const string Company = "Oasys";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Copyright = "Copyright © Oasys 1985 - 2024";
    internal const string PluginName = "GsaGH";
    public const string ProductName = "GSA";
    public static int MajorVer = 10;
    public static int MinorVer = 2;
    public static int PatchVersion = 11;

    internal const string TermsConditions
      = "Oasys terms and conditions apply. See https://www.oasys-software.com/terms-conditions for details. ";
    internal const string Vers = "1.4.0";
    internal static string disclaimer = $"{PluginName} is pre-release and under active development, "
      + $"including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. "
      + $"Future versions may contain breaking changes. Any files, results, or other types of output information created using "
      + $"{PluginName} should not be relied upon without thorough and independent checking. "
      + $"{PluginName} {Vers} requires {ProductName} {MajorVer}.{MinorVer}.{PatchVersion} or higher installed.";
    internal static Guid guid = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    internal static bool isBeta = false;
  }

  public class AddReferencePriority : GH_AssemblyPriority {
    public static string PluginPath => pluginPath ??= TryFindPluginPath("GSA.gha");
    public static string InstallPath = InstallationFolder.GetPath;
    private static string pluginPath;

    private static string GsaApiDll => "\\GsaAPI.dll";
    private static string GsaApiDllLoading => "GSA: GsaAPI.dll loading";
    private static string Gsa_gha => "GSA.gha";
    private static string PluginName = "GSA-Grasshopper";
    private static string MicrosoftDataSqliteDll => @"\Microsoft.Data.Sqlite.dll";

    public override GH_LoadingInstruction PriorityLoad() {
      if (TryFindPluginPath(Gsa_gha) == string.Empty) {
        return GH_LoadingInstruction.Abort;
      }

      SetSysEnv();
      if (!GhLoadingInstruction(out GH_LoadingInstruction ghLoadingInstruction)) {
        return GH_LoadingInstruction.Abort;
      }

      if (!Load(out string gsaVersion, out GH_LoadingInstruction priorityLoad)) {
        return GH_LoadingInstruction.Abort;
      }

      // this is a temporary fix for TDA
      // needs more investigation!
      if (Assembly.GetEntryAssembly() != null && !Assembly.GetEntryAssembly().FullName.Contains("compute.geometry")) {
        Assembly.LoadFile(pluginPath + MicrosoftDataSqliteDll);
      }

      SetInstances();
      Utility.InitialiseMainMenuUnitsAndDependentPluginsCheck();
      RhinoApp.Closing += GsaComHelper.Dispose;
      PostHog.PluginLoaded(PluginInfo.Instance, gsaVersion);
      return GH_LoadingInstruction.Proceed;
    }

    private static void SetInstances() {
      Instances.CanvasCreated += MenuLoad.OnStartup;
      Instances.ComponentServer.AddCategorySymbolName(GsaGhInfo.ProductName, 'G');
      Instances.ComponentServer.AddCategoryIcon(GsaGhInfo.ProductName, Resources.GSALogo);
    }

    private static bool Load(out string gsaVersion, out GH_LoadingInstruction priorityLoad) {
      priorityLoad = GH_LoadingInstruction.Proceed;
      gsaVersion = "-1";
      try {
        Assembly.LoadFile(InstallPath + GsaApiDll);
        FileVersionInfo gsaVers = FileVersionInfo(out gsaVersion);
        if (!UpgradeVersionRequired(ref priorityLoad, gsaVers)) {
          return false;
        }
      } catch (Exception e) {
        ReadOnlyCollection<GH_AssemblyInfo> plugins = Instances.ComponentServer.Libraries;
        string loadedPlugins = LoadedPlugins(plugins);

        string message = NotLoadedPluginsErrorMessage(e, loadedPlugins);
        var exception = new Exception(message);
        var ghLoadingException = new GH_LoadingException(GsaApiDllLoading, exception);
        Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
        PostHog.PluginLoaded(PluginInfo.Instance, message);
        {
          priorityLoad = GH_LoadingInstruction.Abort;
          return false;
        }
      }

      return true;
    }

    private static string NotLoadedPluginsErrorMessage(Exception e, string loadedPlugins) {
      string message = e.Message + Environment.NewLine + Environment.NewLine
        + "This may be due to clash with other referenced dll files by one of these plugins that's already been loaded: "
        + Environment.NewLine + loadedPlugins + Environment.NewLine
        + "You may try disable the above plugins to solve the issue." + Environment.NewLine
        + "The plugin cannot be loaded.";
      return message;
    }

    private static string LoadedPlugins(ReadOnlyCollection<GH_AssemblyInfo> plugins) {
      string loadedPlugins = plugins.Where(plugin => !plugin.IsCoreLibrary)
       .Where(plugin => !plugin.Name.StartsWith("Kangaroo")).Aggregate(string.Empty,
          (current, plugin) => current + "-" + plugin.Name + Environment.NewLine);
      return loadedPlugins;
    }

    private static FileVersionInfo FileVersionInfo(out string gsaVersion) {
      var gsaVers = System.Diagnostics.FileVersionInfo.GetVersionInfo(InstallPath + GsaApiDll);
      gsaVersion = gsaVers.FileMajorPart + "." + gsaVers.FileMinorPart + "." + gsaVers.FileBuildPart;
      return gsaVers;
    }

    private static bool UpgradeVersionRequired(ref GH_LoadingInstruction priorityLoad, FileVersionInfo gsaVers) {
      if (gsaVers.FileBuildPart >= GsaGhInfo.PatchVersion) {
        return true;
      }

      Exception exceptionMsg = GetLoadingVersionException(out GH_LoadingException ghLoadingException);
      Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
      PostHog.PluginLoaded(PluginInfo.Instance, exceptionMsg.Message);
      {
        priorityLoad = GH_LoadingInstruction.Abort;
        return false;
      }
    }

    private static Exception GetLoadingVersionException(out GH_LoadingException ghLoadingException) {
      Exception exceptionMsg = VersionUpgradeRequiredException();
      ghLoadingException
        = new GH_LoadingException($"{GsaGhInfo.ProductName} Version Error: Upgrade required", exceptionMsg);
      return exceptionMsg;
    }

    private static Exception VersionUpgradeRequiredException() {
      return new Exception("Version " + GsaGhInfo.Vers
        + $" of {PluginName} requires {GsaGhInfo.ProductName} {GsaGhInfo.MajorVer}.{GsaGhInfo.MinorVer}."
        + GsaGhInfo.PatchVersion + $" installed. Please upgrade {GsaGhInfo.ProductName}.");
    }

    private static bool GhLoadingInstruction(out GH_LoadingInstruction ghLoadingInstruction) {
      if (!File.Exists(InstallPath + GsaApiDll)) {
        Exception exception = GsaNotInstalledException();
        var ghLoadingException = new GH_LoadingException(GsaApiDllLoading, exception);
        Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
        PostHog.PluginLoaded(PluginInfo.Instance, exception.Message);
        {
          ghLoadingInstruction = GH_LoadingInstruction.Abort;
          return false;
        }
      }

      ghLoadingInstruction = GH_LoadingInstruction.Proceed;
      return true;
    }

    private static Exception GsaNotInstalledException() {
      var exception = new Exception($"GsaGH requires {GsaGhInfo.ProductName} to be installed in " + InstallPath
        + $". Unable to find {GsaApiDll} It looks like you haven't got {GsaGhInfo.ProductName} installed, or it may be installed in an unknown path. Please install or reinstall {GsaGhInfo.ProductName} in "
        + InstallPath + ", restart Rhino and load Grasshopper again.");
      return exception;
    }

    private static void SetSysEnv() {
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      string value = InstallPath + ";" + pathvar;
      EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);
    }

    private static string TryFindPluginPath(string keyword) {
      // initially look in %appdata% folder where package manager will store the plugin
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", RhinoApp.ExeVersion + ".0",
        GsaGhInfo.ProductName);

      if (File.Exists(Path.Combine(path, keyword))) {
        return Path.GetDirectoryName(path);
      }

      if (FindDirectoryPath(keyword, path, out string directoryName)) {
        return directoryName;
      }

      string message = LoadingPluginPathFailedException(keyword, out GH_LoadingException ghLoadingException);
      Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
      PostHog.PluginLoaded(PluginInfo.Instance, message);
      return string.Empty;
    }

    private static string LoadingPluginPathFailedException(string keyword, out GH_LoadingException ghLoadingException) {
      string message = ErrorLoadingTheFileMsg(keyword);
      message = Folders.AssemblyFolders.Aggregate(message,
        (current, pluginFolder) => current + Environment.NewLine + pluginFolder.Folder);

      var exception = new Exception(message);
      ghLoadingException
        = new GH_LoadingException(GsaGhInfo.ProductName + ": " + keyword + " loading failed", exception);
      return message;
    }

    private static bool FindDirectoryPath(string keyword, string path, out string directoryName) {
      directoryName = null;
      string[] files = Directory.GetFiles(AppDataDirPath, keyword, SearchOption.AllDirectories);
      if (files.Length > 0) {
        path = files[0].Replace(keyword, string.Empty);
      }

      if (File.Exists(Path.Combine(path, keyword))) {
        {
          directoryName = Path.GetDirectoryName(path);
          return true;
        }
      }

      foreach (GH_AssemblyFolderInfo pluginFolder in Folders.AssemblyFolders) {
        files = Directory.GetFiles(pluginFolder.Folder, keyword, SearchOption.AllDirectories);
        if (files.Length <= 0) {
          continue;
        }

        path = files[0].Replace(keyword, string.Empty);
        {
          directoryName = Path.GetDirectoryName(path);
          return true;
        }
      }

      return false;
    }

    private static string ErrorLoadingTheFileMsg(string keyword) {
      string message = "Error loading the file " + keyword
        + " from any Grasshopper plugin folders - check if the file exist." + Environment.NewLine
        + "The plugin cannot be loaded." + Environment.NewLine + "Folders (including subfolder) that was searched:"
        + Environment.NewLine + AppDataDirPath;
      return message;
    }

    private static string AppDataDirPath {
      get {
        string sDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper",
          "Libraries");
        return sDir;
      }
    }
  }

  internal sealed class PluginInfo {
    public static OasysPluginInfo Instance => lazy.Value;
    private static readonly Lazy<OasysPluginInfo> lazy = new Lazy<OasysPluginInfo>(()
      => new OasysPluginInfo(GsaGhInfo.ProductName, GsaGhInfo.PluginName, GsaGhInfo.Vers,
        GsaGhInfo.isBeta, "phc_QjmqOoe8GqTMi3u88ynRR3WWvrJA9zAaqcQS1FDVnJD"));
  }
}
