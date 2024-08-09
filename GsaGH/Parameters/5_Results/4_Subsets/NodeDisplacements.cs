using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeDisplacements : IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> {
    public ResultVector6<Entity0dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity0dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IDisplacement>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IDisplacement>>();

    public NodeDisplacements(IDictionary<int, IList<IDisplacement>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6NodeExtremaKeys();
    }

    public IDisplacement GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
