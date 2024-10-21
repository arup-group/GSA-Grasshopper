using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuad8Force : Entity2dQuad8Result<Tensor2, IForce2d> {
    internal Entity2dQuad8Force(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Force2d(x)) {
    }
  }
}
