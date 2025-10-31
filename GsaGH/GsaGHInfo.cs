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
using GsaGH.Helpers.GsaCOM;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Helpers;

using Rhino;

using static GsaGH.GsaGhInfo;

using Utility = OasysGH.Utility;

namespace GsaGH {
  public interface IAnalytics {
    void PluginLoaded(OasysPluginInfo pluginInfo, string error = "");
  }

  public class PostHoGAnalytics : IAnalytics {
    public void PluginLoaded(OasysPluginInfo pluginInfo, string error = "") {
      PostHog.PluginLoaded(pluginInfo, error);
    }
  }

  public class GsaGhInfo : GH_AssemblyInfo {
    internal readonly struct GsaVersionRequired {
      internal static readonly int MajorVersion = 10;
      internal static readonly int MinorVersion = 2;
      internal static readonly int BuildVersion = 11;
      internal static readonly string MainVersion = $"{MajorVersion}.{MinorVersion}";
      internal static readonly string FullVersion = $"{MajorVersion}.{MinorVersion}.{BuildVersion}";
    }

    internal const string Company = "Oasys";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Copyright = "Copyright © Oasys 1985 - 2024";
    public const string PluginName = "GsaGH";
    public const string ProductName = "GSA";
    internal const string TermsConditions
      = "Oasys terms and conditions apply. See https://www.oasys-software.com/terms-conditions for details. ";
    internal const string GrasshopperVersion = "1.5.1";

    internal static string disclaimer = $"{PluginName} is pre-release and under active development, "
      + "including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. "
      + "Future versions may contain breaking changes. Any files, results, or other types of output information created using "
      + $"{PluginName} should not be relied upon without thorough and independent checking. "
      + $"{PluginName} {GrasshopperVersion} requires {ProductName} {GsaVersionRequired.FullVersion} or higher installed.";
    internal static Guid guid = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    internal static bool isBeta = false;
    public override string AuthorContact => Contact;
    public override string AuthorName => Company;
    public override string Description
      =>
        //Return a short string describing the purpose of this GHA library.
        $"Official Oasys {ProductName} Grasshopper Plugin {Environment.NewLine} {(isBeta ? disclaimer : string.Empty)}"
        + Environment.NewLine
        + $"A licensed version of {ProductName} {GsaVersionRequired.FullVersion} or later installed in "
        + $@"C:\Program Files\Oasys\{ProductName} {GsaVersionRequired.MainVersion}\ is required to use this plugin."
        + Environment.NewLine + "Contact oasys@arup.com to request a free trial version." + Environment.NewLine
        + TermsConditions + Environment.NewLine + Copyright;
    public override Bitmap Icon => Resources.GSALogo;
    public override Guid Id => guid;
    public override string Name => ProductName;
    public override string Version => isBeta ? GrasshopperVersion + "-beta" : GrasshopperVersion;
    public static bool IsBeta => isBeta;
  }

  public class AddReferencePriority : GH_AssemblyPriority {
    private const string _GsaApiDllFileName = "GsaAPI.dll";
    private const string _LoadingGsaApiMessage = "GSA: GsaAPI.dll loading";
    private const string _GsaUpgradeRequiredMessage = "GSA Version Error: Upgrade required";
    private static string pluginPath;
    public static string PluginPath => pluginPath ??= TryFindPluginPath("GSA.gha");
    public static string InstallPath => InstallationFolder.GetPath;
    private static string GsaApiDllFullPath => Path.Combine(InstallPath, _GsaApiDllFileName);
    private static string MicrosoftDataSqliteDll => @"\Microsoft.Data.Sqlite.dll";
    // MESSAGES
    private static string GsaghRequiresGsaToBeInstalledMessage
      => $"{PluginName} requires {ProductName} to be installed in {InstallPath}"
        + $". Unable to find {_GsaApiDllFileName}. It looks like you haven't got {ProductName} installed, or it may be installed in an unknown path. Please install or reinstall {ProductName} in "
        + InstallPath + ", restart Rhino and load Grasshopper again.";
    private static string GsaVersionMustBeUpdatedMessage
      => "Version " + GrasshopperVersion
        + $" of GSA-Grasshopper requires {GsaVersionRequired.FullVersion} installed. Please upgrade {ProductName}.";
    private string _GsaApiDllVersion;
    private readonly IAnalytics _analytics;
    private readonly bool _underTest;

