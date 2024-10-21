using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dPyramid5Stress : Entity3dPyramid5Result<Tensor3, IStress> {
    internal Entity3dPyramid5Stress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
