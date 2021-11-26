using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using System.Reflection;
using GsaAPI;
using System.Net;

namespace GhSA
{
    public class AddReferencePriority : GH_AssemblyPriority
    {
        
        /// <summary>
        /// This method finds the user's GSA installation folder and loads GsaAPI.dll so that this plugin does not need to ship with an additional dll file
        /// 
        /// Method also provides access rights to Grasshopper to read dynamically linked dll files in the GSA installation folder.
        /// </summary>
        /// <returns></returns>
        public override GH_LoadingInstruction PriorityLoad()
        {
            // ## Get plugin assembly file location
            PluginPath = Assembly.GetExecutingAssembly().Location; // full path+name
            PluginPath = PluginPath.Replace("GSA.gha", "");

            // ### Set system environment variables to allow user rights to read below dlls ###
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + PluginPath;
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);

            // ### Reference GSA API and SQLite dlls ###
            // set folder to latest GSA version.
            try
            {
                Assembly GsaAPI = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\GsaAPI.dll");
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
                Grasshopper.Kernel.GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI.dll loading", exception);
                Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
                return GH_LoadingInstruction.Abort;
            }

            try
            {
                Assembly ass2 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\System.Data.SQLite.dll");
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
                Grasshopper.Kernel.GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: System.Data.SQLite.dll loading", exception);
                Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
                return GH_LoadingInstruction.Abort;
            }

            //Assembly ass3 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\libiomp5md.dll");

            // ### Use GsaAPI to load referenced dlls ###
            try
            {
                //InitiateGsaAPI.UseGsaAPI();
            }
            catch (Exception)
            {
                return GH_LoadingInstruction.Abort;
            }

            // create main menu dropdown
            GhSA.UI.Menu.Loader menuLoad = new UI.Menu.Loader();
            menuLoad.CreateMainMenuItem();

            // ### Create Ribbon Category name and icon ###
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("GSA", GhSA.Properties.Resources.GsaGhLogo);
            
            return GH_LoadingInstruction.Proceed;
        }
        public static Assembly GsaAPI;
        public static string PluginPath;
    }
    public class InitiateGsaAPI
    {
        internal static void UseGsaAPI()
        {
            // create new GH-GSA model
            try
            {
                Model mTest = new Model();
            }
            catch (Exception e)
            {
                Exception exception = new Exception("Error when creating new empty model using GsaAPI.dll" + System.Environment.NewLine + e.ToString());
                Grasshopper.Kernel.GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Model error", exception);
                Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
            }
            Model m = new Model();


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
                ReturnValue openTest = m.Open(tempPath + "\\Samples\\Env.gwb");
            }
            catch (Exception e)
            {
                Exception exception = new Exception("Error when trying to open example file using GsaAPI.dll" + System.Environment.NewLine + e.ToString());
                Grasshopper.Kernel.GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Open error", exception);
                Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
            }
            ReturnValue open = m.Open(tempPath + "\\Samples\\Env.gwb");
            
            // check if success
            if (open == ReturnValue.GS_FILE_OPEN_ERROR)
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
                    Grasshopper.Kernel.GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: ExampleFile missing", exception);
                    Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
                }

                // try open the file again:
                open = m.Open(tempPath + "\\Samples\\Env.gwb");
                
                // if model is opened run it
                if (open == ReturnValue.GS_OK)
                {
                    try
                    {
                        //m.Analyse(1);
                        ReadOnlyDictionary<int, Section> sDict = m.Sections();
                        sDict.TryGetValue(1, out Section apisection);
                        double area1 = apisection.Area;
                        string profile1 = apisection.Profile;
                        string profile = "CAT HE HE200.B";
                        Section section = new Section();
                        section.Profile = profile;
                        double area = section.Area * Math.Pow(10, 6);
                    }
                    catch (Exception e)
                    {
                        Exception exception = new Exception("Error when running analysis task on example file." + System.Environment.NewLine + e.ToString());
                        Grasshopper.Kernel.GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Analysis error", exception);
                        Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
                    }
                }
            }
            else
            {
                try
                {
                    //m.Analyse(1);
                    ReadOnlyDictionary<int, Section> sDict = m.Sections();
                    sDict.TryGetValue(1, out Section apisection);
                    double area1 = apisection.Area;
                    string profile1 = apisection.Profile;
                    string profile = "CAT HE HE200.B";
                    Section section = new Section();
                    section.Profile = profile;
                    double area = section.Area * Math.Pow(10, 6);
                }
                catch (Exception e)
                {
                    Exception exception = new Exception("Error when running analysis task on example file." + System.Environment.NewLine + e.ToString());
                    Grasshopper.Kernel.GH_LoadingException gH_LoadingException = new GH_LoadingException("GSA: GsaAPI Analysis error", exception);
                    Grasshopper.Instances.ComponentServer.LoadingExceptions.Add(gH_LoadingException);
                }
            }
            m.Dispose();
        }
    }
    public class GSAInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "GSA";
            }
        }
        public override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaModel;
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Official Oasys GSA Grasshopper Plugin" + System.Environment.NewLine
                + System.Environment.NewLine + "A licensed version of GSA 10.1 installed in"
                + System.Environment.NewLine + @"C:\Program Files\Oasys\GSA 10.1\ "
                + System.Environment.NewLine + "is required to use this plugin."
                + System.Environment.NewLine 
                + System.Environment.NewLine + "Contact oasys@arup.com to request a free trial version."
                + System.Environment.NewLine + System.Environment.NewLine + "Copyright © Oasys 1985 - 2021";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Oasys / Kristjan Nielsen";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "https://www.oasys-software.com/";
            }
        }
        public string icon_url
        {
            get
            {
                return "https://raw.githubusercontent.com/arup-group/GSA-Grasshopper/master/GhSA/Properties/Icons/icons/4x/GsaGhLogo%404x.png";
            }
        }

        public override string Version
        {
            get
            {
                return "0.3.8-beta";
            }
        }
    }
}
