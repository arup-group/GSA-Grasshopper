using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Displacements1d : IResultSubset1d<IDisplacement1d, IDisplacement,
    ResultVector6<ExtremaKey1d>> {

    public Displacements1d(ConcurrentDictionary<int, Collection<IDisplacement1d>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IDisplacement1d, IDisplacement>();
    }

    public ResultVector6<ExtremaKey1d> Max { get; private set; }
    public ResultVector6<ExtremaKey1d> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IDisplacement1d>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement1d>>();

    public IDisplacement GetExtrema(ExtremaKey1d key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
