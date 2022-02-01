using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using UnitsNet;
using Oasys.Units;
using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;
using System.Linq;

namespace GhSA.Components
{
    /// <summary>
    /// Component to create a new Material
    /// </summary>
    public class CreateMaterial : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("72bfce91-9204-4fe4-b81d-0036babf0c6d");
        public CreateMaterial()
          : base("Create Material", "Material", "Create GSA Material",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateMaterial;
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
                switch (selecteditems[0])
                {
                    case "Generic":
                        Mode1Clicked();
                        break;
                    case "Steel":
                        Mode2Clicked();
                        break;
                    case "Concrete":
                        Mode3Clicked();
                        break;
                    case "Timber":
                        Mode4Clicked();
                        break;
                    case "Aluminium":
                        Mode5Clicked();
                        break;
                    case "FRP":
                        Mode6Clicked();
                        break;
                    case "Glass":
                        Mode7Clicked();
                        break;
                    case "Fabric":
                        Mode8Clicked();
                        break;
                    case "ElasticIsotropic":
                        Mode9Clicked();
                        break;
                }
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
            UpdateInputs();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        readonly List<string> topLevelDropdownItems = new List<string>(new string[]
        {
            "Generic",
            "Steel",
            "Concrete",
            "Timber",
            "Aluminium",
            "FRP",
            "Glass",
            "Fabric",
            "ElasticIsotropic"
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
            
            pManager.AddIntegerParameter("Analysis Property", "An", "Analysis Property Number (default = 0 -> 'from Grade')", GH_ParamAccess.item, 0);
            pManager.AddGenericParameter("Elastic Modulus [" + stressUnitAbbreviation + "]", "E", "Elastic Modulus of the elastic isotropic material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Poisson's Ratio", "ν", "Poisson's Ratio of the elastic isotropic material", GH_ParamAccess.item);
            pManager.AddGenericParameter("Density [" + densityUnitAbbreviation + "]", "ρ", "Density of the elastic isotropic material", GH_ParamAccess.item);
            pManager.AddGenericParameter("Thermal Expansion [/" + temperatureUnitAbbreviation + "]", "α", "Thermal Expansion Coefficient of the elastic isotropic material", GH_ParamAccess.item);
            pManager[4].Optional = true;
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
                int anal = 0;
                GH_Convert.ToInt32(gh_anal, out anal, GH_Conversion.Both);
                material.AnalysisProperty = anal;
            }

            if (_mode == FoldMode.ElasticIsotropic)
            {
                material.GradeProperty = 1;
                
                double poisson = 0.3;
                DA.GetData(2, ref poisson);

                double tempCoefficient = 0;
                DA.GetData(4, ref tempCoefficient);
                if (temperatureUnit == UnitsNet.Units.TemperatureUnit.DegreeFahrenheit)
                    tempCoefficient = tempCoefficient * 0.555555556;
                material.ElasticIsotropicMaterial = new AnalysisMaterial()
                {
                    ElasticModulus = GetInput.Stress(this, DA, 1, stressUnit).As(UnitsNet.Units.PressureUnit.Pascal),
                    PoissonsRatio = poisson,
                    Density = GetInput.Density(this, DA, 3, densityUnit).As(UnitsNet.Units.DensityUnit.KilogramPerCubicMeter),
                    CoefficientOfThermalExpansion = tempCoefficient
                };
            }
            else
            {
                GH_Integer gh_grade = new GH_Integer();
                if (DA.GetData(1, ref gh_grade))
                {
                    int grade = 1;
                    GH_Convert.ToInt32(gh_grade, out grade, GH_Conversion.Both);
                    material.GradeProperty = grade;
                }
                else
                    material.GradeProperty = 1;

                // element type (picked in dropdown)
                if (_mode == FoldMode.Generic)
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
                if (_mode == FoldMode.Fabric)
                    material.MaterialType = GsaMaterial.MatType.FABRIC;
            }
            

            DA.SetData(0, new GsaMaterialGoo(material));
        }
        #region menu override
        private enum FoldMode
        {
            Generic,
            Steel,
            Concrete,
            Timber,
            Aluminium,
            FRP,
            Glass,
            Fabric,
            ElasticIsotropic
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.ElasticIsotropic;


        private void Mode1Clicked()
        {
            if (_mode == FoldMode.Generic)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");
            
            _mode = FoldMode.Generic;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Steel)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Steel;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.Concrete)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Concrete;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Timber)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Timber;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode5Clicked()
        {
            if (_mode == FoldMode.Aluminium)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Aluminium;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode6Clicked()
        {
            if (_mode == FoldMode.FRP)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.FRP;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode7Clicked()
        {
            if (_mode == FoldMode.Glass)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Glass;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode8Clicked()
        {
            if (_mode == FoldMode.Fabric)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Fabric;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode9Clicked()
        {
            if (_mode == FoldMode.ElasticIsotropic)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.ElasticIsotropic;

            UpdateInputs();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void UpdateInputs()
        {
            if (_mode == FoldMode.ElasticIsotropic)
                if (Params.Input.Count == 5) { return; }
            else
                if (Params.Input.Count == 2) { return; }

            RecordUndoEvent("Changed dropdown");

            // change number of input parameters
            if (_mode == FoldMode.ElasticIsotropic)
            {
                Params.UnregisterInputParameter(Params.Input[1], true);
                Params.RegisterInputParam(new Param_GenericObject());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_GenericObject());
                Params.RegisterInputParam(new Param_GenericObject());
            }
            else
            {
                while (Params.Input.Count > 1)
                    Params.UnregisterInputParameter(Params.Input[1], true);
                Params.RegisterInputParam(new Param_Integer());
            }

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        }
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
            
            _mode = (FoldMode)reader.GetInt32(selecteditems[0]);
            
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
            if (_mode == FoldMode.ElasticIsotropic)
            {
                IQuantity stress = new Pressure(0, stressUnit);
                stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
                IQuantity density = new Density(0, densityUnit);
                densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));
                IQuantity temperature = new Temperature(0, temperatureUnit);
                temperatureUnitAbbreviation = string.Concat(temperature.ToString().Where(char.IsLetter));

                int i = 1;
                Params.Input[i].Name = "Elastic Modulus [" + stressUnitAbbreviation + "]";
                Params.Input[i].NickName = "E";
                Params.Input[i].Description = "Elastic Modulus of the elastic isotropic material";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].Name = "Poisson's Ratio";
                Params.Input[i].NickName = "ν";
                Params.Input[i].Description = "Poisson's Ratio of the elastic isotropic material";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].Name = "Density [" + densityUnitAbbreviation + "]";
                Params.Input[i].NickName = "ρ";
                Params.Input[i].Description = "Density of the elastic isotropic material";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].Name = "Thermal Expansion [/" + temperatureUnitAbbreviation + "]";
                Params.Input[i].NickName = "α";
                Params.Input[i].Description = "Thermal Expansion Coefficient of the elastic isotropic material";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
            }
            else
            {
                // pManager.AddIntegerParameter("Grade", "Gr", "Material Grade (default = 1)", GH_ParamAccess.item, 1);
                Params.Input[1].Name = "Grade";
                Params.Input[1].NickName = "Gr";
                Params.Input[1].Description = "[Optional] Material Grade (default = 1)";
                Params.Input[1].Access = GH_ParamAccess.item;
                Params.Input[1].Optional = true;
            }
        }
        #endregion  
    }
}