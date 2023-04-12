using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaSectionModifierTest {
    [Fact]
    public void AreaModifierToTest() {
      var modifier = new GsaSectionModifier {
        AreaModifier = new Area(1, AreaUnit.SquareMeter),
      };
      Assert.Equal(1, modifier.AreaModifier.As(AreaUnit.SquareMeter));
      Assert.Equal(SectionModifierOptionType.TO, modifier._sectionModifier.AreaModifier.Option);
    }

    [Fact]
    public void AreaModifierByTest() {
      var modifier = new GsaSectionModifier {
        AreaModifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.AreaModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier._sectionModifier.AreaModifier.Option);
    }

    [Fact]
    public void I11ModifierToTest() {
      var modifier = new GsaSectionModifier {
        I11Modifier = new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
      };
      Assert.Equal(1, modifier.I11Modifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(SectionModifierOptionType.TO, modifier._sectionModifier.I11Modifier.Option);
    }

    [Fact]
    public void I11ModifierByTest() {
      var modifier = new GsaSectionModifier {
        I11Modifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.I11Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier._sectionModifier.I11Modifier.Option);
    }

    [Fact]
    public void I22ModifierToTest() {
      var modifier = new GsaSectionModifier {
        I22Modifier = new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
      };
      Assert.Equal(1, modifier.I22Modifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(SectionModifierOptionType.TO, modifier._sectionModifier.I22Modifier.Option);
    }

    [Fact]
    public void I22ModifierByTest() {
      var modifier = new GsaSectionModifier {
        I22Modifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.I22Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier._sectionModifier.I22Modifier.Option);
    }

    [Fact]
    public void K11Test() {
      var modifier = new GsaSectionModifier {
        K11Modifier = new Ratio(1.5, RatioUnit.DecimalFraction),
      };
      Assert.Equal(1.5, modifier.K11Modifier.As(RatioUnit.DecimalFraction));
    }

    [Fact]
    public void K22Test() {
      var modifier = new GsaSectionModifier {
        K22Modifier = new Ratio(150, RatioUnit.Percent),
      };
      Assert.Equal(150, modifier.K22Modifier.As(RatioUnit.Percent));
    }

    [Fact]
    public void JModifierToTest() {
      var modifier = new GsaSectionModifier {
        JModifier = new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
      };
      Assert.Equal(1, modifier.JModifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(SectionModifierOptionType.TO, modifier._sectionModifier.JModifier.Option);
    }

    [Fact]
    public void JModifierByTest() {
      var modifier = new GsaSectionModifier {
        JModifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.JModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier._sectionModifier.JModifier.Option);
    }

    [Fact]
    public void VolumeModifierToTest() {
      var modifier = new GsaSectionModifier {
        VolumeModifier = new VolumePerLength(1, VolumePerLengthUnit.CubicMeterPerMeter),
      };
      Assert.Equal(1, modifier.VolumeModifier.As(VolumePerLengthUnit.CubicMeterPerMeter));
      Assert.Equal(SectionModifierOptionType.TO, modifier._sectionModifier.VolumeModifier.Option);
    }

    [Fact]
    public void VolumeModifierByTest() {
      var modifier = new GsaSectionModifier {
        VolumeModifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.VolumeModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier._sectionModifier.VolumeModifier.Option);
    }

    [Fact]
    public void AdditionalMassTest() {
      var modifier = new GsaSectionModifier {
        AdditionalMass = new LinearDensity(1, LinearDensityUnit.KilogramPerMeter),
      };
      Assert.Equal(1, modifier.AdditionalMass.As(LinearDensityUnit.KilogramPerMeter));
    }

    [Fact]
    public void IsBendingAxesPrincipalTest() {
      var modifier = new GsaSectionModifier {
        IsBendingAxesPrincipal = true,
      };
      Assert.True(modifier.IsBendingAxesPrincipal);

      modifier.IsBendingAxesPrincipal = false;
      Assert.False(modifier.IsBendingAxesPrincipal);
    }

    [Fact]
    public void IsReferencePointCentroidTest() {
      var modifier = new GsaSectionModifier {
        IsReferencePointCentroid = true,
      };
      Assert.True(modifier.IsReferencePointCentroid);

      modifier.IsReferencePointCentroid = false;
      Assert.False(modifier.IsReferencePointCentroid);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaSectionModifier {
        StressOption = GsaSectionModifier.StressOptionType.NoCalculation,
      };
      GsaSectionModifier duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
      Assert.NotEqual(original, duplicate);

      duplicate.AreaModifier = new Area(2, AreaUnit.SquareMeter);
      duplicate.I11Modifier = new AreaMomentOfInertia(2, AreaMomentOfInertiaUnit.MeterToTheFourth);
      duplicate.I22Modifier = new AreaMomentOfInertia(2, AreaMomentOfInertiaUnit.MeterToTheFourth);
      duplicate.JModifier = new AreaMomentOfInertia(2, AreaMomentOfInertiaUnit.MeterToTheFourth);
      duplicate.K11Modifier = new Ratio(2, RatioUnit.DecimalFraction);
      duplicate.K22Modifier = new Ratio(2, RatioUnit.DecimalFraction);
      duplicate.VolumeModifier = new VolumePerLength(2, VolumePerLengthUnit.CubicMeterPerMeter);
      duplicate.AdditionalMass = new LinearDensity(2, LinearDensityUnit.KilogramPerMeter);
      duplicate.StressOption = GsaSectionModifier.StressOptionType.UseModified;
      duplicate.IsBendingAxesPrincipal = true;
      duplicate.IsReferencePointCentroid = true;

      Assert.NotEqual(2, original.AreaModifier.Value);
      Assert.NotEqual(2, original.I11Modifier.Value);
      Assert.NotEqual(2, original.I22Modifier.Value);
      Assert.NotEqual(2, original.JModifier.Value);
      Assert.NotEqual(2, original.K11Modifier.Value);
      Assert.NotEqual(2, original.K22Modifier.Value);
      Assert.NotEqual(2, original.VolumeModifier.Value);
      Assert.NotEqual(2, original.AdditionalMass.Value);
      Assert.NotEqual(GsaSectionModifier.StressOptionType.UseModified, original.StressOption);
      Assert.False(original.IsBendingAxesPrincipal);
      Assert.False(original.IsReferencePointCentroid);
    }
  }
}
