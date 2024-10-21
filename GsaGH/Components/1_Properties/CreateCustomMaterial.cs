using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaAPI.Materials;

using GsaGH.Helpers;
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
  ///   Component to create a new Material
  /// </summary>
  public class CreateCustomMaterial : GH_OasysDropDownComponent {
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
    public override Guid ComponentGuid => new Guid("f2906b65-208f-4a46-8e1f-06d6270cc90c");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateCustomMaterial;
    private DensityUnit _densityUnit = DefaultUnits.DensityUnit;
    private FoldMode _mode = FoldMode.Timber;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;
    private TemperatureUnit _temperatureUnit = DefaultUnits.TemperatureUnit;

    public CreateCustomMaterial() : base("Create Custom Material", "Material",
      "Create a Custom GSA Analysis Material",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0].ToPascalCase());
      } else {
        switch (i) {
          case 1:
            _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
            break;

          case 2:
            _densityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[2]);
            break;

          case 3:
            _temperatureUnit
              = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[3]);
            break;
        }
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(_stressUnit);
      string densityUnitAbbreviation = Density.GetAbbreviation(_densityUnit);
      string temperatureUnitAbbreviation = Temperature.GetAbbreviation(_temperatureUnit);

      int i = 2;
      Params.Input[i].Name = "Elastic Modulus [" + stressUnitAbbreviation + "]";
      i++;
      i++;
      Params.Input[i].Name = "Density [" + densityUnitAbbreviation + "]";
      i++;
      Params.Input[i].Name = "Thermal Expansion [/" + temperatureUnitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Material Type",
        "Stress Unit",
        "Density Unit",
        "Temperature Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(MaterialTypes);
      _selectedItems.Add(_mode.ToString());

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(_stressUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      _selectedItems.Add(Density.GetAbbreviation(_densityUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Temperature));
      _selectedItems.Add(Temperature.GetAbbreviation(_temperatureUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(_stressUnit);
      string densityUnitAbbreviation = Density.GetAbbreviation(_densityUnit);
      string temperatureUnitAbbreviation = Temperature.GetAbbreviation(_temperatureUnit);

      pManager.AddIntegerParameter("Analysis Property Number", "ID",
        "Analysis Property Number (do not use 0 -> 'from Grade')", GH_ParamAccess.item);
      pManager.AddTextParameter("Material Name", "Na", "Material Name of Custom Material",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Elastic Modulus [" + stressUnitAbbreviation + "]", "E",
        "Elastic Modulus of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddNumberParameter("Poisson's Ratio", "ν",
        "Poisson's Ratio of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + densityUnitAbbreviation + "]", "ρ",
        "Density of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thermal Expansion [/" + temperatureUnitAbbreviation + "]", "α",
        "Thermal Expansion Coefficient of the elastic isotropic material", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat", "GSA Custom Material",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      int id = 0;
      da.GetData(0, ref id);

      string name = string.Empty;
      da.GetData(1, ref name);

      double poisson = 0.3;
      da.GetData(3, ref poisson);

      CoefficientOfThermalExpansionUnit thermalExpansionUnit
        = CoefficientOfThermalExpansionUnit.PerDegreeCelsius;
      switch (_temperatureUnit) {
        case TemperatureUnit.DegreeFahrenheit:
          thermalExpansionUnit = CoefficientOfThermalExpansionUnit.PerDegreeFahrenheit;
          break;

        case TemperatureUnit.Kelvin:
          thermalExpansionUnit = CoefficientOfThermalExpansionUnit.PerKelvin;
          break;
      }

      var analysisMaterial = new AnalysisMaterial() {
        ElasticModulus = Input.UnitNumber(this, da, 2, _stressUnit).As(PressureUnit.Pascal),
        PoissonsRatio = poisson,
        Density = Input.UnitNumber(this, da, 4, _densityUnit).As(DensityUnit.KilogramPerCubicMeter),
        CoefficientOfThermalExpansion = Input.UnitNumber(this, da, 5, thermalExpansionUnit, true)
        .As(CoefficientOfThermalExpansionUnit.PerDegreeCelsius),
        Name = name,
      };

      GsaMaterial material = null;

      switch (_mode) {
        case FoldMode.Generic:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Custom);
          break;

        case FoldMode.Steel:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Steel);
          break;

        case FoldMode.Concrete:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Concrete);
          break;

        case FoldMode.Timber:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Timber);
          break;

        case FoldMode.Aluminium:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Aluminium);
          break;

        case FoldMode.Frp:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Frp);
          break;

        case FoldMode.Glass:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Glass);
          break;

        case FoldMode.Fabric:
          material = new GsaCustomMaterial(analysisMaterial, id, MatType.Fabric);
          break;
      }



      da.SetData(0, new GsaMaterialGoo(material));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0].ToPascalCase());
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      _densityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[2]);
      _temperatureUnit
        = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[3]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
