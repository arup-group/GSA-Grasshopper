using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeFootfalls : IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> {
    public ResultFootfall<Entity0dExtremaKey> Max { get; private set; }
    public ResultFootfall<Entity0dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IFootfall>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IFootfall>>();

    public NodeFootfalls(IDictionary<int, IList<IFootfall>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultFootfallExtremaKeys();
    }

    public IFootfall GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
