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
  ///   Component to edit a Prop3d and ouput the information
  /// </summary>
  public class Edit3dProperty : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("5e28d4d9-a0ab-46a8-8476-71781c315855");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit3dProperty;

    public Edit3dProperty() : base("Edit 3D Property", "EditPV", "Modify a GSA 3D Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaProperty3dParameter(), GsaProperty3dGoo.Name, GsaProperty3dGoo.NickName,
        GsaProperty3dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaProperty3dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop3d Number", "ID",
        "Set 3D Property Number. If ID is set it will replace any existing 3D Property in the model",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddIntegerParameter("Axis", "Ax",
        "Set Axis as integer: Global (0) or Topological (-1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop3d Name", "Na", "Set Name of 3D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop3d Colour", "Co", "Set 3D Property Colour",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProperty3dParameter(), GsaProperty3dGoo.Name, GsaProperty3dGoo.NickName,
        GsaProperty3dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "3D Property Number",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddIntegerParameter("Axis", "Ax", "Get Axis: Global (0) or Topological (-1)",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Prop3d Name", "Na", "Name of 3D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop3d Colour", "Co", "3D Property Colour", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProperty3d();

      GsaProperty3dGoo prop3dGoo = null;
      if (da.GetData(0, ref prop3dGoo)) {
        prop = new GsaProperty3d(prop3dGoo.Value);
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        prop.Id = id;
      }

      GsaMaterialGoo materialGoo = null;
      if (da.GetData(2, ref materialGoo)) {
        prop.Material = materialGoo.Value;
      }

      int axisId = 0;
      if (da.GetData(3, ref axisId)) {
        prop.ApiProp3d.AxisProperty = axisId;
      }

      string name = string.Empty;
      if (da.GetData(4, ref name)) {
        prop.ApiProp3d.Name = name;
      }

      Color colour = Color.Empty;
      if (da.GetData(5, ref colour)) {
        prop.ApiProp3d.Colour = colour;
      }

      int ax = (prop.ApiProp3d == null) ? 0 : prop.ApiProp3d.AxisProperty;
      string nm = (prop.ApiProp3d == null) ? "--" : prop.ApiProp3d.Name;

      da.SetData(0, new GsaProperty3dGoo(prop));
      da.SetData(1, prop.Id);
      da.SetData(2, new GsaMaterialGoo(prop.Material));
      da.SetData(3, ax);
      da.SetData(4, nm);
      da.SetData(5, (Color)prop.ApiProp3d.Colour);
    }
  }
}
