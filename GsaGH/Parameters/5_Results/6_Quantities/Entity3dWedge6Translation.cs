using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class Entity3dWedge6Translation : Entity3dWedge6Result<Double3, ITranslation> {
    internal Entity3dWedge6Translation(ReadOnlyCollection<Double3> result)
      : base(result, (x) => new Translation(x)) {
    }
  }
}
