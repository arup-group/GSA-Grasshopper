using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity1dDerivedStress : Entity1dResult<DerivedStressResult1d, IStress1dDerived> {
    internal Entity1dDerivedStress(
      ReadOnlyCollection<DerivedStressResult1d> result, ReadOnlyCollection<double> positions)
      : base(result, positions, (x) => new Stress1dDerived(x)) { }

    private Entity1dDerivedStress(IDictionary<double, IStress1dDerived> results) : base(results) { }

    public override IEntity1dQuantity<IStress1dDerived> TakePositions(ICollection<double> positions) {
      return new Entity1dDerivedStress(TakePositions(this, positions));
    }
  }
}
