using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class MeshCentreOnlyShear : MeshCentreOnlyResult<Vector2, IShear2d> {
    internal MeshCentreOnlyShear(ReadOnlyCollection<Vector2> result)
      : base(result, (x) => new Shear2d(x)) {
    }
  }
}