    public AddReferencePriority() : this(new PostHoGAnalytics(), underTest: false) { }

    internal AddReferencePriority(IAnalytics analytics, bool underTest) {
      _analytics = analytics;
      _underTest = underTest;
    }

    public override GH_LoadingInstruction PriorityLoad() {
      if (string.IsNullOrEmpty(TryFindPluginPath("GSA.gha"))) {
        return GH_LoadingInstruction.Abort;
      }

      SetSystemEnvironmentVariable();
      if (!CheckGsaApiExists()) {
        return GH_LoadingInstruction.Abort;
      }

      if (!TryToLoadGsaDll()) {
        return GH_LoadingInstruction.Abort;
      }

      // this is a temporary fix for TDA
      // needs more investigation!
      if (!_underTest) {
        if (Assembly.GetEntryAssembly() != null && !Assembly.GetEntryAssembly().FullName.Contains("compute.geometry")) {
          Assembly.LoadFile(PluginPath + MicrosoftDataSqliteDll);
        }
      }

      SetInstances();
      SetPlugins();
      return GH_LoadingInstruction.Proceed;
    }

    private bool TryToLoadGsaDll() {
      try {
        Assembly.LoadFile(GsaApiDllFullPath);
        SetGsaApiInstalledVersion();

        if (CheckGsaUpdateIsRequired(_GsaApiDllVersion, GsaVersionRequired.FullVersion)) {
          LoadException(_GsaUpgradeRequiredMessage, GsaVersionMustBeUpdatedMessage);
          return false;
        }
      } catch (Exception e) {
        LoadException(_LoadingGsaApiMessage, DisablePluginsErrorMessage(e.Message, LoadedPlugins()));
        return false;
      }

      return true;
    }

    private void SetGsaApiInstalledVersion() {
      var gsaInstalledVersion = FileVersionInfo.GetVersionInfo(GsaApiDllFullPath);
      _GsaApiDllVersion
        = $"{gsaInstalledVersion.FileMajorPart}.{gsaInstalledVersion.FileMinorPart}.{gsaInstalledVersion.FileBuildPart}";
    }

    private static string DisablePluginsErrorMessage(string errorMessage, string loadedPlugins) {
      return errorMessage + Environment.NewLine + Environment.NewLine
        + "This may be due to clash with other referenced dll files by one of these plugins that's already been loaded: "
        + Environment.NewLine + loadedPlugins + Environment.NewLine
        + "You may try disable the above plugins to solve the issue." + Environment.NewLine
        + "The plugin cannot be loaded.";
    }

    private void SetPlugins() {
      Utility.InitialiseMainMenuUnitsAndDependentPluginsCheck();
      RhinoApp.Closing += GsaComHelper.Dispose;
      _analytics.PluginLoaded(PluginInfo.Instance, _GsaApiDllVersion);
    }

    private static void SetInstances() {
      Instances.CanvasCreated += MenuLoad.OnStartup;
      Instances.ComponentServer.AddCategorySymbolName(ProductName, 'G');
      Instances.ComponentServer.AddCategoryIcon(ProductName, Resources.GSALogo);
    }

    private static string LoadedPlugins() {
      ReadOnlyCollection<GH_AssemblyInfo> plugins = Instances.ComponentServer.Libraries;
      string loadedPlugins = plugins.Where(plugin => !plugin.IsCoreLibrary)
       .Where(plugin => !plugin.Name.StartsWith("Kangaroo")).Aggregate(string.Empty,
          (current, plugin) => current + "-" + plugin.Name + Environment.NewLine);
      return loadedPlugins;
    }

