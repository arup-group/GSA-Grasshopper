using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dTetra4Translation : Entity3dTetra4Result<Double3, ITranslation> {
    internal Entity3dTetra4Translation(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
