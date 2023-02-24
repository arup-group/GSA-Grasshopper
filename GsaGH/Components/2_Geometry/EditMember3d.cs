﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit a 3D Member
    /// </summary>
    public class EditMember3d : GH_OasysComponent, IGH_PreviewObject, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e7d66219-2243-4108-9d6e-4a84dbf07d55");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditMem3d;

    public EditMember3d() : base("Edit 3D Member",
      "Mem3dEdit",
      "Modify GSA 3D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember3dParameter(), GsaMember3dGoo.Name, GsaMember3dGoo.NickName, GsaMember3dGoo.Description + " to get or set information for. Leave blank to create a new " + GsaMember3dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member3d Number", "ID", "Set Member Number. If ID is set it will replace any existing 3d Member in the model", GH_ParamAccess.item);
      pManager.AddGeometryParameter("Solid", "S", "Reposition Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp3dParameter(), "3D Property", "PV", "Set new 3D Property.", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Set Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item);
      pManager.AddTextParameter("Member3d Name", "Na", "Set Name of Member3d", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member3d Group", "Gr", "Set Member 3d Group", GH_ParamAccess.item);
      pManager.AddColourParameter("Member3d Colour", "Co", "Set Member 3d Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember3dParameter(), GsaMember3dGoo.Name, GsaMember3dGoo.NickName, GsaMember3dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddMeshParameter("Solid Mesh", "M", "Member Solid Mesh", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaProp3dParameter(), "3D Property", "PV", "Get 3D Property", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Get Target mesh size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get if Member is Dummy", GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp", "Get the Member's original topology list referencing node IDs in Model that Model was created from", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMember3d gsaMember3d = new GsaMember3d();
      GsaMember3d mem = new GsaMember3d();
      if (DA.GetData(0, ref gsaMember3d))
      {
        if (gsaMember3d == null) { this.AddRuntimeWarning("Member3D input is null"); }
        mem = gsaMember3d.Duplicate(true);
      }

      if (mem != null)
      {
        // #### inputs ####
        // 1 ID
        GH_Integer ghID = new GH_Integer();
        if (DA.GetData(1, ref ghID))
        {
          if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
            mem.Id = id;
        }

        // 2 geometry
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(2, ref gh_typ))
        {
          Brep brep = new Brep();
          Mesh mesh = new Mesh();
          if (GH_Convert.ToBrep(gh_typ.Value, ref brep, GH_Conversion.Both))
            mem = mem.UpdateGeometry(brep);
          else if (GH_Convert.ToMesh(gh_typ.Value, ref mesh, GH_Conversion.Both))
            mem = mem.UpdateGeometry(mesh);
          else
          {
            this.AddRuntimeError("Unable to convert Geometry input to a 3D Member");
            return;
          }
        }

        // 3 prop3d
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(3, ref gh_typ))
        {
          GsaProp3d prop3d = new GsaProp3d();
          if (gh_typ.Value is GsaProp3dGoo)
            gh_typ.CastTo(ref prop3d);
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int id, GH_Conversion.Both))
              prop3d = new GsaProp3d(id);
            else
            {
              this.AddRuntimeError("Unable to convert PA input to a 3D Property of reference integer");
              return;
            }
          }
          mem.Prop3d = prop3d;
        }

        // 4 mesh size
        double meshSize = 0;
        if (DA.GetData(4, ref meshSize))
        {
          mem.MeshSize = meshSize;
        }

        // 5 mesh with others
        GH_Boolean ghbool = new GH_Boolean();
        if (DA.GetData(5, ref ghbool))
        {
          if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
          {
            if (mem.MeshWithOthers != mbool)
              mem.MeshWithOthers = mbool;
          }
        }

        // 6 name
        GH_String ghnm = new GH_String();
        if (DA.GetData(6, ref ghnm))
        {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            mem.Name = name;
        }

        // 7 Group
        GH_Integer ghgrp = new GH_Integer();
        if (DA.GetData(7, ref ghgrp))
        {
          if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
            mem.Group = grp;
        }

        // 8 Colour
        GH_Colour ghcol = new GH_Colour();
        if (DA.GetData(8, ref ghcol))
        {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            mem.Colour = col;
        }

        // 9 Dummy
        GH_Boolean ghdum = new GH_Boolean();
        if (DA.GetData(9, ref ghdum))
        {
          if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
            mem.IsDummy = dum;
        }

        // #### outputs ####
        DA.SetData(0, new GsaMember3dGoo(mem));
        DA.SetData(1, mem.Id);
        DA.SetData(2, mem.SolidMesh);
        DA.SetData(3, new GsaProp3dGoo(mem.Prop3d));
        DA.SetData(4, mem.MeshSize);
        DA.SetData(5, mem.MeshWithOthers);
        DA.SetData(6, mem.Name);
        DA.SetData(7, mem.Group);
        DA.SetData(8, mem.Colour);
        DA.SetData(9, mem.IsDummy);
        DA.SetData(10, mem.ApiMember.Topology.ToString());
      }
    }

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;

    public void VariableParameterMaintenance()
    {
      return;
    }
    #endregion
  }
}

