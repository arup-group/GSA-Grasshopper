using System.Collections.Generic;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement3dTest {
    public static GsaElement3dGoo CreateFromElementsFromMembers() {
      GH_OasysComponent m3d = CreateMember3dTests.ComponentMother();
      var elemFromMem = new CreateElementsFromMembers();
      elemFromMem.CreateAttributes();
      ComponentTestHelper.SetInput(elemFromMem,
        ComponentTestHelper.GetOutput(CreateMember3dTests.ComponentMother()),
        3);
      return (GsaElement3dGoo)ComponentTestHelper.GetOutput(elemFromMem, 3);
    }

    [Fact]
    public void DuplicateTest() {
      GsaElement3d original = CreateFromElementsFromMembers().Value;

      var duplicate = new GsaElement3d(original);

      Duplicates.AreEqual(original, duplicate, new List<string> { "Guid" });
    }

    [Fact]
    public void CreateElement3dFromMeshShouldSetValues() {
      var mesh = new Mesh();
      var element = new GsaElement3d(mesh);

      Assert.NotNull(element.NgonMesh);
      Assert.False(element.NgonMesh.IsClosed);
      Assert.Empty(element.Ids);
      Assert.Empty(element.ApiElements);
      Assert.Empty(element.Topology);
      Assert.Empty(element.TopoInt);
      Assert.Empty(element.FaceInt);
    }
  }
}
