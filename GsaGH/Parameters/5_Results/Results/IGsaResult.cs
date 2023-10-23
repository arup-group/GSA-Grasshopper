﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IGsaResult {
    public int CaseId { get; }
    public string CaseName { get; }
    public GsaModel Model { get; }
    public List<int> SelectedPermutationIds { get; }
    public CaseType CaseType { get; }
  } 
}