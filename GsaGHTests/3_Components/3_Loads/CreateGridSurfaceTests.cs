using System;
using GsaAPI;
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
      ComponentTestHelper.SetInput(comp, "all", 2);
      ComponentTestHelper.SetInput(comp, "test", 3);
      ComponentTestHelper.SetInput(comp, 99, 4);
      ComponentTestHelper.SetInput(comp, 0.5, 5);
      ComponentTestHelper.SetInput(comp, Math.PI, 6);

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      GsaGridPlaneSurface gridSurface = output.Value;

      Assert.Equal(42, gridSurface.GridPlaneId);
      Assert.Equal("10", gridSurface.Elevation);
      Assert.Equal("test", gridSurface.GridPlane.Name);
      Assert.Equal(42, gridSurface.GridSurfaceId);
      Assert.Equal("test", gridSurface.GridSurface.Name);
      Assert.Equal("99", gridSurface.Tolerance);
      Assert.Equal(Span_Type.ONE_WAY, gridSurface.GridSurface.SpanType);
      Assert.Equal(Math.PI, gridSurface.GridSurface.Direction);
    }

    [Theory]
    [InlineData(GridSurfaceExpansionType.PLANE_CORNER, true)]
    [InlineData(GridSurfaceExpansionType.PLANE_SMOOTH, true)]
    [InlineData(GridSurfaceExpansionType.PLANE_ASPECT, true)]
    [InlineData(GridSurfaceExpansionType.LEGACY, true)]
    [InlineData(GridSurfaceExpansionType.PLANE_CORNER, false)]
    [InlineData(GridSurfaceExpansionType.PLANE_SMOOTH, false)]
    [InlineData(GridSurfaceExpansionType.PLANE_ASPECT, false)]
    [InlineData(GridSurfaceExpansionType.LEGACY, false)]
    public void Create1dTwoWaySpanTest(GridSurfaceExpansionType expansionType,  bool simplify) {
      GH_OasysDropDownComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, "all", 2);
      ComponentTestHelper.SetInput(comp, "myGridSurface", 3);
      ComponentTestHelper.SetInput(comp, "10mm", 4);
      ComponentTestHelper.SetInput(comp, expansionType, 5);
      ComponentTestHelper.SetInput(comp, simplify, 6);

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      GsaGridPlaneSurface gridSurface = output.Value;

      Assert.Equal("0", gridSurface.Elevation);
      Assert.Equal(42, gridSurface.GridSurfaceId);
      Assert.Equal("myGridSurface", gridSurface.GridSurface.Name);
      Assert.Equal("10mm", gridSurface.Tolerance);
      Assert.Equal(Span_Type.TWO_WAY, gridSurface.GridSurface.SpanType);
      Assert.Equal(expansionType, gridSurface.GridSurface.ExpansionType);
      //Assert.Equal(simplify, gridSurface.GridSurface.);
    }

    [Fact]
    public void Create2dTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, "all", 2);
      ComponentTestHelper.SetInput(comp, "myGridSurface", 3);
      ComponentTestHelper.SetInput(comp, "10mm", 4);

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      GsaGridPlaneSurface gridSurface = output.Value;

      Assert.Equal("0", gridSurface.Elevation);
      Assert.Equal(42, gridSurface.GridSurfaceId);
      Assert.Equal("myGridSurface", gridSurface.GridSurface.Name);
      Assert.Equal("10mm", gridSurface.Tolerance);
      Assert.Equal(Span_Type.ONE_WAY, gridSurface.GridSurface.SpanType);
    }

    [Fact]
    public void EntityListTypeErrorTest() {
      var comp = new CreateGridSurface();
      ComponentTestHelper.SetInput(comp, "little", 4);

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
    }

  }
}
