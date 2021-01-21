using System;
using System.Drawing;
using Grasshopper.Kernel;
using System.Reflection;
using GsaAPI;

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
            // set folder to latest GSA version.
            Assembly ass1 = Assembly.LoadFile(Util.Gsa.GsaPath.GetPath + "\\GsaAPI.dll");
            Assembly ass2 = Assembly.LoadFile(Util.Gsa.GsaPath.GetPath + "\\System.Data.SQLite.dll");

            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + Util.Gsa.GsaPath.GetPath + "\\";
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);

            InitiateGsaAPI.UseGsaAPI();

            // test if this solves problems with libiomp5md.dll version (karamba using different version)
            System.Environment.SetEnvironmentVariable("KMP_DUPLICATE_LIB_OK", "TRUE");

            // ### Create Category icon ###
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');

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
            string installPath = GhSA.Util.Gsa.GsaPath.GetPath; 

            // open existing GSA model (steel design sample)
            // model containing CAT section profiles which I
            // think loads the SectLib.db3 SQL lite database
            m.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");
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
                return "Oasys";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "kristjan.nielsen@arup.com";
            }
        }
        public override string Version
        {
            get
            {
                return "0.1.19-alpha";
            }
        }
    }
}
