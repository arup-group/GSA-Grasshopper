using System;
using System.Collections.Generic;
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
  ///   Component to create a new Material
  /// </summary>
  public class CreateMaterial : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("40641747-cfb1-4dab-b060-b9dd344d3ac3");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
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

    protected override Bitmap Icon => Resources.CreateMaterial;

    public CreateMaterial() : base("Create" + GsaMaterialGoo.Name.Replace(" ", string.Empty),
          GsaMaterialGoo.Name.Replace(" ", string.Empty),
      "Create a " + GsaMaterialGoo.Description + " for a " + GsaSectionGoo.Description,
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
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
      pManager.AddIntegerParameter("Grade",
                                                                                       "Grd",
                                                                                       "Material Grade (default = 1)",
                                                                                       GH_ParamAccess.item,
                                                                                       1);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(),
                                                                                         "Material",
                                                                                         "Mat",
                                                                                         "GSA Standard Material (reference)",
                                                                                         GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var material = new GsaMaterial();

      var ghGrade = new GH_Integer();
      if (da.GetData(0, ref ghGrade)) {
        GH_Convert.ToInt32(ghGrade, out int grade, GH_Conversion.Both);
        material.GradeProperty = grade;
      }

      switch (_selectedItems[0]) {
        case "Steel":
          material.MaterialType = GsaMaterial.MatType.Steel;
          break;

        case "Concrete":
          material.MaterialType = GsaMaterial.MatType.Concrete;
          break;

        case "Timber":
          material.MaterialType = GsaMaterial.MatType.Timber;
          break;

        case "Aluminium":
          material.MaterialType = GsaMaterial.MatType.Aluminium;
          break;

        case "FRP":
          material.MaterialType = GsaMaterial.MatType.Frp;
          break;

        case "Glass":
          material.MaterialType = GsaMaterial.MatType.Glass;
          break;

        case "Fabric":
          material.MaterialType = GsaMaterial.MatType.Fabric;
          break;

        default:
          material.MaterialType = GsaMaterial.MatType.Generic;
          break;
      }

      da.SetData(0, new GsaMaterialGoo(material));
    }
  }
}
