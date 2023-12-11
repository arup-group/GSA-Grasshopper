using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class MeshStresses : IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> {
    public ResultTensor3<Entity2dExtremaKey> Max { get; private set; }
    public ResultTensor3<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IMeshQuantity<IStress>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IStress>>>();

    public MeshStresses(IDictionary<int, IList<IMeshQuantity<IStress>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultTensor3Entity2dExtremaKeys();
    }

    public IStress GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
