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
    /// Component to create a new Prop2d
    /// </summary>
    public class CreateProp2d : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("3fd61492-b5ff-47ea-8c7c-89cf639b32dc");
        public CreateProp2d()
          : base("Create 2D Property", "Prop2d", "Create GSA 2D Property",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.CreateProp2D;
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
                
            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Element Type");
        }

        public void SetSelected(string selected)
        {
            selecteditem = selected;
            switch (selected)
            {
                case "Plane Stress":
                    Mode1Clicked();
                    break;
                case "Fabric":
                    Mode2Clicked();
                    break;
                case "Flat Plate":
                    Mode3Clicked();
                    break;
                case "Shell":
                    Mode4Clicked();
                    break;
                case "Curved Shell":
                    Mode5Clicked();
                    break;
                case "Load Panel":
                    Mode6Clicked();
                    break;
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Plane Stress",
            "Fabric",
            "Flat Plate",
            "Shell",
            "Curved Shell",
            "Load Panel"
        });

        string selecteditem;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Number", "ID", "Property number PA# (default appended to model = 0). Will overwrite any existing property with same number", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Name", "Na", "GSA 2D Property Name", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour", "Col", "GSA 2D Property Colour)", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            if (first)
            {
                first = false;
                //register input parameter
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_String());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Number());

                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
                Params.OnParametersChanged();
                ExpireSolution(true);
            }
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaProp2d prop = new GsaProp2d();
            prop.Prop2d = new Prop2D();

            // element type (picked in dropdown)
            prop.Prop2d.Type = Property2D_Type.UNDEF;
            if (_mode == FoldMode.PlaneStress)
                prop.Prop2d.Type = Property2D_Type.PL_STRESS;
            if (_mode == FoldMode.Fabric)
                prop.Prop2d.Type = Property2D_Type.FABRIC;
            if (_mode == FoldMode.FlatPlate)
                prop.Prop2d.Type = Property2D_Type.PLATE;
            if (_mode == FoldMode.Shell)
                prop.Prop2d.Type = Property2D_Type.SHELL;
            if (_mode == FoldMode.CurvedShell)
                prop.Prop2d.Type = Property2D_Type.CURVED_SHELL;
            if (_mode == FoldMode.LoadPanel)
                prop.Prop2d.Type = Property2D_Type.LOAD;

            //id
            GH_Integer gh_ID = new GH_Integer();
            DA.GetData(0, ref gh_ID);
            int idd = 0;
            GH_Convert.ToInt32_Primary(gh_ID, ref idd);
            prop.ID = idd;

            //name
            GH_String gh_Name = new GH_String();
            DA.GetData(1, ref gh_Name);
            string name = "";
            GH_Convert.ToString_Primary(gh_Name, ref name);
            prop.Prop2d.Name = name;

            //colour
            GH_Colour gh_Colour = new GH_Colour();
            DA.GetData(2, ref gh_Colour);
            System.Drawing.Color colour = new System.Drawing.Color();
            GH_Convert.ToColor_Primary(gh_Colour, ref colour);
            prop.Prop2d.Colour = (ValueType)colour;

            if (_mode != FoldMode.LoadPanel)
            {
                //axis
                GH_Integer gh_Axis = new GH_Integer();
                DA.GetData(3, ref gh_Axis);
                int axis = 0;
                GH_Convert.ToInt32_Primary(gh_Axis, ref axis);
                prop.Prop2d.AxisProperty = axis;


                if (_mode != FoldMode.Fabric)
                {
                    //Material type
                    GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                    MaterialType matType = MaterialType.CONCRETE;
                    if (DA.GetData(4, ref gh_typ))
                    {
                        if (gh_typ.Value is MaterialType)
                            gh_typ.CastTo(ref matType);
                        if (gh_typ.Value is GH_String)
                        {
                            string typ = "CONCRETE";
                            GH_Convert.ToString_Primary(gh_typ, ref typ);
                            if (typ.ToUpper() == "STEEL")
                                matType = MaterialType.STEEL;
                            if (typ.ToUpper() == "CONCRETE")
                                matType = MaterialType.CONCRETE;
                            if (typ.ToUpper() == "FRP")
                                matType = MaterialType.FRP;
                            if (typ.ToUpper() == "ALUMINIUM")
                                matType = MaterialType.ALUMINIUM;
                            if (typ.ToUpper() == "TIMBER")
                                matType = MaterialType.TIMBER;
                            if (typ.ToUpper() == "GLASS")
                                matType = MaterialType.GLASS;
                            if (typ.ToUpper() == "FABRIC")
                                matType = MaterialType.FABRIC;
                            if (typ.ToUpper() == "GENERIC")
                                matType = MaterialType.GENERIC;
                        }
                    }
                    prop.Prop2d.MaterialType = matType;

                    //thickness
                    GH_Number gh_THK = new GH_Number();

                    double thickness = 0.2;
                    if (DA.GetData(7, ref gh_THK))
                        GH_Convert.ToDouble_Primary(gh_THK, ref thickness);
                    //prop.Prop2d.Thickness = thickness;
                }
                else
                    prop.Prop2d.MaterialType = MaterialType.FABRIC;

                //handle that the last two inputs are at different -1 index for fabric mode
                int fab = 0;
                if (_mode == FoldMode.Fabric)
                    fab = 1;

                //grade
                GH_Integer gh_grd = new GH_Integer();
                DA.GetData(5 - fab, ref gh_grd);
                int grade = 1;
                GH_Convert.ToInt32_Primary(gh_grd, ref grade);
                prop.Prop2d.MaterialGradeProperty = grade;

                //analysis
                GH_Integer gh_anal = new GH_Integer();
                DA.GetData(6 - fab, ref gh_anal);
                int analysis = 1;
                GH_Convert.ToInt32_Primary(gh_anal, ref analysis);
                prop.Prop2d.MaterialAnalysisProperty = analysis;
            }

            DA.SetData(0, new GsaProp2dGoo(prop));

        }
        #region menu override
        private enum FoldMode
        {
            PlaneStress,
            Fabric,
            FlatPlate,
            Shell,
            CurvedShell,
            LoadPanel
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.Shell;


        private void Mode1Clicked()
        {
            if (_mode == FoldMode.PlaneStress)
                return;

            RecordUndoEvent("Plane Stress Parameters");
            if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
            {
                //remove input parameters
                while (Params.Input.Count > 3)
                    Params.UnregisterInputParameter(Params.Input[3], true);

                //register input parameter
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_String());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.PlaneStress;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Fabric)
                return;

            RecordUndoEvent("Fabric Parameters");
            _mode = FoldMode.Fabric;

            //remove input parameters
            while (Params.Input.Count > 3)
                Params.UnregisterInputParameter(Params.Input[3], true);

            //register input parameter
            Params.RegisterInputParam(new Param_Integer());
            Params.RegisterInputParam(new Param_Integer());
            Params.RegisterInputParam(new Param_Integer());


            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.FlatPlate)
                return;

            RecordUndoEvent("Flat Plate Parameters");
            if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
            {
                //remove input parameters
                while (Params.Input.Count > 3)
                    Params.UnregisterInputParameter(Params.Input[3], true);

                //register input parameter
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_String());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.FlatPlate;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Shell)
                return;

            RecordUndoEvent("Shell Parameters");
            if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
            {
                //remove input parameters
                while (Params.Input.Count > 3)
                    Params.UnregisterInputParameter(Params.Input[3], true);

                //register input parameter
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_String());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.Shell;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode5Clicked()
        {
            if (_mode == FoldMode.CurvedShell)
                return;

            RecordUndoEvent("Curved Shell Parameters");
            if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
            {
                //remove input parameters
                while (Params.Input.Count > 3)
                    Params.UnregisterInputParameter(Params.Input[3], true);

                //register input parameter
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_String());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.CurvedShell;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode6Clicked()
        {
            if (_mode == FoldMode.LoadPanel)
                return;

            RecordUndoEvent("Load Panel Parameters");
            _mode = FoldMode.LoadPanel;

            //remove input parameters
            while (Params.Input.Count > 3)
                Params.UnregisterInputParameter(Params.Input[3], true);

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
            if (_mode != FoldMode.LoadPanel && _mode != FoldMode.Fabric)
            {
                int i = 3;
                Params.Input[i].NickName = "Ax";
                Params.Input[i].Name = "Axis";
                Params.Input[i].Description = "Axis as integer: Global (0) or Topological (1) (by default Global)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
                i++;
                Params.Input[i].NickName = "Mt";
                Params.Input[i].Name = "Material Type";
                Params.Input[i].Description = "Material type (default 'Concrete')" +
                    System.Environment.NewLine + "Steel" +
                    System.Environment.NewLine + "Concrete" +
                    System.Environment.NewLine + "FRP" +
                    System.Environment.NewLine + "Aluminium" +
                    System.Environment.NewLine + "Timber" +
                    System.Environment.NewLine + "Glass" +
                    System.Environment.NewLine + "Fabric" +
                    System.Environment.NewLine + "Generic";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
                i++;
                Params.Input[i].NickName = "Mg";
                Params.Input[i].Name = "Material Grade";
                Params.Input[i].Description = "Material grade (default 1)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
                i++;
                Params.Input[i].NickName = "Mat";
                Params.Input[i].Name = "Analysis Material";
                Params.Input[i].Description = "Analysis material (default from Grade)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
                i++;
                Params.Input[i].NickName = "Thk";
                Params.Input[i].Name = "Thickness [m]";
                Params.Input[i].Description = "Section thickness (default 0.2m)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
            }

            if (_mode == FoldMode.Fabric)
            {
                int i = 3;
                Params.Input[i].NickName = "Ax";
                Params.Input[i].Name = "Axis";
                Params.Input[i].Description = "Axis as integer: Global (0) or Topological (1) (by default Global)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
                i++;
                Params.Input[i].NickName = "Mg";
                Params.Input[i].Name = "Material Grade";
                Params.Input[i].Description = "Material grade (default 1)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
                i++;
                Params.Input[i].NickName = "Ma";
                Params.Input[i].Name = "Analysis Material";
                Params.Input[i].Description = "Analysis material (default from Grade)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;

            }
        }
        #endregion  
    }
}