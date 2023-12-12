﻿using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class ExpandBeamToShellTest {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error, "Error");
    }

    [Fact]
    public void NoRuntimeWarningTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData("CAT", 0)]
    [InlineData("A", 0)]
    [InlineData("CH", 0)]
    [InlineData("CHS", 0)]
    [InlineData("X", 0)]
    [InlineData("OVAL", 0)]
    [InlineData("GC", 0)]
    [InlineData("GZ", 0)]
    [InlineData("GI", 0)]
    [InlineData("CB", 0)]
    [InlineData("I", 0)]
    [InlineData("RHS", 0)]
    [InlineData("T", 0)]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Fact]
    public void TestError() {
      GH_Component component = Helper.FindComponent(Document, "Error");
      component.Params.Output[0].CollectData();
      Assert.NotEmpty(component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "2_Geometry",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
