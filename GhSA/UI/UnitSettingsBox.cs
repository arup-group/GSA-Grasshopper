//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace GhSA.UI
//{
//    partial class UnitSettingsBox : Form
//    {
//        public UnitSettingsBox()
//        {
//            InitializeComponent();
//            this.Text = "Default GSA Units";
//            this.labelDescription.Text = "Settings will be saved as default GsaGH Units";

//            this.labelLength.Text = "Length";
//            lengthdropdown.Insert(0, "Use Rhino unit: " + DocumentUnits.GetRhinoLengthUnit(Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem).ToString());
//            this.comboBoxLength.DataSource = lengthdropdown;
//            this.comboBoxLength.DropDownStyle = ComboBoxStyle.DropDownList;
//            if (!DocumentUnits.useRhinoLengthUnit)
//                this.comboBoxLength.SelectedIndex = lengthdropdown.IndexOf(DocumentUnits.LengthUnit.ToString());

//            this.labelForce.Text = "Force";
//            this.comboBoxForce.DataSource = DocumentUnits.FilteredForceUnits;
//            this.comboBoxForce.DropDownStyle = ComboBoxStyle.DropDownList;
//            this.comboBoxForce.SelectedIndex = DocumentUnits.FilteredForceUnits.IndexOf(DocumentUnits.ForceUnit.ToString());

//            this.labelMoment.Text = "Moment";
//            this.comboBoxMoment.DataSource = DocumentUnits.FilteredMomentUnits;
//            this.comboBoxMoment.DropDownStyle = ComboBoxStyle.DropDownList;
//            this.comboBoxMoment.SelectedIndex = DocumentUnits.FilteredMomentUnits.IndexOf(DocumentUnits.MomentUnit.ToString());

//            this.labelStress.Text = "Stress";
//            this.comboBoxStress.DataSource = DocumentUnits.FilteredStressUnits;
//            this.comboBoxStress.DropDownStyle = ComboBoxStyle.DropDownList;
//            this.comboBoxStress.SelectedIndex = DocumentUnits.FilteredStressUnits.IndexOf(DocumentUnits.StressUnit.ToString());

//            this.labelStrain.Text = "Strain";
//            this.comboBoxStrain.DataSource = DocumentUnits.FilteredStrainUnits;
//            this.comboBoxStrain.DropDownStyle = ComboBoxStyle.DropDownList;
//            this.comboBoxStrain.SelectedIndex = DocumentUnits.FilteredStrainUnits.IndexOf(DocumentUnits.StrainUnit.ToString());

//            this.labelAxialStiffness.Text = "Axial Stiffness";
//            this.comboBoxAxialStiffness.DataSource = DocumentUnits.FilteredAxialStiffnessUnits;
//            this.comboBoxAxialStiffness.DropDownStyle = ComboBoxStyle.DropDownList;
//            this.comboBoxAxialStiffness.SelectedIndex = DocumentUnits.FilteredAxialStiffnessUnits.IndexOf(DocumentUnits.AxialStiffnessUnit.ToString());

//            this.labelBendingStiffness.Text = "Bending Stiffness";
//            this.comboBoxBendingStiffness.DataSource = DocumentUnits.FilteredBendingStiffnessUnits;
//            this.comboBoxBendingStiffness.DropDownStyle = ComboBoxStyle.DropDownList;
//            this.comboBoxBendingStiffness.SelectedIndex = DocumentUnits.FilteredBendingStiffnessUnits.IndexOf(DocumentUnits.BendingStiffnessUnit.ToString());
            
//            this.labelCurvature.Text = "Curvature";
//            this.comboBoxCurvature.DataSource = DocumentUnits.FilteredCurvatureUnits;
//            this.comboBoxCurvature.DropDownStyle = ComboBoxStyle.DropDownList;
//            this.comboBoxCurvature.SelectedIndex = DocumentUnits.FilteredCurvatureUnits.IndexOf(DocumentUnits.CurvatureUnit.ToString());
//        }

//        #region Temporary units
//        List<string> lengthdropdown = DocumentUnits.FilteredLengthUnits.ToList();
//        string length = DocumentUnits.LengthUnit.ToString();
//        bool useRhinoUnits = DocumentUnits.useRhinoLengthUnit;
//        string force = DocumentUnits.ForceUnit.ToString();
//        string moment = DocumentUnits.MomentUnit.ToString();
//        string stress = DocumentUnits.StressUnit.ToString();
//        string strain = DocumentUnits.StrainUnit.ToString();
//        string axialstiffness = DocumentUnits.AxialStiffnessUnit.ToString();
//        string curvature = DocumentUnits.CurvatureUnit.ToString();
//        string bendingstiffness = DocumentUnits.BendingStiffnessUnit.ToString();
//        internal void SetUnits()
//        {
//            if (useRhinoUnits)
//            {
//                DocumentUnits.useRhinoLengthUnit = true;
//                DocumentUnits.LengthUnit = DocumentUnits.GetRhinoLengthUnit();
//            }
//            else
//            {
//                DocumentUnits.useRhinoLengthUnit = false;
//                DocumentUnits.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), length);
//            }

//            DocumentUnits.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), force);
//            DocumentUnits.MomentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), moment);
//            DocumentUnits.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), stress);
//            DocumentUnits.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), strain);
//            DocumentUnits.AxialStiffnessUnit = (AxialStiffnessUnit)Enum.Parse(typeof(AxialStiffnessUnit), axialstiffness);
//            DocumentUnits.CurvatureUnit = (CurvatureUnit)Enum.Parse(typeof(CurvatureUnit), curvature);
//            DocumentUnits.BendingStiffnessUnit = (BendingStiffnessUnit)Enum.Parse(typeof(BendingStiffnessUnit), bendingstiffness);

