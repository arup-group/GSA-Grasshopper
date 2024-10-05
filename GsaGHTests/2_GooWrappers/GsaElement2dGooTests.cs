using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;

using Rhino.Geometry;
using Rhino.Geometry.Morphs;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement2dGooTests {
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GsaElement2dGooDrawViewportMeshesAndWiresTest(bool isLoadPanel) {
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(isLoadPanel, isLoadPanel);
      comp.Preview3dSection = true;
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetGeometryTest(bool isLoadPanel) {
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(isLoadPanel, isLoadPanel);
      comp.Preview3dSection = true;
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output.GetGeometry());
    }

    [Fact]
    public void GsaElement2dGooCastToMeshTest() {
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      var mesh = new GH_Mesh();
      Assert.True(output.CastTo(ref mesh));
      Assert.True(mesh.IsValid);
    }

    [Fact]
    public void GsaElement2dGooCastToCurveTest() {
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMotherLoadPanel();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      var curve = new GH_Curve();
      Assert.True(output.CastTo(ref curve));
      Assert.True(curve.IsValid);
    }

    [Fact]
    public void MorphMeshTest() {
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(2, 0, 0), 3);
      var morphed = (GsaElement2dGoo)output.Morph(morph);
      Assert.NotNull(morphed);
      Assert.Equal(1.5, morphed.Value.Mesh.Vertices[1].X);
    }

    [Fact]
    public void MorphCurveTest() {
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMotherLoadPanel();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(2, 0, 0), 3);
      var morphed = (GsaElement2dGoo)output.Morph(morph);
      var polyline = new Rhino.Geometry.Polyline();
      morphed.Value.Curve.TryGetPolyline(out polyline);
      Assert.NotNull(morphed);
      Assert.Equal(1.5, morphed.Value.Curve.PointAt(1).X);
    }
  }
}
