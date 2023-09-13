using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using System;
using Xunit;
using LoadCase = GsaGH.Parameters.Enums.LoadCase;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaFaceThermalLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaFaceThermalLoad();

      Assert.Equal(LoadType.FaceThermal, load.LoadType);
    }

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
        FaceThermalLoad = {
          Case = 6,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          UniformTemperature = 7.0
        },
      };
      var duplicate = (GsaFaceThermalLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.FaceThermalLoad.Case = 1;
      duplicate.FaceThermalLoad.EntityList = "";
      duplicate.FaceThermalLoad.Name = "";
      duplicate.FaceThermalLoad.UniformTemperature = 99;

      Assert.Equal(LoadType.FaceThermal, original.LoadType);
      Assert.Equal(6, original.FaceThermalLoad.Case);
      Assert.Equal("all", original.FaceThermalLoad.EntityList);
      Assert.Equal("name", original.FaceThermalLoad.Name);
      Assert.Equal(7.0, original.FaceThermalLoad.UniformTemperature);
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

      duplicate.LoadCase = new GsaLoadCase(1, LoadCase.LoadCaseType.Dead, "DeadLoad");
      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
