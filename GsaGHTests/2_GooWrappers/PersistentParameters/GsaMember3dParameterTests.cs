using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember3dParameterTests {
    [Fact]
    public void GsaMember3dParameterBakeTest() {
      GH_OasysComponent comp = CreateMember3dTests.ComponentMother();
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaMember3dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }

    [Fact]
    public void GsaMember3dParameterPreferredCastMeshTest() {
      GH_OasysComponent comp = CreateMember3dTests.ComponentMother();
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);
      Mesh m = output.Value.SolidMesh;

      var param = new GsaMember3dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, m);

      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaMember3dParameterPreferredCastBrepTest() {
      Box box = Box.Empty;
      box.X = new Interval(0, 10);
      box.Y = new Interval(0, 10);
      box.Z = new Interval(0, 10);

      var param = new GsaMember3dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, box);

      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaMember3dParameterPreferredCastErrorTest() {
      int i = 0;
      var param = new GsaMember3dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
