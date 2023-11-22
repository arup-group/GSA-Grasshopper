using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeFootfalls : INodeResultSubset<IFootfall, ResultFootfall<NodeExtremaKey>> {
    public ResultFootfall<NodeExtremaKey> Max { get; private set; }
    public ResultFootfall<NodeExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, Collection<IFootfall>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IFootfall>>();

    public NodeFootfalls(IDictionary<int, Collection<IFootfall>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultFootfallExtremaKeys();
    }

    public IFootfall GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}