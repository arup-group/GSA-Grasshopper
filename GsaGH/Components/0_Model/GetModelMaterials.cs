using System;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class GetModelMaterials : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("c5f1ff5e-ae9d-4f85-b765-1b139cd10bcd");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelMaterials;

    public GetModelMaterials() : base("Get Model Materials", "GetMats",
      "Get Standard and Custom Materials from a GSA model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some materials", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), "Steel", "SMat",
        "Standard Steel Materials from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMaterialParameter(), "Concrete", "CMat",
        "Standard Concrete Materials from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMaterialParameter(), "FRP", "PMat",
        "Standard FRP Materials from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMaterialParameter(), "Aluminium", "AMat",
        "Standard Aluminium Materials from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMaterialParameter(), "Timber", "TMat",
        "Standard Timber Materials from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMaterialParameter(), "Glass", "GMat",
        "Standard Glass Materials from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMaterialParameter(), "Fabric", "FMat",
        "Standard Fabric Materials from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMaterialParameter(), "Custom", "Cust",
        "Custom Analysis Materials from GSA Model", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);
      GsaMaterials materials = modelGoo.Value.Materials;
      da.SetDataList(0, materials.SteelMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      da.SetDataList(1, materials.ConcreteMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      da.SetDataList(2, materials.FrpMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      da.SetDataList(3, materials.AluminiumMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      da.SetDataList(4, materials.TimberMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      da.SetDataList(5, materials.GlassMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      da.SetDataList(6, materials.FabricMaterials.Values.Select(x => new GsaMaterialGoo(x)));
      da.SetDataList(7, materials.AnalysisMaterials.Values.Select(x => new GsaMaterialGoo(x)));
    }
  }
}
