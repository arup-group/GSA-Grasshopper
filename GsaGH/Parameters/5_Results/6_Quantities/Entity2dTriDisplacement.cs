using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity2dTriDisplacement : Entity2dTriResult<IDisplacement> {
    internal Entity2dTriDisplacement(ReadOnlyCollection<Double6> result)
      : base(result, (x) => new Displacement(x)) {
    }
  }
}
