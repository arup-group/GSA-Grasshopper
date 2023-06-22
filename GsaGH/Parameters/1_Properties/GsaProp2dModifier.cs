﻿using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using LengthUnit = OasysUnits.Units.LengthUnit;
using RatioUnit = OasysUnits.Units.RatioUnit;
using VolumeUnit = OasysUnits.Units.VolumeUnit;

namespace GsaGH.Parameters {
  public class GsaProp2dModifier {
    //public LinearDensity AdditionalMass {
    //  get => new LinearDensity(_propertyModifier.AdditionalMass, LinearDensityUnit.KilogramPerMeter);
    //  set {
    //    CloneApiObject();
    //    _propertyModifier.AdditionalMass = value.As(LinearDensityUnit.KilogramPerMeter);
    //  }
    //}


    public IQuantity AdditionalMass { get; set; }
    public IQuantity Bending { get; set; }
    public IQuantity InPlane { get; set; }
    //public IQuantity InPlaneModifier {
    //  get {
    //    if (_propertyModifier.InPlane.Option == Prop2DModifierOptionType.BY) {
    //      return new Ratio(_propertyModifier.InPlane.Value,
    //        RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
    //    } else {
    //      Area area = new Area(_propertyModifier.InPlane.Value,
    //        AreaUnit.SquareMeter).ToUnit(DefaultUnits.SectionAreaUnit);
    //      var length = new Length(1, UnitSystem.SI);
    //      return area / length;
    //    }
    //  }
    //  set {
    //    if (value.QuantityInfo.UnitType != typeof( LengthUnit) &
    //      value.QuantityInfo.UnitType != typeof(RatioUnit)) {
    //      throw new ArgumentException("In-plane modifier must be either Area per Length or Ratio");
    //    }
    //    CloneApiObject();
    //    _propertyModifier.InPlane = value.QuantityInfo.UnitType == typeof( LengthUnit) ?
    //      new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, value.As( LengthUnit.Meter)) :
    //      new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
    //  }
    //}

    public IQuantity Shear { get; set; }
    public IQuantity Volume { get; set; }

    public GsaProp2dModifier() { }

    internal GsaProp2dModifier(IQuantity additionalMass, IQuantity inPlane, IQuantity bending, IQuantity shear, IQuantity volume) {
      AdditionalMass = additionalMass;
      InPlane = inPlane;
      Shear = bending;
      Volume = shear;
      Volume = volume;
    }

    internal GsaProp2dModifier(Prop2DModifier apiModifier) {
      if (apiModifier.InPlane.Option == Prop2DModifierOptionType.BY) {
        InPlane = new Ratio(-1 * apiModifier.InPlane.Value, RatioUnit.DecimalFraction);
      } else {
        InPlane = new Length(apiModifier.InPlane.Value, LengthUnit.Meter);
      }
      if (apiModifier.Bending.Option == Prop2DModifierOptionType.BY) {
        Bending = new Ratio(-1 * apiModifier.Bending.Value, RatioUnit.DecimalFraction);
      } else {
        Bending = new Volume(apiModifier.Bending.Value, VolumeUnit.CubicMeter);
      }
      if (apiModifier.Shear.Option == Prop2DModifierOptionType.BY) {
        Shear = new Ratio(-1 * apiModifier.Shear.Value, RatioUnit.DecimalFraction);
      } else {
        Shear = new Length(apiModifier.Shear.Value, LengthUnit.Meter);
      }
      if (apiModifier.Volume.Option == Prop2DModifierOptionType.BY) {
        Volume = new Ratio(-1 * apiModifier.Volume.Value, RatioUnit.DecimalFraction);
      } else {
        Volume = new Length(apiModifier.Volume.Value, LengthUnit.Meter);
      }
      AdditionalMass = new LinearDensity(apiModifier.AdditionalMass, LinearDensityUnit.KilogramPerMeter); // we need to create a new OasysUnit here!
    }

    public GsaProp2dModifier Duplicate() {
      var dup = new GsaProp2dModifier {
        InPlane = InPlane,
        Bending = Bending,
        Shear = Shear,
        Volume = Volume,
        AdditionalMass = AdditionalMass,
      };
      return dup;
    }

    public override string ToString() {
      string inPlane = "In-plane:";
      string bending = "Bending:";
      string shear = "Shear:";
      string volume = "Volume:";
      string mass = "Add.Mass:";

      if (InPlane is Length inPlaneQuantity) {
        inPlane += InPlane.ToString().Replace(" ", string.Empty) + "\u00B2" + "/" + Length.GetAbbreviation(inPlaneQuantity.Unit);
      } else if (InPlane is Ratio ratio) {
        if (ratio.DecimalFractions != 1) {
          inPlane += InPlane.ToString().Replace(" ", string.Empty);
        } else {
          inPlane = "X";
        }
      }

      if (Bending is Volume bendingQuantity) {
        bending += Bending.ToString().Replace(" ", string.Empty);
        bending = bending.Remove(bending.Length - 1, 1);
        bending += "\u2074" + "/" + OasysUnits.Volume.GetAbbreviation(bendingQuantity.Unit);
        bending = bending.Remove(bending.Length - 1, 1);
      } else if (Bending is Ratio ratio) {
        if (ratio.DecimalFractions != 1) {
          bending += ratio.ToString("f0").Replace(" ", string.Empty);
        } else {
          bending = "X";
        }
      }

      if (Shear is Length shearQuantity) {
        shear += Shear.ToString().Replace(" ", string.Empty) + "\u00B2" + "/" + Length.GetAbbreviation(shearQuantity.Unit);
      } else if (Shear is Ratio ratio) {
        if (ratio.DecimalFractions != 1) {
          shear += ratio.ToString("f0").Replace(" ", string.Empty);
        } else {
          shear = "X";
        }
      }

      if (Volume is Length volumeQuantity) {
        volume += Volume.ToString().Replace(" ", string.Empty) + "\u00B3" + "/" + Length.GetAbbreviation(volumeQuantity.Unit) + "\u00B2";
      } else if (Volume is Ratio ratio) {
        if (ratio.DecimalFractions != 1) {
          volume += ratio.ToString("f0").Replace(" ", string.Empty);
        } else {
          volume = "X";
        }
      }

      if (AdditionalMass.Value != 0) {
        mass += AdditionalMass.ToString().Replace(" ", string.Empty);
      } else {
        mass = "X";
      }

      string innerDesc = string
       .Join(" ", inPlane.Trim(), bending.Trim(), shear.Trim(), volume.Trim(), mass.Trim()).
       Replace("X, ", string.Empty).Replace("X ", string.Empty).TrimStart(',').TrimStart(' ').
       TrimEnd('X').TrimEnd(' ').TrimEnd(',').Replace("  ", " ");
      return innerDesc;
    }
  }
}
