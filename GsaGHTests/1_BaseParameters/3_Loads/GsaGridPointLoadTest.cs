using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

using LoadCaseType = GsaGH.Parameters.LoadCaseType;
using GsaGH.Helpers;
namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGridPointLoadTest {
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
        ApiLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.X,
          GridSurface = 7,
          Name = "name",
          X = 5,
          Y = 6,
          Value = 10,
        },
      };

      var duplicate = (GsaGridPointLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.Y;
      duplicate.ApiLoad.GridSurface = 1;
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.X = 0;
      duplicate.ApiLoad.Y = 0;
      duplicate.ApiLoad.Value = 0;

      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.X, original.ApiLoad.Direction);
      Assert.Equal(7, original.ApiLoad.GridSurface);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.Equal(5, original.ApiLoad.X, DoubleComparer.Default);
      Assert.Equal(6, original.ApiLoad.Y, DoubleComparer.Default);
      Assert.Equal(10, original.ApiLoad.Value);
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

      duplicate.LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "DeadLoad");

      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
