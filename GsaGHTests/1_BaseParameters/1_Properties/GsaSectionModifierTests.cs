using System;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaSectionModifierTests {
    [Fact]
    public void CreateFromApiByTest() {
      var api = new SectionModifier() {
        AdditionalMass = 10,
        AreaModifier = new SectionModifierAttribute(SectionModifierOptionType.BY, 0.1),
        I11Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, 0.1),
        I22Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, 0.1),
        JModifier = new SectionModifierAttribute(SectionModifierOptionType.BY, 0.1),
        K11Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, 0.1),
        K22Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, 0.1),
        VolumeModifier = new SectionModifierAttribute(SectionModifierOptionType.BY, 0.1),
        StressOption = SectionModifierStressType.USE_MOD,
        IsBendingAxesPrincipal = true,
        IsReferencePointCentroid = true,
      };

      var modifier = new GsaSectionModifier(api);

      Assert.Equal(0.1, modifier.AreaModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.I11Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.I22Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.JModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.K11Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.K22Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(0.1, modifier.VolumeModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(10, modifier.AdditionalMass.Value);
      Assert.Equal("UseModified", modifier.StressOption.ToString());
      Assert.True(modifier.IsBendingAxesPrincipal);
      Assert.True(modifier.IsReferencePointCentroid);
      Assert.Equal(
        "A:10% I11:10% I22:10% J:10% K11:10% K22:10% V:10% Add.Mass:10kg/m " +
        "StressCalc.Opt.:UseModified BendingAxis(UsePringipal(u,v) AnalysisRefPt(UseCentroid)",
        modifier.ToString());
    }

    [Fact]
    public void CreateFromApiToTest() {
      var api = new SectionModifier() {
        AdditionalMass = 10,
        AreaModifier = new SectionModifierAttribute(SectionModifierOptionType.TO, 0.01),
        I11Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, 0.000001),
        I22Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, 0.000001),
        JModifier = new SectionModifierAttribute(SectionModifierOptionType.TO, 0.000001),
        K11Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, 0.1),
        K22Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, 0.1),
        VolumeModifier = new SectionModifierAttribute(SectionModifierOptionType.TO, 10),
        StressOption = SectionModifierStressType.NO_MOD,
        IsBendingAxesPrincipal = false,
        IsReferencePointCentroid = false,
      };

      var modifier = new GsaSectionModifier(api);
      Assert.Equal(0.01, modifier.AreaModifier.As(AreaUnit.SquareMeter));
      Assert.Equal(0.000001, modifier.I11Modifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(0.000001, modifier.I22Modifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(0.000001, modifier.JModifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(10, modifier.K11Modifier.As(RatioUnit.Percent));
      Assert.Equal(10, modifier.K22Modifier.As(RatioUnit.Percent));
      Assert.Equal(10, modifier.VolumeModifier.As(VolumePerLengthUnit.CubicMeterPerMeter));
      Assert.Equal(10, modifier.AdditionalMass.Value);
      Assert.Equal("NoCalculation", modifier.StressOption.ToString());
      Assert.False(modifier.IsBendingAxesPrincipal);
      Assert.False(modifier.IsReferencePointCentroid);
      Assert.Equal(
        "A:100cm² I11:100cm⁴ I22:100cm⁴ J:100cm⁴ K11:0.100[-] K22:0.100[-] " +
        "V:10m³/m Add.Mass:10kg/m",
        modifier.ToString());
    }


    [Fact]
    public void AdditionalMassTest() {
      var modifier = new GsaSectionModifier {
        AdditionalMass = new LinearDensity(1, LinearDensityUnit.KilogramPerMeter),
      };
      Assert.Equal(1, modifier.AdditionalMass.As(LinearDensityUnit.KilogramPerMeter));
    }

    [Fact]
    public void AreaModifierByTest() {
      var modifier = new GsaSectionModifier {
        AreaModifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.AreaModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier.ApiSectionModifier.AreaModifier.Option);
    }

    [Fact]
    public void AreaModifierToTest() {
      var modifier = new GsaSectionModifier {
        AreaModifier = new Area(1, AreaUnit.SquareMeter),
      };
      Assert.Equal(1, modifier.AreaModifier.As(AreaUnit.SquareMeter));
      Assert.Equal(SectionModifierOptionType.TO, modifier.ApiSectionModifier.AreaModifier.Option);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaSectionModifier {
        StressOption = StressOptionType.NoCalculation,
      };

      var duplicate = new GsaSectionModifier(original);

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
      duplicate.StressOption = StressOptionType.UseModified;
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
      Assert.NotEqual(StressOptionType.UseModified, original.StressOption);
      Assert.False(original.IsBendingAxesPrincipal);
      Assert.False(original.IsReferencePointCentroid);
    }

    [Fact]
    public void I11ModifierByTest() {
      var modifier = new GsaSectionModifier {
        I11Modifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.I11Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier.ApiSectionModifier.I11Modifier.Option);
    }

    [Fact]
    public void I11ModifierToTest() {
      var modifier = new GsaSectionModifier {
        I11Modifier = new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
      };
      Assert.Equal(1, modifier.I11Modifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(SectionModifierOptionType.TO, modifier.ApiSectionModifier.I11Modifier.Option);
    }

    [Fact]
    public void I22ModifierByTest() {
      var modifier = new GsaSectionModifier {
        I22Modifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.I22Modifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier.ApiSectionModifier.I22Modifier.Option);
    }

    [Fact]
    public void I22ModifierToTest() {
      var modifier = new GsaSectionModifier {
        I22Modifier = new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
      };
      Assert.Equal(1, modifier.I22Modifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(SectionModifierOptionType.TO, modifier.ApiSectionModifier.I22Modifier.Option);
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
    public void JModifierByTest() {
      var modifier = new GsaSectionModifier {
        JModifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.JModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier.ApiSectionModifier.JModifier.Option);
    }

    [Fact]
    public void JModifierToTest() {
      var modifier = new GsaSectionModifier {
        JModifier = new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.MeterToTheFourth),
      };
      Assert.Equal(1, modifier.JModifier.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
      Assert.Equal(SectionModifierOptionType.TO, modifier.ApiSectionModifier.JModifier.Option);
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
    public void VolumeModifierByTest() {
      var modifier = new GsaSectionModifier {
        VolumeModifier = new Ratio(2, RatioUnit.DecimalFraction),
      };
      Assert.Equal(2, modifier.VolumeModifier.As(RatioUnit.DecimalFraction));
      Assert.Equal(SectionModifierOptionType.BY, modifier.ApiSectionModifier.VolumeModifier.Option);
    }

    [Fact]
    public void VolumeModifierToTest() {
      var modifier = new GsaSectionModifier {
        VolumeModifier = new VolumePerLength(1, VolumePerLengthUnit.CubicMeterPerMeter),
      };
      Assert.Equal(1, modifier.VolumeModifier.As(VolumePerLengthUnit.CubicMeterPerMeter));
      Assert.Equal(SectionModifierOptionType.TO, modifier.ApiSectionModifier.VolumeModifier.Option);
    }

    [Fact]
    public void AreaModifierThrowsExceptionTest() {
      var modifier = new GsaSectionModifier();
      Assert.Throws<ArgumentException>(() => modifier.AreaModifier = Jerk.Zero);
    }

    [Fact]
    public void I11ModifierThrowsExceptionTest() {
      var modifier = new GsaSectionModifier();
      Assert.Throws<ArgumentException>(() => modifier.I11Modifier = Jerk.Zero);
    }

    [Fact]
    public void I22ModifierThrowsExceptionTest() {
      var modifier = new GsaSectionModifier();
      Assert.Throws<ArgumentException>(() => modifier.I22Modifier = Jerk.Zero);
    }

    [Fact]
    public void JModifierThrowsExceptionTest() {
      var modifier = new GsaSectionModifier();
      Assert.Throws<ArgumentException>(() => modifier.JModifier = Jerk.Zero);
    }

    [Fact]
    public void VolumeModifierThrowsExceptionTest() {
      var modifier = new GsaSectionModifier();
      Assert.Throws<ArgumentException>(() => modifier.VolumeModifier = Jerk.Zero);
    }
  }
}
