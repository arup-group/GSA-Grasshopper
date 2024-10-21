using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTri6Moment : Entity2dTri6Result<Tensor2, IMoment2d> {
    internal Entity2dTri6Moment(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Moment2d(x)) {
    }
  }
}
