using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTri6Displacement : Entity2dTri6Result<Double6, IDisplacement> {
    internal Entity2dTri6Displacement(ReadOnlyCollection<Double6> result)
      : base(result, (x) => new Displacement(x)) {
    }
  }
}
