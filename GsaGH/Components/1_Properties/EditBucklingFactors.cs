using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Eto.Forms;
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
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to edit Buckling Length Factors and ouput the information
    /// </summary>
    public class EditBucklingFactors : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6440b34e-d787-48cc-8e95-c07c6217e40a");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditBucklingLengthFactors;

    public EditBucklingFactors() : base("Edit " + GsaBucklingLengthFactorsGoo.Name,
      "BucklingFactorsEdit",
      "Modify GSA Buckling Length Factors",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Ltb", "fLtb", "Lateral Torsional Buckling Factor", GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Ltb", "fLtb", "Lateral Torsional Buckling Factor", GH_ParamAccess.item);
      pManager.AddGenericParameter("Buckling L Y-Y", "Lsy", "Strong Axis Buckling Length", GH_ParamAccess.item);
      pManager.AddGenericParameter("Buckling L Z-Z", "Lsz", "Weak Axis Buckling Length", GH_ParamAccess.item);
      pManager.AddGenericParameter("LTB Length", "Ltb", "Lateral Torsional Buckling Length", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaBucklingLengthFactors gsafls = new GsaBucklingLengthFactors();
      GsaBucklingLengthFactors fls = new GsaBucklingLengthFactors();
      if (DA.GetData(0, ref gsafls))
      {
        fls = gsafls.Duplicate();
      }

      if (fls != null)
      {
        // #### inputs ####
        GH_Number y = new GH_Number();
        if (DA.GetData(1, ref y))
        {
          if (GH_Convert.ToDouble(y, out double yy, GH_Conversion.Both))
            fls.MomentAmplificationFactorStrongAxis = yy;
        }

        GH_Number z = new GH_Number();
        if (DA.GetData(2, ref z))
        {
          if (GH_Convert.ToDouble(z, out double zz, GH_Conversion.Both))
            fls.MomentAmplificationFactorWeakAxis = zz;
        }

        GH_Number lt = new GH_Number();
        if (DA.GetData(3, ref lt))
        {
          if (GH_Convert.ToDouble(lt, out double ltb, GH_Conversion.Both))
            fls.LateralTorsionalBucklingFactor = ltb;
        }

        //#### outputs ####
        DA.SetData(0, new GsaBucklingLengthFactorsGoo(fls));
        DA.SetData(1, fls.MomentAmplificationFactorStrongAxis);
        DA.SetData(2, fls.MomentAmplificationFactorWeakAxis);
        DA.SetData(3, fls.LateralTorsionalBucklingFactor);
        DA.SetData(4, (fls.LengthIsSet && fls.MomentAmplificationFactorStrongAxis.HasValue) ? new GH_UnitNumber(fls.Length * fls.MomentAmplificationFactorStrongAxis) : null);
        DA.SetData(5, (fls.LengthIsSet && fls.MomentAmplificationFactorWeakAxis.HasValue) ? new GH_UnitNumber(fls.Length * fls.MomentAmplificationFactorWeakAxis) : null);
        DA.SetData(6, (fls.LengthIsSet && fls.LateralTorsionalBucklingFactor.HasValue) ? new GH_UnitNumber(fls.Length * fls.LateralTorsionalBucklingFactor) : null);
      }
      else
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Buckling Factors is Null");
    }
  }
}
