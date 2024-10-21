using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dBrick8Translation : Entity3dBrick8Result<Double3, ITranslation> {
    internal Entity3dBrick8Translation(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
