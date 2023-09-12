using System;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridAreaLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaGridAreaLoad();

      Assert.Equal(LoadType.GridArea, load.LoadType);
      Assert.Equal(GridAreaPolyLineType.PLANE, load.GridAreaLoad.Type);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaGridAreaLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Theory]
    [InlineData("PLANE")]
    [InlineData("POLYREF")]
    [InlineData("POLYGON")]
    public void DuplicateTest(string gridAreaPolyLineTypeString) {
      var type = (GridAreaPolyLineType)Enum.Parse(typeof(GridAreaPolyLineType),
        gridAreaPolyLineTypeString);

      var original = new GsaGridAreaLoad {
        GridAreaLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          GridSurface = 7,
          IsProjected = true,
          Name = "name",
          PolyLineDefinition = "",
          PolyLineReference = 1,
          Type = type,
          Value = 10,
        },
      };
      var originalGridPlaneSurface = new GsaGridPlaneSurface();
      original.GridPlaneSurface = originalGridPlaneSurface;

      var duplicate = (GsaGridAreaLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.GridAreaLoad.AxisProperty = 1;
      duplicate.GridAreaLoad.Case = 1;
      duplicate.GridAreaLoad.Direction = Direction.XX;
      duplicate.GridAreaLoad.GridSurface = 1;
      duplicate.GridAreaLoad.IsProjected = false;
      duplicate.GridAreaLoad.Name = "";
      duplicate.GridAreaLoad.PolyLineDefinition = "";
      duplicate.GridAreaLoad.PolyLineReference = 0;
      duplicate.GridAreaLoad.Type = GridAreaPolyLineType.POLYGON;
      duplicate.GridAreaLoad.Value = 0;
      duplicate.GridPlaneSurface = new GsaGridPlaneSurface(new Plane());

      Assert.Equal(LoadType.GridArea, original.LoadType);
      Assert.Equal(5, original.GridAreaLoad.AxisProperty);
      Assert.Equal(6, original.GridAreaLoad.Case);
      Assert.Equal(Direction.ZZ, original.GridAreaLoad.Direction);
      Assert.Equal(7, original.GridAreaLoad.GridSurface);
      Assert.True(original.GridAreaLoad.IsProjected);
      Assert.Equal("name", original.GridAreaLoad.Name);
      Assert.Equal("", original.GridAreaLoad.PolyLineDefinition);
      Assert.Equal(1, original.GridAreaLoad.PolyLineReference);
      Assert.Equal(type, original.GridAreaLoad.Type);
      Assert.Equal(10, original.GridAreaLoad.Value);
      Assert.Equal(originalGridPlaneSurface, original.GridPlaneSurface);
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaGridAreaLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaGridAreaLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaGridAreaLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, GsaGH.Parameters.LoadCaseType.Dead, "DeadLoad");
      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
