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
  public class GsaElement2dParameterTests {
    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 10)]
    public void GsaElement2dParameterBakeTest(bool isLoadpanel, int expectedObjectCount) {
      var component = (Section3dPreviewComponent)CreateElement2dTests.ComponentMother(isLoadpanel);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);

      var parameter = new GsaElement2dParameter();
      parameter.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);

      var document = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      parameter.BakeGeometry(document, guids);
      Assert.NotEmpty(guids);
      Assert.Equal(expectedObjectCount, document.Objects.Count);
      document.Dispose();
    }

    [Fact]
    public void GsaElement2dParameterPreferredCastTest() {
      GH_OasysComponent component = CreateElement2dTests.ComponentMother();
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      Mesh m = output.Value.Mesh;

      var parameter = new GsaElement2dParameter();
      parameter.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, m);

      Assert.NotNull(parameter.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaElement2dParameterPreferredCastErrorTest() {
      int i = 0;
      var parameter = new GsaElement2dParameter();
      parameter.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(parameter.VolatileData.AllData(false).First().IsValid);
      Assert.Single(parameter.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
