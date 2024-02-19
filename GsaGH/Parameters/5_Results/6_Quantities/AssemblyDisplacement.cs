﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyDisplacement : IAssemblyQuantity<IDisplacement> {
    public IDictionary<double, IDisplacement> Results { get; private set; } = new Dictionary<double, IDisplacement>();
    public IList<string> Storeys { get; private set; } = new List<string>();

    internal AssemblyDisplacement(ReadOnlyCollection<AssemblyResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new Displacement(result[i]));
        Storeys.Add(result[i].Storey);
      }
    }
  }
}
