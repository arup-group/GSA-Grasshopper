using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit Buckling Length Factors and ouput the information
  /// </summary>
  public class EditBucklingFactors : GH_OasysComponent {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6440b34e-d787-48cc-8e95-c07c6217e40a");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.EditBucklingLengthFactors;

    public EditBucklingFactors() : base("Edit " + GsaBucklingLengthFactorsGoo.Name,
      "BucklingFactorsEdit",
      "Modify GSA Buckling Length Factors",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Ltb", "fLtb", "Lateral Torsional Buckling Factor", GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Ltb", "fLtb", "Lateral Torsional Buckling Factor", GH_ParamAccess.item);
      pManager.AddGenericParameter("Buckling L Y-Y", "Lsy", "Strong Axis Buckling Length", GH_ParamAccess.item);
      pManager.AddGenericParameter("Buckling L Z-Z", "Lsz", "Weak Axis Buckling Length", GH_ParamAccess.item);
      pManager.AddGenericParameter("LTB Length", "Ltb", "Lateral Torsional Buckling Length", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaFls = new GsaBucklingLengthFactors();
      var fls = new GsaBucklingLengthFactors();
      if (da.GetData(0, ref gsaFls)) {
        fls = gsaFls.Duplicate();
      }

      if (fls != null) {
        // #### inputs ####
        var y = new GH_Number();
        if (da.GetData(1, ref y)) {
          if (GH_Convert.ToDouble(y, out double yy, GH_Conversion.Both))
            fls.MomentAmplificationFactorStrongAxis = yy;
        }

        var z = new GH_Number();
        if (da.GetData(2, ref z)) {
          if (GH_Convert.ToDouble(z, out double zz, GH_Conversion.Both))
            fls.MomentAmplificationFactorWeakAxis = zz;
        }

        var lt = new GH_Number();
        if (da.GetData(3, ref lt)) {
          if (GH_Convert.ToDouble(lt, out double ltb, GH_Conversion.Both))
            fls.LateralTorsionalBucklingFactor = ltb;
        }

        //#### outputs ####
        da.SetData(0, new GsaBucklingLengthFactorsGoo(fls));
        da.SetData(1, fls.MomentAmplificationFactorStrongAxis);
        da.SetData(2, fls.MomentAmplificationFactorWeakAxis);
        da.SetData(3, fls.LateralTorsionalBucklingFactor);
        da.SetData(4, (fls.LengthIsSet && fls.MomentAmplificationFactorStrongAxis.HasValue) ? new GH_UnitNumber(fls.Length * fls.MomentAmplificationFactorStrongAxis) : null);
        da.SetData(5, (fls.LengthIsSet && fls.MomentAmplificationFactorWeakAxis.HasValue) ? new GH_UnitNumber(fls.Length * fls.MomentAmplificationFactorWeakAxis) : null);
        da.SetData(6, (fls.LengthIsSet && fls.LateralTorsionalBucklingFactor.HasValue) ? new GH_UnitNumber(fls.Length * fls.LateralTorsionalBucklingFactor) : null);
      }
      else
        this.AddRuntimeError("Buckling Factors is Null");
    }
  }
}
