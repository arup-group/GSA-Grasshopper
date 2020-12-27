using System;
using System.Reflection;

namespace UnitTestGhSA
{
    public class Helper
    {
        public static bool LoadRefs()
        {
            // set folder to latest GSA version.
            Assembly ass1 = Assembly.LoadFile(GhSA.Util.Gsa.GsaPath.GetPath + "\\GsaAPI.dll");
            Assembly ass2 = Assembly.LoadFile(GhSA.Util.Gsa.GsaPath.GetPath + "\\System.Data.SQLite.dll");

            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + GhSA.Util.Gsa.GsaPath.GetPath + "\\";
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);

            // load RhinoCommon + GH dlls
            //string rhinopath = "C:\\Users\\Kristjan.Nielsen\\source\\repos\\GhSA\\packages\\RhinoCommon.6.10.18308.14011\\lib\\net45";
            //string rhinopath = "C:\\Program Files\\Rhino 6\\System";
            //Assembly ass3 = Assembly.LoadFile(rhinopath + "\\RhinoCommon.dll");
            //Assembly ass4 = Assembly.LoadFile(rhinopath + "\\Eto.dll");
            //Assembly ass5 = Assembly.LoadFile(rhinopath + "\\Rhino.UI.dll");

            //string ghpath = "C:\\Program Files\\Rhino 6\\Plug-ins\\Grasshopper";
            //Assembly ass6 = Assembly.LoadFile(ghpath + "\\GH_IO.dll");
            //Assembly ass7 = Assembly.LoadFile(ghpath + "\\Grasshopper.dll");

            return true;
        }
    }
}
