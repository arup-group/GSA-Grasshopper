using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components.GraveyardComp {
  /// <summary>
  /// Component to edit a 1D Member
  /// </summary>
  public class EditMember1d3_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("06ae2d01-b152-49c1-9356-c83714c4e5f4");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public EditMember1d3_OBSOLETE() : base("Edit 1D Member",
      "Mem1dEdit",
      "Modify GSA 1D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); }) {
          Enabled = true,
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
      => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
      => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
      => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;

    public override bool Read(GH_IReader reader) {
      _lengthUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }

    public void VariableParameterMaintenance() { }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override Bitmap Icon => Resources.EditMem1d;
    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (Params.Input[10] is Param_Number angleParameter)
        _angleUnit = angleParameter.UseDegrees
          ? AngleUnit.Degree
          : AngleUnit.Radian;
      Message = Length.GetAbbreviation(_lengthUnit);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMember1dParameter(),
        GsaMember1dGoo.Name,
        GsaMember1dGoo.NickName,
        GsaMember1dGoo.Description
        + " to get or set information for. Leave blank to create a new "
        + GsaMember1dGoo.Name,
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number",
        "ID",
        "Set Member Number. If ID is set it will replace any existing 1D Member in the model.",
        GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter(),
        "Section",
        "PB",
        "Set new Section Property.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Group",
        "Gr",
        "Set Member 1D Group",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type",
        "mT",
        "Set 1D Member Type"
        + Environment.NewLine
        + "Default is 0: Generic 1D - Accepted inputs are:"
        + Environment.NewLine
        + "2: Beam"
        + Environment.NewLine
        + "3: Column"
        + Environment.NewLine
        + "6: Cantilever"
        + Environment.NewLine
        + "8: Compos"
        + Environment.NewLine
        + "9: Pile"
        + Environment.NewLine
        + "11: Void cutter",
        GH_ParamAccess.item);
      pManager.AddTextParameter("1D Element Type",
        "eT",
        "Set Element 1D Type"
        + Environment.NewLine
        + "Accepted inputs are:"
        + Environment.NewLine
        + "1: Bar"
        + Environment.NewLine
        + "2: Beam"
        + Environment.NewLine
        + "3: Spring"
        + Environment.NewLine
        + "9: Link"
        + Environment.NewLine
        + "10: Cable"
        + Environment.NewLine
        + "19: Spacer"
        + Environment.NewLine
        + "20: Strut"
        + Environment.NewLine
        + "21: Tie"
        + Environment.NewLine
        + "23: Rod"
        + Environment.NewLine
        + "24: Damper",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(),
        "Offset",
        "Of",
        "Set Member Offset",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(),
        "Start release",
        "⭰",
        "Set Release (Bool6) at Start of Member",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(),
        "End release",
        "⭲",
        "Set Release (Bool6) at End of Member",
        GH_ParamAccess.item);
      pManager.AddAngleParameter("Orientation Angle",
        "⭮A",
        "Set Member Orientation Angle",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeParameter(),
        "Orientation Node",
        "⭮N",
        "Set Member Orientation Node",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Set Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others",
        "M/o",
        "Mesh with others?",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter(),
        "Set " + GsaBucklingLengthFactorsGoo.Name,
        GsaBucklingLengthFactorsGoo.NickName,
        GsaBucklingLengthFactorsGoo.Description,
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member1d Name", "Na", "Set Name of Member1d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member1d Colour",
        "Co",
        "Set Member 1D Colour",
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
      pManager.AddParameter(new GsaMember1dParameter(),
        GsaMember1dGoo.Name,
        GsaMember1dGoo.NickName,
        GsaMember1dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number",
        "ID",
        "Get Member Number",
        GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaSectionParameter(),
        "Section",
        "PB",
        "Get Section Property",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT", "Get 1D Member Type", GH_ParamAccess.item);
      pManager.AddTextParameter("1D Element Type",
        "eT",
        "Get Element 1D Type",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(),
        "Offset",
        "Of",
        "Get Member Offset",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(),
        "Start release",
        "⭰",
        "Get Release (Bool6) at Start of Member",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(),
        "End release",
        "⭲",
        "Get Release (Bool6) at End of Member",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle",
        "⭮A",
        "Get Member Orientation Angle in radians",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node",
        "⭮N",
        "Get Member Orientation Node",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Get Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others",
        "M/o",
        "Get if to mesh with others",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter(),
        "Get " + GsaBucklingLengthFactorsGoo.Name,
        GsaBucklingLengthFactorsGoo.NickName,
        GsaBucklingLengthFactorsGoo.Description,
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);

      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member",
        "Dm",
        "Get it Member is Dummy",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Topology",
        "Tp",
        "Get the Member's original topology list referencing node IDs in Model that Model was created from",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaMember1d = new GsaMember1d();
      var mem = new GsaMember1d();
      if (da.GetData(0, ref gsaMember1d)) {
        if (gsaMember1d == null)
          this.AddRuntimeWarning("Member1D input is null");
        mem = gsaMember1d.Duplicate(true);
      }

      if (mem == null)
        return;

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId))
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
          mem.Id = id;

      var ghcrv = new GH_Curve();
      if (da.GetData(2, ref ghcrv)) {
        Curve crv = null;
        if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both)) {
          if (crv is PolyCurve curve)
            mem.PolyCurve = curve;
          else {
            var tempMem = new GsaMember1d(crv);
            mem.PolyCurve = tempMem.PolyCurve;
          }
        }
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(3, ref ghTyp)) {
        var section = new GsaSection();
        if (ghTyp.Value is GsaSectionGoo)
          ghTyp.CastTo(ref section);
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
            section = new GsaSection(id);
          else {
            this.AddRuntimeError(
              "Unable to convert PB input to a Section Property of reference integer");
            return;
          }
        }

        mem.Section = section;
      }

      var ghgrp = new GH_Integer();
      if (da.GetData(4, ref ghgrp))
        if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
          mem.Group = grp;

      var ghstring = new GH_String();
      if (da.GetData(5, ref ghstring)) {
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
      if (da.GetData(6, ref ghstring)) {
        if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both))
          mem.Type1D = (ElementType)typeInt;
        else if (GH_Convert.ToString(ghstring, out string typestring, GH_Conversion.Both))
          try {
            mem.Type1D = Mappings.GetElementType(typestring);
          }
          catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Element Type");
          }
      }

      var offset = new GsaOffset();
      if (da.GetData(7, ref offset))
        mem.Offset = offset;

      var start = new GsaBool6();
      if (da.GetData(8, ref start))
        mem.ReleaseStart = start;

      var end = new GsaBool6();
      if (da.GetData(9, ref end))
        mem.ReleaseEnd = end;

      var ghangle = new GH_Number();
      if (da.GetData(10, ref ghangle))
        if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
          mem.OrientationAngle = new Angle(angle, _angleUnit);

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(11, ref ghTyp)) {
        var node = new GsaNode();
        if (ghTyp.Value is GsaNodeGoo) {
          ghTyp.CastTo(ref node);
          mem.OrientationNode = node;
        }
        else
          this.AddRuntimeWarning("Unable to convert Orientation Node input to GsaNode");
      }

      double meshSize = 0;
      if (da.GetData(12, ref meshSize))
        mem.MeshSize = meshSize;

      var ghbool = new GH_Boolean();
      if (da.GetData(13, ref ghbool))
        if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
          if (mem.MeshWithOthers != mbool)
            mem.MeshWithOthers = mbool;

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(14, ref ghTyp)) {
        var fls = new GsaBucklingLengthFactors();
        if (ghTyp.Value is GsaBucklingLengthFactorsGoo) {
          ghTyp.CastTo(ref fls);
          mem.ApiMember.MomentAmplificationFactorStrongAxis
            = fls.MomentAmplificationFactorStrongAxis;
          mem.ApiMember.MomentAmplificationFactorWeakAxis = fls.MomentAmplificationFactorWeakAxis;
          mem.ApiMember.EquivalentUniformMomentFactor = fls.EquivalentUniformMomentFactor;
        }
        else
          this.AddRuntimeWarning("Unable to change buckling length factors");
      }

      var ghnm = new GH_String();
      if (da.GetData(15, ref ghnm))
        if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
          mem.Name = name;

      var ghcol = new GH_Colour();
      if (da.GetData(16, ref ghcol))
        if (GH_Convert.ToColor(ghcol, out Color col, GH_Conversion.Both))
          mem.Colour = col;

      var ghdum = new GH_Boolean();
      if (da.GetData(17, ref ghdum))
        if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
          mem.IsDummy = dum;

      da.SetData(0, new GsaMember1dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.PolyCurve);
      da.SetData(3, new GsaSectionGoo(mem.Section));
      da.SetData(4, mem.Group);
      da.SetData(5,
        Mappings.s_memberTypeMapping.FirstOrDefault(x => x.Value == mem.Type)
          .Key);
      da.SetData(6,
        Mappings.s_elementTypeMapping.FirstOrDefault(x => x.Value == mem.Type1D)
          .Key);
      da.SetData(7, new GsaOffsetGoo(mem.Offset));
      da.SetData(8, new GsaBool6Goo(mem.ReleaseStart));
      da.SetData(9, new GsaBool6Goo(mem.ReleaseEnd));
      da.SetData(10, mem.OrientationAngle.Radians);
      da.SetData(11, new GsaNodeGoo(mem.OrientationNode));
      da.SetData(12, mem.MeshSize);
      da.SetData(13, mem.MeshWithOthers);
      da.SetData(14,
        new GsaBucklingLengthFactorsGoo(new GsaBucklingLengthFactors(mem, _lengthUnit)));
      da.SetData(15, mem.Name);
      da.SetData(16, mem.Colour);
      da.SetData(17, mem.IsDummy);
      da.SetData(18, mem.ApiMember.Topology);
    }

    private AngleUnit _angleUnit = AngleUnit.Radian;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
