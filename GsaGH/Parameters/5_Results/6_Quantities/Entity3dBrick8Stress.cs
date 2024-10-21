using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dBrick8Stress : Entity3dBrick8Result<Tensor3, IStress> {
    internal Entity3dBrick8Stress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
