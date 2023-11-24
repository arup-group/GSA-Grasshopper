using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity2dForce : IEntity2dResultSubset<IEntity2dQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> {
    public ResultTensor2InAxis<Entity2dExtremaKey> Max { get; private set; }
    public ResultTensor2InAxis<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>>();

    public Entity2dForce(ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultTensor2InAxisEntity2dExtremaKeys();
    }

    public IForce2d GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
