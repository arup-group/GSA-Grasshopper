using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

using ForceUnit = OasysUnits.Units.ForceUnit;

namespace GsaGH.Parameters.Results {
  public class InternalForce : IInternalForce {
    public Force X { get; internal set; }
    public Force Y { get; internal set; }
    public Force Z { get; internal set; }
    public Force Xyz { get; internal set; } // here Yz!
    public Moment Xx { get; internal set; }
    public Moment Yy { get; internal set; }
    public Moment Zz { get; internal set; }
    public Moment Xxyyzz { get; internal set; } // here Yyzz!

    internal InternalForce(Double6 result) {
      X = new Force(result.X, ForceUnit.Newton);
      Y = new Force(result.Y, ForceUnit.Newton);
      Z = new Force(result.Z, ForceUnit.Newton);
      Xyz = QuantityUtility.PythagoreanTriple(Y, Z);
      Xx = new Moment(result.XX, MomentUnit.NewtonMeter);
      Yy = new Moment(result.YY, MomentUnit.NewtonMeter);
      Zz = new Moment(result.ZZ, MomentUnit.NewtonMeter);
      Xxyyzz = QuantityUtility.PythagoreanTriple(Yy, Zz);
    }

    internal InternalForce(AssemblyResult result) {
      X = new Force(result.Values.X, ForceUnit.Newton);
      Y = new Force(result.Values.Y, ForceUnit.Newton);
      Z = new Force(result.Values.Z, ForceUnit.Newton);
      Xyz = QuantityUtility.PythagoreanTriple(Y, Z);
      Xx = new Moment(result.Values.XX, MomentUnit.NewtonMeter);
      Yy = new Moment(result.Values.YY, MomentUnit.NewtonMeter);
      Zz = new Moment(result.Values.ZZ, MomentUnit.NewtonMeter);
      Xxyyzz = QuantityUtility.PythagoreanTriple(Yy, Zz);
    }
  }
}
