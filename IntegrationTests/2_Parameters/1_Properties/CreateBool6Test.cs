using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class CreateBool6Test {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Theory]
    [InlineData("X", true)]
    [InlineData("Y", false)]
    [InlineData("Z", true)]
    [InlineData("XX", false)]
    [InlineData("YY", true)]
    [InlineData("ZZ", false)]
    [InlineData("Cast", true)]
    [InlineData("ReleaseOppositeToRestraint", true)]
    public void OutputTest(string groupIdentifier, bool expected) {
      GH_Document doc = Document;

      IGH_Param param = Helper.FindParameter(doc, groupIdentifier);

      Assert.Equal(1, param.VolatileData.DataCount);
      IEnumerator<IGH_Goo> data = param.VolatileData.AllData(true).GetEnumerator();
      data.Reset();
      data.MoveNext();
      var b = (GH_Boolean)data.Current;

      Assert.Equal(expected, b.Value);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "1_Properties",
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
