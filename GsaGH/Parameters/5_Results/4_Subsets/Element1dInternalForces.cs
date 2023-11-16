using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Element1dInternalForces : IResultSubset1d<IInternalForce1d, IInternalForce,
    ResultVector6<ExtremaKey1d>> {

    public Element1dInternalForces(
      ConcurrentDictionary<int, Collection<IInternalForce1d>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IInternalForce1d, IInternalForce>();
    }

    public ResultVector6<ExtremaKey1d> Max { get; private set; }
    public ResultVector6<ExtremaKey1d> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IInternalForce1d>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IInternalForce1d>>();

    public IInternalForce GetExtrema(ExtremaKey1d key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
