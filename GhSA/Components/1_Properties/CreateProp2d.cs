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
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateProp2D;
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
            if (first)
            {
                first = false;
                //register input parameter
                //Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_GenericObject());
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
            prop.ID = 0;

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

            if (_mode != FoldMode.LoadPanel)
            {
                prop.Prop2d.AxisProperty = 0;

                if (_mode != FoldMode.Fabric)
                {
                    // 0 Material
                    GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                    if (DA.GetData(0, ref gh_typ))
                    {
                        GsaMaterial material = new GsaMaterial();
                        if (gh_typ.Value is GsaMaterialGoo)
                        {
                            gh_typ.CastTo(ref material);
                            prop.Material = material;
                        }
                        else
                        {
                            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                            {
                                prop.Material = new GsaMaterial(idd);

                            }
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                                return;
                            }
                        }
                    }
                    else
                        prop.Material = new GsaMaterial(2);

                    // 1 thickness
                    //GH_String gh_THK = new GH_String();
                    //string thickness = "0.2";
                    //if (DA.GetData(1, ref gh_THK))
                    //    GH_Convert.ToString(gh_THK, out thickness, GH_Conversion.Both);
                    //prop.Prop2d.Description = thickness;

                    GH_Number gh_THK = new GH_Number();
                    double thickness = 200;
                    if (DA.GetData(1, ref gh_THK))
                        GH_Convert.ToDouble(gh_THK, out thickness, GH_Conversion.Both);
                    prop.Thickness = thickness;
                }
                else
                    prop.Prop2d.MaterialType = MaterialType.FABRIC;
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
                while (Params.Input.Count > 0)
                    Params.UnregisterInputParameter(Params.Input[0], true);

                //register input parameter
                //Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_GenericObject());
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
            while (Params.Input.Count > 0)
                Params.UnregisterInputParameter(Params.Input[0], true);

            //register input parameter
            //Params.RegisterInputParam(new Param_Integer());
            Params.RegisterInputParam(new Param_GenericObject());
            


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
                while (Params.Input.Count > 0)
                    Params.UnregisterInputParameter(Params.Input[0], true);

                //register input parameter
                //Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_GenericObject());
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
                while (Params.Input.Count > 0)
                    Params.UnregisterInputParameter(Params.Input[0], true);

                //register input parameter
                //Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_GenericObject());
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
                while (Params.Input.Count > 0)
                    Params.UnregisterInputParameter(Params.Input[0], true);

                //register input parameter
                //Params.RegisterInputParam(new Param_Integer());
                Params.RegisterInputParam(new Param_GenericObject());
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
            while (Params.Input.Count > 0)
                Params.UnregisterInputParameter(Params.Input[0], true);

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
                int i = 0;
                Params.Input[i].NickName = "Mat";
                Params.Input[i].Name = "Material";
                Params.Input[i].Description = "GsaMaterial or Number for referring to a Material already in Existing GSA Model." + System.Environment.NewLine
                    + "Accepted inputs are: " + System.Environment.NewLine
                    + "0 : Generic" + System.Environment.NewLine
                    + "1 : Steel" + System.Environment.NewLine
                    + "2 : Concrete (default)" + System.Environment.NewLine
                    + "3 : Aluminium" + System.Environment.NewLine
                    + "4 : Glass" + System.Environment.NewLine
                    + "5 : FRP" + System.Environment.NewLine
                    + "7 : Timber" + System.Environment.NewLine
                    + "8 : Fabric";

                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;

                i++;
                Params.Input[i].NickName = "Thk";
                Params.Input[i].Name = "Thickness (" + Units.LengthSection + ")"; // "Thickness [m]";
                Params.Input[i].Description = "Section thickness (default 200mm)";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
            }

            if (_mode == FoldMode.Fabric)
            {
                int i = 0;
                Params.Input[i].NickName = "Mat";
                Params.Input[i].Name = "Material";
                Params.Input[i].Description = "GsaMaterial or Reference ID for Material Property in Existing GSA Model";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = true;
            }
        }
        #endregion  
    }
}