﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AssemblyInternalForce : IAssemblyQuantity<IInternalForce> {
    public IDictionary<double, IInternalForce> Results { get; private set; }

    internal AssemblyInternalForce(ReadOnlyCollection<AssemblyResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new InternalForce(result[i]));
      }
    }

    //private AssemblyDisplacement(IDictionary<double, IDisplacement> results) {
    //  Results = results;
    //}
  }
}