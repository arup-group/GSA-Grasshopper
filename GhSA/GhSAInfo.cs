using System;
using System.Drawing;
using Grasshopper.Kernel;
using System.Reflection;

namespace GhSA
{

    public class AddReferencePriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Assembly ass1 = Assembly.LoadFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Oasys\\GSA 10.1\\GsaAPI.dll");
            //Assembly ass2 = Assembly.LoadFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Oasys\\GSA 10.1\\System.Data.SQLite.dll");
            //Assembly ass3 = Assembly.LoadFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Oasys\\GSA 10.1\\System.Data.SQLite.Linq.dll");

            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Oasys\\GSA 10.1\\";
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);

            // ### Create Category icon ###
            //Grasshopper.Instances.ComponentServer.AddCategoryIcon("GSA", Properties.Resources.GsaIcon);
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GSA", 'G');

            return GH_LoadingInstruction.Proceed;
        }
    }
    public class GhSAInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "GhSA";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
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
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
