using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Parameters;

using Xunit;

namespace IntegrationTests.ExampleFiles {
  public sealed class MidSupportDocumentFixture : IDisposable {
    public GH_Document Document { get; }

    public MidSupportDocumentFixture(GrasshopperFixture grasshopper) {
      Document = Example5_4PointSrfMidSupport_Test.Document();
    }

    public void Dispose() {
      Document.Dispose();
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class Example5_4PointSrfMidSupport_Test : IClassFixture<MidSupportDocumentFixture> {
    private readonly GH_Document _document;

    public Example5_4PointSrfMidSupport_Test(MidSupportDocumentFixture fixture) {
      _document = fixture.Document;
    }

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
    public void MaxDeflectionTest() {
      IGH_Param param = Helper.FindParameter(_document, "RH_OUT:deflection");
      var output = (GH_Number)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(5.563436, output.Value, DoubleComparer.Default);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(_document, GH_RuntimeMessageLevel.Error);
    }

    [Fact]
    public void MeshResultGooTest() {
      IGH_Param param = Helper.FindParameter(_document, "ResultMesh");
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        var item = (MeshResultGoo)data;
        Assert.True(item.Value.IsValid);
        Assert.True(item.Boundingbox.IsValid);
        Assert.True(item.ClippingBox.IsValid);
        Assert.Equal("A GSA result mesh type.", item.TypeDescription);
        Assert.Equal("Result Mesh", item.TypeName);
      }
    }

    [Fact]
    public void PointResultGooTest() {
      IGH_Param param = Helper.FindParameter(_document, "ResultPoint");
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        var item = (PointResultGoo)data;
        Assert.True(item.Value.IsValid);
        Assert.True(item.Boundingbox.IsValid);
        Assert.True(item.ClippingBox.IsValid);
        Assert.Equal("A GSA result point type.", item.TypeDescription);
        Assert.Equal("Result Point", item.TypeName);
      }
    }

    [Theory]
    [InlineData("MorphMeshResult")]
    [InlineData("TransformMeshResult")]
    [InlineData("MorphPointResult")]
    [InlineData("TransformPointResult")]
    [InlineData("Mesh")]
    [InlineData("Point")]
    [InlineData("Colour")]
    [InlineData("Value")]
    [InlineData("NodeId")]
    public void TestCastTo(string groupIdentifier) {
      IGH_Param param = Helper.FindParameter(_document, groupIdentifier);
      foreach (IGH_Goo data in param.VolatileData.AllData(false)) {
        Assert.True(data.IsValid);
      }
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
