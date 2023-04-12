﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel.Data;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.GsaApi {
  internal partial class ResultHelper {
    public static Tuple<List<string>, List<int>, DataTree<int?>> GetAvalailableResults(GsaModel model) {
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.Model.Results();
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.Model.CombinationCaseResults();
      int tempNodeId = model.Model.Nodes().Keys.First();

      var type = new List<string>();
      var caseIds = new List<int>();
      var perm = new DataTree<int?>();
      foreach (int caseId in analysisCaseResults.Keys) {
        type.Add("Analysis");
        caseIds.Add(caseId);
      }
      foreach (int caseId in combinationCaseResults.Keys.OrderByDescending(x => -x)) {
        type.Add("Combination");
        caseIds.Add(caseId);
        IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = combinationCaseResults[caseId].NodeResults(tempNodeId.ToString());
        int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
        var permutationsInCase = Enumerable.Range(1, nP).ToList();
        var path = new GH_Path(caseId);
        if (permutationsInCase.Count == 0)
          perm.Add(null, path);
        foreach (int p in permutationsInCase)
          perm.Add(p, path);

      }
      return new Tuple<List<string>, List<int>, DataTree<int?>>(type, caseIds, perm);
    }
  }
}
