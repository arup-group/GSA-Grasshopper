using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class GsaNodeDisplacements : INodeResultSubset<IDisplacement> {
    public IResultExtrema Max { get; private set; }
    public IResultExtrema Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IDisplacement>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement>>();

    public GsaNodeDisplacements(ConcurrentDictionary<int, Collection<IDisplacement>> results) {
      Subset = results;
      Ids = results.Keys.ToList();
      (Max, Min) = results.Extrema();
    }
  }
}