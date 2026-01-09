using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateProp2dModifierTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new Create2dPropertyModifier();
      comp.CreateAttributes();

      comp.SetSelected(0, 0); // set modify type to "Modify by"

      ComponentTestHelper.SetInput(comp, 0.1, 0);
      ComponentTestHelper.SetInput(comp, 0.2, 1);
      ComponentTestHelper.SetInput(comp, 0.3, 2);
      ComponentTestHelper.SetInput(comp, 0.4, 3);
      ComponentTestHelper.SetInput(comp, 5, 4);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaProperty2dModifierGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0.1, output.Value.InPlane.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.2, output.Value.Bending.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.3, output.Value.Shear.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.4, output.Value.Volume.As(RatioUnit.DecimalFraction));
      Assert.Equal(5, output.Value.AdditionalMass.As(AreaDensityUnit.KilogramPerSquareMeter));
    }
  }
}
