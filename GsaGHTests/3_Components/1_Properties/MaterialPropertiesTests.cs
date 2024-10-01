using GsaGH.Components;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class MaterialPropertiesTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new MaterialProperties();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void UpdateCustomUIUpdateStressTest() {
      var comp = (MaterialProperties)ComponentMother();
      comp.UpdateStress("kN/m^2");
      Assert.Equal("kN/m², kg/m³, °C⁻¹", comp.Message);
    }

    [Fact]
    public void UpdateCustomUIUpdateDensityTest() {
      var comp = (MaterialProperties)ComponentMother();
      comp.UpdateDensity("kg/l");
      Assert.Equal("MPa, kg/l, °C⁻¹", comp.Message);
    }

    [Fact]
    public void UpdateCustomUIUpdateTemperatureTest() {
      var comp = (MaterialProperties)ComponentMother();
      comp.UpdateTemperature("K");
      Assert.Equal("MPa, kg/m³, K⁻¹", comp.Message);
    }
  }
}
