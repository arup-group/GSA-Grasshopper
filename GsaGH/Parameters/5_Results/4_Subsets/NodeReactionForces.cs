using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class NodeReactionForces : INodeResultSubset<IInternalForce, NodeExtremaVector6> {

    public NodeReactionForces(ConcurrentDictionary<int, Collection<IInternalForce>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IInternalForce, Force, Moment>();
    }

    public NodeExtremaVector6 Max { get; private set; }
    public NodeExtremaVector6 Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IInternalForce>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce>>();

    public IInternalForce GetExtrema(NodeExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
