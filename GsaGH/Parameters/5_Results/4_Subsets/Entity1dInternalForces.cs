using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Entity1dInternalForces : IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> {
    public ResultVector6<Entity1dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IEntity1dInternalForce>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IEntity1dInternalForce>>();

    public Entity1dInternalForces(IDictionary<int, IList<IEntity1dInternalForce>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6Entity1dExtremaKeys<IEntity1dInternalForce, IInternalForce>();
    }

    public IInternalForce GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
