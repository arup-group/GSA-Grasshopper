using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using System.Resources;
using UnitsNet.GH;
using UnitsNet;
using GsaAPI;

namespace GhSA.Components
{
    /// <summary>
    /// Component to create a new Stress Strain Point
    /// </summary>
    public class CreateCustomMaterial : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("c8238847-81da-4f96-a926-a388672eaccc");
        public CreateCustomMaterial()
          : base("Create Custom Material", "Material", "Create a custom linear elastic GSA Material",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = false; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateMaterial;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // pressure
                dropdownitems.Add(Units.FilteredStressUnits);
                selecteditems.Add(stressUnit.ToString());

                // density
                dropdownitems.Add(Units.FilteredMassUnits);
                selecteditems.Add(densityUnit.ToString());

                // temperature
                dropdownitems.Add(Units.FilteredTemperatureUnits);
                selecteditems.Add(temperatureUnit.ToString());


                densityUnitAbbreviation = Oasys.Units.Strain.GetAbbreviation(densityUnit);
                IQuantity stress = new UnitsNet.Pressure(0, stressUnit);
                stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }

        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            switch (i)
            {
                case 0:
                    stressUnit = (UnitsNet.Units.PressureUnit)Enum.Parse(typeof(UnitsNet.Units.PressureUnit), selecteditems[i]);
                    break;
                case 1:
                    
                    break;
                case 2:
                    temperatureUnit = (UnitsNet.Units.TemperatureUnit)Enum.Parse(typeof(UnitsNet.Units.TemperatureUnit), selecteditems[i]);
                    break;
            }

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output

        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Stress Unit",
            "Density Unit",
            "Temperature"
        });
        private bool first = true;

        private UnitsNet.Units.DensityUnit densityUnit = ;
        private UnitsNet.Units.PressureUnit stressUnit = Units.StressUnit;
        private UnitsNet.Units.TemperatureUnit temperatureUnit = Units.TemperatureUnit;
        string densityUnitAbbreviation;
        string stressUnitAbbreviation;
        string temperatureUnitAbbreviation;
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Strain [" + densityUnitAbbreviation + "]", "ε", "Value for strain (X-axis)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Stress [" + stressUnitAbbreviation + "]", "σ", "Value for stress (Y-axis)", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("StressStrainPt", "SPt", "AdSec Stress Strain Point", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // create new point
            AdSecStressStrainPointGoo pt = new AdSecStressStrainPointGoo(
                GetInput.Stress(this, DA, 1, stressUnit),
                GetInput.Strain(this, DA, 0, densityUnit));

            DA.SetData(0, pt);
        }

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            AdSecGH.Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);

            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            AdSecGH.Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

            densityUnit = (Oasys.Units.StrainUnit)Enum.Parse(typeof(Oasys.Units.StrainUnit), selecteditems[0]);
            stressUnit = (UnitsNet.Units.PressureUnit)Enum.Parse(typeof(UnitsNet.Units.PressureUnit), selecteditems[1]);
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
            densityUnitAbbreviation = Oasys.Units.Strain.GetAbbreviation(densityUnit);
            IQuantity stress = new UnitsNet.Pressure(0, stressUnit);
            stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
            Params.Input[0].Name = "Strain [" + densityUnitAbbreviation + "]";
            Params.Input[1].Name = "Stress [" + stressUnitAbbreviation + "]";
        }
        #endregion
    }
}