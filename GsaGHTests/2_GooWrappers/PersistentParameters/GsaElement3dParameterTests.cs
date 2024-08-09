using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement3dParameterTests {
    [Fact]
    public void GsaElement3dParameterBakeTest() {
      GsaElement3dGoo output = GsaElement3dTest.CreateFromElementsFromMembers();

      var param = new GsaElement3dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }

    [Fact]
    public void GsaElement3dParameterPreferredCastTest() {
      GsaElement3dGoo e3dGoo = GsaElement3dTest.CreateFromElementsFromMembers();
      Mesh m = e3dGoo.Value.NgonMesh;

      var param = new GsaElement2dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, m);

      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaElement3dParameterPreferredCastErrorTest() {
      int i = 0;
      var param = new GsaElement3dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GsaElement3dParameterPreferredCastErrorFromMeshTest() {
      GH_OasysComponent comp = CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      Mesh m = output.Value.Mesh;
      var param = new GsaElement3dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, m);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
