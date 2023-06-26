using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components.GraveyardComp {
  /// <summary>
  ///   Component to create a new Material
  /// </summary>
  public class CreateMaterial2_OBSOLETE : GH_OasysDropDownComponent {
    public static List<string> MaterialTypes = new List<string>() {
      "Generic",
      "Steel",
      "Concrete",
      "Timber",
      "Aluminium",
      "FRP",
      "Glass",
      "Fabric",
    };
    public override Guid ComponentGuid => new Guid("40641747-cfb1-4dab-b060-b9dd344d3ac3");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    protected override Bitmap Icon => Resources.CreateMaterial;

    public CreateMaterial2_OBSOLETE() : base(
      $"Create{GsaMaterialGoo.Name.Replace(" ", string.Empty)}",
      GsaMaterialGoo.Name.Replace(" ", string.Empty),
      $"Create a {GsaMaterialGoo.Description} for a {GsaSectionGoo.Description}",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Material type",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>(MaterialTypes));
      _selectedItems.Add(MaterialTypes[3]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("Grade", "Grd", "Material Grade (default = 1)",
        GH_ParamAccess.item, 1);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat",
        "GSA Standard Material (reference)", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      this.AddRuntimeError(
        $"This component is obsolete and no longer works with the plugin. {Environment.NewLine}Please use the new CreateMaterial component that now supports standard materials.{Environment.NewLine}Simply drag a new Create Material component onto the canvas.");

      da.SetData(0, new GsaMaterialGoo(null));
    }
  }
}
