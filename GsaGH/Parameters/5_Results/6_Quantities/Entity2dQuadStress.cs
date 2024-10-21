using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dQuadStress : Entity2dQuadResult<Tensor3, IStress> {
    internal Entity2dQuadStress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
