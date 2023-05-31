﻿using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateGridPlaneTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateGridPlane();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, 10, 2);
      ComponentTestHelper.SetInput(comp, "test", 3);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      GsaGridPlaneSurface gridPlane = output.Value;

      Assert.Equal(42, gridPlane.GridPlaneId);
      Assert.Equal("10", gridPlane.Elevation);
      Assert.Equal("test", gridPlane.GridPlane.Name);
    }
  }
}
