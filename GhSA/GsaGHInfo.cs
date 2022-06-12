using System;
using System.IO;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using System.Reflection;
using System.Net;

namespace GsaGH
{
  public class AddReferencePriority : GH_AssemblyPriority
  {
    public override GH_LoadingInstruction PriorityLoad()
    {
      // ### Search for plugin path ###

      // initially look in %appdata% folder where package manager will store the plugin
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      path = Path.Combine(path, "McNeel", "Rhinoceros", "Packages", Rhino.RhinoApp.ExeVersion + ".0", "GSA");

      if (!File.Exists(Path.Combine(path, "GSA.gha"))) // if no plugin file is found there continue search
      {
        // look in all the other Grasshopper assembly (plugin) folders
        foreach (GH_AssemblyFolderInfo pluginFolder in Grasshopper.Folders.AssemblyFolders)
        {
          if (File.Exists(Path.Combine(pluginFolder.Folder, "GSA.gha"))) // if the folder contains the plugin
          {
            path = pluginFolder.Folder;
            break;
          }
        }
      }
      PluginPath = Path.GetDirectoryName(path);

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
        return GH_LoadingInstruction.Abort;
      }

      // ### Use GsaAPI to load referenced dlls ###
      try
      {
        //InitiateGsaAPI.UseGsaAPI();
      }
      catch (Exception)
      {
        return GH_LoadingInstruction.Abort;
      }

      // ### Queue up Main menu loader ###
      UI.Menu.Loader menuLoad = new UI.Menu.Loader();
      menuLoad.CreateMainMenuItem();

      // ### Create Ribbon Category name and icon ###
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');
      Grasshopper.Instances.ComponentServer.AddCategoryIcon("GSA", GsaGH.Properties.Resources.GSALogo);

      // ### Setup units ###
      Units.SetupUnits();

      return GH_LoadingInstruction.Proceed;
    }
    //public static Assembly GsaAPI;
    public static string PluginPath;
    public static string InstallPath = Util.Gsa.InstallationFolderPath.GetPath;
  }
  public class InitiateGsaAPI
  {
    internal static void UseGsaAPI()
    {
      // create new GH-GSA model
      try
      {
        GsaAPI.Model mTest = new GsaAPI.Model();
      }
      catch (Exception e)
      {
        Exception exception = new Exception("Error when creating new empty model using GsaAPI.dll" + System.Environment.NewLine + e.ToString());
        GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Model error", exception);
        Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
      }
      GsaAPI.Model m = new GsaAPI.Model();


      // get the GSA install path
      string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      System.IO.Directory.CreateDirectory(tempPath + "\\Oasys\\");
      tempPath = Path.Combine(tempPath, "Oasys");
      System.IO.Directory.CreateDirectory(tempPath + "\\GsaGrasshopper\\");
      tempPath = Path.Combine(tempPath, "GsaGrasshopper");

      // open existing GSA model (steel design sample)
      // model containing CAT section profiles which I
      // think loads the SectLib.db3 SQL lite database

      // try open stair sample file
      try
      {
        GsaAPI.ReturnValue openTest = m.Open(tempPath + "\\Samples\\Env.gwb");
      }
      catch (Exception e)
      {
        Exception exception = new Exception("Error when trying to open example file using GsaAPI.dll" + System.Environment.NewLine + e.ToString());
        GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Open error", exception);
        Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
      }
      GsaAPI.ReturnValue open = m.Open(tempPath + "\\Samples\\Env.gwb");

      // check if success
      if (open == GsaAPI.ReturnValue.GS_FILE_OPEN_ERROR)
      {
        // if not create new directory
        System.IO.Directory.CreateDirectory(tempPath + "\\Samples\\");
        // create webclient and download example file:
        try
        {
          WebClient webClient = new WebClient();
          webClient.DownloadFile("https://samples.oasys-software.com/gsa/10.1/General/Env.gwb", tempPath + "\\Samples\\Env.gwb");
        }
        catch (Exception e)
        {
          Exception exception = new Exception("Error when trying to download example file from https://samples.oasys-software.com/gsa/10.1/General/" + System.Environment.NewLine + e.ToString()
              + System.Environment.NewLine + "You may manually place the file 'Env.gwb' in this folder to solve the issue: " + System.Environment.NewLine + tempPath + "\\Samples\\");
          GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: ExampleFile missing", exception);
          Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
        }

        // try open the file again:
        open = m.Open(tempPath + "\\Samples\\Env.gwb");

        // if model is opened run it
        if (open == GsaAPI.ReturnValue.GS_OK)
        {
          try
          {
            //m.Analyse(1);
            ReadOnlyDictionary<int, GsaAPI.Section> sDict = m.Sections();
            sDict.TryGetValue(1, out GsaAPI.Section apisection);
            double area1 = apisection.Area;
            string profile1 = apisection.Profile;
            string profile = "CAT HE HE200.B";
            GsaAPI.Section section = new GsaAPI.Section();
            section.Profile = profile;
            double area = section.Area * Math.Pow(10, 6);
          }
          catch (Exception e)
          {
            Exception exception = new Exception("Error when running analysis task on example file." + System.Environment.NewLine + e.ToString());
            GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Analysis error", exception);
            Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
          }
        }
      }
      else
      {
        try
        {
          //m.Analyse(1);
          ReadOnlyDictionary<int, GsaAPI.Section> sDict = m.Sections();
          sDict.TryGetValue(1, out GsaAPI.Section apisection);
          double area1 = apisection.Area;
          string profile1 = apisection.Profile;
          string profile = "CAT HE HE200.B";
          GsaAPI.Section section = new GsaAPI.Section();
          section.Profile = profile;
          double area = section.Area * Math.Pow(10, 6);
        }
        catch (Exception e)
        {
          Exception exception = new Exception("Error when running analysis task on example file." + System.Environment.NewLine + e.ToString());
          GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Analysis error", exception);
          Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
        }
      }
      m.Dispose();
    }
  }
  public class GsaGHInfo : GH_AssemblyInfo
  {
    internal static Guid GUID = new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
    internal const string Company = "Oasys";
    internal const string Copyright = "Copyright © Oasys 1985 - 2022";
    internal const string Contact = "https://www.oasys-software.com/";
    internal const string Vers = "0.9.18";
    internal static bool isBeta = true;
    internal const string ProductName = "GSA";
    internal const string PluginName = "GsaGH";

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
        + System.Environment.NewLine + "A licensed version of GSA 10.1.60 or later installed in"
        + System.Environment.NewLine + @"C:\Program Files\Oasys\GSA 10.1\ "
        + System.Environment.NewLine + "is required to use this plugin."
        + System.Environment.NewLine + "Contact oasys@arup.com to request a free trial version."
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
    public string icon_url
    {
      get
      {
        // to be updated - not supported by yak currently
        return "https://raw.githubusercontent.com/arup-group/GSA-Grasshopper/master/GhSA/Properties/Icons/icons/4x/GsaGhLogo%404x.png";
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
