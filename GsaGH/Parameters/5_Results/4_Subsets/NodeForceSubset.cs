using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeForceSubset : INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> {
    public ResultVector6<NodeExtremaKey> Max { get; private set; }
    public ResultVector6<NodeExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IInternalForce>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IInternalForce>>();

    public NodeForceSubset(IDictionary<int, IList<IInternalForce>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6NodeExtremaKeys();
    }

    public IInternalForce GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
