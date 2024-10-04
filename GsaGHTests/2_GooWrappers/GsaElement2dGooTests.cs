using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

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
      bool isLoadPanel = true;
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(isLoadPanel, isLoadPanel);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      var curve = new GH_Curve();
      Assert.True(output.CastTo(ref curve));
      Assert.True(curve.IsValid);
    }
  }
}
