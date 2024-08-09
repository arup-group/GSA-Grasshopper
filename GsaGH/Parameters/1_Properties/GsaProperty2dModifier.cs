using GsaAPI;

using GsaGH.Helpers;

using OasysUnits;
using OasysUnits.Units;

using LengthUnit = OasysUnits.Units.LengthUnit;
using RatioUnit = OasysUnits.Units.RatioUnit;
using VolumeUnit = OasysUnits.Units.VolumeUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// A 2D Property Modifier is part of a <see cref="GsaProperty2d"/> and can be used to modify property's analytical properties without changing the `Thickness` or <see cref="GsaMaterial"/>. By default the 2D Property Modifier is unmodified.
  /// </summary>
  public class GsaProperty2dModifier {
    public IQuantity AdditionalMass { get; set; } = new AreaDensity();
    public IQuantity Bending { get; set; } = new Ratio(100, RatioUnit.Percent);
    public IQuantity InPlane { get; set; } = new Ratio(100, RatioUnit.Percent);
    public IQuantity Shear { get; set; } = new Ratio(100, RatioUnit.Percent);
    public IQuantity Volume { get; set; } = new Ratio(100, RatioUnit.Percent);

    public GsaProperty2dModifier() { }

    internal GsaProperty2dModifier(Prop2DModifier apiModifier) {
      if (apiModifier.InPlane.Option == Prop2DModifierOptionType.BY) {
        InPlane = new Ratio(100 * apiModifier.InPlane.Value, RatioUnit.Percent);
      } else {
        InPlane = new Length(apiModifier.InPlane.Value, LengthUnit.Meter);
      }
      if (apiModifier.Bending.Option == Prop2DModifierOptionType.BY) {
        Bending = new Ratio(100 * apiModifier.Bending.Value, RatioUnit.Percent);
      } else {
        Bending = new Volume(apiModifier.Bending.Value, VolumeUnit.CubicMeter);
      }
      if (apiModifier.Shear.Option == Prop2DModifierOptionType.BY) {
        Shear = new Ratio(100 * apiModifier.Shear.Value, RatioUnit.Percent);
      } else {
        Shear = new Length(apiModifier.Shear.Value, LengthUnit.Meter);
      }
      if (apiModifier.Volume.Option == Prop2DModifierOptionType.BY) {
        Volume = new Ratio(100 * apiModifier.Volume.Value, RatioUnit.Percent);
      } else {
        Volume = new Length(apiModifier.Volume.Value, LengthUnit.Meter);
      }
      AdditionalMass = new AreaDensity(apiModifier.AdditionalMass, AreaDensityUnit.KilogramPerSquareMeter);
    }

    public GsaProperty2dModifier Duplicate() {
      var dup = new GsaProperty2dModifier {
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
       .Join(" ", inPlane, bending, shear, volume, mass).
       Replace("X, ", string.Empty).Replace("X ", string.Empty).TrimStart(',').TrimStart(' ').
       TrimEnd('X').TrimEnd(' ').TrimEnd(',').TrimSpaces();
      if (innerDesc == string.Empty) {
        innerDesc = "Unmodified";
      }

      return innerDesc;
    }
  }
}
