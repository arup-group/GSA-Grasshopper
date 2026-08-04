using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaNodeGooTests {
    [Fact]
    public void GsaNodeGooDrawViewportMeshesAndWiresTest() {
      GH_OasysComponent comp = CreateSupportTests.ComponentMother();
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Fact]
    public void GsaNodeGooCastToIntTest() {
      GH_OasysComponent comp = CreateSupportTests.ComponentMother();
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);

      var id = new GH_Integer();
      Assert.True(output.CastTo(ref id));
      Assert.Equal(0, id.Value);
    }

    [Fact]
    public void GsaNodeGooCastToPointTest() {
      GH_OasysComponent comp = CreateSupportTests.ComponentMother();
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);

      var pt = new GH_Point();
      Assert.True(output.CastTo(ref pt));
      Assert.Equal(0, pt.Value.X);
      Assert.Equal(-1, pt.Value.Y);
      Assert.Equal(0, pt.Value.Z);
    }
  }
}
