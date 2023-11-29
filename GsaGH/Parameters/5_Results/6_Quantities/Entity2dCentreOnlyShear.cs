using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dCentreOnlyShear : Entity2dCentreOnlyResult<Vector2, IShear2d> {
    internal Entity2dCentreOnlyShear(ReadOnlyCollection<Vector2> result) 
      : base (result, (x) => new Shear2d(x)) {
    }
  }
}
