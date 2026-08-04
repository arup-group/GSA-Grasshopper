using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Parameters;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement3dGooTests {
    [Fact]
    public void GsaElement3dGooDrawViewportMeshesAndWiresTest() {
      GsaElement3dGoo output = GsaElement3dTest.CreateFromElementsFromMembers();
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(output);
    }

    [Fact]
    public void GsaElement3dGooCastToMeshTest() {
      GsaElement3dGoo output = GsaElement3dTest.CreateFromElementsFromMembers();

      var mesh = new GH_Mesh();
      Assert.True(output.CastTo(ref mesh));
      Assert.NotNull(mesh.Value);
    }
  }
}
