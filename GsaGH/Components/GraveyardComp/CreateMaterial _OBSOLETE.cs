using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.UI;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Material
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class CreateMaterial_OBSOLETE : GH_OasysComponent,
    IGH_VariableParameterComponent {
    #region IGH_VariableParameterComponent null implementation

    void IGH_VariableParameterComponent.VariableParameterMaintenance() { }

    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("Analysis Property Number",
        "ID",
        "Analysis Property Number (default = 0 -> 'from Grade')",
        GH_ParamAccess.item,
        0);
      pManager.AddIntegerParameter("Grade",
        "Gr",
        "Material Grade (default = 1)",
        GH_ParamAccess.item,
        1);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddGenericParameter("Material", "Ma", "GSA Material", GH_ParamAccess.item);

    protected override void SolveInstance(IGH_DataAccess da) {
      var material = new GsaMaterial();

      var ghAnal = new GH_Integer();
      if (da.GetData(0, ref ghAnal)) {
        GH_Convert.ToInt32(ghAnal, out int anal, GH_Conversion.Both);
        material.AnalysisProperty = anal;
      }

      var ghGrade = new GH_Integer();
      if (da.GetData(1, ref ghGrade)) {
        GH_Convert.ToInt32(ghGrade, out int grade, GH_Conversion.Both);
        material.GradeProperty = grade;
      }

      switch (_mode) {
        case FoldMode.Generic:
          material.MaterialType = GsaMaterial.MatType.Generic;
          break;
        case FoldMode.Steel:
          material.MaterialType = GsaMaterial.MatType.Steel;
          break;
        case FoldMode.Concrete:
          material.MaterialType = GsaMaterial.MatType.Concrete;
          break;
        case FoldMode.Timber:
          material.MaterialType = GsaMaterial.MatType.Timber;
          break;
        case FoldMode.Aluminium:
          material.MaterialType = GsaMaterial.MatType.Aluminium;
          break;
        case FoldMode.Frp:
          material.MaterialType = GsaMaterial.MatType.Frp;
          break;
        case FoldMode.Glass:
          material.MaterialType = GsaMaterial.MatType.Glass;
          break;
        case FoldMode.Fabric:
          material.MaterialType = GsaMaterial.MatType.Fabric;
          break;
      }

      da.SetData(0, new GsaMaterialGoo(material));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("72bfce91-9204-4fe4-b81d-0036babf0c6d");

    public CreateMaterial_OBSOLETE() : base("Create Material",
      "Material",
      "Create GSA Material by reference to existing type and grade",
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMaterial;

    #endregion

    #region Custom UI

    public override void CreateAttributes() {
      if (_first)
        _selecteditem = _mode.ToString();

      m_attributes = new DropDownComponentAttributes(this,
        SetSelected,
        new List<List<string>>() {
          _dropDownItems,
        },
        new List<string>() {
          _selecteditem,
        },
        new List<string>() {
          "Material Type",
        });
    }

    public void SetSelected(int i, int j) {
      _selecteditem = _dropDownItems[i];
      switch (_selecteditem) {
        case "Generic":
          Mode1Clicked();
          break;
        case "Steel":
          Mode2Clicked();
          break;
        case "Concrete":
          Mode3Clicked();
          break;
        case "Timber":
          Mode4Clicked();
          break;
        case "Aluminium":
          Mode5Clicked();
          break;
        case "Frp":
          Mode6Clicked();
          break;
        case "Glass":
          Mode7Clicked();
          break;
        case "Fabric":
          Mode8Clicked();
          break;
      }
    }

    #endregion

    #region Input and output

    private readonly List<string> _dropDownItems = new List<string>(new[] {
      "Generic",
      "Steel",
      "Concrete",
      "Timber",
      "Aluminium",
      "Frp",
      "Glass",
      "Fabric",
    });

    private string _selecteditem;

    #endregion

    #region menu override

    private enum FoldMode {
      Generic,
      Steel,
      Concrete,
      Timber,
      Aluminium,
      Frp,
      Glass,
      Fabric,
    }

    private readonly bool _first = true;
    private FoldMode _mode = FoldMode.Timber;

    private void Mode1Clicked() {
      if (_mode == FoldMode.Generic)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Generic;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode2Clicked() {
      if (_mode == FoldMode.Steel)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Steel;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode3Clicked() {
      if (_mode == FoldMode.Concrete)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Concrete;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode4Clicked() {
      if (_mode == FoldMode.Timber)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Timber;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode5Clicked() {
      if (_mode == FoldMode.Aluminium)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Aluminium;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode6Clicked() {
      if (_mode == FoldMode.Frp)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Frp;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode7Clicked() {
      if (_mode == FoldMode.Glass)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Glass;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode8Clicked() {
      if (_mode == FoldMode.Fabric)
        return;

      RecordUndoEvent(_mode + "Parameters");

      _mode = FoldMode.Fabric;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    #endregion

    #region (de)serialization

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetString("select", _selecteditem);
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _selecteditem = reader.GetString("select");
      CreateAttributes();
      return base.Read(reader);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
      => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
      => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
      => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;

    #endregion
  }
}