//            DocumentUnits.SaveSettings();
//        }
//        #endregion


//        private void UnitSettingsBox_Load(object sender, EventArgs e)
//        {

//        }
//        private void cancelButton_Click(object sender, EventArgs e)
//        {
            
//            this.Close();
//        }

//        private void okButton_Click(object sender, EventArgs e)
//        {
//            SetUnits();
//            this.Close();
//        }

//        private void comboBoxLength_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            length = this.comboBoxLength.SelectedItem.ToString();
//            if (this.comboBoxLength.SelectedIndex == 0)
//                useRhinoUnits = true;
//            else
//                useRhinoUnits = false;
//        }
//        private void comboBoxForce_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            force = this.comboBoxForce.SelectedItem.ToString();
//        }

//        private void comboBoxMoment_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            moment = this.comboBoxMoment.SelectedItem.ToString();
//        }

//        private void comboBoxStress_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            stress = this.comboBoxStress.SelectedItem.ToString();
//        }

//        private void comboBoxStrain_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            strain = this.comboBoxStrain.SelectedItem.ToString();
//        }

//        private void comboBoxAxialStiffness_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            axialstiffness = this.comboBoxAxialStiffness.SelectedItem.ToString();
//        }

//        private void comboBoxBendingStiffness_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            bendingstiffness = this.comboBoxBendingStiffness.SelectedItem.ToString();
//        }

//        private void comboBoxCurvature_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            curvature = this.comboBoxCurvature.SelectedItem.ToString();
//        }

//        private void updateSelections()
//        {
//            if (!useRhinoUnits)
//                this.comboBoxLength.SelectedIndex = lengthdropdown.IndexOf(length);
//            else
//                this.comboBoxLength.SelectedIndex = 0;
//            this.comboBoxForce.SelectedIndex = DocumentUnits.FilteredForceUnits.IndexOf(force);
//            this.comboBoxMoment.SelectedIndex = DocumentUnits.FilteredMomentUnits.IndexOf(moment);
//            this.comboBoxStress.SelectedIndex = DocumentUnits.FilteredStressUnits.IndexOf(stress);
//            this.comboBoxStrain.SelectedIndex = DocumentUnits.FilteredStrainUnits.IndexOf(strain);
//            this.comboBoxAxialStiffness.SelectedIndex = DocumentUnits.FilteredAxialStiffnessUnits.IndexOf(axialstiffness);
//            this.comboBoxBendingStiffness.SelectedIndex = DocumentUnits.FilteredBendingStiffnessUnits.IndexOf(bendingstiffness);
//            this.comboBoxCurvature.SelectedIndex = DocumentUnits.FilteredCurvatureUnits.IndexOf(curvature);
//        }
//        private void buttonSI_Click(object sender, EventArgs e)
//        {
//            useRhinoUnits = false;
//            length = LengthUnit.Meter.ToString();
//            force = ForceUnit.Newton.ToString();
//            moment = MomentUnit.NewtonMeter.ToString();
//            stress = PressureUnit.Pascal.ToString();
//            strain = StrainUnit.Ratio.ToString();
//            axialstiffness = AxialStiffnessUnit.Newton.ToString();
//            curvature = CurvatureUnit.PerMeter.ToString();
//            bendingstiffness = BendingStiffnessUnit.NewtonSquareMeter.ToString();
//            updateSelections();
//        }

//        private void buttonkNm_Click(object sender, EventArgs e)
//        {
//            useRhinoUnits = false;
//            length = LengthUnit.Meter.ToString();
//            force = ForceUnit.Kilonewton.ToString();
//            moment = MomentUnit.KilonewtonMeter.ToString();
//            stress = PressureUnit.NewtonPerSquareMillimeter.ToString();
//            strain = StrainUnit.MilliStrain.ToString();
//            axialstiffness = AxialStiffnessUnit.Kilonewton.ToString();
//            curvature = CurvatureUnit.PerMeter.ToString();
//            bendingstiffness = BendingStiffnessUnit.NewtonSquareMillimeter.ToString();
//            updateSelections();
//        }

//        private void buttonkipFt_Click(object sender, EventArgs e)
//        {
//            useRhinoUnits = false;
//            length = LengthUnit.Foot.ToString();
//            force = ForceUnit.KilopoundForce.ToString();
//            moment = MomentUnit.KilopoundForceFoot.ToString();
//            stress = PressureUnit.KilopoundForcePerSquareInch.ToString();
//            strain = StrainUnit.Percent.ToString();
//            axialstiffness = AxialStiffnessUnit.KilopoundForce.ToString();
//            curvature = CurvatureUnit.PerMeter.ToString();
//            bendingstiffness = BendingStiffnessUnit.PoundForceSquareFoot.ToString();
//            updateSelections();
//        }

//        private void buttonkipIn_Click(object sender, EventArgs e)
//        {
//            useRhinoUnits = false;
//            length = LengthUnit.Inch.ToString();
//            force = ForceUnit.KilopoundForce.ToString();
//            moment = MomentUnit.KilopoundForceInch.ToString();
//            stress = PressureUnit.KilopoundForcePerSquareInch.ToString();
//            strain = StrainUnit.Percent.ToString();
//            axialstiffness = AxialStiffnessUnit.KilopoundForce.ToString();
//            curvature = CurvatureUnit.PerMeter.ToString();
//            bendingstiffness = BendingStiffnessUnit.PoundForceSquareInch.ToString();
//            updateSelections();
//        }

//    }
//}
