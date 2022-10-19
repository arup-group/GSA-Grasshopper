using System;
using GsaAPI;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Section Modifier class, this class defines the basic properties and methods for any Gsa Offset
  /// </summary>
  public class GsaSectionModifier
  {
    public enum StressOptionType
    {
      NoCalculation,
      UseModified,
      UseUnmodified
    }

    internal SectionModifier _sectionModifier = new SectionModifier();

    #region properties
    public bool IsModified
    {
      get
      {
        if (IsAttributeModified(this._sectionModifier.AreaModifier))
          return true;
        if (IsAttributeModified(this._sectionModifier.I11Modifier))
          return true;
        if (IsAttributeModified(this._sectionModifier.I22Modifier))
          return true;
        if (IsAttributeModified(this._sectionModifier.JModifier))
          return true;
        if (IsAttributeModified(this._sectionModifier.K11Modifier))
          return true;
        if (IsAttributeModified(this._sectionModifier.K22Modifier))
          return true;
        if (IsAttributeModified(this._sectionModifier.VolumeModifier))
          return true;
        if (IsBendingAxesPrincipal)
          return true;
        if (IsReferencePointCentroid)
          return true;
        if (this._sectionModifier.AdditionalMass != 0)
          return true;
        if (this._sectionModifier.StressOption != SectionModifierStressType.NO_MOD)
          return true;
        return false;
      }
    }
    private bool IsAttributeModified(SectionModifierAttribute attribute)
    {
      if (attribute.Option == SectionModifierOptionType.TO)
        return true;
      if (attribute.Value != 1)
        return true;
      else
        return false;
    }
    public IQuantity AreaModifier
    {
      get
      {
        if (this._sectionModifier.AreaModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this._sectionModifier.AreaModifier.Value, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Area(this._sectionModifier.AreaModifier.Value, AreaUnit.SquareMeter).ToUnit(DefaultUnits.SectionAreaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("AreaModifier must be either Area or Ratio");
        else
        {
          this.CloneApiObject();
          if (value.QuantityInfo.UnitType == typeof(AreaUnit))
            this._sectionModifier.AreaModifier = new SectionModifierAttribute(SectionModifierOptionType.TO, value.As(AreaUnit.SquareMeter));
          else
            this._sectionModifier.AreaModifier = new SectionModifierAttribute(SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }
    public IQuantity I11Modifier
    {
      get
      {
        if (this._sectionModifier.I11Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this._sectionModifier.I11Modifier.Value, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this._sectionModifier.I11Modifier.Value, AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I11Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          CloneApiObject();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            this._sectionModifier.I11Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            this._sectionModifier.I11Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }
    public IQuantity I22Modifier
    {
      get
      {
        if (this._sectionModifier.I22Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this._sectionModifier.I22Modifier.Value, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this._sectionModifier.I22Modifier.Value, AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          this.CloneApiObject();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            this._sectionModifier.I22Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            this._sectionModifier.I22Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public IQuantity JModifier
    {
      get
      {
        if (this._sectionModifier.JModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this._sectionModifier.JModifier.Value, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this._sectionModifier.JModifier.Value, AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          this.CloneApiObject();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            this._sectionModifier.JModifier = new SectionModifierAttribute(SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            this._sectionModifier.JModifier = new SectionModifierAttribute(SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }
    public IQuantity VolumeModifier
    {
      get
      {
        if (this._sectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this._sectionModifier.VolumeModifier.Value, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new VolumePerLength(this._sectionModifier.VolumeModifier.Value, VolumePerLengthUnit.CubicMeterPerMeter).ToUnit(DefaultUnits.VolumePerLengthUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(VolumePerLengthUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("VolumeModifier must be either VolumePerLength or Ratio");
        else
        {
          this.CloneApiObject();
          if (value.QuantityInfo.UnitType == typeof(VolumePerLengthUnit))
            this._sectionModifier.VolumeModifier = new SectionModifierAttribute(SectionModifierOptionType.TO, value.As(VolumePerLengthUnit.CubicMeterPerMeter));
          else
            this._sectionModifier.VolumeModifier = new SectionModifierAttribute(SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public Ratio K11Modifier
    {
      get
      {
        if (this._sectionModifier.K11Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this._sectionModifier.K11Modifier.Value, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Ratio(this._sectionModifier.K11Modifier.Value, RatioUnit.DecimalFraction);
      }
      set
      {
        this.CloneApiObject();
        if (value.Unit == RatioUnit.Percent) // assume that percentage unit is modify BY option
          this._sectionModifier.K11Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        else // assume that all other than percentage unit is modify TO option
          this._sectionModifier.K11Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, value.As(RatioUnit.DecimalFraction));
      }
    }
    public Ratio K22Modifier
    {
      get
      {
        if (this._sectionModifier.K22Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this._sectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Ratio(this._sectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction);
      }
      set
      {
        CloneApiObject();
        if (value.Unit == RatioUnit.Percent) // assume that percentage unit is modify BY option
          this._sectionModifier.K22Modifier = new SectionModifierAttribute(SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        else // assume that all other than percentage unit is modify TO option
          this._sectionModifier.K22Modifier = new SectionModifierAttribute(SectionModifierOptionType.TO, value.As(RatioUnit.DecimalFraction));
      }
    }
    public LinearDensity AdditionalMass
    {
      get
      {
        return new LinearDensity(this._sectionModifier.AdditionalMass, LinearDensityUnit.KilogramPerMeter);
      }
      set
      {
        this.CloneApiObject();
        this._sectionModifier.AdditionalMass = value.As(LinearDensityUnit.KilogramPerMeter);
      }
    }

    public StressOptionType StressOption
    {
      get
      {
        switch (this._sectionModifier.StressOption)
        {
          case SectionModifierStressType.USE_MOD:
            return StressOptionType.UseModified;
          case SectionModifierStressType.USE_UNMOD:
            return StressOptionType.UseUnmodified;
          case SectionModifierStressType.NO_MOD:
          default:
            return StressOptionType.NoCalculation;
        }
      }
      set
      {
        CloneApiObject();
        switch (value)
        {
          case StressOptionType.UseModified:
            this._sectionModifier.StressOption = SectionModifierStressType.USE_MOD;
            break;
          case StressOptionType.UseUnmodified:
            this._sectionModifier.StressOption = SectionModifierStressType.USE_UNMOD;
            break;
          case StressOptionType.NoCalculation:
          default:
            this._sectionModifier.StressOption = SectionModifierStressType.NO_MOD;
            break;
        }
      }
    }

    public bool IsBendingAxesPrincipal
    {
      get
      {
        return this._sectionModifier.IsBendingAxesPrincipal;
      }
      set
      {
        CloneApiObject();
        this._sectionModifier.IsBendingAxesPrincipal = value;
      }
    }
    public bool IsReferencePointCentroid
    {
      get
      {
        return this._sectionModifier.IsReferencePointCentroid;
      }
      set
      {
        CloneApiObject();
        this._sectionModifier.IsReferencePointCentroid = value;
      }
    }

    private void CloneApiObject()
    {
      SectionModifier dup = new SectionModifier();
      dup.AreaModifier = new SectionModifierAttribute(this._sectionModifier.AreaModifier.Option, this._sectionModifier.AreaModifier.Value);
      dup.I11Modifier = new SectionModifierAttribute(this._sectionModifier.I11Modifier.Option, this._sectionModifier.I11Modifier.Value);
      dup.I22Modifier = new SectionModifierAttribute(this._sectionModifier.I22Modifier.Option, this._sectionModifier.I22Modifier.Value);
      dup.JModifier = new SectionModifierAttribute(this._sectionModifier.JModifier.Option, this._sectionModifier.JModifier.Value);
      dup.K11Modifier = new SectionModifierAttribute(this._sectionModifier.K11Modifier.Option, this._sectionModifier.K11Modifier.Value);
      dup.K22Modifier = new SectionModifierAttribute(this._sectionModifier.K22Modifier.Option, this._sectionModifier.K22Modifier.Value);
      dup.VolumeModifier = new SectionModifierAttribute(this._sectionModifier.VolumeModifier.Option, this._sectionModifier.VolumeModifier.Value);
      dup.AdditionalMass = this._sectionModifier.AdditionalMass;
      dup.StressOption = this._sectionModifier.StressOption;
      dup.IsBendingAxesPrincipal = this._sectionModifier.IsBendingAxesPrincipal;
      dup.IsReferencePointCentroid = this._sectionModifier.IsReferencePointCentroid;
      this._sectionModifier = dup;
    }
    #endregion

    #region constructors
    public GsaSectionModifier()
    {
    }

    internal GsaSectionModifier(SectionModifier sectionModifier)
    {
      this._sectionModifier = sectionModifier;
    }
    #endregion

    #region methods
    public GsaSectionModifier Duplicate()
    {
      GsaSectionModifier dup = new GsaSectionModifier();
      dup._sectionModifier = this._sectionModifier;
      dup.CloneApiObject();
      return dup;
    }

    public override string ToString()
    {
      if (!this.IsModified)
      {
        return "Unmodified";
      }
      string A = "A(";
      string I11 = "I11(";
      string I22 = "I22(";
      string J = "J(";
      string K11 = "K11(";
      string K22 = "K22(";
      string V = "V(";
      string mass = "Add.Mass(";
      string stress = "StressCalc.Opt.(";
      string axis = "X";
      string refPt = "X";

      // Area
      if (this._sectionModifier.AreaModifier.Option == SectionModifierOptionType.TO)
      {
        Area val = (Area)this.AreaModifier;
        A += val.ToString("f0").Replace(" ", string.Empty) + ")";
      }
      else
      {
        Ratio val = (Ratio)this.AreaModifier;
        if (val.DecimalFractions != 1)
          A += val.ToString("f0").Replace(" ", string.Empty) + ")";
        else
          A = "X";
      }

      // I11
      if (this._sectionModifier.I11Modifier.Option == SectionModifierOptionType.TO)
      {
        AreaMomentOfInertia val = (AreaMomentOfInertia)this.I11Modifier;
        I11 += val.ToString("f0").Replace(" ", string.Empty) + ")";
      }
      else
      {
        Ratio val = (Ratio)this.I11Modifier;
        if (val.DecimalFractions != 1)
          I11 += val.ToString("f0").Replace(" ", string.Empty) + ")";
        else
          I11 = "X";
      }

      // I22
      if (this._sectionModifier.I22Modifier.Option == SectionModifierOptionType.TO)
      {
        AreaMomentOfInertia val = (AreaMomentOfInertia)this.I22Modifier;
        I22 += val.ToString("f0").Replace(" ", string.Empty) + ")";
      }
      else
      {
        Ratio val = (Ratio)this.I22Modifier;
        if (val.DecimalFractions != 1)
          I22 += val.ToString("f0").Replace(" ", string.Empty) + ")";
        else
          I22 = "X";
      }

      // J
      if (this._sectionModifier.JModifier.Option == SectionModifierOptionType.TO)
      {
        AreaMomentOfInertia val = (AreaMomentOfInertia)this.JModifier;
        J += val.ToString("f0").Replace(" ", string.Empty) + ")";
      }
      else
      {
        Ratio val = (Ratio)this.JModifier;
        if (val.DecimalFractions != 1)
          J += val.ToString("f0").Replace(" ", string.Empty) + ")";
        else
          J = "X";
      }

      // K11
      if (this._sectionModifier.K11Modifier.Option == SectionModifierOptionType.TO)
      {
        K11 += this._sectionModifier.K11Modifier.Value.ToString("f3") + "[-])";
      }
      else
      {
        Ratio val = this.K11Modifier;
        if (val.DecimalFractions != 1)
          K11 += val.ToString("f0").Replace(" ", string.Empty) + ")";
        else
          K11 = "X";
      }

      // K22
      if (this._sectionModifier.K22Modifier.Option == SectionModifierOptionType.TO)
      {
        K22 += this._sectionModifier.K22Modifier.Value.ToString("f3") + "[-])";
      }
      else
      {
        Ratio val = this.K22Modifier;
        if (val.DecimalFractions != 1)
          K22 += val.ToString("f0").Replace(" ", string.Empty) + ")";
        else
          K22 = "X";
      }

      // Volume
      if (this._sectionModifier.VolumeModifier.Option == SectionModifierOptionType.TO)
      {
        VolumePerLength val = (VolumePerLength)this.VolumeModifier;
        V += val.ToString("f0").Replace(" ", string.Empty) + ")";
      }
      else
      {
        Ratio val = (Ratio)this.VolumeModifier;
        if (val.DecimalFractions != 1)
          V += val.ToString("f0").Replace(" ", string.Empty) + ")";
        else
          V = "X";
      }

      // Additional Mass
      if (this.AdditionalMass.Value != 0)
        mass += this.AdditionalMass.ToString("f0").Replace(" ", string.Empty) + ")";
      else
        mass = "X";

      if (this._sectionModifier.StressOption == SectionModifierStressType.NO_MOD)
        stress = "X";
      else if (this._sectionModifier.StressOption == SectionModifierStressType.USE_MOD)
        stress += "UseModified)";
      else
        stress += "UseUnmodified)";

      if (this._sectionModifier.IsBendingAxesPrincipal)
        axis = "BendingAxis(UsePringipal(u,v))";
      if (this._sectionModifier.IsReferencePointCentroid)
        refPt = "AnalysisRefPt(UseCentroid)";

      string innerDesc = string.Join(", ", A, I11, I22, J, K11, K22, V, mass, stress, axis, refPt).Replace("X, ", string.Empty).TrimStart(',').TrimStart(' ').TrimEnd('X').TrimEnd(' ').TrimEnd(',');
      return innerDesc;
    }
    #endregion
  }
}
