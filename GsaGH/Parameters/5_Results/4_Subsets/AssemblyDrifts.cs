using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class AssemblyDrifts : IEntity1dResultSubset<IDrift<Length>, DriftResultVector<Entity1dExtremaKey>> {
    public DriftResultVector<Entity1dExtremaKey> Max { get; private set; }
    public DriftResultVector<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dQuantity<IDrift<Length>>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IDrift<Length>>>>();

    public AssemblyDrifts(IDictionary<int, IList<IEntity1dQuantity<IDrift<Length>>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetDriftResultExtremaKeys<IEntity1dQuantity<IDrift<Length>>, IDrift<Length>>();
    }

    public IDrift<Length> GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
