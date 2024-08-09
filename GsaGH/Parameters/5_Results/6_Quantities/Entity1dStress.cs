using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity1dStress : Entity1dResult<StressResult1d, IStress1d> {
    internal Entity1dStress(
      ReadOnlyCollection<StressResult1d> result, ReadOnlyCollection<double> positions)
      : base(result, positions, (x) => new Stress1d(x)) { }

    private Entity1dStress(IDictionary<double, IStress1d> results) : base(results) { }

    public override IEntity1dQuantity<IStress1d> TakePositions(ICollection<double> positions) {
      return new Entity1dStress(TakePositions(this, positions));
    }
  }
}
