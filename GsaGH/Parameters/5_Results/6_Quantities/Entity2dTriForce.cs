using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTriForce : Entity2dTriResult<Tensor2, IForce2d> {
    internal Entity2dTriForce(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Force2d(x)) {
    }
  }
}
