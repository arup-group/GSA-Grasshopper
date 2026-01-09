using System;
using System.Collections.Generic;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement1dParameterTests {
    [Fact]
    public void GsaElement1dParameterBakeTest() {
      var comp = (Section3dPreviewComponent)CreateElement1dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp);

      var param = new GsaElement1dParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(26, doc.Objects.Count);
      doc.Dispose();
    }
  }
}
