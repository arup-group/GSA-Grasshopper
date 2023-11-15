using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Element1dInternalForces : IResultSubset1D<IInternalForce1D, IInternalForce,
    ResultVector6<ExtremaKey1D>> {

    public Element1dInternalForces(
      ConcurrentDictionary<int, Collection<IInternalForce1D>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IInternalForce1D, IInternalForce>();
    }

    public ResultVector6<ExtremaKey1D> Max { get; private set; }
    public ResultVector6<ExtremaKey1D> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IInternalForce1D>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce1D>>();

    public IInternalForce GetExtrema(ExtremaKey1D key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
