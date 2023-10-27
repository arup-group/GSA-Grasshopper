using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class BeamDisplacements : IBeamResultSubset<IBeamDisplacement, ResultVector6<NodeExtremaKey>> {
    public ResultVector6<NodeExtremaKey> Max { get; private set; }
    public ResultVector6<NodeExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IBeamDisplacement>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IBeamDisplacement>>();

    public BeamDisplacements(ConcurrentDictionary<int, Collection<IBeamDisplacement>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema();
    }

    public IBeamDisplacement GetExtrema(NodeExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
