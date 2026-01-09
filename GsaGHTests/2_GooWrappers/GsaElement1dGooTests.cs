using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement1dGooTests {
    [Fact]
    public void GsaElement1dGooDrawViewportMeshesAndWiresTest() {
      var comp = (Section3dPreviewComponent)CreateElement1dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Fact]
    public void GsaElement1dGooCastToIntTest() {
      var comp = (Section3dPreviewComponent)CreateElement1dTests.ComponentMother();
      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp);

      var id = new GH_Integer();
      Assert.True(output.CastTo(ref id));
      Assert.Equal(0, id.Value);
    }
  }
}
