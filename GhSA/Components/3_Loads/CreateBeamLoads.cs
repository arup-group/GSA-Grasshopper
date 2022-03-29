using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GsaGH.Parameters;
using UnitsNet;
using UnitsNet.Units;

namespace GsaGH.Components
{
    public class CreateBeamLoads : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateBeamLoads()
            : base("Create Beam Load", "BeamLoad", "Create GSA Beam Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("e034b346-a6e8-4dd1-b12c-6104baa2586e");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.BeamLoad;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(loadTypeOptions);
                dropdownitems.Add(Units.FilteredForcePerLengthUnits);

                selecteditems = new List<string>();
                selecteditems.Add(_mode.ToString());
                selecteditems.Add(Units.ForcePerLengthUnit.ToString());

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
            else
            {
                forcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[1]);
                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            }
                
            // update input params
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            _mode = (FoldMode)Enum.Parse(typeof(FoldMode), selecteditems[0]);
            forcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[1]);

            CreateAttributes();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        readonly List<string> loadTypeOptions = new List<string>(new string[]
        {
            "Point",
            "Uniform",
            "Linear",
            "Patch",
            "Trilinear"
        });

        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Load Type",
            "Unit",
        });

        private ForcePerLengthUnit forcePerLengthUnit = Units.ForcePerLengthUnit;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity force = new ForcePerLength(0, forcePerLengthUnit);
            string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Element list", "El", "List of Elements to apply load to." + System.Environment.NewLine +
                "Element list should take the form:" + System.Environment.NewLine +
                " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
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
            pManager.AddNumberParameter("Value [" + unitAbbreviation + "]", "V", "Load Value", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;

            _mode = FoldMode.Uniform;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Beam Load", "Ld", "GSA Beam Load", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaBeamLoad beamLoad = new GsaBeamLoad();
            
            // 0 Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
            beamLoad.BeamLoad.Case = lc;

            // 1 element/beam list
            // check that user has not inputted Gsa geometry elements here
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(1, ref gh_typ))
            {
                string type = gh_typ.Value.ToString().ToUpper();
                if (type.StartsWith("GSA "))
                {
                    Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, 
                        "You cannot input a Node/Element/Member in ElementList input!" +System.Environment.NewLine +
                        "Element list should take the form:" + System.Environment.NewLine +
                        "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + System.Environment.NewLine +
                        "Refer to GSA help file for definition of lists and full vocabulary."); 
                    return;
                }
            }

            // Get Geometry input
            string beamList = ""; 
            GH_String gh_bl = new GH_String();
            if (DA.GetData(1, ref gh_bl))
                GH_Convert.ToString(gh_bl, out beamList, GH_Conversion.Both);
            //var isNumeric = int.TryParse(beamList, out int n);
            //if (isNumeric)
            //    beamList = "PB" + n;
            beamLoad.BeamLoad.Elements = beamList;

            // 2 Name
            string name = "";
            GH_String gh_name = new GH_String();
            if (DA.GetData(2, ref gh_name))
            {
                if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
                    beamLoad.BeamLoad.Name = name;
            }

            // 3 axis
            int axis = 0;
            beamLoad.BeamLoad.AxisProperty = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
            GH_Integer gh_ax = new GH_Integer();
            if (DA.GetData(3, ref gh_ax))
            {
                GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
                if (axis == 0 || axis == -1)
                    beamLoad.BeamLoad.AxisProperty = axis;
            }

            // 4 direction
            string dir = "Z";
            Direction direc = Direction.Z;
            
            GH_String gh_dir = new GH_String();
            if (DA.GetData(4, ref gh_dir))
                GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
            dir = dir.ToUpper().Trim();
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

            // 5 projection
            bool prj = false;
            GH_Boolean gh_prj = new GH_Boolean();
            if (DA.GetData(5, ref gh_prj))
                GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
            beamLoad.BeamLoad.IsProjected = prj;

            // 6 value (1)
            ForcePerLength load1 = GetInput.ForcePerLength(this, DA, 6, forcePerLengthUnit);
            
            switch (_mode)
            {
                case FoldMode.Point:
                    if (_mode == FoldMode.Point)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.POINT;
                        
                        // 7 pos (1)
                        double pos = 0;
                        if (DA.GetData(7, ref pos))
                            pos *= -1;

                        // set position and value
                        beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
                        beamLoad.BeamLoad.SetPosition(0, pos);
                    }
                    break;

                case FoldMode.Uniform:
                    if (_mode == FoldMode.Uniform)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.UNIFORM;
                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
                    }
                    break;

                case FoldMode.Linear:
                    if (_mode == FoldMode.Linear)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.LINEAR;

                        // 7 value (2)
                        ForcePerLength load2 = GetInput.ForcePerLength(this, DA, 7, forcePerLengthUnit);

                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
                        beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
                    }
                    break;

                case FoldMode.Patch:
                    if (_mode == FoldMode.Patch)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.PATCH;

                        // 7 pos (1)
                        double pos1 = 0;
                        if (DA.GetData(7, ref pos1))
                            pos1 *= -1;

                        // 9 pos (2)
                        double pos2 = 1;
                        if (DA.GetData(9, ref pos2))
                            pos2 *= -1;

                        // 8 value (2)
                        ForcePerLength load2 = GetInput.ForcePerLength(this, DA, 8, forcePerLengthUnit);

                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
                        beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
                        beamLoad.BeamLoad.SetPosition(0, pos1);
                        beamLoad.BeamLoad.SetPosition(1, pos2);
                    }
                    break;

                case FoldMode.Trilinear:
                    if (_mode == FoldMode.Trilinear)
                    {
                        beamLoad.BeamLoad.Type = BeamLoadType.TRILINEAR;

                        // 7 pos (1)
                        double pos1 = 0;
                        if (DA.GetData(7, ref pos1))
                            pos1 *= -1;

                        // 9 pos (2)
                        double pos2 = 1;
                        if (DA.GetData(9, ref pos2))
                            pos2 *= -1;

                        // 8 value (2)
                        ForcePerLength load2 = GetInput.ForcePerLength(this, DA, 8, forcePerLengthUnit);

                        // set value
                        beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
                        beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
                        beamLoad.BeamLoad.SetPosition(0, pos1);
                        beamLoad.BeamLoad.SetPosition(1, pos2);
                    }
                    break;
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
            while (Params.Input.Count > 7)
                Params.UnregisterInputParameter(Params.Input[7], true);
            Params.RegisterInputParam(new Param_GenericObject());
            
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
            while (Params.Input.Count > 7)
                Params.UnregisterInputParameter(Params.Input[7], true);

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
            while (Params.Input.Count > 7)
                Params.UnregisterInputParameter(Params.Input[7], true);

            //add input parameters
            Params.RegisterInputParam(new Param_GenericObject());

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
                while (Params.Input.Count > 7)
                    Params.UnregisterInputParameter(Params.Input[7], true);

                //add input parameters
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_GenericObject());
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
                while (Params.Input.Count > 7)
                    Params.UnregisterInputParameter(Params.Input[7], true);

                //add input parameters
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_GenericObject());
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
            Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            try // this will fail if user has an old version of the component
            {
                Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            }
            catch (Exception) // we set the stored values like first initation of component
            {
                _mode = (FoldMode)reader.GetInt32("Mode");

                dropdownitems = new List<List<string>>();
                dropdownitems.Add(loadTypeOptions);
                dropdownitems.Add(Units.FilteredForceUnits);
                dropdownitems.Add(Units.FilteredLengthUnits);

                selecteditems = new List<string>();
                selecteditems.Add(reader.GetString("select"));
                selecteditems.Add(ForceUnit.Kilonewton.ToString());
                selecteditems.Add(LengthUnit.Meter.ToString());
            }
            first = false;

            UpdateUIFromSelectedItems();
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
            IQuantity force = new ForcePerLength(0, forcePerLengthUnit);
            string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

            if (_mode == FoldMode.Point)
            {
                Params.Input[6].NickName = "V";
                Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = false;
                
                Params.Input[7].NickName = "t";
                Params.Input[7].Name = "Position (%)";
                Params.Input[7].Description = "Line parameter where point load act (between 0.0 and 1.0)";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;
            }

            if (_mode == FoldMode.Uniform)
            {
                Params.Input[6].NickName = "V";
                Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = false;
            }

            if (_mode == FoldMode.Linear)
            {
                Params.Input[6].NickName = "V1";
                Params.Input[6].Name = "Value Start [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value at Beam Start";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "V2";
                Params.Input[7].Name = "Value End [" + unitAbbreviation + "]";
                Params.Input[7].Description = "Load Value at Beam End";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;
            }

            if (_mode == FoldMode.Patch)
            {
                Params.Input[6].NickName = "V1";
                Params.Input[6].Name = "Load t1 [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value at Position 1";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "t1";
                Params.Input[7].Name = "Position 1 [%]";
                Params.Input[7].Description = "Line parameter where patch load begins (between 0.0 and 1.0, but less than t2)";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;

                Params.Input[8].NickName = "V2";
                Params.Input[8].Name = "Load t2 [" + unitAbbreviation + "]";
                Params.Input[8].Description = "Load Value at Position 2";
                Params.Input[8].Access = GH_ParamAccess.item;
                Params.Input[8].Optional = true;

                Params.Input[9].NickName = "t2";
                Params.Input[9].Name = "Position 2 [%]";
                Params.Input[9].Description = "Line parameter where patch load ends (between 0.0 and 1.0, but bigger than t1)";
                Params.Input[9].Access = GH_ParamAccess.item;
                Params.Input[9].Optional = true;
            }

            if (_mode == FoldMode.Trilinear)
            {
                Params.Input[6].NickName = "V1";
                Params.Input[6].Name = "Load t1 [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value at Position 1";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "t1";
                Params.Input[7].Name = "Position 1 [%]";
                Params.Input[7].Description = "Line parameter where L1 applies (between 0.0 and 1.0, but less than t2)";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;

                Params.Input[8].NickName = "V2";
                Params.Input[8].Name = "Load t2 [" + unitAbbreviation + "]";
                Params.Input[8].Description = "Load Value at Position 2";
                Params.Input[8].Access = GH_ParamAccess.item;
                Params.Input[8].Optional = true;

                Params.Input[9].NickName = "t2";
                Params.Input[9].Name = "Position 2 [%]";
                Params.Input[9].Description = "Line parameter where L2 applies (between 0.0 and 1.0, but bigger than t1)";
                Params.Input[9].Access = GH_ParamAccess.item;
                Params.Input[9].Optional = true;
            }
        }
        #endregion
    }
}
