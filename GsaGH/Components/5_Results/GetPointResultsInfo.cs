﻿using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;

namespace GsaGH.Components {
  public class GetPointResultsInfo : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("9bcce796-5524-40ab-a779-2947af9b18d2");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Result0dInfo;

    public GetPointResultsInfo() : base("PointResultsInfo",
      "PtResInfo",
      "Get Node Contour Result values",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Result Point", "P", "Contoured Points with result values", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPointParameter("Point", "P", "Location of the Node", GH_ParamAccess.item);
      pManager.AddGenericParameter("Result Value", "R", "Result value as UnitNumber", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      PointResultGoo res = null;
      da.GetData(0, ref res);
      da.SetData(0, res.Value);
      da.SetData(1, new GH_UnitNumber(res.Result));
    }
  }
}
