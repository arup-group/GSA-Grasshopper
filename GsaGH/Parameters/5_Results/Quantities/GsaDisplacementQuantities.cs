using GsaAPI;
using OasysUnits;
using Rhino.Commands;
using System;
using System.IO;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters._5_Results.Quantities {
  public class GsaDisplacementQuantities {
    internal Length X { get; set; }
    internal Length Xyz { get; set; }
    internal Length Y { get; set; }
    internal Length Z { get; set; }
    internal Angle Xx { get; set; }
    internal Angle Xxyyzz { get; set; }
    internal Angle Yy { get; set; }
    internal Angle Zz { get; set; }

    internal GsaDisplacementQuantities(Double6 result, LengthUnit LUnit, AngleUnit AUnit) {
      SetLengthUnit(result, LUnit);
      SetAngleUnit(result, AUnit);
    }

    private void SetLengthUnit(Double6 result, LengthUnit unit) {
      X = new Length(new Length(result.X, LengthUnit.Meter).As(unit), unit);
      Y = new Length(new Length(result.Y, LengthUnit.Meter).As(unit), unit);
      Z = new Length(new Length(result.Z, LengthUnit.Meter).As(unit), unit);
      double pyth
        = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
      Xyz = new Length(new Length(pyth, LengthUnit.Meter).As(unit), unit);
    }

    private void SetAngleUnit(Double6 result, AngleUnit unit) {
      IQuantity x;
      if (!double.IsNaN(result.XX)) // TO-DO: GSA-5351 remove NaN and Infinity values from GsaAPI results
      {
        x = !double.IsInfinity(result.XX) ?
          new Angle(new Angle(result.XX, AngleUnit.Radian).As(unit), unit) :
          (IQuantity)(double.IsPositiveInfinity(result.XX) ? new Angle(360, AngleUnit.Degree) :
            new Angle(-360, AngleUnit.Degree));
      } else {
        x = new Angle(0, unit);
      }

      IQuantity y;
      if (!double.IsNaN(result.YY)) {
        y = !double.IsInfinity(result.YY) ?
          new Angle(new Angle(result.YY, AngleUnit.Radian).As(unit), unit) :
          (IQuantity)(double.IsPositiveInfinity(result.YY) ? new Angle(360, AngleUnit.Degree) :
            new Angle(-360, AngleUnit.Degree));
      } else {
        y = new Angle(0, unit);
      }

      IQuantity z;
      if (!double.IsNaN(result.ZZ)) {
        z = !double.IsInfinity(result.ZZ) ?
          new Angle(new Angle(result.ZZ, AngleUnit.Radian).As(unit), unit) :
          (IQuantity)(double.IsPositiveInfinity(result.ZZ) ? new Angle(360, AngleUnit.Degree) :
            new Angle(-360, AngleUnit.Degree));
      } else {
        z = new Angle(0, unit);
      }

      double pyth = Math.Sqrt(Math.Pow(Xx.Value, 2) + Math.Pow(y.Value, 2) + Math.Pow(z.Value, 2));
      IQuantity xyz = double.IsInfinity(pyth) ? new Angle(360, AngleUnit.Degree) :
        new Angle(new Angle(pyth, AngleUnit.Radian).As(unit), unit);
    }
  }
}
