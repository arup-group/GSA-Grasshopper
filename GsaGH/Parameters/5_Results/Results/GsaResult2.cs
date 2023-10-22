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
    internal INodeResultCache<IDisplacement> NodeDisplacements { get; set; }

    // Other members
    public int CaseId { get; set; }
    public string CaseName { get; set; }
    public GsaModel Model { get; set; }

    public List<int> SelectedPermutationIds { get; set; }
    public CaseType CaseType { get; set; }

    internal GsaResult2(GsaModel model, AnalysisCaseResult result, int caseId) {
      Model = model;
      AnalysisCaseResult = result;
      CaseType = CaseType.AnalysisCase;
      CaseId = caseId;
      CaseName = model.Model.AnalysisCaseName(CaseId);
      NodeDisplacements = new AnalysisCaseNodeDisplacementCache(result);
    }

    internal GsaResult2(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      Model = model;
      CombinationCaseResult = result;
      CaseType = CaseType.CombinationCase;
      CaseId = caseId;
      SelectedPermutationIds = permutations.OrderBy(x => x).ToList();
      NodeDisplacements = new CombinationCaseNodeDisplacementCache(result);
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
      var entityList = new EntityList() {
        Definition = nodelist,
        Type = GsaAPI.EntityType.Node,
        Name = "tmp"
      };
      ReadOnlyCollection<int> nodeIds = Model.Model.ExpandList(entityList);
      return (GsaNodeDisplacements)NodeDisplacements.ResultSubset(nodeIds);
    }
  }
}