    public static bool CheckGsaUpdateIsRequired(string foundVersion, string minimumVersion) {
      char separator = '.';
      string[] splitFoundVersion = foundVersion.Split(separator);
      string[] splitMinimumVersion = minimumVersion.Split(separator);

      if (int.Parse(splitFoundVersion[0]) >= int.Parse(splitMinimumVersion[0])
        && int.Parse(splitFoundVersion[1]) >= int.Parse(splitMinimumVersion[1])
        && int.Parse(splitFoundVersion[2]) >= int.Parse(splitMinimumVersion[2])) {
        return false;
      }

      return true;
    }

    private bool CheckGsaApiExists() {
      if (File.Exists(GsaApiDllFullPath)) {
        return true;
      }

      LoadException(_LoadingGsaApiMessage, GsaghRequiresGsaToBeInstalledMessage);
      return false;
    }

    private static void LoadException(string name, string exceptionMessage) {
      var exception = new Exception(exceptionMessage);
      var ghLoadingException = new GH_LoadingException(name, exception);
      Instances.ComponentServer.LoadingExceptions.Add(ghLoadingException);
      PostHog.PluginLoaded(PluginInfo.Instance, exception.Message);
    }

    private static void SetSystemEnvironmentVariable() {
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      string value = InstallPath + ";" + pathvar;
      Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }

    private static string TryFindPluginPath(string keyword) {
      // initially look in %appdata% folder where package manager will store the plugin
      if (!McNeelPathExists(keyword, out string path)) {
        return Path.GetDirectoryName(path);
      }

      if (!LibrariesFilesExists(keyword, ref path, out string libraryPath)) {
        return Path.GetDirectoryName(path);
      }

      if (PluginFilesExists(keyword, ref path)) {
        return Path.GetDirectoryName(path);
      }

      CantFindPluginError(keyword, libraryPath);
      return string.Empty;
    }

    private static bool McNeelPathExists(string keyword, out string path) {
      path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", RhinoApp.ExeVersion + ".0", ProductName);

      return File.Exists(Path.Combine(path, keyword));
    }

    private static bool LibrariesFilesExists(string keyword, ref string path, out string libraryPath) {
      libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper",
        "Libraries");

      string[] files = Directory.GetFiles(libraryPath, keyword, SearchOption.AllDirectories);
      if (files.Length > 0) {
        path = files[0].Replace(keyword, string.Empty);
      }

      return File.Exists(Path.Combine(path, keyword));
    }

    private static bool PluginFilesExists(string keyword, ref string path) {
      foreach (string[] files in Folders.AssemblyFolders
       .Select(pluginFolder => Directory.GetFiles(pluginFolder.Folder, keyword, SearchOption.AllDirectories))
       .Where(files => files.Length > 0)) {
        path = files[0].Replace(keyword, string.Empty);
        return true;
      }

      return false;
    }

    private static void CantFindPluginError(string keyword, string sDir) {
      string message = "Error loading the file " + keyword
        + " from any Grasshopper plugin folders - check if the file exist." + Environment.NewLine
        + "The plugin cannot be loaded." + Environment.NewLine + "Folders (including subfolder) that was searched:"
        + Environment.NewLine + sDir;
      message = Folders.AssemblyFolders.Aggregate(message,
        (current, pluginFolder) => current + Environment.NewLine + pluginFolder.Folder);

      string exceptionName = ProductName + ": " + keyword + " loading failed";
      LoadException(exceptionName, message);
    }
  }

  internal sealed class PluginInfo {
    private static readonly Lazy<OasysPluginInfo> lazy = new Lazy<OasysPluginInfo>(()
      => new OasysPluginInfo(ProductName, PluginName, GrasshopperVersion, isBeta,
        "phc_QjmqOoe8GqTMi3u88ynRR3WWvrJA9zAaqcQS1FDVnJD"));
    public static OasysPluginInfo Instance => lazy.Value;
  }
}
