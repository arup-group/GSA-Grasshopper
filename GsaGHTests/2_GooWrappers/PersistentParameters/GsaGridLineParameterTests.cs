using System;
using System.Collections.Generic;

using GsaGH.Parameters;

using GsaGHTests.Helpers;
using GsaGHTests.Model;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineParameterTests {
    [Fact]
    public void GsaGridLineParameterLineBakeTest() {
      GH_OasysComponent comp = CreateGridLineTest.GridLineComponentMother();
      var output = (GsaGridLineGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaGridLineParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }

    [Fact]
    public void GsaGridLineParameterArcBakeTest() {
      GH_OasysComponent comp = CreateGridLineTest.GridArcComponentMother();
      var output = (GsaGridLineGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaGridLineParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }
  }
}
