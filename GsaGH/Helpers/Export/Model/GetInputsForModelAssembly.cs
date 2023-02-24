using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export
{
  internal class GetInputsForModelAssembly
  {
    internal static List<GsaModel> GetModels(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      // Get Model input
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_types))
      {
        List<GsaModel> in_models = new List<GsaModel>();
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null) { owner.AddRuntimeWarning("Model input (index: " + i + ") is null and has been ignored"); continue; }
          if (gh_typ.Value is GsaModelGoo)
          {
            GsaModel in_model = new GsaModel();
            gh_typ.CastTo(ref in_model);
            in_models.Add(in_model);
          }
          else
          {
            string type = gh_typ.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            owner.AddRuntimeError("Unable to convert GSA input parameter of type " +
                type + " to GsaModel");
            return null;
          }
        }
        return in_models;
      }
      else if (!isOptional)
      {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return null;
    }

    internal static Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>> GetProperties(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      // Get Section Property input
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_types))
      {
        List<GsaSection> in_sect = new List<GsaSection>();
        List<GsaProp2d> in_prop2d = new List<GsaProp2d>();
        List<GsaProp3d> in_prop3d = new List<GsaProp3d>();
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null) { owner.AddRuntimeWarning("Property input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaSectionGoo)
          {
            GsaSection gsasection = new GsaSection();
            gh_typ.CastTo(ref gsasection);
            in_sect.Add(gsasection.Duplicate());
          }
          else if (gh_typ.Value is GsaProp2dGoo)
          {
            GsaProp2d gsaprop = new GsaProp2d();
            gh_typ.CastTo(ref gsaprop);
            in_prop2d.Add(gsaprop.Duplicate());
          }
          else if (gh_typ.Value is GsaProp3dGoo)
          {
            GsaProp3d gsaprop = new GsaProp3d();
            gh_typ.CastTo(ref gsaprop);
            in_prop3d.Add(gsaprop.Duplicate());
          }
          else
          {
            string type = gh_typ.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            owner.AddRuntimeError("Unable to convert Prop input parameter of type " +
                type + " to GsaSection or GsaProp2d");
            return null;
          }
        }
        if (!(in_sect.Count > 0))
          in_sect = null;
        if (!(in_prop2d.Count > 0))
          in_prop2d = null;
        if (!(in_prop3d.Count > 0))
          in_prop3d = null;
        return new Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>>(in_sect, in_prop2d, in_prop3d);
      }
      else if (!isOptional)
      {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaSection>, List<GsaProp2d>, List<GsaProp3d>>(null, null, null);
    }

    internal static Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> GetGeometry(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      // Get Geometry input
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      List<GsaNode> in_nodes = new List<GsaNode>();
      List<GsaElement1d> in_elem1ds = new List<GsaElement1d>();
      List<GsaElement2d> in_elem2ds = new List<GsaElement2d>();
      List<GsaElement3d> in_elem3ds = new List<GsaElement3d>();
      List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
      List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
      List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
      if (DA.GetDataList(inputid, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          gh_typ = gh_types[i];
          if (gh_typ == null) { owner.AddRuntimeWarning("Geometry input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaNodeGoo)
          {
            GsaNode gsanode = new GsaNode();
            gh_typ.CastTo(ref gsanode);
            in_nodes.Add(gsanode.Duplicate());
          }
          else if (gh_typ.Value is GsaElement1dGoo)
          {
            GsaElement1d gsaelem1 = new GsaElement1d();
            gh_typ.CastTo(ref gsaelem1);
            in_elem1ds.Add(gsaelem1.Duplicate());
          }
          else if (gh_typ.Value is GsaElement2dGoo)
          {
            GsaElement2d gsaelem2 = new GsaElement2d();
            gh_typ.CastTo(ref gsaelem2);
            in_elem2ds.Add(gsaelem2.Duplicate());
          }
          else if (gh_typ.Value is GsaElement3dGoo)
          {
            GsaElement3d gsaelem3 = new GsaElement3d();
            gh_typ.CastTo(ref gsaelem3);
            in_elem3ds.Add(gsaelem3.Duplicate());
          }
          else if (gh_typ.Value is GsaMember1dGoo)
          {
            GsaMember1d gsamem1 = new GsaMember1d();
            gh_typ.CastTo(ref gsamem1);
            in_mem1ds.Add(gsamem1.Duplicate());
          }
          else if (gh_typ.Value is GsaMember2dGoo)
          {
            GsaMember2d gsamem2 = new GsaMember2d();
            gh_typ.CastTo(ref gsamem2);
            in_mem2ds.Add(gsamem2.Duplicate());
          }
          else if (gh_typ.Value is GsaMember3dGoo)
          {
            GsaMember3d gsamem3 = new GsaMember3d();
            gh_typ.CastTo(ref gsamem3);
            in_mem3ds.Add(gsamem3.Duplicate());
          }
          else
          {
            string type = gh_typ.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            owner.AddRuntimeError("Unable to convert Geometry input parameter of type " +
                type + Environment.NewLine + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
            return null;
          }
        }
        if (!(in_nodes.Count > 0))
          in_nodes = null;
        if (!(in_elem1ds.Count > 0))
          in_elem1ds = null;
        if (!(in_elem2ds.Count > 0))
          in_elem2ds = null;
        if (!(in_elem3ds.Count > 0))
          in_elem3ds = null;
        if (!(in_mem1ds.Count > 0))
          in_mem1ds = null;
        if (!(in_mem2ds.Count > 0))
          in_mem2ds = null;
        if (!(in_mem3ds.Count > 0))
          in_mem3ds = null;
        return new Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(in_nodes, in_elem1ds, in_elem2ds, in_elem3ds, in_mem1ds, in_mem2ds, in_mem3ds);
      }
      else if (!isOptional)
      {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaNode>, List<GsaElement1d>, List<GsaElement2d>, List<GsaElement3d>, List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(null, null, null, null, null, null, null);
    }

    internal static Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>> GetMembers(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      // Get Geometry input
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
      List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
      List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
      if (DA.GetDataList(inputid, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          gh_typ = gh_types[i];
          if (gh_typ == null) { owner.AddRuntimeWarning("Geometry input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaMember1dGoo)
          {
            GsaMember1d gsamem1 = new GsaMember1d();
            gh_typ.CastTo(ref gsamem1);
            in_mem1ds.Add(gsamem1.Duplicate());
          }
          else if (gh_typ.Value is GsaMember2dGoo)
          {
            GsaMember2d gsamem2 = new GsaMember2d();
            gh_typ.CastTo(ref gsamem2);
            in_mem2ds.Add(gsamem2.Duplicate());
          }
          else if (gh_typ.Value is GsaMember3dGoo)
          {
            GsaMember3d gsamem3 = new GsaMember3d();
            gh_typ.CastTo(ref gsamem3);
            in_mem3ds.Add(gsamem3.Duplicate());
          }
          else
          {
            string type = gh_typ.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            owner.AddRuntimeError("Unable to convert Geometry input parameter of type " +
                type + Environment.NewLine + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
            return null;
          }
        }
        if (!(in_mem1ds.Count > 0))
          in_mem1ds = null;
        if (!(in_mem2ds.Count > 0))
          in_mem2ds = null;
        if (!(in_mem3ds.Count > 0))
          in_mem3ds = null;
        return new Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(in_mem1ds, in_mem2ds, in_mem3ds);
      }
      else if (!isOptional)
      {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaMember1d>, List<GsaMember2d>, List<GsaMember3d>>(null, null, null);
    }

    internal static Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>> GetLoading(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      // Get Loads input
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_types))
      {
        List<GsaLoad> in_loads = new List<GsaLoad>();
        List<GsaGridPlaneSurface> in_gps = new List<GsaGridPlaneSurface>();
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null) { owner.AddRuntimeWarning("Load input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaLoadGoo)
          {
            GsaLoad gsaload = null;
            gh_typ.CastTo(ref gsaload);
            in_loads.Add(gsaload.Duplicate());
          }
          else if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
          {
            GsaGridPlaneSurface gsaGPS = new GsaGridPlaneSurface();
            gh_typ.CastTo(ref gsaGPS);
            in_gps.Add(gsaGPS.Duplicate());
          }
          else
          {
            string type = gh_typ.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            owner.AddRuntimeError("Unable to convert Load input parameter of type " +
                type + " to Load or GridPlaneSurface");
            return null;
          }
        }
        if (!(in_loads.Count > 0))
          in_loads = null;
        if (!(in_gps.Count > 0))
          in_gps = null;
        return new Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>>(in_loads, in_gps);
      }
      else if (!isOptional)
      {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaLoad>, List<GsaGridPlaneSurface>>(null, null);
    }

    internal static Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>> GetAnalysis(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      // Get AnalysisTasks input
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(inputid, gh_types))
      {
        List<GsaAnalysisTask> in_tasks = new List<GsaAnalysisTask>();
        List<GsaCombinationCase> in_comb = new List<GsaCombinationCase>();
        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null) { owner.AddRuntimeWarning("Analysis input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaAnalysisTaskGoo)
          {
            in_tasks.Add(((GsaAnalysisTaskGoo)gh_typ.Value).Value.Duplicate());
          }
          else if (gh_typ.Value is GsaCombinationCaseGoo)
          {
            in_comb.Add(((GsaCombinationCaseGoo)gh_typ.Value).Value.Duplicate());
          }
          else
          {
            string type = gh_typ.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            owner.AddRuntimeError("Unable to convert Analysis input parameter of type " +
                type + " to Analysis Task or Combination Case");
            return null;
          }
        }
        if (!(in_tasks.Count > 0))
          in_tasks = null;
        if (!(in_comb.Count > 0))
          in_comb = null;
        return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>>(in_tasks, in_comb);
      }
      else if (!isOptional)
      {
        owner.AddRuntimeWarning("Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }
      return new Tuple<List<GsaAnalysisTask>, List<GsaCombinationCase>>(null, null);
    }
  }
}
