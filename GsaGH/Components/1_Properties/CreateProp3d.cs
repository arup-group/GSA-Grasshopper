using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Prop2d
  /// </summary>
  public class CreateProp3d : GH_OasysComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProp3d();

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(0, ref ghTyp)) {
        var material = new GsaMaterial();
        if (ghTyp.Value is GsaMaterialGoo) {
          ghTyp.CastTo(ref material);
          prop.Material = material ?? new GsaMaterial();
        }
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
            prop.Material = new GsaMaterial(idd);
          else {
            this.AddRuntimeError(
              "Unable to convert PV input to a 3D Property of reference integer");
            return;
          }
        }
      }
      else
        prop.Material = new GsaMaterial(2);

      prop.AxisProperty = 0;

      da.SetData(0, new GsaProp3dGoo(prop));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("4919553a-8d96-4170-a357-74cfbe930897");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateProp3d;

    public CreateProp3d() : base("Create 3D Property",
      "Prop3d",
      "Create GSA 3D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
      => pManager.AddParameter(new GsaMaterialParameter());

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaProp3dParameter());

    #endregion
  }
}
