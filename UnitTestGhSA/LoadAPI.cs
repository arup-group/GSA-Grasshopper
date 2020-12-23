using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GsaAPI;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;

namespace UnitTestGhSA
{
    public class Helper
    {
        public static bool LoadAPI()
        {
            // set folder to latest GSA version.
            Assembly ass1 = Assembly.LoadFile(GhSA.Util.Gsa.GsaPath.GetPath + "\\GsaAPI.dll");
            Assembly ass2 = Assembly.LoadFile(GhSA.Util.Gsa.GsaPath.GetPath + "\\System.Data.SQLite.dll");

            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + GhSA.Util.Gsa.GsaPath.GetPath + "\\";
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);
            
            return true;
        }
    }
}
