using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity1dInternalForce : IEntity1dInternalForce {
    public IDictionary<double, IInternalForce> Results { get; private set; }

    internal Entity1dInternalForce(IDictionary<double, IInternalForce> results) {
      Results = results;
    }

    internal Entity1dInternalForce(ReadOnlyCollection<Double6> result, ReadOnlyCollection<double> positions) {
      Results = new SortedDictionary<double, IInternalForce>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new InternalForce(result[i]));
      }
    }

    public IEntity1dQuantity<IInternalForce> TakePositions(ICollection<double> positions) {
      var results = new SortedDictionary<double, IInternalForce>();
      foreach (double position in positions) {
        results.Add(position, Results[position]);
      }

      return new Entity1dInternalForce(results);
    }
  }
}
