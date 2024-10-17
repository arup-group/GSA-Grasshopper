using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaFaceThermalLoadTest {
    [Fact]
    public void LoadCaseTest() {
      var load = new GsaFaceThermalLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaFaceThermalLoad {
        ApiLoad = {
          Case = 6,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          UniformTemperature = 7.0
        },
      };
      var duplicate = (GsaFaceThermalLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.UniformTemperature = 99;

      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.Equal(7.0, original.ApiLoad.UniformTemperature);
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaFaceThermalLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaFaceThermalLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaFaceThermalLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "DeadLoad");

      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
