﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class NodeForceSubset : IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> {
    public ResultVector6<Entity0dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity0dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IReactionForce>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IReactionForce>>();

    public NodeForceSubset(IDictionary<int, IList<IReactionForce>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6NodeExtremaKeys();
    }

    public IReactionForce GetExtrema(IExtremaKey key) {
      return Subset[key.Id][key.Permutation];
    }
  }
}
