using GsaAPI;

using OasysUnits;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  public class Displacement : IDisplacement {
    public Length X { get; internal set; }
    public Length Y { get; internal set; }
    public Length Z { get; internal set; }
    public Length Xyz { get; internal set; }
    public Angle Xx { get; internal set; }
    public Angle Yy { get; internal set; }
    public Angle Zz { get; internal set; }
    public Angle Xxyyzz { get; internal set; }

    internal Displacement(Double6 result) {
      X = new Length(result.X, LengthUnit.Meter);
      Y = new Length(result.Y, LengthUnit.Meter);
      Z = new Length(result.Z, LengthUnit.Meter);
      Xyz = QuantityUtility.PythagoreanQuadruple(X, Y, Z);
      Xx = CreateAngle(result.XX);
      Yy = CreateAngle(result.YY);
      Zz = CreateAngle(result.ZZ);
      Xxyyzz = QuantityUtility.PythagoreanQuadruple(Xx, Yy, Zz);
    }

    internal Displacement(AssemblyResult result) {
      X = new Length(result.Values.X, LengthUnit.Meter);
      Y = new Length(result.Values.Y, LengthUnit.Meter);
      Z = new Length(result.Values.Z, LengthUnit.Meter);
      Xyz = QuantityUtility.PythagoreanQuadruple(X, Y, Z);
      Xx = CreateAngle(result.Values.XX);
      Yy = CreateAngle(result.Values.YY);
      Zz = CreateAngle(result.Values.ZZ);
      Xxyyzz = QuantityUtility.PythagoreanQuadruple(Xx, Yy, Zz);
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
