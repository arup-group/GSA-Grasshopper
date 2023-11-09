using GsaAPI;
using OasysUnits;
using MassUnit = OasysUnits.Units.MassUnit;

namespace GsaGH.Parameters.Results {
  public class EffectiveMass : IEffectiveMass {
    public Mass X { get; internal set; }
    public Mass Y { get; internal set; }
    public Mass Z { get; internal set; }
    public Mass Xyz { get; internal set; }

    internal EffectiveMass(Vector3 result) {
      X = new Mass(result.X, MassUnit.Kilogram);
      Y = new Mass(result.Y, MassUnit.Kilogram);
      Z = new Mass(result.Z, MassUnit.Kilogram);
      Xyz = QuantityUtility.PythagoreanQuadruple(X, Y, Z);
    }
  }
}
