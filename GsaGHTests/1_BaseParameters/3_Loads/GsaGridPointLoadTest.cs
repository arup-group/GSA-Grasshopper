using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;
using LoadCase = GsaGH.Parameters.Enums.LoadCase;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridPointLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaGridPointLoad();

      Assert.Equal(LoadType.GridPoint, load.LoadType);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaGridPointLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGridPointLoad {
        GridPointLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          GridSurface = 7,
          Name = "name",
          X = 5,
          Y = 6,
          Value = 10,
        },
      };

      var duplicate = (GsaGridPointLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.GridPointLoad.AxisProperty = 1;
      duplicate.GridPointLoad.Case = 1;
      duplicate.GridPointLoad.Direction = Direction.XX;
      duplicate.GridPointLoad.GridSurface = 1;
      duplicate.GridPointLoad.Name = "";
      duplicate.GridPointLoad.X = 0;
      duplicate.GridPointLoad.Y = 0;
      duplicate.GridPointLoad.Value = 0;

      Assert.Equal(LoadType.GridPoint, original.LoadType);
      Assert.Equal(5, original.GridPointLoad.AxisProperty);
      Assert.Equal(6, original.GridPointLoad.Case);
      Assert.Equal(Direction.ZZ, original.GridPointLoad.Direction);
      Assert.Equal(7, original.GridPointLoad.GridSurface);
      Assert.Equal("name", original.GridPointLoad.Name);
      Assert.Equal(5, original.GridPointLoad.X);
      Assert.Equal(6, original.GridPointLoad.Y);
      Assert.Equal(10, original.GridPointLoad.Value);
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaGridPointLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaGridPointLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaGridPointLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, LoadCase.LoadCaseType.Dead, "DeadLoad");
      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
