using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dTetraDisplacement : Entity3dTetraResult<Double3, ITranslation> {
    internal Entity3dTetraDisplacement(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
