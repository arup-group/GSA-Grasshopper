using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity2dTriStress : Entity2dTriResult<Tensor3, IStress> {
    internal Entity2dTriStress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
