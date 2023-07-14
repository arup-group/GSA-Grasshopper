using System;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;
using static GsaAPI.GridLineLoad;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridLineLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaGridLineLoad();

      Assert.Equal(LoadType.GridLine, load.LoadType);
      Assert.Equal(0, load.GridLineLoad.PolyLineReference);
    }

    [Theory]
    [InlineData("EXPLICIT_POLYLINE")]
    [InlineData("POLYLINE_REFERENCE")]
    public void DuplicateTest(string polyLineTypeString) {
      var type = (PolyLineType)Enum.Parse(typeof(PolyLineType), polyLineTypeString);

      var original = new GsaGridLineLoad {
        GridLineLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          GridSurface = 7,
          IsProjected = true,
          Name = "name",
          PolyLineDefinition = "",
          PolyLineReference = 1,
          Type = type,
          ValueAtStart = 10,
          ValueAtEnd = 20,
        },
      };

      var duplicate = (GsaGridLineLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.GridLineLoad.AxisProperty = 1;
      duplicate.GridLineLoad.Case = 1;
      duplicate.GridLineLoad.Direction = Direction.XX;
      duplicate.GridLineLoad.GridSurface = 1;
      duplicate.GridLineLoad.IsProjected = false;
      duplicate.GridLineLoad.Name = "";
      duplicate.GridLineLoad.PolyLineDefinition = "";
      duplicate.GridLineLoad.PolyLineReference = 0;
      duplicate.GridLineLoad.Type = PolyLineType.EXPLICIT_POLYLINE;
      duplicate.GridLineLoad.ValueAtStart = 0;
      duplicate.GridLineLoad.ValueAtEnd = 0;

      Assert.Equal(LoadType.GridLine, original.LoadType);
      Assert.Equal(5, original.GridLineLoad.AxisProperty);
      Assert.Equal(6, original.GridLineLoad.Case);
      Assert.Equal(Direction.ZZ, original.GridLineLoad.Direction);
      Assert.Equal(7, original.GridLineLoad.GridSurface);
      Assert.True(original.GridLineLoad.IsProjected);
      Assert.Equal("name", original.GridLineLoad.Name);
      Assert.Equal("", original.GridLineLoad.PolyLineDefinition);
      Assert.Equal(1, original.GridLineLoad.PolyLineReference);
      Assert.Equal(type, original.GridLineLoad.Type);
      Assert.Equal(10, original.GridLineLoad.ValueAtStart);
      Assert.Equal(20, original.GridLineLoad.ValueAtEnd);
    }
  }
}
