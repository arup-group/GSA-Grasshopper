using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dCentreOnlyMoment : Entity2dCentreOnlyResult<Tensor2, IMoment2d> {
    internal Entity2dCentreOnlyMoment(ReadOnlyCollection<Tensor2> result) 
      : base (result, (x) => new Moment2d(x)) {
    }
  }
}
