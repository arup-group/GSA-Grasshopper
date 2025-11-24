using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters {
  /// <summary>
  /// An Offset will locally move the <see cref="GsaSection"/> or <see cref="GsaProperty2d"/> away from the centre of the local axis in <see cref="GsaElement1D"/>, <see cref="GsaMember1D"/>, <see cref="GsaElement2D"/>, or <see cref="GsaMember2D"/>.
  /// </summary>
  public class GsaOffset {
    public Length X1 { get; set; } = Length.Zero;
    public Length X2 { get; set; } = Length.Zero;
    public Length Y { get; set; } = Length.Zero;
    public Length Z { get; set; } = Length.Zero;

    public GsaOffset() { }

    public GsaOffset(double x1, double x2, double y, double z, LengthUnit unit = LengthUnit.Meter) {
      X1 = new Length(x1, unit);
      X2 = new Length(x2, unit);
      Y = new Length(y, unit);
      Z = new Length(z, unit);
    }

    public GsaOffset Duplicate() {
      return (GsaOffset)MemberwiseClone(); // all members are structs
    }

    public override string ToString() {
      LengthUnit unit = Z.Unit;
      string unitAbbreviation = Length.GetAbbreviation(unit);

      return "X1:" + X1.As(unit).ToString("g") + " X2:" + X2.As(unit).ToString("g") + " Y:"
        + Y.As(unit).ToString("g") + " Z:" + Z.As(unit).ToString("g") + " [" + unitAbbreviation
        + "]";
    }
  }
}
