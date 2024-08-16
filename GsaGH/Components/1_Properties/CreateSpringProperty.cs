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
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateSpringProperty;
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

    public CreateSpringProperty() : base("Create Spring Property", "Spring", "Create a GSA Spring Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;

      foreach (string unitstring in FilteredRotationalStiffnessUnits) {
        _rotationalStiffnessUnitAbbreviations.Add(RotationalStiffness.GetAbbreviation((RotationalStiffnessUnit)Enum.Parse(typeof(RotationalStiffnessUnit), unitstring)));
      }
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      SpringPropertyType mode = GetModeBy(_selectedItems[0]);
      if (i == 0) {
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }
      ParseSelectedItems(mode);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string lengthAbr = Length.GetAbbreviation(_lengthUnit);
      string rotationalStiffnessAbr = RotationalStiffness.GetAbbreviation(_rotationalStiffnessUnit);
      string stiffnessAbr = ForcePerLength.GetAbbreviation(_stiffnessUnit);

      switch (_mode) {
        case SpringPropertyType.Axial:
        case SpringPropertyType.TensionOnly:
        case SpringPropertyType.CompressionOnly:
        case SpringPropertyType.Gap:
          SetStiffnessInputAt(1);
          SetDampingRatioInputAt(2);
          return;

        case SpringPropertyType.Torsional:
          SetInputProperties(1, "Stiffness xx [" + rotationalStiffnessAbr + "]", "Sxx", "Stiffness in xx direction", false);
          SetDampingRatioInputAt(2);
          return;

        case SpringPropertyType.General:
          SetInputProperties(1, "Spring Curve x", "SCx", "[Optional] Spring Curve in x direction");
          SetInputProperties(2, "Stiffness x [" + stiffnessAbr + "]", "Sx", "[Optional] Stiffness in x direction");
          SetInputProperties(3, "Spring Curve y", "SCy", "[Optional] Spring Curve in y direction");
          SetInputProperties(4, "Stiffness y [" + stiffnessAbr + "]", "Sy", "[Optional] Stiffness in y direction");
          SetInputProperties(5, "Spring Curve z", "SCz", "[Optional] Spring Curve in z direction");
          SetInputProperties(6, "Stiffness z [" + stiffnessAbr + "]", "Sz", "[Optional] Stiffness in z direction");
          SetInputProperties(7, "Spring Curve xx", "SCxx", "[Optional] Spring Curve in xx direction");
          SetInputProperties(8, "Stiffness xx [" + rotationalStiffnessAbr + "]", "Sxx", "[Optional] Stiffness in xx direction");
          SetInputProperties(9, "Spring Curve yy", "SCyy", "[Optional] Spring Curve in yy direction");
          SetInputProperties(10, "Stiffness yy [" + rotationalStiffnessAbr + "]", "Syy", "[Optional] Stiffness in yy direction");
          SetInputProperties(11, "Spring Curve zz", "SCzz", "[Optional] Spring Curve in zz direction");
          SetInputProperties(12, "Stiffness zz [" + rotationalStiffnessAbr + "]", "Szz", "[Optional] Stiffness in zz direction");
          SetDampingRatioInputAt(13);
          return;

        case SpringPropertyType.Matrix:
          SetInputProperties(1, "Spring Matrix", "SM", "Spring Matrix", false);
          SetDampingRatioInputAt(2);
          return;

        case SpringPropertyType.Connector:
          SetDampingRatioInputAt(1);
          return;

        case SpringPropertyType.Lockup:
          SetStiffnessInputAt(1, true);
          SetInputProperties(2, "Lockup -ve [" + lengthAbr + "]", "L-ve", "Lockup -ve", false);
          SetInputProperties(3, "Lockup +ve [" + lengthAbr + "]", "L+ve", "Lockup +ve", false);
          SetDampingRatioInputAt(4);
          return;

        case SpringPropertyType.Friction:
          SetInputProperties(1, "Stiffness x [" + stiffnessAbr + "]", "Sx", "Stiffness in x direction", false);
          SetInputProperties(2, "Stiffness y [" + stiffnessAbr + "]", "Sy", "Stiffness in y direction", false);
          SetInputProperties(3, "Stiffness z [" + stiffnessAbr + "]", "Sz", "Stiffness in z direction", false);
          SetInputProperties(4, "Coeff. of Friction [" + stiffnessAbr + "]", "CF", "Coefficient of Friction", false);
          SetDampingRatioInputAt(5);
          return;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Stiffness Unit"
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_springPropertyTypes.Values.ToList());
      _selectedItems.Add(_springPropertyTypes.Values.ElementAt(0));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_stiffnessUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Name", "Na", "[Optional] Spring Property Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Stiffness [" + ForcePerLength.GetAbbreviation(_stiffnessUnit) + "]", "S", "Axial Stiffness", GH_ParamAccess.item);
      pManager.AddNumberParameter("Damping Ratio", "DR", "[Optional] Damping Ratio (Default = 0.0 -> 0%)", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSpringPropertyParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaSpringProperty spring;

      double stiffness = 0;
      switch (_mode) {
        case SpringPropertyType.Axial:
        case SpringPropertyType.TensionOnly:
        case SpringPropertyType.CompressionOnly:
        case SpringPropertyType.Lockup:
        case SpringPropertyType.Gap:
          stiffness = Input.UnitNumber(this, da, 1, _stiffnessUnit).As(ForcePerLengthUnit.NewtonPerMeter);
          break;

        case SpringPropertyType.Torsional:
          stiffness = Input.UnitNumber(this, da, 1, _rotationalStiffnessUnit).As(RotationalStiffnessUnit.NewtonMeterPerRadian);
          break;
      }

      int dampingRatioIndex = 2;

      switch (_mode) {
        case SpringPropertyType.Axial:
          var axialProperty = new AxialSpringProperty {
            Stiffness = stiffness
          };
          spring = new GsaSpringProperty(axialProperty);
          break;

        case SpringPropertyType.Torsional:
          var torsionalProperty = new TorsionalSpringProperty {
            Stiffness = stiffness
          };
          spring = new GsaSpringProperty(torsionalProperty);
          break;

        case SpringPropertyType.General:
          dampingRatioIndex = 13;

          int? springCurveX = null;
          int? springCurveY = null;
          int? springCurveZ = null;
          int? springCurveXX = null;
          int? springCurveYY = null;
          int? springCurveZZ = null;
          da.GetData(1, ref springCurveX);
          da.GetData(3, ref springCurveY);
          da.GetData(5, ref springCurveZ);
          da.GetData(7, ref springCurveXX);
          da.GetData(9, ref springCurveYY);
          da.GetData(11, ref springCurveZZ);

          double? stiffnessX = null;
          double? stiffnessY = null;
          double? stiffnessZ = null;
          double? stiffnessXX = null;
          double? stiffnessYY = null;
          double? stiffnessZZ = null;

          var gh_typ = new GH_ObjectWrapper();
          if (da.GetData(2, ref gh_typ)) {
            stiffnessX = Input.UnitNumber(this, da, 2, _stiffnessUnit).As(ForcePerLengthUnit.NewtonPerMeter);
          }
          if (da.GetData(4, ref gh_typ)) {
            stiffnessY = Input.UnitNumber(this, da, 4, _stiffnessUnit).As(ForcePerLengthUnit.NewtonPerMeter);
          }
          if (da.GetData(6, ref gh_typ)) {
            stiffnessZ = Input.UnitNumber(this, da, 6, _stiffnessUnit).As(ForcePerLengthUnit.NewtonPerMeter);
          }
          if (da.GetData(8, ref gh_typ)) {
            stiffnessXX = Input.UnitNumber(this, da, 8, _rotationalStiffnessUnit).As(RotationalStiffnessUnit.NewtonMeterPerRadian);
          }
          if (da.GetData(10, ref gh_typ)) {
            stiffnessYY = Input.UnitNumber(this, da, 10, _rotationalStiffnessUnit).As(RotationalStiffnessUnit.NewtonMeterPerRadian);
          }
          if (da.GetData(12, ref gh_typ)) {
            stiffnessZZ = Input.UnitNumber(this, da, 12, _rotationalStiffnessUnit).As(RotationalStiffnessUnit.NewtonMeterPerRadian);
          }

          var generalProperty = new GeneralSpringProperty {
            SpringCurveX = springCurveX,
            SpringCurveY = springCurveY,
            SpringCurveZ = springCurveZ,
            SpringCurveXX = springCurveXX,
            SpringCurveYY = springCurveYY,
            SpringCurveZZ = springCurveZZ,
            StiffnessX = stiffnessX,
            StiffnessY = stiffnessY,
            StiffnessZ = stiffnessZ,
            StiffnessXX = stiffnessXX,
            StiffnessYY = stiffnessYY,
            StiffnessZZ = stiffnessZZ,
          };
          spring = new GsaSpringProperty(generalProperty);
          break;

        case SpringPropertyType.Matrix:
          int matrix = 0;
          if (da.GetData(1, ref matrix)) {
            var matrixProperty = new MatrixSpringProperty {
              SpringMatrix = matrix
            };
            spring = new GsaSpringProperty(matrixProperty);
          } else {
            this.AddRuntimeError("Input SM failed to collect data");
            return;
          }

          break;

        case SpringPropertyType.TensionOnly:
          var tensionProperty = new TensionSpringProperty {
            Stiffness = stiffness
          };
          spring = new GsaSpringProperty(tensionProperty);
          break;

        case SpringPropertyType.CompressionOnly:
          var compressionProperty = new CompressionSpringProperty {
            Stiffness = stiffness
          };
          spring = new GsaSpringProperty(compressionProperty);
          break;

        case SpringPropertyType.Connector:
          dampingRatioIndex = 1;

          var connectorProperty = new ConnectorSpringProperty();
          spring = new GsaSpringProperty(connectorProperty);
          break;

        case SpringPropertyType.Lockup:
          dampingRatioIndex = 4;

          double negativeLockup = Input.UnitNumber(this, da, 2, _lengthUnit).As(LengthUnit.Meter);
          double positiveLockup = Input.UnitNumber(this, da, 3, _lengthUnit).As(LengthUnit.Meter);
          var lockupProperty = new LockupSpringProperty {
            Stiffness = stiffness,
            NegativeLockup = negativeLockup,
            PositiveLockup = positiveLockup
          };
          spring = new GsaSpringProperty(lockupProperty);
          break;

        case SpringPropertyType.Gap:
          var gapProperty = new GapSpringProperty() {
            Stiffness = stiffness
          };
          spring = new GsaSpringProperty(gapProperty);
          break;

        case SpringPropertyType.Friction:
          dampingRatioIndex = 5;

          double frictionStiffnessX = Input.UnitNumber(this, da, 1, _stiffnessUnit).As(ForcePerLengthUnit.NewtonPerMeter);
          double frictionStiffnessY = Input.UnitNumber(this, da, 2, _stiffnessUnit).As(ForcePerLengthUnit.NewtonPerMeter);
          double frictionStiffnessZ = Input.UnitNumber(this, da, 3, _stiffnessUnit).As(ForcePerLengthUnit.NewtonPerMeter);
          double frictionCoefficient = 0;
          if (da.GetData(4, ref frictionCoefficient)) {
            var frictionProperty = new FrictionSpringProperty {
              StiffnessX = frictionStiffnessX,
              StiffnessY = frictionStiffnessY,
              StiffnessZ = frictionStiffnessZ,
              FrictionCoefficient = frictionCoefficient
            };
            spring = new GsaSpringProperty(frictionProperty);
          } else {
            this.AddRuntimeError("Input CF failed to collect data");
            return;
          }
          break;

        default:
          return;
      }

      string name = string.Empty;
      if (da.GetData(0, ref name)) {
        spring.ApiProperty.Name = name;
      }

      double dampingRatio = 0;
      if (da.GetData(dampingRatioIndex, ref dampingRatio)) {
        spring.ApiProperty.DampingRatio = dampingRatio;
      }

      da.SetData(0, new GsaSpringPropertyGoo(spring));
    }

    protected override void UpdateUIFromSelectedItems() {
      SpringPropertyType mode = GetModeBy(_selectedItems[0]);

      ParseSelectedItems(mode);
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
      throw new ArgumentException("Unable to convert " + name + " to Spring Property Type");
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

    private void SetInputProperties(int index, string name, string nickname, string description,
      bool optional = true) {
      Params.Input[index].Name = name;
      Params.Input[index].NickName = nickname;
      Params.Input[index].Description = description;
      Params.Input[index].Access = GH_ParamAccess.item;
      Params.Input[index].Optional = optional;
    }

    private void SetDampingRatioInputAt(int index) {
      SetInputProperties(index, "Damping Ratio", "DR", "[Optional] Damping Ratio (Default = 0.0 -> 0%)");
    }

    private void ParseSelectedItems(SpringPropertyType mode) {
      switch (mode) {
        case SpringPropertyType.Axial:
        case SpringPropertyType.CompressionOnly:
        case SpringPropertyType.Friction:
        case SpringPropertyType.Gap:
        case SpringPropertyType.General:
        case SpringPropertyType.Lockup:
        case SpringPropertyType.TensionOnly:
          _stiffnessUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
          break;

        case SpringPropertyType.Connector:
        case SpringPropertyType.Matrix:
          // do nothing
          return;
      }

      if (mode == SpringPropertyType.General) {
        _rotationalStiffnessUnit = (RotationalStiffnessUnit)UnitsHelper.Parse(typeof(RotationalStiffnessUnit), _selectedItems[2]);
      } else if (mode == SpringPropertyType.Lockup) {
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[2]);
      } else if (mode == SpringPropertyType.Torsional) {
        _rotationalStiffnessUnit = (RotationalStiffnessUnit)UnitsHelper.Parse(typeof(RotationalStiffnessUnit), _selectedItems[1]);
      }
    }

    private void SetStiffnessInputAt(int index, bool optional = false) {
      string description = string.Empty;
      if (optional) {
        description += "[Optional]";
      }
      description += " Axial Stiffness";
      SetInputProperties(index, "Stiffness [" + ForcePerLength.GetAbbreviation(_stiffnessUnit) + "]", "S", description, optional);
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

      if (mode == SpringPropertyType.Lockup) {
        AddLengthUnitDropDown();
      }
    }

    private void UpdateParameters(SpringPropertyType mode) {
      if (mode == _mode) {
        return;
      }

      _springPropertyTypes.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      while (Params.Input.Count > 1) {
        Params.UnregisterInputParameter(Params.Input[1], true);
      }

      switch (mode) {
        case SpringPropertyType.Axial:
        case SpringPropertyType.TensionOnly:
        case SpringPropertyType.CompressionOnly:
        case SpringPropertyType.Gap:
        case SpringPropertyType.Torsional:
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Number());
          break;

        case SpringPropertyType.General:
          Params.RegisterInputParam(new Param_Integer());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Integer());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Integer());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Integer());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Integer());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Integer());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Number());
          break;

        case SpringPropertyType.Matrix:
          Params.RegisterInputParam(new Param_Integer());
          Params.RegisterInputParam(new Param_Number());
          break;

        case SpringPropertyType.Connector:
          Params.RegisterInputParam(new Param_Number());
          break;

        case SpringPropertyType.Lockup:
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Number());
          Params.RegisterInputParam(new Param_Number());
          Params.RegisterInputParam(new Param_Number());
          break;

        case SpringPropertyType.Friction:
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Number());
          Params.RegisterInputParam(new Param_Number());
          break;
      }

      _mode = mode;
    }
  }
}
