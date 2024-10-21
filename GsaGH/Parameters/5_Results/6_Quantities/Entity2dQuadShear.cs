using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuadShear : Entity2dQuadResult<Vector2, IShear2d> {
    internal Entity2dQuadShear(ReadOnlyCollection<Vector2> result)
      : base(result, (x) => new Shear2d(x)) {
    }
  }
}
