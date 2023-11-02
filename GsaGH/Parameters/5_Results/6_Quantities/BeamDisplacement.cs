using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class BeamDisplacement : IBeamDisplacement {
    public IDictionary<double, IDisplacement> Displacements { get; private set; }

    internal BeamDisplacement(ReadOnlyCollection<Double6> result, ReadOnlyCollection<double> positions) {
      Displacements = new Dictionary<double, IDisplacement>();
      for (int i = 0; i < result.Count; i++) { 
        Displacements.Add(positions[i], new Displacement(result[i]));
      }
    }
  }
}
