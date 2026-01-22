using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndex : IEntity1dQuantity<DriftIndex> {
    public IDictionary<double, DriftIndex> Results { get; private set; } = new Dictionary<double, DriftIndex>();
    public IList<string> Storeys { get; private set; } = new List<string>();

    internal AssemblyDriftIndex(ReadOnlyCollection<AssemblyDriftIndexResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results[result[i].Position] = new DriftIndex(result[i]);
      }
      Storeys = result.Select(r => r.Storey).Distinct().ToList();
    }

    public IEntity1dQuantity<DriftIndex> TakePositions(ICollection<double> positions) {
      throw new NotImplementedException("Assembly drift indices don´t support dynamic positions");
    }
  }
}
