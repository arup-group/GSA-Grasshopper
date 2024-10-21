using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTriDisplacement : Entity2dTriResult<Double6, IDisplacement> {
    internal Entity2dTriDisplacement(ReadOnlyCollection<Double6> result)
      : base(result, (x) => new Displacement(x)) {
    }
  }
}
