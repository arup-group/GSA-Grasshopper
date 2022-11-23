using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit a Node
    /// </summary>
    public class Elem2dFromBrep : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("83948408-c55d-49b9-b9a7-98034bcf3ce1");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElemsFromBreps;

    public Elem2dFromBrep() : base("Element2d from Brep",
      "Elem2dFromBrep",
      "Mesh a non-planar Brep",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddBrepParameter("Brep", "B", "Brep (can be non-planar)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Incl. Points or Nodes", "(P)", "Inclusion points or Nodes", GH_ParamAccess.list);
      pManager.AddGenericParameter("Incl. Curves or 1D Members", "(C)", "Inclusion curves or 1D Members", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Target mesh size", GH_ParamAccess.item, 0);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaElement2dParameter(), "2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Brep ghbrep = new GH_Brep();
      if (DA.GetData(0, ref ghbrep))
      {
        if (ghbrep == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Brep input is null"); }
        Brep brep = new Brep();
        if (GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
        {
          // 1 Points
          List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
          List<Point3d> pts = new List<Point3d>();
          List<GsaNode> nodes = new List<GsaNode>();
          if (DA.GetDataList(1, gh_types))
          {
            for (int i = 0; i < gh_types.Count; i++)
            {
              Point3d pt = new Point3d();
              if (gh_types[i].Value is GsaNodeGoo)
              {
                GsaNode gsanode = new GsaNode();
                gh_types[i].CastTo(ref gsanode);
                nodes.Add(gsanode);
              }
              else if (GH_Convert.ToPoint3d(gh_types[i].Value, ref pt, GH_Conversion.Both))
              {
                pts.Add(pt);
              }
              else
              {
                string type = gh_types[i].Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert incl. Point/Node input parameter of type " +
                    type + " to point or node");
              }
            }
          }

          // 2 Curves
          gh_types = new List<GH_ObjectWrapper>();
          List<Curve> crvs = new List<Curve>();
          List<GsaMember1d> mem1ds = new List<GsaMember1d>();
          if (DA.GetDataList(2, gh_types))
          {
            for (int i = 0; i < gh_types.Count; i++)
            {
              Curve crv = null;
              if (gh_types[i].Value is GsaMember1dGoo)
              {
                GsaMember1d gsamem1d = new GsaMember1d();
                gh_types[i].CastTo(ref gsamem1d);
                mem1ds.Add(gsamem1d);
              }
              else if (GH_Convert.ToCurve(gh_types[i].Value, ref crv, GH_Conversion.Both))
              {
                crvs.Add(crv);
              }
              else
              {
                string type = gh_types[i].Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert incl. Curve/Mem1D input parameter of type " +
                    type + " to curve or 1D Member");
              }
            }
          }

          // 4 mesh size
          double meshSize = 0;
          DA.GetData(4, ref meshSize);

          // build new element2d with brep, crv and pts
          GsaElement2d elem2d = new GsaElement2d(brep, crvs, pts, meshSize, mem1ds, nodes);

          // 3 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaProp2d prop2d = new GsaProp2d();
          if (DA.GetData(3, ref gh_typ))
          {
            if (gh_typ.Value is GsaProp2dGoo)
              gh_typ.CastTo(ref prop2d);
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                prop2d.ID = idd;
              else
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                return;
              }
            }
          }
          else
            prop2d.ID = 1;
          List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
          for (int i = 0; i < elem2d.API_Elements.Count; i++)
            prop2Ds.Add(prop2d);
          elem2d.Properties = prop2Ds;

          DA.SetData(0, new GsaElement2dGoo(elem2d));

          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "This component is work-in-progress and provided 'as-is'. It will unroll the surface, do the meshing, map the mesh back on the original surface. Only single surfaces will work. Surfaces of high curvature and not-unrollable geometries (like a sphere) is unlikely to produce good results");
        }
      }
    }
  }
}
