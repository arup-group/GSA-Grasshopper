using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity1dStrainEnergyDensities 
    : IEntity1dResultSubset<IEntity1dStrainEnergyDensity, IEnergyDensity, Entity1dExtremaKey> {
    public Entity1dExtremaKey Max { get; private set; }
    public Entity1dExtremaKey Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity1dStrainEnergyDensity>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dStrainEnergyDensity>>();

    public Entity1dStrainEnergyDensities(
      ConcurrentDictionary<int, Collection<IEntity1dStrainEnergyDensity>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema();
    }

    public IEnergyDensity GetExtrema(Entity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
