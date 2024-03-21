using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndex : IEntity1dQuantity<IDrift<double>> {
    public IDictionary<double, IDrift<double>> Results { get; private set; } = new Dictionary<double, IDrift<double>>();
    public IList<string> Storeys { get; private set; } = new List<string>();

    internal AssemblyDriftIndex(ReadOnlyCollection<AssemblyDriftIndexResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new DriftIndex(result[i]));
        Storeys.Add(result[i].Storey);
      }
    }

    public IEntity1dQuantity<IDrift<double>> TakePositions(ICollection<double> positions) {
      throw new NotImplementedException("Assembly drift indices don´t support dynamic positions");
    }
  }
}
