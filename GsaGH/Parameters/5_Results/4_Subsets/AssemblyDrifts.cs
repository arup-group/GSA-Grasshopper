using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class AssemblyDrifts : IAssemblyResultSubset<IDrift<Length>, ResultVector6<Entity1dExtremaKey>> {
    public ResultVector6<Entity1dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IAssemblyQuantity<IDrift<Length>>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IAssemblyQuantity<IDrift<Length>>>>();

    public AssemblyDrifts(IDictionary<int, IList<IAssemblyQuantity<IDrift<Length>>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6AssemblyExtremaKeys<IAssemblyQuantity<IDrift<Length>>, IDrift<Length>>();
    }

    public IDrift<Length> GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
