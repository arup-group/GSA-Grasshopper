using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class GetInputsForModelAssembly {

    internal static Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>> GetAnalysis(
      GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inTasks = new List<GsaAnalysisTask>();
        var inComb = new List<GsaCombinationCase>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Analysis input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaAnalysisTaskGoo goo:
              inTasks.Add(goo.Value.Duplicate());
              break;

            case GsaCombinationCaseGoo caseGoo:
              inComb.Add(caseGoo.Value.Duplicate());
              break;

            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", string.Empty);
                type = type.Replace("Goo", string.Empty);
                owner.AddRuntimeError("Unable to convert Analysis input parameter of type " + type
                  + " to Analysis Task or Combination Case");
                return null;
              }
          }
        }

        if (!(inTasks.Count > 0)) {
          inTasks = null;
        }

        if (!(inComb.Count > 0)) {
          inComb = null;
        }

        return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>>(inTasks, inComb);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>>(null, null);
    }

    internal static
      Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>,
        List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> GetGeometry(
        GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      var inNodes = new List<GsaNode>();
      var inElem1ds = new List<GsaElement1d>();
      var inElem2ds = new List<GsaElement2d>();
      var inElem3ds = new List<GsaElement3d>();
      var inMem1ds = new List<GsaMember1d>();
      var inMem2ds = new List<GsaMember2d>();
      var inMem3ds = new List<GsaMember3d>();
      if (da.GetDataList(inputid, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Geometry input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaNodeGoo nodeGoo: {
                inNodes.Add(nodeGoo.Value);
                break;
              }
            case GsaElement1dGoo element1dGoo: {
                inElem1ds.Add(element1dGoo.Value);
                break;
              }
            case GsaElement2dGoo element2dGoo: {
                inElem2ds.Add(element2dGoo.Value);
                break;
              }
            case GsaElement3dGoo element3dGoo: {
                inElem3ds.Add(element3dGoo.Value);
                break;
              }
            case GsaMember1dGoo member1dGoo: {
                inMem1ds.Add(member1dGoo.Value);
                break;
              }
            case GsaMember2dGoo member2dGoo: {
                inMem2ds.Add(member2dGoo.Value);
                break;
              }
            case GsaMember3dGoo member3dGoo: {
                inMem3ds.Add(member3dGoo.Value);
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", string.Empty);
                type = type.Replace("Goo", string.Empty);
                owner.AddRuntimeError("Unable to convert Geometry input parameter of type " + type
                  + Environment.NewLine
                  + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
                return null;
              }
          }
        }

        if (!(inNodes.Count > 0)) {
          inNodes = null;
        }

        if (!(inElem1ds.Count > 0)) {
          inElem1ds = null;
        }

        if (!(inElem2ds.Count > 0)) {
          inElem2ds = null;
        }

        if (!(inElem3ds.Count > 0)) {
          inElem3ds = null;
        }

        if (!(inMem1ds.Count > 0)) {
          inMem1ds = null;
        }

        if (!(inMem2ds.Count > 0)) {
          inMem2ds = null;
        }

        if (!(inMem3ds.Count > 0)) {
          inMem3ds = null;
        }

        return new
          Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>,
            List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(inNodes, inElem1ds, inElem2ds,
            inElem3ds, inMem1ds, inMem2ds, inMem3ds);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return new
        Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>,
          List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(null, null, null, null, null,
          null, null);
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
                  + " to Load or GridPlaneSurface");
                return null;
              }
          }
        }

        if (!(inLoads.Count > 0)) {
          inLoads = null;
        }

        if (!(inGps.Count > 0)) {
          inGps = null;
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

    internal static Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> GetMembers(
      GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      var inMem1ds = new List<GsaMember1d>();
      var inMem2ds = new List<GsaMember2d>();
      var inMem3ds = new List<GsaMember3d>();
      if (da.GetDataList(inputid, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Geometry input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaMember1dGoo member1dGoo: {
                inMem1ds.Add(member1dGoo.Value.Duplicate());
                break;
              }
            case GsaMember2dGoo member2dGoo: {
                inMem2ds.Add(member2dGoo.Value.Duplicate());
                break;
              }
            case GsaMember3dGoo member3dGoo: {
                inMem3ds.Add(member3dGoo.Value.Duplicate());
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", string.Empty);
                type = type.Replace("Goo", string.Empty);
                owner.AddRuntimeError("Unable to convert Geometry input parameter of type " + type
                  + Environment.NewLine
                  + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
                return null;
              }
          }
        }

        if (!(inMem1ds.Count > 0)) {
          inMem1ds = null;
        }

        if (!(inMem2ds.Count > 0)) {
          inMem2ds = null;
        }

        if (!(inMem3ds.Count > 0)) {
          inMem3ds = null;
        }

        return new Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(inMem1ds,
          inMem2ds, inMem3ds);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return new Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(null, null, null);
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
                + " to GsaModel or GsaList");
              return (null, null, null);
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

    internal static Tuple<List<GsaSection>, List<GsaProperty2d>, List<GsaProperty3d>> GetProperties(
      GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inSect = new List<GsaSection>();
        var inProp2d = new List<GsaProperty2d>();
        var inProp3d = new List<GsaProperty3d>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning(
              "Property input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaSectionGoo sectionGoo: {
                inSect.Add(sectionGoo.Value);
                break;
              }
            case GsaProperty2dGoo prop2dGoo: {
                inProp2d.Add(prop2dGoo.Value);
                break;
              }
            case GsaProperty3dGoo prop3dGoo: {
                inProp3d.Add(prop3dGoo.Value);
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", string.Empty);
                type = type.Replace("Goo", string.Empty);
                owner.AddRuntimeError("Unable to convert Prop input parameter of type " + type
                  + " to GsaSection or GsaProp2d");
                return null;
              }
          }
        }

        if (!(inSect.Count > 0)) {
          inSect = null;
        }

        if (!(inProp2d.Count > 0)) {
          inProp2d = null;
        }

        if (!(inProp3d.Count > 0)) {
          inProp3d = null;
        }

        return new Tuple<List<GsaSection>, List<GsaProperty2d>, List<GsaProperty3d>>(inSect, inProp2d,
          inProp3d);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName
          + " failed to collect data!");
      }

      return new Tuple<List<GsaSection>, List<GsaProperty2d>, List<GsaProperty3d>>(null, null, null);
    }
  }
}
