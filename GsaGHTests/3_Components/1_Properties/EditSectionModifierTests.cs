using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class EditSectionModifierTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new EditSectionModifier();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void GetValuesFromExistingComponent()
    {
      GsaSectionModifier modifier = new GsaSectionModifier();
      modifier.AreaModifier = new Ratio(1.2, RatioUnit.DecimalFraction);
      modifier.I11Modifier = new Ratio(1.3, RatioUnit.DecimalFraction);
      modifier.I22Modifier = new Ratio(1.4, RatioUnit.DecimalFraction);
      modifier.JModifier = new Ratio(0.9, RatioUnit.DecimalFraction);
      modifier.K11Modifier = new Ratio(1.1, RatioUnit.DecimalFraction);
      modifier.K22Modifier = new Ratio(0.1, RatioUnit.DecimalFraction);
      modifier.VolumeModifier = new Ratio(0.8, RatioUnit.DecimalFraction);
      modifier.AdditionalMass = new LinearDensity(5, LinearDensityUnit.KilogramPerMeter);
      //modifier.IsBendingAxesPrincipal = true; TO-DO GSA-6036
      modifier.IsReferencePointCentroid = false;
      modifier.StressOption = GsaSectionModifier.StressOptionType.UseModified;

      var comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaSectionModifierGoo(modifier), 0);

      int i = 0;
      GsaSectionModifierGoo modifierdGoo = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber area_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber i11_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber i22_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber j_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber k11_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber k22_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber vol_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber addMass_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_Boolean isBending_out = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      GH_Boolean isRefPt_out = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer stressOpt_out = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(modifier, modifierdGoo.Value);
      Assert.NotEqual(modifier, modifierdGoo.Value);
      Assert.Equal(1.2, area_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.3, i11_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.4, i22_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.9, j_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.1, k11_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.1, k22_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.8, vol_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(5, addMass_out.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
      //Assert.True(isBending_out.Value); TO-DO GSA-6036
      Assert.False(isRefPt_out.Value);
      Assert.Equal(1, stressOpt_out.Value);
      Assert.True(modifierdGoo.Value.IsModified);
    }

    [Fact]
    public void SetValuesFromNewComponent()
    {
      GsaSectionModifier modifier = new GsaSectionModifier();

      var comp = ComponentMother();

      int i = 0;
      GsaSectionModifierGoo modifierdGoo = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber area_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber i11_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber i22_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber j_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber k11_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber k22_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber vol_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_UnitNumber addMass_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      GH_Boolean isBending_out = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      GH_Boolean isRefPt_out = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      GH_Integer stressOpt_out = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);

      Duplicates.AreEqual(modifier, modifierdGoo.Value);
      Assert.NotEqual(modifier, modifierdGoo.Value);
      Assert.Equal(1, area_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, i11_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, i22_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, j_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, k11_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, k22_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, vol_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0, addMass_out.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
      //Assert.False(isBending_out.Value); TO-DO GSA-6036
      Assert.False(isRefPt_out.Value);
      Assert.Equal(0, stressOpt_out.Value);
      Assert.False(modifierdGoo.Value.IsModified);

      i = 1;
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Area(1.2, AreaUnit.SquareMeter)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new AreaMomentOfInertia(1.3, AreaMomentOfInertiaUnit.MeterToTheFourth)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new AreaMomentOfInertia(1.4, AreaMomentOfInertiaUnit.MeterToTheFourth)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new AreaMomentOfInertia(0.9, AreaMomentOfInertiaUnit.MeterToTheFourth)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Ratio(110, RatioUnit.Percent)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Ratio(10, RatioUnit.Percent)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new VolumePerLength(0.8, VolumePerLengthUnit.CubicMeterPerMeter)), i++);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new LinearDensity(5, LinearDensityUnit.KilogramPerMeter)), i++);
      ComponentTestHelper.SetInput(comp, new GH_Integer(2), 11);

      i = 0;
      modifierdGoo = (GsaSectionModifierGoo)ComponentTestHelper.GetOutput(comp, i++);
      area_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      i11_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      i22_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      j_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      k11_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      k22_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      vol_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      addMass_out = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i++);
      isBending_out = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      isRefPt_out = (GH_Boolean)ComponentTestHelper.GetOutput(comp, i++);
      stressOpt_out = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);

      Assert.Equal(1.2, area_out.Value.As(AreaUnit.SquareMeter), 6);
      Assert.Equal(1.3, i11_out.Value.As(AreaMomentOfInertiaUnit.MeterToTheFourth), 6);
      Assert.Equal(1.4, i22_out.Value.As(AreaMomentOfInertiaUnit.MeterToTheFourth), 6);
      Assert.Equal(0.9, j_out.Value.As(AreaMomentOfInertiaUnit.MeterToTheFourth), 6);
      Assert.Equal(1.1, k11_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.1, k22_out.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0.8, vol_out.Value.As(VolumePerLengthUnit.CubicMeterPerMeter), 6);
      Assert.Equal(5, addMass_out.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
      Assert.Equal(2, stressOpt_out.Value);
      Assert.True(modifierdGoo.Value.IsModified);
    }
  }
}
