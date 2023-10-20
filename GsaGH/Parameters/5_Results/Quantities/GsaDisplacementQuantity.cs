using GsaAPI;
using OasysUnits;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  public class GsaDisplacementQuantity : IDisplacement {
    public Length X { get; internal set; }
    public Length Xyz { get; internal set; }
    public Length Y { get; internal set; }
    public Length Z { get; internal set; }
    public Angle Xx { get; internal set; }
    public Angle Xxyyzz { get; internal set; }
    public Angle Yy { get; internal set; }
    public Angle Zz { get; internal set; }

    internal GsaDisplacementQuantity(
      Length x, Length y, Length z, Length xyz, Angle xx, Angle yy, Angle zz, Angle xxyyzz) {
      X = x;
      Y = y; 
      Z = z;
      Xyz = xyz;
      Xx = xx;
      Yy = yy;
      Zz = zz;
      Xxyyzz = xxyyzz;
    }

    internal GsaDisplacementQuantity(Double6 result) {
      X = new Length(result.X, LengthUnit.Meter);
      Y = new Length(result.Y, LengthUnit.Meter);
      Z = new Length(result.Z, LengthUnit.Meter);
      Xyz = ResultUtility.PythagoreanQuadruple(X, Y, Z);
      Xx = CreateAngle(result.XX);
      Yy = CreateAngle(result.YY);
      Zz = CreateAngle(result.ZZ);
      Xxyyzz = ResultUtility.PythagoreanQuadruple(Xx, Yy, Zz);
    }

    private Angle CreateAngle(double val) {
      // TO-DO: GSA-5351 remove NaN and Infinity values from GsaAPI results
      if (!double.IsNaN(val)) {
        return !double.IsInfinity(val)
          ? new Angle(val, AngleUnit.Radian)
          : (double.IsPositiveInfinity(val)
            ? new Angle(360, AngleUnit.Degree)
            : new Angle(-360, AngleUnit.Degree));
      } else {
        return Angle.Zero;
      }
    }
  }
}
