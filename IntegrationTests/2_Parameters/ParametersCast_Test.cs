using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]

  public class ParametersCast_Test {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Theory]
    [InlineData("List")]
    [InlineData("Model")]
    [InlineData("Bool6")]
    [InlineData("Material")]
    [InlineData("BucklingLengthFactors")]
    [InlineData("Offset")]
    [InlineData("Section")]
    [InlineData("Prop2d")]
    [InlineData("Prop3d")]
    [InlineData("SectionModifier")]
    [InlineData("Node")]
    [InlineData("Element1d")]
    [InlineData("Element2d")]
    [InlineData("Element3d")]
    [InlineData("Member1d")]
    [InlineData("Member2d")]
    [InlineData("Member3d")]
    [InlineData("GridPlaneSurface")]
    [InlineData("Load")]
    [InlineData("AnalysisCase", false)]
    [InlineData("AnalysisTask")]
    [InlineData("CombinationCase", false)]
    [InlineData("Result")]
    [InlineData("GridPlaneSurfaceFromLoad")]
    public void TestCast(string groupIdentifier, bool checkError = true) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        Assert.True(data.IsValid);
      }
      
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
      
      if (checkError) {
        TestCastError(groupIdentifier + "Error");
      }
    }

    private void TestCastError(string groupIdentifier) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        Assert.False(data.IsValid);
      }
      Assert.NotEmpty(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Theory]
    [InlineData("OffsetWarning")]
    [InlineData("Prop2dWarning")]
    public void TestCastWarning(string groupIdentifier) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        Assert.True(data.IsValid);
      }
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Theory]
    [InlineData("PropId")]
    [InlineData("NodePoint")]
    [InlineData("NodeId")]
    [InlineData("ElementLine")]
    [InlineData("ElementId")]
    [InlineData("Elem2dMesh")]
    [InlineData("Elem3dMesh")]
    [InlineData("Mem1dCrv")]
    [InlineData("MemberId")]
    [InlineData("Member2dBrep")]
    [InlineData("Member3dMesh")]
    [InlineData("MaterialFromProp")]
    [InlineData("MaterialFromGeo")]
    [InlineData("SectionFromGeo")]
    [InlineData("Prop2dFromGeo")]
    [InlineData("Prop3dFromGeo")]
    [InlineData("Plane")]
    [InlineData("LoadCaseId")]
    [InlineData("CaseId")]
    [InlineData("CrvFromGridLoad")]
    [InlineData("PtFromGridLoad")]
    [InlineData("SrfFromGridLoad")]
    [InlineData("BrepFromGridLoad")]
    [InlineData("GeoMorph")]
    [InlineData("GeoTransform")]
    [InlineData("LoadMorph")]
    [InlineData("LoadTransform")]
    public void TestCastTo(string groupIdentifier) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        Assert.True(data.IsValid);
      }
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
