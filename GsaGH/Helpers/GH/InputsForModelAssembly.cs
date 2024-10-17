using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Import;
using GsaGH.Parameters;

namespace GsaGH.Helpers.GH {
  internal class InputsForModelAssembly {
    internal static GsaAnalysis GetAnalysis(GH_Component owner, IGH_DataAccess da, int inputid,
      bool isOptional = false) {
      var analysis = new GsaAnalysis();
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Analysis input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaAnalysisTaskGoo goo:
              analysis.AnalysisTasks.Add(goo.Value);
              break;

            case GsaCombinationCaseGoo caseGoo:
              analysis.CombinationCases.Add(caseGoo.Value);
              break;

            case GsaDesignTaskGoo designTaskGoo:
              analysis.DesignTasks.Add(designTaskGoo.Value);
              break;

            default:
              string type = ghTyp.Value.GetType().ToString();
              type = type.Replace("GsaGH.Parameters.Gsa", string.Empty);
              type = type.Replace("GsaGH.Parameters.", string.Empty);
              type = type.Replace("Goo", string.Empty);
              string analysisCase = string.Empty;
              if (type == "AnalysisCase") {
                analysisCase = "\nAnalysisCase should be added through an Analysis Task and " +
                  "cannot be added directly in a model";
              }
              owner.AddRuntimeError("Unable to convert Analysis input parameter of type " + type
                + " to Analysis Task, Design Task or Combination Case");
              break;
          }
        }
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return analysis;
    }

    internal static GsaGeometry GetGeometry(GH_Component owner, IGH_DataAccess da, int inputid,
      bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      var geometry = new GsaGeometry();

      if (da.GetDataList(inputid, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Geometry input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaNodeGoo nodeGoo:
              geometry.Nodes.Add(nodeGoo.Value);
              break;

            case GsaElement1dGoo element1dGoo:
              geometry.Element1ds.Add(element1dGoo.Value);
              break;

            case GsaElement2dGoo element2dGoo:
              geometry.Element2ds.Add(element2dGoo.Value);
              break;

            case GsaElement3dGoo element3dGoo:
              geometry.Element3ds.Add(element3dGoo.Value);
              break;

            case GsaMember1dGoo member1dGoo:
              geometry.Member1ds.Add(member1dGoo.Value);
              break;

            case GsaMember2dGoo member2dGoo:
              geometry.Member2ds.Add(member2dGoo.Value);
              break;

            case GsaMember3dGoo member3dGoo:
              geometry.Member3ds.Add(member3dGoo.Value);
              break;

            case GsaAssemblyGoo assemblyGoo:
              geometry.Assemblies.Add(assemblyGoo.Value);
              break;

            default:
              string type = ghTyp.Value.GetType().ToString();
              type = type.Replace("GsaGH.Parameters.", string.Empty);
              type = type.Replace("Goo", string.Empty);
              owner.AddRuntimeError("Unable to convert Geometry input parameter of type " + type
                + Environment.NewLine
                + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D, Member3D or Assembly");
              break;
          }
        }
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return geometry;
    }

    internal static GsaLoading GetLoading(GH_Component owner, IGH_DataAccess da, int inputid,
      bool isOptional = false) {
      var loads = new GsaLoading();
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning("Load input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaLoadGoo loadGoo:
              loads.Loads.Add(loadGoo.Value);
              break;

            case GsaGridPlaneSurfaceGoo gridPlaneSurfaceGoo:
              loads.GridPlaneSurfaces.Add(gridPlaneSurfaceGoo.Value);
              break;

            case GsaLoadCaseGoo loadCaseGoo:
              loads.LoadCases.Add(loadCaseGoo.Value);
              break;

            default:
              string type = ghTyp.Value.GetType().ToString();
              type = type.Replace("GsaGH.Parameters.", string.Empty);
              type = type.Replace("Goo", string.Empty);
              owner.AddRuntimeError("Unable to convert Load input parameter of type " + type
                + " to Load, LoadCase or GridPlaneSurface");
              break;
          }
        }

      } else if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return loads;
    }

    internal static (List<GsaModel> models, List<GsaList> lists, List<GsaGridLine> gridLines) GetModelsAndLists(
      GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inModels = new List<GsaModel>();
        var inLists = new List<GsaList>();
        var inGridLines = new List<GsaGridLine>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning("Model input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaModelGoo modelGoo:
              inModels.Add(modelGoo.Value);
              break;

            case GsaListGoo listGoo:
              inLists.Add(listGoo.Value);
              break;

            case GsaGridLineGoo gridLineGoo:
              inGridLines.Add(gridLineGoo.Value);
              break;

            default:
              string type = ghTyp.Value.GetType().ToString();
              type = type.Replace("GsaGH.Parameters.", string.Empty);
              type = type.Replace("Goo", string.Empty);
              owner.AddRuntimeError("Unable to convert GSA input parameter of type " + type
                + " to GsaModel, GsaList or GridLine");
              break;
          }
        }

        return (inModels, inLists, inGridLines);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return (null, null, null);
    }

    internal static GsaProperties GetProperties(GH_Component owner, IGH_DataAccess da, int inputid,
      bool isOptional = false) {
      var properties = new GsaProperties();
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {

        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Property input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaMaterialGoo materialGoo:
              properties.Materials.Add(materialGoo.Value);
              break;

            case GsaSectionGoo sectionGoo:
              properties.Sections.Add(sectionGoo.Value);
              break;

            case GsaProperty2dGoo prop2dGoo:
              properties.Property2ds.Add(prop2dGoo.Value);
              break;

            case GsaProperty3dGoo prop3dGoo:
              properties.Property3ds.Add(prop3dGoo.Value);
              break;

            case GsaSpringPropertyGoo springPropGoo:
              properties.SpringProperties.Add(springPropGoo.Value);
              break;

            default:
              string type = ghTyp.Value.GetType().ToString();
              type = type.Replace("GsaGH.Parameters.", string.Empty);
              type = type.Replace("Goo", string.Empty);
              owner.AddRuntimeError("Unable to convert Prop input parameter of type " + type
                + " to GsaSection or GsaProp2d");
              break;
          }
        }
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return properties;
    }
  }
}
