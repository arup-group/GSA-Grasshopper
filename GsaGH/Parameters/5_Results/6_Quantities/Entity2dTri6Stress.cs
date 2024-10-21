using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTri6Stress : Entity2dTri6Result<Tensor3, IStress> {
    internal Entity2dTri6Stress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
