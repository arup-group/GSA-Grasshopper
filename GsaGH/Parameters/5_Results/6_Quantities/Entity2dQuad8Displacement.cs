using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuad8Displacement : Entity2dQuad8Result<Double6, IDisplacement> {
    internal Entity2dQuad8Displacement(ReadOnlyCollection<Double6> result)
      : base(result, (x) => new Displacement(x)) {
    }
  }
}
