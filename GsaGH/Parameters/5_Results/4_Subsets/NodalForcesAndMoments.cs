using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodalForcesAndMoments {
    public IList<int> Ids { get; private set; }

    public IDictionary<int, Collection<IDictionary<int, IReactionForce>>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IDictionary<int, IReactionForce>>>();

    public NodalForcesAndMoments(IDictionary<int, Collection<IDictionary<int, IReactionForce>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
    }
  }
}
