using System;
using System.Collections.Generic;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

using Polyline = GsaAPI.Polyline;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridAreaLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaGridAreaLoad();

      Assert.Equal(GridAreaPolyLineType.PLANE, load.ApiLoad.Type);
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
        ApiLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.X,
          GridSurface = 7,
          IsProjected = true,
          Name = "name",
          PolyLineDefinition = "",
          PolyLineReference = 1,
          Type = type,
          Value = 10,
        },
        ApiPolyline = new Polyline(new List<Vector2>() {
          new Vector2(-3.1, 8.8),
          new Vector2(13.6, 9.8),
          new Vector2(12.2 , 14.3),
          new Vector2(-0.7, 15.6),
        })
      };
      var originalGridPlaneSurface = new GsaGridPlaneSurface();
      original.GridPlaneSurface = originalGridPlaneSurface;

      var duplicate = (GsaGridAreaLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.Y;
      duplicate.ApiLoad.GridSurface = 1;
      duplicate.ApiLoad.IsProjected = false;
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.PolyLineDefinition = "";
      duplicate.ApiLoad.PolyLineReference = 0;
      duplicate.ApiLoad.Type = GridAreaPolyLineType.POLYGON;
      duplicate.ApiLoad.Value = 0;
      duplicate.GridPlaneSurface = new GsaGridPlaneSurface(new Plane());

      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.X, original.ApiLoad.Direction);
      Assert.Equal(7, original.ApiLoad.GridSurface);
      Assert.True(original.ApiLoad.IsProjected);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.Equal("", original.ApiLoad.PolyLineDefinition);
      Assert.Equal(1, original.ApiLoad.PolyLineReference);
      Assert.Equal(type, original.ApiLoad.Type);
      Assert.Equal(10, original.ApiLoad.Value);
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
