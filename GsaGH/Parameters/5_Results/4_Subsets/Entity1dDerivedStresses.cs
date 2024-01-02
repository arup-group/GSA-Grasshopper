using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Entity1dDerivedStresses : IEntity1dResultSubset<IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> {
    public ResultDerivedStress1d<Entity1dExtremaKey> Max { get; private set; }
    public ResultDerivedStress1d<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dQuantity<IStress1dDerived>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IStress1dDerived>>>();

    public Entity1dDerivedStresses(IDictionary<int, IList<IEntity1dQuantity<IStress1dDerived>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultDerivedStress1dExtremaKeys();
    }

    public IStress1dDerived GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
