using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTriMoment : Entity2dTriResult<Tensor2, IMoment2d> {
    internal Entity2dTriMoment(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Moment2d(x)) {
    }
  }
}
