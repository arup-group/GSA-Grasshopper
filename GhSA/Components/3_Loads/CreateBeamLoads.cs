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
    public class CreateBeamLoads : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateBeamLoads()
            : base("Create Beam Load", "BeamLoad", "Create GSA Beam Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("a2bc3c66-eb22-43ec-9936-84d2944be414");
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
                case "Point":
                    Mode1Clicked();
                    break;
                case "Uniform":
                    Mode2Clicked();
                    break;
                case "Linear":
                    Mode3Clicked();
                    break;
                case "Patch":
                    Mode4Clicked();
                    break;
                case "Trilinear":
                    Mode5Clicked();
                    break;
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Point",
            "Uniform",
            "Linear",
            "Patch",
            "Trilinear"
        });

        string selecteditem;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Elements", "El", "1D element list", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Global). " +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "0 : Global" +
                    System.Environment.NewLine + "-1 : Local", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "x" +
                    System.Environment.NewLine + "y" +
                    System.Environment.NewLine + "z" +
                    System.Environment.NewLine + "xx" +
                    System.Environment.NewLine + "yy" +
                    System.Environment.NewLine + "zz", GH_ParamAccess.item, "z");
            pManager.AddBooleanParameter("Projected", "Pj", "Projected (default not)", GH_ParamAccess.item, false);

            pManager.AddNumberParameter("Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", "V", "Load Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            
            _mode = FoldMode.Uniform;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Beam Load", "Load", "GSA Beam Load", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaBeamLoad beamLoad = new GsaBeamLoad();
            
            //Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
            beamLoad.BeamLoad.Case = lc;

            //element/beam list
            string beamList = ""; //pick an initial name that is sure not to be used...
            GH_String gh_bl = new GH_String();
            if (DA.GetData(1, ref gh_bl))
                GH_Convert.ToString(gh_bl, out beamList, GH_Conversion.Both);
            //var isNumeric = int.TryParse(beamList, out int n);
            //if (isNumeric)
            //    beamList = "PB" + n;
            beamLoad.BeamLoad.Elements = beamList;

            //axis
            int axis = 0;
            beamLoad.BeamLoad.AxisProperty = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
            GH_Integer gh_ax = new GH_Integer();
            if (DA.GetData(2, ref gh_ax))
            {
                GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
                if (axis == 0 || axis == -1)
                    beamLoad.BeamLoad.AxisProperty = axis;
            }

            //direction
            string dir = "Z";
            Direction direc = Direction.Z;
            
            GH_String gh_dir = new GH_String();
            if (DA.GetData(5, ref gh_dir))
                GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
            dir = dir.ToUpper();
            if (dir == "X")
                direc = Direction.X;
            if (dir == "Y")
                direc = Direction.Y;
            if (dir == "XX")
                direc = Direction.XX;
            if (dir == "YY")
                direc = Direction.YY;
            if (dir == "ZZ")
                direc = Direction.ZZ;

            beamLoad.BeamLoad.Direction = direc;

            //projection
            bool prj = false;
            GH_Boolean gh_prj = new GH_Boolean();
            if (DA.GetData(4, ref gh_prj))
                GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
            beamLoad.BeamLoad.IsProjected = prj;

            double load1 = 0;
            if (DA.GetData(5, ref load1))
                load1 *= -1000; //convert to kN
            
            switch (_mode)
            {
                case FoldMode.Point:
                    if (_mode == FoldMode.Point)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.POINT;
                        
                        // get data
                        double pos = 0;
                        if (DA.GetData(6, ref pos))
                            pos *= -1;

                        // set position and value
                        beamLoad.BeamLoad.SetValue(0, load1);
                        beamLoad.BeamLoad.SetPosition(0, pos);
                    }
                    break;

                case FoldMode.Uniform:
                    if (_mode == FoldMode.Uniform)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.UNIFORM;
                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1);
                    }
                    break;

                case FoldMode.Linear:
                    if (_mode == FoldMode.Linear)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.LINEAR;

                        // get data
                        double load2 = 0;
                        if (DA.GetData(6, ref load2))
                            load2 *= -1000; //convert to kN

                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1);
                        beamLoad.BeamLoad.SetValue(1, load2);
                    }
                    break;

                case FoldMode.Patch:
                    if (_mode == FoldMode.Patch)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.PATCH;

                        // get data
                        double pos1 = 0;
                        if (DA.GetData(6, ref pos1))
                            pos1 *= -1;

                        double pos2 = 1;
                        if (DA.GetData(8, ref pos2))
                            pos2 *= -1;

                        double load2 = 0;
                        if (DA.GetData(7, ref load2))
                            load2 *= -1000; //convert to kN

                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1);
                        beamLoad.BeamLoad.SetValue(1, load2);
                        beamLoad.BeamLoad.SetPosition(0, pos1);
                        beamLoad.BeamLoad.SetPosition(1, pos2);
                    }
                    break;

                case FoldMode.Trilinear:
                    if (_mode == FoldMode.Trilinear)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.TRILINEAR;

                        // get data
                        double pos1 = 0;
                        if (DA.GetData(6, ref pos1))
                            pos1 *= -1;

                        double pos2 = 1;
                        if (DA.GetData(8, ref pos2))
                            pos2 *= -1;

                        double load2 = 0;
                        if (DA.GetData(7, ref load2))
                            load2 *= -1000; //convert to kN

                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1);
                        beamLoad.BeamLoad.SetValue(1, load2);
                        beamLoad.BeamLoad.SetPosition(0, pos1);
                        beamLoad.BeamLoad.SetPosition(1, pos2);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            GsaLoad gsaLoad = new GsaLoad(beamLoad);
            DA.SetData(0, new GsaLoadGoo(gsaLoad));
        }

        #region menu override
        private enum FoldMode
        {
            Point,
            Uniform,
            Linear,
            Patch,
            Trilinear
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.Uniform;

        private void Mode1Clicked()
        {
            if (_mode == FoldMode.Point)
                return;

            RecordUndoEvent("Point Parameters");
            _mode = FoldMode.Point;

            //remove input parameters
            while (Params.Input.Count > 6)
                Params.UnregisterInputParameter(Params.Input[6], true);
            Params.RegisterInputParam(new Param_Number());
            
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Uniform)
                return;

            RecordUndoEvent("Uniform Parameters");
            _mode = FoldMode.Uniform;

            //remove input parameters
            while (Params.Input.Count > 6)
                Params.UnregisterInputParameter(Params.Input[6], true);

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.Linear)
                return;

            RecordUndoEvent("Linear Parameters");
            _mode = FoldMode.Linear;

            //remove input parameters
            while (Params.Input.Count > 6)
                Params.UnregisterInputParameter(Params.Input[6], true);

            //add input parameters
            Params.RegisterInputParam(new Param_Number());

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Patch)
                return;

            RecordUndoEvent("Patch Parameters");
            if (_mode != FoldMode.Trilinear)
            {
                //remove input parameters
                while (Params.Input.Count > 6)
                    Params.UnregisterInputParameter(Params.Input[6], true);

                //add input parameters
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
                
            _mode = FoldMode.Patch;
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode5Clicked()
        {
            if (_mode == FoldMode.Trilinear)
                return;

            RecordUndoEvent("Trilinear Parameters");
            if(_mode != FoldMode.Patch)
            {
                //remove input parameters
                while (Params.Input.Count > 6)
                    Params.UnregisterInputParameter(Params.Input[6], true);

                //add input parameters
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
            _mode = FoldMode.Trilinear;
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
            if (_mode == FoldMode.Point)
            {
                Params.Input[5].NickName = "V";
                Params.Input[5].Name = "Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Description = "Load Value (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = false;
                
                Params.Input[6].NickName = "t";
                Params.Input[6].Name = "Position (%)";
                Params.Input[6].Description = "Line parameter where point load act (between 0.0 and 1.0)";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;
            }

            if (_mode == FoldMode.Uniform)
            {
                Params.Input[5].NickName = "V";
                Params.Input[5].Name = "Load (kN/m)";
                Params.Input[5].Description = "Load value (kN/m)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = false;
            }

            if (_mode == FoldMode.Linear)
            {
                Params.Input[5].NickName = "V1";
                Params.Input[5].Name = "Value Start (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Description = "Load Value at Beam Start (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;

                Params.Input[6].NickName = "V2";
                Params.Input[6].Name = "Value End (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[6].Description = "Load Value at Beam End (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;
            }

            if (_mode == FoldMode.Patch)
            {
                Params.Input[5].NickName = "V1";
                Params.Input[5].Name = "Load t1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Description = "Load Value at Position 1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;

                Params.Input[6].NickName = "t1";
                Params.Input[6].Name = "Position 1 (%)";
                Params.Input[6].Description = "Line parameter where patch load begins (between 0.0 and 1.0, but less than t2)";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "V2";
                Params.Input[7].Name = "Load t2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[7].Description = "Load Value at Position 2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;

                Params.Input[8].NickName = "t2";
                Params.Input[8].Name = "Position 2 (%)";
                Params.Input[8].Description = "Line parameter where patch load ends (between 0.0 and 1.0, but bigger than t1)";
                Params.Input[8].Access = GH_ParamAccess.item;
                Params.Input[8].Optional = true;
            }

            if (_mode == FoldMode.Trilinear)
            {
                Params.Input[5].NickName = "V1";
                Params.Input[5].Name = "Load t1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Description = "Load Value at Position 1 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;

                Params.Input[6].NickName = "t1";
                Params.Input[6].Name = "Position 1 (%)";
                Params.Input[6].Description = "Line parameter where L1 applies (between 0.0 and 1.0, but less than t2)";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "V2";
                Params.Input[7].Name = "Load t2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[7].Description = "Load Value at Position 2 (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;

                Params.Input[8].NickName = "t2";
                Params.Input[8].Name = "Position 2 (%)";
                Params.Input[8].Description = "Line parameter where L2 applies (between 0.0 and 1.0, but bigger than t1)";
                Params.Input[8].Access = GH_ParamAccess.item;
                Params.Input[8].Optional = true;
            }
        }
        #endregion
    }
}
