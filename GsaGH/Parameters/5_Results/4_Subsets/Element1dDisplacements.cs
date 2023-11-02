using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class Element1dDisplacements : IElement1dResultSubset<IElement1dDisplacement, IDisplacement, ResultVector6<Element1dExtremaKey>> {
    public ResultVector6<Element1dExtremaKey> Max { get; private set; }
    public ResultVector6<Element1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public ConcurrentDictionary<int, Collection<IElement1dDisplacement>> Subset { get; }
      = new ConcurrentDictionary<int, Collection<IElement1dDisplacement>>();

    public Element1dDisplacements(ConcurrentDictionary<int, Collection<IElement1dDisplacement>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.BeamExtrema<IElement1dDisplacement, IDisplacement>();
    }

    public IDisplacement GetExtrema(Element1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
