using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Element1dStresses : IElement1dResultSubset<IElement1dStress, IStress1d, ResultStress1d<Element1dExtremaKey>> {
    public ResultStress1d<Element1dExtremaKey> Max { get; private set; }
    public ResultStress1d<Element1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IElement1dStress>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IElement1dStress>>();

    public Element1dStresses(ConcurrentDictionary<int, Collection<IElement1dStress>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema();
    }

    public IStress1d GetExtrema(Element1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
