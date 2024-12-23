using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers {
  internal static partial class TaskHelper {
    public static int CreateDefaultStaticAnalysisTask(ref GsaModel model) {
      int taskId = model.ApiModel.AddAnalysisTask();
      model.ApiModel.CreateDefaultAnalysisCasesForTheTask(taskId);
      return taskId;
    }

    public static int AddAnalysisTask(ref GsaAnalysisTask task, ref GsaModel model) {
      int taskId = model.ApiModel.AddAnalysisTask(task.ApiTask);
      model.ApiModel.CreateDefaultAnalysisCasesForTheTask(taskId);
      return taskId;
    }

    public static void ImportAnalysisTask(GsaAnalysisTask task, ref GsaModel model) {
      ReadOnlyDictionary<int, AnalysisTask> existingTasks = model.ApiModel.AnalysisTasks();
      if (task != null && !existingTasks.Keys.Contains(task.Id)) {
        int highestTask = existingTasks.Count();
        var analysisCases = new Dictionary<int, AnalysisCase>();
        foreach (GsaAnalysisCase analysisCase in task.Cases) {
          analysisCases.Add(analysisCase.Id, new AnalysisCase(analysisCase.Name, analysisCase.Definition));
        }
        if (task.ApiTask != null) {
          if (analysisCases.Count == 0) {
            AddAnalysisTask(ref task, ref model);
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
  }
}
