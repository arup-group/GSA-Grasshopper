using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement2dParameterTests {
    [Fact]
    public void GsaElement2dParameterBakeTest() {
      var comp = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaElement2dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(10, doc.Objects.Count);
    }

    [Fact]
    public void GsaElement2dParameterPreferredCastTest() {
      GH_OasysComponent comp = CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      Mesh m = output.Value.Mesh;

      var param = new GsaElement2dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, m);

      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaElement2dParameterPreferredCastErrorTest() {
      int i = 0;
      var param = new GsaElement2dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
