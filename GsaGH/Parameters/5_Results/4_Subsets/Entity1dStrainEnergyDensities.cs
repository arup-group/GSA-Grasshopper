﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Entity1dStrainEnergyDensities : IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> {
    public Entity1dExtremaKey Max { get; private set; }
    public Entity1dExtremaKey Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dQuantity<IEnergyDensity>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IEnergyDensity>>>();

    public Entity1dStrainEnergyDensities(
      IDictionary<int, IList<IEntity1dQuantity<IEnergyDensity>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetEntity1dExtremaKeys<IEntity1dQuantity<IEnergyDensity>, IEnergyDensity>();
    }

    public IEnergyDensity GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
