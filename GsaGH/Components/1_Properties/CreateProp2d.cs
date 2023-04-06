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
      { SupportType.AllEdges, "All sides"},
      { SupportType.ThreeEdges, "Three sides"},
      { SupportType.TwoEdges, "Two sides"},
      { SupportType.TwoAdjacentEdges, "Two adjacent sides"},
      { SupportType.OneEdge, "One side"},
      { SupportType.Cantilever, "Cantilever"},
    };

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private int _supportTypeIndex = 0;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Type",
        "Unit",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(_dropdownTopLevel.Values.ToList());
      SelectedItems.Add(_dropdownTopLevel.Values.ElementAt(3));

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];

      Prop2dType mode = GetModeBy(SelectedItems[0]);
      if (i == 0) {
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }

      if (i != 0 && mode != Prop2dType.LoadPanel)
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

      if (i == 1 && mode == Prop2dType.LoadPanel) {
        _supportTypeIndex = j;
        UpdateDropDownItems(mode);
      }

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      Prop2dType mode = GetModeBy(SelectedItems[0]);

      if (mode == Prop2dType.LoadPanel)
        _supportTypeIndex = _supportDropDown.ToList()
          .FindIndex(x => x.Value == SelectedItems[1]);
      else
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[1]);

      ResetDropdownMenus();

      switch (mode) {
        case Prop2dType.PlaneStress:
          AddLengthUnitDropDown();
          Mode1Clicked();
          break;
        case Prop2dType.Fabric:
          Mode2Clicked();
          break;
        case Prop2dType.FlatPlate:
          AddLengthUnitDropDown();
          Mode3Clicked();
          break;
        case Prop2dType.Shell:
          AddLengthUnitDropDown();
          Mode4Clicked();
          break;
        case Prop2dType.CurvedShell:
          AddLengthUnitDropDown();
          Mode5Clicked();
          break;
        case Prop2dType.LoadPanel:
          AddSupportTypeDropDown();
          Mode6Clicked();
          break;
      }

      base.UpdateUIFromSelectedItems();
    }
    #region update inputs

    private Prop2dType _mode = Prop2dType.Shell;

    private void UpdateParameters(Prop2dType mode) {
      if (_mode == mode)
        return;

      _dropdownTopLevel.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      switch (mode) {
        case Prop2dType.Shell:
        case Prop2dType.PlaneStress:
        case Prop2dType.FlatPlate:
        case Prop2dType.CurvedShell:
          switch (Params.Input.Count) {
            case 0:
              Params.RegisterInputParam(new Param_GenericObject());
              Params.RegisterInputParam(new GsaMaterialParameter());
              break;
            case 1:
              Params.RegisterInputParam(new GsaMaterialParameter());
              break;
          }

          break;
        case Prop2dType.Fabric:
          switch (Params.Input.Count) {
            case 0:
              Params.RegisterInputParam(new Param_GenericObject());
              break;
            case 2:
              Params.UnregisterInputParameter(Params.Input[1], true);
              break;
          }

          break;
        case Prop2dType.LoadPanel:
          while (Params.Input.Count > 0)
            Params.UnregisterInputParameter(Params.Input[0], true);
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

    // shitty methods - extracted it to updateDropDown and UpdateParameters but then
    // using updateParameter in UpdateUIFromSelectedItems brokes tests...so left for now
    private void Mode1Clicked() {
      if (_mode == Prop2dType.PlaneStress)
        return;

      RecordUndoEvent("Plane Stress Parameters");
      if (_mode == Prop2dType.LoadPanel || _mode == Prop2dType.Fabric) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Prop2dType.PlaneStress;
    }

    private void Mode2Clicked() {
      if (_mode == Prop2dType.Fabric)
        return;

      RecordUndoEvent("Fabric Parameters");
      _mode = Prop2dType.Fabric;

      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      Params.RegisterInputParam(new Param_GenericObject());
    }

    private void Mode3Clicked() {
      if (_mode == Prop2dType.FlatPlate)
        return;

      RecordUndoEvent("Flat Plate Parameters");
      if (_mode == Prop2dType.LoadPanel || _mode == Prop2dType.Fabric) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Prop2dType.FlatPlate;
    }

    private void Mode4Clicked() {
      if (_mode == Prop2dType.Shell)
        return;

      RecordUndoEvent("Shell Parameters");
      if (_mode == Prop2dType.LoadPanel || _mode == Prop2dType.Fabric) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Prop2dType.Shell;
    }

    private void Mode5Clicked() {
      if (_mode == Prop2dType.CurvedShell)
        return;

      RecordUndoEvent("Curved Shell Parameters");
      if (_mode == Prop2dType.LoadPanel || _mode == Prop2dType.Fabric) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Prop2dType.CurvedShell;
    }

    private void Mode6Clicked() {
      if (_mode == Prop2dType.LoadPanel)
        return;

      RecordUndoEvent("Load Panel Parameters");
      _mode = Prop2dType.LoadPanel;

      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);
    }
  }
}
