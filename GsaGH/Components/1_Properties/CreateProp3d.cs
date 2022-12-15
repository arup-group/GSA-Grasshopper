using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to create a new Prop2d
    /// </summary>
    public class CreateProp3d : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("4919553a-8d96-4170-a357-74cfbe930897");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateProp3d;

    public CreateProp3d() : base("Create 3D Property",
      "Prop3d",
      "Create GSA 3D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaMaterialParameter());
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaProp3dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaProp3d prop = new GsaProp3d();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        GsaMaterial material = new GsaMaterial();
        if (gh_typ.Value is GsaMaterialGoo)
        {
          gh_typ.CastTo(ref material);
          prop.Material = material;
        }
        else
        {
          if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
            prop.Material = new GsaMaterial(idd);
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PV input to a 3D Property of reference integer");
            return;
          }
        }
      }
      else
        prop.Material = new GsaMaterial(2);

      prop.AxisProperty = 0;

      DA.SetData(0, new GsaProp3dGoo(prop));
    }
  }
}
