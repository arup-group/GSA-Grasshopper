using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Properties {
  [Collection("GrasshopperFixture collection")]
  public class GetProp2dModifierTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new Get2dPropertyModifier();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void GetValuesFromExistingComponent() {
      var modifier = new GsaProperty2dModifier {
        InPlane = new Ratio(1.2, RatioUnit.DecimalFraction),
        Bending = new Ratio(1.3, RatioUnit.DecimalFraction),
        Shear = new Ratio(1.4, RatioUnit.DecimalFraction),
        Volume = new Ratio(1.5, RatioUnit.DecimalFraction),
        AdditionalMass = new AreaDensity(6, AreaDensityUnit.KilogramPerSquareMeter),
      };

      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaProperty2dModifierGoo(modifier), 0);

      var inplane = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 0);
      var bending = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1);
      var shear = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 2);
      var volume = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 3);
      var addMass = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 4);

      Assert.Equal(1.2, inplane.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.3, bending.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.4, shear.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.5, volume.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(6, addMass.Value.As(AreaDensityUnit.KilogramPerSquareMeter), 6);
    }
  }
}
