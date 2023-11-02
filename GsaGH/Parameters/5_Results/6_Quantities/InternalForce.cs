using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using ForceUnit = OasysUnits.Units.ForceUnit;

namespace GsaGH.Parameters.Results {
  public class InternalForce : IInternalForce {

    internal InternalForce(Double6 result) {
      X = new Force(result.X, ForceUnit.Newton);
      Y = new Force(result.Y, ForceUnit.Newton);
      Z = new Force(result.Z, ForceUnit.Newton);
      Xyz = QuantityUtility.PythagoreanQuadruple(X, Y, Z);
      Xx = new Moment(result.XX, MomentUnit.NewtonMeter);
      Yy = new Moment(result.YY, MomentUnit.NewtonMeter);
      Zz = new Moment(result.ZZ, MomentUnit.NewtonMeter);
      Xxyyzz = QuantityUtility.PythagoreanQuadruple(Xx, Yy, Zz);
    }

    public Force X { get; internal set; }
    public Force Y { get; internal set; }
    public Force Z { get; internal set; }
    public Force Xyz { get; internal set; }
    public Moment Xx { get; internal set; }
    public Moment Yy { get; internal set; }
    public Moment Zz { get; internal set; }
    public Moment Xxyyzz { get; internal set; }
  }
}
