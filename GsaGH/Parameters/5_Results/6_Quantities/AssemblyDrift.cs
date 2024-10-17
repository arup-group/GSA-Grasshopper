using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDrift : IEntity1dQuantity<Drift> {
    public IDictionary<double, Drift> Results { get; private set; } = new Dictionary<double, Drift>();
    public IList<string> Storeys { get; private set; } = new List<string>();

    internal AssemblyDrift(ReadOnlyCollection<AssemblyDriftResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new Drift(result[i]));
        Storeys.Add(result[i].Storey);
      }
    }

    public IEntity1dQuantity<Drift> TakePositions(ICollection<double> positions) {
      throw new NotImplementedException("Assembly drifts don´t support dynamic positions");
    }
  }
}
