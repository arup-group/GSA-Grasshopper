using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity3dDisplacements : IEntity2dResultSubset<IEntity2dQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> {
    public ResultVector3InAxis<Entity2dExtremaKey> Max { get; private set; }
    public ResultVector3InAxis<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IEntity2dQuantity<ITranslation>>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IEntity2dQuantity<ITranslation>>>();

    public Entity3dDisplacements(ConcurrentDictionary<int, Collection<IEntity2dQuantity<ITranslation>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector3Entity2dExtremaKeys();
    }

    public ITranslation GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
