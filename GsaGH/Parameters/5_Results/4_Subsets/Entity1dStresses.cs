using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity1dStresses : IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> {
    public ResultStress1d<Entity1dExtremaKey> Max { get; private set; }
    public ResultStress1d<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity1dStress>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dStress>>();

    public Entity1dStresses(ConcurrentDictionary<int, Collection<IEntity1dStress>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema();
    }

    public IStress1d GetExtrema(Entity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
