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
        case Property2D_Type.PL_STRESS:
        case Property2D_Type.FABRIC:
        case Property2D_Type.PLATE:
        case Property2D_Type.SHELL:
        case Property2D_Type.CURVED_SHELL:
        case Property2D_Type.LOAD:
          prop.Type = _mode;
          break;

        default:
          this.AddRuntimeWarning("Property type is undefined");
          prop.Type = Property2D_Type.UNDEF;
          break;
      }

      if (_mode != Property2D_Type.LOAD) {
        prop.AxisProperty = 0;

        if (_mode != Property2D_Type.FABRIC) {
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

      da.SetData(0, new GsaProp2dGoo(prop));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("796c3b14-e34b-4cd0-8364-d4003b32857a");
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

    private readonly IReadOnlyDictionary<Property2D_Type, string> _dropdownTopLevel = new Dictionary<Property2D_Type, string>{
      { Property2D_Type.PL_STRESS, "Plane Stress"},
      { Property2D_Type.FABRIC, "Fabric"},
      { Property2D_Type.PLATE, "Flat Plate"},
      { Property2D_Type.SHELL, "Shell"},
      { Property2D_Type.CURVED_SHELL, "Curved Shell"},
      { Property2D_Type.LOAD, "Load Panel"},
    };

    private readonly IReadOnlyDictionary<SupportType, string> _supportDropDown = new Dictionary<SupportType, string>{
      { SupportType.Auto, "Automatic"},
      { SupportType.AllEdges, "All sides"},
      { SupportType.ThreeEdges, "3 sides"},
      { SupportType.TwoEdges, "2 sides"},
      { SupportType.TwoAdjacentEdges, "2 adjacent sides"},
      { SupportType.OneEdge, "1 side"},
      { SupportType.Cantilever, "Cantilever"},
    };

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

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

      if (i == 0) {
        Property2D_Type mode = GetModeBy(SelectedItems[0]);
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }
      else
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      if (DropDownItems.Count > 1)
        DropDownItems.RemoveAt(1);

      Property2D_Type mode = GetModeBy(SelectedItems[0]);

      switch (mode) {
        case Property2D_Type.PL_STRESS:
          if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
            while (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(DropDownItems.Count - 1);
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          }

          Mode1Clicked();
          break;
        case Property2D_Type.FABRIC:
          while (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(DropDownItems.Count - 1);
          Mode2Clicked();
          break;
        case Property2D_Type.PLATE:
          if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
            while (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(DropDownItems.Count - 1);
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          }

          Mode3Clicked();
          break;
        case Property2D_Type.SHELL:
          if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
            while (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(DropDownItems.Count - 1);
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          }

          Mode4Clicked();
          break;
        case Property2D_Type.CURVED_SHELL:
          if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
            while (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(DropDownItems.Count - 1);
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          }

          Mode5Clicked();
          break;
        case Property2D_Type.LOAD:
          while (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(DropDownItems.Count - 1);
          // DropDownItems.Add(_supportDropDown.Values.ToList());
          // SelectedItems.Add(_supportDropDown.Values.ElementAt(0));
          Mode6Clicked();
          break;
      }

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    #region update inputs

    private Property2D_Type _mode = Property2D_Type.SHELL;

    private void UpdateParameters(Property2D_Type mode) {
      if (_mode == mode)
        return;

      _dropdownTopLevel.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      switch (mode) {
        case Property2D_Type.SHELL:
        case Property2D_Type.PL_STRESS:
        case Property2D_Type.PLATE:
        case Property2D_Type.CURVED_SHELL:
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
        case Property2D_Type.FABRIC:
          switch (Params.Input.Count) {
            case 0:
              Params.RegisterInputParam(new Param_GenericObject());
              break;
            case 2:
              Params.UnregisterInputParameter(Params.Input[1], true);
              break;
          }

          break;
        case Property2D_Type.LOAD:
          while (Params.Input.Count > 0)
            Params.UnregisterInputParameter(Params.Input[0], true);
          break;
      }

      _mode = mode;
    }

    private void UpdateDropDownItems(Property2D_Type mode) {
      switch (mode) {
        case Property2D_Type.PL_STRESS:
        case Property2D_Type.PLATE:
        case Property2D_Type.SHELL:
        case Property2D_Type.CURVED_SHELL:
          if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
            while (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(DropDownItems.Count - 1);
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          }

          break;
        case Property2D_Type.FABRIC:
        case Property2D_Type.LOAD:
          while (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(DropDownItems.Count - 1);
          break;
      }
    }

    private Property2D_Type GetModeBy(string name) {
      Property2D_Type mode = Property2D_Type.SHELL;

      foreach (KeyValuePair<Property2D_Type, string> item in _dropdownTopLevel)
        if (item.Value.Contains(name))
          mode = item.Key;
      return mode;
    }

    #endregion

    public override void VariableParameterMaintenance() {
      switch (_mode) {
        case Property2D_Type.LOAD:
          return;
        case Property2D_Type.FABRIC:
          SetMaterialInputAt(0);
          break;
        case Property2D_Type.SHELL:
        case Property2D_Type.PL_STRESS:
        case Property2D_Type.PLATE:
        case Property2D_Type.CURVED_SHELL:
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

    // shitty methods - extracted it to updateDropDown and UpdateParameters but then
    // using updateParameter in UpdateUIFromSelectedItems brokes tests...so left for now
    private void Mode1Clicked() {
      if (_mode == Property2D_Type.PL_STRESS)
        return;

      RecordUndoEvent("Plane Stress Parameters");
      if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Property2D_Type.PL_STRESS;
    }

    private void Mode2Clicked() {
      if (_mode == Property2D_Type.FABRIC)
        return;

      RecordUndoEvent("Fabric Parameters");
      _mode = Property2D_Type.FABRIC;

      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      Params.RegisterInputParam(new Param_GenericObject());
    }

    private void Mode3Clicked() {
      if (_mode == Property2D_Type.PLATE)
        return;

      RecordUndoEvent("Flat Plate Parameters");
      if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Property2D_Type.PLATE;
    }

    private void Mode4Clicked() {
      if (_mode == Property2D_Type.SHELL)
        return;

      RecordUndoEvent("Shell Parameters");
      if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Property2D_Type.SHELL;
    }

    private void Mode5Clicked() {
      if (_mode == Property2D_Type.CURVED_SHELL)
        return;

      RecordUndoEvent("Curved Shell Parameters");
      if (_mode == Property2D_Type.LOAD || _mode == Property2D_Type.FABRIC) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = Property2D_Type.CURVED_SHELL;
    }

    private void Mode6Clicked() {
      if (_mode == Property2D_Type.LOAD)
        return;

      RecordUndoEvent("Load Panel Parameters");
      _mode = Property2D_Type.LOAD;

      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);
    }
  }
}
