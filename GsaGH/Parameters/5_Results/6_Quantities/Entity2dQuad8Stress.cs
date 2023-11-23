using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuad8Stress : Entity2dQuad8Result<Tensor3, IStress2d> {
    internal Entity2dQuad8Stress(ReadOnlyCollection<Tensor3> result) 
      : base (result, (x) => new Stress2d(x)) {
    }
  }
}
