using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class AssemblyDrift : IAssemblyQuantity<IDrift<Length>> {
    public IDictionary<double, IDrift<Length>> Results { get; private set; }

    internal AssemblyDrift(ReadOnlyCollection<AssemblyDriftResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new Drift(result[i]));
      }
    }

    //private AssemblyDisplacement(IDictionary<double, IDisplacement> results) {
    //  Results = results;
    //}
  }
}
