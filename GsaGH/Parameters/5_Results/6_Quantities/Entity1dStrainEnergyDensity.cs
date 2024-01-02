using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public class Entity1dStrainEnergyDensity : IEntity1dStrainEnergyDensity {
    public IDictionary<double, IEnergyDensity> Results { get; private set; }

    internal Entity1dStrainEnergyDensity(IDictionary<double, IEnergyDensity> results) {
      Results = results;
    }

    internal Entity1dStrainEnergyDensity(ReadOnlyCollection<double> result, ReadOnlyCollection<double> positions) {
      Results = new SortedDictionary<double, IEnergyDensity>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new StrainEnergyDensity(result[i]));
      }
    }

    public IEntity1dQuantity<IEnergyDensity> TakePositions(ICollection<double> positions) {
      var results = new SortedDictionary<double, IEnergyDensity>();
      foreach (double position in positions) {
        results.Add(position, Results[position]);
      }

      return new Entity1dStrainEnergyDensity(results);
    }
  }
}
