using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuadDisplacement : Entity2dQuadResult<Double6, IDisplacement> {
    internal Entity2dQuadDisplacement(ReadOnlyCollection<Double6> result)
      : base(result, (x) => new Displacement(x)) {
    }
  }
}
