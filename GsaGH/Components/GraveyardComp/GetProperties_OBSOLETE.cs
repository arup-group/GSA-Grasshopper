using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI.Materials;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components.GraveyardComp {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetProperties_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("e7914f27-ea03-48e4-b7bd-a87121141f1e");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetSection;

    public GetProperties_OBSOLETE() : base("Get Model Properties", "GetProps",
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

      var materials = new List<GsaMaterialGoo>();
      materials.AddRange(
        modelGoo.Value.Materials.SteelMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      materials.AddRange(
        modelGoo.Value.Materials.ConcreteMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      materials.AddRange(
        modelGoo.Value.Materials.FrpMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      materials.AddRange(
        modelGoo.Value.Materials.AluminiumMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      materials.AddRange(
        modelGoo.Value.Materials.TimberMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      materials.AddRange(
        modelGoo.Value.Materials.GlassMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      materials.AddRange(
        modelGoo.Value.Materials.FabricMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      materials.AddRange(
        modelGoo.Value.Materials.AnalysisMaterials.Values.Select(x => new GsaMaterialGoo(x)));

      da.SetDataList(0, modelGoo.Value.Properties.Sections);
      da.SetDataList(1, modelGoo.Value.Properties.Prop2ds);
      da.SetDataList(2, modelGoo.Value.Properties.Prop3ds);
      da.SetDataList(3, materials);
    }
  }
}
