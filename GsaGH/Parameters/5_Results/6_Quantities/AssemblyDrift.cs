﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public class AssemblyDrift : IEntity1dQuantity<IDrift<Length>> {
    public IDictionary<double, IDrift<Length>> Results { get; private set; } = new Dictionary<double, IDrift<Length>>();
    public IList<string> Storeys { get; private set; } = new List<string>();

    internal AssemblyDrift(ReadOnlyCollection<AssemblyDriftResult> result) {
      for (int i = 0; i < result.Count; i++) {
        Results.Add(result[i].Position, new Drift(result[i]));
        Storeys.Add(result[i].Storey);
      }
    }

    public IEntity1dQuantity<IDrift<Length>> TakePositions(ICollection<double> positions) {
      throw new NotImplementedException("Assembly drifts don´t support dynamic positions");
    }
  }
}
