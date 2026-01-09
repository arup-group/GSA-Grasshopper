using System;

using Grasshopper.Kernel.Parameters;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

using static GsaAPI.GridSurface;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class GridSurfaceTests {
    private readonly GH_OasysDropDownComponent _component;

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateGridSurface();
      comp.CreateAttributes();
      return comp;
    }

    public GridSurfaceTests() { _component = ComponentMother(); }

    [Fact]
    public void CreateComponentTest() {
      var gridPlane = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(CreateGridPlaneTests.ComponentMother());

      ComponentTestHelper.SetInput(_component, gridPlane, 0);
      ComponentTestHelper.SetInput(_component, 42, 1);
      ComponentTestHelper.SetInput(_component, "all", 2);
      ComponentTestHelper.SetInput(_component, "test", 3);
      ComponentTestHelper.SetInput(_component, 99, 4);
      ComponentTestHelper.SetInput(_component, Math.PI, 5);

      GsaGridPlaneSurface gridSurface = GetComponentOutput().Value;

      Assert.Equal("all", gridSurface.GridSurface.Elements);
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
    public void Create1dTwoWaySpanTest(
      int expansionType, int expectedExpansionType, bool simplify, int expectedSpanType) {
      _component.CreateAttributes();
      _component.SetSelected(0, 1); // 1D, Two-way span
      ComponentTestHelper.SetInput(_component, 42, 1);
      ComponentTestHelper.SetInput(_component, "1 to 10", 2);
      ComponentTestHelper.SetInput(_component, "myGridSurface", 3);
      ComponentTestHelper.SetInput(_component, "10mm", 4);
      ComponentTestHelper.SetInput(_component, expansionType, 5);
      ComponentTestHelper.SetInput(_component, simplify, 6);

      GsaGridPlaneSurface gridSurface = GetComponentOutput().Value;

      Assert.Equal("1 to 10", gridSurface.GridSurface.Elements);
      Assert.Equal("0", gridSurface.Elevation);
      Assert.Equal(42, gridSurface.GridSurfaceId);
      Assert.Equal("myGridSurface", gridSurface.GridSurface.Name);
      Assert.Equal("10mm", gridSurface.Tolerance);
      Assert.Equal(expectedExpansionType, (int)gridSurface.GridSurface.ExpansionType);
      Assert.Equal(expectedSpanType, (int)gridSurface.GridSurface.SpanType);
    }

    [Fact]
    public void Create2dTest() {
      _component.CreateAttributes();
      _component.SetSelected(0, 2); // 2D
      ComponentTestHelper.SetInput(_component, 42, 1);
      ComponentTestHelper.SetInput(_component, "PA1", 2);
      ComponentTestHelper.SetInput(_component, "myGridSurface", 3);
      ComponentTestHelper.SetInput(_component, "10mm", 4);

      GsaGridPlaneSurface gridSurface = GetComponentOutput().Value;
      //PA1 = all 2d element having property 1
      Assert.Equal("PA1", gridSurface.GridSurface.Elements);
      Assert.Equal("0", gridSurface.Elevation);
      Assert.Equal(42, gridSurface.GridSurfaceId);
      Assert.Equal("myGridSurface", gridSurface.GridSurface.Name);
      Assert.Equal("10mm", gridSurface.Tolerance);
      Assert.Equal(Span_Type.TWO_WAY, gridSurface.GridSurface.SpanType);
    }

    [Fact]
    public void EntityListTypeErrorTest() {
      ComponentTestHelper.SetInput(_component, "little", 4);

      _ = GetComponentOutput();
      Assert.Single(_component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void DirectionWarningTest() {
      _component.CreateAttributes();
      ComponentTestHelper.SetInput(_component, 2 * Math.PI, 5);

      _ = GetComponentOutput();
      _component.Params.Output[0].ExpireSolution(true);
      _component.Params.Output[0].CollectData();
      Assert.Single(_component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void GridSurfaceLoadableObjectIsPassedAndRead() {
      ComponentTestHelper.SetInput(_component, "G6", 2);

      GsaGridPlaneSurface gridSurface = GetComponentOutput().Value;

      Assert.Equal("G6", gridSurface.GridSurface.Elements);
    }

    [Fact]
    public void GridSurfaceDirectionShouldAlwaysConvertToDegreesFromRadians() {
      var angleParameter = _component.Params.Input[5] as Param_Number;
      angleParameter.UseDegrees = false;
      ComponentTestHelper.SetInput(_component, Math.PI, 5);

      GsaGridPlaneSurface gridSurface = GetComponentOutput().Value;

      Assert.Equal(180, gridSurface.GridSurface.Direction);
    }

    [Fact]
    public void GridSurfaceDirectionShouldAlwaysConvertToDegreesFromDegrees() {
      var angleParameter = _component.Params.Input[5] as Param_Number;
      angleParameter.UseDegrees = true;
      ComponentTestHelper.SetInput(_component, 180, 5);

      GsaGridPlaneSurface gridSurface = GetComponentOutput().Value;

      Assert.Equal(180, gridSurface.GridSurface.Direction);
    }

    private GsaGridPlaneSurfaceGoo GetComponentOutput() {
      var output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(_component);
      return output;
    }
  }

}
