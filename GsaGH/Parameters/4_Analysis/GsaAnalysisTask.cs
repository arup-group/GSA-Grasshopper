using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters.Enums;

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

    internal GsaAnalysisTask(int id, AnalysisTask task, Model model) {
      Id = id;
      foreach (int caseId in task.Cases) {
        string caseName = model.AnalysisCaseName(caseId);
        string caseDescription = model.AnalysisCaseDescription(caseId);
        Cases.Add(new GsaAnalysisCase(caseId, caseName, caseDescription));
      }

      ApiTask = task;
    }

    public override string ToString() {
      return (Id > 0 ? $"ID:{Id} " : string.Empty) + $"'{ApiTask.Name}' {ApiTask.Type}".Replace("_", " ")
        .TrimSpaces();
    }

    internal void CreateDefaultCases(GsaModel gsaModel) {
      Cases.Clear();
      switch ((AnalysisTaskType)ApiTask.Type) {
        case AnalysisTaskType.Static:
        case AnalysisTaskType.StaticPDelta:
          Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple
       = gsaModel.GetAnalysisTasksAndCombinations();
          Cases = tuple.Item2.Select(x => x.Value).ToList();
          break;
        case AnalysisTaskType.Footfall:
          var footfallAnalysisCase = new GsaAnalysisCase {
            Name = ApiTask.Name,
            Definition = "Footfall",
          };
          Cases.Add(footfallAnalysisCase);
          break;
        default:
          break;
      }
    }
  }
}
