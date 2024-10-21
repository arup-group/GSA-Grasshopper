using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using Xunit;

namespace IntegrationTests.ExampleFiles {
  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class Example2_Reciprocal_Test {

    public static GH_Document Document() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = "GsaGH_" + thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void MaxMyyAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "MaxMyy");
      var output = (GH_Number)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(-157.193672, output.Value, 6);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }

    [Fact]
    public void SumLoadForceAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "SumLoadForce");
      var output = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
      Assert.True(output.Value);
    }

    [Fact]
    public void VectorResultGooTest() {
      IGH_Param param = Helper.FindParameter(Document(), "ReactionForceVector");
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        var item = (GsaDiagramGoo)data;
        Assert.True(item.IsValid);
        Assert.True(item.Boundingbox.IsValid);
        Assert.True(item.ClippingBox.IsValid);
        Assert.Equal("GSA Diagram Parameter", item.TypeDescription);
        Assert.Equal("Diagram", item.TypeName);
      }
    }

    [Theory]
    [InlineData("MorphVectorResult")]
    [InlineData("TransformVectorResult")]
    [InlineData("MorphLineResult")]
    [InlineData("TransformLineResult")]
    [InlineData("Line")]
    [InlineData("Curve")]
    [InlineData("CaseId")]
    [InlineData("Vector")]
    public void TestCastTo(string groupIdentifier) {
      IGH_Param param = Helper.FindParameter(Document(), groupIdentifier);
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        Assert.True(data.IsValid);
      }

      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
