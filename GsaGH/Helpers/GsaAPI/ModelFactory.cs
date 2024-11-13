using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

    public static void BuildAnalysisTask(GsaModel model, List<GsaAnalysisTask> analysisTasks) {
      if (analysisTasks != null) {
        ReadOnlyDictionary<int, AnalysisTask> existingTasks = model.ApiModel.AnalysisTasks();
        foreach (GsaAnalysisTask task in analysisTasks) {
          bool isNewTask = !existingTasks.Keys.Contains(task.Id);
          if (isNewTask) {
            task.Id = model.ApiModel.AddAnalysisTask(task.ApiTask);
          }

          if (task.Cases.Count == 0) {
            task.CreateDefaultCases(model);
          }

          if (task.Cases.Count == 0) {
            //still no case indicate no load present in gsa model
            continue;
          }

          foreach (GsaAnalysisCase analysisCase in task.Cases) {
            string caseDefinition = model.ApiModel.AnalysisCaseDescription(analysisCase.Id);
            if (!string.IsNullOrEmpty(analysisCase.Definition)) {
              //create new case and assign to task
              model.ApiModel.AddAnalysisCaseToTask(task.Id, analysisCase.Name, analysisCase.Definition);
            } else {
              model.ApiModel.SetAnalysisCaseToTask(task.Id, analysisCase.Id, analysisCase.Name, analysisCase.Definition);
            }
          }
        }
      }
    }
  }
}
