using GsaAPI;

using OasysUnits;

using MassUnit = OasysUnits.Units.MassUnit;

namespace GsaGH.Parameters.Results {
  public class EffectiveMass : IEffectiveMass {

    internal EffectiveMass(Vector3 result) {
      X = new Mass(result.X, MassUnit.Kilogram);
      Y = new Mass(result.Y, MassUnit.Kilogram);
      Z = new Mass(result.Z, MassUnit.Kilogram);
    }

    public Mass X { get; internal set; }
    public Mass Y { get; internal set; }
    public Mass Z { get; internal set; }
  }
}
