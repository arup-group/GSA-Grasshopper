﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Entity1dDisplacements : IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> {
    public ResultVector6<Entity1dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dQuantity<IDisplacement>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IDisplacement>>>();

    public Entity1dDisplacements(IDictionary<int, IList<IEntity1dQuantity<IDisplacement>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6Entity1dExtremaKeys<IEntity1dQuantity<IDisplacement>, IDisplacement>();
    }

    public IDisplacement GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
