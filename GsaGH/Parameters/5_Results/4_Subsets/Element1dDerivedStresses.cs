using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Element1dDerivedStresses : IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> {
    public ResultDerivedStress1d<Element1dExtremaKey> Max { get; private set; }
    public ResultDerivedStress1d<Element1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IElement1dDerivedStress>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IElement1dDerivedStress>>();

    public Element1dDerivedStresses(ConcurrentDictionary<int, Collection<IElement1dDerivedStress>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.Extrema();
    }

    public IStress1dDerived GetExtrema(Element1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
