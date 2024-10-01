using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Entity1dAverageStrainEnergyDensity : IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> {
    public Entity0dExtremaKey Max { get; private set; }
    public Entity0dExtremaKey Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEnergyDensity>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEnergyDensity>>();

    public Entity1dAverageStrainEnergyDensity(IDictionary<int, IList<IEnergyDensity>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetEntity0dExtremaKeys();
    }

    public IEnergyDensity GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
