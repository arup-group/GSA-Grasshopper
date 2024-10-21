using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuadForce : Entity2dQuadResult<Tensor2, IForce2d> {
    internal Entity2dQuadForce(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Force2d(x)) {
    }
  }
}
