using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  /// <summary>
  ///   Class containing functions to import various object types from GSA
  /// </summary>
  internal static class AnalysisHelper {
    internal static Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>>
      GetAnalysisTasksAndCombinations(GsaModel gsaModel) {
      return GetAnalysisTasksAndCombinations(gsaModel.Model);
    }

    internal static Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>>
      GetAnalysisTasksAndCombinations(Model model) {
      ReadOnlyDictionary<int, AnalysisTask> tasks = model.AnalysisTasks();

      var tasksList = new List<GsaAnalysisTaskGoo>();
      var caseList = new List<GsaAnalysisCaseGoo>();
      var caseIDs = new List<int>();

      foreach (KeyValuePair<int, AnalysisTask> item in tasks) {
        var task = new GsaAnalysisTask(item.Key, item.Value, model);
        tasksList.Add(new GsaAnalysisTaskGoo(task));
        caseIDs.AddRange(task.Cases.Select(acase => acase.Id));
      }

      caseIDs.AddRange(Loads.GetLoadCases(model));

      foreach (int caseId in caseIDs) {
        string caseName = model.AnalysisCaseName(caseId);
        if (caseName == string.Empty) {
          caseName = "Case " + caseId;
        }

        string caseDescription = model.AnalysisCaseDescription(caseId);
        if (caseDescription == string.Empty) {
          caseDescription = "L" + caseId;
        }

        caseList.Add(
          new GsaAnalysisCaseGoo(new GsaAnalysisCase(caseId, caseName, caseDescription)));
      }

      return new Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>>(tasksList, caseList);
    }
  }
}
