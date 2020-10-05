using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using GsaAPI;
using GhSA.Parameters;

namespace GhSA.Components
{
    public class CreateFaceLoads : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateFaceLoads()
            : base("Create Face Load", "FaceLoad", "Create GSA Face Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("55aeaf97-ef0c-4061-a391-a6419448a0b5");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                selecteditem = _mode.ToString();
                first = false;
            }

            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Load Type");
        }

        public void SetSelected(string selected)
        {
            selecteditem = selected;
            switch (selected)
            {
                case "Uniform":
                    Mode1Clicked();
                    break;
                case "Variable":
                    Mode2Clicked();
                    break;
                case "Point":
                    Mode3Clicked();
                    break;
                case "Edge":
                    Mode4Clicked();
                    break;
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Uniform",
            "Variable",
            "Point",
            "Edge"
        });

        string selecteditem;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Elements", "El", "2D element list", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Local). " +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "0 : Global" +
                    System.Environment.NewLine + "-1 : Local", GH_ParamAccess.item, -1);
            pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "x" +
                    System.Environment.NewLine + "y" +
                    System.Environment.NewLine + "z", GH_ParamAccess.item, "z");
            pManager.AddBooleanParameter("Projected", "Pj", "Projected (default not)", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)", "V", "Load Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            
            _mode = FoldMode.Uniform;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Face Load", "Load", "GSA Face Load", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaFaceLoad faceLoad = new GsaFaceLoad();
            
            //Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32_Primary(gh_lc, ref lc);
            faceLoad.FaceLoad.Case = lc;

            //element/beam list
            string elemList = ""; 
            GH_String gh_el = new GH_String();
            if (DA.GetData(1, ref gh_el))
                GH_Convert.ToString_Primary(gh_el, ref elemList);
            //var isNumeric = int.TryParse(elemList, out int n);
            //if (isNumeric)
            //    elemList = "PA" + n;

            faceLoad.FaceLoad.Elements = elemList;

            //axis
            int axis = -1;
            faceLoad.FaceLoad.AxisProperty = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
            GH_Integer gh_ax = new GH_Integer();
            if (DA.GetData(2, ref gh_ax))
            {
                GH_Convert.ToInt32_Primary(gh_ax, ref axis);
                if (axis == 0 || axis ==-1)
                    faceLoad.FaceLoad.AxisProperty = axis;
            }
            
            //direction
            string dir = "Z";
            Direction direc = Direction.Z;
            
            GH_String gh_dir = new GH_String();
            if (DA.GetData(3, ref gh_dir))
                GH_Convert.ToString_Primary(gh_dir, ref dir);
            dir = dir.ToUpper();
            if (dir == "X")
                direc = Direction.X;
            if (dir == "Y")
                direc = Direction.Y;

            faceLoad.FaceLoad.Direction = direc;

            
            switch (_mode)
            {
                case FoldMode.Uniform:
                    if (_mode == FoldMode.Uniform)
                    {
                        faceLoad.FaceLoad.Type = FaceLoadType.CONSTANT;

                        //projection
                        bool prj = false;
                        GH_Boolean gh_prj = new GH_Boolean();
                        if (DA.GetData(4, ref gh_prj))
                            GH_Convert.ToBoolean_Primary(gh_prj, ref prj);
                        faceLoad.FaceLoad.IsProjected = prj;


                        double load1 = 0;
                        if (DA.GetData(5, ref load1))
                            load1 *= -1000; //convert to kN

                        // set position and value
                        faceLoad.FaceLoad.SetValue(0, load1);
                    }
                    break;

                case FoldMode.Variable:
                    if (_mode == FoldMode.Variable)
                    {
                        faceLoad.FaceLoad.Type = FaceLoadType.GENERAL;

                        //projection
                        bool prj = false;
                        GH_Boolean gh_prj = new GH_Boolean();
                        if (DA.GetData(4, ref gh_prj))
                            GH_Convert.ToBoolean_Primary(gh_prj, ref prj);
                        faceLoad.FaceLoad.IsProjected = prj;

                        double load1 = 0;
                        if (DA.GetData(5, ref load1))
                            load1 *= -1000; //convert to kN
                        double load2 = 0;
                        if (DA.GetData(6, ref load2))
                            load2 *= -1000; //convert to kN
                        double load3 = 0;
                        if (DA.GetData(7, ref load3))
                            load3 *= -1000; //convert to kN
                        double load4 = 0;
                        if (DA.GetData(8, ref load4))
                            load4 *= -1000; //convert to kN

                        // set value
                        faceLoad.FaceLoad.SetValue(0, load1);
                        faceLoad.FaceLoad.SetValue(1, load2);
                        faceLoad.FaceLoad.SetValue(2, load3);
                        faceLoad.FaceLoad.SetValue(3, load4);
                    }
                    break;

                case FoldMode.Point:
                    if (_mode == FoldMode.Point)
                    {
                        faceLoad.FaceLoad.Type = FaceLoadType.POINT;

                        //projection
                        bool prj = false;
                        GH_Boolean gh_prj = new GH_Boolean();
                        if (DA.GetData(4, ref gh_prj))
                            GH_Convert.ToBoolean_Primary(gh_prj, ref prj);
                        faceLoad.FaceLoad.IsProjected = prj;

                        double load1 = 0;
                        if (DA.GetData(5, ref load1))
                            load1 *= -1000; //convert to kN
                        double r = 0;
                        DA.GetData(6, ref r);
                            
                        double s = 0;
                        DA.GetData(7, ref s);
                        
                        // set position and value
                        faceLoad.FaceLoad.SetValue(0, load1);
                        //faceLoad.Position.X = r; //note Vector2 currently only get in GsaAPI
                        //faceLoad.Position.Y = s;

                    }
                    break;

                case FoldMode.Edge:
                    if (_mode == FoldMode.Edge)
                    {
                        //faceLoad.Type = BeamLoadType.EDGE; GsaAPI implementation missing

                        // get data
                        int edge = 1;
                        DA.GetData(4, ref edge);

                        double load1 = 0;
                        if (DA.GetData(5, ref load1))
                            load1 *= -1000; //convert to kN

                        double load2 = 0;
                        if (DA.GetData(6, ref load2))
                            load2 *= -1000; //convert to kN

                        // set value
                        faceLoad.FaceLoad.SetValue(0, load1);
                        faceLoad.FaceLoad.SetValue(1, load2);
                        //faceLoad.Edge = edge; //note implementation of edge-load is not yet supported in GsaAPI

                        faceLoad = null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            DA.SetData(0, new GsaLoad(faceLoad));
        }

        #region menu override
        private enum FoldMode
        {
            Uniform,
            Variable,
            Point,
            Edge
        }
        
        private FoldMode _mode = FoldMode.Uniform;
        private bool first = true;
        private void Mode1Clicked()
        {
            if (_mode == FoldMode.Uniform)
                return;

            RecordUndoEvent("Uniform Parameters");
            //remove input parameters
            if (_mode == FoldMode.Edge)
            {
                while (Params.Input.Count > 4)
                    Params.UnregisterInputParameter(Params.Input[4], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Boolean());
                Params.RegisterInputParam(new Param_Number());
            }
            else
            {
                while (Params.Input.Count > 5)
                    Params.UnregisterInputParameter(Params.Input[5], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.Uniform;
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Variable)
                return;

            RecordUndoEvent("Variable Parameters");
            //remove input parameters
            if (_mode == FoldMode.Edge)
            {
                while (Params.Input.Count > 4)
                    Params.UnregisterInputParameter(Params.Input[4], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Boolean());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
            else
            {
                while (Params.Input.Count > 5)
                    Params.UnregisterInputParameter(Params.Input[5], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.Variable;
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode3Clicked()
        {
            if (_mode == FoldMode.Point)
                return;

            RecordUndoEvent("Point Parameters");
            //remove input parameters
            if (_mode == FoldMode.Edge)
            {
                while (Params.Input.Count > 4)
                    Params.UnregisterInputParameter(Params.Input[4], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Boolean());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
            else
            {
                while (Params.Input.Count > 5)
                    Params.UnregisterInputParameter(Params.Input[5], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.Point;
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Edge)
                return;

            RecordUndoEvent("Edge Parameters");

            //remove input parameters
            while (Params.Input.Count > 4)
                Params.UnregisterInputParameter(Params.Input[4], true);

            //add input parameters
            Params.RegisterInputParam(new Param_Number());
            Params.RegisterInputParam(new Param_Number());
            Params.RegisterInputParam(new Param_Number());
            
                
            _mode = FoldMode.Edge;
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
            if (_mode == FoldMode.Uniform)
            {
                Params.Input[4].NickName = "Pj";
                Params.Input[4].Name = "Projected";
                Params.Input[4].Description = "Projected (default not)";
                Params.Input[4].Access = GH_ParamAccess.item;
                Params.Input[4].Optional = true;
                
                Params.Input[5].NickName = "V";
                Params.Input[5].Name = "Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Description = "Load Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = false;
            }

            if (_mode == FoldMode.Variable)
            {
                Params.Input[4].NickName = "Pj";
                Params.Input[4].Name = "Projected";
                Params.Input[4].Description = "Projected (default not)";
                Params.Input[4].Access = GH_ParamAccess.item;
                Params.Input[4].Optional = true;

                Params.Input[5].NickName = "V1";
                Params.Input[5].Name = "Value 1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Description = "Load Value Corner 1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;

                Params.Input[6].NickName = "V2";
                Params.Input[6].Name = "Value 2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[6].Description = "Load Value Corner 2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "V3";
                Params.Input[7].Name = "Value 3 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[7].Description = "Load Value Corner 3 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;

                Params.Input[8].NickName = "V4";
                Params.Input[8].Name = "Value 4 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[8].Description = "Load Value Corner 4 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[8].Access = GH_ParamAccess.item;
                Params.Input[8].Optional = true;
            }

            if (_mode == FoldMode.Point)
            {
                Params.Input[4].NickName = "Pj";
                Params.Input[4].Name = "Projected";
                Params.Input[4].Description = "Projected (default not)";
                Params.Input[4].Access = GH_ParamAccess.item;
                Params.Input[4].Optional = true;

                Params.Input[5].NickName = "V";
                Params.Input[5].Name = "Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Description = "Load Value Corner 1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = false;

                Params.Input[6].NickName = "r";
                Params.Input[6].Name = "Position r";
                Params.Input[6].Description = "The position r of the point load to be specified in ( r , s )" +
                    System.Environment.NewLine + "coordinates based on two-dimensional shape function." +
                    System.Environment.NewLine + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." +
                    System.Environment.NewLine + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "s";
                Params.Input[7].Name = "Position s";
                Params.Input[6].Description = "The position s of the point load to be specified in ( r , s )" +
                    System.Environment.NewLine + "coordinates based on two-dimensional shape function." +
                    System.Environment.NewLine + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." +
                    System.Environment.NewLine + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;
            }

            if (_mode == FoldMode.Edge)
            {
                Params.Input[4].NickName = "Ed";
                Params.Input[4].Name = "Edge";
                Params.Input[4].Description = "Edge (1, 2, 3 or 4)";
                Params.Input[4].Access = GH_ParamAccess.item;
                Params.Input[4].Optional = false;

                Params.Input[5].NickName = "V1";
                Params.Input[5].Name = "Value 1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Description = "Load Value Corner 1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = false;

                Params.Input[6].NickName = "V2";
                Params.Input[6].Name = "Value 2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[6].Description = "Load Value Corner 2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + "\xB2)";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = false;
            }
        }
        #endregion
    }
}
