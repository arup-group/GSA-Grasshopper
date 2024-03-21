using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndices : IEntity1dResultSubset<IDrift<double>, DriftResultVector<Entity1dExtremaKey>> {
    public DriftResultVector<Entity1dExtremaKey> Max { get; private set; }
    public DriftResultVector<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dQuantity<IDrift<double>>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IDrift<double>>>>();

    public AssemblyDriftIndices(IDictionary<int, IList<IEntity1dQuantity<IDrift<double>>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetDriftResultExtremaKeys<IEntity1dQuantity<IDrift<double>>, IDrift<double>>();
    }

    public IDrift<double> GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
