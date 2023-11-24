using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity2dForce : IEntity2dResultSubset<IEntity2dQuantity<IForce2d>, IForce2d, ResultVector6<Entity2dExtremaKey>> {
    public ResultVector6<Entity2dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>>();

    public Entity2dForce(ConcurrentDictionary<int, Collection<IEntity2dQuantity<IForce2d>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6Entity2dExtremaKeys();
    }

    public IForce2d GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
