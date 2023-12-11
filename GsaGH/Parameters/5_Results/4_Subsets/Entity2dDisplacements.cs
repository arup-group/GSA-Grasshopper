using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Entity2dDisplacements : IMeshResultSubset<IMeshQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> {
    public ResultVector6<Entity2dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IMeshQuantity<IDisplacement>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IDisplacement>>>();

    public Entity2dDisplacements(IDictionary<int, IList<IMeshQuantity<IDisplacement>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6Entity2dExtremaKeys();
    }

    public IDisplacement GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
