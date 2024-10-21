using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuadMoment : Entity2dQuadResult<Tensor2, IMoment2d> {
    internal Entity2dQuadMoment(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Moment2d(x)) {
    }
  }
}
