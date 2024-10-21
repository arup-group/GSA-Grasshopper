using System;
using System.Drawing;
using System.Linq;

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

using OasysUnits;

using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 1D Member
  /// </summary>
  public class Edit1dMember : Section3dPreviewComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("32e744e5-7352-4308-81d0-13bf06db5e82");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit1dMember;
    private AngleUnit _angleUnit = AngleUnit.Radian;

    public Edit1dMember() : base("Edit 1D Member", "Mem1dEdit", "Modify GSA 1D Member",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (Params.Input[10] is Param_Number angleParameter) {
        _angleUnit = angleParameter.UseDegrees ? AngleUnit.Degree : AngleUnit.Radian;
      }
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public void VariableParameterMaintenance() { }

    public override bool Read(GH_IReader reader) {
      bool flag = base.Read(reader);
      if (Params.Input[3].Name == new GsaSectionParameter().Name) {
        Params.ReplaceInputParameter(new GsaPropertyParameter(), 3, true);
        Params.ReplaceOutputParameter(new GsaPropertyParameter(), 3);
      }

      return flag;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMember1dParameter(), GsaMember1dGoo.Name,
        GsaMember1dGoo.NickName,
        GsaMember1dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaMember1dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID",
        "Set Member Number. If ID is set it will replace any existing 1D Member in the model.",
        GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.AddParameter(new GsaPropertyParameter(), "Section", "PB", "Set new Section or Spring Property.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Group", "Gr", "Set Member 1D Group",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT",
        "Set 1D Member Type" + Environment.NewLine
        + "Default is 0: Generic 1D - Accepted inputs are:" + Environment.NewLine + "2: Beam"
        + Environment.NewLine + "3: Column" + Environment.NewLine + "6: Cantilever"
        + Environment.NewLine + "8: Compos" + Environment.NewLine + "9: Pile" + Environment.NewLine
        + "11: Void cutter", GH_ParamAccess.item);
      pManager.AddTextParameter("1D Element Type", "eT",
        "Set Element 1D Type" + Environment.NewLine + "Accepted inputs are:" + Environment.NewLine
        + "1: Bar" + Environment.NewLine + "2: Beam" + Environment.NewLine + "3: Spring"
        + Environment.NewLine + "9: Link" + Environment.NewLine + "10: Cable" + Environment.NewLine
        + "19: Spacer" + Environment.NewLine + "20: Strut" + Environment.NewLine + "21: Tie"
        + Environment.NewLine + "23: Rod" + Environment.NewLine + "24: Damper",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Member Offset",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰",
        "Set Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲",
        "Set Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Automatic Offset End 1", "AO1",
        "Set Automatic Offset at End 1 of Member", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Automatic Offset End 2", "AO2",
        "Set Automatic Offset at End 2 of Member", GH_ParamAccess.item);
      pManager.AddAngleParameter("Orientation Angle", "⭮A", "Set Member Orientation Angle",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeParameter(), "Orientation Node", "⭮N",
        "Set Member Orientation Node", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Set Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaEffectiveLengthOptionsParameter(),
        "Set " + GsaEffectiveLengthOptionsGoo.Name, GsaEffectiveLengthOptionsGoo.NickName,
        GsaEffectiveLengthOptionsGoo.Description, GH_ParamAccess.item);
      pManager.AddTextParameter("Member1d Name", "Na", "Set Name of Member1d", GH_ParamAccess.item);
      pManager.AddColourParameter("Member1d Colour", "Co", "Set Member 1D Colour",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember1dParameter(), GsaMember1dGoo.Name,
        GsaMember1dGoo.NickName, GsaMember1dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member1d Number", "ID", "Get Member Number",
        GH_ParamAccess.item);
      pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddGenericParameter("Section", "PB", "Get Section or Spring Property",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Type", "mT", "Get 1D Member Type", GH_ParamAccess.item);
      pManager.AddTextParameter("1D Element Type", "eT", "Get Element 1D Type",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Member Offset",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "Start release", "⭰",
        "Get Release (Bool6) at Start of Member", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "End release", "⭲",
        "Get Release (Bool6) at End of Member", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Automatic Offset End 1", "AO1",
        "Get Automatic Offset at End 1 of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Offset Length 1", "Ol1",
        "Get Automatic Offset Length at End 1 of Member", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Automatic Offset End 2", "AO2",
        "Get Automatic Offset at End 2 of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Offset Length 2", "Ol2",
        "Get Automatic Offset Length at End 2 of Member", GH_ParamAccess.item);
      pManager.AddNumberParameter("Orientation Angle", "⭮A",
        "Get Member Orientation Angle in radians", GH_ParamAccess.item);
      pManager.AddGenericParameter("Orientation Node", "⭮N", "Get Member Orientation Node",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Get Member Mesh Size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaEffectiveLengthOptionsParameter(),
        "Get " + GsaEffectiveLengthOptionsGoo.Name, GsaEffectiveLengthOptionsGoo.NickName,
        GsaEffectiveLengthOptionsGoo.Description, GH_ParamAccess.item);
      pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);

      pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Dummy Member", "Dm", "Get it Member is Dummy",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Topology", "Tp",
        "Get the Member's original topology list referencing node IDs in Model that Model was created from",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var mem = new GsaMember1d();

      GsaMember1dGoo member1dGoo = null;
      if (da.GetData(0, ref member1dGoo)) {
        mem = new GsaMember1d(member1dGoo.Value);
      }

      GH_String ghstring = null;
      if (da.GetData(6, ref ghstring)) {
        if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both)) {
          mem.ApiMember.Type1D = (ElementType)typeInt;
        } else {
          try {
            mem.ApiMember.Type1D = Mappings.GetElementType(ghstring.Value);
          } catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Element Type");
          }
        }
      }

      GsaPropertyGoo sectionGoo = null;
      if (da.GetData(3, ref sectionGoo)) {
        switch (sectionGoo.Value) {
          case GsaSection section:
            if (section.IsReferencedById && mem.ApiMember.Type1D == ElementType.SPRING) {
              mem.Section = null;
              mem.SpringProperty = new GsaSpringProperty(section.Id);
            } else {
              if (mem.ApiMember.Type1D == ElementType.SPRING) {
                this.AddRuntimeError("PB input must be a SpringProperty");
                return;
              }
              mem.Section = section;
              mem.SpringProperty = null;
            }

            break;

          case GsaSpringProperty springProperty:
            mem.ApiMember.Type1D = ElementType.SPRING;
            this.AddRuntimeRemark("ElementType changed to Spring");
            mem.Section = null;
            mem.SpringProperty = springProperty;
            break;
        }
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        mem.Id = id;
      }

      GH_Curve ghcrv = null;
      if (da.GetData(2, ref ghcrv)) {
        Curve crv = null;
        if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both)) {
          mem.UpdateGeometry(crv);
        }
      }

      int group = 0;
      if (da.GetData(4, ref group)) {
        mem.ApiMember.Group = group;
      }

      ghstring = null;
      if (da.GetData(5, ref ghstring)) {
        if (GH_Convert.ToInt32(ghstring, out int typeInt, GH_Conversion.Both)) {
          mem.ApiMember.Type = (MemberType)typeInt;
        } else {
          try {
            mem.ApiMember.Type = Mappings.GetMemberType(ghstring.Value);
          } catch (ArgumentException) {
            this.AddRuntimeError("Unable to change Member Type");
          }
        }
      }

      GsaOffsetGoo offset = null;
      if (da.GetData(7, ref offset)) {
        mem.Offset = offset.Value;
      }

      GsaBool6Goo start = null;
      if (da.GetData(8, ref start)) {
        mem.ReleaseStart = start.Value;
      }

      GsaBool6Goo end = null;
      if (da.GetData(9, ref end)) {
        mem.ReleaseEnd = end.Value;
      }

      bool autoOffset1 = false;
      if (da.GetData(10, ref autoOffset1)) {
        mem.ApiMember.AutomaticOffset.End1 = autoOffset1;
      }

      bool autoOffset2 = false;
      if (da.GetData(11, ref autoOffset2)) {
        mem.ApiMember.AutomaticOffset.End2 = autoOffset2;
      }

      double angle = 0;
      if (da.GetData(12, ref angle)) {
        mem.OrientationAngle = new Angle(angle, _angleUnit);
      }

      GsaNodeGoo nodeGoo = null;
      if (da.GetData(13, ref nodeGoo)) {
        mem.OrientationNode = nodeGoo.Value;
      }

      double meshSize = 0;
      if (da.GetData(14, ref meshSize)) {
        mem.ApiMember.MeshSize = meshSize;
      }

      bool intersector = false;
      if (da.GetData(15, ref intersector)) {
        mem.ApiMember.IsIntersector = intersector;
      }

      GsaEffectiveLengthOptionsGoo effLengthGoo = null;
      if (da.GetData(16, ref effLengthGoo)) {
        GsaBucklingFactors blf = effLengthGoo.Value.BucklingFactors;
        mem.ApiMember.MomentAmplificationFactorStrongAxis
          = blf.MomentAmplificationFactorStrongAxis;
        mem.ApiMember.MomentAmplificationFactorWeakAxis
          = blf.MomentAmplificationFactorWeakAxis;
        mem.ApiMember.EquivalentUniformMomentFactor
          = blf.EquivalentUniformMomentFactor;
        mem.ApiMember.EffectiveLength = effLengthGoo.Value.EffectiveLength;
      }

      string name = string.Empty;
      if (da.GetData(17, ref name)) {
        mem.ApiMember.Name = name;
      }

      Color colour = Color.Empty;
      if (da.GetData(18, ref colour)) {
        mem.ApiMember.Colour = colour;
      }

      bool dummy = false;
      if (da.GetData(19, ref dummy)) {
        mem.ApiMember.IsDummy = dummy;
      }

      if ((mem.ApiMember.Type1D == ElementType.BAR || mem.ApiMember.Type1D == ElementType.TIE
        || mem.ApiMember.Type1D == ElementType.STRUT) && mem.ApiMember.MeshSize != 0) {
        this.AddRuntimeWarning($"Element type is {mem.ApiMember.Type1D} and mesh size is not zero. " +
          Environment.NewLine + $"This may cause model instabilities.");
      }

      if (Preview3dSection || mem.Section3dPreview != null) {
        mem.CreateSection3dPreview();
      }

      mem.UpdateReleasesPreview();

      da.SetData(0, new GsaMember1dGoo(mem));
      da.SetData(1, mem.Id);
      da.SetData(2, mem.PolyCurve);
      if (mem.ApiMember.Type1D == ElementType.SPRING) {
        da.SetData(3, new GsaSpringPropertyGoo(mem.SpringProperty));
      } else {
        da.SetData(3, new GsaSectionGoo(mem.Section));
      }
      da.SetData(4, mem.ApiMember.Group);
      da.SetData(5,
        Mappings._memberTypeMapping.FirstOrDefault(x => x.Value == mem.ApiMember.Type).Key);
      da.SetData(6,
        Mappings._elementTypeMapping.FirstOrDefault(x => x.Value == mem.ApiMember.Type1D).Key);
      da.SetData(7, new GsaOffsetGoo(mem.Offset));
      da.SetData(8, new GsaBool6Goo(mem.ReleaseStart));
      da.SetData(9, new GsaBool6Goo(mem.ReleaseEnd));
      da.SetData(10, mem.ApiMember.AutomaticOffset.End1);
      da.SetData(11, mem.ApiMember.AutomaticOffset.X1);
      da.SetData(12, mem.ApiMember.AutomaticOffset.End2);
      da.SetData(13, mem.ApiMember.AutomaticOffset.X2);
      da.SetData(14, mem.OrientationAngle.Radians);
      da.SetData(15, new GsaNodeGoo(mem.OrientationNode));
      da.SetData(16, mem.ApiMember.MeshSize);
      da.SetData(17, mem.ApiMember.IsIntersector);
      var effLength = new GsaEffectiveLengthOptions(mem) {
        BucklingFactors = new GsaBucklingFactors(mem)
      };
      da.SetData(18, new GsaEffectiveLengthOptionsGoo(effLength));
      da.SetData(19, mem.ApiMember.Name);
      da.SetData(20, (Color)mem.ApiMember.Colour);
      da.SetData(21, mem.ApiMember.IsDummy);
      da.SetData(22, mem.ApiMember.Topology);
    }
  }
}
