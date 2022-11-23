using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a 2D Member
  /// </summary>
  public class EditMember2d : GH_OasysComponent, IGH_PreviewObject, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("54bcb967-d3a1-4878-925b-5fd765d1b476");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditMem2d;

    public EditMember2d()
      : base("Edit 2D Member", "Mem2dEdit", "Modify GSA 2D Member",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember2dParameter(), GsaMember2dGoo.Name, GsaMember2dGoo.NickName, GsaMember2dGoo.Description + " to get or set information for. Leave blank to create a new " + GsaMember2dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member2d Number", "ID", "Set Member Number. If ID is set it will replace any existing 2d Member in the model", GH_ParamAccess.item);
      pManager.AddBrepParameter("Brep", "B", "Reposition Member Brep (non-planar geometry will be automatically converted to an average plane from exterior boundary control points)", GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points", "(P)", "Add inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves", "(C)", "Add inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter(), "2D Property", "PA", "Set new 2D Property.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member2d Group", "Gr", "Set Member 2d Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT", "Set 2D Member Type" + System.Environment.NewLine
          + "Default is 1: Generic 2D - Accepted inputs are:" + System.Environment.NewLine +
          "4: Slab" + System.Environment.NewLine +
          "5: Wall" + System.Environment.NewLine +
          "7: Ribbed Slab" + System.Environment.NewLine +
          "12: Void-cutter", GH_ParamAccess.item);
      pManager.AddTextParameter("2D Element Type", "aT", "Set Member 2D Analysis Element Type" + System.Environment.NewLine +
          "Accepted inputs are:" + System.Environment.NewLine +
          "0: Linear - Tri3/Quad4 Elements (default)" + System.Environment.NewLine +
          "1: Quadratic - Tri6/Quad8 Elements" + System.Environment.NewLine +
          "2: Rigid Diaphragm", GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Member Offset", GH_ParamAccess.item);

      pManager.AddGenericParameter("Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Ms", "Set target mesh size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item);
      pManager.AddTextParameter("Member2d Name", "Na", "Set Name of Member2d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member2d Colour", "Co", "Set Member 2d Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
      pManager.HideParameter(4);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember2dParameter(), GsaMember2dGoo.Name, GsaMember2dGoo.NickName, GsaMember2dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddBrepParameter("Brep", "B", "Member Brep", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddPointParameter("Incl. Points", "(P)", "Get Inclusion points", GH_ParamAccess.list);
      pManager.HideParameter(3);
      pManager.AddCurveParameter("Incl. Curves", "(C)", "Get Inclusion curves", GH_ParamAccess.list);
      pManager.HideParameter(4);
      pManager.AddParameter(new GsaProp2dParameter(), "2D Property", "PA", "Get 2D Section Property", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);

      pManager.AddTextParameter("Member Type", "mT", "Get 2D Member Type", GH_ParamAccess.item);
      pManager.AddTextParameter("2D Element Type", "eT", "Get Member 2D Analysis Element Type" + System.Environment.NewLine +
          "0: Linear (Tri3/Quad4), 1: Quadratic (Tri6/Quad8), 2: Rigid Diaphragm", GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Member Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Ms", "Set Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);

      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get if Member is Dummy", GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp", "Get the Member's original topology list referencing node IDs in Model that Model was created from", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMember2d gsaMember2d = new GsaMember2d();
      GsaMember2d mem = new GsaMember2d();
      if (DA.GetData(0, ref gsaMember2d))
      {
        if (gsaMember2d == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member2D input is null");
        }
        mem = gsaMember2d.Duplicate();
      }

      if (mem != null)
      {
        // #### inputs ####
        // 1 ID
        GH_Integer ghID = new GH_Integer();
        if (DA.GetData(1, ref ghID))
        {
          if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
            mem.ID = id;
        }

        // 2/3/4 Brep, incl.pts and incl.lns
        Brep brep = mem.Brep; //existing brep
        GH_Brep ghbrep = new GH_Brep();
        CurveList crvlist = new CurveList(mem.InclusionLines == null ? new List<PolyCurve>() : mem.InclusionLines);
        List<Curve> crvs = crvlist.ToList();
        List<GH_Curve> ghcrvs = new List<GH_Curve>();
        List<GH_Point> ghpts = new List<GH_Point>();
        List<Point3d> pts = mem.InclusionPoints;

        if ((DA.GetData(2, ref ghbrep)) || (DA.GetDataList(3, ghpts)) || (DA.GetDataList(4, ghcrvs)))
        {
          // 2 brep
          if (DA.GetData(2, ref ghbrep))
          {
            GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both);
          }

          // 3 inclusion points
          if (DA.GetDataList(3, ghpts))
          {
            pts = new List<Point3d>();
            for (int i = 0; i < ghpts.Count; i++)
            {
              Point3d pt = new Point3d();
              if (GH_Convert.ToPoint3d(ghpts[i], ref pt, GH_Conversion.Both))
              {
                pts.Add(pt);
              }
            }
          }

          // 4 inclusion lines
          if (DA.GetDataList(4, ghcrvs))
          {
            crvs = new List<Curve>();
            for (int i = 0; i < ghcrvs.Count; i++)
            {
              Curve crv = null;
              if (GH_Convert.ToCurve(ghcrvs[i], ref crv, GH_Conversion.Both))
              {
                crvs.Add(crv);
              }
            }
          }
          // rebuild 
          mem = mem.UpdateGeometry(brep, crvs, pts);
        }

        // 5 section
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(5, ref gh_typ))
        {
          GsaProp2d prop2d = new GsaProp2d();
          if (gh_typ.Value is GsaProp2dGoo)
          {
            gh_typ.CastTo(ref prop2d);
          }
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
            {
              prop2d = new GsaProp2d(idd);
            }
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
              return;
            }
          }
          mem.Property = prop2d;
        }

        // 6 Group
        GH_Integer ghgrp = new GH_Integer();
        if (DA.GetData(6, ref ghgrp))
        {
          if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
            mem.Group = grp;
        }

        // 7 type
        GH_String ghstring = new GH_String();
        if (DA.GetData(7, ref ghstring))
        {
          if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
            mem.Type = (MemberType)typeInt;
          if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          {
            if (Helpers.Mappings.MemberTypeMapping.ContainsKey(typestring))
              mem.Type = Helpers.Mappings.MemberTypeMapping[typestring];
            else
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to change Member Type");
          }
        }

        // 8 element type / analysis order
        ghstring = new GH_String();
        if (DA.GetData(8, ref ghstring))
        {
          if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
            mem.Type2D = (AnalysisOrder)typeInt;
          if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          {
            if (Helpers.Mappings.AnalysisOrderMapping.ContainsKey(typestring))
              mem.Type2D = Helpers.Mappings.AnalysisOrderMapping[typestring];
            else
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to change Analysis Element Type");
          }
        }

        // 9 offset
        GsaOffset offset = new GsaOffset();
        if (DA.GetData(9, ref offset))
        {
          mem.Offset = offset;
        }

        // 10 mesh size
        GH_Number ghmsz = new GH_Number();
        if (Params.Input[10].Sources.Count > 0)
        {
          mem.MeshSize = (Length)Input.UnitNumber(this, DA, 10, this.LengthUnit, true);
        }

        // 11 mesh with others
        GH_Boolean ghbool = new GH_Boolean();
        if (DA.GetData(11, ref ghbool))
        {
          if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
          {
            if (mem.MeshWithOthers != mbool)
              mem.MeshWithOthers = mbool;
          }
        }

        // 12 name
        GH_String ghnm = new GH_String();
        if (DA.GetData(12, ref ghnm))
        {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            mem.Name = name;
        }

        // 13 Colour
        GH_Colour ghcol = new GH_Colour();
        if (DA.GetData(13, ref ghcol))
        {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            mem.Colour = col;
        }

        // 14 Dummy
        GH_Boolean ghdum = new GH_Boolean();
        if (DA.GetData(14, ref ghdum))
        {
          if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
            mem.IsDummy = dum;
        }

        // #### outputs ####

        DA.SetData(0, new GsaMember2dGoo(mem));
        DA.SetData(1, mem.ID);
        DA.SetData(2, mem.Brep);
        DA.SetDataList(3, mem.InclusionPoints);
        DA.SetDataList(4, mem.InclusionLines);

        DA.SetData(5, new GsaProp2dGoo(mem.Property));
        DA.SetData(6, mem.Group);

        DA.SetData(7, Helpers.Mappings.MemberTypeMapping.FirstOrDefault(x => x.Value == mem.Type).Key);
        DA.SetData(8, Helpers.Mappings.AnalysisOrderMapping.FirstOrDefault(x => x.Value == mem.Type2D).Key);

        DA.SetData(9, new GsaOffsetGoo(mem.Offset));

        DA.SetData(10, new GH_UnitNumber(mem.MeshSize.ToUnit(this.LengthUnit)));
        DA.SetData(11, mem.MeshWithOthers);

        DA.SetData(12, mem.Name);
        DA.SetData(13, mem.Colour);
        DA.SetData(14, mem.IsDummy);
        DA.SetData(15, mem.API_Member.Topology.ToString());
      }
    }

    #region Custom UI
    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      this.Message = Length.GetAbbreviation(this.LengthUnit);
    }

    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripMenuItem unitsMenu = new ToolStripMenuItem("Select unit", Properties.Resources.Units);
      unitsMenu.Enabled = true;
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); });
        toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthUnit);
        toolStripMenuItem.Enabled = true;
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }
      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void Update(string unit)
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      this.Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("LengthUnit", this.LengthUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.LengthUnit = this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }

    #region IGH_VariableParameterComponent null implementation
    public virtual void VariableParameterMaintenance()
    {
      this.Params.Input[10].Name = "Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]";
      this.Params.Output[10].Name = "Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]";
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}
