using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using Xunit;
using DiagramType = GsaGH.Parameters.DiagramType;

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

      Assert.Equal(DiagramType.ArrowHead, arrowhead.DiagramType);
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


  }
}
