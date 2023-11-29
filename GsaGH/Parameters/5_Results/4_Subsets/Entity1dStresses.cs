using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity1dStresses : IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> {
    public ResultStress1d<Entity1dExtremaKey> Max { get; private set; }
    public ResultStress1d<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dStress>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dStress>>();

    public Entity1dStresses(IDictionary<int, IList<IEntity1dStress>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultStress1dExtremaKeys();
    }

    public IStress1d GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
