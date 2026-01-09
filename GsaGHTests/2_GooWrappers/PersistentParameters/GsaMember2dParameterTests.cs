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
  public class GsaMember2dParameterTests {
    [Fact]
    public void GsaMember2dParameterBakeTest() {
      var comp = (Section3dPreviewDropDownComponent)CreateMember2dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaMember2dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(14, doc.Objects.Count);
      doc.Dispose();
    }

    [Fact]
    public void GsaMember2dParameterPreferredCastTest() {
      GH_OasysComponent comp = CreateMember2dTests.ComponentMother();
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Brep brep = output.Value.Brep;

      var param = new GsaMember2dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, brep);

      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaMember2dParameterPreferredCastErrorTest() {
      int i = 0;
      var param = new GsaMember2dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
