using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  public class GetLineResultsInfo : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("89f754b4-48a1-4cb8-980b-9ac7c51e101e");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result1dInfo;

    public GetLineResultsInfo() : base("LineResultInfo",
      "LnResInfo",
      "Get Element 1D Contour Result values",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result Line", "L", "Contoured Line segments with result values", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddLineParameter("Line", "L", "Line Segment", GH_ParamAccess.item);
      pManager.AddGenericParameter("Result Value Start", "R1", "Result value at Segment Start as UnitNumber", GH_ParamAccess.item);
      pManager.AddGenericParameter("Result Value End", "R2", "Result value at Segment End as UnitNumber", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      LineResultGoo res = null;
      DA.GetData(0, ref res);
      DA.SetData(0, res.Value);
      DA.SetData(1, new GH_UnitNumber(res._result1));
      DA.SetData(2, new GH_UnitNumber(res._result2));
    }
  }
}
