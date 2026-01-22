using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyInternalForce : IEntity1dQuantity<IInternalForce> {
    public IDictionary<double, IInternalForce> Results { get; private set; } = new Dictionary<double, IInternalForce>();
    public IList<string> Storeys { get; private set; } = new List<string>();

    internal AssemblyInternalForce(ReadOnlyCollection<AssemblyResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results[result[i].Position] = new InternalForce(result[i]);
      }
      Storeys = result.Select(r => r.Storey).Distinct().ToList();
    }

    public IEntity1dQuantity<IInternalForce> TakePositions(ICollection<double> positions) {
      throw new NotImplementedException("Assembly forces don´t support dynamic positions");
    }
  }
}
