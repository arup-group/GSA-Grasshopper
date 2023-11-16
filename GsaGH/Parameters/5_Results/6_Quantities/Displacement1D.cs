using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Displacement1d : IDisplacement1d {

    internal Displacement1d(
      ReadOnlyCollection<Double6> result, ReadOnlyCollection<double> positions) {
      Results = new Dictionary<double, IDisplacement>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new Displacement(result[i]));
      }
    }

    public IDictionary<double, IDisplacement> Results { get; private set; }
  }
}
