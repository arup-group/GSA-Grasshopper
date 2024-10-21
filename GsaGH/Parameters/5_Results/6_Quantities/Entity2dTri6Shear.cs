using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTri6Shear : Entity2dTri6Result<Vector2, IShear2d> {
    internal Entity2dTri6Shear(ReadOnlyCollection<Vector2> result)
      : base(result, (x) => new Shear2d(x)) {
    }
  }
}
