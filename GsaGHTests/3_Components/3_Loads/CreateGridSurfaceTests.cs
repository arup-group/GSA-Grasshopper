using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static GsaAPI.GridSurface;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class GridSurfaceTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateGridSurface();
      comp.CreateAttributes();

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var gridPlane
        = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(
          CreateGridPlaneTests.ComponentMother());

      ComponentTestHelper.SetInput(comp, gridPlane, 0);
      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, "test", 3);
      ComponentTestHelper.SetInput(comp, 99, 4);
      ComponentTestHelper.SetInput(comp, 0.5, 5);

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      GsaGridPlaneSurface gridSurface = null;
      output.CastTo(ref gridSurface);

      Assert.Equal(42, gridSurface.GridPlaneId);
      Assert.Equal("10", gridSurface.Elevation);
      Assert.Equal("test", gridSurface.GridPlane.Name);
      Assert.Equal(42, gridSurface.GridSurfaceId);
      Assert.Equal("test", gridSurface.GridSurface.Name);
      Assert.Equal("99", gridSurface.Tolerance);
      Assert.Equal(Span_Type.ONE_WAY, gridSurface.GridSurface.SpanType);
    }
  }
}
