using System;

using GsaAPI;

using GsaGH.Helpers;

using OasysGH.Units;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters {
  /// <summary>
  /// A Section Modifier is part of a <see cref="GsaSection"/> and can be used to modify property's analytical properties without changing the `Profile` or <see cref="GsaMaterial"/>. By default the Section Modifier is unmodified.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-sect-lib.html#modifiers">Section Modifiers</see> to read more.</para>
  /// </summary>
  public class GsaSectionModifier {
    public LinearDensity AdditionalMass {
      get => new LinearDensity(ApiSectionModifier.AdditionalMass, LinearDensityUnit.KilogramPerMeter);
      set => ApiSectionModifier.AdditionalMass = value.As(LinearDensityUnit.KilogramPerMeter);
    }
    public IQuantity AreaModifier {
      get => ApiSectionModifier.AreaModifier.Option == SectionModifierOptionType.BY
        ? new Ratio(ApiSectionModifier.AreaModifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent)
        : (IQuantity)new Area(ApiSectionModifier.AreaModifier.Value, AreaUnit.SquareMeter).ToUnit(
            DefaultUnits.SectionAreaUnit);
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaUnit)
          && value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("AreaModifier must be either Area or Ratio");
        }

        ApiSectionModifier.AreaModifier = value.QuantityInfo.UnitType == typeof(AreaUnit)
          ? new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(AreaUnit.SquareMeter))
          : new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
      }
    }
    public IQuantity I11Modifier {
      get => ApiSectionModifier.I11Modifier.Option == SectionModifierOptionType.BY
        ? new Ratio(ApiSectionModifier.I11Modifier.Value, RatioUnit.DecimalFraction)
            .ToUnit(RatioUnit.Percent)
        : (IQuantity)new AreaMomentOfInertia(ApiSectionModifier.I11Modifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth)
            .ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          && value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("I11Modifier must be either AreaMomentOfInertia or Ratio");
        }

        ApiSectionModifier.I11Modifier
          = value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit)
          ? new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(AreaMomentOfInertiaUnit.MeterToTheFourth))
          : new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
      }
    }
    public IQuantity I22Modifier {
      get => ApiSectionModifier.I22Modifier.Option == SectionModifierOptionType.BY
        ? new Ratio(ApiSectionModifier.I22Modifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent)
        : (IQuantity)new AreaMomentOfInertia(ApiSectionModifier.I22Modifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth)
              .ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          && value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        }

        ApiSectionModifier.I22Modifier
          = value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit)
          ? new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(AreaMomentOfInertiaUnit.MeterToTheFourth))
          : new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
      }
    }
    public bool IsBendingAxesPrincipal {
      get => ApiSectionModifier.IsBendingAxesPrincipal;
      set => ApiSectionModifier.IsBendingAxesPrincipal = value;
    }
    public bool IsModified
      => IsAttributeModified(ApiSectionModifier.AreaModifier)
        || IsAttributeModified(ApiSectionModifier.I11Modifier)
        || IsAttributeModified(ApiSectionModifier.I22Modifier)
        || IsAttributeModified(ApiSectionModifier.JModifier)
        || IsAttributeModified(ApiSectionModifier.K11Modifier)
        || IsAttributeModified(ApiSectionModifier.K22Modifier)
        || IsAttributeModified(ApiSectionModifier.VolumeModifier) || IsBendingAxesPrincipal
        || IsReferencePointCentroid || ApiSectionModifier.AdditionalMass != 0
        || ApiSectionModifier.StressOption != SectionModifierStressType.NO_MOD;
    public bool IsReferencePointCentroid {
      get => ApiSectionModifier.IsReferencePointCentroid;
      set => ApiSectionModifier.IsReferencePointCentroid = value;
    }
    public IQuantity JModifier {
      get => ApiSectionModifier.JModifier.Option == SectionModifierOptionType.BY
        ? new Ratio(ApiSectionModifier.JModifier.Value, RatioUnit.DecimalFraction).ToUnit(
          RatioUnit.Percent)
        : (IQuantity)new AreaMomentOfInertia(ApiSectionModifier.JModifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth)
             .ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          && value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        }

        ApiSectionModifier.JModifier = value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit)
          ? new SectionModifierAttribute(SectionModifierOptionType.TO,
            value.As(AreaMomentOfInertiaUnit.MeterToTheFourth))
          : new SectionModifierAttribute(SectionModifierOptionType.BY,
            value.As(RatioUnit.DecimalFraction));
      }
    }
    public Ratio K11Modifier {
      get
        => ApiSectionModifier.K11Modifier.Option == SectionModifierOptionType.BY ?
          new Ratio(ApiSectionModifier.K11Modifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent)
        : new Ratio(ApiSectionModifier.K11Modifier.Value, RatioUnit.DecimalFraction);
      set => ApiSectionModifier.K11Modifier = value.Unit == RatioUnit.Percent
        ? new SectionModifierAttribute(SectionModifierOptionType.BY,
            value.As(RatioUnit.DecimalFraction))
        // assume that percentage unit is modify BY option
        // assume that all other than percentage unit is modify TO option
        : new SectionModifierAttribute(SectionModifierOptionType.TO,
            value.As(RatioUnit.DecimalFraction));
    }
    public Ratio K22Modifier {
      get
        => ApiSectionModifier.K22Modifier.Option == SectionModifierOptionType.BY
        ? new Ratio(ApiSectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent)
        : new Ratio(ApiSectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction);
      set => ApiSectionModifier.K22Modifier = value.Unit == RatioUnit.Percent
        ? new SectionModifierAttribute(SectionModifierOptionType.BY,
            value.As(RatioUnit.DecimalFraction))
        // assume that percentage unit is modify BY option
        // assume that all other than percentage unit is modify TO option
        : new SectionModifierAttribute(SectionModifierOptionType.TO,
            value.As(RatioUnit.DecimalFraction));
    }
    public StressOptionType StressOption {
      get => ApiSectionModifier.StressOption switch {
        SectionModifierStressType.USE_MOD => StressOptionType.UseModified,
        SectionModifierStressType.USE_UNMOD => StressOptionType.UseUnmodified,
        _ => StressOptionType.NoCalculation,
      };
      set => ApiSectionModifier.StressOption = value switch {
        StressOptionType.UseModified => SectionModifierStressType.USE_MOD,
        StressOptionType.UseUnmodified => SectionModifierStressType.USE_UNMOD,
        _ => SectionModifierStressType.NO_MOD,
      };
    }
    public IQuantity VolumeModifier {
      get => ApiSectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY
          ? new Ratio(ApiSectionModifier.VolumeModifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent)
          : (IQuantity)new VolumePerLength(ApiSectionModifier.VolumeModifier.Value,
            VolumePerLengthUnit.CubicMeterPerMeter).ToUnit(DefaultUnits.VolumePerLengthUnit);
      set {
        if (value.QuantityInfo.UnitType != typeof(VolumePerLengthUnit)
          && value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("VolumeModifier must be either VolumePerLength or Ratio");
        }

        ApiSectionModifier.VolumeModifier
          = value.QuantityInfo.UnitType == typeof(VolumePerLengthUnit)
          ? new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(VolumePerLengthUnit.CubicMeterPerMeter))
          : new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
      }
    }

    public SectionModifier ApiSectionModifier { get; private set; }

    public GsaSectionModifier() {
      ApiSectionModifier = new SectionModifier();
    }

    internal GsaSectionModifier(SectionModifier sectionModifier) {
      ApiSectionModifier = sectionModifier;
    }

    public GsaSectionModifier(GsaSectionModifier other) {
      ApiSectionModifier = other.DuplicateApiObject();
    }

    public SectionModifier DuplicateApiObject() {
      return new SectionModifier {
        AreaModifier
          = new SectionModifierAttribute(ApiSectionModifier.AreaModifier.Option,
            ApiSectionModifier.AreaModifier.Value),
        I11Modifier
          = new SectionModifierAttribute(ApiSectionModifier.I11Modifier.Option,
            ApiSectionModifier.I11Modifier.Value),
        I22Modifier
          = new SectionModifierAttribute(ApiSectionModifier.I22Modifier.Option,
            ApiSectionModifier.I22Modifier.Value),
        JModifier
          = new SectionModifierAttribute(ApiSectionModifier.JModifier.Option,
            ApiSectionModifier.JModifier.Value),
        K11Modifier
          = new SectionModifierAttribute(ApiSectionModifier.K11Modifier.Option,
            ApiSectionModifier.K11Modifier.Value),
        K22Modifier
          = new SectionModifierAttribute(ApiSectionModifier.K22Modifier.Option,
            ApiSectionModifier.K22Modifier.Value),
        VolumeModifier
          = new SectionModifierAttribute(ApiSectionModifier.VolumeModifier.Option,
            ApiSectionModifier.VolumeModifier.Value),
        AdditionalMass = ApiSectionModifier.AdditionalMass,
        StressOption = ApiSectionModifier.StressOption,
        IsBendingAxesPrincipal = ApiSectionModifier.IsBendingAxesPrincipal,
        IsReferencePointCentroid = ApiSectionModifier.IsReferencePointCentroid,
      };
    }

    public override string ToString() {
      if (!IsModified) {
        return "Unmodified";
      }

      string a = "A:";
      string i11 = "I11:";
      string i22 = "I22:";
      string j = "J:";
      string k11 = "K11:";
      string k22 = "K22:";
      string v = "V:";
      string mass = "Add.Mass:";
      string stress = "StressCalc.Opt.:";
      string axis = "X";
      string refPt = "X";

      if (ApiSectionModifier.AreaModifier.Option == SectionModifierOptionType.TO) {
        var val = (Area)AreaModifier;
        a += val.ToString("f0").Replace(" ", string.Empty);
      } else {
        var val = (Ratio)AreaModifier;
        if (val.DecimalFractions != 1) {
          a += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          a = "X";
        }
      }

      if (ApiSectionModifier.I11Modifier.Option == SectionModifierOptionType.TO) {
        var val = (AreaMomentOfInertia)I11Modifier;
        i11 += val.ToString("f0").Replace(" ", string.Empty);
      } else {
        var val = (Ratio)I11Modifier;
        if (val.DecimalFractions != 1) {
          i11 += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          i11 = "X";
        }
      }

      if (ApiSectionModifier.I22Modifier.Option == SectionModifierOptionType.TO) {
        var val = (AreaMomentOfInertia)I22Modifier;
        i22 += val.ToString("f0").Replace(" ", string.Empty);
      } else {
        var val = (Ratio)I22Modifier;
        if (val.DecimalFractions != 1) {
          i22 += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          i22 = "X";
        }
      }

      if (ApiSectionModifier.JModifier.Option == SectionModifierOptionType.TO) {
        var val = (AreaMomentOfInertia)JModifier;
        j += val.ToString("f0").Replace(" ", string.Empty);
      } else {
        var val = (Ratio)JModifier;
        if (val.DecimalFractions != 1) {
          j += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          j = "X";
        }
      }

      if (ApiSectionModifier.K11Modifier.Option == SectionModifierOptionType.TO) {
        k11 += ApiSectionModifier.K11Modifier.Value.ToString("f3") + "[-]";
      } else {
        Ratio val = K11Modifier;
        if (val.DecimalFractions != 1) {
          k11 += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          k11 = "X";
        }
      }

      if (ApiSectionModifier.K22Modifier.Option == SectionModifierOptionType.TO) {
        k22 += ApiSectionModifier.K22Modifier.Value.ToString("f3") + "[-]";
      } else {
        Ratio val = K22Modifier;
        if (val.DecimalFractions != 1) {
          k22 += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          k22 = "X";
        }
      }

      if (ApiSectionModifier.VolumeModifier.Option == SectionModifierOptionType.TO) {
        var val = (VolumePerLength)VolumeModifier;
        v += val.ToString("f0").Replace(" ", string.Empty);
      } else {
        var val = (Ratio)VolumeModifier;
        if (val.DecimalFractions != 1) {
          v += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          v = "X";
        }
      }

      if (AdditionalMass.Value != 0) {
        mass += AdditionalMass.ToString("f0").Replace(" ", string.Empty);
      } else {
        mass = "X";
      }

      switch (ApiSectionModifier.StressOption) {
        case SectionModifierStressType.NO_MOD:

          stress = "X";
          break;

        case SectionModifierStressType.USE_MOD:
          stress += "UseModified";
          break;

        default:
          stress += "UseUnmodified";
          break;
      }

      if (ApiSectionModifier.IsBendingAxesPrincipal) {
        axis = "BendingAxis(UsePringipal(u,v)";
      }

      if (ApiSectionModifier.IsReferencePointCentroid) {
        refPt = "AnalysisRefPt(UseCentroid)";
      }

      string innerDesc = string
       .Join(" ", a, i11, i22, j, k11, k22, v, mass, stress, axis, refPt)
       .Replace("X, ", string.Empty).Replace("X ", string.Empty).TrimStart(',').TrimStart(' ')
       .TrimEnd('X').TrimEnd(' ').TrimEnd(',').TrimSpaces();
      return innerDesc;
    }

    private static bool IsAttributeModified(SectionModifierAttribute attribute) {
      if (attribute.Option == SectionModifierOptionType.TO) {
        return true;
      }

      return attribute.Value != 1;
    }
  }
}
