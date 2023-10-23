using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;

namespace GsaGH.Parameters.Results {

  public class GsaResult2 : IGsaResult {
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
      CaseType = CaseType.AnalysisCase;
      CaseId = caseId;
      CaseName = model.Model.AnalysisCaseName(CaseId);
      NodeDisplacements = new AnalysisCaseNodeDisplacementCache(result);
    }

    internal GsaResult2(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      Model = model;
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