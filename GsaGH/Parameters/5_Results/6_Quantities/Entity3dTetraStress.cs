using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dTetraStress : Entity3dTetraResult<Tensor3, IStress> {
    internal Entity3dTetraStress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
