﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 3D Member
  /// </summary>
  public class EditMember3d : GH_OasysComponent,
    IGH_VariableParameterComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaMember3d = new GsaMember3d();
      var mem = new GsaMember3d();
      if (da.GetData(0, ref gsaMember3d)) {
        if (gsaMember3d == null)
          this.AddRuntimeWarning("Member3D input is null");
        mem = gsaMember3d.Duplicate(true);
      }

      if (mem == null)
        return;

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId))
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
          mem.Id = id;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp)) {
        var brep = new Brep();
        var mesh = new Mesh();
        if (GH_Convert.ToBrep(ghTyp.Value, ref brep, GH_Conversion.Both))
          mem = mem.UpdateGeometry(brep);
        else if (GH_Convert.ToMesh(ghTyp.Value, ref mesh, GH_Conversion.Both))
          mem = mem.UpdateGeometry(mesh);
        else {
          this.AddRuntimeError("Unable to convert Geometry input to a 3D Member");
          return;
        }
      }

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(3, ref ghTyp)) {
        var prop3d = new GsaProp3d();
        if (ghTyp.Value is GsaProp3dGoo)
          ghTyp.CastTo(ref prop3d);
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
            prop3d = new GsaProp3d(id);
          else {
            this.AddRuntimeError(
              "Unable to convert PA input to a 3D Property of reference integer");
            return;
          }
        }

        mem.Prop3d = prop3d;
      }

      double meshSize = 0;
      if (da.GetData(4, ref meshSize))
        mem.MeshSize = meshSize;

      var ghbool = new GH_Boolean();
      if (da.GetData(5, ref ghbool))
        if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
          if (mem.MeshWithOthers != mbool)
            mem.MeshWithOthers = mbool;

      var ghnm = new GH_String();
      if (da.GetData(6, ref ghnm))
        if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
          mem.Name = name;

      var ghgrp = new GH_Integer();
      if (da.GetData(7, ref ghgrp))
        if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
          mem.Group = grp;

      var ghcol = new GH_Colour();
      if (da.GetData(8, ref ghcol))
        if (GH_Convert.ToColor(ghcol, out Color col, GH_Conversion.Both))
          mem.Colour = col;

      var ghdum = new GH_Boolean();
      if (da.GetData(9, ref ghdum))
        if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
          mem.IsDummy = dum;

      da.SetData(0, new GsaMember3dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.SolidMesh);
      da.SetData(3, new GsaProp3dGoo(mem.Prop3d));
      da.SetData(4, mem.MeshSize);
      da.SetData(5, mem.MeshWithOthers);
      da.SetData(6, mem.Name);
      da.SetData(7, mem.Group);
      da.SetData(8, mem.Colour);
      da.SetData(9, mem.IsDummy);
      da.SetData(10, mem.ApiMember.Topology);
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("e7d66219-2243-4108-9d6e-4a84dbf07d55");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditMem3d;

    public EditMember3d() : base("Edit 3D Member",
      "Mem3dEdit",
      "Modify GSA 3D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter(),
        GsaMember3dGoo.Name,
        GsaMember3dGoo.NickName,
        GsaMember3dGoo.Description
        + " to get or set information for. Leave blank to create a new "
        + GsaMember3dGoo.Name,
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member3d Number",
        "ID",
        "Set Member Number. If ID is set it will replace any existing 3d Member in the model",
        GH_ParamAccess.item);
      pManager.AddGeometryParameter("Solid",
        "S",
        "Reposition Solid Geometry - Closed Brep or Mesh",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp3dParameter(),
        "3D Property",
        "PV",
        "Set new 3D Property.",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Set Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others",
        "M/o",
        "Mesh with others?",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member3d Name", "Na", "Set Name of Member3d", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member3d Group",
        "Gr",
        "Set Member 3d Group",
        GH_ParamAccess.item);
      pManager.AddColourParameter("Member3d Colour",
        "Co",
        "Set Member 3d Colour",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member",
        "Dm",
        "Set Member to Dummy",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i]
          .Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter(),
        GsaMember3dGoo.Name,
        GsaMember3dGoo.NickName,
        GsaMember3dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddMeshParameter("Solid Mesh", "M", "Member Solid Mesh", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaProp3dParameter(),
        "3D Property",
        "PV",
        "Get 3D Property",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Get Target mesh size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others",
        "M/o",
        "Get if to mesh with others",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member",
        "Dm",
        "Get if Member is Dummy",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Topology",
        "Tp",
        "Get the Member's original topology list referencing node IDs in Model that Model was created from",
        GH_ParamAccess.item);
    }

    #endregion

    #region IGH_VariableParameterComponent null implementation

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
      => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
      => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
      => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;

    public void VariableParameterMaintenance() { }

    #endregion
  }
}
