using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using UnitsNet.GH;
using UnitsNet;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new UnitNumber
  /// </summary>
  public class CreateUnitNumber : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("53f42580-8ed7-42fb-8cc6-c6f6171a0248");
    public CreateUnitNumber()
      : base("Create UnitNumber", "CreateUnit", "Create a unit number (quantity) from value, unit and measure",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat9())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateUnitNumber;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // unit types
        dropdownitems.Add(Enum.GetNames(typeof(Units.GsaUnits)).ToList());
        selecteditems.Add(dropdownitems[0][0]);

        // first type
        dropdownitems.Add(Enum.GetNames(typeof(UnitsNet.Units.LengthUnit)).ToList());
        selecteditems.Add(Units.LengthUnitGeometry.ToString());

        // set selected unit to
        quantity = new Length(0, Units.LengthUnitGeometry);
        selectedMeasure = quantity.Unit;
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        // create new dictionary and set all unit measures from quantity to it
        measureDictionary = new Dictionary<string, Enum>();
        foreach (UnitInfo unit in quantity.QuantityInfo.UnitInfos)
        {
          measureDictionary.Add(unit.Name, unit.Value);
        }

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      // if change is made to first (unit type) list we have to update lists
      if (i == 0)
      {
        // get selected unit
        Units.GsaUnits unit = (Units.GsaUnits)Enum.Parse(typeof(Units.GsaUnits), selecteditems[0]);
        UpdateQuantityUnitTypeFromUnitString(unit);
        UpdateMeasureDictionary();
        selecteditems[1] = selectedMeasure.ToString();
      }
      else // if change is made to the measure of a unit
      {
        selectedMeasure = measureDictionary[selecteditems.Last()];
        UpdateUnitMeasureAndAbbreviation();
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUnitMeasureAndAbbreviation()
    {
      Units.GsaUnits unit = (Units.GsaUnits)Enum.Parse(typeof(Units.GsaUnits), selecteditems[0]);
      // switch case
      switch (unit)
      {
        case Units.GsaUnits.Length:
          quantity = new Length(val, (UnitsNet.Units.LengthUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Area:
          quantity = new Area(val, (UnitsNet.Units.AreaUnit)selectedMeasure);
          break;
        case Units.GsaUnits.AreaMomentOfInertia:
          quantity = new AreaMomentOfInertia(val, (UnitsNet.Units.AreaMomentOfInertiaUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Force:
          quantity = new Force(val, (UnitsNet.Units.ForceUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Moment:
          quantity = new Oasys.Units.Moment(val, (Oasys.Units.MomentUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Stress:
          quantity = new Pressure(val, (UnitsNet.Units.PressureUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Strain:
          quantity = new Oasys.Units.Strain(val, (Oasys.Units.StrainUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Mass:
          quantity = new Mass(val, (UnitsNet.Units.MassUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Density:
          quantity = new Density(val, (UnitsNet.Units.DensityUnit)selectedMeasure);
          break;
        case Units.GsaUnits.LinearDensity:
          quantity = new LinearDensity(val, (UnitsNet.Units.LinearDensityUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Temperature:
          quantity = new Temperature(val, (UnitsNet.Units.TemperatureUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Velocity:
          quantity = new Speed(val, (UnitsNet.Units.SpeedUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Acceleration:
          quantity = new Acceleration(val, (UnitsNet.Units.AccelerationUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Ratio:
          quantity = new Ratio(val, (UnitsNet.Units.RatioUnit)selectedMeasure);
          break;
      }
    }

    private void UpdateQuantityUnitTypeFromUnitString(Units.GsaUnits unit)
    {
      // switch case
      switch (unit)
      {
        case Units.GsaUnits.Length:
          quantity = new Length(val, Units.LengthUnitGeometry);
          break;
        case Units.GsaUnits.Area:
          quantity = new Area(val, Units.SectionAreaUnit);
          break;
        case Units.GsaUnits.AreaMomentOfInertia:
          quantity = new AreaMomentOfInertia(val, Units.SectionAreaMomentOfInertiaUnit);
          break;
        case Units.GsaUnits.Force:
          quantity = new Force(val, Units.ForceUnit);
          break;
        case Units.GsaUnits.Moment:
          quantity = new Oasys.Units.Moment(val, Units.MomentUnit);
          break;
        case Units.GsaUnits.Stress:
          quantity = new Pressure(val, Units.StressUnit);
          break;
        case Units.GsaUnits.Strain:
          quantity = new Oasys.Units.Strain(val, Units.StrainUnit);
          break;
        case Units.GsaUnits.Mass:
          quantity = new Mass(val, Units.MassUnit);
          break;
        case Units.GsaUnits.Density:
          quantity = new Density(val, Units.DensityUnit);
          break;
        case Units.GsaUnits.LinearDensity:
          quantity = new LinearDensity(val, Units.LinearDensityUnit);
          break;
        case Units.GsaUnits.Temperature:
          quantity = new Temperature(val, Units.TemperatureUnit);
          break;
        case Units.GsaUnits.Velocity:
          quantity = new Speed(val, Units.VelocityUnit);
          break;
        case Units.GsaUnits.Acceleration:
          quantity = new Acceleration(val, Units.AccelerationUnit);
          break;
        case Units.GsaUnits.Ratio:
          quantity = new Ratio(val, UnitsNet.Units.RatioUnit.Percent);
          break;
        default:
          quantity = new Length(val, Units.LengthUnitGeometry);
          break;
      }
      selectedMeasure = quantity.Unit;
    }

    private void UpdateMeasureDictionary()
    {
      // create new dictionary and set all unit measures from quantity to it
      measureDictionary = new Dictionary<string, Enum>();
      foreach (UnitInfo unitype in quantity.QuantityInfo.UnitInfos)
      {
        measureDictionary.Add(unitype.Name, unitype.Value);
      }
      // update dropdown list
      dropdownitems[1] = measureDictionary.Keys.ToList();
    }

    private void UpdateUIFromSelectedItems()
    {
      // get selected unit
      Units.GsaUnits unit = (Units.GsaUnits)Enum.Parse(typeof(Units.GsaUnits), selecteditems[0]);
      UpdateQuantityUnitTypeFromUnitString(unit);
      UpdateMeasureDictionary();
      UpdateUnitMeasureAndAbbreviation();
      selectedMeasure = measureDictionary[selecteditems.Last()];

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      CreateAttributes();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    // get list of material types defined in material parameter

    // list of materials
    Dictionary<string, Enum> measureDictionary;
    Enum selectedMeasure;
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Unit type",
            "Measure"
    });
    private bool first = true;
    IQuantity quantity;
    double val;
    string unitAbbreviation;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("Number [" + unitAbbreviation + "]", "N", "Number representing the value of selected unit and measure", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("UnitNumber", "UN", "Number converted to selected unit", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (DA.GetData(0, ref val))
      {
        Units.GsaUnits unit = (Units.GsaUnits)Enum.Parse(typeof(Units.GsaUnits), selecteditems[0]);

        // switch case
        switch (unit)
        {
          case Units.GsaUnits.Length:
            quantity = new Length(val, (UnitsNet.Units.LengthUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Area:
            quantity = new Area(val, (UnitsNet.Units.AreaUnit)selectedMeasure);
            break;
          case Units.GsaUnits.AreaMomentOfInertia:
            quantity = new AreaMomentOfInertia(val, (UnitsNet.Units.AreaMomentOfInertiaUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Force:
            quantity = new Force(val, (UnitsNet.Units.ForceUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Moment:
            quantity = new Oasys.Units.Moment(val, (Oasys.Units.MomentUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Stress:
            quantity = new Pressure(val, (UnitsNet.Units.PressureUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Strain:
            quantity = new Oasys.Units.Strain(val, (Oasys.Units.StrainUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Mass:
            quantity = new Mass(val, (UnitsNet.Units.MassUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Density:
            quantity = new Density(val, (UnitsNet.Units.DensityUnit)selectedMeasure);
            break;
          case Units.GsaUnits.LinearDensity:
            quantity = new LinearDensity(val, (UnitsNet.Units.LinearDensityUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Temperature:
            quantity = new Temperature(val, (UnitsNet.Units.TemperatureUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Velocity:
            quantity = new Speed(val, (UnitsNet.Units.SpeedUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Acceleration:
            quantity = new Acceleration(val, (UnitsNet.Units.AccelerationUnit)selectedMeasure);
            break;
          case Units.GsaUnits.Ratio:
            quantity = new Ratio(val, (UnitsNet.Units.RatioUnit)selectedMeasure);
            break;
        }

        // convert unit to selected output
        GH_UnitNumber unitNumber = new GH_UnitNumber(quantity);

        // set output data
        DA.SetData(0, unitNumber);
      }
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);

      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      UpdateUIFromSelectedItems();
      first = false;
      return base.Read(reader);
    }
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
    #endregion

    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      Units.GsaUnits unit = (Units.GsaUnits)Enum.Parse(typeof(Units.GsaUnits), selecteditems[0]);
      // switch case
      switch (unit)
      {
        case Units.GsaUnits.Length:
          unitAbbreviation = ((Length)quantity).ToString("a");
          break;
        case Units.GsaUnits.Area:
          unitAbbreviation = ((Area)quantity).ToString("a");
          break;
        case Units.GsaUnits.AreaMomentOfInertia:
          unitAbbreviation = ((AreaMomentOfInertia)quantity).ToString("a");
          break;
        case Units.GsaUnits.Force:
          unitAbbreviation = ((Force)quantity).ToString("a");
          break;
        case Units.GsaUnits.Moment:
          unitAbbreviation = Oasys.Units.Moment.GetAbbreviation((Oasys.Units.MomentUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Stress:
          unitAbbreviation = ((Pressure)quantity).ToString("a");
          break;
        case Units.GsaUnits.Strain:
          unitAbbreviation = Oasys.Units.Strain.GetAbbreviation((Oasys.Units.StrainUnit)selectedMeasure);
          break;
        case Units.GsaUnits.Mass:
          unitAbbreviation = ((Mass)quantity).ToString("a");
          break;
        case Units.GsaUnits.Density:
          unitAbbreviation = ((Density)quantity).ToString("a");
          break;
        case Units.GsaUnits.LinearDensity:
          unitAbbreviation = ((LinearDensity)quantity).ToString("a");
          break;
        case Units.GsaUnits.Temperature:
          unitAbbreviation = ((Temperature)quantity).ToString("a");
          break;
        case Units.GsaUnits.Velocity:
          unitAbbreviation = ((Speed)quantity).ToString("a");
          break;
        case Units.GsaUnits.Acceleration:
          unitAbbreviation = ((Acceleration)quantity).ToString("a");
          break;
        case Units.GsaUnits.Ratio:
          unitAbbreviation = ((Ratio)quantity).ToString("a");
          break;
      }
      Params.Input[0].Name = "Number [" + unitAbbreviation + "]";
    }
    #endregion
  }
}