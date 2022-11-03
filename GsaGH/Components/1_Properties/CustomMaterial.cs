using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Material
  /// </summary>
  public class CustomMaterial : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("83bfce91-9204-4fe4-b81d-0036babf0c6d");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CustomMaterial;

    public CustomMaterial() : base("Custom Material",
      "Material",
      "Create a Custom GSA Analysis Material",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      string densityUnitAbbreviation = Density.GetAbbreviation(this.DensityUnit);
      string temperatureUnitAbbreviation = Temperature.GetAbbreviation(this.TemperatureUnit);

      pManager.AddIntegerParameter("Analysis Property Number", "ID", "Analysis Property Number (do not use 0 -> 'from Grade')", GH_ParamAccess.item);
      pManager.AddGenericParameter("Elastic Modulus [" + stressUnitAbbreviation + "]", "E", "Elastic Modulus of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddNumberParameter("Poisson's Ratio", "ν", "Poisson's Ratio of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + densityUnitAbbreviation + "]", "ρ", "Density of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thermal Expansion [/" + temperatureUnitAbbreviation + "]", "α", "Thermal Expansion Coefficient of the elastic isotropic material", GH_ParamAccess.item);
      //pManager.AddTextParameter("Material Name", "Na", "Custom Material Name", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[4].Optional = true;
      //pManager[5].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat", "GSA Custom Material", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMaterial material = new GsaMaterial();

      GH_Integer gh_anal = new GH_Integer();
      if (DA.GetData(0, ref gh_anal))
      {
        int anal = 1;
        GH_Convert.ToInt32(gh_anal, out anal, GH_Conversion.Both);
        material.AnalysisProperty = anal;
        if (anal == 0)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Analysis Material ID cannot be 0 - that is 'from Grade'. " +
               System.Environment.NewLine + "Leave blank or use -1 for automatic assigning.");
          return;
        }
      }
      else
      {
        material.AnalysisProperty = -1;
      }

      double poisson = 0.3;
      DA.GetData(2, ref poisson);

      CoefficientOfThermalExpansionUnit thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
      switch (TemperatureUnit)
      {
        case TemperatureUnit.DegreeFahrenheit:
          thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeFahrenheit;
          break;
        case TemperatureUnit.Kelvin:
          thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseKelvin;
          break;
      }

      material.AnalysisMaterial = new AnalysisMaterial()
      {
        ElasticModulus = Input.UnitNumber(this, DA, 1, StressUnit).As(PressureUnit.Pascal),
        PoissonsRatio = poisson,
        Density = Input.UnitNumber(this, DA, 3, DensityUnit).As(DensityUnit.KilogramPerCubicMeter),
        CoefficientOfThermalExpansion = Input.UnitNumber(this, DA, 4, thermalExpansionUnit, true).As(CoefficientOfThermalExpansionUnit.InverseDegreeCelsius)
      };

      material.GradeProperty = 0; //will be ignored

      // element type (picked in dropdown)
      if (_mode == FoldMode.Undefined)
        material.MaterialType = GsaMaterial.MatType.GENERIC;
      if (_mode == FoldMode.Steel)
        material.MaterialType = GsaMaterial.MatType.STEEL;
      if (_mode == FoldMode.Concrete)
        material.MaterialType = GsaMaterial.MatType.CONCRETE;
      if (_mode == FoldMode.Timber)
        material.MaterialType = GsaMaterial.MatType.TIMBER;
      if (_mode == FoldMode.Aluminium)
        material.MaterialType = GsaMaterial.MatType.ALUMINIUM;
      if (_mode == FoldMode.FRP)
        material.MaterialType = GsaMaterial.MatType.FRP;
      if (_mode == FoldMode.Glass)
        material.MaterialType = GsaMaterial.MatType.GLASS;

      DA.SetData(0, new GsaMaterialGoo(material));
    }

    #region Custom UI
    private enum FoldMode
    {
      Steel,
      Concrete,
      Timber,
      Aluminium,
      FRP,
      Glass,
      Undefined
    }
    private FoldMode _mode = FoldMode.Timber;

    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;
    private PressureUnit StressUnit = DefaultUnits.StressUnitResult;
    private TemperatureUnit TemperatureUnit = DefaultUnits.TemperatureUnit;

    private readonly List<string> _topLevelDropDownItems = new List<string>(new string[]
    {
      "Steel",
      "Concrete",
      "Timber",
      "Aluminium",
      "FRP",
      "Glass",
      "Undefined"
    });

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Material Type",
          "Stress Unit",
          "Density Unit",
          "Temperature Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(this._topLevelDropDownItems);
      this.SelectedItems.Add(this._mode.ToString());

      // Stress unit
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      this.SelectedItems.Add(Pressure.GetAbbreviation(this.StressUnit));

      // Density unit
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      this.SelectedItems.Add(Density.GetAbbreviation(this.DensityUnit));

      // Temperature unit
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Temperature));
      this.SelectedItems.Add(Temperature.GetAbbreviation(this.TemperatureUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0) // change is made to the first dropdown list
      {
        _mode = (FoldMode)Enum.Parse(typeof(FoldMode), this.SelectedItems[0]);
      }
      else
      {
        switch (i)
        {
          case 1:
            this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[1]);
            break;
          case 2:
            this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[2]);
            break;
          case 3:
            this.TemperatureUnit = (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), this.SelectedItems[3]);
            break;
        }
      }

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), this.SelectedItems[0]);
      this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[1]);
      this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[2]);
      this.TemperatureUnit = (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), this.SelectedItems[3]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      string densityUnitAbbreviation = Density.GetAbbreviation(this.DensityUnit);
      string temperatureUnitAbbreviation = Temperature.GetAbbreviation(this.TemperatureUnit);

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
