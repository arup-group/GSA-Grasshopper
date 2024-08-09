using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class GetModelProperties : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("470103dd-879c-431a-8e89-34a50f46e63d");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelProperties;

    public GetModelProperties() : base("Get Model Properties", "GetProps",
      "Get Sections, 2D, 3D and Spring Properties from a GSA model", CategoryName.Name(),
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
      pManager.AddParameter(new GsaSpringPropertyParameter(), "Spring Properties", "PS",
        "Spring Properties from GSA Model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      da.SetDataList(0, modelGoo.Value.Sections.Values);
      da.SetDataList(1, modelGoo.Value.Prop2ds.Values);
      da.SetDataList(2, modelGoo.Value.Prop3ds.Values);
      da.SetDataList(3, modelGoo.Value.SpringProps.Values);
    }
  }
}
