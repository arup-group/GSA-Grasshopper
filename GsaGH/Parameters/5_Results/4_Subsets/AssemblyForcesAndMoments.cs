using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public class AssemblyForcesAndMoments : IAssemblyResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> {
    public ResultVector6<Entity1dExtremaKey> Max { get; private set; }
    public ResultVector6<Entity1dExtremaKey> Min { get; private set; }
    public IList<int> Ids { get; private set; }

    public IDictionary<int, IList<IAssemblyQuantity<IInternalForce>>> Subset { get; }
      = new ConcurrentDictionary<int, IList<IAssemblyQuantity<IInternalForce>>>();

    public AssemblyForcesAndMoments(IDictionary<int, IList<IAssemblyQuantity<IInternalForce>>> results) {
      Subset = results;
      Ids = results.Keys.OrderBy(x => x).ToList();
      (Max, Min) = results.GetResultVector6AssemblyExtremaKeys<IAssemblyQuantity<IInternalForce>, IInternalForce>();
    }

    public IInternalForce GetExtrema(IEntity1dExtremaKey key) {
      return Subset[key.Id][key.Permutation].Results[key.Position];
    }
  }
}
