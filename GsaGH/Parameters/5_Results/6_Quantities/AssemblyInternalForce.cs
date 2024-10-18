using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyInternalForce : IEntity1dQuantity<IInternalForce> {
    public IDictionary<double, IInternalForce> Results { get; private set; } = new Dictionary<double, IInternalForce>();
    public IList<string> Storeys { get; private set; } = new List<string>();
    public ICollection<string> Warnings { get; internal set; } = new HashSet<string>();
    public ICollection<string> Errors { get; internal set; } = new HashSet<string>();

    internal AssemblyInternalForce(ReadOnlyCollection<AssemblyResult> result) {
      for (int i = 0; i < result.Count; i++) {
        if (Results.ContainsKey(result[i].Position)) {
          Warnings.Add(
            "Some of the assembly internal forces has been skipped. Two or more results has the same positions.");
          continue;
        }

        Results.Add(result[i].Position, new InternalForce(result[i]));
        Storeys.Add(result[i].Storey);
      }
    }

    public IEntity1dQuantity<IInternalForce> TakePositions(ICollection<double> positions) {
      throw new NotImplementedException("Assembly forces don´t support dynamic positions");
    }
  }
}
