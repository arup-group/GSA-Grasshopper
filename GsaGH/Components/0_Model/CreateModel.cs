using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaGH.Helpers;
using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to assemble and analyse a GSA model
  /// </summary>
  public class CreateModel : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("326aa021-10b3-45a0-8286-eefb3dc3e2e1");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateModel;
    private readonly List<string> _checkboxTexts = new List<string>() {
      "ElemsFromMems",
    };
    private List<bool> _initialCheckState = new List<bool>() {
      true,
    };
    private bool _reMesh = true;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    internal ToleranceContextMenu ToleranceMenu { get; set; } = new ToleranceContextMenu();

    public CreateModel() : base("Create Model", "Model", "Assemble a GSA Model",
      CategoryName.Name(), SubCategoryName.Cat0()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      ToleranceMenu.AppendAdditionalMenuItems(this, menu, _lengthUnit);
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownCheckBoxesComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, SetAnalysis, _initialCheckState, _checkboxTexts, _spacerDescriptions);
    }

    public override bool Read(GH_IReader reader) {
      _reMesh = reader.GetBoolean("ReMesh");
      _initialCheckState = new List<bool>() {
        _reMesh,
      };
      bool flag = base.Read(reader);
      if (reader.ItemExists("Tolerance")) {
        double tol = reader.GetDouble("Tolerance");
        ToleranceMenu.Tolerance = new Length(tol, _lengthUnit);
      } else {
        ToleranceMenu.Tolerance = DefaultUnits.Tolerance;
      }

      ToleranceMenu.UpdateMessage(this, _lengthUnit);
      return flag;
    }

    public void SetAnalysis(List<bool> value) {
      _reMesh = value[0];
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      ToleranceMenu.UpdateMessage(this, _lengthUnit);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[2].Name = "GSA Geometry in [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("ReMesh", _reMesh);
      writer.SetDouble("Tolerance", ToleranceMenu.Tolerance.Value);
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      ToleranceMenu.UpdateMessage(this, _lengthUnit);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
        "Settings",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Model(s), Lists and Grid Lines", "GSA",
        "Existing GSA Model(s) to append to, Lists and Grid Lines" + Environment.NewLine
        + "If you input more than one model they will be merged" + Environment.NewLine
        + "with first model in list taking priority for IDs", GH_ParamAccess.list);
      pManager.AddGenericParameter("Properties", "Pro",
        "GSA Sections (PB), Prop2Ds (PA) and Prop3Ds (PV) to add/set in the model"
        + Environment.NewLine + "Properties already added to Elements or Members"
        + Environment.NewLine + "will automatically be added with Geometry input",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("GSA Geometry in [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Geo",
        "GSA Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Load", "Ld",
        "Loads to add to the model" + Environment.NewLine
        + "You can also use this input to add Edited GridPlaneSurfaces", GH_ParamAccess.list);
      pManager.AddGenericParameter("Analysis Tasks & Combinations", "ΣT",
        "GSA Analysis Tasks and Combination Cases to add to the model", GH_ParamAccess.list);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      // Collect inputs
      (List<GsaModel> models, List<GsaList> lists, List<GsaGridLine> gridLines) =
        InputsForModelAssembly.GetModelsAndLists(this, da, 0, true);
      GsaProperties properties = InputsForModelAssembly.GetProperties(this, da, 1, true);
      GsaGeometry geometry = InputsForModelAssembly.GetGeometry(this, da, 2, true);
      GsaLoading loading = InputsForModelAssembly.GetLoading(this, da, 3, true);
      GsaAnalysis analysis = InputsForModelAssembly.GetAnalysis(this, da, 4, true);

      if (models.IsNullOrEmpty() & lists.IsNullOrEmpty() & gridLines.IsNullOrEmpty()
        & geometry.IsNullOrEmpty() & properties.IsNullOrEmpty()
        & loading.IsNullOrEmpty() & analysis.IsNullOrEmpty()) {
        this.AddRuntimeWarning("Input parameters failed to collect data");
        return;
      }

      // Merge models
      var model = new GsaModel();
      if (models != null) {
        if (models.Count > 0) {
          model = models.Count > 1
            ? MergeModels.MergeModel(models, this, ToleranceMenu.Tolerance) :
            models[0].Clone();
        }
      }
      // Assemble model
      var assembly = new ModelAssembly(model, lists, gridLines, geometry, properties, loading,
        analysis, _lengthUnit, ToleranceMenu.Tolerance, _reMesh, this);
      model.ApiModel = assembly.GetModel();

      ToleranceMenu.UpdateMessage(this, _lengthUnit);

      da.SetData(0, new GsaModelGoo(model));
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
