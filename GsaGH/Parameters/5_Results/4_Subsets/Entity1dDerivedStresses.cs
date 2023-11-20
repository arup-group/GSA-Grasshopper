using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity1dDerivedStresses : IEntity1dResultSubset<IEntity1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> {
    public ResultDerivedStress1d<Entity1dExtremaKey> Max { get; private set; }
    public ResultDerivedStress1d<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity1dDerivedStress>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dDerivedStress>>();

    public Entity1dDerivedStresses(ConcurrentDictionary<int, Collection<IEntity1dDerivedStress>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema();
    }

    public IStress1dDerived GetExtrema(Entity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
