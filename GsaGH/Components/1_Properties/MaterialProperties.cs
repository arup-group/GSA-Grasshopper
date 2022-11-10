using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get geometric properties of a section
  /// </summary>
  public class GetMaterialProperties : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7504a99f-a4e2-4e30-8251-de31ea83e8cb");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.MaterialProperties;

    public GetMaterialProperties() : base("Material Properties",
      "MatProp",
      "Get GSA Material Properties for Elastic Isotropic material type",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaMaterialParameter(), "Material", "Mat", "GSA Custom Material", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Elastic Modulus [" + Pressure.GetAbbreviation(this.StressUnit) + "]", "E", "Elastic Modulus of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddNumberParameter("Poisson's Ratio", "ν", "Poisson's Ratio of the elastic isotropic material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + Density.GetAbbreviation(this.DensityUnit) + "]", "ρ", "Density of the elastic isotropic material", GH_ParamAccess.item);
      CoefficientOfThermalExpansionUnit temp = UnitsHelper.GetCoefficientOfThermalExpansionUnit(this.TemperatureUnit);
      pManager.AddGenericParameter("Thermal Expansion [" + CoefficientOfThermalExpansion.GetAbbreviation(temp) + "]", "α", "Thermal Expansion Coefficient of the elastic isotropic material", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaMaterial gsaMaterial = null;
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GsaMaterialGoo)
          gh_typ.CastTo(ref gsaMaterial);
      }
      if (gsaMaterial != null)
      {
        if (gsaMaterial.AnalysisMaterial == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more materials are not custom material");
          return;
        }

        Pressure eModulus = new Pressure(gsaMaterial.AnalysisMaterial.ElasticModulus, PressureUnit.Pascal); //create unit from SI as API is in SI units
        eModulus = new Pressure(eModulus.As(this.StressUnit), this.StressUnit);
        DA.SetData(0, new GH_UnitNumber(eModulus));

        DA.SetData(1, gsaMaterial.AnalysisMaterial.PoissonsRatio);

        Density density = new Density(gsaMaterial.AnalysisMaterial.Density, DensityUnit.KilogramPerCubicMeter);//create unit from SI as API is in SI units
        density = new Density(density.As(this.DensityUnit), this.DensityUnit);
        DA.SetData(2, new GH_UnitNumber(density));

        CoefficientOfThermalExpansion deltaT = new CoefficientOfThermalExpansion(gsaMaterial.AnalysisMaterial.CoefficientOfThermalExpansion, CoefficientOfThermalExpansionUnit.InverseDegreeCelsius);//create unit from SI as API is in SI units
        CoefficientOfThermalExpansionUnit temp = UnitsHelper.GetCoefficientOfThermalExpansionUnit(this.TemperatureUnit);
        deltaT = new CoefficientOfThermalExpansion(deltaT.As(temp), temp);
        DA.SetData(3, new GH_UnitNumber(deltaT));
      }
    }

    #region Custom UI
    private PressureUnit StressUnit = DefaultUnits.StressUnitResult;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;
    private TemperatureUnit TemperatureUnit = DefaultUnits.TemperatureUnit;
    protected override void BeforeSolveInstance()
    {
      UpdateMessage();
    }
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      ToolStripMenuItem stressUnitsMenu = new ToolStripMenuItem("Stress");
      stressUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateStress(unit); });
        toolStripMenuItem.Checked = unit == Pressure.GetAbbreviation(this.StressUnit);
        toolStripMenuItem.Enabled = true;
        stressUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem densityUnitsMenu = new ToolStripMenuItem("Density");
      densityUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateDensity(unit); });
        toolStripMenuItem.Checked = unit == Density.GetAbbreviation(this.DensityUnit);
        toolStripMenuItem.Enabled = true;
        densityUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem temperatureUnitsMenu = new ToolStripMenuItem("Temperature");
      temperatureUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Temperature))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateTemperature(unit); });
        toolStripMenuItem.Checked = unit == Temperature.GetAbbreviation(this.TemperatureUnit);
        toolStripMenuItem.Enabled = true;
        temperatureUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { stressUnitsMenu, densityUnitsMenu, temperatureUnitsMenu });
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    
    private void UpdateStress(string unit)
    {
      this.StressUnit = Pressure.ParseUnit(unit);
      Update();
    }
    private void UpdateDensity(string unit)
    {
      this.DensityUnit = Density.ParseUnit(unit);
      Update();
    }
    private void UpdateTemperature(string unit)
    {
      this.TemperatureUnit = Temperature.ParseUnit(unit);
      Update();
    }
    private void Update()
    {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
    private void UpdateMessage()
    {
      CoefficientOfThermalExpansionUnit temp = UnitsHelper.GetCoefficientOfThermalExpansionUnit(this.TemperatureUnit);
      this.Message =
        Pressure.GetAbbreviation(this.StressUnit) + ", " +
        Density.GetAbbreviation(this.DensityUnit) + ", " +
        CoefficientOfThermalExpansion.GetAbbreviation(temp);
    }

    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("StressUnit", this.StressUnit.ToString());
      writer.SetString("DensityUnit", this.DensityUnit.ToString());
      writer.SetString("TemperatureUnit", this.TemperatureUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      try
      {
        this.StressUnit = Pressure.ParseUnit(reader.GetString("StressUnit"));
        this.DensityUnit = Density.ParseUnit(reader.GetString("DensityUnit"));
        this.TemperatureUnit = Temperature.ParseUnit(reader.GetString("TemperatureUnit"));
      }
      catch (Exception)
      {
        this.StressUnit = DefaultUnits.StressUnitResult;
        this.DensityUnit = DefaultUnits.DensityUnit;
        this.TemperatureUnit = DefaultUnits.TemperatureUnit;
      }
      return base.Read(reader);
    }

    #region IGH_VariableParameterComponent null implementation
    public virtual void VariableParameterMaintenance()
    {
      this.Params.Output[0].Name = "Elastic Modulus [" + Pressure.GetAbbreviation(this.StressUnit) + "]";
      this.Params.Output[2].Name = "Density [" + Density.GetAbbreviation(this.DensityUnit) + "]";
      CoefficientOfThermalExpansionUnit temp = UnitsHelper.GetCoefficientOfThermalExpansionUnit(this.TemperatureUnit);
      this.Params.Output[3].Name = "Thermal Expansion [" + CoefficientOfThermalExpansion.GetAbbreviation(temp) + "]";
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}
