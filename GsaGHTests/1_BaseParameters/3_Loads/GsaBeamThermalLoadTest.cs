using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaBeamThermalLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaBeamThermalLoad();

      Assert.Equal(LoadType.BeamThermal, load.LoadType);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaBeamThermalLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaBeamThermalLoad {
        BeamThermalLoad = {
          Case = 6,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          UniformTemperature = 7.0,
        },
      };

      var duplicate = (GsaBeamThermalLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.BeamThermalLoad.Case = 1;
      duplicate.BeamThermalLoad.EntityList = "";
      duplicate.BeamThermalLoad.Name = "";
      duplicate.BeamThermalLoad.UniformTemperature = 99;

      Assert.Equal(LoadType.BeamThermal, original.LoadType);

      Assert.Equal(6, original.BeamThermalLoad.Case);
      Assert.Equal("all", original.BeamThermalLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.BeamThermalLoad.EntityType);
      Assert.Equal("name", original.BeamThermalLoad.Name);
      Assert.Equal(7.0, original.BeamThermalLoad.UniformTemperature);
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaBeamThermalLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaBeamThermalLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaBeamThermalLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, GsaGH.Parameters.LoadCaseType.Dead, "DeadLoad");
      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
