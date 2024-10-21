using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.GooWrappers;

using Rhino.Geometry;
using Rhino.Geometry.Morphs;

using Xunit;

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

    [Fact]
    public void DrawViewportMeshesAndWiresLineTest() {
      GraphicDrawResult graphic = GsaLineDiagramTests.Element1dMyyDiagramResults();
      var goo = new GsaDiagramGoo(new GsaLineDiagram(graphic.Lines[1], 1, Color.Empty));
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(goo);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresVectorForceTest() {
      var vec = new GsaVectorDiagram(new Point3d(0, 0, 0), new Vector3d(0, 0, -1), false, Color.Teal);
      var goo = new GsaDiagramGoo(vec);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(goo);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresVectorMomentTest() {
      var vec = new GsaVectorDiagram(new Point3d(0, 0, 0), new Vector3d(0, 0, -1), true, Color.Teal);
      var goo = new GsaDiagramGoo(vec);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(goo);
    }

    [Fact]
    public void MorphArrowheadTest() {
      GraphicDrawResult graphic =
        GsaArrowheadDiagramTests.Element1dUniformLoadDiagramResults();
      var goo = new GsaDiagramGoo(new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Empty));
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(10, 10, 10), 10);
      var morphed = (GsaArrowheadDiagram)goo.Morph(morph);
      Assert.NotNull(morphed);
    }

    [Fact]
    public void MorphLineTest() {
      GraphicDrawResult graphic = GsaLineDiagramTests.Element1dMyyDiagramResults();
      var goo = new GsaDiagramGoo(new GsaLineDiagram(graphic.Lines[1], 1, Color.Empty));
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(10, 10, 10), 10);
      var morphed = (GsaLineDiagram)goo.Morph(morph);
      Assert.NotNull(morphed);
    }

    [Fact]
    public void TransformArrowheadTest() {
      GraphicDrawResult graphic =
        GsaArrowheadDiagramTests.Element1dUniformLoadDiagramResults();
      var goo = new GsaDiagramGoo(new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Empty));
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      var transformed = (GsaArrowheadDiagram)goo.Transform(transform);
      Assert.NotNull(transformed);
    }

    [Fact]
    public void TransformLineTest() {
      GraphicDrawResult graphic = GsaLineDiagramTests.Element1dMyyDiagramResults();
      var goo = new GsaDiagramGoo(new GsaLineDiagram(graphic.Lines[1], 1, Color.Empty));
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      var transformed = (GsaLineDiagram)goo.Transform(transform);
      Assert.NotNull(transformed);
    }

    [Fact]
    public void LineCastToTest() {
      GraphicDrawResult graphic = GsaLineDiagramTests.Element1dMyyDiagramResults();
      var goo = new GsaDiagramGoo(new GsaLineDiagram(graphic.Lines[1], 1, Color.Empty));

      GH_Line ln = null;
      Assert.True(goo.CastTo(ref ln));
      Assert.NotNull(ln);

      GH_Colour col = null;
      Assert.True(goo.CastTo(ref col));
      Assert.NotNull(col);

      double n = 0;
      Assert.False(goo.CastTo(ref n));
    }

    [Fact]
    public void VectorCastToTest() {
      var vec = new GsaVectorDiagram(new Point3d(0, 0, 0), new Vector3d(0, 0, -1), false, Color.Teal);
      var goo = new GsaDiagramGoo(vec);

      GH_Vector vector = null;
      Assert.True(goo.CastTo(ref vector));
      Assert.NotNull(vector);

      GH_Line ln = null;
      Assert.True(goo.CastTo(ref ln));
      Assert.NotNull(ln);

      GH_Point pt = null;
      Assert.True(goo.CastTo(ref pt));
      Assert.NotNull(pt);

      double n = 0;
      Assert.False(goo.CastTo(ref n));
    }
  }
}
