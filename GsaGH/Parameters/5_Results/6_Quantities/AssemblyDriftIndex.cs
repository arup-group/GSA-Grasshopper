using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDriftIndex : IEntity1dQuantity<DriftIndex> {
    public IDictionary<double, DriftIndex> Results { get; private set; } = new Dictionary<double, DriftIndex>();
    public IList<string> Storeys { get; private set; } = new List<string>();
    public ICollection<string> Warnings { get; internal set; } = new HashSet<string>();
    public ICollection<string> Errors { get; internal set; } = new HashSet<string>();

    internal AssemblyDriftIndex(ReadOnlyCollection<AssemblyDriftIndexResult> result) {
      for (int i = 0; i < result.Count; i++) {
        if (Results.ContainsKey(result[i].Position)) {
          Warnings.Add(
            "Some of the assembly drift indexes has been skipped. Two or more results has the same positions.");
          continue;
        }

        Results.Add(result[i].Position, new DriftIndex(result[i]));
        Storeys.Add(result[i].Storey);
      }
    }

    public IEntity1dQuantity<DriftIndex> TakePositions(ICollection<double> positions) {
      throw new NotImplementedException("Assembly drift indices don´t support dynamic positions");
    }
  }
}
