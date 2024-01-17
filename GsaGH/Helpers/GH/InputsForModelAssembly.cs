﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;

namespace GsaGH.Helpers.GH {
  internal class InputsForModelAssembly {
    internal static Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>, List<IGsaDesignTask>> GetAnalysis(
      GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inAnalysisTasks = new List<GsaAnalysisTask>();
        var inComb = new List<GsaCombinationCase>();
        var inDesignTasks = new List<IGsaDesignTask>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Analysis input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaAnalysisTaskGoo goo:
              inAnalysisTasks.Add(goo.Value);
              break;

            case GsaCombinationCaseGoo caseGoo:
              inComb.Add(caseGoo.Value);
              break;

            case GsaDesignTaskGoo designTaskGoo:
              inDesignTasks.Add(designTaskGoo.Value);
              break;

            default: {
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

        if (!(inAnalysisTasks.Count > 0)) {
          inAnalysisTasks = null;
        }

        if (!(inComb.Count > 0)) {
          inComb = null;
        }

        if (!(inDesignTasks.Count > 0)) {
          inDesignTasks = null;
        }

        return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>, List<IGsaDesignTask>>(
          inAnalysisTasks, inComb, inDesignTasks);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>, List<IGsaDesignTask>>(
        null, null, null);
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

        if (!(geometry.Nodes.Count > 0)) {
          geometry.Nodes = null;
        }

        if (!(geometry.Element1ds.Count > 0)) {
          geometry.Element1ds = null;
        }

        if (!(geometry.Element2ds.Count > 0)) {
          geometry.Element2ds = null;
        }

        if (!(geometry.Element3ds.Count > 0)) {
          geometry.Element3ds = null;
        }

        if (!(geometry.Member1ds.Count > 0)) {
          geometry.Member1ds = null;
        }

        if (!(geometry.Member2ds.Count > 0)) {
          geometry.Member2ds = null;
        }

        if (!(geometry.Member3ds.Count > 0)) {
          geometry.Member3ds = null;
        }
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return geometry;
    }

    internal static Tuple<List<IGsaLoad>, List<GsaGridPlaneSurface>, List<GsaLoadCase>> GetLoading(
      GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inLoads = new List<IGsaLoad>();
        var inGps = new List<GsaGridPlaneSurface>();
        var inCases = new List<GsaLoadCase>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning("Load input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaLoadGoo loadGoo: {
                inLoads.Add(loadGoo.Value);
                break;
              }
            case GsaGridPlaneSurfaceGoo gridPlaneSurfaceGoo: {
                inGps.Add(gridPlaneSurfaceGoo.Value);
                break;
              }
            case GsaLoadCaseGoo loadCaseGoo: {
                inCases.Add(loadCaseGoo.Value);
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", string.Empty);
                type = type.Replace("Goo", string.Empty);
                owner.AddRuntimeError("Unable to convert Load input parameter of type " + type
                  + " to Load, LoadCase or GridPlaneSurface");
                break;
              }
          }
        }

        if (!(inLoads.Count > 0)) {
          inLoads = null;
        }

        if (!(inGps.Count > 0)) {
          inGps = null;
        }

        if (!(inCases.Count > 0)) {
          inCases = null;
        }

        return new Tuple<List<IGsaLoad>, List<GsaGridPlaneSurface>, List<GsaLoadCase>>
          (inLoads, inGps, inCases);
      } else if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return new Tuple<List<IGsaLoad>, List<GsaGridPlaneSurface>, List<GsaLoadCase>>
        (null, null, null);
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

        if (!(inModels.Count > 0)) {
          inModels = null;
        }

        if (!(inLists.Count > 0)) {
          inLists = null;
        }

        if (!(inGridLines.Count > 0)) {
          inGridLines = null;
        }

        return (inModels, inLists, inGridLines);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return (null, null, null);
    }

    internal static Tuple<List<GsaMaterial>, List<GsaSection>, List<GsaProperty2d>,
      List<GsaProperty3d>, List<GsaSpringProperty>> GetProperties(GH_Component owner,
      IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inMat = new List<GsaMaterial>();
        var inSect = new List<GsaSection>();
        var inProp2d = new List<GsaProperty2d>();
        var inProp3d = new List<GsaProperty3d>();
        var inSpringProps = new List<GsaSpringProperty>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Property input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaMaterialGoo materialGoo:
              inMat.Add(materialGoo.Value);
              break;

            case GsaSectionGoo sectionGoo:
              inSect.Add(sectionGoo.Value);
              break;

            case GsaProperty2dGoo prop2dGoo:
              inProp2d.Add(prop2dGoo.Value);
              break;

            case GsaProperty3dGoo prop3dGoo:
              inProp3d.Add(prop3dGoo.Value);
              break;

            case GsaSpringPropertyGoo springPropGoo:
              inSpringProps.Add(springPropGoo.Value);
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

        if (inSect.IsNullOrEmpty()) {
          inSect = null;
        }

        if (inProp2d.IsNullOrEmpty()) {
          inProp2d = null;
        }

        if (inProp3d.IsNullOrEmpty()) {
          inProp3d = null;
        }

        if (inSpringProps.IsNullOrEmpty()) {
          inSpringProps = null;
        }

        return new Tuple<List<GsaMaterial>, List<GsaSection>, List<GsaProperty2d>, List<GsaProperty3d>,
          List<GsaSpringProperty>>(inMat, inSect, inProp2d, inProp3d, inSpringProps);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return new Tuple<List<GsaMaterial>, List<GsaSection>, List<GsaProperty2d>, List<GsaProperty3d>,
        List<GsaSpringProperty>>(null, null, null, null, null);
    }
  }
}
