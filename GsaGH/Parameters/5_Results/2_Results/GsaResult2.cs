﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;

namespace GsaGH.Parameters.Results {

  public class GsaResult2 : IGsaResult {
    // Caches
    public INodeResultCache<IDisplacement, ResultVector6<NodeExtremaKey>> NodeDisplacements {
      get;
      private set;
    }
    public INodeResultCache<IInternalForce, ResultVector6<NodeExtremaKey>> NodeReactionForces {
      get;
      private set;
    }
    public INodeResultCache<IInternalForce, ResultVector6<NodeExtremaKey>> NodeSpringForces {
      get;
      private set;
    }

    // temp conversion from old class
    internal GsaResult2(GsaResult result) {
      switch (result.CaseType) {
        case CaseType.AnalysisCase:
          InitialiseAnalysisCaseResults(result.Model, result.AnalysisCaseResult, result.CaseId);
          break;

        case CaseType.CombinationCase:
          InitialiseCombinationsCaseResults(result.Model, result.CombinationCaseResult,
            result.CaseId, result.SelectedPermutationIds);
          break;
      }
    }

    internal GsaResult2(GsaModel model, AnalysisCaseResult result, int caseId) {
      InitialiseAnalysisCaseResults(model, result, caseId);
    }

    internal GsaResult2(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      InitialiseCombinationsCaseResults(model, result, caseId, permutations.OrderBy(x => x));
    }

    // Other members
    public int CaseId { get; set; }
    public string CaseName { get; set; }
    public GsaModel Model { get; set; }
    public List<int> SelectedPermutationIds { get; set; }
    public CaseType CaseType { get; set; }

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

    internal ReadOnlyCollection<int> NodeIds(string nodeList) {
      var entityList = new EntityList() {
        Definition = nodeList,
        Type = GsaAPI.EntityType.Node,
        Name = "node",
      };
      return Model.Model.ExpandList(entityList);
    }

    internal ReadOnlyCollection<int> ElementIds(string elementList) {
      var entityList = new EntityList() {
        Definition = elementList,
        Type = GsaAPI.EntityType.Element,
        Name = "elem",
      };
      return Model.Model.ExpandList(entityList);
    }

    internal ReadOnlyCollection<int> MemberIds(string memberList) {
      var entityList = new EntityList() {
        Definition = memberList,
        Type = GsaAPI.EntityType.Member,
        Name = "mem",
      };
      return Model.Model.ExpandList(entityList);
    }

    private void InitialiseAnalysisCaseResults(
      GsaModel model, AnalysisCaseResult result, int caseId) {
      Model = model;
      CaseType = CaseType.AnalysisCase;
      CaseId = caseId;
      CaseName = model.Model.AnalysisCaseName(CaseId);

      NodeDisplacements = new NodeDisplacementCache(result);
      NodeReactionForces = new NodeReactionForceCache(result, model.Model);
      NodeSpringForces = new NodeSpringForceCache(result);
    }

    private void InitialiseCombinationsCaseResults(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      Model = model;
      CaseType = CaseType.CombinationCase;
      CaseId = caseId;
      SelectedPermutationIds = permutations.ToList();

      NodeDisplacements = new NodeDisplacementCache(result);
      NodeReactionForces = new NodeReactionForceCache(result, model.Model);
      NodeSpringForces = new NodeSpringForceCache(result);
    }
  }
}
