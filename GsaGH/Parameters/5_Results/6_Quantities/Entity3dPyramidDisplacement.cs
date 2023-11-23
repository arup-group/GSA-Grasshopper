using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dPyramidDisplacement : Entity3dPyramidResult<Double3, ITranslation> {
    internal Entity3dPyramidDisplacement(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
