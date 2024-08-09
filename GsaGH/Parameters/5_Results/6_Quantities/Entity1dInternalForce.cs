using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity1dInternalForce : Entity1dResult<Double6, IInternalForce> {
    internal Entity1dInternalForce(
      ReadOnlyCollection<Double6> result, ReadOnlyCollection<double> positions)
      : base(result, positions, (x) => new InternalForce(x)) { }

    private Entity1dInternalForce(IDictionary<double, IInternalForce> results) : base(results) { }

    public override IEntity1dQuantity<IInternalForce> TakePositions(ICollection<double> positions) {
      return new Entity1dInternalForce(TakePositions(this, positions));
    }
  }
}
