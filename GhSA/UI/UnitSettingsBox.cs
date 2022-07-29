using Oasys.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnitsNet;
using UnitsNet.Units;

namespace GsaGH.UI
{
  partial class UnitSettingsBox : Form
  {
    public UnitSettingsBox()
    {
      InitializeComponent();
      this.Text = "Default GSA Units";
      this.labelDescription.Text = "Settings will apply to new components and display";

      List<string> lengthGeometrydropdown = lengthdropdown.ToList();
      lengthGeometrydropdown.Insert(0, "Use Rhino unit: " + Units.GetRhinoLengthUnit(Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem).ToString());
      this.labelLengthGeometry.Text = "Length - geometry";
      this.comboBoxLengthGeometry.DataSource = lengthGeometrydropdown;
      this.comboBoxLengthGeometry.DropDownStyle = ComboBoxStyle.DropDownList;
      if (!Units.useRhinoLengthGeometryUnit)
      {
        this.comboBoxLengthGeometry.SelectedIndex = lengthGeometrydropdown.IndexOf(Units.LengthUnitGeometry.ToString());
        lengthUnit = Units.LengthUnitGeometry;
      }
      else
        lengthUnit = Units.GetRhinoLengthUnit();

      List<string> lengthSectiondropdown = lengthdropdown.ToList();
      this.labelLengthSection.Text = "Length - section";
      this.comboBoxLengthSection.DataSource = lengthSectiondropdown;
      this.comboBoxLengthSection.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxLengthSection.SelectedIndex = lengthSectiondropdown.IndexOf(Units.LengthUnitSection.ToString());

      List<string> lengthResultdropdown = lengthdropdown.ToList();
      this.labelLengthResult.Text = "Length - result";
      this.comboBoxLengthResult.DataSource = lengthResultdropdown;
      this.comboBoxLengthResult.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxLengthResult.SelectedIndex = lengthResultdropdown.IndexOf(Units.LengthUnitResult.ToString());

      this.labelForce.Text = "Force";
      this.comboBoxForce.DataSource = Units.FilteredForceUnits;
      this.comboBoxForce.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxForce.SelectedIndex = Units.FilteredForceUnits.IndexOf(Units.ForceUnit.ToString());

      this.labelMoment.Text = "Moment";
      this.comboBoxMoment.DataSource = Units.FilteredMomentUnits;
      this.comboBoxMoment.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxMoment.SelectedIndex = Units.FilteredMomentUnits.IndexOf(Units.MomentUnit.ToString());

      this.labelStress.Text = "Stress";
      this.comboBoxStress.DataSource = Units.FilteredStressUnits;
      this.comboBoxStress.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxStress.SelectedIndex = Units.FilteredStressUnits.IndexOf(Units.StressUnit.ToString());

      this.labelStrain.Text = "Strain";
      this.comboBoxStrain.DataSource = Units.FilteredStrainUnits;
      this.comboBoxStrain.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxStrain.SelectedIndex = Units.FilteredStrainUnits.IndexOf(Units.StrainUnit.ToString());

      this.labelMass.Text = "Mass";
      this.comboBoxMass.DataSource = Units.FilteredMassUnits;
      this.comboBoxMass.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxMass.SelectedIndex = Units.FilteredMassUnits.IndexOf(Units.MassUnit.ToString());

      this.labelTemperature.Text = "Temperatur";
      this.comboBoxTemperature.DataSource = Units.FilteredTemperatureUnits;
      this.comboBoxTemperature.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBoxTemperature.SelectedIndex = Units.FilteredTemperatureUnits.IndexOf(Units.TemperatureUnit.ToString());

      this.toleranceValueBox.Value = (decimal)Units.Tolerance.As(Units.LengthUnitGeometry);
      this.useRhinoToleranceCheckBox.Checked = Units.useRhinoTolerance;
      if (this.useRhinoToleranceCheckBox.Checked)
      {
        this.toleranceValueBox.Value = (decimal)Units.GetRhinoTolerance().As(lengthUnit);
        this.toleranceValueBox.Enabled = false;
      }
      this.labelTolerance.Text = "Tolerance " + lengthAbbr;
    }

