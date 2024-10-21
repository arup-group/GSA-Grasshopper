using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dPyramid5Translation : Entity3dPyramid5Result<Double3, ITranslation> {
    internal Entity3dPyramid5Translation(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
