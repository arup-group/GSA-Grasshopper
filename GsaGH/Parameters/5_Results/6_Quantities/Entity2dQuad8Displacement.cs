using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity2dQuad8Displacement : Entity2dQuad8Result<IDisplacement> {
    public Entity2dQuad8Displacement(ReadOnlyCollection<Double6> result) 
      : base (result, (x) => new Displacement(x)) {
    }
  }
}
