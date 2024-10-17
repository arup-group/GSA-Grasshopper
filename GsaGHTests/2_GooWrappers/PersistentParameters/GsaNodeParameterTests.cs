using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaNodeParameterTests {
    [Fact]
    public void GsaNodeParameterFixBakeTest() {
      var comp = (CreateSupport)CreateSupportTests.ComponentMother();
      comp.SetRestraints(true, true, true, true, true, true);
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaNodeParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(2, doc.Objects.Count);
      doc.Dispose();
    }

    [Fact]
    public void GsaNodeParameterPinBakeTest() {
      var comp = (CreateSupport)CreateSupportTests.ComponentMother();
      comp.SetRestraints(true, true, true, false, false, false);
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaNodeParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(2, doc.Objects.Count);
      doc.Dispose();
    }

    [Fact]
    public void GsaNodeParameterTextBakeTest() {
      var comp = (CreateSupport)CreateSupportTests.ComponentMother();
      comp.SetRestraints(true, false, true, false, true, false);
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaNodeParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(2, doc.Objects.Count);
      doc.Dispose();
    }

    [Fact]
    public void GsaNodeParameterPreferredCastTest() {
      var pt = new Point3d(1, 2, 3);

      var param = new GsaNodeParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, pt);

      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaNodeParameterPreferredCastErrorTest() {
      int i = 0;
      var param = new GsaNodeParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
