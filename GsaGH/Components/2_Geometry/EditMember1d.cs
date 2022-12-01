using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
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
    /// Component to edit a 1D Member
    /// </summary>
    public class EditMember1d : GH_OasysComponent, IGH_PreviewObject, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("06ae2d01-b152-49c1-9356-c83714c4e5f4");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditMem1d;

    public EditMember1d() : base("Edit 1D Member",
      "Mem1dEdit",
      "Modify GSA 1D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember1dParameter(), GsaMember1dGoo.Name, GsaMember1dGoo.NickName, GsaMember1dGoo.Description + " to get or set information for. Leave blank to create a new " + GsaMember1dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID", "Set Member Number. If ID is set it will replace any existing 1D Member in the model.", GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter(), "Section", "PB", "Set new Section Property.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Group", "Gr", "Set Member 1D Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT", "Set 1D Member Type" + Environment.NewLine +
          "Default is 0: Generic 1D - Accepted inputs are:" + Environment.NewLine +
          "2: Beam" + Environment.NewLine +
          "3: Column" + Environment.NewLine +
          "6: Cantilever" + Environment.NewLine +
          "8: Compos" + Environment.NewLine +
          "9: Pile" + Environment.NewLine +
          "11: Void cutter", GH_ParamAccess.item);
      pManager.AddTextParameter("1D Element Type", "eT", "Set Element 1D Type" + Environment.NewLine +
          "Accepted inputs are:" + Environment.NewLine +
          "1: Bar" + Environment.NewLine +
          "2: Beam" + Environment.NewLine +
          "3: Spring" + Environment.NewLine +
          "9: Link" + Environment.NewLine +
          "10: Cable" + Environment.NewLine +
          "19: Spacer" + Environment.NewLine +
          "20: Strut" + Environment.NewLine +
          "21: Tie" + Environment.NewLine +
          "23: Rod" + Environment.NewLine +
          "24: Damper", GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Member Offset", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰", "Set Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲", "Set Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddAngleParameter("Orientation Angle", "⭮A", "Set Member Orientation Angle", GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeParameter(), "Orientation Node", "⭮N", "Set Member Orientation Node", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Set Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter(), "Set " + GsaBucklingLengthFactorsGoo.Name, GsaBucklingLengthFactorsGoo.NickName, GsaBucklingLengthFactorsGoo.Description, GH_ParamAccess.item);
      pManager.AddTextParameter("Member1d Name", "Na", "Set Name of Member1d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member1d Colour", "Co", "Set Member 1D Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember1dParameter(), GsaMember1dGoo.Name, GsaMember1dGoo.NickName, GsaMember1dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaSectionParameter(), "Section", "PB", "Get Section Property", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT", "Get 1D Member Type", GH_ParamAccess.item);
      pManager.AddTextParameter("1D Element Type", "eT", "Get Element 1D Type", GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Member Offset", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰", "Get Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲", "Get Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle", "⭮A", "Get Member Orientation Angle in radians", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Get Member Orientation Node", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Get Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter(), "Get " + GsaBucklingLengthFactorsGoo.Name, GsaBucklingLengthFactorsGoo.NickName, GsaBucklingLengthFactorsGoo.Description, GH_ParamAccess.item);
      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);

      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get it Member is Dummy", GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp", "Get the Member's original topology list referencing node IDs in Model that Model was created from", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMember1d gsaMember1d = new GsaMember1d();
      GsaMember1d mem = new GsaMember1d();
      if (DA.GetData(0, ref gsaMember1d))
      {
        if (gsaMember1d == null) { 
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member1D input is null");
        }
        mem = gsaMember1d.Duplicate(true);
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

        // 2 curve
        GH_Curve ghcrv = new GH_Curve();
        if (DA.GetData(2, ref ghcrv))
        {
          Curve crv = null;
          if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both))
          {
            if (crv is PolyCurve)
              mem.PolyCurve = (PolyCurve)crv;
            else
            {
              GsaMember1d tempMem = new GsaMember1d(crv);
              mem.PolyCurve = tempMem.PolyCurve;
            }
          }
        }

        // 3 section
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(3, ref gh_typ))
        {
          GsaSection section = new GsaSection();
          if (gh_typ.Value is GsaSectionGoo)
          {
            gh_typ.CastTo(ref section);
          }
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
            {
              section = new GsaSection(idd);
            }
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
          mem.Section = section;
        }

        // 4 Group
        GH_Integer ghgrp = new GH_Integer();
        if (DA.GetData(4, ref ghgrp))
        {
          if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
            mem.Group = grp;
        }

        // 5 member type
        GH_String ghstring = new GH_String();
        if (DA.GetData(5, ref ghstring))
        {
          if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
            mem.Type = (MemberType)typeInt;
          else if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          {
            if (Mappings.MemberTypeMapping.ContainsKey(typestring))
              mem.Type = Mappings.MemberTypeMapping[typestring];
            else
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to change Element1D Type");
          }
        }

        // 6 element type
        ghstring = new GH_String();
        if (DA.GetData(6, ref ghstring))
        {
          if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
            mem.Type1D = (ElementType)typeInt;
          else if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          {
            if (Mappings.ElementTypeMapping.ContainsKey(typestring))
              mem.Type1D = Mappings.ElementTypeMapping[typestring];
            else
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to change Element1D Type");
          }
        }

        // 7 offset
        GsaOffset offset = new GsaOffset();
        if (DA.GetData(7, ref offset))
        {
          mem.Offset = offset;
        }

        // 8 start release
        GsaBool6 start = new GsaBool6();
        if (DA.GetData(8, ref start))
        {
          mem.ReleaseStart = start;
        }

        // 9 end release
        GsaBool6 end = new GsaBool6();
        if (DA.GetData(9, ref end))
        {
          mem.ReleaseEnd = end;
        }

        // 10 orientation angle
        GH_Number ghangle = new GH_Number();
        if (DA.GetData(10, ref ghangle))
        {
          if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
            mem.OrientationAngle = new Angle(angle, this.AngleUnit);
        }

        // 11 orientation node
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(11, ref gh_typ))
        {
          GsaNode node = new GsaNode();
          if (gh_typ.Value is GsaNodeGoo)
          {
            gh_typ.CastTo(ref node);
            mem.OrientationNode = node;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert Orientation Node input to GsaNode");
          }
        }

        // 12 mesh size
        double meshSize = 0;
        if (DA.GetData(12, ref meshSize))
        {
          mem.MeshSize = meshSize;
        }

        // 13 mesh with others
        GH_Boolean ghbool = new GH_Boolean();
        if (DA.GetData(13, ref ghbool))
        {
          if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
          {
            if (mem.MeshWithOthers != mbool)
              mem.MeshWithOthers = mbool;
          }
        }

        // 14 buckling length factors
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(14, ref gh_typ))
        {
          GsaBucklingLengthFactors fls = new GsaBucklingLengthFactors();
          if (gh_typ.Value is GsaBucklingLengthFactorsGoo)
          {
            gh_typ.CastTo(ref fls);
            mem.ApiMember.MomentAmplificationFactorStrongAxis = fls.MomentAmplificationFactorStrongAxis;
            mem.ApiMember.MomentAmplificationFactorWeakAxis = fls.MomentAmplificationFactorWeakAxis;
            mem.ApiMember.LateralTorsionalBucklingFactor = fls.LateralTorsionalBucklingFactor;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to change buckling length factors");
          }
        }

        // 15 name
        GH_String ghnm = new GH_String();
        if (DA.GetData(15, ref ghnm))
        {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            mem.Name = name;
        }

        // 16 Colour
        GH_Colour ghcol = new GH_Colour();
        if (DA.GetData(16, ref ghcol))
        {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            mem.Colour = col;
        }

        // 17 Dummy
        GH_Boolean ghdum = new GH_Boolean();
        if (DA.GetData(17, ref ghdum))
        {
          if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
            mem.IsDummy = dum;
        }

        // #### outputs ####
        DA.SetData(0, new GsaMember1dGoo(mem));
        DA.SetData(1, mem.Id);
        DA.SetData(2, mem.PolyCurve);
        DA.SetData(3, new GsaSectionGoo(mem.Section));
        DA.SetData(4, mem.Group);
        DA.SetData(5, Mappings.MemberTypeMapping.FirstOrDefault(x => x.Value == mem.Type).Key);
        DA.SetData(6, Mappings.ElementTypeMapping.FirstOrDefault(x => x.Value == mem.Type1D).Key);

        DA.SetData(7, new GsaOffsetGoo(mem.Offset));

        DA.SetData(8, new GsaBool6Goo(mem.ReleaseStart));
        DA.SetData(9, new GsaBool6Goo(mem.ReleaseEnd));

        DA.SetData(10, mem.OrientationAngle.As(AngleUnit.Radian));
        DA.SetData(11, new GsaNodeGoo(mem.OrientationNode));

        DA.SetData(12, mem.MeshSize);
        DA.SetData(13, mem.MeshWithOthers);

        DA.SetData(14, new GsaBucklingLengthFactorsGoo(new GsaBucklingLengthFactors(mem, this.LengthUnit)));

        DA.SetData(15, mem.Name);

        DA.SetData(16, mem.Colour);
        DA.SetData(17, mem.IsDummy);
        DA.SetData(18, mem.ApiMember.Topology.ToString());
      }
    }

    #region Custom UI
    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      Param_Number angleParameter = Params.Input[10] as Param_Number;
      if (angleParameter != null)
      {
        if (angleParameter.UseDegrees)
          this.AngleUnit = AngleUnit.Degree;
        else
          this.AngleUnit = AngleUnit.Radian;
      }
      this.Message = Length.GetAbbreviation(this.LengthUnit);
    }
    AngleUnit AngleUnit = AngleUnit.Radian;

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
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      this.Params.Input[12].Name = "Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]";
      this.Params.Output[12].Name = "Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]";
    }

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}

