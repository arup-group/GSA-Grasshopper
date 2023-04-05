using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using GsaAPI;
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
    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaModel = new GsaModel();
      if (!da.GetData(0, ref gsaModel))
        return;

      Model model = gsaModel.Model;

      List<GsaSectionGoo> sections = Helpers.Import.Properties.GetSections(model.Sections(),
        model.AnalysisMaterials(),
        model.SectionModifiers());
      List<GsaProp2dGoo> prop2Ds
        = Helpers.Import.Properties.GetProp2ds(model.Prop2Ds(),
          model.AnalysisMaterials(),
          model.Axes());
      List<GsaProp3dGoo> prop3Ds
        = Helpers.Import.Properties.GetProp3ds(model.Prop3Ds(), model.AnalysisMaterials());

      da.SetDataList(0, sections);
      da.SetDataList(1, prop2Ds);
      da.SetDataList(2, prop3Ds);
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("f5926fb3-06e5-4b18-b037-6234fff16586");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetSection;

    public GetProperties() : base("Get Model Properties",
      "GetProps",
      "Get Sections, 2D Properties and Springs from GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
      => pManager.AddParameter(new GsaModelParameter(),
        "GSA Model",
        "GSA",
        "GSA model containing some properties",
        GH_ParamAccess.item);

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(),
        "Sections",
        "PB",
        "Section Properties from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter(),
        "2D Properties",
        "PA",
        "2D Properties from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp3dParameter(),
        "3D Properties",
        "PV",
        "3D Properties from GSA Model",
        GH_ParamAccess.list);
    }

    #endregion
  }
}
