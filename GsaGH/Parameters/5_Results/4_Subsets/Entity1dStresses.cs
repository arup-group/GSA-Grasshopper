using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Entity1dStresses : IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> {
    public ResultStress1d<Entity1dExtremaKey> Max { get; private set; }
    public ResultStress1d<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dQuantity<IStress1d>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dQuantity<IStress1d>>>();

    public Entity1dStresses(IDictionary<int, IList<IEntity1dQuantity<IStress1d>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultStress1dExtremaKeys();
    }

    public IStress1d GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
