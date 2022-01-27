using Oasys.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnitsNet.Units;

namespace GhSA.UI
{
    partial class UnitSettingsBox : Form
    {
        public UnitSettingsBox()
        {
            InitializeComponent();
            this.Text = "Default GSA Units";
            this.labelDescription.Text = "Settings will apply to new components and display";

            this.labelMass.Text = "Length";
            this.comboBoxMass.DataSource = lengthdropdown;
            this.comboBoxMass.DropDownStyle = ComboBoxStyle.DropDownList;
            if (!Units.useRhinoLengthGeometryUnit)
                this.comboBoxMass.SelectedIndex = lengthdropdown.IndexOf(Units.LengthUnitGeometry.ToString());

            lengthdropdown.Insert(0, "Use Rhino unit: " + Units.GetRhinoLengthUnit(Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem).ToString());
            this.labelLengthGeometry.Text = "Length - geometry";
            this.comboBoxLengthGeometry.DataSource = lengthdropdown;
            this.comboBoxLengthGeometry.DropDownStyle = ComboBoxStyle.DropDownList;
            if (!Units.useRhinoLengthGeometryUnit)
                this.comboBoxLengthGeometry.SelectedIndex = lengthdropdown.IndexOf(Units.LengthUnitGeometry.ToString());

            this.labelLengthSection.Text = "Length - section";
            this.comboBoxLengthSection.DataSource = lengthdropdown;
            this.comboBoxLengthSection.DropDownStyle = ComboBoxStyle.DropDownList;
            if (!Units.useRhinoLengthSectionUnit)
                this.comboBoxLengthSection.SelectedIndex = lengthdropdown.IndexOf(Units.LengthUnitSection.ToString());

            this.labelLengthResult.Text = "Length - result";
            this.comboBoxLengthResult.DataSource = lengthdropdown;
            this.comboBoxLengthResult.DropDownStyle = ComboBoxStyle.DropDownList;
            if (!Units.useRhinoLengthResultUnit)
                this.comboBoxLengthResult.SelectedIndex = lengthdropdown.IndexOf(Units.LengthUnitSection.ToString());

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

            
        }

