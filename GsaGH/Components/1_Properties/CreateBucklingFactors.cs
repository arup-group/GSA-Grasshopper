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
  public class CreateBucklingFactors : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("0c32af28-5057-4649-bd56-0850541c954b");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateBucklingLengthFactors;

    public CreateBucklingFactors() : base(
      $"Create {GsaBucklingLengthFactorsGoo.Name.Replace(" ", string.Empty)}",
      GsaBucklingLengthFactorsGoo.NickName.Replace(" ", string.Empty),
      $"Create a {GsaBucklingLengthFactorsGoo.Description}", CategoryName.Name(),
      SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Equivalent uniform moment factor for LTB", "fLtb",
        "Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:\r\n AISC 360: C_b \r\n AS 4100: alpha_m \r\n BS 5950: m_LT \r\n CSA S16: omega_2 \r\n EN 1993-1-1 and EN 1993-1-2: C_1 \r\n Hong Kong Code of Practice: m_LT \r\n IS 800: C_mLT \r\n SANS 10162-1: omega_2",
        GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var fls = new GsaBucklingLengthFactors();
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

      da.SetData(0, new GsaBucklingLengthFactorsGoo(fls));
    }
  }
}
