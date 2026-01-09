using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember2dGooTests {
    [Fact]
    public void GsaMember2dGooDrawViewportMeshesAndWiresTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember2dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Fact]
    public void GsaMember2dGooCastToIntTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember2dTests.ComponentMother();
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);

      var id = new GH_Integer();
      Assert.True(output.CastTo(ref id));
      Assert.Equal(0, id.Value);
    }

    [Fact]
    public void GsaMember2dGooCastToCurveTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember2dTests.ComponentMother();
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);

      var crv = new GH_Curve();
      Assert.True(output.CastTo(ref crv));
      Assert.True(crv.IsValid);
    }

    [Fact]
    public void GsaMember2dGooCastToBrepTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember2dTests.ComponentMother();
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);

      var brep = new GH_Brep();
      Assert.True(output.CastTo(ref brep));
      Assert.True(brep.IsValid);
    }

    [Fact]
    public void GsaMember2dGooCastToMeshTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember2dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);

      var m = new GH_Mesh();
      Assert.True(output.CastTo(ref m));
      Assert.True(m.IsValid);
    }
  }
}
