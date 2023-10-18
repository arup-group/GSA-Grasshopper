using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;

namespace GsaGH.Parameters.Results {

  public class GsaResult2 : IGsaResult {
    // API Global results
    internal AnalysisCaseResult AnalysisCaseResult { get; set; }
    internal CombinationCaseResult CombinationCaseResult { get; set; }

    // API Node results (will not be needed after GSA-7517)
    internal Dictionary<string, ReadOnlyDictionary<int, NodeResult>> AnalysisCaseNodeResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, NodeResult>>();

    internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>
      CombinationCaseNodeResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>();

    // Caches
    internal Dictionary<string, GsaNodeDisplacements> NodeDisplacementCache { get; set; }
      = new Dictionary<string, GsaNodeDisplacements>();

    // Other members
    public int CaseId { get; set; }
    public string CaseName { get; set; }
    public GsaModel Model { get; set; }

    public List<int> SelectedPermutationIds { get; set; }
    public CaseType CaseType { get; set; }

    public GsaResult2() { }

    internal GsaResult2(GsaModel model, AnalysisCaseResult result, int caseId) {
      Model = model;
      AnalysisCaseResult = result;
      CaseType = CaseType.AnalysisCase;
      CaseId = caseId;
      CaseName = model.Model.AnalysisCaseName(CaseId);
    }

    internal GsaResult2(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      Model = model;
      CombinationCaseResult = result;
      CaseType = CaseType.CombinationCase;
      CaseId = caseId;
      SelectedPermutationIds = permutations.OrderBy(x => x).ToList();
    }

    public override string ToString() {
      string txt = string.Empty;
      switch (CaseType) {
        case CaseType.AnalysisCase:
          txt = "A" + CaseId;
          break;

        case CaseType.CombinationCase:
          txt = "C" + CaseId;
          if (SelectedPermutationIds.Count > 0) {
            txt = SelectedPermutationIds.Count > 1 ? txt + " P:" + SelectedPermutationIds.Count :
              txt + " p" + SelectedPermutationIds[0];
          }

          break;
      }

      return txt.TrimSpaces();
    }

    internal GsaNodeDisplacements NodeDisplacementValues(string nodelist) {
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        nodelist = "All";
      }

      if (!NodeDisplacementCache.ContainsKey(nodelist)) {
        switch (CaseType) {
          case CaseType.AnalysisCase:
            if (!AnalysisCaseNodeResults.ContainsKey(nodelist)) {
              AnalysisCaseNodeResults.Add(nodelist, AnalysisCaseResult.NodeResults(nodelist));
            }

            NodeDisplacementCache.Add(nodelist, new GsaNodeDisplacements(AnalysisCaseNodeResults[nodelist]));
            break;

          case CaseType.CombinationCase:
            if (!CombinationCaseNodeResults.ContainsKey(nodelist)) {
              CombinationCaseNodeResults.Add(nodelist, CombinationCaseResult.NodeResults(nodelist));
            }

            NodeDisplacementCache.Add(nodelist, new GsaNodeDisplacements(CombinationCaseNodeResults[nodelist]));
            break;
        }
      }

      return NodeDisplacementCache[nodelist];
    }
  }
}