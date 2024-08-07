using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateElementsFromMembersTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateElementsFromMembers();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, (GsaNodeGoo)ComponentTestHelper.GetOutput(
        CreateSupportTests.ComponentMother()), 0);
      ComponentTestHelper.SetInput(comp, (GsaMember1dGoo)ComponentTestHelper.GetOutput(
        CreateMember1dTests.ComponentMother()), 1);
      ComponentTestHelper.SetInput(comp, (GsaMember2dGoo)ComponentTestHelper.GetOutput(
        CreateMember2dTests.ComponentMother()), 2);
      ComponentTestHelper.SetInput(comp, (GsaMember3dGoo)ComponentTestHelper.GetOutput(
        CreateMember3dTests.ComponentMother()), 3);
      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();
      var node = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 0);
      var e1d = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 1);
      var e2d = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp, 2);
      var e3d = (GsaElement3dGoo)ComponentTestHelper.GetOutput(comp, 3);

      Assert.NotNull(node);
      Assert.Equal(1, node.Value.Id);
      Assert.Equal(0, node.Value.Point.X);
      Assert.Equal(-1, node.Value.Point.Y);
      Assert.Equal(0, node.Value.Point.Z);

      Assert.NotNull(e1d);
      Assert.Equal(1, e1d.Value.Id);
      Assert.Equal(0, e1d.Value.Line.PointAtStart.X);
      Assert.Equal(-1, e1d.Value.Line.PointAtStart.Y);
      Assert.Equal(0, e1d.Value.Line.PointAtStart.Z);

      Assert.NotNull(e2d);
      Assert.Equal(400, e2d.Value.Ids.Count);
      Assert.Equal(400, e2d.Value.Mesh.Faces.Count);

      Assert.NotNull(e3d);
      Assert.Equal(8000, e3d.Value.Ids.Count);
      Assert.Equal(48000, e3d.Value.NgonMesh.Faces.Count);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      GH_OasysComponent comp = ComponentMother();
      var node = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 0);
      var e1d = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 1);
      var e2d = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp, 2);
      var e3d = (GsaElement3dGoo)ComponentTestHelper.GetOutput(comp, 3);

      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
