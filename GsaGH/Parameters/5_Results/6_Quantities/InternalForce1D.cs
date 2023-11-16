using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class InternalForce1d : IInternalForce1d {

    internal InternalForce1d(
      ReadOnlyCollection<Double6> result, ReadOnlyCollection<double> positions) {
      Results = new Dictionary<double, IInternalForce>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new InternalForce(result[i]));
      }
    }

    public IDictionary<double, IInternalForce> Results { get; private set; }
  }
}
