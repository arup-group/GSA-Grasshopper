using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using OasysUnits;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class Assemble {
    internal static Model AssembleForLocalAxis(GsaMember1d member) {
      var assembledModel = new ModelAssembly(null, LengthUnit.Meter);
      var mem1ds = new List<GsaMember1d>() {
        member
      };
      assembledModel.ConvertMembers(mem1ds, null, null);
      assembledModel.AssembleNodesElementsMembersAndLists();
      return assembledModel.Model;
    }

    internal static Model AssembleForLocalAxis(GsaElement1d element) {
      var assembledModel = new ModelAssembly(null, LengthUnit.Meter);
      var elem1ds = new List<GsaElement1d>() {
        element
      };
      assembledModel.ConvertElements(elem1ds, null, null);
      assembledModel.AssembleNodesElementsMembersAndLists();
      return assembledModel.Model;
    }

    internal static Model AssembleForPreview(GsaModel model, List<GsaList> lists, 
      List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds, List<GsaMember1d> mem1ds, 
      List<GsaMember2d> mem2ds, LengthUnit modelUnit) {
      var assembledModel = new ModelAssembly(model, modelUnit);
      assembledModel.ConvertElements(elem1ds, elem2ds, null);
      assembledModel.ConvertMembers(mem1ds, mem2ds, null);
      assembledModel.ConvertLists(lists);
      assembledModel.AssembleNodesElementsMembersAndLists();
      return assembledModel.Model;
    }

    internal static Model AssembleModel(
      GsaModel model, List<GsaList> lists, List<GsaGridLine> gridLines, List<GsaNode> nodes, 
      List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds, List<GsaElement3d> elem3ds, 
      List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds, List<GsaMember3d> mem3ds, 
      List<GsaSection> sections, List<GsaProp2d> prop2Ds, List<GsaProp3d> prop3Ds, 
      List<IGsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces, 
      List<GsaLoadCase> loadCases, List<GsaAnalysisTask> analysisTasks, 
      List<GsaCombinationCase> combinations, LengthUnit modelUnit, 
      Length toleranceCoincidentNodes, bool createElementsFromMembers,
      GH_Component owner) {
      var assembledModel = new ModelAssembly(model, modelUnit);
      assembledModel.ConvertNodes(nodes);
      assembledModel.ConvertProperties(sections, prop2Ds, prop3Ds);
      assembledModel.ConvertElements(elem1ds, elem2ds, elem3ds);
      assembledModel.ConvertMembers(mem1ds, mem2ds, mem3ds);
      assembledModel.ConvertNodeList(lists);
      assembledModel.ConvertNodeLoads(loads);
      assembledModel.AssembleNodesElementsMembersAndLists();

      assembledModel.ElementsFromMembers(
        createElementsFromMembers, toleranceCoincidentNodes, owner);

      Loads.ConvertList(lists, loads, ref assembledModel, owner);
      GridPlaneSurfaces.ConvertGridPlaneSurface(gridPlaneSurfaces, ref assembledModel, owner);
      Loads.ConvertLoad(loads, ref assembledModel, owner);
      assembledModel.ConvertLoadCases(loadCases, owner);

      assembledModel.AssembleLoadsCasesAxesGridPlaneSurfacesAndLists();
      assembledModel.ConvertAndAssembleGridLines(gridLines);
      assembledModel.ConvertAndAssembleAnalysisTasks(analysisTasks);
      assembledModel.ConvertAndAssembleCombinations(combinations);

      assembledModel.DeleteExistingResults();
      return assembledModel.Model;
    }
  }
}
