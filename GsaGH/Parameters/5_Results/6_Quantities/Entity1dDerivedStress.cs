using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity1dDerivedStress : IEntity1dDerivedStress {
    public IDictionary<double, IStress1dDerived> Results { get; private set; }

    internal Entity1dDerivedStress(IDictionary<double, IStress1dDerived> results) {
      Results = results;
    }

    internal Entity1dDerivedStress(ReadOnlyCollection<DerivedStressResult1d> result, ReadOnlyCollection<double> positions) {
      Results = new SortedDictionary<double, IStress1dDerived>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new Stress1dDerived(result[i]));
      }
    }

    public IEntity1dQuantity<IStress1dDerived> TakePositions(ICollection<double> positions) {
      var results = new SortedDictionary<double, IStress1dDerived>();
      foreach (double position in positions) {
        results.Add(position, Results[position]);
      }

      return new Entity1dDerivedStress(results);
    }
  }
}
