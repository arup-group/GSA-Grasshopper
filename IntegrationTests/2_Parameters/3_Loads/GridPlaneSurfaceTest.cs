using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GridPlaneSurfaceTest {
    public static GH_Document Document => s_document ?? (s_document = OpenDocument());
    private static GH_Document s_document = null;

    [Theory]
    [InlineData("DefaultGpElevation", "0cm")]
    [InlineData("DefaultGpTolAbove", "auto")]
    [InlineData("DefaultGpTolBelow", "auto")]
    [InlineData("DefaultGpTolerance", "1cm")]
    public void TestDefaultGridPlane(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Theory]
    [InlineData("DefaultGsElevation", "0cm")]
    [InlineData("DefaultGsTolAbove", "auto")]
    [InlineData("DefaultGsTolBelow", "auto")]
    [InlineData("DefaultGsTolerance", "1cm")]
    public void TestDefaultGridSurface(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Theory]
    [InlineData("NumInputElevation", "1500cm")]
    [InlineData("NumInputTolAbove", "11cm")]
    [InlineData("NumInputTolBelow", "12cm")]
    [InlineData("NumInputTolerance", "25cm")]
    public void TestNumInput(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Theory]
    [InlineData("TxtInputElevation", "1500cm")]
    [InlineData("TxtInputTolAbove", "11cm")]
    [InlineData("TxtInputTolBelow", "12cm")]
    [InlineData("TxtInputTolerance", "25cm")]
    public void TestTxtInput(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty)
       .Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "3_Loads",
      });
      var io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);

      GH_ProcessStep state = io.Document.SolutionState;
      Assert.Equal(GH_ProcessStep.PostProcess, state);

      foreach (IGH_DocumentObject obj in io.Document.Objects) {
        if (obj is IGH_Param p) {
          p.CollectData();
          p.ComputeData();
          foreach (string message in p.RuntimeMessages(GH_RuntimeMessageLevel.Error)) {
            Console.WriteLine("Parameter " + p.NickName + ", Error: " + message);
          }

          foreach (string message in p.RuntimeMessages(GH_RuntimeMessageLevel.Warning)) {
            Console.WriteLine("Parameter " + p.NickName + ", Warning: " + message);
          }

          foreach (string message in p.RuntimeMessages(GH_RuntimeMessageLevel.Remark)) {
            Console.WriteLine("Parameter " + p.NickName + ", Remark: " + message);
          }
        }
      }

      foreach (IGH_DocumentObject obj in io.Document.Objects) {
        if (obj is IGH_Component comp) {
          foreach (string message in comp.RuntimeMessages(GH_RuntimeMessageLevel.Error)) {
            Console.WriteLine("Component " + comp.NickName + ", Error: " + message);
          }

          foreach (string message in comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning)) {
            Console.WriteLine("Component \" + comp.NickName + \", Warning: " + message);
          }

          foreach (string message in comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)) {
            Console.WriteLine("Component \" + comp.NickName + \", Remark: " + message);
          }
        }
      }

      return io.Document;
    }
  }
}
