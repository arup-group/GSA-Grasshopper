using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Prop2d
  /// </summary>
  public class CreateProp2d : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProp2d();
      switch (_mode) {
        case Prop2dType.PlaneStress:
        case Prop2dType.Fabric:
        case Prop2dType.FlatPlate:
        case Prop2dType.Shell:
        case Prop2dType.CurvedShell:
        case Prop2dType.LoadPanel:
          prop.Type = (Property2D_Type)(int)_mode;
          break;

        default:
          this.AddRuntimeWarning("Property type is undefined");
          prop.Type = Property2D_Type.UNDEF;
          break;
      }

      if (_mode != Prop2dType.LoadPanel) {
        prop.AxisProperty = 0;

        if (_mode != Prop2dType.Fabric) {
          prop.Thickness = (Length)Input.UnitNumber(this, da, 0, _lengthUnit);
          var ghTyp = new GH_ObjectWrapper();
          if (da.GetData(1, ref ghTyp)) {
            GsaMaterial material = null;
            if (ghTyp.Value is GsaMaterialGoo) {
              ghTyp.CastTo(ref material);
              prop.Material = material ?? new GsaMaterial();
            }
            else {
              if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
                prop.Material = new GsaMaterial(idd);
              else {
                this.AddRuntimeError(
                  "Unable to convert PB input to a Section Property of reference integer");
                return;
              }
            }
          }
          else
            prop.Material = new GsaMaterial(2);
        }
        else
          prop.Material = new GsaMaterial(8);
      }
      else {
        prop.SupportType = _supportDropDown.FirstOrDefault(x => x.Value == SelectedItems[1]).Key;
        if (prop.SupportType != SupportType.Auto) {
          int referenceEdge = 0;
          prop.ReferenceEdge = da.GetData("Reference edge", ref referenceEdge)
            ? referenceEdge
            : prop.ReferenceEdge;
        }
      }

      da.SetData(0, new GsaProp2dGoo(prop));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("d693b4ad-7aaf-450e-a436-afbb9d2061fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateProp2d;

    public CreateProp2d() : base("Create 2D Property",
      "Prop2d",
      "Create GSA 2D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Thk",
        "Section thickness",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaProp2dParameter());

    #endregion

    #region Custom UI

    private readonly IReadOnlyDictionary<Prop2dType, string> _dropdownTopLevel = new Dictionary<Prop2dType, string>{
      { Prop2dType.PlaneStress, "Plane Stress"},
      { Prop2dType.Fabric, "Fabric"},
      { Prop2dType.FlatPlate, "Flat Plate"},
      { Prop2dType.Shell, "Shell"},
      { Prop2dType.CurvedShell, "Curved Shell"},
      { Prop2dType.LoadPanel, "Load Panel"},
    };

    private readonly IReadOnlyDictionary<SupportType, string> _supportDropDown = new Dictionary<SupportType, string>{
      { SupportType.Auto, "Automatic"},
      { SupportType.AllEdges, "All edges"},
      { SupportType.ThreeEdges, "Three edges"},
      { SupportType.TwoEdges, "Two edges"},
      { SupportType.TwoAdjacentEdges, "Two adjacent edges"},
      { SupportType.OneEdge, "One edge"},
      { SupportType.Cantilever, "Cantilever"},
    };

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private int _supportTypeIndex;

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      DropDownItems.Add(_dropdownTopLevel.Values.ToList());
      SelectedItems.Add(_dropdownTopLevel.Values.ElementAt(3));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      Prop2dType mode = GetModeBy(SelectedItems[0]);
      if (i == 0) {
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }

      if (i != 0 && mode != Prop2dType.LoadPanel)
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);


      if (i == 1 && mode == Prop2dType.LoadPanel) {
        _supportTypeIndex = j;
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      Prop2dType mode = GetModeBy(SelectedItems[0]);

      if (mode == Prop2dType.LoadPanel)
        _supportTypeIndex = _supportDropDown.ToList()
          .FindIndex(x => x.Value == SelectedItems[1]);
      else if (mode != Prop2dType.Fabric)
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[1]);


      UpdateDropDownItems(mode);
      UpdateParameters(mode);

      base.UpdateUIFromSelectedItems();
    }
    
    #region update inputs

    private Prop2dType _mode = Prop2dType.Shell;

    private void UpdateParameters(Prop2dType mode) {
      if (_mode == mode && mode != Prop2dType.LoadPanel)
        return;

      _dropdownTopLevel.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      switch (mode) {
        case Prop2dType.Shell:
        case Prop2dType.PlaneStress:
        case Prop2dType.FlatPlate:
        case Prop2dType.CurvedShell:
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new GsaMaterialParameter());
          break;
        case Prop2dType.Fabric:
          Params.RegisterInputParam(new Param_GenericObject());
          break;
        case Prop2dType.LoadPanel:
          if (_supportTypeIndex != _supportDropDown.Keys.ToList().IndexOf(SupportType.Auto))
            Params.RegisterInputParam(new Param_Integer());
          break;
      }

      _mode = mode;
    }

    private void UpdateDropDownItems(Prop2dType mode) {
      ResetDropdownMenus();
      switch (mode) {
        case Prop2dType.PlaneStress:
        case Prop2dType.FlatPlate:
        case Prop2dType.Shell:
        case Prop2dType.CurvedShell:
          AddLengthUnitDropDown();
          break;
        case Prop2dType.Fabric:
          break;
        case Prop2dType.LoadPanel:
          AddSupportTypeDropDown();
          break;
      }
    }

    private Prop2dType GetModeBy(string name) {
      Prop2dType mode = Prop2dType.Shell;

      foreach (KeyValuePair<Prop2dType, string> item in _dropdownTopLevel)
        if (item.Value.Contains(name))
          mode = item.Key;
      return mode;
    }

    #endregion

    public override void VariableParameterMaintenance() {
      switch (_mode) {
        case Prop2dType.LoadPanel:
          if (_supportTypeIndex != _supportDropDown.Keys.ToList().IndexOf(SupportType.Auto))
            SetReferenceEdgeInputAt(0);
          return;
        case Prop2dType.Fabric:
          SetMaterialInputAt(0);
          break;
        case Prop2dType.Shell:
        case Prop2dType.PlaneStress:
        case Prop2dType.FlatPlate:
        case Prop2dType.CurvedShell:
          SetInputProperties(index: 0, nickname: "Thk", name: $"Thickness [{Length.GetAbbreviation(_lengthUnit)}]", description: "Section thickness", optional: false);
          SetMaterialInputAt(1);
          break;
      }
    }

    private void SetInputProperties(
      int index,
      string nickname,
      string name,
      string description,
      GH_ParamAccess access = GH_ParamAccess.item,
      bool optional = true) {
      Params.Input[index]
        .NickName = nickname;
      Params.Input[index]
        .Name = name;
      Params.Input[index]
        .Description = description;
      Params.Input[index]
        .Access = access;
      Params.Input[index]
        .Optional = optional;
    }

    private void SetMaterialInputAt(int index) => SetInputProperties(index, "Mat", "Material", "GSA Material");
    private void SetReferenceEdgeInputAt(int index) => SetInputProperties(index, "RE", "Reference edge", "Reference edge for automatic support type");
    #endregion

    private void AddLengthUnitDropDown() {
      SpacerDescriptions.Add("Unit");
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));
    }

    private void AddSupportTypeDropDown() {
      if (_supportTypeIndex < 0 || _supportTypeIndex >= _supportDropDown.Count) {
        this.AddRuntimeError("Index for selected items out of range!");
        _supportTypeIndex = 0;
      }

      SpacerDescriptions.Add("Support Type");
      DropDownItems.Add(_supportDropDown.Values.ToList());
      SelectedItems.Add(_supportDropDown.Values.ElementAt(_supportTypeIndex));
    }

    private void ResetDropdownMenus() {
      while (SpacerDescriptions.Count > 1)
        SpacerDescriptions.RemoveAt(SpacerDescriptions.Count - 1);
      while (DropDownItems.Count > 1)
        DropDownItems.RemoveAt(DropDownItems.Count - 1);
      while (SelectedItems.Count > 1)
        SelectedItems.RemoveAt(SelectedItems.Count - 1);
    }
  }
}
