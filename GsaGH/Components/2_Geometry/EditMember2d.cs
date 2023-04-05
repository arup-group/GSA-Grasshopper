﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 2D Member
  /// </summary>
  public class EditMember2d : GH_OasysComponent, IGH_VariableParameterComponent {
    private AngleUnit _angleUnit = AngleUnit.Radian;

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaMember2d = new GsaMember2d();
      var mem = new GsaMember2d();
      if (da.GetData(0, ref gsaMember2d)) {
        if (gsaMember2d == null) {
          this.AddRuntimeWarning("Member2D input is null");
          return;
        }

        mem = gsaMember2d.Duplicate(true);
      }

      if (mem == null)
        return;

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId))
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
          mem.Id = id;

      Brep brep = mem.Brep;
      var ghbrep = new GH_Brep();
      var crvlist = new CurveList(mem.InclusionLines ?? new List<PolyCurve>());
      var crvs = crvlist.ToList();
      var ghcrvs = new List<GH_Curve>();
      var ghpts = new List<GH_Point>();
      List<Point3d> pts = mem.InclusionPoints;

      if ((da.GetData(2, ref ghbrep))
        || (da.GetDataList(3, ghpts))
        || (da.GetDataList(4, ghcrvs))) {
        if (da.GetData(2, ref ghbrep))
          GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both);

        ghpts = new List<GH_Point>();
        if (da.GetDataList(3, ghpts)) {
          pts = new List<Point3d>();
          foreach (GH_Point ghPoint in ghpts) {
            var pt = new Point3d();
            if (GH_Convert.ToPoint3d(ghPoint, ref pt, GH_Conversion.Both))
              pts.Add(pt);
          }
        }

        ghcrvs = new List<GH_Curve>();
        if (da.GetDataList(4, ghcrvs)) {
          crvs = new List<Curve>();
          foreach (GH_Curve curve in ghcrvs) {
            Curve crv = null;
            if (GH_Convert.ToCurve(curve, ref crv, GH_Conversion.Both))
              crvs.Add(crv);
          }
        }

        mem = mem.UpdateGeometry(brep, crvs, pts);
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(5, ref ghTyp)) {
        var prop2d = new GsaProp2d();
        if (ghTyp.Value is GsaProp2dGoo)
          ghTyp.CastTo(ref prop2d);
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
            prop2d = new GsaProp2d(id);
          else {
            this.AddRuntimeError(
              "Unable to convert PA input to a 2D Property of reference integer");
            return;
          }
        }

        mem.Property = prop2d;
      }

      var ghgrp = new GH_Integer();
      if (da.GetData(6, ref ghgrp))
        if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
          mem.Group = grp;

      var ghstring = new GH_String();
      if (da.GetData(7, ref ghstring)) {
        if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
          mem.Type = (MemberType)typeInt;
        else if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          try {
            mem.Type = Mappings.GetMemberType(typestring);
          }
          catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Member Type");
          }
      }

      ghstring = new GH_String();
      if (da.GetData(8, ref ghstring)) {
        if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
          mem.Type2D = (AnalysisOrder)typeInt;
        else if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          try {
            mem.Type2D = Mappings.GetAnalysisOrder(typestring);
          }
          catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Analysis Element Type");
          }
      }

      var offset = new GsaOffset();
      if (da.GetData(9, ref offset))
        mem.Offset = offset;

      var ioData = new GH_Boolean();
      if (da.GetData(10, ref ioData))
        if (GH_Convert.ToBoolean(ioData, out bool ioBool, GH_Conversion.Both))
          mem.AutomaticInternalOffset = ioBool;

      double meshSize = 0;
      if (da.GetData(11, ref meshSize))
        mem.MeshSize = meshSize;

      var ghbool = new GH_Boolean();
      if (da.GetData(12, ref ghbool))
        if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
          mem.MeshWithOthers = mbool;

      var ghangle = new GH_Number();
      if (da.GetData(13, ref ghangle))
        if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
          mem.OrientationAngle = new Angle(angle, _angleUnit);

      var ghnm = new GH_String();
      if (da.GetData(14, ref ghnm))
        if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
          mem.Name = name;

      var ghcol = new GH_Colour();
      if (da.GetData(15, ref ghcol))
        if (GH_Convert.ToColor(ghcol, out Color col, GH_Conversion.Both))
          mem.Colour = col;

      var ghdum = new GH_Boolean();
      if (da.GetData(16, ref ghdum))
        if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
          mem.IsDummy = dum;

      da.SetData(0, new GsaMember2dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.Brep);
      da.SetDataList(3, mem.InclusionPoints);
      da.SetDataList(4, mem.InclusionLines);
      da.SetData(5, new GsaProp2dGoo(mem.Property));
      da.SetData(6, mem.Group);
      da.SetData(7,
        Mappings.s_memberTypeMapping.FirstOrDefault(x => x.Value == mem.Type)
          .Key);
      da.SetData(8,
        Mappings.s_analysisOrderMapping.FirstOrDefault(x => x.Value == mem.Type2D)
          .Key);
      da.SetData(9, new GsaOffsetGoo(mem.Offset));
      da.SetData(10, mem.AutomaticInternalOffset);
      da.SetData(11, mem.MeshSize);
      da.SetData(12, mem.MeshWithOthers);
      da.SetData(13, mem.OrientationAngle.Radians);
      da.SetData(14, mem.Name);
      da.SetData(15, mem.Colour);
      da.SetData(16, mem.IsDummy);
      da.SetData(17, mem.ApiMember.Topology);
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (Params.Input[12] is Param_Number angleParameter)
        _angleUnit = angleParameter.UseDegrees
          ? AngleUnit.Degree
          : AngleUnit.Radian;
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("01390fdb-4319-46e9-9ff9-6a5d274e185d");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditMem2d;

    public EditMember2d() : base("Edit 2D Member",
      "Mem2dEdit",
      "Modify GSA 2D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMember2dParameter(),
        GsaMember2dGoo.Name,
        GsaMember2dGoo.NickName,
        GsaMember2dGoo.Description
        + " to get or set information for. Leave blank to create a new "
        + GsaMember2dGoo.Name,
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member2d Number",
        "ID",
        "Set Member Number. If ID is set it will replace any existing 2d Member in the model",
        GH_ParamAccess.item);
      pManager.AddBrepParameter("Brep",
        "B",
        "Reposition Member Brep (non-planar geometry will be automatically converted to an average plane from exterior boundary control points)",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points",
        "(P)",
        "Add inclusion points (will automatically be projected onto Brep)",
        GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves",
        "(C)",
        "Add inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter(),
        "2D Property",
        "PA",
        "Set new 2D Property.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member2d Group",
        "Gr",
        "Set Member 2d Group",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type",
        "mT",
        "Set 2D Member Type"
        + Environment.NewLine
        + "Default is 1: Generic 2D - Accepted inputs are:"
        + Environment.NewLine
        + "4: Slab"
        + Environment.NewLine
        + "5: Wall"
        + Environment.NewLine
        + "7: Ribbed Slab"
        + Environment.NewLine
        + "12: Void-cutter",
        GH_ParamAccess.item);
      pManager.AddTextParameter("2D Element Type",
        "eT",
        "Set Member 2D Analysis Element Type"
        + Environment.NewLine
        + "Accepted inputs are:"
        + Environment.NewLine
        + "0: Linear - Tri3/Quad4 Elements (default)"
        + Environment.NewLine
        + "1: Quadratic - Tri6/Quad8 Elements"
        + Environment.NewLine
        + "2: Rigid Diaphragm",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(),
        "Offset",
        "Of",
        "Set Member Offset",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Internal Offset",
        "IO",
        "Set Automatic Internal Offset of Member",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Set target mesh size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others",
        "M/o",
        "Mesh with others?",
        GH_ParamAccess.item);
      pManager.AddAngleParameter("Orientation Angle",
        "⭮A",
        "Set Member Orientation Angle",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member2d Name", "Na", "Set Name of Member2d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member2d Colour",
        "Co",
        "Set Member 2d Colour",
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
      pManager.HideParameter(3);
      pManager.HideParameter(4);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember2dParameter(),
        GsaMember2dGoo.Name,
        GsaMember2dGoo.NickName,
        GsaMember2dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddBrepParameter("Brep", "B", "Member Brep", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddPointParameter("Incl. Points",
        "(P)",
        "Get Inclusion points",
        GH_ParamAccess.list);
      pManager.HideParameter(3);
      pManager.AddCurveParameter("Incl. Curves",
        "(C)",
        "Get Inclusion curves",
        GH_ParamAccess.list);
      pManager.HideParameter(4);
      pManager.AddParameter(new GsaProp2dParameter(),
        "2D Property",
        "PA",
        "Get 2D Section Property",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT", "Get 2D Member Type", GH_ParamAccess.item);
      pManager.AddTextParameter("2D Element Type",
        "eT",
        "Get Member 2D Analysis Element Type"
        + Environment.NewLine
        + "0: Linear (Tri3/Quad4), 1: Quadratic (Tri6/Quad8), 2: Rigid Diaphragm",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(),
        "Offset",
        "Of",
        "Get Member Offset",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Internal Offset",
        "IO",
        "Get Automatic Internal Offset of Member",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Get Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others",
        "M/o",
        "Get if to mesh with others",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle",
        "⭮A",
        "Get Member Orientation Angle in radians",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
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
