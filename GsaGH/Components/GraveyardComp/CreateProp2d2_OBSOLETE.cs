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
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Prop2d
  /// </summary>
  public class CreateProp2d2_OBSOLETE : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("d693b4ad-7aaf-450e-a436-afbb9d2061fc");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Create2dProperty;
    private readonly IReadOnlyDictionary<Prop2dType, string> _dropdownTopLevel
      = new Dictionary<Prop2dType, string> {
        {
          Prop2dType.PlaneStress, "Plane Stress"
        }, {
          Prop2dType.Fabric, "Fabric"
        }, {
          Prop2dType.FlatPlate, "Flat Plate"
        }, {
          Prop2dType.Shell, "Shell"
        }, {
          Prop2dType.CurvedShell, "Curved Shell"
        }, {
          Prop2dType.LoadPanel, "Load Panel"
        },
      };
    private readonly IReadOnlyDictionary<SupportType, string> _supportDropDown
      = new Dictionary<SupportType, string> {
        {
          SupportType.Auto, "Automatic"
        }, {
          SupportType.AllEdges, "All edges"
        }, {
          SupportType.ThreeEdges, "Three edges"
        }, {
          SupportType.TwoEdges, "Two edges"
        }, {
          SupportType.TwoAdjacentEdges, "Two adjacent edges"
        }, {
          SupportType.OneEdge, "One edge"
        }, {
          SupportType.Cantilever, "Cantilever"
        },
      };
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private Prop2dType _mode = Prop2dType.Shell;
    private int _supportTypeIndex;

    public CreateProp2d2_OBSOLETE() : base("Create 2D Property", "Prop2d", "Create GSA 2D Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      Prop2dType mode = GetModeBy(_selectedItems[0]);
      if (i == 0) {
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }

      if (i != 0 && mode != Prop2dType.LoadPanel) {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      }

      if (i == 1 && mode == Prop2dType.LoadPanel) {
        _supportTypeIndex = j;
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      switch (_mode) {
        case Prop2dType.LoadPanel:
          if (_supportTypeIndex != _supportDropDown.Keys.ToList().IndexOf(SupportType.Auto)
            && _supportTypeIndex != _supportDropDown.Keys.ToList().IndexOf(SupportType.AllEdges)) {
            SetReferenceEdgeInputAt(0);
          }

          return;

        case Prop2dType.Fabric:
          SetMaterialInputAt(0);
          break;

        case Prop2dType.Shell:
        case Prop2dType.PlaneStress:
        case Prop2dType.FlatPlate:
        case Prop2dType.CurvedShell:
          SetInputProperties(0, "Thk", $"Thickness [{Length.GetAbbreviation(_lengthUnit)}]",
            "Section thickness", optional: false);
          SetMaterialInputAt(1);
          break;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_dropdownTopLevel.Values.ToList());
      _selectedItems.Add(_dropdownTopLevel.Values.ElementAt(3));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Thk",
        "Section thickness", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProperty2dParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var prop = new GsaProperty2d();
      switch (_mode) {
        case Prop2dType.PlaneStress:
        case Prop2dType.Fabric:
        case Prop2dType.FlatPlate:
        case Prop2dType.Shell:
        case Prop2dType.CurvedShell:
        case Prop2dType.LoadPanel:
          prop.ApiProp2d.Type = (Property2D_Type)(int)_mode;
          break;

        default:
          this.AddRuntimeWarning("Property type is undefined");
          prop.ApiProp2d.Type = Property2D_Type.UNDEF;
          break;
      }

      if (_mode != Prop2dType.LoadPanel) {
        prop.ApiProp2d.AxisProperty = 0;

        if (_mode != Prop2dType.Fabric) {
          prop.Thickness = (Length)Input.UnitNumber(this, da, 0, _lengthUnit);
          var ghTyp = new GH_ObjectWrapper();
          if (da.GetData(1, ref ghTyp)) {
            if (ghTyp.Value is GsaMaterialGoo materialGoo) {
              prop.Material = materialGoo.Value.Duplicate() ?? new GsaMaterial();
            } else {
              if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both)) {
                prop.Material = new GsaMaterial(idd);
              } else {
                this.AddRuntimeError(
                  "Unable to convert PB input to a Section Property of reference integer");
                return;
              }
            }
          } else {
            prop.Material = new GsaMaterial(2);
          }
        } else {
          prop.Material = new GsaMaterial(8);
        }
      } else {
        prop.ApiProp2d.SupportType = _supportDropDown.FirstOrDefault(x => x.Value == _selectedItems[1]).Key;
        if (prop.ApiProp2d.SupportType != SupportType.Auto && prop.ApiProp2d.SupportType != SupportType.AllEdges) {
          int referenceEdge = 0;
          if (da.GetData("Reference edge", ref referenceEdge) && referenceEdge > 0
            && referenceEdge <= 4) {
            prop.ApiProp2d.ReferenceEdge = referenceEdge;
          } else {
            this.AddRuntimeWarning("Input RE failed to collect data");
          }
        }
      }

      da.SetData(0, new GsaProperty2dGoo(prop));
    }

    protected override void UpdateUIFromSelectedItems() {
      Prop2dType mode = GetModeBy(_selectedItems[0]);

      if (mode == Prop2dType.LoadPanel) {
        _supportTypeIndex = _supportDropDown.ToList().FindIndex(x => x.Value == _selectedItems[1]);
      } else if (mode != Prop2dType.Fabric) {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      }

      UpdateDropDownItems(mode);
      UpdateParameters(mode);

      base.UpdateUIFromSelectedItems();
    }

    private void AddLengthUnitDropDown() {
      _spacerDescriptions.Add("Unit");
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));
    }

    private void AddSupportTypeDropDown() {
      if (_supportTypeIndex < 0 || _supportTypeIndex >= _supportDropDown.Count) {
        this.AddRuntimeError("Index for selected items out of range!");
        _supportTypeIndex = 0;
      }

      _spacerDescriptions.Add("Support Type");
      _dropDownItems.Add(_supportDropDown.Values.ToList());
      _selectedItems.Add(_supportDropDown.Values.ElementAt(_supportTypeIndex));
    }

    private Prop2dType GetModeBy(string name) {
      foreach (KeyValuePair<Prop2dType, string> item in _dropdownTopLevel) {
        if (item.Value.Equals(name)) {
          return item.Key;
        }
      }

      throw new Exception("Unable to convert " + name + " to Prop2d Type");
    }

    private void ResetDropdownMenus() {
      while (_spacerDescriptions.Count > 1) {
        _spacerDescriptions.RemoveAt(_spacerDescriptions.Count - 1);
      }

      while (_dropDownItems.Count > 1) {
        _dropDownItems.RemoveAt(_dropDownItems.Count - 1);
      }

      while (_selectedItems.Count > 1) {
        _selectedItems.RemoveAt(_selectedItems.Count - 1);
      }
    }

    private void SetInputProperties(
      int index, string nickname, string name, string description,
      bool optional = true) {
      Params.Input[index].NickName = nickname;
      Params.Input[index].Name = name;
      Params.Input[index].Description = description;
      Params.Input[index].Access = GH_ParamAccess.item;
      Params.Input[index].Optional = optional;
    }

    private void SetMaterialInputAt(int index) {
      SetInputProperties(index, "Mat", "Material", "GSA Material");
    }

    private void SetReferenceEdgeInputAt(int index) {
      SetInputProperties(index, "RE", "Reference edge",
        "Reference edge for automatic support type");
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

        case Prop2dType.Fabric: break;

        case Prop2dType.LoadPanel:
          AddSupportTypeDropDown();
          break;
      }
    }

    private void UpdateParameters(Prop2dType mode) {
      if (_mode == mode && mode != Prop2dType.LoadPanel) {
        return;
      }

      _dropdownTopLevel.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      while (Params.Input.Count > 0) {
        Params.UnregisterInputParameter(Params.Input[0], true);
      }

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
          if (_supportTypeIndex != _supportDropDown.Keys.ToList().IndexOf(SupportType.Auto)
            && _supportTypeIndex != _supportDropDown.Keys.ToList().IndexOf(SupportType.AllEdges)) {
            Params.RegisterInputParam(new Param_Integer());
          }

          break;
      }

      _mode = mode;
    }
  }
}