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
    private readonly GsaElement3d _element3d;

    public GsaElement3dTest() {
      var mesh = new Mesh();
      _element3d = new GsaElement3d(mesh);
    }

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
    public void CreateElement3dFromMeshShouldSetNgonMesh() {
      Assert.NotNull(_element3d.NgonMesh);
    }

    [Fact]
    public void CreateElement3dFromMeshShouldSetNgonMeshToOpen() {
      Assert.False(_element3d.NgonMesh.IsClosed);
    }

    [Fact]
    public void CreateElement3dFromMeshShouldSetIdsToEmptyList() {
      Assert.Empty(_element3d.Ids);
    }

    [Fact]
    public void CreateElement3dFromMeshShouldSetApiElementsToEmptyList() {
      Assert.Empty(_element3d.ApiElements);
    }

    [Fact]
    public void CreateElement3dFromMeshShouldSetTopologyToEmptyList() {
      Assert.Empty(_element3d.Topology);
    }

    [Fact]
    public void CreateElement3dFromMeshShouldSetTopoIntToEmptyList() {
      Assert.Empty(_element3d.TopoInt);
    }

    [Fact]
    public void CreateElement3dFromMeshShouldSetFaceIntToEmptyList() {
      Assert.Empty(_element3d.FaceInt);
    }
  }
}
