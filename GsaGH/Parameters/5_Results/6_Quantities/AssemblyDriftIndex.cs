using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndex : IAssemblyQuantity<IDrift<double>> {
    public IDictionary<double, IDrift<double>> Results { get; private set; }

    internal AssemblyDriftIndex(ReadOnlyCollection<AssemblyDriftIndicesResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new DriftIndex(result[i]));
      }
    }

    //private AssemblyDisplacement(IDictionary<double, IDisplacement> results) {
    //  Results = results;
    //}
  }
}
