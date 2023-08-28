using System;
using GsaAPI;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters {
  /// <summary>
  /// A Section Modifier is part of a <see cref="GsaSection"/> and can be used to modify property's analytical properties without changing the `Profile` or <see cref="GsaMaterial"/>. By default the Section Modifier is unmodified.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-sect-lib.html#modifiers">Section Modifiers</see> to read more.</para>
  /// </summary>
  public class GsaSectionModifier {
    public enum StressOptionType {
      NoCalculation,
      UseModified,
      UseUnmodified,
    }

    public LinearDensity AdditionalMass {
      get => new LinearDensity(_sectionModifier.AdditionalMass, LinearDensityUnit.KilogramPerMeter);
      set {
        CloneApiObject();
        _sectionModifier.AdditionalMass = value.As(LinearDensityUnit.KilogramPerMeter);
      }
    }
    public IQuantity AreaModifier {
      get {
        if (_sectionModifier.AreaModifier.Option == SectionModifierOptionType.BY) {
          return new Ratio(_sectionModifier.AreaModifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent);
        } else {
          return new Area(_sectionModifier.AreaModifier.Value, AreaUnit.SquareMeter).ToUnit(
            DefaultUnits.SectionAreaUnit);
        }
      }
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("AreaModifier must be either Area or Ratio");
        }

        CloneApiObject();
        _sectionModifier.AreaModifier = value.QuantityInfo.UnitType == typeof(AreaUnit) ?
          new SectionModifierAttribute(SectionModifierOptionType.TO,
            value.As(AreaUnit.SquareMeter)) :
          new SectionModifierAttribute(SectionModifierOptionType.BY,
            value.As(RatioUnit.DecimalFraction));
      }
    }
    public IQuantity I11Modifier {
      get {
        if (_sectionModifier.I11Modifier.Option == SectionModifierOptionType.BY) {
          return new Ratio(_sectionModifier.I11Modifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent);
        } else {
          return new AreaMomentOfInertia(_sectionModifier.I11Modifier.Value,
              AreaMomentOfInertiaUnit.MeterToTheFourth)
           .ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
        }
      }
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("I11Modifier must be either AreaMomentOfInertia or Ratio");
        }

        CloneApiObject();
        _sectionModifier.I11Modifier
          = value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit) ?
            new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(AreaMomentOfInertiaUnit.MeterToTheFourth)) :
            new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
      }
    }
    public IQuantity I22Modifier {
      get {
        if (_sectionModifier.I22Modifier.Option == SectionModifierOptionType.BY) {
          return new Ratio(_sectionModifier.I22Modifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent);
        }

        return new AreaMomentOfInertia(_sectionModifier.I22Modifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth)
         .ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        }

        CloneApiObject();
        _sectionModifier.I22Modifier
          = value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit) ?
            new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(AreaMomentOfInertiaUnit.MeterToTheFourth)) :
            new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
      }
    }
    public bool IsBendingAxesPrincipal {
      get => _sectionModifier.IsBendingAxesPrincipal;
      set {
        CloneApiObject();
        _sectionModifier.IsBendingAxesPrincipal = value;
      }
    }
    public bool IsModified
      => IsAttributeModified(_sectionModifier.AreaModifier)
        || IsAttributeModified(_sectionModifier.I11Modifier)
        || IsAttributeModified(_sectionModifier.I22Modifier)
        || IsAttributeModified(_sectionModifier.JModifier)
        || IsAttributeModified(_sectionModifier.K11Modifier)
        || IsAttributeModified(_sectionModifier.K22Modifier)
        || IsAttributeModified(_sectionModifier.VolumeModifier) || IsBendingAxesPrincipal
        || IsReferencePointCentroid || _sectionModifier.AdditionalMass != 0
        || _sectionModifier.StressOption != SectionModifierStressType.NO_MOD;
    public bool IsReferencePointCentroid {
      get => _sectionModifier.IsReferencePointCentroid;
      set {
        CloneApiObject();
        _sectionModifier.IsReferencePointCentroid = value;
      }
    }
    public IQuantity JModifier {
      get {
        if (_sectionModifier.JModifier.Option == SectionModifierOptionType.BY) {
          return new Ratio(_sectionModifier.JModifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent);
        } else {
          return new AreaMomentOfInertia(_sectionModifier.JModifier.Value,
              AreaMomentOfInertiaUnit.MeterToTheFourth)
           .ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
        }
      }
      set {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        } else {
          CloneApiObject();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit)) {
            _sectionModifier.JModifier = new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          } else {
            _sectionModifier.JModifier = new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
          }
        }
      }
    }
    public Ratio K11Modifier {
      get
        => _sectionModifier.K11Modifier.Option == SectionModifierOptionType.BY ?
          new Ratio(_sectionModifier.K11Modifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent) :
          new Ratio(_sectionModifier.K11Modifier.Value, RatioUnit.DecimalFraction);
      set {
        CloneApiObject();
        _sectionModifier.K11Modifier = value.Unit == RatioUnit.Percent ?
          new SectionModifierAttribute(SectionModifierOptionType.BY,
            value.As(RatioUnit
             .DecimalFraction)) : // assume that percentage unit is modify BY option
                                  // assume that all other than percentage unit is modify TO option
          new SectionModifierAttribute(SectionModifierOptionType.TO,
            value.As(RatioUnit.DecimalFraction));
      }
    }
    public Ratio K22Modifier {
      get
        => _sectionModifier.K22Modifier.Option == SectionModifierOptionType.BY ?
          new Ratio(_sectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent) :
          new Ratio(_sectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction);
      set {
        CloneApiObject();
        _sectionModifier.K22Modifier = value.Unit == RatioUnit.Percent ?
          new SectionModifierAttribute(SectionModifierOptionType.BY,
            value.As(RatioUnit
             .DecimalFraction)) : // assume that percentage unit is modify BY option
                                  // assume that all other than percentage unit is modify TO option
          new SectionModifierAttribute(SectionModifierOptionType.TO,
            value.As(RatioUnit.DecimalFraction));
      }
    }
    public StressOptionType StressOption {
      get {
        switch (_sectionModifier.StressOption) {
          case SectionModifierStressType.USE_MOD: return StressOptionType.UseModified;

          case SectionModifierStressType.USE_UNMOD: return StressOptionType.UseUnmodified;

          case SectionModifierStressType.NO_MOD:
          default:
            return StressOptionType.NoCalculation;
        }
      }
      set {
        CloneApiObject();
        switch (value) {
          case StressOptionType.UseModified:
            _sectionModifier.StressOption = SectionModifierStressType.USE_MOD;
            break;

          case StressOptionType.UseUnmodified:
            _sectionModifier.StressOption = SectionModifierStressType.USE_UNMOD;
            break;

          case StressOptionType.NoCalculation:
          default:
            _sectionModifier.StressOption = SectionModifierStressType.NO_MOD;
            break;
        }
      }
    }
    public IQuantity VolumeModifier {
      get {
        if (_sectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY) {
          return new Ratio(_sectionModifier.VolumeModifier.Value, RatioUnit.DecimalFraction).ToUnit(
            RatioUnit.Percent);
        } else {
          return new VolumePerLength(_sectionModifier.VolumeModifier.Value,
            VolumePerLengthUnit.CubicMeterPerMeter).ToUnit(DefaultUnits.VolumePerLengthUnit);
        }
      }
      set {
        if (value.QuantityInfo.UnitType != typeof(VolumePerLengthUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("VolumeModifier must be either VolumePerLength or Ratio");
        }

        CloneApiObject();
        _sectionModifier.VolumeModifier
          = value.QuantityInfo.UnitType == typeof(VolumePerLengthUnit) ?
            new SectionModifierAttribute(SectionModifierOptionType.TO,
              value.As(VolumePerLengthUnit.CubicMeterPerMeter)) :
            new SectionModifierAttribute(SectionModifierOptionType.BY,
              value.As(RatioUnit.DecimalFraction));
      }
    }
    internal SectionModifier _sectionModifier = new SectionModifier();

    public GsaSectionModifier() { }

    internal GsaSectionModifier(SectionModifier sectionModifier) {
      _sectionModifier = sectionModifier;
    }

    public GsaSectionModifier Clone() {
      var dup = new GsaSectionModifier {
        _sectionModifier = _sectionModifier,
      };
      dup.CloneApiObject();

      return dup;
    }
    public GsaSectionModifier Duplicate() {
      return this;
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

      if (_sectionModifier.AreaModifier.Option == SectionModifierOptionType.TO) {
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

      if (_sectionModifier.I11Modifier.Option == SectionModifierOptionType.TO) {
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

      if (_sectionModifier.I22Modifier.Option == SectionModifierOptionType.TO) {
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

      if (_sectionModifier.JModifier.Option == SectionModifierOptionType.TO) {
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

      if (_sectionModifier.K11Modifier.Option == SectionModifierOptionType.TO) {
        k11 += _sectionModifier.K11Modifier.Value.ToString("f3") + "[-]";
      } else {
        Ratio val = K11Modifier;
        if (val.DecimalFractions != 1) {
          k11 += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          k11 = "X";
        }
      }

      if (_sectionModifier.K22Modifier.Option == SectionModifierOptionType.TO) {
        k22 += _sectionModifier.K22Modifier.Value.ToString("f3") + "[-]";
      } else {
        Ratio val = K22Modifier;
        if (val.DecimalFractions != 1) {
          k22 += val.ToString("f0").Replace(" ", string.Empty);
        } else {
          k22 = "X";
        }
      }

      if (_sectionModifier.VolumeModifier.Option == SectionModifierOptionType.TO) {
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

      switch (_sectionModifier.StressOption) {
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

      if (_sectionModifier.IsBendingAxesPrincipal) {
        axis = "BendingAxis(UsePringipal(u,v)";
      }

      if (_sectionModifier.IsReferencePointCentroid) {
        refPt = "AnalysisRefPt(UseCentroid)";
      }

      string innerDesc = string
       .Join(" ", a.Trim(), i11.Trim(), i22.Trim(), j.Trim(), k11.Trim(), k22.Trim(), v.Trim(),
          mass.Trim(), stress.Trim(), axis.Trim(), refPt.Trim()).Replace("X, ", string.Empty)
       .Replace("X ", string.Empty).TrimStart(',').TrimStart(' ').TrimEnd('X').TrimEnd(' ')
       .TrimEnd(',').Replace("  ", " ");
      return innerDesc;
    }

    private static bool IsAttributeModified(SectionModifierAttribute attribute) {
      if (attribute.Option == SectionModifierOptionType.TO) {
        return true;
      }

      return attribute.Value != 1;
    }

    private void CloneApiObject() {
      var dup = new SectionModifier {
        AreaModifier
          = new SectionModifierAttribute(_sectionModifier.AreaModifier.Option,
            _sectionModifier.AreaModifier.Value),
        I11Modifier
          = new SectionModifierAttribute(_sectionModifier.I11Modifier.Option,
            _sectionModifier.I11Modifier.Value),
        I22Modifier
          = new SectionModifierAttribute(_sectionModifier.I22Modifier.Option,
            _sectionModifier.I22Modifier.Value),
        JModifier
          = new SectionModifierAttribute(_sectionModifier.JModifier.Option,
            _sectionModifier.JModifier.Value),
        K11Modifier
          = new SectionModifierAttribute(_sectionModifier.K11Modifier.Option,
            _sectionModifier.K11Modifier.Value),
        K22Modifier
          = new SectionModifierAttribute(_sectionModifier.K22Modifier.Option,
            _sectionModifier.K22Modifier.Value),
        VolumeModifier
          = new SectionModifierAttribute(_sectionModifier.VolumeModifier.Option,
            _sectionModifier.VolumeModifier.Value),
        AdditionalMass = _sectionModifier.AdditionalMass,
        StressOption = _sectionModifier.StressOption,
        IsBendingAxesPrincipal = _sectionModifier.IsBendingAxesPrincipal,
        IsReferencePointCentroid = _sectionModifier.IsReferencePointCentroid,
      };
      _sectionModifier = dup;
    }
  }
}
