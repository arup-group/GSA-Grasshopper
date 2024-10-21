using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class MeshCentreOnlyMoment : MeshCentreOnlyResult<Tensor2, IMoment2d> {
    internal MeshCentreOnlyMoment(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Moment2d(x)) {
    }
  }
}
