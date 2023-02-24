﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 2D Member
  /// </summary>
  public class CreateMember2d : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("01450bfc-7ac1-4c51-97a2-42d81d6476b6");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateMem2d;

    public CreateMember2d() : base("Create 2D Member",
      "Mem2D",
      "Create GSA Member 2D",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddBrepParameter("Brep", "B", "Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points))", GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points", "(P)", "Inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves", "(C)", "Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Target mesh size", GH_ParamAccess.item);

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember2dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Brep ghbrep = new GH_Brep();
      if (DA.GetData(0, ref ghbrep))
      {
        if (ghbrep == null) { this.AddRuntimeWarning("Brep input is null"); }
        Brep brep = new Brep();
        if (GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
        {
          // 1 Points
          List<Point3d> pts = new List<Point3d>();
          List<GH_Point> ghpts = new List<GH_Point>();
          if (DA.GetDataList(1, ghpts))
          {
            for (int i = 0; i < ghpts.Count; i++)
            {
              Point3d pt = new Point3d();
              if (GH_Convert.ToPoint3d(ghpts[i], ref pt, GH_Conversion.Both))
                pts.Add(pt);
            }
          }

          // 2 Curves
          List<Curve> crvs = new List<Curve>();
          List<GH_Curve> ghcrvs = new List<GH_Curve>();
          if (DA.GetDataList(2, ghcrvs))
          {
            for (int i = 0; i < ghcrvs.Count; i++)
            {
              Curve crv = null;
              if (GH_Convert.ToCurve(ghcrvs[i], ref crv, GH_Conversion.Both))
                crvs.Add(crv);
            }
          }

          // build new member with brep, crv and pts
          GsaMember2d mem = new GsaMember2d();
          try
          {
            mem = new GsaMember2d(brep, crvs, pts);
          }
          catch (Exception e)
          {
            this.AddRuntimeWarning(e.Message);
          }

          // 3 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaProp2d prop2d = new GsaProp2d();
          if (DA.GetData(3, ref gh_typ))
          {
            if (gh_typ.Value is GsaProp2dGoo)
            {
              gh_typ.CastTo(ref prop2d);
              mem.Property = prop2d;
            }
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int id, GH_Conversion.Both))
                mem.Property = new GsaProp2d(id);
              else
              {
                this.AddRuntimeError("Unable to convert PA input to a 2D Property of reference integer");
              }
            }
          }

          // 4 mesh size
          double meshSize = 0;
          if (DA.GetData(4, ref meshSize))
          {
            mem.MeshSize = meshSize;
          }

          DA.SetData(0, new GsaMember2dGoo(mem));
        }
      }
    }
  }
}

