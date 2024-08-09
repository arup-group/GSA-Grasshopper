using System;

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
      ComponentTestHelper.SetInput(comp, Math.PI, 5);

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      GsaGridPlaneSurface gridSurface = output.Value;

      Assert.Equal(42, gridSurface.GridPlaneId);
      Assert.Equal("10", gridSurface.Elevation);
      Assert.Equal("test", gridSurface.GridPlane.Name);
      Assert.Equal(42, gridSurface.GridSurfaceId);
      Assert.Equal("test", gridSurface.GridSurface.Name);
      Assert.Equal("99", gridSurface.Tolerance);
      Assert.Equal(Span_Type.ONE_WAY, gridSurface.GridSurface.SpanType);
      Assert.Equal(180, gridSurface.GridSurface.Direction);
    }

    [Theory]
    [InlineData(0, 3, true, 1)]
    [InlineData(1, 2, true, 1)]
    [InlineData(2, 1, true, 1)]
    [InlineData(3, 0, true, 1)]
    [InlineData(0, 3, false, 2)]
    [InlineData(1, 2, false, 2)]
    [InlineData(2, 1, false, 2)]
    [InlineData(3, 0, false, 2)]
    public void Create1dTwoWaySpanTest(int expansionType, int expectedExpansionType, bool simplify, int expectedSpanType) {
      GH_OasysDropDownComponent comp = ComponentMother();
      comp.CreateAttributes();
      comp.SetSelected(0, 1); // 1D, Two-way span
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
      Assert.Equal(expectedExpansionType, (int)gridSurface.GridSurface.ExpansionType);
      Assert.Equal(expectedSpanType, (int)gridSurface.GridSurface.SpanType);
    }

    [Fact]
    public void Create2dTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      comp.CreateAttributes();
      comp.SetSelected(0, 2); // 2D
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
      Assert.Equal(Span_Type.TWO_WAY, gridSurface.GridSurface.SpanType);
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

    [Fact]
    public void DirectionWarningTest() {
      var comp = new CreateGridSurface();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, 2 * Math.PI, 5);

      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
    }
  }
}
