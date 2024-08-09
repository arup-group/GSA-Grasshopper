using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class GetModelProperties_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("f5926fb3-06e5-4b18-b037-6234fff16586");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelProperties;

    public GetModelProperties_OBSOLETE() : base("Get Model Properties", "GetProps",
      "Get Sections, 2D Properties and Springs from GSA model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some properties", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), "Sections", "PB",
        "Section Properties from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProperty2dParameter(), "2D Properties", "PA",
        "2D Properties from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProperty3dParameter(), "3D Properties", "PV",
        "3D Properties from GSA Model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      da.SetDataList(0, modelGoo.Value.Sections.Values);
      da.SetDataList(1, modelGoo.Value.Prop2ds.Values);
      da.SetDataList(2, modelGoo.Value.Prop3ds.Values);
    }
  }
}
