using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dWedge6Stress : Entity3dWedge6Result<Tensor3, IStress> {
    internal Entity3dWedge6Stress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
