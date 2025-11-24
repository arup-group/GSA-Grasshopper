using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Buckling Length Factors
  /// </summary>
  public class CreateBucklingFactors_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("0c32af28-5057-4649-bd56-0850541c954b");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateBucklingFactors;

    public CreateBucklingFactors_OBSOLETE() : base(
      "Create " + GsaEffectiveLengthOptionsGoo.Name,
      GsaEffectiveLengthOptionsGoo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaEffectiveLengthOptionsGoo.Description, CategoryName.Name(),
      SubCategoryName.Cat2()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Equivalent uniform moment factor for LTB", "fLtb",
        $"Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:{Environment.NewLine} AISC 360: C_b {Environment.NewLine} AS 4100: alpha_m {Environment.NewLine} BS 5950: m_LT {Environment.NewLine} CSA S16: omega_2 {Environment.NewLine} EN 1993-1-1 and EN 1993-1-2: C_1 {Environment.NewLine} Hong Kong Code of Practice: m_LT {Environment.NewLine} IS 800: C_mLT {Environment.NewLine} SANS 10162-1: omega_2",
        GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaEffectiveLengthOptionsParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      this.AddRuntimeError("This component is obsolete and will be removed in future versions. " +
        "\nThis component has been replaced by Design Properties component, please update " +
        "your script to use that instead.");
      var fls = new GsaBucklingFactors();
      double? input = null;
      if (da.GetData(0, ref input)) {
        fls.MomentAmplificationFactorStrongAxis = input;
      }

      if (da.GetData(1, ref input)) {
        fls.MomentAmplificationFactorWeakAxis = input;
      }

      if (da.GetData(2, ref input)) {
        fls.EquivalentUniformMomentFactor = input;
      }

      var leff = new GsaEffectiveLengthOptions(new GsaMember1D()) {
        BucklingFactors = fls
      };

      da.SetData(0, new GsaEffectiveLengthOptionsGoo(leff));
    }
  }
}
