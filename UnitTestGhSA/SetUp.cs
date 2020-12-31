using System;
using System.Reflection;
using NUnit.Framework;
using GsaAPI;
using GhSA;
using GhSA.Parameters;

// A SetUpFixture outside of any namespace provides SetUp and TearDown for the entire assembly.
[SetUpFixture]
public class SetUp
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        UnitTestGhSA.Initiate.LoadRefs();
        UnitTestGhSA.Initiate.UseGsaAPI();
        UnitTestGhSA.Initiate.SetUnits();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        // Executes once after the test run. (Optional)
    }
}

namespace UnitTestGhSA
{
    public class Initiate
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

            // get rid of the model again
            m.Close();
            m.Dispose();
        }

        public static void SetUnits()
        {
            GhSA.Util.GsaUnit.SetUnits_kN_m();
        }
    }
}
