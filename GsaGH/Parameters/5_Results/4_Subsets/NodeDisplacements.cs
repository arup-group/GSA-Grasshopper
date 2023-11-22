using OasysUnits;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeDisplacements : INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> {
    public ResultVector6<NodeExtremaKey> Max { get; private set; }
    public ResultVector6<NodeExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, Collection<IDisplacement>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement>>();

    public NodeDisplacements(IDictionary<int, Collection<IDisplacement>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6NodeExtremaKeys();
    }

    public IDisplacement GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}