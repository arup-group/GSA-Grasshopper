using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndices : IEntity1dResultSubset<DriftIndex, DriftResultVector<Entity1dExtremaKey>> {
    public DriftResultVector<Entity1dExtremaKey> Max { get; private set; }
    public DriftResultVector<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dQuantity<DriftIndex>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<DriftIndex>>>();

    public AssemblyDriftIndices(IDictionary<int, IList<IEntity1dQuantity<DriftIndex>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetDriftResultExtremaKeys<IEntity1dQuantity<DriftIndex>, DriftIndex>();
    }

    public DriftIndex GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
