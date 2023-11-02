using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeForceSubset : INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> {

    public NodeForceSubset(IDictionary<int, Collection<IInternalForce>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema();
    }

    public ResultVector6<NodeExtremaKey> Max { get; private set; }
    public ResultVector6<NodeExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, Collection<IInternalForce>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce>>();

    public IInternalForce GetExtrema(NodeExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
