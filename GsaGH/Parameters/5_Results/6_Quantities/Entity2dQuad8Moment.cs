using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuad8Moment : Entity2dQuad8Result<Tensor2, IMoment2d> {
    internal Entity2dQuad8Moment(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Moment2d(x)) {
    }
  }
}
