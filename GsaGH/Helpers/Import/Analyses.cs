using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Analyses {
    internal static Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> GetAnalysisTasksAndCombinations(GsaModel gsaModel) => GetAnalysisTasksAndCombinations(gsaModel.Model);
    internal static Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> GetAnalysisTasksAndCombinations(Model model) {
      ReadOnlyDictionary<int, AnalysisTask> tasks = model.AnalysisTasks();

      var tasksList = new List<GsaAnalysisTaskGoo>();
      var caseList = new List<GsaAnalysisCaseGoo>();
      var caseIDs = new List<int>();

      foreach (KeyValuePair<int, AnalysisTask> item in tasks) {
        var task = new GsaAnalysisTask(item.Key, item.Value, model);
        tasksList.Add(new GsaAnalysisTaskGoo(task));
        caseIDs.AddRange(task.Cases.Select(acase => acase.Id));
      }
      ReadOnlyCollection<GravityLoad> gravities = model.GravityLoads();
      caseIDs.AddRange(gravities.Select(x => x.Case));

      foreach (NodeLoadType typ in Enum.GetValues(typeof(NodeLoadType))) {
        ReadOnlyCollection<NodeLoad> nodeLoads;
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          nodeLoads = model.NodeLoads(typ);
          caseIDs.AddRange(nodeLoads.Select(x => x.Case));
        }
        catch (Exception) {
          // ignored
        }
      }

      ReadOnlyCollection<BeamLoad> beamLoads = model.BeamLoads();
      caseIDs.AddRange(beamLoads.Select(x => x.Case));

      ReadOnlyCollection<FaceLoad> faceLoads = model.FaceLoads();
      caseIDs.AddRange(faceLoads.Select(x => x.Case));

      ReadOnlyCollection<GridPointLoad> gridPointLoads = model.GridPointLoads();
      caseIDs.AddRange(gridPointLoads.Select(x => x.Case));

      ReadOnlyCollection<GridLineLoad> gridLineLoads = model.GridLineLoads();
      caseIDs.AddRange(gridLineLoads.Select(x => x.Case));

      ReadOnlyCollection<GridAreaLoad> gridAreaLoads = model.GridAreaLoads();
      caseIDs.AddRange(gridAreaLoads.Select(x => x.Case));

      caseIDs = caseIDs.GroupBy(x => x).Select(y => y.First()).ToList();

      foreach (int caseId in caseIDs) {
        string caseName = model.AnalysisCaseName(caseId);
        if (caseName == "")
          caseName = "Case " + caseId;
        string caseDescription = model.AnalysisCaseDescription(caseId);
        if (caseDescription == "")
          caseDescription = "L" + caseId;
        caseList.Add(new GsaAnalysisCaseGoo(new GsaAnalysisCase(caseId, caseName, caseDescription)));
      }

      return new Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>>(tasksList, caseList);
    }
  }
}
