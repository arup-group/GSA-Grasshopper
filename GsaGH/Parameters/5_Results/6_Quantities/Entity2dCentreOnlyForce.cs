using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dCentreOnlyForce : Entity2dCentreOnlyResult<Tensor2, IForce2d> {
    internal Entity2dCentreOnlyForce(ReadOnlyCollection<Tensor2> result) 
      : base (result, (x) => new Force2d(x)) {
    }
  }
}
