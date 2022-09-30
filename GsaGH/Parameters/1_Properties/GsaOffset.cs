using System;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
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
    public Length X1 { get; set; } = Length.Zero;
    public Length X2 { get; set; } = Length.Zero;
    public Length Y { get; set; } = Length.Zero;
    public Length Z { get; set; } = Length.Zero;

    #region constructors
    public GsaOffset()
    {
      // empty constructor
    }

    public GsaOffset(double x1, double x2, double y, double z, LengthUnit unit = LengthUnit.Meter)
    {
      X1 = new Length(x1, unit);
      X2 = new Length(x2, unit);
      Y = new Length(y, unit);
      Z = new Length(z, unit);
    }

    public GsaOffset Duplicate()
    {
      if (this == null) { return null; }
      return (GsaOffset)this.MemberwiseClone(); // all members are structs
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
      IQuantity quantity = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      return "Offset" + " {X1: "
          + Math.Round(X1.As(DefaultUnits.LengthUnitGeometry), 4) + ", X2: "
          + Math.Round(X2.As(DefaultUnits.LengthUnitGeometry), 4) + ", Y: "
          + Math.Round(Y.As(DefaultUnits.LengthUnitGeometry), 4) + ", Z: "
          + Math.Round(Z.As(DefaultUnits.LengthUnitGeometry), 4) + " [" + unitAbbreviation + "]}";
    }
    #endregion
  }
}
