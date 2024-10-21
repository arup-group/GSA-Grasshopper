using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class MeshCentreOnlyStress : MeshCentreOnlyResult<Tensor3, IStress> {
    internal MeshCentreOnlyStress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
