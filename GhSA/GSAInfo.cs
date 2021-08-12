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
            // ### Reference GSA API and SQLite dlls ###
            // set folder to latest GSA version.
            try
            {
                Assembly ass1 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\GsaAPI.dll");
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

                System.Windows.Forms.MessageBox.Show(e.Message
                    + System.Environment.NewLine + System.Environment.NewLine +
                    "This may be due to clash with other referenced dll files by one of these plugins that's already been loaded: "
                    + System.Environment.NewLine + loadedPlugins
                    + System.Environment.NewLine + "The plugin cannot be loaded.", "GSA plugin failed to load", System.Windows.Forms.MessageBoxButtons.OK);
                return GH_LoadingInstruction.Abort;
            }
            
            Assembly ass2 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\System.Data.SQLite.dll");
            //Assembly ass3 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\libiomp5md.dll");

            // ### Set system environment variables to allow user rights to read above dll ###
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + Util.Gsa.InstallationFolderPath.GetPath + "\\";
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);

            // ### Use GsaAPI to load referenced dlls ###
            if (!InitiateGsaAPI.UseGsaAPI())
            {
                System.Windows.Forms.MessageBox.Show("Unable to run a test analysis using the GsaAPI."
                    + System.Environment.NewLine + "The plugin cannot be loaded.", "GSA plugin failed to load", System.Windows.Forms.MessageBoxButtons.OK);
                return GH_LoadingInstruction.Abort;
            }

            // ### Create Ribbon Category name and icon ###
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("GSA", GhSA.Properties.Resources.GsaGhLogo);
            
            return GH_LoadingInstruction.Proceed;
        }
    }
    public class InitiateGsaAPI
    {
        public static bool UseGsaAPI()
        {
            // create new GH-GSA model 
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
            ReturnValue open = m.Open(tempPath + "\\Samples\\Env.gwb");
            
            // check if success
            if (open == ReturnValue.GS_FILE_OPEN_ERROR)
            {
                // if not create new directory
                System.IO.Directory.CreateDirectory(tempPath + "\\Samples\\");
                // create webclient and download example file:
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://samples.oasys-software.com/gsa/10.1/General/Env.gwb", tempPath + "\\Samples\\Env.gwb");

                // try open the file again:
                open = m.Open(tempPath + "\\Samples\\Env.gwb");
                
                // if model is opened run it
                if (open == ReturnValue.GS_OK)
                {
                    try
                    {
                        return m.Analyse(1);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
                
            }
            else
            {
                try
                {
                    return m.Analyse(1);
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
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
                return "GSA Plugin";
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
        public string icon
        {
            get
            {
                return "https://github.com/arup-group/GSA-Grasshopper/blob/master/GhSA/Properties/GsaGhLogo.png";
            }
        }

        public override string Version
        {
            get
            {
                return "0.3.2";
            }
        }
    }
}
