using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public class Entity1dStrainEnergyDensity : IEntity1dStrainEnergyDensity {
    public IDictionary<double, IEnergyDensity> Results { get; private set; }

    internal Entity1dStrainEnergyDensity(ReadOnlyCollection<double> result, ReadOnlyCollection<double> positions) {
      Results = new Dictionary<double, IEnergyDensity>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new StrainEnergyDensity(result[i]));
      }
    }
  }
}
