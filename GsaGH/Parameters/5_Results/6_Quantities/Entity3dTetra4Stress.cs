using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dTetra4Stress : Entity3dTetra4Result<Tensor3, IStress> {
    internal Entity3dTetra4Stress(ReadOnlyCollection<Tensor3> result)
      : base(result, (x) => new Stress(x)) {
    }
  }
}
