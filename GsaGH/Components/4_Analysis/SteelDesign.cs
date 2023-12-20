using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.Assembly;
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
      pManager.AddTextParameter("Logs", "Log", "Errors, Warnings, Notes, Remarks and Analysis Task logs", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Member IDs", "MID", "IDs of the changed Members", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Pool IDs", "PID", "Pool IDs of the Members' section", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Initial Section IDs", "IPB", "Initial Section Property ID per member", GH_ParamAccess.list);
      pManager.AddNumberParameter("Initial Utilisation", "Iη", "Initial utilisations per member", GH_ParamAccess.list);
      pManager.AddIntegerParameter("New Section IDs", "NPB", "New Section Property ID per member", GH_ParamAccess.list);
      pManager.AddNumberParameter("Utilisation", "η", "Utilisations per member after optimisation", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      Model newModel;
      da.GetData(0, ref modelGoo);
      int taskId = 0;
      da.GetData(1, ref taskId);
      if (taskId == 0) {
        taskId = modelGoo.Value.Model.SteelDesignTasks().Keys.Min();
      }

      int iterations = 0;
      da.GetData(2, ref iterations);
      bool check = false;
      da.GetData(3, ref check);

      var logs = new List<string>();
      var notes = new List<string>();
      var remarks = new List<string>();
      var warnings = new List<string>();
      var errors = new List<string>();
      TaskReport report = null;
      var memberIds = new List<int>();
      var poolIds = new List<int>();
      var initSectionIds = new List<int>();
      var initUtils = new List<double>();
      var newSectionIds = new List<int>();
      var utils = new List<double>();
      
      if (check) {
        newModel = modelGoo.Value.Model;
        newModel.Check(taskId, out report);
        logs.Add(report.Log);
        notes.AddRange(report.Notes);
        remarks.AddRange(report.Warnings);
        foreach (string message in report.SevereWarnings) {
          this.AddRuntimeWarning($"Task {taskId}: {message}");
          warnings.Add($"Task {taskId}: {message}");
        }

        foreach (string message in report.Errors) {
          this.AddRuntimeError($"Task {taskId}: {message}");
          errors.Add($"Task {taskId}: {message}");
        }
      } else {
        newModel = modelGoo.Value.Model.Clone();
        List<int> analysisTaskIds = GetAnalysisTasksFromDesignTask(newModel, taskId);
        newModel.Design(taskId, out report);
        (memberIds, poolIds, initSectionIds, initUtils, newSectionIds, utils) =
          GetOptimisationInfo(report.Log);
        List<int> iterationsSectionIds = initSectionIds;
        List<double> iterationsUtilisation = initUtils;
        for (int i = 0; i < iterations; i++) {
          newModel.CreateElementsFromMembers();
          foreach(int analysisTaskId in analysisTaskIds) {
            newModel.Analyse(analysisTaskId, out report);
          }
          
          newModel.Design(taskId, out report);
          (memberIds, poolIds, iterationsSectionIds, iterationsUtilisation, newSectionIds, utils) =
            GetOptimisationInfo(report.Log);
          if (iterationsSectionIds.SequenceEqual(newSectionIds) 
            && iterationsUtilisation.SequenceEqual(utils)) {
            if (i == 0) {
              this.AddRuntimeRemark($"Optimisation converged first iteration");
            } else {
              this.AddRuntimeRemark($"Optimisation converged in {i + 1} iterations");
            }
            break;
          }
        }

        logs.Add(report.Log);
        notes.AddRange(report.Notes);
        remarks.AddRange(report.Warnings);
        foreach (string message in report.SevereWarnings) {
          this.AddRuntimeWarning($"Task {taskId}: {message}");
          warnings.Add($"Task {taskId}: {message}");
        }

        foreach (string message in report.Errors) {
          this.AddRuntimeError($"Task {taskId}: {message}");
          errors.Add($"Task {taskId}: {message}");
        }
      }

      var notesAndRemarks = new List<string>();
      foreach (string message in remarks) {
        if (!RuntimeMessages(GH_RuntimeMessageLevel.Remark).Contains(message)) {
          this.AddRuntimeRemark(message);
          notesAndRemarks.Add($"Warning: {message}");
        }
      }

      foreach (string message in notes) {
        if (!notesAndRemarks.Contains($"Note: {message}")) {
          notesAndRemarks.Add($"Note: {message}");
        }
      }

      var reportList = new List<string>();
      reportList.AddRange(errors);
      reportList.AddRange(warnings);
      reportList.AddRange(notesAndRemarks);
      reportList.AddRange(logs);
      da.SetData(0, new GsaModelGoo(new GsaModel(newModel)));
      da.SetDataList(1, reportList);
      da.SetDataList(2, memberIds);
      da.SetDataList(3, poolIds);
      da.SetDataList(4, initSectionIds);
      da.SetDataList(5, initUtils);
      da.SetDataList(6, newSectionIds);
      da.SetDataList(7, utils);
    }

    internal List<int> GetAnalysisTasksFromDesignTask(Model model, int taskId) {
      int combinationCaseId = model.SteelDesignTasks()[taskId].CombinationCaseId;
      string comboDefinitions = model.CombinationCases()[combinationCaseId].Definition;
      var caseIds = new List<int>();
      var split = comboDefinitions.Split('A').ToList();
      split.RemoveAt(0);
      foreach (string caseId in split) {
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

    internal (List<int> MemberIds, List<int> PoolIds, List<int> InitSectionIds,
      List<double> InitUtils, List<int> NewSectionIds, List<double> Utils) GetOptimisationInfo(string log) {
      var memberIds = new List<int>();
      var poolIds = new List<int>();
      var initSectionIds = new List<int>();
      var initUtils = new List<double>();
      var newSectionIds = new List<int>();
      var utils = new List<double>();
      string[] split = log.Split(new string[] {
        "\tMember\tPool\tInitial section\tInitial util\tNew section\tUtil\t\n"
      }, StringSplitOptions.None);
      string[] values = split[1].Split(new string[] {
        "Member section references may have changed"
      }, StringSplitOptions.None);
      values = values[0].Split('\n');
      foreach (string row in values) {
        if (string.IsNullOrEmpty(row)) {
          continue;
        }

        var rowValues = row.Replace("\t\t", "\t").Split('\t').ToList();
        memberIds.Add(int.Parse(rowValues[1]));
        poolIds.Add(int.Parse(rowValues[2]));
        initSectionIds.Add(int.Parse(rowValues[3]));
        initUtils.Add(double.Parse(rowValues[4]));
        newSectionIds.Add(int.Parse(rowValues[5]));
        utils.Add(double.Parse(rowValues[6]));
      }
      return (memberIds, poolIds, initSectionIds, initUtils, newSectionIds, utils);
    }
  }
}
