﻿using System;
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
    internal const string Company = "Oasys";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Copyright = "Copyright © Oasys 1985 - 2024";
    public const string PluginName = "GsaGH";
    public const string ProductName = "GSA";
    internal const string TermsConditions
      = "Oasys terms and conditions apply. See https://www.oasys-software.com/terms-conditions for details. ";
    internal const string GrasshopperVersion = "1.4.0";
    internal static int GsaMajorVersion = 10;
    public static int GsaMinorVersion = 2;
    internal static int GsaBuildVersion = 11;
    internal static string GsaMainVersion = $"{GsaMajorVersion}.{GsaMinorVersion}";
    internal static string GsaFullVersion = $"{GsaMajorVersion}.{GsaMinorVersion}.{GsaBuildVersion}";
    internal static string disclaimer = $"{PluginName} is pre-release and under active development, "
      + "including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. "
      + "Future versions may contain breaking changes. Any files, results, or other types of output information created using "
      + $"{PluginName} should not be relied upon without thorough and independent checking. "
      + $"{PluginName} {GrasshopperVersion} requires {ProductName} {GsaFullVersion} or higher installed.";
    internal static Guid guid = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    internal static bool isBeta = false;
    public override string AuthorContact => Contact;
    public override string AuthorName => Company;
    public override string Description
      =>
        //Return a short string describing the purpose of this GHA library.
        $"Official Oasys {ProductName} Grasshopper Plugin {Environment.NewLine} {(isBeta ? disclaimer : string.Empty)}"
        + Environment.NewLine + $"A licensed version of {ProductName} {GsaFullVersion} or later installed in "
        + $@"C:\Program Files\Oasys\{ProductName} {GsaMainVersion}\ is required to use this plugin."
        + Environment.NewLine + "Contact oasys@arup.com to request a free trial version." + Environment.NewLine
        + TermsConditions + Environment.NewLine + Copyright;
    public override Bitmap Icon => Resources.GSALogo;
    public override Guid Id => guid;
    public override string Name => ProductName;
    public override string Version => isBeta ? GrasshopperVersion + "-beta" : GrasshopperVersion;
  }

  public class AddReferencePriority : GH_AssemblyPriority {
    private const string _GsaApiDllFileName = "GsaAPI.dll";
    private const string _LoadingGsaApiMessage = "GSA: GsaAPI.dll loading";
    private const string _GsaUpgradeRequiredMessage = "GSA Version Error: Upgrade required";
    private const string _GsaComputeVersionMessage = "GSA Version Error: Cannot compute the version";

    public static string PluginPath => pluginPath ??= TryFindPluginPath("GSA.gha");
    public static string InstallPath => InstallationFolder.GetPath;

    private static string pluginPath;
    private static string GsaApiDllFullPath => Path.Combine(InstallPath, _GsaApiDllFileName);
    private static readonly string GsaVersionCannotBeReadMessage
      = $"Please conntact with {GsaGhInfo.Company} to solve this error";
    private static string MicrosoftDataSqliteDll => @"\Microsoft.Data.Sqlite.dll";
    // MESSAGES
    private static string GsaghRequiresGsaToBeInstalledMessage
      => $"{GsaGhInfo.PluginName} requires {GsaGhInfo.ProductName} to be installed in {InstallPath}"
        + $". Unable to find {_GsaApiDllFileName}. It looks like you haven't got {GsaGhInfo.ProductName} installed, or it may be installed in an unknown path. Please install or reinstall {GsaGhInfo.ProductName} in "
        + InstallPath + ", restart Rhino and load Grasshopper again.";
    private static string GsaVersionMustBeUpdatedMessage
      => "Version " + GsaGhInfo.GrasshopperVersion
        + $" of GSA-Grasshopper requires {GsaGhInfo.GsaFullVersion} installed. Please upgrade {GsaGhInfo.ProductName}.";
    private string _GsaApiDllVersion;

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
      if (Assembly.GetEntryAssembly() != null && !Assembly.GetEntryAssembly().FullName.Contains("compute.geometry")) {
        Assembly.LoadFile(PluginPath + MicrosoftDataSqliteDll);
      }

      SetInstances();
      SetPlugins();
      return GH_LoadingInstruction.Proceed;
    }

    private bool TryToLoadGsaDll() {
      try {
        Assembly.LoadFile(GsaApiDllFullPath);
        SetGsaApiInstalledVersion();

        if (!TryCalculateVersions(_GsaApiDllVersion, GsaGhInfo.GsaFullVersion, out int gsaApiDllVersionAsInt,
          out int gsaVersionNeededAsInt)) {
          return false;
        }

        if (CheckGsaUpdateIsRequired(gsaApiDllVersionAsInt, gsaVersionNeededAsInt)) {
          return false;
        }
      } catch (Exception e) {
        string loadedPlugins = LoadedPlugins();
        string message = DisablePluginsErrorMessage(e.Message, loadedPlugins);
        LoadException(_LoadingGsaApiMessage, message);
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

    private bool TryCalculateVersions(
      string dll1Version, string dll2Version, out int dll1VersionAsInt, out int dll2VersionAsInt) {
      if (!int.TryParse(dll1Version.Replace(".", string.Empty), out dll1VersionAsInt)
        & !int.TryParse(dll2Version.Replace(".", string.Empty), out dll2VersionAsInt)) {
        { //maybe new error?
          LoadException(_GsaComputeVersionMessage, GsaVersionCannotBeReadMessage);
          return false;
        }
      }

      return true;
    }

    private void SetPlugins() {
      Utility.InitialiseMainMenuUnitsAndDependentPluginsCheck();
      RhinoApp.Closing += GsaComHelper.Dispose;
      PostHog.PluginLoaded(PluginInfo.Instance, _GsaApiDllVersion);
    }

    private static void SetInstances() {
      Instances.CanvasCreated += MenuLoad.OnStartup;
      Instances.ComponentServer.AddCategorySymbolName(GsaGhInfo.ProductName, 'G');
      Instances.ComponentServer.AddCategoryIcon(GsaGhInfo.ProductName, Resources.GSALogo);
    }

    private static string LoadedPlugins() {
      ReadOnlyCollection<GH_AssemblyInfo> plugins = Instances.ComponentServer.Libraries;
      string loadedPlugins = plugins.Where(plugin => !plugin.IsCoreLibrary)
       .Where(plugin => !plugin.Name.StartsWith("Kangaroo")).Aggregate(string.Empty,
          (current, plugin) => current + "-" + plugin.Name + Environment.NewLine);
      return loadedPlugins;
    }

    private static bool CheckGsaUpdateIsRequired(int gsaApiDllVersion, int gsaVersionNeeded) {
      if (gsaApiDllVersion >= gsaVersionNeeded) {
        return false;
      }

      LoadException(_GsaUpgradeRequiredMessage, GsaVersionMustBeUpdatedMessage);
      return true;
    }

    private static bool CheckGsaApiExists() {
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
      EnvironmentVariableTarget target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);
    }

    private static string TryFindPluginPath(string keyword) {
      // initially look in %appdata% folder where package manager will store the plugin
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", RhinoApp.ExeVersion + ".0", GsaGhInfo.ProductName);

      if (!File.Exists(Path.Combine(path, keyword))) {
        return Path.GetDirectoryName(path);
      }

      string sDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Grasshopper",
        "Libraries");

      string[] files = Directory.GetFiles(sDir, keyword, SearchOption.AllDirectories);
      if (files.Length > 0) {
        path = files[0].Replace(keyword, string.Empty);
      }

      if (!File.Exists(Path.Combine(path, keyword))) {
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

      CantFindPluginError(keyword, sDir);
      return string.Empty;
    }

    private static void CantFindPluginError(string keyword, string sDir) {
      string message = "Error loading the file " + keyword
        + " from any Grasshopper plugin folders - check if the file exist." + Environment.NewLine
        + "The plugin cannot be loaded." + Environment.NewLine + "Folders (including subfolder) that was searched:"
        + Environment.NewLine + sDir;
      message = Folders.AssemblyFolders.Aggregate(message,
        (current, pluginFolder) => current + Environment.NewLine + pluginFolder.Folder);

      string exceptionName = GsaGhInfo.ProductName + ": " + keyword + " loading failed";
      LoadException(exceptionName, message);
    }
  }

  internal sealed class PluginInfo {
    public static OasysPluginInfo Instance => lazy.Value;
    private static readonly Lazy<OasysPluginInfo> lazy = new Lazy<OasysPluginInfo>(()
      => new OasysPluginInfo(GsaGhInfo.ProductName, GsaGhInfo.PluginName, GsaGhInfo.GrasshopperVersion,
        GsaGhInfo.isBeta, "phc_QjmqOoe8GqTMi3u88ynRR3WWvrJA9zAaqcQS1FDVnJD"));
  }
}
