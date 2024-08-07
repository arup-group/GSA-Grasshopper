using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember3dGooTests {
    [Fact]
    public void GsaMember3dGooDrawViewportMeshesAndWiresTest() {
      GH_OasysComponent comp = CreateMember3dTests.ComponentMother();
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Fact]
    public void GsaMember3dGooCastToIntTest() {
      GH_OasysComponent comp = CreateMember3dTests.ComponentMother();
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);

      var id = new GH_Integer();
      Assert.True(output.CastTo(ref id));
      Assert.Equal(0, id.Value);
    }

    [Fact]
    public void GsaMember3dGooCastToMeshTest() {
      GH_OasysComponent comp = CreateMember3dTests.ComponentMother();
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);

      var m = new GH_Mesh();
      Assert.True(output.CastTo(ref m));
      Assert.True(m.IsValid);
    }
  }
}
