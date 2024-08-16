using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaAPI;

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
    public override Guid ComponentGuid => new Guid("1d1127d4-2c8d-4ae6-bec4-df8023afcba8");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public static List<string> MaterialTypes = new List<string>() {
      "Steel",
      "Concrete",
      "FRP",
      "Aluminium",
      "Timber",
      "Glass",
      "Fabric",
    };
    protected override Bitmap Icon => Resources.CreateMaterial;
    private string _concreteCode = string.Empty;
    private string _steelCode = string.Empty;
    private Dictionary<string, GsaMaterial> _gradeMaterials;

    public CreateMaterial() : base("Create " + GsaMaterialGoo.Name,
      GsaMaterialGoo.NickName,
      "Create a " + GsaMaterialGoo.Description + " for a GSA Section, Prop2d or Prop3d",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0) {
        UpdateMaterialType();
      } else {
        if (i == 1 && _selectedItems[0] == "Steel") {
          _steelCode = _selectedItems[1];
          UpdateGrades();
        } else if (i == 1 && _selectedItems[0] == "Concrete") {
          _concreteCode = _selectedItems[1];
          UpdateGrades();
        }
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Material type",
        "Design Code",
        "Grade"
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(MaterialTypes);
      _selectedItems.Add(MaterialTypes[1]); // concrete

      _dropDownItems.Add(GetCodeNames(false));
      _concreteCode = _dropDownItems[1][8];
      _selectedItems.Add(_concreteCode); // EC2-1-1

      _dropDownItems.Add(GsaMaterialFactory.GetGradeNames(MatType.Concrete, _concreteCode, string.Empty));
      _selectedItems.Add(_dropDownItems[2][4]); // C30/37

      _isInitialised = true;
    }

    protected override void UpdateUIFromSelectedItems() {
      var type = (MatType)Enum.Parse(
        typeof(MatType), _selectedItems[0], ignoreCase: true);

      if (type == MatType.Concrete) {
        _concreteCode = _selectedItems[1];
      }

      if (type == MatType.Steel) {
        _steelCode = _selectedItems[1];
      }

      string grade = _selectedItems.Last().ToString();
      UpdateMaterialType();
      _selectedItems[_selectedItems.Count - 1] = grade;
      base.UpdateUIFromSelectedItems();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter(
        "Grade Name", "Grd", "Search for a Grade name in the selected material and code",
        GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat",
        "GSA Standard Material", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      string search = string.Empty;
      if (da.GetData(0, ref search)) {
        search = search.Trim().ToLower().Replace(".", string.Empty).Replace(" ", string.Empty);
        bool wildCard = search.Contains("*");
        var materials = new List<GsaMaterialGoo>();
        foreach (KeyValuePair<string, GsaMaterial> mat in _gradeMaterials) {
          if (wildCard ? mat.Key.Contains(search.Replace("*", string.Empty)) : mat.Key == search) {
            materials.Add(new GsaMaterialGoo(mat.Value));
          }
        }

        if (materials.Count == 0) {
          this.AddRuntimeError($"No grade named {search} was found");
        } else {
          da.SetDataList(0, materials);
        }

        return;
      }

      var type = (MatType)Enum.Parse(
        typeof(MatType), _selectedItems[0], ignoreCase: true);

      string steelCode = _steelCode.Replace("HK", "Hong Kong").Replace("CoP", "Code of Practice");
      string concreteCode = _concreteCode.Replace("CoP", "CP");

      string code = string.Empty;
      switch (type) {
        case MatType.Concrete:
          code = concreteCode;
          break;

        case MatType.Steel:
          code = steelCode;
          break;
      }

      var material = (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(type, _selectedItems.Last(), code);
      da.SetData(0, new GsaMaterialGoo(material));
    }

    private void UpdateMaterialType() {
      var type = (MatType)Enum.Parse(
        typeof(MatType), _selectedItems[0], ignoreCase: true);
      switch (type) {
        case MatType.Steel:
        case MatType.Concrete:
          if (_dropDownItems.Count < 3) {
            _dropDownItems.Add(new List<string>());
            _selectedItems.Add(string.Empty);
            _spacerDescriptions = new List<string>(new[] {
              "Material type",
              "Design Code",
              "Grade"
            });
            ReDrawComponent();
          }

          _dropDownItems[1] = GetCodeNames(type == MatType.Steel);

          if (type == MatType.Steel && _steelCode == string.Empty) {
            _steelCode = _dropDownItems[1][8]; // EN 1993-1-1
          }

          _selectedItems[1] = type == MatType.Steel ? _steelCode : _concreteCode;
          break;

        default:
          if (_dropDownItems.Count > 2) {
            _dropDownItems.RemoveAt(1);
            _selectedItems.RemoveAt(1);
            _spacerDescriptions = new List<string>(new[] {
            "Material type",
            "Grade"
            });

            ReDrawComponent();
          }
          break;
      }

      UpdateGrades();
    }

    private void UpdateGrades() {
      var type = (MatType)Enum.Parse(
        typeof(MatType), _selectedItems[0], ignoreCase: true);

      List<string> grades = GsaMaterialFactory.GetGradeNames(type, _concreteCode, _steelCode);
      for (int i = grades.Count - 1; i >= 0; i--) {
        if (grades[i].StartsWith("<")) {
          grades.RemoveAt(i);
        }
      }

      _dropDownItems[_dropDownItems.Count - 1] = grades;
      _selectedItems[_selectedItems.Count - 1] = grades[0];

      _gradeMaterials = new Dictionary<string, GsaMaterial>();
      string code = string.Empty;
      switch (type) {
        case MatType.Concrete:
          code = _concreteCode;
          break;

        case MatType.Steel:
          code = _steelCode;
          break;
      }

      foreach (string grade in grades) {
        _gradeMaterials.Add(
          grade.ToLower().Replace(".", string.Empty).Replace("*", string.Empty)
          .Replace(" ", string.Empty), (GsaMaterial)GsaMaterialFactory.CreateStandardMaterial(type, grade, code));
      }
    }

    private void ReDrawComponent() {
      var pivot = new PointF(Attributes.Pivot.X, Attributes.Pivot.Y);
      base.CreateAttributes();
      Attributes.Pivot = pivot;
      Attributes.ExpireLayout();
      Attributes.PerformLayout();
    }

    private List<string> GetCodeNames(bool isSteel) {
      List<string> codes = isSteel
        ? DesignCode.GetSteelDesignCodeNames().ToList()
        : DesignCode.GetConcreteDesignCodeNames().ToList();
      for (int i = 0; i < codes.Count; i++) {
        codes[i] = codes[i].Replace("Hong Kong", "HK").Replace("Code of Practice", "CoP")
          .Replace("CP", "CoP");
      }

      for (int i = codes.Count - 1; i >= 0; i--) {
        if (codes[i].StartsWith("<")) {
          codes.RemoveAt(i);
        }
      }

      return codes;
    }
  }
}
