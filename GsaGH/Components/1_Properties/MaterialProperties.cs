using System;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get geometric properties of a section
  /// </summary>
  public class MaterialProperties : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("7504a99f-a4e2-4e30-8251-de31ea83e8cb");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.MaterialProperties;
    private DensityUnit _densityUnit = DefaultUnits.DensityUnit;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;
    private TemperatureUnit _temperatureUnit = DefaultUnits.TemperatureUnit;

    public MaterialProperties() : base("Material Properties", "PropMat",
      "Get GSA Material Properties for Elastic Isotropic material type", CategoryName.Name(),
      SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var stressUnitsMenu = new ToolStripMenuItem("Stress") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateStress(unit)) {
          Checked = unit == Pressure.GetAbbreviation(_stressUnit),
          Enabled = true,
        };
        stressUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var densityUnitsMenu = new ToolStripMenuItem("Density") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateDensity(unit)) {
          Checked = unit == Density.GetAbbreviation(_densityUnit),
          Enabled = true,
        };
        densityUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var temperatureUnitsMenu = new ToolStripMenuItem("Temperature") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Temperature)) {
        var toolStripMenuItem
          = new ToolStripMenuItem(unit, null, (s, e) => UpdateTemperature(unit)) {
            Checked = unit == Temperature.GetAbbreviation(_temperatureUnit),
            Enabled = true,
          };
        temperatureUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        stressUnitsMenu,
        densityUnitsMenu,
        temperatureUnitsMenu,
      });
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public override bool Read(GH_IReader reader) {
      try {
        _stressUnit
          = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), reader.GetString("StressUnit"));
        _densityUnit
          = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), reader.GetString("DensityUnit"));
        _temperatureUnit = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit),
          reader.GetString("TemperatureUnit"));
      } catch (Exception) {
        _stressUnit = DefaultUnits.StressUnitResult;
        _densityUnit = DefaultUnits.DensityUnit;
        _temperatureUnit = DefaultUnits.TemperatureUnit;
      }

      return base.Read(reader);
    }

    public virtual void VariableParameterMaintenance() {
      Params.Output[0].Name = "Elastic Modulus [" + Pressure.GetAbbreviation(_stressUnit) + "]";
      Params.Output[2].Name = "Density [" + Density.GetAbbreviation(_densityUnit) + "]";
      CoefficientOfThermalExpansionUnit temp
        = UnitsHelper.GetCoefficientOfThermalExpansionUnit(_temperatureUnit);
      Params.Output[3].Name = "Thermal Expansion ["
        + CoefficientOfThermalExpansion.GetAbbreviation(temp) + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("StressUnit", _stressUnit.ToString());
      writer.SetString("DensityUnit", _densityUnit.ToString());
      writer.SetString("TemperatureUnit", _temperatureUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      UpdateMessage();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat", "GSA Custom Material",
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter(
        "Elastic Modulus [" + Pressure.GetAbbreviation(_stressUnit) + "]", "E",
        "Elastic Modulus of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddNumberParameter("Poisson's Ratio", "ν",
        "Poisson's Ratio of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + Density.GetAbbreviation(_densityUnit) + "]", "ρ",
        "Density of the elastic isotropic material", GH_ParamAccess.item);
      CoefficientOfThermalExpansionUnit temp
        = UnitsHelper.GetCoefficientOfThermalExpansionUnit(_temperatureUnit);
      pManager.AddGenericParameter(
        "Thermal Expansion [" + CoefficientOfThermalExpansion.GetAbbreviation(temp) + "]", "α",
        "Thermal Expansion Coefficient of the elastic isotropic material", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaMaterialGoo materialGoo = null;
      da.GetData(0, ref materialGoo);
      GsaMaterial material = materialGoo.Value;

      if (material.AnalysisMaterial == null) {
        this.AddRuntimeWarning("One or more materials are not custom material");
        return;
      }

      var eModulus
        = new Pressure(material.AnalysisMaterial.ElasticModulus,
          PressureUnit.Pascal); //create unit from SI as API is in SI units
      eModulus = new Pressure(eModulus.As(_stressUnit), _stressUnit);
      da.SetData(0, new GH_UnitNumber(eModulus));

      da.SetData(1, material.AnalysisMaterial.PoissonsRatio);

      var density = new Density(material.AnalysisMaterial.Density,
        DensityUnit.KilogramPerCubicMeter); //create unit from SI as API is in SI units
      density = new Density(density.As(_densityUnit), _densityUnit);
      da.SetData(2, new GH_UnitNumber(density));

      var deltaT = new CoefficientOfThermalExpansion(
        material.AnalysisMaterial.CoefficientOfThermalExpansion,
        CoefficientOfThermalExpansionUnit
         .PerDegreeCelsius); //create unit from SI as API is in SI units
      CoefficientOfThermalExpansionUnit temp
        = UnitsHelper.GetCoefficientOfThermalExpansionUnit(_temperatureUnit);
      deltaT = new CoefficientOfThermalExpansion(deltaT.As(temp), temp);
      da.SetData(3, new GH_UnitNumber(deltaT));
    }


    internal void UpdateDensity(string unit) {
      _densityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), unit);
      Update();
    }

    internal void UpdateStress(string unit) {
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), unit);
      Update();
    }

    internal void UpdateTemperature(string unit) {
      _temperatureUnit = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), unit);
      Update();
    }
    private void Update() {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    private void UpdateMessage() {
      CoefficientOfThermalExpansionUnit temp
        = UnitsHelper.GetCoefficientOfThermalExpansionUnit(_temperatureUnit);
      Message = Pressure.GetAbbreviation(_stressUnit) + ", " + Density.GetAbbreviation(_densityUnit)
        + ", " + CoefficientOfThermalExpansion.GetAbbreviation(temp);
    }
  }
}
