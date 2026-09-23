using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers {
  internal static partial class TaskHelper {
    public static int CreateDefaultStaticAnalysisTask(GsaModel model) {
      int taskId = model.ApiModel.AddAnalysisTask();
      model.ApiModel.CreateDefaultAnalysisCasesForTheTask(taskId);
      return taskId;
    }

    public static void AddAnalysisTask(GsaAnalysisTask task, GsaModel model) {
      task.Id = model.ApiModel.AddAnalysisTask(task.ApiTask);
      model.ApiModel.CreateDefaultAnalysisCasesForTheTask(task.Id);
    }

    public static void ImportAnalysisTask(GsaAnalysisTask task, ref GsaModel model) {
      ReadOnlyDictionary<int, AnalysisTask> existingTasks = model.ApiModel.AnalysisTasks();
      if (task != null && !existingTasks.Keys.Contains(task.Id)) {
        int highestTask = existingTasks.Count;
        Dictionary<int, AnalysisCase> analysisCases = BuildCustomCases(task);
        if (task.ApiTask != null) {
          if (analysisCases.Count == 0) {
            AddAnalysisTask(task, model);
          } else {
            model.ApiModel.ImportAnalysisTask(task.ApiTask, new ReadOnlyDictionary<int, AnalysisCase>(analysisCases));
            task.Id = highestTask + 1;
          }
        }
      }
    }

    public static void ImportAnalysisTask(GsaAnalysisTask task, Model model) {
      var gsaModel = new GsaModel(model);
      ImportAnalysisTask(task, ref gsaModel);
    }

    private static Dictionary<int, AnalysisCase> BuildCustomCases(GsaAnalysisTask task) {
      if (task.Cases == null || task.Cases.Count == 0) {
        return new Dictionary<int, AnalysisCase>();
      }
      return task.Cases
        .Select((c, i) => new { Index = i, Case = c })
        .ToDictionary(
          x => x.Index,
          x => new AnalysisCase(x.Case.Name, x.Case.Definition)
        );
    }
  }
}
