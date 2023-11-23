using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dBrickDisplacement : Entity3dBrickResult<Double3, ITranslation> {
    internal Entity3dBrickDisplacement(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
