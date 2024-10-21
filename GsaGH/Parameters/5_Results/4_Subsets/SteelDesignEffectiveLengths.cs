using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class SteelDesignEffectiveLengths {
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<ISteelDesignEffectiveLength>> Subset { get; }
      = new ConcurrentDictionary<int, IList<ISteelDesignEffectiveLength>>();

    public SteelDesignEffectiveLengths(IDictionary<int, IList<ISteelDesignEffectiveLength>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
    }
  }
}
