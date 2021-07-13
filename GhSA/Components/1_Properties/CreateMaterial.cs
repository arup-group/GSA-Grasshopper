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

using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;

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
                selecteditem = _mode.ToString();
                //first = false;
            }
                
            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Material Type");
        }

        public void SetSelected(string selected)
        {
            selecteditem = selected;
            switch (selected)
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
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Generic",
            "Steel",
            "Concrete",
            "Timber",
            "Aluminium",
            "FRP",
            "Glass",
            "Fabric"
        });

        string selecteditem;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Analysis Property", "An", "Analysis Property Number (default = 0 -> 'from Grade')", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Grade", "Gr", "Material Grade (default = 1)", GH_ParamAccess.item, 1);
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

            GH_Integer gh_grade = new GH_Integer();
            if (DA.GetData(1, ref gh_grade))
            {
                int grade = 1;
                GH_Convert.ToInt32(gh_grade, out grade, GH_Conversion.Both);
                material.GradeProperty = grade;
            }

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
            Fabric
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.Timber;


        private void Mode1Clicked()
        {
            if (_mode == FoldMode.Generic)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");
            
            _mode = FoldMode.Generic;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Steel)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Steel;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.Concrete)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Concrete;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Timber)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Timber;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode5Clicked()
        {
            if (_mode == FoldMode.Aluminium)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Aluminium;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode6Clicked()
        {
            if (_mode == FoldMode.FRP)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.FRP;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode7Clicked()
        {
            if (_mode == FoldMode.Glass)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Glass;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode8Clicked()
        {
            if (_mode == FoldMode.Fabric)
                return;

            RecordUndoEvent(_mode.ToString() + "Parameters");

            _mode = FoldMode.Fabric;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        #endregion
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Mode", (int)_mode);
            writer.SetString("select", selecteditem);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            selecteditem = reader.GetString("select");
            this.CreateAttributes();
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
            
        }
        #endregion  
    }
}