    #region Temporary units
    List<string> lengthdropdown = Units.FilteredLengthUnits.ToList();
    string lengthGeometry = Units.LengthUnitGeometry.ToString();
    LengthUnit lengthUnit = Units.LengthUnitGeometry;
    string lengthAbbr = "[" + new Length(1, Units.LengthUnitGeometry).ToString("a") + "]";
    string lengthResult = Units.LengthUnitResult.ToString();
    string lengthSection = Units.LengthUnitSection.ToString();
    bool useRhinoUnitsGeometry = Units.useRhinoLengthGeometryUnit;
    bool useRhinoTolerance = Units.useRhinoTolerance;
    string force = Units.ForceUnit.ToString();
    string moment = Units.MomentUnit.ToString();
    string stress = Units.StressUnit.ToString();
    string strain = Units.StrainUnit.ToString();
    string mass = Units.MassUnit.ToString();
    string temperature = Units.TemperatureUnit.ToString();
    decimal tolerance = (decimal)Units.Tolerance.As(Units.LengthUnitGeometry);
    internal void SetUnits()
    {
      // Length - geometry
      if (useRhinoUnitsGeometry)
      {
        Units.LengthUnitGeometry = Units.GetRhinoLengthUnit();
        Units.useRhinoLengthGeometryUnit = true;
      }
      else
      {
        Units.useRhinoLengthGeometryUnit = false;
        Units.LengthUnitGeometry = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
      }
      if (useRhinoTolerance)
      {
        Units.Tolerance = Units.GetRhinoTolerance();
        Units.useRhinoTolerance = true;
      }
      else
      {
        Units.useRhinoTolerance = false;
        Units.Tolerance = new Length((double)tolerance, Units.LengthUnitGeometry);
      }
      // Length - section
      Units.LengthUnitSection = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthSection);
      // Length - result
      Units.LengthUnitResult = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthResult);

