using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember1dGooTests {
    [Fact]
    public void GsaMember1dGooDrawViewportMeshesAndWiresTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember1dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Fact]
    public void GsaMember1dGooCastToIntTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember1dTests.ComponentMother();
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);

      var id = new GH_Integer();
      Assert.True(output.CastTo(ref id));
      Assert.Equal(0, id.Value);
    }

    [Fact]
    public void GsaMember1dGooCastToCurveTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember1dTests.ComponentMother();
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);

      var crv = new GH_Curve();
      Assert.True(output.CastTo(ref crv));
      Assert.True(crv.IsValid);
    }

    [Fact]
    public void GsaMember1dGooCastToMeshTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember1dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);

      var m = new GH_Mesh();
      Assert.True(output.CastTo(ref m));
      Assert.True(m.IsValid);
    }
  }
}
