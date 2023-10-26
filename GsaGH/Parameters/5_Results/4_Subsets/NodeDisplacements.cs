using OasysUnits;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeDisplacements : INodeResultSubset<IDisplacement, NodeExtremaVector6> {
    public NodeExtremaVector6 Max { get; private set; }
    public NodeExtremaVector6 Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IDisplacement>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement>>();

    public NodeDisplacements(ConcurrentDictionary<int, Collection<IDisplacement>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IDisplacement, Length, Angle>();
    }

    public IDisplacement GetExtrema(NodeExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}