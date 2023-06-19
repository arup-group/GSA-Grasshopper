using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class AssembleModel {
    internal static Model Assemble(
      GsaModel model, List<GsaList> lists, List<GsaNode> nodes, List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds,
      List<GsaElement3d> elem3ds, List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds,
      List<GsaMember3d> mem3ds, List<GsaSection> sections, List<GsaProp2d> prop2Ds,
      List<GsaProp3d> prop3Ds, List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces,
      List<GsaAnalysisTask> analysisTasks, List<GsaCombinationCase> combinations,
      LengthUnit modelUnit, Length toleranceCoincidentNodes, bool createElementsFromMembers,
      GH_Component owner) {
      var assembledModel = new ModelAssembly(model);
      assembledModel.ConvertProperties(sections, prop2Ds, prop3Ds);
      assembledModel.ConvertElements(elem1ds, elem2ds, elem3ds);
      assembledModel.ConvertMembers(mem1ds, mem2ds, mem3ds);
      assembledModel.ConvertNodeList(lists);
      assembledModel.ConvertNodeLoads(loads);
      assembledModel.AssemblePreMeshing();

      if (createElementsFromMembers) {
        assembledModel.ElementsFromMembers(toleranceCoincidentNodes, owner);
      }

      assembledModel.ConvertList(lists, loads, owner);
      assembledModel.ConvertLoads(loads, gridPlaneSurfaces);

      

      // Set Analysis Tasks in model
      if (analysisTasks != null) {
        ReadOnlyDictionary<int, AnalysisTask> existingTasks = gsa.AnalysisTasks();
        foreach (GsaAnalysisTask task in analysisTasks) {
          if (!existingTasks.Keys.Contains(task.Id)) {
            task.Id = gsa.AddAnalysisTask();
          }

          if (task.Cases == null || task.Cases.Count == 0) {
            task.CreateDefaultCases(gsa);
          }

          if (task.Cases == null) {
            continue;
          }

          foreach (GsaAnalysisCase ca in task.Cases) {
            gsa.AddAnalysisCaseToTask(task.Id, ca.Name, ca.Description);
          }
        }
      }

      // Add Combination Cases to model
      if (combinations != null && combinations.Count > 0) {
        foreach (GsaCombinationCase co in combinations) {
          gsa.AddCombinationCase(co.Name, co.Description);
        }
      }

      return gsa;
    }
  }
}
