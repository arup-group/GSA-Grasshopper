using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetProperties : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("e7914f27-ea03-48e4-b7bd-a87121141f1e");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetSection;

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
      pManager.AddParameter(new GsaMaterialParameter(), "Custom Materials", "Mat",
        "Custom Materials from GSA Model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      (List<GsaMaterialGoo> materials, List<GsaSectionGoo> sections,
      List<GsaProp2dGoo> prop2ds, List<GsaProp3dGoo> prop3ds) =
        Helpers.Import.Properties.GetProperties(modelGoo.Value.Model);

      da.SetDataList(0, sections);
      da.SetDataList(1, prop2ds);
      da.SetDataList(2, prop3ds);
      da.SetDataList(3, materials);
    }
  }
}
