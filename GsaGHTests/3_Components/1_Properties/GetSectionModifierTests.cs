using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Properties {
  [Collection("GrasshopperFixture collection")]
  public class GetSectionModifierTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new GetSectionModifier();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void GetValuesFromExistingComponent() {
      var modifier = new GsaSectionModifier {
        AreaModifier = new Ratio(1.2, RatioUnit.DecimalFraction),
        I11Modifier = new Ratio(1.3, RatioUnit.DecimalFraction),
        I22Modifier = new Ratio(1.4, RatioUnit.DecimalFraction),
        JModifier = new Ratio(0.9, RatioUnit.DecimalFraction),
        K11Modifier = new Ratio(1.1, RatioUnit.DecimalFraction),
        K22Modifier = new Ratio(0.1, RatioUnit.DecimalFraction),
        VolumeModifier = new Ratio(0.8, RatioUnit.DecimalFraction),
        AdditionalMass = new LinearDensity(5, LinearDensityUnit.KilogramPerMeter),
        IsBendingAxesPrincipal = true,
        IsReferencePointCentroid = false,
        StressOption = StressOptionType.UseModified,
      };

      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaSectionModifierGoo(modifier), 0);

      int i = 0;
      var modifierdGoo = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, i++);
      var areaOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var i11Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var i22Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var jOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var k11Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var k22Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var volOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var addMassOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      var isBendingOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      var isRefPtOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      var stressOptOut = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(modifier, modifierdGoo.Value);
      Assert.Equal(1.2, areaOut.Value.As(RatioUnit.DecimalFraction), DoubleComparer.Default);
      Assert.Equal(1.3, i11Out.Value.As(RatioUnit.DecimalFraction), DoubleComparer.Default);
      Assert.Equal(1.4, i22Out.Value.As(RatioUnit.DecimalFraction), DoubleComparer.Default);
      Assert.Equal(0.9, jOut.Value.As(RatioUnit.DecimalFraction), DoubleComparer.Default);
      Assert.Equal(1.1, k11Out.Value.As(RatioUnit.DecimalFraction), DoubleComparer.Default);
      Assert.Equal(0.1, k22Out.Value.As(RatioUnit.DecimalFraction), DoubleComparer.Default);
      Assert.Equal(0.8, volOut.Value.As(RatioUnit.DecimalFraction), DoubleComparer.Default);
      Assert.Equal(5, addMassOut.Value.As(LinearDensityUnit.KilogramPerMeter), DoubleComparer.Default);
      Assert.True(isBendingOut.Value);
      Assert.False(isRefPtOut.Value);
      Assert.Equal(1, stressOptOut.Value);
      Assert.True(modifierdGoo.Value.IsModified);
    }

    [Fact]
    public void UpdateCustomUIUpdateDensityTest() {
      var comp = (GetSectionModifier)ComponentMother();
      comp.UpdateDensity("kg/cm");
      Assert.Equal("cm, kg/cm", comp.Message);
    }

    [Fact]
    public void UpdateCustomUIUpdateLengthTest() {
      var comp = (GetSectionModifier)ComponentMother();
      comp.UpdateLength("ft");
      Assert.Equal("ft, kg/m", comp.Message);
    }
  }
}
