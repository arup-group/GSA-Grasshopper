using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTri6Force : Entity2dTri6Result<Tensor2, IForce2d> {
    internal Entity2dTri6Force(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Force2d(x)) {
    }
  }
}
