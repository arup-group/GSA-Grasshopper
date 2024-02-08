using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDisplacement : IAssemblyQuantity<IDisplacement> {
    public IDictionary<double, IDisplacement> Results { get; private set; }

    internal AssemblyDisplacement(ReadOnlyCollection<AssemblyResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new Displacement(result[i]));
      }
    }

    //private AssemblyDisplacement(IDictionary<double, IDisplacement> results) {
    //  Results = results;
    //}
  }
}
