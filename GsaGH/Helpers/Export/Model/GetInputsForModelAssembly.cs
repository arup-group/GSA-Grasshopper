using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class GetInputsForModelAssembly {
    internal static List<GsaModel> GetModels(GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inModels = new List<GsaModel>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) { owner.AddRuntimeWarning("Model input (index: " + i + ") is null and has been ignored"); continue; }
          if (ghTyp.Value is GsaModelGoo) {
            var inModel = new GsaModel();
            ghTyp.CastTo(ref inModel);
            inModels.Add(inModel);
          }
          else {
            string type = ghTyp.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            owner.AddRuntimeError("Unable to convert GSA input parameter of type " +
                type + " to GsaModel");
            return null;
          }
        }
        return inModels;
      }
      else if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }

    internal static Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>> GetProperties(GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inSect = new List<GsaSection>();
        var inProp2d = new List<GsaProp2d>();
        var inProp3d = new List<GsaProp3d>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            owner.AddRuntimeWarning("Property input (index: " + i + ") is null and has been ignored");
            continue;
          }

          switch (ghTyp.Value) {
            case GsaSectionGoo _: {
                var gsasection = new GsaSection();
                ghTyp.CastTo(ref gsasection);
                inSect.Add(gsasection.Duplicate());
                break;
              }
            case GsaProp2dGoo _: {
                var gsaprop = new GsaProp2d();
                ghTyp.CastTo(ref gsaprop);
                inProp2d.Add(gsaprop.Duplicate());
                break;
              }
            case GsaProp3dGoo _: {
                var gsaprop = new GsaProp3d();
                ghTyp.CastTo(ref gsaprop);
                inProp3d.Add(gsaprop.Duplicate());
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                owner.AddRuntimeError("Unable to convert Prop input parameter of type " +
                                      type + " to GsaSection or GsaProp2d");
                return null;
              }
          }
        }
        if (!(inSect.Count > 0))
          inSect = null;
        if (!(inProp2d.Count > 0))
          inProp2d = null;
        if (!(inProp3d.Count > 0))
          inProp3d = null;
        return new Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>>(inSect, inProp2d, inProp3d);
      }

      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>>(null, null, null);
    }

    internal static Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> GetGeometry(GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
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
          var ghTyp = new GH_ObjectWrapper();
          ghTyp = ghTypes[i];
          if (ghTyp == null) { owner.AddRuntimeWarning("Geometry input (index: " + i + ") is null and has been ignored"); continue; }

          switch (ghTyp.Value) {
            case GsaNodeGoo _: {
                var gsanode = new GsaNode();
                ghTyp.CastTo(ref gsanode);
                inNodes.Add(gsanode.Duplicate());
                break;
              }
            case GsaElement1dGoo _: {
                var gsaelem1 = new GsaElement1d();
                ghTyp.CastTo(ref gsaelem1);
                inElem1ds.Add(gsaelem1.Duplicate());
                break;
              }
            case GsaElement2dGoo _: {
                var gsaelem2 = new GsaElement2d();
                ghTyp.CastTo(ref gsaelem2);
                inElem2ds.Add(gsaelem2.Duplicate());
                break;
              }
            case GsaElement3dGoo _: {
                var gsaelem3 = new GsaElement3d();
                ghTyp.CastTo(ref gsaelem3);
                inElem3ds.Add(gsaelem3.Duplicate());
                break;
              }
            case GsaMember1dGoo _: {
                var gsamem1 = new GsaMember1d();
                ghTyp.CastTo(ref gsamem1);
                inMem1ds.Add(gsamem1.Duplicate());
                break;
              }
            case GsaMember2dGoo _: {
                var gsamem2 = new GsaMember2d();
                ghTyp.CastTo(ref gsamem2);
                inMem2ds.Add(gsamem2.Duplicate());
                break;
              }
            case GsaMember3dGoo _: {
                var gsamem3 = new GsaMember3d();
                ghTyp.CastTo(ref gsamem3);
                inMem3ds.Add(gsamem3.Duplicate());
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                owner.AddRuntimeError("Unable to convert Geometry input parameter of type " +
                                      type + Environment.NewLine + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
                return null;
              }
          }
        }
        if (!(inNodes.Count > 0))
          inNodes = null;
        if (!(inElem1ds.Count > 0))
          inElem1ds = null;
        if (!(inElem2ds.Count > 0))
          inElem2ds = null;
        if (!(inElem3ds.Count > 0))
          inElem3ds = null;
        if (!(inMem1ds.Count > 0))
          inMem1ds = null;
        if (!(inMem2ds.Count > 0))
          inMem2ds = null;
        if (!(inMem3ds.Count > 0))
          inMem3ds = null;
        return new Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(inNodes, inElem1ds, inElem2ds, inElem3ds, inMem1ds, inMem2ds, inMem3ds);
      }
      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(null, null, null, null, null, null, null);
    }

    internal static Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> GetMembers(GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      var inMem1ds = new List<GsaMember1d>();
      var inMem2ds = new List<GsaMember2d>();
      var inMem3ds = new List<GsaMember3d>();
      if (da.GetDataList(inputid, ghTypes)) {
        for (int i = 0; i < ghTypes.Count; i++) {
          var ghTyp = new GH_ObjectWrapper();
          ghTyp = ghTypes[i];
          if (ghTyp == null) { owner.AddRuntimeWarning("Geometry input (index: " + i + ") is null and has been ignored"); continue; }

          switch (ghTyp.Value) {
            case GsaMember1dGoo _: {
                var gsamem1 = new GsaMember1d();
                ghTyp.CastTo(ref gsamem1);
                inMem1ds.Add(gsamem1.Duplicate());
                break;
              }
            case GsaMember2dGoo _: {
                var gsamem2 = new GsaMember2d();
                ghTyp.CastTo(ref gsamem2);
                inMem2ds.Add(gsamem2.Duplicate());
                break;
              }
            case GsaMember3dGoo _: {
                var gsamem3 = new GsaMember3d();
                ghTyp.CastTo(ref gsamem3);
                inMem3ds.Add(gsamem3.Duplicate());
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                owner.AddRuntimeError("Unable to convert Geometry input parameter of type " +
                                      type + Environment.NewLine + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
                return null;
              }
          }
        }
        if (!(inMem1ds.Count > 0))
          inMem1ds = null;
        if (!(inMem2ds.Count > 0))
          inMem2ds = null;
        if (!(inMem3ds.Count > 0))
          inMem3ds = null;
        return new Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(inMem1ds, inMem2ds, inMem3ds);
      }
      else if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(null, null, null);
    }

    internal static Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>> GetLoading(GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inLoads = new List<GsaLoad>();
        var inGps = new List<GsaGridPlaneSurface>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) { owner.AddRuntimeWarning("Load input (index: " + i + ") is null and has been ignored"); continue; }

          switch (ghTyp.Value) {
            case GsaLoadGoo _: {
                GsaLoad gsaload = null;
                ghTyp.CastTo(ref gsaload);
                inLoads.Add(gsaload.Duplicate());
                break;
              }
            case GsaGridPlaneSurfaceGoo _: {
                var gsaGps = new GsaGridPlaneSurface();
                ghTyp.CastTo(ref gsaGps);
                inGps.Add(gsaGps.Duplicate());
                break;
              }
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                owner.AddRuntimeError("Unable to convert Load input parameter of type " +
                                      type + " to Load or GridPlaneSurface");
                return null;
              }
          }
        }
        if (!(inLoads.Count > 0))
          inLoads = null;
        if (!(inGps.Count > 0))
          inGps = null;
        return new Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>>(inLoads, inGps);
      }
      else if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>>(null, null);
    }

    internal static Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>> GetAnalysis(GH_Component owner, IGH_DataAccess da, int inputid, bool isOptional = false) {
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(inputid, ghTypes)) {
        var inTasks = new List<GsaAnalysisTask>();
        var inComb = new List<GsaCombinationCase>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) { owner.AddRuntimeWarning("Analysis input (index: " + i + ") is null and has been ignored"); continue; }

          switch (ghTyp.Value) {
            case GsaAnalysisTaskGoo goo:
              inTasks.Add(goo.Value.Duplicate());
              break;
            case GsaCombinationCaseGoo caseGoo:
              inComb.Add(caseGoo.Value.Duplicate());
              break;
            default: {
                string type = ghTyp.Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                owner.AddRuntimeError("Unable to convert Analysis input parameter of type " +
                                      type + " to Analysis Task or Combination Case");
                return null;
              }
          }
        }
        if (!(inTasks.Count > 0))
          inTasks = null;
        if (!(inComb.Count > 0))
          inComb = null;
        return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>>(inTasks, inComb);
      }
      if (!isOptional) {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>>(null, null);
    }
  }
}
