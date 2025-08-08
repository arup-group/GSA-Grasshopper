using System.IO;
using System.Linq;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class Section3dPreviewTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void DeformedMeshTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "DeformedMesh");
      Assert.NotNull(param);
      var valOut = (GH_Mesh)param.VolatileData.get_Branch(0)[0];
      Mesh mesh = valOut.Value;
      Assert.NotNull(mesh);
      Assert.InRange(mesh.Vertices.Count, 3450, 3455);
      Assert.InRange(mesh.Faces.Count, 4715, 4725);
    }

    [Fact]
    public void DeformedOutlinesTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "DeformedOutlines");
      Assert.NotNull(param);
      var valOut = param.VolatileData.AllData(false).ToList();
      Assert.NotNull(valOut);
      Assert.Equal(1852, valOut.Count);
    }

    [Fact]
    public void AnalysisLayerMeshTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "AnalysisLayerMesh");
      Assert.NotNull(param);
      var valOut = (GH_Mesh)param.VolatileData.get_Branch(0)[0];
      Mesh mesh = valOut.Value;
      Assert.NotNull(mesh);
      Assert.Equal(972, mesh.Vertices.Count);
      Assert.Equal(2132, mesh.Faces.Count);
    }

    [Fact]
    public void AnalysisLayerOutlinesTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "AnalysisLayerOutlines");
      Assert.NotNull(param);
      var valOut = param.VolatileData.AllData(false).ToList();
      Assert.NotNull(valOut);
      Assert.Equal(1468, valOut.Count);
    }

    [Fact]
    public void DesignLayerMeshTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "DesignLayerMesh");
      Assert.NotNull(param);
      var valOut = (GH_Mesh)param.VolatileData.get_Branch(0)[0];
      Mesh mesh = valOut.Value;
      Assert.NotNull(mesh);
      Assert.Equal(728, mesh.Vertices.Count);
      Assert.Equal(786, mesh.Faces.Count);
    }

    [Fact]
    public void DesignLayerOutlinesTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "DesignLayerOutlines");
      Assert.NotNull(param);
      var valOut = param.VolatileData.AllData(false).ToList();
      Assert.NotNull(valOut);
      Assert.Equal(300, valOut.Count);
    }

    [Fact]
    public void TransformedMember1dPreviewTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "TransformedMember1dPreview");
      Assert.NotNull(param);
      var valOut1 = (GH_Mesh)param.VolatileData.get_Branch(0)[0];
      Mesh mesh1 = valOut1.Value;
      Assert.NotNull(mesh1);
      Assert.Equal(80, mesh1.Vertices.Count);
      Assert.Equal(110, mesh1.Faces.Count);

      var valOut2 = (GH_Mesh)param.VolatileData.get_Branch(0)[1];
      Mesh mesh2 = valOut2.Value;
      Assert.NotNull(mesh2);
      Assert.Equal(80, mesh2.Vertices.Count);
      Assert.Equal(110, mesh2.Faces.Count);
    }

    [Fact]
    public void TransformedMember2dPreviewTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "TransformedMember2dPreview");
      Assert.NotNull(param);
      var valOut = (GH_Mesh)param.VolatileData.get_Branch(0)[0];
      Mesh mesh = valOut.Value;
      Assert.NotNull(mesh);
      Assert.Equal(8, mesh.Vertices.Count);
      Assert.Equal(6, mesh.Faces.Count);
    }

    [Fact]
    public void MorphedMember1dPreviewTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "MorphedMember1dPreview");
      Assert.NotNull(param);
      var valOut1 = (GH_Mesh)param.VolatileData.get_Branch(0)[0];
      Mesh mesh1 = valOut1.Value;
      Assert.NotNull(mesh1);
      Assert.Equal(80, mesh1.Vertices.Count);
      Assert.Equal(110, mesh1.Faces.Count);

      var valOut2 = (GH_Mesh)param.VolatileData.get_Branch(0)[1];
      Mesh mesh2 = valOut2.Value;
      Assert.NotNull(mesh2);
      Assert.Equal(80, mesh2.Vertices.Count);
      Assert.Equal(110, mesh2.Faces.Count);
    }

    [Fact]
    public void MorphedMember2dPreviewTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "MorphedMember2dPreview");
      Assert.NotNull(param);
      var valOut = (GH_Mesh)param.VolatileData.get_Branch(0)[0];
      Mesh mesh = valOut.Value;
      Assert.NotNull(mesh);
      Assert.Equal(8, mesh.Vertices.Count);
      Assert.Equal(6, mesh.Faces.Count);
    }

    [Fact]
    public void TestNoWarningOrErrors() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Tests", string.Empty);

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
