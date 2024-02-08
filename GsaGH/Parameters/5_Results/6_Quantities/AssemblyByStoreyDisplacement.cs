using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal class AssemblyByStoreyDisplacement {
    public IDictionary<string, IDisplacement> Results { get; private set; }

    internal AssemblyByStoreyDisplacement(ReadOnlyCollection<AssemblyResult> result, ReadOnlyCollection<string> storeyList) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(storeyList[i], new Displacement(result[i]));
      }
    }

    private AssemblyByStoreyDisplacement(IDictionary<string, IDisplacement> results) {
      Results = results;
    }
  }
}
