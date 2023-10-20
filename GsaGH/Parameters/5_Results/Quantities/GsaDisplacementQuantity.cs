using GsaAPI;
using OasysUnits;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  public class GsaDisplacementQuantity : IDisplacement {
    public Length X { get; private set; }
    public Length Xyz { get; internal set; }
    public Length Y { get; private set; }
    public Length Z { get; private set; }
    public Angle Xx { get; private set; }
    public Angle Xxyyzz { get; internal set; }
    public Angle Yy { get; private set; }
    public Angle Zz { get; private set; }

    internal GsaDisplacementQuantity(Double6 result) {
      X = new Length(result.X, LengthUnit.Meter);
      Y = new Length(result.Y, LengthUnit.Meter);
      Z = new Length(result.Z, LengthUnit.Meter);
      Xyz = ResultUtility.PythagoreanQuadruple(X, Y, Z, LengthUnit.Meter);
      Xx = CreateAngle(result.XX);
      Yy = CreateAngle(result.YY);
      Zz = CreateAngle(result.ZZ);
      Xxyyzz = ResultUtility.PythagoreanQuadruple(Xx, Yy, Zz, AngleUnit.Radian);
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
