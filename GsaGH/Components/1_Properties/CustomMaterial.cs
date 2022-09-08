using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet;
using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GsaGH.Parameters;
using System.Linq;
using UnitsNet.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Material
  /// </summary>
  public class CustomMaterial : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("83bfce91-9204-4fe4-b81d-0036babf0c6d");
    public CustomMaterial()
      : base("Custom Material", "Material", "Create a Custom GSA Analysis Material",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CustomMaterial;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        dropdownitems.Add(topLevelDropdownItems);
        dropdownitems.Add(Units.FilteredStressUnits);
        dropdownitems.Add(Units.FilteredDensityUnits);
        dropdownitems.Add(Units.FilteredTemperatureUnits);

        selecteditems = new List<string>();
        selecteditems.Add(_mode.ToString());
        selecteditems.Add(Units.StressUnit.ToString());
        selecteditems.Add(Units.DensityUnit.ToString());
        selecteditems.Add(Units.TemperatureUnit.ToString());
        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0) // change is made to the first dropdown list
      {
        _mode = (FoldMode)Enum.Parse(typeof(FoldMode), selecteditems[0]);
      }
      else
      {
        switch (i)
        {
          case 1:
            stressUnit = (UnitsNet.Units.PressureUnit)Enum.Parse(typeof(UnitsNet.Units.PressureUnit), selecteditems[1]);
            break;
          case 2:
            densityUnit = (UnitsNet.Units.DensityUnit)Enum.Parse(typeof(UnitsNet.Units.DensityUnit), selecteditems[2]);
            break;
          case 3:
            temperatureUnit = (UnitsNet.Units.TemperatureUnit)Enum.Parse(typeof(UnitsNet.Units.TemperatureUnit), selecteditems[3]);
            break;

        }
        Params.OnParametersChanged();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      }

      // update input params
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      CreateAttributes();
      Params.OnParametersChanged();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    readonly List<string> topLevelDropdownItems = new List<string>(new string[]
    {
            "Steel",
            "Concrete",
            "Timber",
            "Aluminium",
            "FRP",
            "Glass",
            "Undefined"
    });
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Material Type",
            "Stress Unit",
            "Density Unit",
            "Temperature Unit"
    });

    private UnitsNet.Units.DensityUnit densityUnit = Units.DensityUnit;
    private UnitsNet.Units.PressureUnit stressUnit = Units.StressUnit;
    private UnitsNet.Units.TemperatureUnit temperatureUnit = Units.TemperatureUnit;
    string densityUnitAbbreviation;
    string stressUnitAbbreviation;
    string temperatureUnitAbbreviation;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity stress = new Pressure(0, stressUnit);
      stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
      IQuantity density = new Density(0, densityUnit);
      densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));
      IQuantity temperature = new Temperature(0, temperatureUnit);
      temperatureUnitAbbreviation = string.Concat(temperature.ToString().Where(char.IsLetter));

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
      pManager.AddGenericParameter("Material", "Ma", "GSA Material", GH_ParamAccess.item);
    }

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
      switch (temperatureUnit)
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
        ElasticModulus = GetInput.Stress(this, DA, 1, stressUnit).As(UnitsNet.Units.PressureUnit.Pascal),
        PoissonsRatio = poisson,
        Density = GetInput.Density(this, DA, 3, densityUnit).As(UnitsNet.Units.DensityUnit.KilogramPerCubicMeter),
        CoefficientOfThermalExpansion = GetInput.CoefficientOfThermalExpansion(this, DA, 4, thermalExpansionUnit, true).As(UnitsNet.Units.CoefficientOfThermalExpansionUnit.InverseDegreeCelsius)
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
    #region menu override
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
    private bool first = true;
    private FoldMode _mode = FoldMode.Timber;

    #endregion
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);

      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), selecteditems[0]);

      stressUnit = (UnitsNet.Units.PressureUnit)Enum.Parse(typeof(UnitsNet.Units.PressureUnit), selecteditems[1]);
      densityUnit = (UnitsNet.Units.DensityUnit)Enum.Parse(typeof(UnitsNet.Units.DensityUnit), selecteditems[2]);
      temperatureUnit = (UnitsNet.Units.TemperatureUnit)Enum.Parse(typeof(UnitsNet.Units.TemperatureUnit), selecteditems[3]);

      UpdateUIFromSelectedItems();
      first = false;
      return base.Read(reader);
    }

    #endregion
    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity stress = new Pressure(0, stressUnit);
      stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
      IQuantity density = new Density(0, densityUnit);
      densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));
      IQuantity temperature = new Temperature(0, temperatureUnit);
      temperatureUnitAbbreviation = string.Concat(temperature.ToString().Where(char.IsLetter));

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