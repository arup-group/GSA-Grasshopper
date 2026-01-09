using System.Drawing;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helper;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaLineDiagramTests {
    public static GraphicDrawResult Element1dMyyDiagramResults() {
      var graphic = new DiagramSpecification() {
        ListDefinition = "All",
        Type = GsaAPI.DiagramType.MomentMyy,
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
      GraphicDrawResult graphic = Element1dMyyDiagramResults();
      var linediagram = new GsaLineDiagram(graphic.Lines[1], 1, Color.Empty);

      Assert.Equal("Line Diagram", linediagram.TypeName);
      Assert.Equal("A GSA line diagram.", linediagram.TypeDescription);
      Assert.Equal(graphic.Lines[1].Colour, linediagram.Color);
    }

    [Fact]
    public void CustomColorTest() {
      GraphicDrawResult graphic = Element1dMyyDiagramResults();
      var linediagram = new GsaLineDiagram(graphic.Lines[1], 1, Color.Teal);

      Assert.Equal(Color.Teal.ToArgb(), linediagram.Color.ToArgb());
    }

    [Fact]
    public void GetBoundingBoxTest() {
      GraphicDrawResult graphic = Element1dMyyDiagramResults();
      var arrowhead = new GsaLineDiagram(graphic.Lines[1], 1, Color.Teal);
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      BoundingBox boundingBox = arrowhead.GetBoundingBox(transform);
      Assert.Equal(1.375, boundingBox.Corner(true, true, true).X, 2);
      Assert.Equal(1, boundingBox.Corner(true, true, true).Y, 2);
      Assert.Equal(0.822, boundingBox.Corner(true, true, true).Z, 2);
      Assert.Equal(1.375, boundingBox.Corner(false, false, false).X, 2);
      Assert.Equal(1, boundingBox.Corner(false, false, false).Y, 2);
      Assert.Equal(1, boundingBox.Corner(false, false, false).Z, 2);
    }

    [Fact]
    public void GetGeometryTest() {
      GraphicDrawResult graphic = Element1dMyyDiagramResults();
      GeometryBase geometry = new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Teal).GetGeometry();
      Assert.NotNull(geometry);
    }
  }
}
