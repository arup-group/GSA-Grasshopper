using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to retrieve Grid Lines from a GSA model
  /// </summary>
  public class GetModelGridLines : GH_OasysComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("9def51cc-6166-4c6e-9ca9-2668a08f3dd2");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelGridLines;

    public GetModelGridLines() : base("Get Model Grid Lines", "GetGridLines",
      "Get Grid Lines from a GSA model.", CategoryName.Name(), SubCategoryName.Cat0()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA Model containing Grid Lines", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaGridLineParameter(), "GSA Grid Line", "GL", "Grid Lines from GSA Model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);
      List<GsaGridLine> gridLines = modelGoo.Value.GetGridLines();
      da.SetDataList(0, gridLines.Select(x => new GsaGridLineGoo(x)));
    }
  }
}
