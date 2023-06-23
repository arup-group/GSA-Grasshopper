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
  public class EditProp2dModifierTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditProp2dModifier();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void GetValuesFromExistingComponent() {
      var modifier = new GsaProp2dModifier {
        InPlane = new Ratio(1.2, RatioUnit.DecimalFraction),
        Bending = new Ratio(1.3, RatioUnit.DecimalFraction),
        Shear = new Ratio(1.4, RatioUnit.DecimalFraction),
        Volume = new Ratio(1.5, RatioUnit.DecimalFraction),
        AdditionalMass = new LinearDensity(6, LinearDensityUnit.KilogramPerMeter),
      };

      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new GsaProp2dModifierGoo(modifier), 0);

      var modifierGoo = (GsaProp2dModifierGoo)ComponentTestHelper.GetOutput(comp, 0);
      var inplane = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1);
      var bending = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 2);
      var shear = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 3);
      var volume = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 4);
      var addMass = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 5);

      Duplicates.AreEqual(modifier, modifierGoo.Value);
      Assert.NotEqual(modifier, modifierGoo.Value);
      Assert.Equal(1.2, inplane.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.3, bending.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.4, shear.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1.5, volume.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(6, addMass.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
    }

    [Fact]
    public void SetValuesFromNewComponent() {
      var modifier = new GsaProp2dModifier();

      GH_OasysComponent comp = ComponentMother();

      var modifierGoo = (GsaProp2dModifierGoo)ComponentTestHelper.GetOutput(comp, 0);
      var inplane = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1);
      var bending = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 2);
      var shear = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 3);
      var volume = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 4);
      var addMass = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 5);

      Duplicates.AreEqual(modifier, modifierGoo.Value);
      Assert.NotEqual(modifier, modifierGoo.Value);
      Assert.Equal(1, inplane.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, bending.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, shear.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(1, volume.Value.As(RatioUnit.DecimalFraction), 6);
      Assert.Equal(0, addMass.Value.As(LinearDensityUnit.KilogramPerMeter), 6);

      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(1.2, LengthUnit.Meter)), 1);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Volume(1.3, VolumeUnit.CubicMeter)), 2);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(1.4, LengthUnit.Meter)), 3);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new Length(1.5, LengthUnit.Meter)), 4);
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(new LinearDensity(6, LinearDensityUnit.KilogramPerMeter)), 5);

      modifierGoo = (GsaProp2dModifierGoo)ComponentTestHelper.GetOutput(comp, 0);
      inplane = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 1);
      bending = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 2);
      shear = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 3);
      volume = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 4);
      addMass = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 5);

      Assert.Equal(1.2, inplane.Value.As(LengthUnit.Meter), 6);
      Assert.Equal(1.3, bending.Value.As(VolumeUnit.CubicMeter), 6);
      Assert.Equal(1.4, shear.Value.As(LengthUnit.Meter), 6);
      Assert.Equal(1.5, volume.Value.As(LengthUnit.Meter), 6);
      Assert.Equal(6, addMass.Value.As(LinearDensityUnit.KilogramPerMeter), 6);
    }
  }
}
