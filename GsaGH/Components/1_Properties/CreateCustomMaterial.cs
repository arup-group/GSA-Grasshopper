using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a new Material
  /// </summary>
  public class CreateCustomMaterial : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("83bfce91-9204-4fe4-b81d-0036babf0c6d");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CustomMaterial;

    public CreateCustomMaterial() : base("Custom Material",
      "Material",
      "Create a Custom GSA Analysis Material",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
      Hidden = true;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(_stressUnit);
      string densityUnitAbbreviation = Density.GetAbbreviation(_densityUnit);
      string temperatureUnitAbbreviation = Temperature.GetAbbreviation(_temperatureUnit);

      pManager.AddIntegerParameter("Analysis Property Number", "ID", "Analysis Property Number (do not use 0 -> 'from Grade')", GH_ParamAccess.item);
      pManager.AddGenericParameter("Elastic Modulus [" + stressUnitAbbreviation + "]", "E", "Elastic Modulus of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddNumberParameter("Poisson's Ratio", "ν", "Poisson's Ratio of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + densityUnitAbbreviation + "]", "ρ", "Density of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thermal Expansion [/" + temperatureUnitAbbreviation + "]", "α", "Thermal Expansion Coefficient of the elastic isotropic material", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat", "GSA Custom Material", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var material = new GsaMaterial();

      var ghAnal = new GH_Integer();
      if (da.GetData(0, ref ghAnal)) {
        GH_Convert.ToInt32(ghAnal, out int anal, GH_Conversion.Both);
        material.AnalysisProperty = anal;
        if (anal == 0) {
          this.AddRuntimeError("Analysis Material ID cannot be 0 - that is 'from Grade'. " +
               Environment.NewLine + "Leave blank or use -1 for automatic assigning.");
          return;
        }
      }
      else {
        material.AnalysisProperty = -1;
      }

      double poisson = 0.3;
      da.GetData(2, ref poisson);

      CoefficientOfThermalExpansionUnit thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
      switch (_temperatureUnit) {
        case TemperatureUnit.DegreeFahrenheit:
          thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeFahrenheit;
          break;
        case TemperatureUnit.Kelvin:
          thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseKelvin;
          break;
      }

      material.AnalysisMaterial = new AnalysisMaterial() {
        ElasticModulus = Input.UnitNumber(this, da, 1, _stressUnit).As(PressureUnit.Pascal),
        PoissonsRatio = poisson,
        Density = Input.UnitNumber(this, da, 3, _densityUnit).As(DensityUnit.KilogramPerCubicMeter),
        CoefficientOfThermalExpansion = Input.UnitNumber(this, da, 4, thermalExpansionUnit, true).As(CoefficientOfThermalExpansionUnit.InverseDegreeCelsius)
      };

      material.GradeProperty = 0;

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

    #region Custom UI
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
    private FoldMode _mode = FoldMode.Timber;

    private DensityUnit _densityUnit = DefaultUnits.DensityUnit;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;
    private TemperatureUnit _temperatureUnit = DefaultUnits.TemperatureUnit;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
        {
          "Material Type",
          "Stress Unit",
          "Density Unit",
          "Temperature Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(CreateMaterial.MaterialTypes);
      SelectedItems.Add(_mode.ToString());

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      SelectedItems.Add(Pressure.GetAbbreviation(_stressUnit));

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      SelectedItems.Add(Density.GetAbbreviation(_densityUnit));

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Temperature));
      SelectedItems.Add(Temperature.GetAbbreviation(_temperatureUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0) {
        _mode = (FoldMode)Enum.Parse(typeof(FoldMode), SelectedItems[0]);
      }
      else {
        switch (i) {
          case 1:
            _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), SelectedItems[1]);
            break;
          case 2:
            _densityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), SelectedItems[2]);
            break;
          case 3:
            _temperatureUnit = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), SelectedItems[3]);
            break;
        }
      }

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), SelectedItems[0]);
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), SelectedItems[1]);
      _densityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), SelectedItems[2]);
      _temperatureUnit = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), SelectedItems[3]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(_stressUnit);
      string densityUnitAbbreviation = Density.GetAbbreviation(_densityUnit);
      string temperatureUnitAbbreviation = Temperature.GetAbbreviation(_temperatureUnit);

      int i = 1;
      Params.Input[i].Name = "Elastic Modulus [" + stressUnitAbbreviation + "]";
      i++;
      i++;
      Params.Input[i].Name = "Density [" + densityUnitAbbreviation + "]";
      i++;
      Params.Input[i].Name = "Thermal Expansion [/" + temperatureUnitAbbreviation + "]";
    }
    #endregion
  }
}
