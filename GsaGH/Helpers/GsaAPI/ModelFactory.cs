using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GsaAPI;

using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;

namespace GsaGH.Helpers.GsaApi {
  internal static class ModelFactory {
    internal static Model CreateModelFromCodes(string concreteDesignCode = "", string steelDesignCode = "") {
      if (concreteDesignCode == string.Empty) {
        concreteDesignCode = DesignCode.GetConcreteDesignCodeNames()[8];
      }

      if (steelDesignCode == string.Empty) {
        steelDesignCode = DesignCode.GetSteelDesignCodeNames()[8];
      }

      return TryUpgradeCode(concreteDesignCode, steelDesignCode);
    }

    internal static void SetUserDefaultUnits(Model model) {
      UiUnits uiUnits = model.UiUnits();
      uiUnits.Acceleration = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.AccelerationUnit);
      uiUnits.Angle = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.AngleUnit);
      uiUnits.Energy = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.EnergyUnit);
      uiUnits.Force = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.ForceUnit);
      uiUnits.LengthLarge = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.LengthUnitGeometry);
      uiUnits.LengthSections = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.LengthUnitSection);
      uiUnits.LengthSmall = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.LengthUnitResult);
      uiUnits.Mass = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.MassUnit);
      uiUnits.Stress = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.StressUnitResult);
      uiUnits.TimeLong = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.TimeLongUnit);
      uiUnits.TimeMedium = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.TimeMediumUnit);
      uiUnits.TimeShort = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.TimeShortUnit);
      uiUnits.Velocity = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.VelocityUnit);
    }

    private static string FindSimilarCode(string code, ReadOnlyCollection<string> codes) {
      var codeList = codes.ToList();
      for (int i = 0; i < code.Length; i++) {
        for (int j = codeList.Count - 1; j >= 0; j--) {
          if (codeList[j][i] != code[i]) {
            if (codeList.Count > 1) {
              codeList.RemoveAt(j);
              if (codeList.Count == 1) {
                return codeList[0];
              }
            }
          }
        }
      }
      return codeList[0];
    }

    private static Model TryUpgradeCode(string ssConcreteCode, string ssSteelCode) {
      string concreteDesignCode = ssConcreteCode;
      string steelDesignCode = ssSteelCode;
      try {
        // will fail for superseeded codes
        return new Model(concreteDesignCode, steelDesignCode);
      } catch (GsaApiException) { //GsaAPI.GsaApiException: 'Concrete design code is not supported.'
        ReadOnlyCollection<string> concreteCodes = DesignCode.GetConcreteDesignCodeNames();
        if (!concreteCodes.Contains(concreteDesignCode)) {
          concreteDesignCode = FindSimilarCode(concreteDesignCode, concreteCodes);
        }

        ReadOnlyCollection<string> steelCodes = DesignCode.GetSteelDesignCodeNames();
        if (!steelCodes.Contains(steelDesignCode)) {
          steelDesignCode = FindSimilarCode(steelDesignCode, steelCodes);
        }

        return new Model(concreteDesignCode, steelDesignCode);
      }
    }

    public static void BuildAnalysisTask(Model model, List<GsaAnalysisTask> analysisTasks, bool createDefaultCase = false) {
      if (analysisTasks != null) {
        ReadOnlyDictionary<int, AnalysisTask> existingTasks = model.AnalysisTasks();
        foreach (GsaAnalysisTask task in analysisTasks) {
          bool isNewTask = !existingTasks.Keys.Contains(task.Id);
          if (isNewTask) {
            task.Id = model.AddAnalysisTask(task.ApiTask);
          }

          if (task.Cases.Count == 0 && createDefaultCase) {
            task.CreateDefaultCases(new GsaModel(model));
          } else {
            //clear anaysis case list from task
            foreach (GsaAnalysisCase analysisCase in task.Cases) {
              model.DeleteAnalysisCaseFromTask(task.Id, analysisCase.Id);
            }
          }
          //then create case and assign to task
          foreach (GsaAnalysisCase analysisCase in task.Cases) {
            if (AnalysisCaseNeedToBeAppended(model, task.Id, analysisCase.Id)) {
              //when two different analysis task is referring same analysis case
              model.AddAnalysisCaseToTask(task.Id, analysisCase.Name, analysisCase.Definition);
            } else {
              model.SetAnalysisCaseToTask(task.Id, analysisCase.Id, analysisCase.Name, analysisCase.Definition);
            }
          }
        }
      }
    }

    private static bool AnalysisCaseNeedToBeAppended(Model model, int taskId, int analysisCaseId) {
      foreach (KeyValuePair<int, AnalysisTask> existingTask in model.AnalysisTasks()) {
        if (taskId == existingTask.Key) {
          continue;
        }
        foreach (int existingCase in existingTask.Value.Cases) {
          //i.e. same anlysis case is existing in another task
          if (existingCase == analysisCaseId) {
            {
              return true;
            }
          }
        }
      }
      return false;
    }
  }
}
