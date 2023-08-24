using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class GetProperties : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("f5926fb3-06e5-4b18-b037-6234fff16586");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelProperties;

    public GetProperties() : base("Get Model Properties", "GetProps",
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
      pManager.AddParameter(new GsaProp2dParameter(), "2D Properties", "PA",
        "2D Properties from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp3dParameter(), "3D Properties", "PV",
        "3D Properties from GSA Model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      da.SetDataList(0, modelGoo.Value.Properties.Sections.Values);
      da.SetDataList(1, modelGoo.Value.Properties.Prop2ds.Values);
      da.SetDataList(2, modelGoo.Value.Properties.Prop3ds.Values);
    }
  }
}
