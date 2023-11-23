using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dWedgeDisplacement : Entity3dWedgeResult<Double3, ITranslation> {
    internal Entity3dWedgeDisplacement(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
