using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity1dStress : IEntity1dStress {
    public IDictionary<double, IStress1d> Results { get; private set; }

    internal Entity1dStress(IDictionary<double, IStress1d> results) {
      Results = results;
    }

    internal Entity1dStress(ReadOnlyCollection<StressResult1d> result, ReadOnlyCollection<double> positions) {
      Results = new SortedDictionary<double, IStress1d>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new Stress1d(result[i]));
      }
    }

    public IEntity1dQuantity<IStress1d> TakePositions(ICollection<double> positions) {
      var results = new SortedDictionary<double, IStress1d>();
      foreach (double position in positions) {
        results.Add(position, Results[position]);
      }

      return new Entity1dStress(results);
    }
  }
}
