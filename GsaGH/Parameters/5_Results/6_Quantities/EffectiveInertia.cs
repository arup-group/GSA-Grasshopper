using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class EffectiveInertia : IEffectiveInertia {
    public AreaMomentOfInertia X { get; internal set; }
    public AreaMomentOfInertia Y { get; internal set; }
    public AreaMomentOfInertia Z { get; internal set; }

    internal EffectiveInertia(Vector3 result) {
      X = new AreaMomentOfInertia(result.X, AreaMomentOfInertiaUnit.MeterToTheFourth);
      Y = new AreaMomentOfInertia(result.Y, AreaMomentOfInertiaUnit.MeterToTheFourth);
      Z = new AreaMomentOfInertia(result.Z, AreaMomentOfInertiaUnit.MeterToTheFourth);
    }
  }
}
