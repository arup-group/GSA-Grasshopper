using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity1dDisplacement : Entity1dResult<Double6, IDisplacement> {
    internal Entity1dDisplacement(
      ReadOnlyCollection<Double6> result, ReadOnlyCollection<double> positions)
      : base(result, positions, (x) => new Displacement(x)) { }

    private Entity1dDisplacement(IDictionary<double, IDisplacement> results) : base(results) { }

    public override IEntity1dQuantity<IDisplacement> TakePositions(ICollection<double> positions) {
      return new Entity1dDisplacement(TakePositions(this, positions));
    }
  }
}
