using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity2dForce : IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> {
    public ResultTensor2InAxis<Entity2dExtremaKey> Max { get; private set; }
    public ResultTensor2InAxis<Entity2dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IMeshQuantity<IForce2d>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IMeshQuantity<IForce2d>>>();

    public Entity2dForce(IDictionary<int, IList<IMeshQuantity<IForce2d>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultTensor2InAxisEntity2dExtremaKeys();
    }

    public IForce2d GetExtrema(IEntity2dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results()[key.VertexId];
    }
  }
}
