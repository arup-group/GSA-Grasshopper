using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
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
      var assembledModel = new ModelAssembly(model, modelUnit);
      assembledModel.ConvertNodes(nodes);
      assembledModel.ConvertProperties(sections, prop2Ds, prop3Ds);
      assembledModel.ConvertElements(elem1ds, elem2ds, elem3ds);
      assembledModel.ConvertMembers(mem1ds, mem2ds, mem3ds);
      assembledModel.ConvertNodeList(lists);
      assembledModel.ConvertNodeLoads(loads);
      assembledModel.AssemblePreMeshing();

      if (createElementsFromMembers) {
        assembledModel.ElementsFromMembers(toleranceCoincidentNodes, owner);
      }

      Loads.ConvertList(lists, loads, ref assembledModel, owner);
      GridPlaneSurfaces.ConvertGridPlaneSurface(gridPlaneSurfaces, ref assembledModel, owner);
      Loads.ConvertLoad(loads, ref assembledModel, owner);

      assembledModel.AssemblePostMeshing();
      assembledModel.ConvertAnalysisTasks(analysisTasks);
      assembledModel.ConvertCombinations(combinations);

      assembledModel.DeleteExistingResults();
      return assembledModel.Model;
    }
  }
}
