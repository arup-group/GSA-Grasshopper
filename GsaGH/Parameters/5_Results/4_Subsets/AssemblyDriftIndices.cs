using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndices : IAssemblyResultSubset<IDrift<double>, DriftResultVector<Entity1dExtremaKey>> {
    public DriftResultVector<Entity1dExtremaKey> Max { get; private set; }
    public DriftResultVector<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IAssemblyQuantity<IDrift<double>>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IAssemblyQuantity<IDrift<double>>>>();

    public AssemblyDriftIndices(IDictionary<int, IList<IAssemblyQuantity<IDrift<double>>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetDriftResultExtremaKeys<IAssemblyQuantity<IDrift<double>>, IDrift<double>>();
    }

    public IDrift<double> GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
