using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity1dDisplacements : IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>> {
    public ResultVector6<Entity1dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity1dDisplacement>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity1dDisplacement>>();

    public Entity1dDisplacements(ConcurrentDictionary<int, Collection<IEntity1dDisplacement>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IEntity1dDisplacement, IDisplacement>();
    }

    public IDisplacement GetExtrema(Entity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
