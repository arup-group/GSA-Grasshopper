using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dPyramidStress : Entity3dPyramidResult<Tensor3, IStress> {
    internal Entity3dPyramidStress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
