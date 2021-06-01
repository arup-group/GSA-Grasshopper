using System;
using System.Drawing;
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
            Assembly ass1 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\GsaAPI.dll");
            Assembly ass2 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\System.Data.SQLite.dll");
            //Assembly ass3 = Assembly.LoadFile(Util.Gsa.InstallationFolderPath.GetPath + "\\libiomp5md.dll");

            // ### Set system environment variables to allow user rights to read above dll ###
            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + Util.Gsa.InstallationFolderPath.GetPath + "\\";
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);

            // ### Use GsaAPI to load referenced dlls ###
            InitiateGsaAPI.UseGsaAPI();

            // test if this solves problems with libiomp5md.dll version (karamba using different version)
            //System.Environment.SetEnvironmentVariable("KMP_DUPLICATE_LIB_OK", "TRUE");
            // Note: it did not, but pushed the error 

            // ### Create Ribbon Category name and icon ###
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("GSA", GhSA.Properties.Resources.GsaGhLogo);

            return GH_LoadingInstruction.Proceed;
        }

    }
    public class InitiateGsaAPI
    {
        public static void UseGsaAPI()
        {
            // create new GH-GSA model 
            Model m = new Model();

            // get the GSA install path
            string installPath = GhSA.Util.Gsa.InstallationFolderPath.GetPath; 

            // open existing GSA model (steel design sample)
            // model containing CAT section profiles which I
            // think loads the SectLib.db3 SQL lite database
            
            // try open stair sample file
            ReturnValue open = m.Open(installPath + "\\Samples\\Env.gwb");
            
            // check if success
            if (open == ReturnValue.GS_FILE_OPEN_ERROR)
            {
                // if not create new directory
                System.IO.Directory.CreateDirectory(installPath + "\\Samples\\");
                // create webclient and download example file:
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://samples.oasys-software.com/gsa/10.1/General/Env.gwb", installPath + "\\Samples\\Env.gwb");

                // try open the file again:
                open = m.Open(installPath + "\\Samples\\Env.gwb");
                
                // if model is opened run it
                if (open == ReturnValue.GS_OK)
                    m.Analyse(1);
            }
            else
                m.Analyse(1);
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
        public string Icon_url
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
                return "0.2.13";
            }
        }
    }
}
