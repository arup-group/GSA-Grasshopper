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
using System.Linq;

namespace GhSA.Components
{
    /// <summary>
    /// Component to create a new Prop2d
    /// </summary>
    public class NodeResults : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("7a8250b8-781e-4aeb-a05b-1b1fce9fbe3b");
        public NodeResults()
          : base("Node Results", "NodeResult", "Get GSA Node Results",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.NodeResults;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdowncontents = new List<List<string>>();
                dropdowncontents.Add(dropdownitems);
                dropdowncontents.Add(dropdowndisplacement);
                selections = new List<string>();
                selections.Add(dropdowncontents[0][0]);
                selections.Add(dropdowncontents[1][3]);
                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdowncontents, selections, spacertext);
        }

        public void SetSelected(int dropdownlistidd, int selectedidd)
        {
            if (dropdownlistidd == 0) // if change is made to first list
            {
                if (selectedidd == 0)
                {
                    if (dropdowncontents[1] != dropdowndisplacement)
                    {
                        dropdowncontents[1] = dropdowndisplacement;
                        selections[0] = dropdowncontents[0][0];
                        selections[1] = dropdowncontents[1][3];
                        getresults = true;
                        Mode1Clicked();
                    }
                    
                }
                if (selectedidd == 1)
                {
                    if (dropdowncontents[1] != dropdownreaction)
                    {
                        dropdowncontents[1] = dropdownreaction;
                        selections[0] = dropdowncontents[0][1];
                        selections[1] = dropdowncontents[1][3];
                        getresults = true;
                        Mode2Clicked();
                    }
                        
                }
                if (selectedidd == 2)
                {
                    if (dropdowncontents[1] != dropdownforce)
                    {
                        dropdowncontents[1] = dropdownforce;
                        selections[0] = dropdowncontents[0][2];
                        selections[1] = dropdowncontents[1][3];
                        getresults = true;
                        Mode3Clicked();
                    }
                }
            }
            else
            {
                if (selectedidd == 0)
                    _disp = DisplayValue.X;
                if (selectedidd == 1)
                    _disp = DisplayValue.Y;
                if (selectedidd == 2)
                    _disp = DisplayValue.Z;
                if (selectedidd == 3)
                    _disp = DisplayValue.resXYZ;
                if (selectedidd == 4)
                    _disp = DisplayValue.XX;
                if (selectedidd == 5)
                    _disp = DisplayValue.YY;
                if (selectedidd == 6)
                    _disp = DisplayValue.ZZ;
                if (selectedidd == 7)
                    _disp = DisplayValue.resXXYYZZ;
                selections[1] = dropdowncontents[1][selectedidd];
                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
                Params.OnParametersChanged();
                ExpireSolution(true);
            }
        }
        #endregion

        #region Input and output
        List<List<string>> dropdowncontents; // list that holds all dropdown contents
        List<string> selections;
        bool first = true;
        readonly List<string> spacertext = new List<string>(new string[]
        {
            "Result Type",
            "Display Result",
        });
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Displacement",
            "Reaction",
            "Spring Force"
        });

        readonly List<string> dropdowndisplacement = new List<string>(new string[]
        {
            "Translation Ux",
            "Translation Uy",
            "Translation Uz",
            "Resolved |U|",
            "Rotation Rxx",
            "Rotation Ryy",
            "Rotation Rzz",
            "Resolved |R|",
        });

        readonly List<string> dropdownreaction = new List<string>(new string[]
        {
            "Reaction Fx",
            "Reaction Fy",
            "Reaction Fz",
            "Resolved |F|",
            "Reaction Mxx",
            "Reaction Myy",
            "Reaction Mzz",
            "Resolved |M|",
        });

        readonly List<string> dropdownforce = new List<string>(new string[]
        {
            "Force Fx",
            "Force Fy",
            "Force Fz",
            "Resolved |F|",
            "Moment Mxx",
            "Moment Myy",
            "Moment Mzz",
            "Resolved |M|",
        });

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + System.Environment.NewLine +
                "Node list should take the form:" + System.Environment.NewLine +
                " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager.AddColourParameter("Colour", "Col", "Optional list of colours to override default colours." +
                System.Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scalar", "Scal", "Scale the result display size", GH_ParamAccess.item, 10);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Translation", "U", "X, Y, Z translation values (" + Util.GsaUnit.LengthSmall + ")", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rotation", "R", "XX, YY, ZZ rotation values (" + Util.GsaUnit.Angle + ")", GH_ParamAccess.list);
            pManager.AddGenericParameter("Point", "Pt", "Position", GH_ParamAccess.list);
            pManager.AddGenericParameter("Result Colour", "Col", "Colours representing the result value at each point", GH_ParamAccess.list);
            pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
            pManager.AddGenericParameter("Values", "LT", "Legend Values (" + Util.GsaUnit.LengthSmall + ")", GH_ParamAccess.list);
        }

        #region fields
        List<Vector3d> xyz = new List<Vector3d>();
        List<Vector3d> xxyyzz = new List<Vector3d>();

        double dmax_x;
        double dmax_y;
        double dmax_z;
        double dmax_xx;
        double dmax_yy;
        double dmax_zz;
        double dmax_xyz;
        double dmax_xxyyzz;
        double dmin_x;
        double dmin_y;
        double dmin_z;
        double dmin_xx;
        double dmin_yy;
        double dmin_zz;
        double dmin_xyz;
        double dmin_xxyyzz;
        bool getresults = true;

        int analCase = 0;
        string nodeList = "";
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Model to work on
            GsaModel gsaModel = new GsaModel();
            
            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                #region Inputs
                if (gh_typ.Value is GsaModelGoo)
                    gh_typ.CastTo(ref gsaModel);
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                    return;
                }

                // Get analysis case 
                GH_Integer gh_aCase = new GH_Integer();
                DA.GetData(1, ref gh_aCase);
                GH_Convert.ToInt32(gh_aCase, out int tempanalCase, GH_Conversion.Both);

                // Get node filter list
                GH_String gh_noList = new GH_String();
                DA.GetData(2, ref gh_noList);
                GH_Convert.ToString(gh_noList, out string tempnodeList, GH_Conversion.Both);

                // Get colours
                List<Grasshopper.Kernel.Types.GH_Colour> gh_Colours = new List<Grasshopper.Kernel.Types.GH_Colour>();
                List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
                if (DA.GetDataList(3, gh_Colours))
                {
                    for (int i = 0; i < gh_Colours.Count; i++)
                    {
                        System.Drawing.Color color = new System.Drawing.Color();
                        GH_Convert.ToColor(gh_Colours[i], out color, GH_Conversion.Both);
                        colors.Add(color);
                    }
                }
                Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = UI.Colour.Stress_Gradient(colors);

                // Get scalar 
                GH_Number gh_Scale = new GH_Number();
                DA.GetData(4, ref gh_Scale);
                double scale = 1;
                GH_Convert.ToDouble(gh_Scale, out scale, GH_Conversion.Both);
                #endregion

                #region get results?
                // check if we must get results or just update display
                if (analCase == 0 || analCase != tempanalCase)
                {
                    analCase = tempanalCase;
                    getresults = true;
                }

                if (nodeList == "" || nodeList != tempnodeList)
                {
                    nodeList = tempnodeList;
                    getresults = true;
                }
                #endregion

                if (getresults)
                {
                    #region Get results from GSA
                    // ### Get results ###
                    //Get analysis case from model
                    AnalysisCaseResult analysisCaseResult = null;
                    gsaModel.Model.Results().TryGetValue(analCase, out analysisCaseResult);
                    if (analysisCaseResult == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Analysis Case " + analCase + " does not exist in file,");
                        return;
                    }
                    IReadOnlyDictionary<int, NodeResult> results = analysisCaseResult.NodeResults(nodeList);
                    IReadOnlyDictionary<int, Node> nodes = gsaModel.Model.Nodes(nodeList);
                    #endregion

                    #region Create results output
                    // ### Loop through results ###
                    // clear any existing lists of vectors to output results in:
                    xyz = new List<Vector3d>();
                    xxyyzz = new List<Vector3d>();

                    // maximum and minimum result values for colouring later
                    dmax_x = 0;
                    dmax_y = 0;
                    dmax_z = 0;
                    dmax_xx = 0;
                    dmax_yy = 0;
                    dmax_zz = 0;
                    dmax_xyz = 0;
                    dmax_xxyyzz = 0;
                    dmin_x = 0;
                    dmin_y = 0;
                    dmin_z = 0;
                    dmin_xx = 0;
                    dmin_yy = 0;
                    dmin_zz = 0;
                    dmin_xyz = 0;
                    dmin_xxyyzz = 0;

                    double unitfactorxyz = 1;
                    double unitfactorxxyyzz = 1;

                    // if reaction type, then we reuse the nodeList to filter support nodes from the rest
                    if (_mode == FoldMode.Reaction)
                        nodeList = "";

                    foreach (var key in results.Keys)
                    {
                        NodeResult result;
                        Double6 values = null;
                        double d = 0;

                        if (_mode == FoldMode.Reaction)
                        {
                            bool isSupport = false;
                            Node node = new Node();
                            nodes.TryGetValue(key, out node);
                            NodalRestraint rest = node.Restraint;
                            if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ)
                                isSupport = true;
                            if (!isSupport)
                                continue;
                            else
                            {
                                if (nodeList == "")
                                    nodeList = key.ToString();
                                else
                                    nodeList += " " + key;
                            }
                        }

                        results.TryGetValue(key, out result);
                        switch (_mode)
                        {
                            case (FoldMode.Displacement):
                                values = result.Displacement;
                                unitfactorxyz = 0.001;
                                unitfactorxxyyzz = 1;
                                break;
                            case (FoldMode.Reaction):
                                values = result.Reaction;
                                unitfactorxyz = 1000;
                                unitfactorxxyyzz = 1000;
                                break;
                            case (FoldMode.SpringForce):
                                values = result.SpringForce;
                                unitfactorxyz = 1000;
                                unitfactorxxyyzz = 1000;
                                break;
                            case (FoldMode.Constraint):
                                values = result.Constraint;
                                break;
                        }
                        

                        // update max and min values
                        if (values.X / unitfactorxyz > dmax_x)
                            dmax_x = values.X / unitfactorxyz;
                        if (values.Y / unitfactorxyz > dmax_y)
                            dmax_y = values.Y / unitfactorxyz;
                        if (values.Z / unitfactorxyz > dmax_z)
                            dmax_z = values.Z / unitfactorxyz;
                        if (Math.Sqrt(Math.Pow(values.X, 2) + Math.Pow(values.Y, 2) + Math.Pow(values.Z, 2)) / unitfactorxyz > dmax_xyz)
                            dmax_xyz = Math.Sqrt(Math.Pow(values.X, 2) + Math.Pow(values.Y, 2) + Math.Pow(values.Z, 2)) / unitfactorxyz;

                        if (values.XX / unitfactorxxyyzz > dmax_xx)
                            dmax_xx = values.XX / unitfactorxxyyzz;
                        if (values.YY / unitfactorxxyyzz > dmax_yy)
                            dmax_yy = values.YY / unitfactorxxyyzz;
                        if (values.ZZ / unitfactorxxyyzz > dmax_zz)
                            dmax_zz = values.ZZ / unitfactorxxyyzz;
                        if (Math.Sqrt(Math.Pow(values.XX, 2) + Math.Pow(values.YY, 2) + Math.Pow(values.ZZ, 2)) / unitfactorxxyyzz > dmax_xxyyzz)
                            dmax_xxyyzz = Math.Sqrt(Math.Pow(values.XX, 2) + Math.Pow(values.YY, 2) + Math.Pow(values.ZZ, 2)) / unitfactorxxyyzz;

                        if (values.X / unitfactorxyz < dmin_x)
                            dmin_x = values.X / unitfactorxyz;
                        if (values.Y / unitfactorxyz < dmin_y)
                            dmin_y = values.Y / unitfactorxyz;
                        if (values.Z / unitfactorxyz < dmin_z)
                            dmin_z = values.Z / unitfactorxyz;
                        if (Math.Sqrt(Math.Pow(values.X, 2) + Math.Pow(values.Y, 2) + Math.Pow(values.Z, 2)) / unitfactorxyz < dmin_xyz)
                            dmin_xyz = Math.Sqrt(Math.Pow(values.X, 2) + Math.Pow(values.Y, 2) + Math.Pow(values.Z, 2)) / unitfactorxyz;

                        if (values.XX / unitfactorxxyyzz < dmin_xx)
                            dmin_xx = values.XX / unitfactorxxyyzz;
                        if (values.YY / unitfactorxxyyzz < dmin_yy)
                            dmin_yy = values.YY / unitfactorxxyyzz;
                        if (values.ZZ / unitfactorxxyyzz < dmin_zz)
                            dmin_zz = values.ZZ / unitfactorxxyyzz;
                        if (Math.Sqrt(Math.Pow(values.XX, 2) + Math.Pow(values.YY, 2) + Math.Pow(values.ZZ, 2)) / unitfactorxxyyzz < dmin_xxyyzz)
                            dmin_xxyyzz = Math.Sqrt(Math.Pow(values.XX, 2) + Math.Pow(values.YY, 2) + Math.Pow(values.ZZ, 2)) / unitfactorxxyyzz;

                        // add the values to the vector lists
                        xyz.Add(new Vector3d(values.X / unitfactorxyz, values.Y / unitfactorxyz, values.Z / unitfactorxyz));
                        xxyyzz.Add(new Vector3d(values.XX / unitfactorxxyyzz, values.YY / unitfactorxxyyzz, values.ZZ / unitfactorxxyyzz));
                    }
                    #endregion
                    getresults = false;
                }


                #region Result point values
                // ### Coloured Result Points ###

                // Get nodes for point location and restraint check in case of reaction force
                List<GsaNode> gsanodes = Util.Gsa.GsaImport.GsaGetPoint(gsaModel.Model, nodeList, true);

                //Find Colour and Values for legend output
                
                List<double> ts = new List<double>();
                List<System.Drawing.Color> cs = new List<System.Drawing.Color>();

                // round max and min to reasonable numbers
                double dmax = 0;
                double dmin = 0;
                switch (_disp)
                {
                    case (DisplayValue.X):
                        dmax = dmax_x;
                        dmin = dmin_x;
                        break;
                    case (DisplayValue.Y):
                        dmax = dmax_y;
                        dmin = dmin_y;
                        break;
                    case (DisplayValue.Z):
                        dmax = dmax_z;
                        dmin = dmin_z;
                        break;
                    case (DisplayValue.resXYZ):
                        dmax = dmax_xyz;
                        dmin = dmin_xyz;
                        break;
                    case (DisplayValue.XX):
                        dmax = dmax_xx;
                        dmin = dmin_xx;
                        break;
                    case (DisplayValue.YY):
                        dmax = dmax_yy;
                        dmin = dmin_yy;
                        break;
                    case (DisplayValue.ZZ):
                        dmax = dmax_zz;
                        dmin = dmin_zz;
                        break;
                    case (DisplayValue.resXXYYZZ):
                        dmax = dmax_xxyyzz;
                        dmin = dmin_xxyyzz;
                        break;
                }

                List<double> rounded = Util.Gsa.ResultHelper.SmartRounder(new List<double>(new double[] { dmax, dmin }));
                dmax = rounded[0];
                dmin = rounded[1];
                
                // Loop through nodes and set result colour into ResultPoint format
                List<ResultPoint> pts = new List<ResultPoint>();
                List<System.Drawing.Color> col = new List<System.Drawing.Color>();
                
                for (int i = 0; i < gsanodes.Count; i++)
                {
                    if (gsanodes[i] != null)
                    {
                        if (!(dmin == 0 & dmax == 0))
                        {
                            double t = 0;
                            // pick the right value to display
                            switch (_disp)
                            {
                                case (DisplayValue.X):
                                    t = xyz[i].X;
                                    break;
                                case (DisplayValue.Y):
                                    t = xyz[i].Y;
                                    break;
                                case (DisplayValue.Z):
                                    t = xyz[i].Z;
                                    break;
                                case (DisplayValue.resXYZ):
                                    t = Math.Sqrt(Math.Pow(xyz[i].X, 2) + Math.Pow(xyz[i].Y, 2) + Math.Pow(xyz[i].Z, 2));
                                    break;
                                case (DisplayValue.XX):
                                    t = xxyyzz[i].X;
                                    break;
                                case (DisplayValue.YY):
                                    t = xxyyzz[i].Y;
                                    break;
                                case (DisplayValue.ZZ):
                                    t = xxyyzz[i].Z;
                                    break;
                                case (DisplayValue.resXXYYZZ):
                                    t = Math.Sqrt(Math.Pow(xxyyzz[i].X, 2) + Math.Pow(xxyyzz[i].Y, 2) + Math.Pow(xxyyzz[i].Z, 2));
                                    break;
                            }

                            //normalised value between -1 and 1
                            double tnorm = 2 * (t - dmin) / (dmax - dmin) - 1;

                            // get colour for that normalised value
                            System.Drawing.Color valcol = gH_Gradient.ColourAt(tnorm);

                            // set the size of the point for ResultPoint class. Size is calculated from 0-base, so not a normalised value between extremes
                            float size = (t >= 0 && dmax != 0) ? 
                                Math.Max(2, (float)(t / dmax * scale)) : 
                                Math.Max(2, (float)(Math.Abs(t) / Math.Abs(dmin) * scale));

                            // add our special resultpoint to the list of points
                            pts.Add(new ResultPoint(gsanodes[i].Point, t, valcol, size));

                            // add the colour to the colours list
                            col.Add(valcol);
                        }
                    }
                }
                #endregion 

                #region Legend
                // ### Legend ###
                // loop through number of grip points in gradient to create legend
                for (int i = 0; i < gH_Gradient.GripCount; i++)
                {
                    double t = dmin + (dmax - dmin) / ((double)gH_Gradient.GripCount - 1) * (double)i;
                    double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
                    scl = Math.Max(scl, 1);
                    t = scl * Math.Round(t / scl, 3);
                    ts.Add(t);

                    System.Drawing.Color gradientcolour = gH_Gradient.ColourAt(2 * (double)i / ((double)gH_Gradient.GripCount - 1) - 1);
                    cs.Add(gradientcolour);
                }
                #endregion

                // set outputs
                DA.SetDataList(0, xyz);
                DA.SetDataList(1, xxyyzz);
                DA.SetDataList(2, pts);
                DA.SetDataList(3, col);
                DA.SetDataList(4, cs);
                DA.SetDataList(5, ts);
            }
        }

        #region menu override
        private enum FoldMode
        {
            Displacement,
            Reaction,
            SpringForce,
            Constraint
        }
        private FoldMode _mode = FoldMode.Displacement;

        private enum DisplayValue
        {
            X,
            Y,
            Z,
            resXYZ,
            XX,
            YY,
            ZZ,
            resXXYYZZ
        }
        private DisplayValue _disp = DisplayValue.resXYZ;

        private void Mode1Clicked()
        {
            if (_mode == FoldMode.Displacement)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Displacement;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Reaction)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Reaction;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.SpringForce)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.SpringForce;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Constraint)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Constraint;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        #endregion
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Mode", (int)_mode);
            writer.SetInt32("Display", (int)_disp);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            _disp = (DisplayValue)reader.GetInt32("Display");

            dropdowncontents = new List<List<string>>();
            dropdowncontents.Add(dropdownitems);
            if (_mode == FoldMode.Displacement)
                dropdowncontents.Add(dropdowndisplacement);
            if (_mode == FoldMode.Reaction)
                dropdowncontents.Add(dropdownreaction);
            if (_mode == FoldMode.SpringForce)
                dropdowncontents.Add(dropdownforce);

            selections = new List<string>();
            selections.Add(dropdowncontents[0][(int)_mode]);
            selections.Add(dropdowncontents[1][(int)_disp]);
            first = false;

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
            
            if (_mode == FoldMode.Displacement)
            {
                Params.Output[0].NickName = "U";
                Params.Output[0].Name = "Translation";
                Params.Output[0].Description = "Translation Vector [Ux, Uy, Uz] (" + Util.GsaUnit.LengthSmall + ")";

                Params.Output[1].NickName = "R";
                Params.Output[1].Name = "Rotation";
                Params.Output[1].Description = "Rotation Vector [Rxx, Ryy, Rzz] (" + Util.GsaUnit.Angle + ")";

                if ((int)_disp < 4)
                    Params.Output[5].Description = "Legend Values (" + Util.GsaUnit.LengthSmall + ")";
                else
                    Params.Output[5].Description = "Legend Values (" + Util.GsaUnit.Angle + ")";

            }

            if (_mode == FoldMode.Reaction | _mode == FoldMode.SpringForce)
            {
                Params.Output[0].NickName = "F";
                Params.Output[0].Name = "Force";
                Params.Output[0].Description = "Force Vector [Nx, Vy, Vz] (" + Util.GsaUnit.Force + ")";

                Params.Output[1].NickName = "M";
                Params.Output[1].Name = "Moment";
                Params.Output[1].Description = "Moment Vector [Mxx, Myy, Mzz] (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";

                if ((int)_disp < 4)
                    Params.Output[5].Description = "Legend Values (" + Util.GsaUnit.Force + ")";
                else
                    Params.Output[5].Description = "Legend Values (" + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")";
            }
        }
        #endregion  
    }
}