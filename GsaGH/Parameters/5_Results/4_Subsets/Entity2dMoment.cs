﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Entity2dMoment : IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> {
    public ResultTensor2AroundAxis<Entity2dExtremaKey> Max { get; private set; }
    public ResultTensor2AroundAxis<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IMeshQuantity<IMoment2d>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IMoment2d>>>();

    public Entity2dMoment(IDictionary<int, IList<IMeshQuantity<IMoment2d>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultTensor2AroundAxisEntity2dExtremaKeys();
    }

    public IMoment2d GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
