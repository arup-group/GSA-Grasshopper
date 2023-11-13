using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Element1dInternalForces : IElement1dResultSubset<IElement1dInternalForce, IInternalForce, ResultVector6<Element1dExtremaKey>> {
    public ResultVector6<Element1dExtremaKey> Max { get; private set; }
    public ResultVector6<Element1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IElement1dInternalForce>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IElement1dInternalForce>>();

    public Element1dInternalForces(ConcurrentDictionary<int, Collection<IElement1dInternalForce>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IElement1dInternalForce, IInternalForce>();
    }

    public IInternalForce GetExtrema(Element1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
