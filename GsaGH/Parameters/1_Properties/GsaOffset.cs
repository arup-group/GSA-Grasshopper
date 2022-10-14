using System;
using System.Linq;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Offset class, this class defines the basic properties and methods for any Gsa Offset
  /// </summary>
  public class GsaOffset
  {
    #region properties
    public Length X1 { get; set; } = Length.Zero;
    public Length X2 { get; set; } = Length.Zero;
    public Length Y { get; set; } = Length.Zero;
    public Length Z { get; set; } = Length.Zero;
    #endregion

    #region constructors
    public GsaOffset()
    {
    }

    public GsaOffset(double x1, double x2, double y, double z, LengthUnit unit = LengthUnit.Meter)
    {
      this.X1 = new Length(x1, unit);
      this.X2 = new Length(x2, unit);
      this.Y = new Length(y, unit);
      this.Z = new Length(z, unit);
    }

    public GsaOffset Duplicate()
    {
      return (GsaOffset)this.MemberwiseClone(); // all members are structs
    }
    #endregion

    #region methods
    public override string ToString()
    {
      IQuantity quantity = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      return "X1:"
          + Math.Round(X1.As(DefaultUnits.LengthUnitGeometry), 4) + ", X2:"
          + Math.Round(X2.As(DefaultUnits.LengthUnitGeometry), 4) + ", Y:"
          + Math.Round(Y.As(DefaultUnits.LengthUnitGeometry), 4) + ", Z:"
          + Math.Round(Z.As(DefaultUnits.LengthUnitGeometry), 4) + " [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
