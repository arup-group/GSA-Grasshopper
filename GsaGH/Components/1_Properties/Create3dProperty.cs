using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Prop2d
  /// </summary>
  public class Create3dProperty : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("4919553a-8d96-4170-a357-74cfbe930897");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Create3dProperty;

    public Create3dProperty() : base("Create 3D Property", "Prop3d", "Create GSA 3D Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProperty3dParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProperty3d();

      GsaMaterialGoo materialGoo = null;
      if (da.GetData(0, ref materialGoo)) {
        prop.Material = materialGoo.Value;
      }

      prop.ApiProp3d.AxisProperty = 0;

      da.SetData(0, new GsaProperty3dGoo(prop));
    }
  }
}
