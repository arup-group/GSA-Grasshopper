using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  internal enum SpringPropertyType {
    Axial,
    Torsional,
    General,
    Matrix,
    TensionOnly,
    CompressionOnly,
    Connector,
    Lockup,
    Gap,
    Friction
  }

  /// <summary>
  /// Component to create a new SpringProperty
  /// </summary>
  public class CreateSpringProperty : GH_OasysDropDownComponent {
    public static List<string> FilteredRotationalStiffnessUnits = new List<string>(new[] {
      RotationalStiffnessUnit.NewtonMeterPerDegree.ToString(),
      RotationalStiffnessUnit.NewtonMeterPerRadian.ToString(),
      RotationalStiffnessUnit.NewtonMillimeterPerDegree.ToString(),
      RotationalStiffnessUnit.NewtonMillimeterPerRadian.ToString(),
      RotationalStiffnessUnit.KilonewtonMeterPerDegree.ToString(),
      RotationalStiffnessUnit.KilonewtonMeterPerRadian.ToString(),
      RotationalStiffnessUnit.KilonewtonMillimeterPerDegree.ToString(),
      RotationalStiffnessUnit.KilonewtonMillimeterPerRadian.ToString(),
      RotationalStiffnessUnit.MeganewtonMeterPerDegree.ToString(),
      RotationalStiffnessUnit.MeganewtonMeterPerRadian.ToString(),
      RotationalStiffnessUnit.MeganewtonMillimeterPerDegree.ToString(),
      RotationalStiffnessUnit.MeganewtonMillimeterPerRadian.ToString(),
      RotationalStiffnessUnit.PoundForceFeetPerRadian.ToString(),
      RotationalStiffnessUnit.PoundForceFootPerDegrees.ToString(),
      RotationalStiffnessUnit.KilopoundForceFootPerDegrees.ToString()
    });

    public override Guid ComponentGuid => new Guid("f48965a0-00e7-4de8-9839-a4480075459f");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => null;
    private readonly IReadOnlyDictionary<SpringPropertyType, string> _springPropertyTypes
      = new Dictionary<SpringPropertyType, string> {
        { SpringPropertyType.Axial, "Axial" },
        { SpringPropertyType.CompressionOnly, "Compression-only" },
        { SpringPropertyType.Connector, "Connector" },
        { SpringPropertyType.Friction, "Friction" },
        { SpringPropertyType.Gap, "Gap" },
        { SpringPropertyType.General, "General" },
        { SpringPropertyType.Lockup, "Lockup" },
        { SpringPropertyType.Matrix, "Matrix" },
        { SpringPropertyType.TensionOnly, "Tension-only" },
        { SpringPropertyType.Torsional, "Torsional" },
      };
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private RotationalStiffnessUnit _rotationalStiffnessUnit = RotationalStiffnessUnit.NewtonMeterPerRadian;
    private List<string> _rotationalStiffnessUnitAbbreviations = new List<string>();
    private ForcePerLengthUnit _stiffnessUnit = DefaultUnits.ForcePerLengthUnit;
    private SpringPropertyType _mode = SpringPropertyType.Axial;


    private int _supportTypeIndex;

    public CreateSpringProperty() : base("Create Spring Property", "Spring", "Create a GSA Spring Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;

      foreach (string unitstring in FilteredRotationalStiffnessUnits) {
        _rotationalStiffnessUnitAbbreviations.Add(ForcePerLength.GetAbbreviation((ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), unitstring)));
      }
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      SpringPropertyType mode = GetModeBy(_selectedItems[0]);
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
        case SpringPropertyType.Axial:

          return;

        case SpringPropertyType.CompressionOnly:
          SetMaterialInputAt(0);
          return;

        case SpringPropertyType.Connector:
        case SpringPropertyType.Friction:
        case SpringPropertyType.Gap:
        case SpringPropertyType.General:
          SetInputProperties(0, "Thk", $"Thickness [{Length.GetAbbreviation(_lengthUnit)}]",
            "Section thickness", optional: false);
          SetMaterialInputAt(1);

          if (_mode != SpringPropertyType.General) {
            SetInputProperties(2, "RS", "Reference Surface",
              "Reference Surface Middle = 0 (default), Top = 1, Bottom = 2", optional: true);
            SetInputProperties(3, "Off", $"Offset [{Length.GetAbbreviation(_lengthUnit)}]",
              "Additional Offset", optional: true);
          }
          return;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Stiffness Unit",
        //"Length Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_springPropertyTypes.Values.ToList());
      _selectedItems.Add(_springPropertyTypes.Values.ElementAt(0));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_stiffnessUnit));

      //_dropDownItems.Add(abbreviations);
      //_selectedItems.Add(RotationalStiffness.GetAbbreviation(_rotationalStiffnessUnit));

      //_dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      //_selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Name", "Na", "[Optional] Spring Property Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Stiffness [foo]", "S", "Axial Stiffness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Damping Ratio", "DR", "[Optional] Damping Ratio (Default = 0.0 -> 0%)", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSpringPropertyParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaSpringProperty spring;

      switch (_mode) {
        case SpringPropertyType.Axial:
          //var _fooUnit = new AxialStiffness();
          //double asdf = Input.UnitNumber(this, da, 1, _fooUnit).As(AxialStiffnessUnit.Newton);

          var property = new AxialSpringProperty {
            Stiffness = 0
          };
          spring = new GsaAxialSpringProperty(property);
          break;

        default:
          return;
      }

      string name = string.Empty;
      if (da.GetData(0, ref name)) {
        spring.ApiProperty.Name = name;
      }




      da.SetData(0, new GsaSpringPropertyGoo(spring));
    }

    protected override void UpdateUIFromSelectedItems() {
      SpringPropertyType mode = GetModeBy(_selectedItems[0]);

      if (mode == SpringPropertyType.Axial) {

      } else if (mode != SpringPropertyType.Gap) {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      }

      UpdateDropDownItems(mode);
      UpdateParameters(mode);

      base.UpdateUIFromSelectedItems();
    }

    private void AddLengthUnitDropDown() {
      _spacerDescriptions.Add("Length Unit");
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));
    }

    private void AddRotationalStiffnessUnitDropDown() {
      _spacerDescriptions.Add("Rotational Stiffness Unit");
      _dropDownItems.Add(_rotationalStiffnessUnitAbbreviations);
      _selectedItems.Add(RotationalStiffness.GetAbbreviation(_rotationalStiffnessUnit));
    }

    private void AddStiffnessUnitDropDown() {
      _spacerDescriptions.Add("Stiffness Unit");
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_stiffnessUnit));
    }

    private SpringPropertyType GetModeBy(string name) {
      foreach (KeyValuePair<SpringPropertyType, string> item in _springPropertyTypes) {
        if (item.Value.Equals(name)) {
          return item.Key;
        }
      }
      throw new Exception("Unable to convert " + name + " to Spring Property Type");
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
      SetInputProperties(index, "RE", "Reference edge", "Reference edge for automatic support type");
    }

    private void UpdateDropDownItems(SpringPropertyType mode) {
      ResetDropdownMenus();

      switch (mode) {
        case SpringPropertyType.Axial:
        case SpringPropertyType.TensionOnly:
        case SpringPropertyType.CompressionOnly:
        case SpringPropertyType.Lockup:
        case SpringPropertyType.Gap:
        case SpringPropertyType.Friction:
          AddStiffnessUnitDropDown();
          break;

        case SpringPropertyType.Torsional:
          AddRotationalStiffnessUnitDropDown();
          break;

        case SpringPropertyType.General:
          AddStiffnessUnitDropDown();
          AddRotationalStiffnessUnitDropDown();
          break;

        case SpringPropertyType.Matrix:
        case SpringPropertyType.Connector:
        default:
          // do nothing
          break;
      }

      if (mode == SpringPropertyType.Gap) {
        AddLengthUnitDropDown();
      }
    }

    private void UpdateParameters(SpringPropertyType mode) {
      if (_mode == mode && mode != Prop2dType.LoadPanel) {
        return;
      }

      _springPropertyTypes.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      while (Params.Input.Count > 0) {
        Params.UnregisterInputParameter(Params.Input[0], true);
      }

      switch (mode) {
        case Prop2dType.Shell:
        case Prop2dType.FlatPlate:
        case Prop2dType.CurvedShell:
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new GsaMaterialParameter());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Number());
          break;

        case Prop2dType.PlaneStress:
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
