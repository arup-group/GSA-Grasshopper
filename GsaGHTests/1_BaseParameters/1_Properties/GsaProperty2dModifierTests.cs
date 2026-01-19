using GsaAPI;

using GsaGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProperty2dModifierTests {
    [Fact]
    public void CreateFromApiByTest() {
      var prop2d = new Prop2D();
      prop2d.PropertyModifier.InPlane = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 0.1);
      prop2d.PropertyModifier.Bending = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 0.1);
      prop2d.PropertyModifier.Shear = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 0.1);
      prop2d.PropertyModifier.Volume = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 0.1);
      prop2d.PropertyModifier.AdditionalMass = 0.1;

      var modifier = new GsaProperty2dModifier(prop2d.PropertyModifier);

      Assert.Equal(0.1, modifier.InPlane.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.Bending.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.Shear.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.Volume.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.AdditionalMass.As(AreaDensityUnit.KilogramPerSquareMeter));
      Assert.Equal(
        "In-plane:10% Bending:10% Shear:10% Volume:10% Add.Mass:0.1kg/m²",
        modifier.ToString());
    }

    [Fact]
    public void CreateFromApiToTest() {
      var prop2d = new Prop2D();
      prop2d.PropertyModifier.InPlane = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, 0.1);
      prop2d.PropertyModifier.Bending = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, 0.1);
      prop2d.PropertyModifier.Shear = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, 0.1);
      prop2d.PropertyModifier.Volume = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, 0.1);
      prop2d.PropertyModifier.AdditionalMass = 0.1;

      var modifier = new GsaProperty2dModifier(prop2d.PropertyModifier);

      Assert.Equal(0.1, modifier.InPlane.As(LengthUnit.Meter));
      Assert.Equal(0.1, modifier.Bending.As(VolumeUnit.CubicMeter));
      Assert.Equal(0.1, modifier.Shear.As(LengthUnit.Meter));
      Assert.Equal(0.1, modifier.Volume.As(LengthUnit.Meter));
      Assert.Equal(0.1, modifier.AdditionalMass.As(AreaDensityUnit.KilogramPerSquareMeter));
      Assert.Equal(
        "In-plane:0.1m²/m Bending:0.1m⁴/m Shear:0.1m²/m Volume:0.1m³/m² Add.Mass:0.1kg/m²",
        modifier.ToString());
    }

    [Fact]
    public void AdditionalMassTest() {
      var modifier = new GsaProperty2dModifier {
        AdditionalMass = new AreaDensity(1, AreaDensityUnit.KilogramPerSquareMeter),
      };
      Assert.Equal(1, modifier.AdditionalMass.As(AreaDensityUnit.KilogramPerSquareMeter));
    }

    [Fact]
    public void BendingByTest() {
      var modifier = new GsaProperty2dModifier {
        Bending = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.Bending.As(RatioUnit.DecimalFraction));
    }

    [Fact]
    public void BendingToTest() {
      var modifier = new GsaProperty2dModifier {
        Bending = new Volume(1, VolumeUnit.CubicMeter),
      };
      Assert.Equal(1, modifier.Bending.As(VolumeUnit.CubicMeter));
    }

    [Fact]
    public void VolumeByTest() {
      var modifier = new GsaProperty2dModifier {
        Volume = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.Volume.As(RatioUnit.DecimalFraction));
    }

    [Fact]
    public void VolumeToTest() {
      var modifier = new GsaProperty2dModifier {
        Volume = new Length(1, LengthUnit.Meter),
      };
      Assert.Equal(1, modifier.Volume.As(LengthUnit.Meter));
    }

    [Fact]
    public void ShearByTest() {
      var modifier = new GsaProperty2dModifier {
        Shear = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.Shear.As(RatioUnit.DecimalFraction));
    }

    [Fact]
    public void ShearToTest() {
      var modifier = new GsaProperty2dModifier {
        Shear = new Length(1, LengthUnit.Meter),
      };
      Assert.Equal(1, modifier.Shear.As(LengthUnit.Meter));
    }

    [Fact]
    public void InPlaneByTest() {
      var modifier = new GsaProperty2dModifier {
        InPlane = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.InPlane.As(RatioUnit.DecimalFraction));
    }

    [Fact]
    public void InPlaneToTest() {
      var modifier = new GsaProperty2dModifier {
        InPlane = new Length(1, LengthUnit.Meter),
      };
      Assert.Equal(1, modifier.InPlane.As(LengthUnit.Meter));
    }
  }
}
