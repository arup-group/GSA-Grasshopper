using System.Drawing;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helper;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaArrowheadDiagramTests {
    public static GraphicDrawResult Element1dUniformLoadDiagramResults() {
      var graphic = new DiagramSpecification() {
        ListDefinition = "All",
        Type = GsaAPI.DiagramType.Load1dPatchForce,
        Cases = "A1",
        ScaleFactor = 1,
        IsNormalised = true,
        StructuralScale = 1,
      };

      var model = new GsaAPI.Model(GsaFile.SteelDesignSimple);
      return model.GetDiagrams(graphic);
    }

    [Fact]
    public void ConstructorTest() {
      GraphicDrawResult graphic = Element1dUniformLoadDiagramResults();
      var arrowhead = new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Empty);

      Assert.Equal("Arrowhead Diagram", arrowhead.TypeName);
      Assert.Equal("A GSA arrowhead diagram.", arrowhead.TypeDescription);
      Assert.Equal(graphic.Triangles.FirstOrDefault().Colour, arrowhead.Value.VertexColors.FirstOrDefault());
    }

    [Fact]
    public void CustomColorTest() {
      GraphicDrawResult graphic = Element1dUniformLoadDiagramResults();
      var arrowhead = new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Teal);

      Assert.Equal(Color.Teal.ToArgb(), arrowhead.Value.VertexColors.FirstOrDefault().ToArgb());
    }

    [Fact]
    public void GetBoundingBoxTest() {
      GraphicDrawResult graphic = Element1dUniformLoadDiagramResults();
      var arrowhead = new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Teal);
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      BoundingBox boundingBox = arrowhead.GetBoundingBox(transform);
      Assert.Equal(-6, boundingBox.Corner(true, true, true).X, 2);
      Assert.Equal(-6, boundingBox.Corner(true, true, true).Y, 2);
      Assert.Equal(1, boundingBox.Corner(true, true, true).Z, 2);
      Assert.Equal(15.5, boundingBox.Corner(false, false, false).X, 2);
      Assert.Equal(8, boundingBox.Corner(false, false, false).Y, 2);
      Assert.Equal(21, boundingBox.Corner(false, false, false).Z, 2);
    }

    [Fact]
    public void GetGeometryTest() {
      GraphicDrawResult graphic = Element1dUniformLoadDiagramResults();
      GeometryBase geometry = new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Teal).GetGeometry();
      Assert.NotNull(geometry);
    }
  }
}
