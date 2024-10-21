using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuad8Shear : Entity2dQuad8Result<Vector2, IShear2d> {
    internal Entity2dQuad8Shear(ReadOnlyCollection<Vector2> result)
      : base(result, (x) => new Shear2d(x)) {
    }
  }
}
