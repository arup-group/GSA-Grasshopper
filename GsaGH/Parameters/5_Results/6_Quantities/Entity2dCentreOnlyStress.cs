using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dCentreOnlyStress : Entity2dCentreOnlyResult<Tensor3, IStress> {
    internal Entity2dCentreOnlyStress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
