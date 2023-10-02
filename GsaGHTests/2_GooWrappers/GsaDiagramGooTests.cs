using System.Drawing;
using System.Linq;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.GooWrappers;
using GsaGHTests.Helper;
using Rhino.Geometry;
using Xunit;
using DiagramType = GsaGH.Parameters.DiagramType;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaDiagramGooTests {

    [Fact]
    public void ArrowheadCastToTest() {
      GraphicDrawResult graphic = 
        GsaArrowheadDiagramTests.Element1dUniformLoadDiagramResults();
      var goo = new GsaDiagramGoo(new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Empty));

      GH_Mesh m = null;
      Assert.True(goo.CastTo(ref m));
      Assert.NotNull(m);

      double n = 0;
      Assert.False(goo.CastTo(ref n));
    }

    [Fact]
    public void DrawViewportMeshesAndWiresArrowheadTest() {
      GraphicDrawResult graphic =
        GsaArrowheadDiagramTests.Element1dUniformLoadDiagramResults();
      var goo = new GsaDiagramGoo(new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Empty));
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(goo);
    }
  }
}
