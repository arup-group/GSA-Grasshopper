using Grasshopper.Kernel.Types;
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
  public class EditSectionModifierTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditSectionModifier();
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
        IsReferencePointCentroid = false,
        StressOption = GsaSectionModifier.StressOptionType.UseModified
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
      //var isBendingOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      var isRefPtOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      var stressOptOut = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(modifier, modifierdGoo.Value);
      Assert.NotEqual(modifier, modifierdGoo.Value);
      Assert.Equal(1.2, areaOut.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.3, i11Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.4, i22Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.9, jOut.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.1, k11Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.1, k22Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.8, volOut.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(5, addMassOut.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
      //Assert.True(isBending_out.Value); TO-DO GSA-6036
      Assert.False(isRefPtOut.Value);
      Assert.Equal(1, stressOptOut.Value);
      Assert.True(modifierdGoo.Value.IsModified);
    }

    [Fact]
    public void SetValuesFromNewComponent() {
      var modifier = new GsaSectionModifier();

      GH_OasysComponent comp = ComponentMother();

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
      //var isBendingOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      var isRefPtOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      var stressOptOut = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(modifier, modifierdGoo.Value);
      Assert.NotEqual(modifier, modifierdGoo.Value);
      Assert.Equal(1, areaOut.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, i11Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, i22Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, jOut.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, k11Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, k22Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, volOut.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0, addMassOut.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
      Assert.False(isRefPtOut.Value);
      Assert.Equal(0, stressOptOut.Value);
      Assert.False(modifierdGoo.Value.IsModified);

      i = 1;
      ComponentTestHelper.SetInput(comp,
        new GH_UnitNumber(new Area(1.2, AreaUnit.SquareMeter)),
        i++);
      ComponentTestHelper.SetInput(comp,
        new GH_UnitNumber(new AreaMomentOfInertia(1.3, AreaMomentOfInertiaUnit.MeterToTheFourth)),
        i++);
      ComponentTestHelper.SetInput(comp,
        new GH_UnitNumber(new AreaMomentOfInertia(1.4, AreaMomentOfInertiaUnit.MeterToTheFourth)),
        i++);
      ComponentTestHelper.SetInput(comp,
        new GH_UnitNumber(new AreaMomentOfInertia(0.9, AreaMomentOfInertiaUnit.MeterToTheFourth)),
        i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Ratio(110, RatioUnit.Percent)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Ratio(10, RatioUnit.Percent)), i++);
      ComponentTestHelper.SetInput(comp,
        new GH_UnitNumber(new VolumePerLength(0.8, VolumePerLengthUnit.CubicMeterPerMeter)),
        i++);
      ComponentTestHelper.SetInput(comp,
        new GH_UnitNumber(new LinearDensity(5, LinearDensityUnit.KilogramPerMeter)),
        i);
      ComponentTestHelper.SetInput(comp, new GH_Integer(2), 11);

      i = 0;
      modifierdGoo = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, i++);
      areaOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      i11Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      i22Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      jOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      k11Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      k22Out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      volOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      addMassOut = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      //isBendingOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      //isRefPtOut = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      stressOptOut = (GH_Integer)ComponentTestHelper.GetOutput(comp, i);

      Assert.Equal(1.2, areaOut.Value.As(AreaUnit.SquareMeter), 6);
      Assert.Equal(1.3, i11Out.Value.As(AreaMomentOfInertiaUnit.MeterToTheFourth), 6);
      Assert.Equal(1.4, i22Out.Value.As(AreaMomentOfInertiaUnit.MeterToTheFourth), 6);
      Assert.Equal(0.9, jOut.Value.As(AreaMomentOfInertiaUnit.MeterToTheFourth), 6);
      Assert.Equal(1.1, k11Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.1, k22Out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.8, volOut.Value.As(VolumePerLengthUnit.CubicMeterPerMeter), 6);
      Assert.Equal(5, addMassOut.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
      Assert.Equal(2, stressOptOut.Value);
      Assert.True(modifierdGoo.Value.IsModified);
    }
  }
}
