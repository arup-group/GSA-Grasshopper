using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity1dDisplacement : IEntity1dDisplacement {
    public IDictionary<double, IDisplacement> Results { get; private set; }

    internal Entity1dDisplacement(IDictionary<double, IDisplacement> results) {
      Results = results;
    }

    internal Entity1dDisplacement(ReadOnlyCollection<Double6> result, ReadOnlyCollection<double> positions) {
      Results = new SortedDictionary<double, IDisplacement>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new Displacement(result[i]));
      }
    }

    public IEntity1dQuantity<IDisplacement> TakePositions(ICollection<double> positions) {
      var results = new SortedDictionary<double, IDisplacement>();
      foreach (double position in positions) {
        results.Add(position, Results[position]);
      }

      return new Entity1dDisplacement(results);
    }
  }
}
