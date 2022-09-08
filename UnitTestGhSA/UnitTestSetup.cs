using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit;
using GsaAPI;
using GsaGH;
using GsaGH.Parameters;
using System.Runtime.InteropServices;
using UnitsNet.Units;
using Oasys.Units;

// A SetUpFixture outside of any namespace provides SetUp and TearDown for the entire assembly.
[SetUpFixture]
public class SetUp
{
  [OneTimeSetUp]
  public void RunBeforeAnyTests()
  {
    // Executes once before any test run

    // load GsaAPI.dll and set process user-rights to GSA installation folder
    UnitTestGsaGH.InitiateGsa.LoadRefs();

    // use GsaAPI once to open a model and force loading of sectlib.db3 SQL library
    UnitTestGsaGH.InitiateGsa.UseGsaAPI();

    // set units used in the unit-test (kN-m). Avoids conflict with trying to read Rhino doc units
    UnitTestGsaGH.InitiateGsa.SetUnits();

    // add current project (for GSA.gha) to grasshopper folder:
    string rootfolder = AppDomain.CurrentDomain.BaseDirectory;
    rootfolder = rootfolder.Split(new string[] { "UnitTestGsaGH" }, StringSplitOptions.None)[0];

    Grasshopper.Folders.CustomAssemblyFolders.Add(rootfolder);

    // setup Rhino7 (headless) and resolve assembly conflicts for RhinoCommon.dll and Grasshopper.dll
    UnitTestGsaGH.InitiateRhinoGH.LoadRhino7GH();
  }

  [OneTimeTearDown]
  public void RunAfterAnyTests()
  {
    // Executes once after the test run. (Optional)
    UnitTestGsaGH.InitiateRhinoGH.ExitInProcess();
  }
}
namespace ComponentsTest
{
  [SetUpFixture]
  public class SetUpComponentsTest
  {
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
      // Executes once before test runs in ComponentsTest class

    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
      // Executes once after the test run. (Optional)

    }
  }
}
namespace UnitTestGsaGH
{
  public class InitiateGsa
  {
    public static void LoadRefs()
    {
      // set folder to latest GSA version.
      Assembly ass1 = Assembly.LoadFile(GsaGH.Util.Gsa.InstallationFolderPath.GetPath + "\\GsaAPI.dll");
      Assembly ass2 = Assembly.LoadFile(GsaGH.Util.Gsa.InstallationFolderPath.GetPath + "\\System.Data.SQLite.dll");

      const string name = "PATH";
      string pathvar = System.Environment.GetEnvironmentVariable(name);
      var value = pathvar + ";" + GsaGH.Util.Gsa.InstallationFolderPath.GetPath + "\\";
      var target = EnvironmentVariableTarget.Process;
      System.Environment.SetEnvironmentVariable(name, value, target);
    }
    public static void UseGsaAPI()
    {
      // create new GH-GSA model 
      Model m = new Model();

      string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      tempPath = System.IO.Path.Combine(tempPath, "Oasys", "GsaGrasshopper");
      string file = tempPath + "\\Samples\\Env.gwb";

      // open existing GSA model (steel design sample)
      // model containing CAT section profiles which I
      // think loads the SectLib.db3 SQL lite database
      m.Open(file);
    }

    public static void SetUnits()
    {
      Units.LengthUnitGeometry = LengthUnit.Meter;
      Units.LengthUnitSection = LengthUnit.Millimeter;
      Units.LengthUnitResult = LengthUnit.Millimeter;
      Units.StressUnit = PressureUnit.Megapascal;
      Units.ForceUnit = ForceUnit.Kilonewton;
      Units.MomentUnit = MomentUnit.KilonewtonMeter;
      Units.MassUnit = MassUnit.Tonne;
    }
  }
  public class InitiateRhinoGH
  {
    public static void LoadRhino7GH()
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
