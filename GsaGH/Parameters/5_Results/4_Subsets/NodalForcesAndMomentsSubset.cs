using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodalForcesAndMomentsSubset {
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IDictionary<int, IList<IInternalForce>>> Subset { get; }
      = new ConcurrentDictionary<int, IDictionary<int, IList<IInternalForce>>>();

    public NodalForcesAndMomentsSubset(IDictionary<int, IDictionary<int, IList<IInternalForce>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
    }
  }
}
