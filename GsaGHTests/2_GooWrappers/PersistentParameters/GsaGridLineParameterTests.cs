using System;
using System.Collections.Generic;

using Grasshopper.Kernel.Data;

using GsaGH.Parameters;

using GsaGHTests.Model;

using Rhino;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineParameterTests {
    private CreateGridLineTestHelper _helper;

    public GsaGridLineParameterTests() {
      _helper = new CreateGridLineTestHelper();
    }

    [Fact]
    public void GsaGridLineParameterLineBakeTest() {
      _helper.CreateComponentWithLineInput();
      GsaGridLineGoo output = _helper.GetGridLineOutput();

      var param = new GsaGridLineParameter();
      param.AddVolatileData(new GH_Path(0), 0, output);

      var doc = RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }

    [Fact]
    public void GsaGridLineParameterArcBakeTest() {
      _helper.CreateComponentWithArcInput();
      GsaGridLineGoo output = _helper.GetGridLineOutput();

      var param = new GsaGridLineParameter();
      param.AddVolatileData(new GH_Path(0), 0, output);

      var doc = RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }
  }
}
