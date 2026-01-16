using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateSectionModifierTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateSectionModifier();
      comp.CreateAttributes();

      comp.SetSelected(0, 0); // set modify type to "Modify by"
      comp.SetSelected(1, 1); // set density unit to "g/cm"
      comp.SetSelected(2, 1); // set stress calc. to "Use unmodified"

      ComponentTestHelper.SetInput(comp, 0.1, 0);
      ComponentTestHelper.SetInput(comp, 0.2, 1);
      ComponentTestHelper.SetInput(comp, 0.3, 2);
      ComponentTestHelper.SetInput(comp, 0.4, 3);
      ComponentTestHelper.SetInput(comp, 0.5, 4);
      ComponentTestHelper.SetInput(comp, 0.6, 5);
      ComponentTestHelper.SetInput(comp, new Ratio(70, RatioUnit.Percent), 6);
      ComponentTestHelper.SetInput(comp, 1, 7);
      ComponentTestHelper.SetInput(comp, true, 8);
      ComponentTestHelper.SetInput(comp, true, 9);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0.1, output.Value.AreaModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.2, output.Value.I11Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.3, output.Value.I22Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.4, output.Value.JModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.5, output.Value.K11Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.6, output.Value.K22Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.7, output.Value.VolumeModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(1, output.Value.AdditionalMass.As(LinearDensityUnit.GramPerCentimeter));
      Assert.True(output.Value.IsBendingAxesPrincipal);
      Assert.True(output.Value.IsReferencePointCentroid);
    }
  }
}
