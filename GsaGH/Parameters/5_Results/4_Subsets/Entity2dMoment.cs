using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity2dMoment : IEntity2dResultSubset<IEntity2dQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> {
    public ResultTensor2AroundAxis<Entity2dExtremaKey> Max { get; private set; }
    public ResultTensor2AroundAxis<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<IMoment2d>>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<IMoment2d>>>();

    public Entity2dMoment(ConcurrentDictionary<int, Collection<IEntity2dQuantity<IMoment2d>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultTensor2AroundAxisEntity2dExtremaKeys();
    }

    public IMoment2d GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
