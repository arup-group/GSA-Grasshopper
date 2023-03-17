using System;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit a 1D Member
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditMember1d2_OBSOLETE : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("2a121578-f9ff-4d80-ae90-2982faa425a6");
    public EditMember1d2_OBSOLETE()
      : base("Edit 1D Member", "Mem1dEdit", "Modify GSA 1D Member",
            CategoryName.Name(),
            SubCategoryName.Cat2()) { }

    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.EditMem1d;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {

      pManager.AddGenericParameter("1D Member", "M1D", "GSA 1D Member to Modify", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID", "Set Member Number. If ID is set it will replace any existing 1D Member in the model", GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section", "PB", "Change Section Property", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Group", "Gr", "Set Member 1D Group", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Type", "mT", "Set 1D Member Type" + Environment.NewLine +
          "Default is 0: Generic 1D - Accepted inputs are:" + Environment.NewLine +
          "2: Beam" + Environment.NewLine +
          "3: Column" + Environment.NewLine +
          "6: Cantilever" + Environment.NewLine +
          "8: Compos" + Environment.NewLine +
          "9: Pile" + Environment.NewLine +
          "11: Void cutter", GH_ParamAccess.item);
      pManager.AddIntegerParameter("1D Element Type", "eT", "Set Element 1D Type" + Environment.NewLine +
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
      pManager.AddGenericParameter("Offset", "Of", "Set Member Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start release", "⭰", "Set Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddGenericParameter("End release", "⭲", "Set Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle", "⭮A", "Set Member Orientation Angle in degrees", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Set Member Orientation Node", GH_ParamAccess.item);
      var ms = new Length(1, DefaultUnits.LengthUnitGeometry);
      pManager.AddGenericParameter("Mesh Size [" + ms.ToString("a") + "]", "Ms", "Set Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item);

      pManager.AddTextParameter("Member1d Name", "Na", "Set Name of Member1d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member1d Colour", "Co", "Set Member 1D Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

      for (int i = 1; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("1D Member", "M1D", "Modified GSA 1D Member", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID", "Get Member Number", GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddGenericParameter("Section", "PB", "Change Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Type", "mT", "Get 1D Member Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("1D Element Type", "eT", "Get Element 1D Type", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset", "Of", "Get Member Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start release", "⭰", "Get Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddGenericParameter("End release", "⭲", "Get Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle", "⭮A", "Get Member Orientation Angle in degrees", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Get Member Orientation Node", GH_ParamAccess.item);
      pManager.AddGenericParameter("Mesh Size", "Ms", "Get Member Mesh Size", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);

      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);

      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get it Member is Dummy", GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp", "Get the Member's original topology list referencing node IDs in Model that Model was created from", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaMember1d = new GsaMember1d();
      if (!da.GetData(0, ref gsaMember1d)) {
        return;
      }

      if (gsaMember1d == null) {
        this.AddRuntimeWarning("Member1D input is null");
      }
      GsaMember1d mem = gsaMember1d.Duplicate();

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId)) {
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
          mem.Id = id;
      }

      var ghCurve = new GH_Curve();
      if (da.GetData(2, ref ghCurve)) {
        Curve crv = null;
        if (GH_Convert.ToCurve(ghCurve, ref crv, GH_Conversion.Both)) {
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
        if (ghTyp.Value is GsaSectionGoo) {
          ghTyp.CastTo(ref section);
        }
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both)) {
            section = new GsaSection(idd);
          }
          else {
            this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
            return;
          }
        }
        mem.Section = section;
      }

      var ghGroup = new GH_Integer();
      if (da.GetData(4, ref ghGroup)) {
        if (GH_Convert.ToInt32(ghGroup, out int grp, GH_Conversion.Both))
          mem.Group = grp;
      }

      var ghInteger = new GH_Integer();
      if (da.GetData(5, ref ghInteger)) {
        if (GH_Convert.ToInt32(ghInteger, out int type, GH_Conversion.Both))
          mem.Type = (MemberType)type;
      }

      var integer = new GH_Integer();
      if (da.GetData(6, ref integer)) {
        if (GH_Convert.ToInt32(integer, out int type, GH_Conversion.Both))
          mem.Type1D = (ElementType)type;
      }

      var offset = new GsaOffset();
      if (da.GetData(7, ref offset)) {
        mem.Offset = offset;
      }

      var start = new GsaBool6();
      if (da.GetData(8, ref start)) {
        mem.ReleaseStart = start;
      }

      var end = new GsaBool6();
      if (da.GetData(9, ref end)) {
        mem.ReleaseEnd = end;
      }

      var ghAngle = new GH_Number();
      if (da.GetData(10, ref ghAngle)) {
        if (GH_Convert.ToDouble(ghAngle, out double angle, GH_Conversion.Both))
          mem.OrientationAngle = new Angle(angle, AngleUnit.Degree);
      }

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(11, ref ghTyp)) {
        var node = new GsaNode();
        if (ghTyp.Value is GsaNodeGoo) {
          ghTyp.CastTo(ref node);
          mem.OrientationNode = node;
        }
        else {
          this.AddRuntimeWarning("Unable to convert Orientation Node input to GsaNode");
        }
      }

      if (Params.Input[12].Sources.Count > 0) {
        mem.MeshSize = ((Length)Input.UnitNumber(this, da, 12, DefaultUnits.LengthUnitGeometry, true)).Meters;
        if (DefaultUnits.LengthUnitGeometry != LengthUnit.Meter)
          this.AddRuntimeRemark("Mesh size input set in [" + string.Concat(mem.MeshSize.ToString().Where(char.IsLetter)) + "]. "
                                + Environment.NewLine + "Note that this is based on your unit settings and may be changed to a different unit if you share this file or change your 'Length - geometry' unit settings. Use a UnitNumber input to use a specific unit.");
      }

      var ghBoolean = new GH_Boolean();
      if (da.GetData(13, ref ghBoolean)) {
        if (GH_Convert.ToBoolean(ghBoolean, out bool mbool, GH_Conversion.Both)) {
          if (mem.MeshWithOthers != mbool)
            mem.MeshWithOthers = mbool;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(14, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both))
          mem.Name = name;
      }

      var ghColour = new GH_Colour();
      if (da.GetData(15, ref ghColour)) {
        if (GH_Convert.ToColor(ghColour, out System.Drawing.Color col, GH_Conversion.Both))
          mem.Colour = col;
      }

      var ghDummy = new GH_Boolean();
      if (da.GetData(16, ref ghDummy)) {
        if (GH_Convert.ToBoolean(ghDummy, out bool dum, GH_Conversion.Both))
          mem.IsDummy = dum;
      }

      da.SetData(0, new GsaMember1dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.PolyCurve);
      da.SetData(3, new GsaSectionGoo(mem.Section));
      da.SetData(4, mem.Group);
      da.SetData(5, mem.Type);
      da.SetData(6, mem.Type1D);

      da.SetData(7, new GsaOffsetGoo(mem.Offset));

      da.SetData(8, mem.ReleaseStart);
      da.SetData(9, mem.ReleaseEnd);

      da.SetData(10, mem.OrientationAngle);
      da.SetData(11, new GsaNodeGoo(mem.OrientationNode));

      da.SetData(12, new GH_UnitNumber(new Length(mem.MeshSize, LengthUnit.Meter).ToUnit(DefaultUnits.LengthUnitGeometry)));
      da.SetData(13, mem.MeshWithOthers);

      da.SetData(14, mem.Name);

      da.SetData(15, mem.Colour);
      da.SetData(16, mem.IsDummy);
      da.SetData(17, mem.ApiMember.Topology);
    }
  }
}
