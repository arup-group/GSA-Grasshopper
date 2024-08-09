using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaMember1dParameterTests {
    [Fact]
    public void GsaMember1dParameterBakeTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember1dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaMember1dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(26, doc.Objects.Count);
      doc.Dispose();
    }

    [Fact]
    public void GsaMember1dParameterPreferredCastTest() {
      GH_OasysComponent comp = CreateMember1dTests.ComponentMother();
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
      Curve crv = output.Value.PolyCurve;

      var param = new GsaMember1dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, crv);

      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaMember1dParameterPreferredCastErrorTest() {
      int i = 0;
      var param = new GsaMember1dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
