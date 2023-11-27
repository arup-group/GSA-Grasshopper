using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity2dShearForce : IEntity2dResultSubset<IEntity2dQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> {
    public ResultVector2<Entity2dExtremaKey> Max { get; private set; }
    public ResultVector2<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IShear2d>>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IShear2d>>>();

    public Entity2dShearForce(ConcurrentDictionary<int, Collection<IEntity2dQuantity<IShear2d>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector2Entity2dExtremaKeys();
    }

    public IShear2d GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