      Units.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), force);
      Units.MomentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), moment);
      Units.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), stress);
      Units.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), strain);
      Units.MassUnit = (MassUnit)Enum.Parse(typeof(MassUnit), mass);
      Units.TemperatureUnit = (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), temperature);

      //Units.AxialStiffnessUnit = (AxialStiffnessUnit)Enum.Parse(typeof(AxialStiffnessUnit), lengthGeometry);
      //Units.CurvatureUnit = (CurvatureUnit)Enum.Parse(typeof(CurvatureUnit), lengthResult);
      //Units.BendingStiffnessUnit = (BendingStiffnessUnit)Enum.Parse(typeof(BendingStiffnessUnit), lengthSection);

      Units.SaveSettings();
    }
    #endregion


    private void UnitSettingsBox_Load(object sender, EventArgs e)
    {

    }
    private void cancelButton_Click(object sender, EventArgs e)
    {

      this.Close();
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      SetUnits();
      this.Close();
    }

    private void comboBoxMass_SelectedIndexChanged(object sender, EventArgs e)
    {
      mass = this.comboBoxMass.SelectedItem.ToString();

    }
    private void comboBoxForce_SelectedIndexChanged(object sender, EventArgs e)
    {
      force = this.comboBoxForce.SelectedItem.ToString();
    }

    private void comboBoxMoment_SelectedIndexChanged(object sender, EventArgs e)
    {
      moment = this.comboBoxMoment.SelectedItem.ToString();
    }

    private void comboBoxStress_SelectedIndexChanged(object sender, EventArgs e)
    {
      stress = this.comboBoxStress.SelectedItem.ToString();
    }

    private void comboBoxStrain_SelectedIndexChanged(object sender, EventArgs e)
    {
      strain = this.comboBoxStrain.SelectedItem.ToString();
    }

    private void comboBoxLengthGeometry_SelectedIndexChanged(object sender, EventArgs e)
    {
      lengthGeometry = this.comboBoxLengthGeometry.SelectedItem.ToString();
      if (this.comboBoxLengthGeometry.SelectedIndex == 0)
      {
        lengthUnit = Units.GetRhinoLengthUnit();
        useRhinoUnitsGeometry = true;
      }
      else
      {
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
        useRhinoUnitsGeometry = false;
      }
      lengthAbbr = "[" + new Length(1, lengthUnit).ToString("a") + "]";
      this.labelTolerance.Text = "Tolerance " + lengthAbbr;
    }

    private void comboBoxLengthSection_SelectedIndexChanged(object sender, EventArgs e)
    {
      lengthSection = this.comboBoxLengthSection.SelectedItem.ToString();
    }

    private void comboBoxLengthResult_SelectedIndexChanged(object sender, EventArgs e)
    {
      lengthResult = this.comboBoxLengthResult.SelectedItem.ToString();
    }
    private void toleranceValueBox_ValueChanged(object sender, EventArgs e)
    {
      tolerance = this.toleranceValueBox.Value;
    }
    private void useRhinoToleranceCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      useRhinoTolerance = useRhinoToleranceCheckBox.Checked;
      if (useRhinoTolerance)
        this.toleranceValueBox.Value = (decimal)Units.GetRhinoTolerance().As((LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry));
      else
        this.toleranceValueBox.Value = (decimal)Units.Tolerance.As(lengthUnit);

      this.toleranceValueBox.Enabled = !useRhinoTolerance;
    }
    private void updateSelections()
    {
      this.comboBoxForce.SelectedIndex = Units.FilteredForceUnits.IndexOf(force);
      this.comboBoxMoment.SelectedIndex = Units.FilteredMomentUnits.IndexOf(moment);
      this.comboBoxStress.SelectedIndex = Units.FilteredStressUnits.IndexOf(stress);
      this.comboBoxStrain.SelectedIndex = Units.FilteredStrainUnits.IndexOf(strain);
      this.comboBoxMass.SelectedIndex = Units.FilteredMassUnits.IndexOf(mass);
      this.comboBoxTemperature.SelectedIndex = Units.FilteredTemperatureUnits.IndexOf(temperature);
      if (!useRhinoUnitsGeometry)
        this.comboBoxLengthGeometry.SelectedIndex = lengthdropdown.IndexOf(lengthGeometry) + 1;
      else
        this.comboBoxLengthGeometry.SelectedIndex = 0;
      this.comboBoxLengthSection.SelectedIndex = lengthdropdown.IndexOf(lengthSection);
      this.comboBoxLengthResult.SelectedIndex = lengthdropdown.IndexOf(lengthResult);
      this.toleranceValueBox.Enabled = !useRhinoTolerance;
      this.toleranceValueBox.Value = tolerance;
      this.useRhinoToleranceCheckBox.Checked = useRhinoTolerance;

    }
    private void buttonSI_Click(object sender, EventArgs e)
    {
      useRhinoUnitsGeometry = false;
      force = ForceUnit.Newton.ToString();
      moment = MomentUnit.NewtonMeter.ToString();
      stress = PressureUnit.Pascal.ToString();
      strain = StrainUnit.Ratio.ToString();
      mass = MassUnit.Kilogram.ToString();
      temperature = TemperatureUnit.Kelvin.ToString();
      lengthGeometry = LengthUnit.Meter.ToString();
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
      lengthResult = LengthUnit.Meter.ToString();
      lengthSection = LengthUnit.Meter.ToString();
      tolerance = (decimal)Units.Tolerance.As(lengthUnit);
      useRhinoTolerance = false;
      updateSelections();
    }

    private void buttonkNm_Click(object sender, EventArgs e)
    {
      useRhinoUnitsGeometry = false;
      force = ForceUnit.Kilonewton.ToString();
      moment = MomentUnit.KilonewtonMeter.ToString();
      stress = PressureUnit.NewtonPerSquareMillimeter.ToString();
      strain = StrainUnit.MilliStrain.ToString();
      mass = MassUnit.Tonne.ToString();
      temperature = TemperatureUnit.DegreeCelsius.ToString();
      lengthGeometry = LengthUnit.Meter.ToString();
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
      lengthResult = LengthUnit.Millimeter.ToString();
      lengthSection = LengthUnit.Centimeter.ToString();
      tolerance = (decimal)Units.Tolerance.As(lengthUnit);
      useRhinoTolerance = false;
      updateSelections();
    }

    private void buttonkipFt_Click(object sender, EventArgs e)
    {
      useRhinoUnitsGeometry = false;
      force = ForceUnit.KilopoundForce.ToString();
      moment = MomentUnit.KilopoundForceFoot.ToString();
      stress = PressureUnit.KilopoundForcePerSquareInch.ToString();
      strain = StrainUnit.Percent.ToString();
      mass = MassUnit.Kilopound.ToString();
      temperature = TemperatureUnit.DegreeFahrenheit.ToString();
      lengthGeometry = LengthUnit.Foot.ToString();
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
      lengthResult = LengthUnit.Inch.ToString();
      lengthSection = LengthUnit.Inch.ToString();
      tolerance = (decimal)Units.Tolerance.As(lengthUnit);
      useRhinoTolerance = false;
      updateSelections();
    }

    private void buttonkipIn_Click(object sender, EventArgs e)
    {
      useRhinoUnitsGeometry = false;
      force = ForceUnit.KilopoundForce.ToString();
      moment = MomentUnit.KilopoundForceInch.ToString();
      stress = PressureUnit.KilopoundForcePerSquareInch.ToString();
      strain = StrainUnit.Percent.ToString();
      mass = MassUnit.Kilopound.ToString();
      temperature = TemperatureUnit.DegreeFahrenheit.ToString();
      lengthGeometry = LengthUnit.Inch.ToString();
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
      lengthResult = LengthUnit.Inch.ToString();
      lengthSection = LengthUnit.Inch.ToString();
      tolerance = (decimal)Units.Tolerance.As(lengthUnit);
      useRhinoTolerance = false;
      updateSelections();
    }
  }
}
