using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class MeshCentreOnlyForce : MeshCentreOnlyResult<Tensor2, IForce2d> {
    internal MeshCentreOnlyForce(ReadOnlyCollection<Tensor2> result)
      : base(result, (x) => new Force2d(x)) {
    }
  }
}
