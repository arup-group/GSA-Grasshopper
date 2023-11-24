using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity2dStresses : IEntity2dResultSubset<IEntity2dQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> {
    public ResultTensor3<Entity2dExtremaKey> Max { get; private set; }
    public ResultTensor3<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>>();

    public Entity2dStresses(ConcurrentDictionary<int, Collection<IEntity2dQuantity<IStress>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultTensor3Entity2dExtremaKeys();
    }

    public IStress GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
