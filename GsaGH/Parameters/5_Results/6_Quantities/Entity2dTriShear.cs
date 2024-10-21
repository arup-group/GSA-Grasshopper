using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTriShear : Entity2dTriResult<Vector2, IShear2d> {
    internal Entity2dTriShear(ReadOnlyCollection<Vector2> result)
      : base(result, (x) => new Shear2d(x)) {
    }
  }
}
