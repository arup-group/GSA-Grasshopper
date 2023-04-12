using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit Buckling Length Factors and ouput the information
  /// </summary>
  public class EditBucklingFactors : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("6440b34e-d787-48cc-8e95-c07c6217e40a");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditBucklingLengthFactors;

    public EditBucklingFactors() : base("Edit " + GsaBucklingLengthFactorsGoo.Name,
          "BucklingFactorsEdit",
      "Modify GSA Buckling Length Factors",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
      pManager.AddNumberParameter("Factor Lsy",
        "fLy",
        "Moment Amplification Factor, Strong Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz",
        "fLz",
        "Moment Amplification Factor, Weak Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Equivalent uniform moment factor for LTB",
        "fLtb",
        "Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:\r\n AISC 360: C_b \r\n AS 4100: alpha_m \r\n BS 5950: m_LT \r\n CSA S16: omega_2 \r\n EN 1993-1-1 and EN 1993-1-2: C_1 \r\n Hong Kong Code of Practice: m_LT \r\n IS 800: C_mLT \r\n SANS 10162-1: omega_2",
        GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i]
          .Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
      pManager.AddNumberParameter("Factor Lsy",
        "fLy",
        "Moment Amplification Factor, Strong Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz",
        "fLz",
        "Moment Amplification Factor, Weak Axis",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Equivalent uniform moment factor for LTB",
        "fLtb",
        "Override the automatically calculated factor to account for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.  This override is applied to the following variable for each design code:\r\n AISC 360: C_b \r\n AS 4100: alpha_m \r\n BS 5950: m_LT \r\n CSA S16: omega_2 \r\n EN 1993-1-1 and EN 1993-1-2: C_1 \r\n Hong Kong Code of Practice: m_LT \r\n IS 800: C_mLT \r\n SANS 10162-1: omega_2",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaFls = new GsaBucklingLengthFactors();
      var fls = new GsaBucklingLengthFactors();
      if (da.GetData(0, ref gsaFls)) {
        fls = gsaFls.Duplicate();
      }

      if (fls != null) {
        var y = new GH_Number();
        if (da.GetData(1, ref y)) {
          if (GH_Convert.ToDouble(y, out double yy, GH_Conversion.Both)) {
            fls.MomentAmplificationFactorStrongAxis = yy;
          }
        }

        var z = new GH_Number();
        if (da.GetData(2, ref z)) {
          if (GH_Convert.ToDouble(z, out double zz, GH_Conversion.Both)) {
            fls.MomentAmplificationFactorWeakAxis = zz;
          }
        }

        var lt = new GH_Number();
        if (da.GetData(3, ref lt)) {
          if (GH_Convert.ToDouble(lt, out double ltb, GH_Conversion.Both)) {
            fls.EquivalentUniformMomentFactor = ltb;
          }
        }

        da.SetData(0, new GsaBucklingLengthFactorsGoo(fls));
        da.SetData(1, fls.MomentAmplificationFactorStrongAxis);
        da.SetData(2, fls.MomentAmplificationFactorWeakAxis);
        da.SetData(3, fls.EquivalentUniformMomentFactor);
      }
      else {
        this.AddRuntimeError("Buckling Factors is Null");
      }
    }
  }
}
