using GsaAPI;
using OasysUnits;

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
    //    if (value.QuantityInfo.UnitType != typeof(OasysUnits.Units.LengthUnit) &
    //      value.QuantityInfo.UnitType != typeof(RatioUnit)) {
    //      throw new ArgumentException("In-plane modifier must be either Area per Length or Ratio");
    //    }
    //    CloneApiObject();
    //    _propertyModifier.InPlane = value.QuantityInfo.UnitType == typeof(OasysUnits.Units.LengthUnit) ?
    //      new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, value.As(OasysUnits.Units.LengthUnit.Meter)) :
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

    public GsaProp2dModifier(Prop2DModifier apiModifier) {
      if (apiModifier.InPlane.Option == Prop2DModifierOptionType.BY) {
        InPlane = new Ratio(-1 * apiModifier.InPlane.Value, OasysUnits.Units.RatioUnit.DecimalFraction);
      } else {
        InPlane = new Length(apiModifier.InPlane.Value, OasysUnits.Units.LengthUnit.Meter);
      }
      if (apiModifier.Bending.Option == Prop2DModifierOptionType.BY) {
        Bending = new Ratio(-1 * apiModifier.Bending.Value, OasysUnits.Units.RatioUnit.DecimalFraction);
      } else {
        Bending = new Volume(apiModifier.Bending.Value, OasysUnits.Units.VolumeUnit.CubicMeter);
      }
      if (apiModifier.Shear.Option == Prop2DModifierOptionType.BY) {
        Shear = new Ratio(-1 * apiModifier.Shear.Value, OasysUnits.Units.RatioUnit.DecimalFraction);
      } else {
        Shear = new Length(apiModifier.Shear.Value, OasysUnits.Units.LengthUnit.Meter);
      }
      if (apiModifier.Volume.Option == Prop2DModifierOptionType.BY) {
        Volume = new Ratio(-1 * apiModifier.Volume.Value, OasysUnits.Units.RatioUnit.DecimalFraction);
      } else {
        Volume = new Length(apiModifier.Volume.Value, OasysUnits.Units.LengthUnit.Meter);
      }
      AdditionalMass = new Density(apiModifier.AdditionalMass,
        OasysUnits.Units.DensityUnit.KilogramPerCubicMeter);
    }

    public override string ToString() {
      string inPlane = "In-plane:";
      string bending = "Bending:";
      string shear = "Shear:";
      string volume = "Volume:";
      string mass = "Add.Mass:";

      if (InPlane is Area) {
        inPlane += InPlane.ToString().Replace(" ", string.Empty);
      } else if (InPlane is Ratio ratio) {
        if (ratio.DecimalFractions != 1) {
          inPlane += InPlane.ToString().Replace(" ", string.Empty);
        } else {
          inPlane = "X";
        }
      }

      if (Bending is Volume) {
        bending += Bending.ToString().Replace(" ", string.Empty);
      } else if (Bending is Ratio ratio) {
        if (ratio.DecimalFractions != 1) {
          bending += ratio.ToString("f0").Replace(" ", string.Empty);
        } else {
          bending = "X";
        }
      }

      if (Shear is Length) {
        shear += Shear.ToString().Replace(" ", string.Empty);
      } else if (Shear is Ratio ratio) {
        if (ratio.DecimalFractions != 1) {
          shear += ratio.ToString("f0").Replace(" ", string.Empty);
        } else {
          shear = "X";
        }
      }

      if (Volume is Length) {
        volume += Volume.ToString().Replace(" ", string.Empty);
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
