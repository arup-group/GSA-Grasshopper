using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Element1dDerivedStress : IElement1dDerivedStress {
    public IDictionary<double, IStress1dDerived> Results { get; private set; }

    internal Element1dDerivedStress(ReadOnlyCollection<DerivedStressResult1d> result, ReadOnlyCollection<double> positions) {
      Results = new Dictionary<double, IStress1dDerived>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], new Stress1dDerived(result[i]));
      }
    }
  }
}
