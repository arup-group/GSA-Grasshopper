using System;
using System.Reflection;
using NUnit.Framework;
using NUnit;
using GsaAPI;
using GhSA;
using GhSA.Parameters;
using System.Runtime.InteropServices;

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
        UnitTestGhSA.Initiate.InitiateRhinoGH();
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
        public static void LoadRefs()
        {
            // set folder to latest GSA version.
            Assembly ass1 = Assembly.LoadFile(GhSA.Util.Gsa.GsaPath.GetPath + "\\GsaAPI.dll");
            Assembly ass2 = Assembly.LoadFile(GhSA.Util.Gsa.GsaPath.GetPath + "\\System.Data.SQLite.dll");

            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + ";" + GhSA.Util.Gsa.GsaPath.GetPath + "\\";
            var target = EnvironmentVariableTarget.Process;
            System.Environment.SetEnvironmentVariable(name, value, target);
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

        public static void InitiateRhinoGH()
        {
            // Ensure we are 64 bit
            Assert.IsTrue(Environment.Is64BitProcess, "Tests must be run as x64");

            // Set path to rhino system directory
            string envPath = Environment.GetEnvironmentVariable("path");
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string systemDir = System.IO.Path.Combine(programFiles, "Rhino 7", "System");
            
            Assert.IsTrue(System.IO.Directory.Exists(systemDir), "Rhino system dir not found: {0}", systemDir);

            Environment.SetEnvironmentVariable("path", envPath + ";" + systemDir);

            // Add hook for .Net assmbly resolve (for RhinoCommmon.dll and Grasshopper.dll)
            AppDomain.CurrentDomain.AssemblyResolve += ResolveRhinoCommon;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveGrasshopper;

            // Start headless Rhino process
            LaunchInProcess(0, 0);
        }

        private static Assembly ResolveRhinoCommon(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (!name.StartsWith("RhinoCommon", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            string fullPath = AppDomain.CurrentDomain.BaseDirectory;

            var path = System.IO.Path.Combine(fullPath, "RhinoCommon.dll");
            return Assembly.LoadFrom(path);
        }

        private static Assembly ResolveGrasshopper(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (!name.StartsWith("Grasshopper", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            string fullPath = AppDomain.CurrentDomain.BaseDirectory;

            var path = System.IO.Path.Combine(fullPath, "Grasshopper.dll");
            return Assembly.LoadFrom(path);
        }
        
        [DllImport("RhinoLibrary.dll")]
        internal static extern int LaunchInProcess(int reserved1, int reserved2);

        [DllImport("RhinoLibrary.dll")]
        internal static extern int ExitInProcess();
    }
}
