using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;
using OasysGH.Components;
using OasysGH.Parameters;
using Rhino.Geometry;
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
