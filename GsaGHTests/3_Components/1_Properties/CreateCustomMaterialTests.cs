using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateCustomMaterialTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateCustomMaterial();
      comp.CreateAttributes();

      comp.SetSelected(0, 3); // set material type to "Timber"
      comp.SetSelected(1, 3); // set stress unit to "GPa"
      comp.SetSelected(2, 5); // set density unit to "kg/m^3"
      comp.SetSelected(3, 1); // set temperature unit to "K"

      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "name", 1);
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 4);
      ComponentTestHelper.SetInput(comp, 4, 5);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(1, output.Value.Id);
      Assert.Equal("name", output.Value.AnalysisMaterial.Name);
      Assert.Equal(MatType.Timber, output.Value.MaterialType);
      Assert.Equal(new Pressure(1, PressureUnit.Gigapascal).As(PressureUnit.Pascal),
        output.Value.AnalysisMaterial.ElasticModulus);
      Assert.Equal(2, output.Value.AnalysisMaterial.PoissonsRatio);
      Assert.Equal(new Density(3, DensityUnit.KilogramPerCubicMeter).As(DensityUnit.KilogramPerCubicMeter),
        output.Value.AnalysisMaterial.Density);
      Assert.Equal(new CoefficientOfThermalExpansion(4, CoefficientOfThermalExpansionUnit.PerKelvin).As(
          CoefficientOfThermalExpansionUnit.PerDegreeCelsius),
        output.Value.AnalysisMaterial.CoefficientOfThermalExpansion);
    }
  }
}
