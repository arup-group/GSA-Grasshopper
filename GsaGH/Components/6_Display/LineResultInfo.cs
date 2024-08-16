using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;

namespace GsaGH.Components {
  public class LineResultInfo : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("89f754b4-48a1-4cb8-980b-9ac7c51e101e");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.LineResultInfo;

    public LineResultInfo() : base("Line Result Info", "LnResInfo",
      "Get Element 1D Contour Result values", CategoryName.Name(), SubCategoryName.Cat6()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Result Line", "L", "Contoured Line segments with result values",
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddLineParameter("Line", "L", "Line Segment", GH_ParamAccess.item);
      pManager.AddGenericParameter("Result Value Start", "R1",
        "Result value at Segment Start as UnitNumber", GH_ParamAccess.item);
      pManager.AddGenericParameter("Result Value End", "R2",
        "Result value at Segment End as UnitNumber", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      LineResultGoo res = null;
      da.GetData(0, ref res);
      da.SetData(0, res.Value);
      da.SetData(1, new GH_UnitNumber(res.Result1));
      da.SetData(2, new GH_UnitNumber(res.Result2));
    }
  }
}