        #region Temporary units
        List<string> lengthdropdown = Units.FilteredLengthUnits.ToList();
        string lengthGeometry = Units.LengthUnitGeometry.ToString();
        string lengthResult = Units.LengthUnitResult.ToString();
        string lengthSection = Units.LengthUnitSection.ToString();
        bool useRhinoUnitsGeometry = Units.useRhinoLengthGeometryUnit;
        bool useRhinoUnitsSection = Units.useRhinoLengthSectionUnit;
        bool useRhinoUnitsResult = Units.useRhinoLengthResultUnit;
        string force = Units.ForceUnit.ToString();
        string moment = Units.MomentUnit.ToString();
        string stress = Units.StressUnit.ToString();
        string strain = Units.StrainUnit.ToString();
        string mass = Units.MassUnit.ToString();
        string temperature = Units.TemperatureUnit.ToString();
        internal void SetUnits()
        {
            // Length - geometry
            if (useRhinoUnitsGeometry)
            {
                Units.useRhinoLengthGeometryUnit = true;
                Units.LengthUnitGeometry = Units.GetRhinoLengthUnit();
            }
            else
            {
                Units.useRhinoLengthGeometryUnit = false;
                Units.LengthUnitGeometry = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
            }
            // Length - section
            if (useRhinoUnitsSection)
            {
                Units.useRhinoLengthSectionUnit = true;
                Units.LengthUnitSection = Units.GetRhinoLengthUnit();
            }
            else
            {
                Units.useRhinoLengthSectionUnit = false;
                Units.LengthUnitSection = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthSection);
            }
            // Length - result
            if (useRhinoUnitsResult)
            {
                Units.useRhinoLengthResultUnit = true;
                Units.LengthUnitResult = Units.GetRhinoLengthUnit();
            }
            else
            {
                Units.useRhinoLengthResultUnit = false;
                Units.LengthUnitResult = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthResult);
            }
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
                useRhinoUnitsGeometry = true;
            else
                useRhinoUnitsGeometry = false;
        }

        private void comboBoxLengthSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            lengthSection = this.comboBoxLengthSection.SelectedItem.ToString();
        }

        private void comboBoxLengthResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            lengthResult = this.comboBoxLengthResult.SelectedItem.ToString();
        }

        private void updateSelections()
        {
            if (!useRhinoUnitsGeometry)
                this.comboBoxMass.SelectedIndex = lengthdropdown.IndexOf(mass);
            else
                this.comboBoxMass.SelectedIndex = 0;
            this.comboBoxForce.SelectedIndex = Units.FilteredForceUnits.IndexOf(force);
            this.comboBoxMoment.SelectedIndex = Units.FilteredMomentUnits.IndexOf(moment);
            this.comboBoxStress.SelectedIndex = Units.FilteredStressUnits.IndexOf(stress);
            this.comboBoxStrain.SelectedIndex = Units.FilteredStrainUnits.IndexOf(strain);
            this.comboBoxLengthGeometry.SelectedIndex = Units.FilteredLengthUnits.IndexOf(lengthGeometry);
            this.comboBoxLengthSection.SelectedIndex = Units.FilteredLengthUnits.IndexOf(lengthSection);
            this.comboBoxLengthResult.SelectedIndex = Units.FilteredLengthUnits.IndexOf(lengthResult);
        }
        private void buttonSI_Click(object sender, EventArgs e)
        {
            useRhinoUnitsGeometry = false;
            mass = LengthUnit.Meter.ToString();
            force = ForceUnit.Newton.ToString();
            moment = MomentUnit.NewtonMeter.ToString();
            stress = PressureUnit.Pascal.ToString();
            strain = StrainUnit.Ratio.ToString();
            lengthGeometry = LengthUnit.Meter.ToString();
            lengthResult = LengthUnit.Meter.ToString();
            lengthSection = LengthUnit.Meter.ToString();
            updateSelections();
        }

        private void buttonkNm_Click(object sender, EventArgs e)
        {
            useRhinoUnitsGeometry = false;
            mass = LengthUnit.Meter.ToString();
            force = ForceUnit.Kilonewton.ToString();
            moment = MomentUnit.KilonewtonMeter.ToString();
            stress = PressureUnit.NewtonPerSquareMillimeter.ToString();
            strain = StrainUnit.MilliStrain.ToString();
            lengthGeometry = AxialStiffnessUnit.Kilonewton.ToString();
            lengthResult = CurvatureUnit.PerMeter.ToString();
            lengthSection = BendingStiffnessUnit.NewtonSquareMillimeter.ToString();
            updateSelections();
        }

        private void buttonkipFt_Click(object sender, EventArgs e)
        {
            useRhinoUnitsGeometry = false;
            mass = LengthUnit.Foot.ToString();
            force = ForceUnit.KilopoundForce.ToString();
            moment = MomentUnit.KilopoundForceFoot.ToString();
            stress = PressureUnit.KilopoundForcePerSquareInch.ToString();
            strain = StrainUnit.Percent.ToString();
            lengthGeometry = AxialStiffnessUnit.KilopoundForce.ToString();
            lengthResult = CurvatureUnit.PerMeter.ToString();
            lengthSection = BendingStiffnessUnit.PoundForceSquareFoot.ToString();
            updateSelections();
        }

        private void buttonkipIn_Click(object sender, EventArgs e)
        {
            useRhinoUnitsGeometry = false;
            mass = LengthUnit.Inch.ToString();
            force = ForceUnit.KilopoundForce.ToString();
            moment = MomentUnit.KilopoundForceInch.ToString();
            stress = PressureUnit.KilopoundForcePerSquareInch.ToString();
            strain = StrainUnit.Percent.ToString();
            lengthGeometry = AxialStiffnessUnit.KilopoundForce.ToString();
            lengthResult = CurvatureUnit.PerMeter.ToString();
            lengthSection = BendingStiffnessUnit.PoundForceSquareInch.ToString();
            updateSelections();
        }

    }
}
