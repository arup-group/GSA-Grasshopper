using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers;

using Rhino.Commands;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>An analysis task is a package of work for the solver. Thus we can have a static analysis task, a modal analysis task, etc. Each analysis task has one or more analysis case(s). The distinction is that the cases corresponds to result sets and define items such as loading (in the static case) while the task describes what the solver has to do. </para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/analysistasks.html">Analysis Tasks</see> to read more.</para>
  /// </summary>
  public class GsaAnalysisTask {
    public List<GsaAnalysisCase> Cases { get; set; } = new List<GsaAnalysisCase>();
    public int Id { get; set; } = 0;
    public AnalysisTask ApiTask { get; internal set; }

    public GsaAnalysisTask() {
      Id = 0;
    }

    internal GsaAnalysisTask(int id, Model model) {
      Id = id;
      ApiTask = model.AnalysisTasks()[Id];
      CreateCases(model);
    }

    internal GsaAnalysisTask(AnalysisTask task, Model model) {
      ApiTask = task;
      CreateCases(model);
    }
    private void CreateCases(Model model) {
      ReadOnlyDictionary<int, AnalysisCase> analysisCases = model.AnalysisCases();
      foreach (int caseId in ApiTask.Cases.Where(x => analysisCases.ContainsKey(x))) {
        AnalysisCase analysisCase = model.AnalysisCases()[caseId];
        Cases.Add(new GsaAnalysisCase(caseId, analysisCase.Name, analysisCase.Description));
      }
    }
    public override string ToString() {
      return (Id > 0 ? $"ID:{Id} " : string.Empty) + $"'{ApiTask.Name}' {ApiTask.Type}".Replace("_", " ")
        .TrimSpaces();
    }
  }
}
