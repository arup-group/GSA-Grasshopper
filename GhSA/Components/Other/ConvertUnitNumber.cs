using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to convert a UnitNumber
    /// </summary>
    public class ConvertUnitNumber : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("9864b817-3ac4-4d3f-b565-e43174f4fb0c");
        public ConvertUnitNumber()
          : base("Convert UnitNumber", "ConvertUnit", "Convert a unit number (quantity) into another unit",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat9())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.quinary;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertUnitNumber;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                if (selecteditems == null)
                {
                    // create a new list of selected items and add the first material type
                    selecteditems = new List<string>();
                    selecteditems.Add("   ");
                }
                if (dropdownitems == null)
                {
                    // create a new list of selected items and add the first material type
                    dropdownitems = new List<List<string>>();
                    List<string> unitTypes = new List<string>(new string[]
                    {
                        " ",
                    });
                    dropdownitems.Add(unitTypes);
                }

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }

        public void SetSelected(int i, int j)
        {
            if (unitDict != null)
            {
                // change selected item
                selecteditems[i] = dropdownitems[i][j];
                dropdownitems[0] = unitDict.Keys.ToList();
            }
        }
        private void UpdateUIFromSelectedItems()
        {
            CreateAttributes();
            ExpireSolution(true);
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        // get list of material types defined in material parameter

        // list of materials
        Dictionary<string, Enum> unitDict;
        Enum selectedUnit;
        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Select output unit"
        });
        private bool first = true;
        GH_UnitNumber convertedUnitNumber;
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("UnitNumber", "UN", "Number with a unit to be converted into another unit", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("UnitNumber", "UN", "Number converted to selected unit", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // get input
            GH_UnitNumber inUnitNumber = null;

            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                // try cast directly to quantity type
                if (gh_typ.Value is GH_UnitNumber)
                {
                    inUnitNumber = (GH_UnitNumber)gh_typ.Value;
                    if (convertedUnitNumber == null || !convertedUnitNumber.Value.QuantityInfo.UnitType.Equals(inUnitNumber.Value.QuantityInfo.UnitType))
                    {

                        unitDict = new Dictionary<string, Enum>();
                        foreach (UnitsNet.UnitInfo unit in inUnitNumber.Value.QuantityInfo.UnitInfos)
                        {
                            unitDict.Add(unit.Name, unit.Value);
                        }
                        dropdownitems[0] = unitDict.Keys.ToList();
                        if (!comingFromSave)
                        {
                            selecteditems[0] = inUnitNumber.Value.Unit.ToString();
                        }
                        else
                        {
                            comingFromSave = false;
                        }
                    }
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert UnitNumber input");
                    return;
                }
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input UnitNumber to populate dropdown menu");
                return;
            }

            // update selected material
            selectedUnit = unitDict[selecteditems.Last()];

            // convert unit to selected output
            convertedUnitNumber = new GH_UnitNumber(inUnitNumber.Value.ToUnit(selectedUnit));

            // set output data
            DA.SetData(0, convertedUnitNumber);
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
            comingFromSave = true;
            first = false;
            return base.Read(reader);
        }
        bool comingFromSave = false;
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
        }
        #endregion
    }
}