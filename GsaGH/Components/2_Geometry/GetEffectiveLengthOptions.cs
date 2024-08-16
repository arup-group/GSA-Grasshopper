using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  public class GetEffectiveLengthOptions : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("363fe80b-d18d-4d5a-88ad-dd3ab72595d9");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetEffectiveLengthOptions;

    public GetEffectiveLengthOptions() : base("Get Effective Length Options", "EffectiveLengthOptions", "Get information of a 1D Member's Design Options for Effective Length, Restraints and Buckling Factors", CategoryName.Name(),
      SubCategoryName.Cat2()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaEffectiveLengthOptionsParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Calculation Option", "CO", "The option of the Effective Length " +
        "calculation.", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Restraint Start", "ER1",
        "Restraint Description Syntax for Member End Restraint at Member Start", GH_ParamAccess.item);
      pManager.AddTextParameter("Member Restraint Start", "ER2",
        "Restraint Description Syntax for Member End Restraint at Member End", GH_ParamAccess.item);
      pManager.AddTextParameter("Restraint Along Member", "RAM",
        "The internal continous restraint along the member.", GH_ParamAccess.item);
      pManager.AddTextParameter("Intermediate Bracing Point Restraints", "IBR",
        "The internal restraint at intermediate bracing points of the member.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Length About Y", "Lsy",
        "The user-defined effective length about y.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Length About Z", "Lsz",
        "The user-defined effective length about y.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Effective Length LTB", "Ltb",
        "The user-defined effective length for lateral torsional buckling.", GH_ParamAccess.item);
      pManager.AddNumberParameter("Destabilising Load Height", "h",
        "Destabilising Load Height in model units", GH_ParamAccess.item);
      pManager.AddTextParameter("Load Position", "LP", "The destabilising load height is relative to this reference position.", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Equivalent uniform moment factor for LTB", "fLtb",
        "Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:\r\n AISC 360: C_b \r\n AS 4100: alpha_m \r\n BS 5950: m_LT \r\n CSA S16: omega_2 \r\n EN 1993-1-1 and EN 1993-1-2: C_1 \r\n Hong Kong Code of Practice: m_LT \r\n IS 800: C_mLT \r\n SANS 10162-1: omega_2",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaEffectiveLengthOptionsGoo effLengthGoo = null;
      da.GetData(0, ref effLengthGoo);
      GsaEffectiveLengthOptions leff = effLengthGoo.Value;
      switch (leff.EffectiveLength) {
        case EffectiveLengthFromEndAndInternalRestraint internalRes:
          da.SetData(0, "InternalRestraints");
          da.SetData(1, MemberEndRestraintFactory.MemberEndRestraintToString(internalRes.End1));
          da.SetData(2, MemberEndRestraintFactory.MemberEndRestraintToString(internalRes.End2));
          da.SetData(3, internalRes.RestraintAlongMember.ToString());
          da.SetData(4, internalRes.RestraintAtBracedPoints.ToString());
          break;

        case EffectiveLengthFromEndRestraintAndGeometry auto:
          da.SetData(0, "Automatic");
          da.SetData(1, MemberEndRestraintFactory.MemberEndRestraintToString(auto.End1));
          da.SetData(2, MemberEndRestraintFactory.MemberEndRestraintToString(auto.End2));
          break;

        case EffectiveLengthFromUserSpecifiedValue user:
          da.SetData(0, "UserSpecified");
          da.SetData(5, ConvertEffectiveLengthAttribute(user.EffectiveLengthAboutY));
          da.SetData(6, ConvertEffectiveLengthAttribute(user.EffectiveLengthAboutZ));
          da.SetData(7, ConvertEffectiveLengthAttribute(user.EffectiveLengthLaterialTorsional));
          break;
      }

      da.SetData(8, leff.EffectiveLength.DestablisingLoad);
      da.SetData(9, leff.EffectiveLength.DestablisingLoadPositionRelativeTo.ToString());

      da.SetData(10, leff.BucklingFactors.MomentAmplificationFactorStrongAxis);
      da.SetData(11, leff.BucklingFactors.MomentAmplificationFactorWeakAxis);
      da.SetData(12, leff.BucklingFactors.EquivalentUniformMomentFactor);
    }

    private string ConvertEffectiveLengthAttribute(EffectiveLengthAttribute leffAttribute) {
      if (leffAttribute.Option == EffectiveLengthOptionType.Absolute) {
        return new GH_Number(leffAttribute.Value).ToString();
      }

      return new GH_UnitNumber(new Ratio(leffAttribute.Value, RatioUnit.DecimalFraction).
        ToUnit(RatioUnit.Percent)).ToString();
    }
  }
}
