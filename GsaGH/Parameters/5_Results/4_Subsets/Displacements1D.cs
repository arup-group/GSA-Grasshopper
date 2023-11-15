using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class Displacements1D : IElement1dResultSubset<IDisplacement1D, IDisplacement,
    ResultVector6<ExtremaKey1D>> {

    public Displacements1D(ConcurrentDictionary<int, Collection<IDisplacement1D>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema<IDisplacement1D, IDisplacement>();
    }

    public ResultVector6<ExtremaKey1D> Max { get; private set; }
    public ResultVector6<ExtremaKey1D> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IDisplacement1D>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IDisplacement1D>>();

    public IDisplacement GetExtrema(ExtremaKey1D key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
