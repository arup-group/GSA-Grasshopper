using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class SteelUtilisations : IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> {
    public Entity0dExtremaKey Max { get; private set; }
    public Entity0dExtremaKey Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<ISteelUtilisation>> Subset { get; }
      = new ConcurrentDictionary<int, IList<ISteelUtilisation>>();

    public SteelUtilisations(IDictionary<int, IList<ISteelUtilisation>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetEntity0dExtremaKeys();
    }

    public ISteelUtilisation GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}