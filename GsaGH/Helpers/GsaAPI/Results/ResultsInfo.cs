using Grasshopper;
using Grasshopper.Kernel.Data;
using GsaAPI;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GsaGH.Helpers.GsaAPI
{
  internal partial class ResultHelper
  {
    public static Tuple<List<string>, List<int>, DataTree<int?>> GetAvalailableResults(GsaModel model)
    {
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.Model.Results();
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.Model.CombinationCaseResults();
      int tempNodeID = model.Model.Nodes().Keys.First();

      List<string> type = new List<string>();
      List<int> caseIds = new List<int>();
      DataTree<int?> perm = new DataTree<int?>();
      foreach (int caseId in analysisCaseResults.Keys)
      {
        type.Add("Analysis");
        caseIds.Add(caseId);
      }
      foreach (int caseId in combinationCaseResults.Keys.OrderByDescending(x => -x))
      {
        type.Add("Combination");
        caseIds.Add(caseId);
        IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = combinationCaseResults[caseId].NodeResults(tempNodeID.ToString());
        int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
        List<int> permutationsInCase = Enumerable.Range(1, nP).ToList();
        GH_Path path = new GH_Path(caseId);
        foreach (int p in permutationsInCase)
          perm.Add(p, path);
      }
      return new Tuple<List<string>, List<int>, DataTree<int?>>(type, caseIds, perm);
    }
  }
}
