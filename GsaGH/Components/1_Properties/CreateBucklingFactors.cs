﻿using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a new Buckling Length Factors
  /// </summary>
  public class CreateBucklingFactors : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("0c32af28-5057-4649-bd56-0850541c954b");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateBucklingLengthFactors;

    public CreateBucklingFactors() : base("Create " + GsaBucklingLengthFactorsGoo.Name.Replace(" ", string.Empty),
      GsaBucklingLengthFactorsGoo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaBucklingLengthFactorsGoo.Description,
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddNumberParameter("Factor Lsy", "fLy", "Moment Amplification Factor, Strong Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Lsz", "fLz", "Moment Amplification Factor, Weak Axis", GH_ParamAccess.item);
      pManager.AddNumberParameter("Factor Ltb", "fLtb", "Lateral Torsional Buckling Factor", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaBucklingLengthFactorsParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var fls = new GsaBucklingLengthFactors();
      double input = 1;
      if (da.GetData(0, ref input))
        fls.MomentAmplificationFactorStrongAxis = input;
      double optional = input;
      da.GetData(1, ref optional);
      fls.MomentAmplificationFactorWeakAxis = optional;
      da.GetData(2, ref input);
      fls.LateralTorsionalBucklingFactor = input;
      da.SetData(0, new GsaBucklingLengthFactorsGoo(fls));
    }
  }
}
