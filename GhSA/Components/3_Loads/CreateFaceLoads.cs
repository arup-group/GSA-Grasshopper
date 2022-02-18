using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GsaGH.Parameters;
using UnitsNet.Units;
using UnitsNet;

namespace GsaGH.Components
{
    public class CreateFaceLoads : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateFaceLoads()
            : base("Create Face Load", "FaceLoad", "Create GSA Face Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("c4ad7a1e-350b-48b2-b636-24b6ef7bd0f3");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.FaceLoad;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(loadTypeOptions);
                dropdownitems.Add(Units.FilteredStressUnits);

                selecteditems = new List<string>();
                selecteditems.Add(_mode.ToString());

                PressureUnit pressureUnit = (Force.From(1, Units.ForceUnit) / (Length.From(1, Units.LengthUnitGeometry) * Length.From(1, Units.LengthUnitGeometry))).Unit;
                selecteditems.Add(pressureUnit.ToString());

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
            else
            {
                forceAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[1]);
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
            forceAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[1]);

            CreateAttributes();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        readonly List<string> loadTypeOptions = new List<string>(new string[]
        {
            "Uniform",
            "Variable",
            "Point",
            "Edge"
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

        private PressureUnit forceAreaUnit = (Force.From(1, Units.ForceUnit) / (Length.From(1, Units.LengthUnitGeometry) * Length.From(1, Units.LengthUnitGeometry))).Unit;
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity force = new Pressure(0, forceAreaUnit);
            string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Element list", "El", "List of Elements to apply load to." + System.Environment.NewLine +
                "Element list should take the form:" + System.Environment.NewLine +
                " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
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
            pManager.AddGenericParameter("Face Load", "Ld", "GSA Face Load", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaFaceLoad faceLoad = new GsaFaceLoad();
            
            // 0 Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
            faceLoad.FaceLoad.Case = lc;

            // 1 element/beam list
            // check that user has not inputted Gsa geometry elements here
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(1, ref gh_typ))
            {
                string type = gh_typ.Value.ToString().ToUpper();
                if (type.StartsWith("GSA "))
                {
                    Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                        "You cannot input a Node/Element/Member in ElementList input!" + System.Environment.NewLine +
                        "Element list should take the form:" + System.Environment.NewLine +
                        "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + System.Environment.NewLine +
                        "Refer to GSA help file for definition of lists and full vocabulary.");
                    return;
                }
            }
            string elemList = ""; 
            GH_String gh_el = new GH_String();
            if (DA.GetData(1, ref gh_el))
                GH_Convert.ToString(gh_el, out elemList, GH_Conversion.Both);
            //var isNumeric = int.TryParse(elemList, out int n);
            //if (isNumeric)
            //    elemList = "PA" + n;

            faceLoad.FaceLoad.Elements = elemList;

            // 2 Name
            string name = "";
            GH_String gh_name = new GH_String();
            if (DA.GetData(2, ref gh_name))
            {
                if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
                    faceLoad.FaceLoad.Name = name;
            }

            // 3 axis
            int axis = -1;
            faceLoad.FaceLoad.AxisProperty = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
            GH_Integer gh_ax = new GH_Integer();
            if (DA.GetData(3, ref gh_ax))
            {
                GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
                if (axis == 0 || axis ==-1)
                    faceLoad.FaceLoad.AxisProperty = axis;
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
                        if (DA.GetData(5, ref gh_prj))
                            GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
                        faceLoad.FaceLoad.IsProjected = prj;

                        Pressure load1 = GetInput.Stress(this, DA, 6, forceAreaUnit);

                        // set position and value
                        faceLoad.FaceLoad.SetValue(0, load1.NewtonsPerSquareMeter);
                    }
                    break;

                case FoldMode.Variable:
                    if (_mode == FoldMode.Variable)
                    {
                        faceLoad.FaceLoad.Type = FaceLoadType.GENERAL;

                        //projection
                        bool prj = false;
                        GH_Boolean gh_prj = new GH_Boolean();
                        if (DA.GetData(5, ref gh_prj))
                            GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
                        faceLoad.FaceLoad.IsProjected = prj;

                        // set value
                        faceLoad.FaceLoad.SetValue(0, GetInput.Stress(this, DA, 6, forceAreaUnit, true).NewtonsPerSquareMeter);
                        faceLoad.FaceLoad.SetValue(1, GetInput.Stress(this, DA, 7, forceAreaUnit, true).NewtonsPerSquareMeter);
                        faceLoad.FaceLoad.SetValue(2, GetInput.Stress(this, DA, 8, forceAreaUnit, true).NewtonsPerSquareMeter);
                        faceLoad.FaceLoad.SetValue(3, GetInput.Stress(this, DA, 9, forceAreaUnit, true).NewtonsPerSquareMeter);
                    }
                    break;

                case FoldMode.Point:
                    if (_mode == FoldMode.Point)
                    {
                        faceLoad.FaceLoad.Type = FaceLoadType.POINT;

                        //projection
                        bool prj = false;
                        GH_Boolean gh_prj = new GH_Boolean();
                        if (DA.GetData(5, ref gh_prj))
                            GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
                        faceLoad.FaceLoad.IsProjected = prj;

                        double r = 0;
                        DA.GetData(7, ref r);
                            
                        double s = 0;
                        DA.GetData(8, ref s);
                        
                        // set position and value
                        faceLoad.FaceLoad.SetValue(0, GetInput.Stress(this, DA, 6, forceAreaUnit).NewtonsPerSquareMeter);
                        //faceLoad.Position.X = r; //note Vector2 currently only get in GsaAPI
                        //faceLoad.Position.Y = s;
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Warning: the position cannot be set in GsaAPI at the moment");
                    }
                    break;

                case FoldMode.Edge:
                    if (_mode == FoldMode.Edge)
                    {
                        //faceLoad.Type = BeamLoadType.EDGE; GsaAPI implementation missing

                        // get data
                        int edge = 1;
                        DA.GetData(5, ref edge);

                        faceLoad.FaceLoad.SetValue(0, GetInput.Stress(this, DA, 6, forceAreaUnit).NewtonsPerSquareMeter);
                        if (this.Params.Input[7].SourceCount != 0)
                            faceLoad.FaceLoad.SetValue(1, GetInput.Stress(this, DA, 7, forceAreaUnit).NewtonsPerSquareMeter);
                        else
                            faceLoad.FaceLoad.SetValue(1, faceLoad.FaceLoad.Value(0));

                        //faceLoad.FaceLoad. = edge; //note implementation of edge-load is not yet supported in GsaAPI

                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Warning: edge-load is not yet supported in GsaAPI");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            GsaLoad gsaLoad = new GsaLoad(faceLoad);
            DA.SetData(0, new GsaLoadGoo(gsaLoad));
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
                while (Params.Input.Count > 5)
                    Params.UnregisterInputParameter(Params.Input[5], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Boolean());
                Params.RegisterInputParam(new Param_Number());
            }
            else
            {
                while (Params.Input.Count > 6)
                    Params.UnregisterInputParameter(Params.Input[6], true);
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
                while (Params.Input.Count > 5)
                    Params.UnregisterInputParameter(Params.Input[5], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Boolean());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
            else
            {
                while (Params.Input.Count > 6)
                    Params.UnregisterInputParameter(Params.Input[6], true);
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
                while (Params.Input.Count > 5)
                    Params.UnregisterInputParameter(Params.Input[5], true);
                //add input parameters
                Params.RegisterInputParam(new Param_Boolean());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
                Params.RegisterInputParam(new Param_Number());
            }
            else
            {
                while (Params.Input.Count > 6)
                    Params.UnregisterInputParameter(Params.Input[6], true);
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
            while (Params.Input.Count > 5)
                Params.UnregisterInputParameter(Params.Input[5], true);

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
                dropdownitems.Add(Units.FilteredStressUnits);

                selecteditems = new List<string>();
                selecteditems.Add(reader.GetString("select"));
                selecteditems.Add(PressureUnit.KilonewtonPerSquareMeter.ToString());
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
            IQuantity force = new Pressure(0, forceAreaUnit);
            string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

            if (_mode == FoldMode.Uniform)
            {
                Params.Input[5].NickName = "Pj";
                Params.Input[5].Name = "Projected";
                Params.Input[5].Description = "Projected (default not)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;
                
                Params.Input[6].NickName = "V";
                Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = false;
            }

            if (_mode == FoldMode.Variable)
            {
                Params.Input[5].NickName = "Pj";
                Params.Input[5].Name = "Projected";
                Params.Input[5].Description = "Projected (default not)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;

                Params.Input[6].NickName = "V1";
                Params.Input[6].Name = "Value 1 [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value Corner 1";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = true;

                Params.Input[7].NickName = "V2";
                Params.Input[7].Name = "Value 2 [" + unitAbbreviation + "]";
                Params.Input[7].Description = "Load Value Corner 2";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;

                Params.Input[8].NickName = "V3";
                Params.Input[8].Name = "Value 3 [" + unitAbbreviation + "]";
                Params.Input[8].Description = "Load Value Corner 3";
                Params.Input[8].Access = GH_ParamAccess.item;
                Params.Input[8].Optional = true;

                Params.Input[9].NickName = "V4";
                Params.Input[9].Name = "Value 4 [" + unitAbbreviation + "]";
                Params.Input[9].Description = "Load Value Corner 4";
                Params.Input[9].Access = GH_ParamAccess.item;
                Params.Input[9].Optional = true;
            }

            if (_mode == FoldMode.Point)
            {
                Params.Input[5].NickName = "Pj";
                Params.Input[5].Name = "Projected";
                Params.Input[5].Description = "Projected (default not)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;

                Params.Input[6].NickName = "V";
                Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value Corner 1";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = false;

                Params.Input[7].NickName = "r";
                Params.Input[7].Name = "Position r";
                Params.Input[7].Description = "The position r of the point load to be specified in ( r , s )" +
                    System.Environment.NewLine + "coordinates based on two-dimensional shape function." +
                    System.Environment.NewLine + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." +
                    System.Environment.NewLine + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = true;

                Params.Input[8].NickName = "s";
                Params.Input[8].Name = "Position s";
                Params.Input[8].Description = "The position s of the point load to be specified in ( r , s )" +
                    System.Environment.NewLine + "coordinates based on two-dimensional shape function." +
                    System.Environment.NewLine + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." +
                    System.Environment.NewLine + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
                Params.Input[8].Access = GH_ParamAccess.item;
                Params.Input[8].Optional = true;
            }

            if (_mode == FoldMode.Edge)
            {
                Params.Input[5].NickName = "Ed";
                Params.Input[5].Name = "Edge";
                Params.Input[5].Description = "Edge (1, 2, 3 or 4)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = false;

                Params.Input[6].NickName = "V1";
                Params.Input[6].Name = "Value 1 [" + unitAbbreviation + "]";
                Params.Input[6].Description = "Load Value Corner 1";
                Params.Input[6].Access = GH_ParamAccess.item;
                Params.Input[6].Optional = false;

                Params.Input[7].NickName = "V2";
                Params.Input[7].Name = "Value 2 [" + unitAbbreviation + "]";
                Params.Input[7].Description = "Load Value Corner 2";
                Params.Input[7].Access = GH_ParamAccess.item;
                Params.Input[7].Optional = false;
            }
        }
        #endregion
    }
}
