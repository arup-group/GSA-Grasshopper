using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGHTests.Components.Geometry;
using GsaGH.Parameters;

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
      var component = CreateElement2dTests.ComponentMother(isLoadPanel);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true,false)]
    [InlineData(false,true)]
    [InlineData(false, false)]
    public void GetGeometryTest(bool isLoadPanel, bool preview3dSection) {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(isLoadPanel);
      component.Preview3dSection = preview3dSection;
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      Assert.NotNull(output.GetGeometry());
    }

    [Fact]
    public void GsaElement2dGooCastToMeshTest() {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      var mesh = new GH_Mesh();
      Assert.True(output.CastTo(ref mesh));
      Assert.True(mesh.IsValid);
    }

    [Fact]
    public void GsaElement2dGooCastToCurveTest() {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(true);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      var curve = new GH_Curve();
      Assert.True(output.CastTo(ref curve));
      Assert.True(curve.IsValid);
    }

    [Fact]
    public void StretchingASection3dShouldOutputAValidMesh() {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(2, 0, 0), 3);
      var morphed = (GsaElement2dGoo)output.Morph(morph);
      Assert.NotNull(morphed);
      Assert.Equal(1.5, morphed.Value.Mesh.Vertices[1].X);
    }

    [Fact]
    public void StretchingASection3dShouldOutputAValidCurve() {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(true);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(2, 0, 0), 3);
      var morphed = (GsaElement2dGoo)output.Morph(morph);
      Assert.NotNull(morphed);
      Assert.Equal(1.5, morphed.Value.Curve.PointAt(1).X);
    }

    [Fact]
    public void TransformingASection3dShouldOutputAValidCurve() {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(true,true);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      var transformed = (GsaElement2dGoo)output.Transform(Transform.Translation(1, 1, 1));
      Assert.NotNull(transformed);
      for(int i=0;i<4;i++) {
        Assert.Equal(output.Value.Curve.PointAt(i).X + 1, transformed.Value.Curve.PointAt(i).X);
        Assert.Equal(output.Value.Curve.PointAt(i).Y + 1, transformed.Value.Curve.PointAt(i).Y);
        Assert.Equal(output.Value.Curve.PointAt(i).Z + 1, transformed.Value.Curve.PointAt(i).Z);
      }
    }

    [Fact]
    public void TransformingASection3dShouldOutputAValidMesh() {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      var transformed = (GsaElement2dGoo)output.Transform(Transform.Translation(1, 1, 1));
      Assert.NotNull(transformed);
      for (int i = 0; i < output.Value.Mesh.Vertices.Count; i++) {
        Assert.Equal(output.Value.Mesh.Vertices[i].X + 1, transformed.Value.Mesh.Vertices[i].X);
        Assert.Equal(output.Value.Mesh.Vertices[i].Y + 1, transformed.Value.Mesh.Vertices[i].Y);
        Assert.Equal(output.Value.Mesh.Vertices[i].Z + 1, transformed.Value.Mesh.Vertices[i].Z);
      }
    }
  }
}
