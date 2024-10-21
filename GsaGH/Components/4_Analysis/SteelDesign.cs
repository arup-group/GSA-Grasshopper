using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to trigger design task from a GSA model
  /// </summary>
  public class SteelDesign : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("5f4e2474-1d23-48a1-98bd-14f7b942879f");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SteelDesign;

    public SteelDesign() : base("Steel Design", "SteelDesign", "Optimise or check the steel sections in a Model",
      CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddIntegerParameter("Task number", "ID", "[Optional] The ID of the Task to Design or Check." +
        "By default the first steel task will be run.",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Iterations", "Itr", "Set this input iterate through the steps " +
        "\nA) Design -> B) ElementsFromMembers -> C) Analyse -> A) Design" +
        "\nTo only run the above loop once set the input to 1.",
        GH_ParamAccess.item, 0);
      pManager.AddBooleanParameter("Check only", "Chk",
        "Set to true to only perform a check of the section capacities",
        GH_ParamAccess.item, false);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddTextParameter("Errors", "E", "Analysis Task Errors", GH_ParamAccess.list);
      pManager.AddTextParameter("Warnings", "W", "Analysis Task Warnings", GH_ParamAccess.list);
      pManager.AddTextParameter("Remarks", "R", "Analysis Task Notes and Remarks", GH_ParamAccess.list);
      pManager.AddTextParameter("Logs", "L", "Analysis Task logs", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      Model newModel;
      da.GetData(0, ref modelGoo);
      int taskId = 0;
      ReadOnlyDictionary<int, SteelDesignTask> steelDesignTasks =
          modelGoo.Value.ApiModel.SteelDesignTasks();
      if (steelDesignTasks.Count == 0) {
        this.AddRuntimeError("Model contains no design tasks");
        return;
      }

      da.GetData(1, ref taskId);
      if (taskId == 0) {
        taskId = steelDesignTasks.Keys.Min();
      } else {
        if (!steelDesignTasks.ContainsKey(taskId)) {
          this.AddRuntimeError("Model contains no design task with ID " + taskId);
          return;
        }
      }

      int iterations = 0;
      da.GetData(2, ref iterations);
      bool check = false;
      da.GetData(3, ref check);

      var logs = new List<string>();
      var notes = new List<string>();
      var warnings = new List<string>();
      var errors = new List<string>();
      TaskReport report = null;

      if (check) {
        newModel = modelGoo.Value.ApiModel;
        newModel.Check(taskId, out report);
        logs.Add(report.Log);
        notes.AddRange(report.Notes);
        foreach (string message in report.Warnings) {
          this.AddRuntimeWarning($"Task {taskId}: {message}");
          warnings.Add($"Task {taskId}: {message}");
        }

        foreach (string message in report.SevereWarnings) {
          this.AddRuntimeWarning($"Task {taskId}: {message}");
          warnings.Add($"Task {taskId}: {message}");
        }

        foreach (string message in report.Errors) {
          this.AddRuntimeError($"Task {taskId}: {message}");
          errors.Add($"Task {taskId}: {message}");
        }
      } else {
        newModel = modelGoo.Value.ApiModel.Clone();
        List<int> analysisTaskIds = GetDependentAnalysisTasksFromDesignTask(newModel, taskId);
        string memberList = steelDesignTasks[taskId].ListDefinition;
        List<int> initialSectionIds = GetMemberSectionIds(newModel, memberList);
        newModel.Design(taskId, out report);
        List<int> postDesignSectionIds = GetMemberSectionIds(newModel, memberList);
        bool converged = false;
        bool hasError = false;
        for (int i = 0; i < iterations; i++) {
          if (report.Errors.Count > 0) {
            this.AddRuntimeError($"Optimisation was stopped after {i} iteration(s)" +
            $" as the Design Task reported errors and was unable to run");

            hasError = true;
            break;
          }

          // synchronise analysis layer after design task has updated sections
          newModel.CreateElementsFromMembers();
          // re-run analysis, but only for tasks required for the combination case definition
          foreach (int analysisTaskId in analysisTaskIds) {
            if (!newModel.Analyse(analysisTaskId, out report)) {
              this.AddRuntimeError($"Optimisation was stopped after {i} iteration(s)" +
              $" as the Analysis Task {analysisTaskId} failed with one or more errors. " +
              $"Check report output for details");
              hasError = true;
              break;
            }
          }

          if (hasError) {
            break;
          }

          List<int> preDesignSectionIds = GetMemberSectionIds(newModel, memberList);
          newModel.Design(taskId, out report);
          postDesignSectionIds = GetMemberSectionIds(newModel, memberList);
          if (preDesignSectionIds.SequenceEqual(postDesignSectionIds)) {
            iterations = i + 1;
            converged = true;
            break;
          }
        }

        if (!hasError) {
          int changedSections = 0;
          int notChangedSections = 0;
          for (int j = 0; j < initialSectionIds.Count; j++) {
            if (initialSectionIds[j] != postDesignSectionIds[j]) {
              changedSections++;
            } else {
              notChangedSections++;
            }
          }

          string sync = (iterations == 0 && changedSections > 0)
            ? "\nRemember to synchronise the changes to the Analysis layer!"
            : string.Empty;
          string noChanges = notChangedSections == 0
            ? string.Empty
            : $" and leaving {notChangedSections} section(s) unchanged";
          string changes = changedSections == 0
            ? "without changing any sections."
            : $"with {changedSections} changed section(s)" + noChanges;
          string note = converged
            ? $"Optimisation converged after {iterations} iteration(s) {changes}"
            : iterations == 0
              ? $"Design Task succeeded {changes}{sync}"
              : $"Optimisation did not converge within {iterations} iterations {changes}";
          this.AddRuntimeRemark(note);
          notes.Add(note);
        }

        logs.Add(report.Log);
        notes.AddRange(report.Notes);
        foreach (string message in report.Warnings) {
          this.AddRuntimeWarning(message);
          warnings.Add(message);
        }

        foreach (string message in report.SevereWarnings) {
          this.AddRuntimeWarning(message);
          warnings.Add(message);
        }

        foreach (string message in report.Errors) {
          this.AddRuntimeError(message);
          errors.Add(message);
        }
      }

      da.SetData(0, new GsaModelGoo(new GsaModel(newModel)));
      da.SetDataList(1, errors);
      da.SetDataList(2, warnings);
      da.SetDataList(3, notes);
      da.SetDataList(4, logs);
    }

    internal static List<int> GetDependentAnalysisTasksFromDesignTask(Model model, int taskId) {
      int combinationCaseId = model.SteelDesignTasks()[taskId].CombinationCaseId;
      string comboDefinitions = model.CombinationCases()[combinationCaseId].Definition;
      var caseIds = new List<int>();
      var split = comboDefinitions.Split('A').ToList();
      split.RemoveAt(0);
      foreach (string caseId in split) {
        // 1.35A4 + 1.5A67 - might need something extra to catch edge cases
        caseIds.Add(int.Parse(new string(caseId.TakeWhile(char.IsDigit).ToArray())));
      }

      var taskIds = new List<int>();
      foreach (KeyValuePair<int, AnalysisTask> kvp in model.AnalysisTasks()) {
        foreach (int caseId in caseIds) {
          if (kvp.Value.Cases.Contains(caseId)) {
            taskIds.Add(kvp.Key);
            break;
          }
        }
      }

      return taskIds;
    }

    internal static List<int> GetMemberSectionIds(Model model, string memberList) {
      ReadOnlyDictionary<int, Member> members = model.Members(memberList);
      var memSectIds = new List<int>();
      foreach (Member member in members.Values) {
        memSectIds.Add(member.Property);
      }

      return memSectIds;
    }
  }
}
