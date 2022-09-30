﻿using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH;
using OasysGH.Parameters;

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
    internal SectionModifier API_SectionModifier;

    public bool isModified
    {
      get
      {
        if (API_SectionModifier == null) return false;
        if (isAttributeModified(this.API_SectionModifier.AreaModifier))
          return true;
        if (isAttributeModified(this.API_SectionModifier.I11Modifier))
          return true;
        if (isAttributeModified(this.API_SectionModifier.I22Modifier))
          return true;
        if (isAttributeModified(this.API_SectionModifier.JModifier))
          return true;
        if (isAttributeModified(this.API_SectionModifier.K11Modifier))
          return true;
        if (isAttributeModified(this.API_SectionModifier.K22Modifier))
          return true;
        if (isAttributeModified(this.API_SectionModifier.VolumeModifier))
          return true;
        if (isBendingAxesPrincipal)
          return true;
        if (isReferencePointCentroid)
          return true;
        if (this.API_SectionModifier.AdditionalMass != 0)
          return true;
        if (this.API_SectionModifier.StressOption != SectionModifierStressType.NO_MOD)
          return true;
        return false;
      }
    }
    private bool isAttributeModified(SectionModifierAttribute attribute)
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
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.AreaModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.AreaModifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Area(this.API_SectionModifier.AreaModifier.Value, AreaUnit.SquareMeter).ToUnit(DefaultUnits.SectionAreaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("AreaModifier must be either Area or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaUnit))
            API_SectionModifier.AreaModifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaUnit.SquareMeter));
          else
            API_SectionModifier.AreaModifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public IQuantity I11Modifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.I11Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.I11Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this.API_SectionModifier.I11Modifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I11Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            API_SectionModifier.I11Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            API_SectionModifier.I11Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }
    public IQuantity I22Modifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.I22Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.I22Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this.API_SectionModifier.I22Modifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            API_SectionModifier.I22Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            API_SectionModifier.I22Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public IQuantity JModifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.JModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.JModifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this.API_SectionModifier.JModifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            API_SectionModifier.JModifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            API_SectionModifier.JModifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }
    public IQuantity VolumeModifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.VolumeModifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new VolumePerLength(this.API_SectionModifier.VolumeModifier.Value,
            VolumePerLengthUnit.CubicMeterPerMeter).ToUnit(DefaultUnits.VolumePerLengthUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(VolumePerLengthUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("VolumeModifier must be either VolumePerLength or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(VolumePerLengthUnit))
            API_SectionModifier.VolumeModifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(VolumePerLengthUnit.CubicMeterPerMeter));
          else
            API_SectionModifier.VolumeModifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public Ratio K11Modifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.K11Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.K11Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Ratio(this.API_SectionModifier.K11Modifier.Value, RatioUnit.DecimalFraction);
      }
      set
      {
        CloneAPIModifier();
        if (value.Unit == RatioUnit.Percent) // assume that percentage unit is modify BY option
          API_SectionModifier.K11Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        else // assume that all other than percentage unit is modify TO option
          API_SectionModifier.K11Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.TO, value.As(RatioUnit.DecimalFraction));
      }
    }
    public Ratio K22Modifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.K22Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.K22Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Ratio(this.API_SectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction);
      }
      set
      {
        CloneAPIModifier();
        if (value.Unit == RatioUnit.Percent) // assume that percentage unit is modify BY option
          API_SectionModifier.K22Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        else // assume that all other than percentage unit is modify TO option
          API_SectionModifier.K22Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.TO, value.As(RatioUnit.DecimalFraction));
      }
    }
    public LinearDensity AdditionalMass
    {
      get
      {
        if (this.API_SectionModifier == null) { return LinearDensity.Zero; }
        return new LinearDensity(this.API_SectionModifier.AdditionalMass, LinearDensityUnit.KilogramPerMeter);
      }
      set
      {
        CloneAPIModifier();
        API_SectionModifier.AdditionalMass = value.As(LinearDensityUnit.KilogramPerMeter);
      }
    }

    public StressOptionType StressOption
    {
      get
      {
        switch (this.API_SectionModifier.StressOption)
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
        CloneAPIModifier();
        switch (value)
        {
          case StressOptionType.UseModified:
            this.API_SectionModifier.StressOption = SectionModifierStressType.USE_MOD;
            break;
          case StressOptionType.UseUnmodified:
            this.API_SectionModifier.StressOption = SectionModifierStressType.USE_UNMOD;
            break;
          case StressOptionType.NoCalculation:
          default:
            this.API_SectionModifier.StressOption = SectionModifierStressType.NO_MOD;
            break;
        }
      }
    }

    public bool isBendingAxesPrincipal
    {
      get
      {
        return this.API_SectionModifier.IsBendingAxesPrincipal;
      }
      set
      {
        CloneAPIModifier();
        this.API_SectionModifier.IsBendingAxesPrincipal = value;
      }
    }
    public bool isReferencePointCentroid
    {
      get
      {
        return this.API_SectionModifier.IsReferencePointCentroid;
      }
      set
      {
        CloneAPIModifier();
        this.API_SectionModifier.IsReferencePointCentroid = value;
      }
    }

    private void CloneAPIModifier()
    {
      if (this.API_SectionModifier == null)
      {
        this.API_SectionModifier = new SectionModifier();
        return;
      }

      SectionModifier dup = new SectionModifier();
      dup.AreaModifier = new SectionModifierAttribute(this.API_SectionModifier.AreaModifier.Option, this.API_SectionModifier.AreaModifier.Value);
      dup.I11Modifier = new SectionModifierAttribute(this.API_SectionModifier.I11Modifier.Option, this.API_SectionModifier.I11Modifier.Value);
      dup.I22Modifier = new SectionModifierAttribute(
        this.API_SectionModifier.I22Modifier.Option, this.API_SectionModifier.I22Modifier.Value);
      dup.JModifier = new SectionModifierAttribute(this.API_SectionModifier.JModifier.Option, this.API_SectionModifier.JModifier.Value);
      dup.K11Modifier = new SectionModifierAttribute(this.API_SectionModifier.K11Modifier.Option, this.API_SectionModifier.K11Modifier.Value);
      dup.K22Modifier = new SectionModifierAttribute(this.API_SectionModifier.K22Modifier.Option, this.API_SectionModifier.K22Modifier.Value);
      dup.VolumeModifier = new SectionModifierAttribute(this.API_SectionModifier.VolumeModifier.Option, this.API_SectionModifier.VolumeModifier.Value);
      dup.AdditionalMass = this.API_SectionModifier.AdditionalMass;
      dup.StressOption = this.API_SectionModifier.StressOption;
      dup.IsBendingAxesPrincipal = this.API_SectionModifier.IsBendingAxesPrincipal;
      dup.IsReferencePointCentroid = this.API_SectionModifier.IsReferencePointCentroid;
      this.API_SectionModifier = dup;
    }

    #region constructors
    public GsaSectionModifier()
    {
      // empty constructor
    }

    internal GsaSectionModifier(SectionModifier apiMod)
    {
      this.API_SectionModifier = apiMod;
    }

    public GsaSectionModifier Duplicate()
    {
      if (this == null) { return null; }
      return this;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (this == null) { return false; }
        return true;
      }
    }
    #endregion

    #region methods

    public override string ToString()
    {
      if (!this.isModified)
      {
        return "Section Modifier {Unmodified}";
      }
      string str = "Section Modifier ";
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
      if (this.API_SectionModifier.AreaModifier.Option == SectionModifierOptionType.TO)
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
      if (this.API_SectionModifier.I11Modifier.Option == SectionModifierOptionType.TO)
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
      if (this.API_SectionModifier.I22Modifier.Option == SectionModifierOptionType.TO)
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
      if (this.API_SectionModifier.JModifier.Option == SectionModifierOptionType.TO)
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
      if (this.API_SectionModifier.K11Modifier.Option == SectionModifierOptionType.TO)
      {
        K11 += this.API_SectionModifier.K11Modifier.Value.ToString("f3") + "[-])";
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
      if (this.API_SectionModifier.K22Modifier.Option == SectionModifierOptionType.TO)
      {
        K22 += this.API_SectionModifier.K22Modifier.Value.ToString("f3") + "[-])";
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
      if (this.API_SectionModifier.VolumeModifier.Option == SectionModifierOptionType.TO)
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

      if (this.API_SectionModifier.StressOption == SectionModifierStressType.NO_MOD)
        stress = "X";
      else if (this.API_SectionModifier.StressOption == SectionModifierStressType.USE_MOD)
        stress += "UseModified)";
      else
        stress += "UseUnmodified)";

      if (this.API_SectionModifier.IsBendingAxesPrincipal)
        axis = "BendingAxis(UsePringipal(u,v))";
      if (this.API_SectionModifier.IsReferencePointCentroid)
        refPt = "AnalysisRefPt(UseCentroid)";

      string innerDesc = string.Join(", ", A, I11, I22, J, K11, K22, V, mass, stress, axis, refPt).Replace("X, ", string.Empty).TrimStart(',').TrimStart(' ').TrimEnd('X').TrimEnd(' ').TrimEnd(',');
      return str + "{" + innerDesc + "}";
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaSectionModifier"/> can be used in Grasshopper.
  /// </summary>
  public class GsaSectionModifierGoo : GH_OasysGoo<GsaSectionModifier>
  {
    public static string Name => "Section Modifier";
    public static string NickName => "SM";
    public static string Description => "GSA Section Modifier";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaSectionModifierGoo(GsaSectionModifier item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaSectionModifierGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaSectionModifier)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(SectionModifier)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from other Section Modifier
      else if (typeof(GsaSectionModifier).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaSectionModifier)source;
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
