using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using GsaAPI;
using GsaGH.Parameters;
using System.Linq;
using Oasys.Units;
using UnitsNet.Units;
using UnitsNet;
using GsaGH.Util.Gsa;
using UnitsNet.GH;
using System.Windows.Forms;
using Rhino.Display;
using System.Drawing;
using Rhino;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to create a new Prop2d
    /// </summary>
    public class ContourNodeResults : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("47053884-2c22-4f2c-b092-8531fa5751e1");
        public ContourNodeResults()
          : base("Node Contour Results", "ContourNode", "Diplays GSA Node Results as Contours",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result0D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(topleveldropdown); // 0 type of result (displacement, force)
                dropdownitems.Add(dropdowndisplacement); // 1 component to display (x, y, z etc)
                dropdownitems.Add(Units.FilteredLengthUnits); // geometry unit
                selecteditems = new List<string>();
                selecteditems.Add(dropdownitems[0][0]);
                selecteditems.Add(dropdownitems[1][3]);
                selecteditems.Add(Units.LengthUnitGeometry.ToString());
                first = false;
            }
            m_attributes = new UI.MultiDropDownSliderComponentUI(this, SetSelected, dropdownitems, selecteditems, slider, SetVal, SetMaxMin, DefScale, MaxValue, MinValue, noDigits, spacerDescriptions);
        }

        double MinValue = 0;
        double MaxValue = 1000;
        double DefScale = 250;
        int noDigits = 0;
        bool slider = true;

        public void SetVal(double value)
        {
            DefScale = value;
        }
        public void SetMaxMin(double max, double min)
        {
            MaxValue = max;
            MinValue = min;
        }

        public void SetSelected(int dropdownlistidd, int selectedidd)
        {
            if (dropdownlistidd == 0) // if change is made to first list
            {
                if (selectedidd == 0)
                {
                    if (dropdownitems[1] != dropdowndisplacement)
                    {
                        dropdownitems[1] = dropdowndisplacement;
                        selecteditems[0] = dropdownitems[0][0];
                        selecteditems[1] = dropdownitems[1][3];
                        Mode1Clicked();
                    }
                    
                }
                if (selectedidd == 1)
                {
                    if (dropdownitems[1] != dropdownreaction)
                    {
                        dropdownitems[1] = dropdownreaction;
                        selecteditems[0] = dropdownitems[0][1];
                        selecteditems[1] = dropdownitems[1][3];
                        Mode2Clicked();
                    }
                        
                }
            }
            else if (dropdownlistidd == 1)
            {
                _disp = (DisplayValue)selectedidd;
                selecteditems[1] = dropdownitems[1][selectedidd];

                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
                Params.OnParametersChanged();
                ExpireSolution(true);
            }
            else // change is made to the unit
            {
                geometryLengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);
            }
        }
        private void UpdateUIFromSelectedItems()
        {
            geometryLengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);

            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        List<List<string>> dropdownitems; // list that holds all dropdown contents
        List<string> selecteditems;
        bool first = true;
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Result Type",
            "Component",
            "Geometry Unit",
            "Deform Shape"
        });
        readonly List<string> topleveldropdown = new List<string>(new string[]
        {
            "Displacement",
            "Reaction"
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
            "Resolved |R|"
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

        private LengthUnit geometryLengthUnit = Units.LengthUnitGeometry;
        Grasshopper.GUI.Gradient.GH_Gradient GH_Gradient;
        string _case = "";
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.item);
            pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + System.Environment.NewLine +
                "Node list should take the form:" + System.Environment.NewLine +
                " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager.AddColourParameter("Colour", "Co", "Optional list of colours to override default colours." +
                System.Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scalar", "x:X", "Scale the result display size", GH_ParamAccess.item, 10);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            IQuantity length = new Length(0, geometryLengthUnit);
            string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Point", "P", "Point with result value", GH_ParamAccess.list);
            pManager.AddGenericParameter("Result Colour", "Co", "Colours representing the result value at each point", GH_ParamAccess.list);
            pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
            pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Result to work on
            GsaResult result = new GsaResult();

            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                #region Inputs
                if (gh_typ.Value is GsaResultGoo)
                {
                    result = ((GsaResultGoo)gh_typ.Value).Value;
                    if (result.Type == GsaResult.ResultType.Combination && result.CombPermutationID < 1)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Combination case contains " 
                            + result.NumPermutations + " - only one permutation can be displayed at a time."+
                            System.Environment.NewLine + "Displaying Permutation 1; please use the 'Select Results' to select other single permutations");
                    }
                    if (result.Type == GsaResult.ResultType.Combination)
                        _case = "Case C" + result.CaseID + " P" + result.CombPermutationID;
                    if (result.Type == GsaResult.ResultType.AnalysisCase)
                        _case = "Case A" + result.CaseID;
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
                    return;
                }

                // Get node filter list
                string nodeList = "All";
                GH_String gh_noList = new GH_String();
                if (DA.GetData(1, ref gh_noList))
                {
                    if (GH_Convert.ToString(gh_noList, out string tempnodeList, GH_Conversion.Both))
                        nodeList = tempnodeList;
                }

                // Get colours
                List<GH_Colour> gh_Colours = new List<GH_Colour>();
                List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
                if (DA.GetDataList(2, gh_Colours))
                {
                    for (int i = 0; i < gh_Colours.Count; i++)
                    {
                        System.Drawing.Color color = new System.Drawing.Color();
                        GH_Convert.ToColor(gh_Colours[i], out color, GH_Conversion.Both);
                        colors.Add(color);
                    }
                }
                GH_Gradient = UI.Colour.Stress_Gradient(colors);

                // Get scalar 
                GH_Number gh_Scale = new GH_Number();
                DA.GetData(3, ref gh_Scale);
                double scale = 1;
                GH_Convert.ToDouble(gh_Scale, out scale, GH_Conversion.Both);
                #endregion

                // get stuff for drawing
                GsaResultsValues res = new GsaResultsValues();
                switch (_mode)
                {
                    case FoldMode.Displacement:
                        res = result.NodeDisplacementValues(nodeList, geometryLengthUnit)[0];
                        break;

                    case FoldMode.Reaction:
                        Tuple<List<GsaResultsValues>, string> resultgetter = result.NodeReactionForceValues(nodeList, Units.ForceUnit, Units.MomentUnit);
                        res = resultgetter.Item1[0];
                        nodeList = resultgetter.Item2;
                        break;
                }
                ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(result.Model.Nodes(nodeList));

                ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
                ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

                Enum xyzunit = geometryLengthUnit;
                Enum xxyyzzunit = AngleUnit.Radian;
                if (_mode == FoldMode.Reaction)
                {
                    xyzunit = Units.ForceUnit;
                    xxyyzzunit = Units.MomentUnit;
                }

                double dmax_x = res.dmax_x.As(xyzunit);
                double dmax_y = res.dmax_y.As(xyzunit);
                double dmax_z = res.dmax_z.As(xyzunit);
                double dmax_xyz = res.dmax_xyz.As(xyzunit);
                double dmin_x = res.dmin_x.As(xyzunit);
                double dmin_y = res.dmin_y.As(xyzunit);
                double dmin_z = res.dmin_z.As(xyzunit);
                double dmin_xyz = res.dmin_xyz.As(xyzunit);
                double dmax_xx = res.dmax_xx.As(xxyyzzunit);
                double dmax_yy = res.dmax_yy.As(xxyyzzunit);
                double dmax_zz = res.dmax_zz.As(xxyyzzunit);
                double dmax_xxyyzz = res.dmax_xxyyzz.As(xxyyzzunit);
                double dmin_xx = res.dmin_xx.As(xxyyzzunit);
                double dmin_yy = res.dmin_yy.As(xxyyzzunit);
                double dmin_zz = res.dmin_zz.As(xxyyzzunit);
                double dmin_xxyyzz = res.dmin_xxyyzz.As(xxyyzzunit);

                #region Result point values
                // ### Coloured Result Points ###

                // Get nodes for point location and restraint check in case of reaction force
                ConcurrentDictionary<int, GsaNodeGoo> gsanodes = Util.Gsa.FromGSA.GetNodeDictionary(nodes, geometryLengthUnit);

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

                List<double> rounded = Util.Gsa.ResultHelper.SmartRounder(dmax, dmin);
                dmax = rounded[0];
                dmin = rounded[1];
                
                // Loop through nodes and set result colour into ResultPoint format
                ConcurrentDictionary<int, ResultPoint> pts = new ConcurrentDictionary<int, ResultPoint>();
                ConcurrentDictionary<int, System.Drawing.Color> col = new ConcurrentDictionary<int, System.Drawing.Color>();

                Parallel.ForEach(gsanodes, node =>
                {
                    if (node.Value.Value != null)
                    {
                        int nodeID = node.Value.Value.ID;
                        if (xyzResults.ContainsKey(nodeID))
                        {
                            if (!(dmin == 0 & dmax == 0))
                            {
                                // create deflection point
                                Point3d def = new Point3d(node.Value.Value.Point);

                                double t = 0;
                                switch (_mode)
                                {
                                    case FoldMode.Displacement:
                                        Vector3d translation = new Vector3d(0, 0, 0);
                                        // pick the right value to display
                                        switch (_disp)
                                        {
                                            case (DisplayValue.X):
                                                t = xyzResults[nodeID][0].X.As(geometryLengthUnit);
                                                translation.X = t * DefScale;
                                                break;
                                            case (DisplayValue.Y):
                                                t = xyzResults[nodeID][0].Y.As(geometryLengthUnit);
                                                translation.Y = t * DefScale;
                                                break;
                                            case (DisplayValue.Z):
                                                t = xyzResults[nodeID][0].Z.As(geometryLengthUnit);
                                                translation.Z = t * DefScale;
                                                break;
                                            case (DisplayValue.resXYZ):
                                                t = xyzResults[nodeID][0].XYZ.As(geometryLengthUnit);
                                                translation.X = xyzResults[nodeID][0].X.As(geometryLengthUnit) * DefScale;
                                                translation.Y = xyzResults[nodeID][0].Y.As(geometryLengthUnit) * DefScale;
                                                translation.Z = xyzResults[nodeID][0].Z.As(geometryLengthUnit) * DefScale;
                                                break;
                                            case (DisplayValue.XX):
                                                t = xxyyzzResults[nodeID][0].X.As(geometryLengthUnit);
                                                break;
                                            case (DisplayValue.YY):
                                                t = xxyyzzResults[nodeID][0].Y.As(geometryLengthUnit);
                                                break;
                                            case (DisplayValue.ZZ):
                                                t = xxyyzzResults[nodeID][0].Z.As(geometryLengthUnit);
                                                break;
                                            case (DisplayValue.resXXYYZZ):
                                                t = xxyyzzResults[nodeID][0].XYZ.As(geometryLengthUnit);
                                                break;
                                        }
                                        def.Transform(Transform.Translation(translation));
                                        break;
                                    case FoldMode.Reaction:
                                        // pick the right value to display
                                        switch (_disp)
                                        {
                                            case (DisplayValue.X):
                                                t = xyzResults[nodeID][0].X.As(Units.ForceUnit);
                                                break;
                                            case (DisplayValue.Y):
                                                t = xyzResults[nodeID][0].Y.As(Units.ForceUnit);
                                                break;
                                            case (DisplayValue.Z):
                                                t = xyzResults[nodeID][0].Z.As(Units.ForceUnit);
                                                break;
                                            case (DisplayValue.resXYZ):
                                                t = xyzResults[nodeID][0].XYZ.As(Units.ForceUnit);
                                                break;
                                            case (DisplayValue.XX):
                                                t = xxyyzzResults[nodeID][0].X.As(Units.MomentUnit);
                                                break;
                                            case (DisplayValue.YY):
                                                t = xxyyzzResults[nodeID][0].Y.As(Units.MomentUnit);
                                                break;
                                            case (DisplayValue.ZZ):
                                                t = xxyyzzResults[nodeID][0].Z.As(Units.MomentUnit);
                                                break;
                                            case (DisplayValue.resXXYYZZ):
                                                t = xxyyzzResults[nodeID][0].XYZ.As(Units.MomentUnit);
                                                break;
                                        }
                                        break;
                                }

                                //normalised value between -1 and 1
                                double tnorm = 2 * (t - dmin) / (dmax - dmin) - 1;

                                // get colour for that normalised value
                                System.Drawing.Color valcol = GH_Gradient.ColourAt(tnorm);

                                // set the size of the point for ResultPoint class. Size is calculated from 0-base, so not a normalised value between extremes
                                float size = (t >= 0 && dmax != 0) ?
                                    Math.Max(2, (float)(t / dmax * scale)) :
                                    Math.Max(2, (float)(Math.Abs(t) / Math.Abs(dmin) * scale));

                                // add our special resultpoint to the list of points
                                pts[nodeID] = new ResultPoint(def, t, valcol, size);

                                // add the colour to the colours list
                                col[nodeID] = valcol;
                            }
                        }
                    }
                });
                #endregion 

                #region Legend
                // ### Legend ###
                // loop through number of grip points in gradient to create legend
                int gripheight = legend.Height / GH_Gradient.GripCount;
                legendValues = new List<string>();
                legendValuesPosY = new List<int>();
                for (int i = 0; i < GH_Gradient.GripCount; i++)
                {
                    double t = dmin + (dmax - dmin) / ((double)GH_Gradient.GripCount - 1) * (double)i;
                    double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
                    scl = Math.Max(scl, 1);
                    t = scl * Math.Round(t / scl, 3);
                    ts.Add(t);

                    System.Drawing.Color gradientcolour = GH_Gradient.ColourAt(2 * (double)i / ((double)GH_Gradient.GripCount - 1) - 1);
                    cs.Add(gradientcolour);

                    // create legend for viewport
                    int starty = i * gripheight;
                    int endy = starty + gripheight;
                    for (int y = starty; y < endy; y++ )
                    {
                        for (int x = 0; x < legend.Width; x++)
                        {
                            legend.SetPixel(x, legend.Height - y - 1, gradientcolour);
                        }
                        
                    }
                    if (_mode == FoldMode.Displacement)
                    {
                        if ((int)_disp < 4)
                            legendValues.Add(new Length(t, geometryLengthUnit).ToString());
                        else
                            legendValues.Add(new Angle(t, AngleUnit.Radian).ToString());
                    }
                    if (_mode == FoldMode.Reaction)
                    {
                        if ((int)_disp < 4)
                            legendValues.Add(new Force(t, Units.ForceUnit).ToString());
                        else
                            legendValues.Add(new Moment(t, Units.MomentUnit).ToString());
                    }
                    legendValuesPosY.Add(legend.Height - starty + gripheight/2 - 2);
                }
                #endregion


                // set outputs
                DA.SetDataList(0, pts.OrderBy(x => x.Key).Select(y => y.Value).ToList());
                DA.SetDataList(1, col.OrderBy(x => x.Key).Select(y => y.Value).ToList());
                DA.SetDataList(2, cs);
                DA.SetDataList(3, ts);
            }
        }

        #region menu override
        private enum FoldMode
        {
            Displacement,
            Reaction
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

        private void ReDrawComponent()
        {
            System.Drawing.PointF pivot = new System.Drawing.PointF(this.Attributes.Pivot.X, this.Attributes.Pivot.Y);
            this.CreateAttributes();
            this.Attributes.Pivot = pivot;
            this.Attributes.ExpireLayout();
            this.Attributes.PerformLayout();
        }
        private void Mode1Clicked()
        {
            if (_mode == FoldMode.Displacement)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Displacement;

            slider = true;
            DefScale = 100;

            ReDrawComponent();

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
            slider = false;
            DefScale = 0;

            ReDrawComponent();

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Show Legend", ShowLegend, true, showLegend);
        }
        bool showLegend = true;
        private void ShowLegend(object sender, EventArgs e)
        {
            showLegend = !showLegend;
            this.ExpirePreview(true);
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
            Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

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

            if (_mode == FoldMode.Displacement)
            {

                if ((int)_disp < 4)
                {
                    IQuantity length = new Length(0, geometryLengthUnit);
                    string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
                    Params.Output[3].Name = "Values [" + lengthunitAbbreviation + "]";
                }
                else
                    Params.Output[3].Name = "Values [rad]";
            }

            if (_mode == FoldMode.Reaction)
            {
                if ((int)_disp < 4)
                {
                    IQuantity force = new Force(0, Units.ForceUnit);
                    string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
                    Params.Output[3].Name = "Values [" + forceunitAbbreviation + "]";
                }
                else
                {
                    string momentunitAbbreviation = Oasys.Units.Moment.GetAbbreviation(Units.MomentUnit);
                    Params.Output[3].Name = "Values [" + momentunitAbbreviation + "]";
                }
            }
        }
        #endregion

        #region draw legend

        Bitmap legend = new Bitmap(15, 120);
        List<string> legendValues;
        List<int> legendValuesPosY;
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);
            if (showLegend)
            {
                args.Display.DrawBitmap(new DisplayBitmap(legend), args.Viewport.Bounds.Right - 100, 20);
                for (int i = 0; i < legendValues.Count; i++)
                    args.Display.Draw2dText(legendValues[i], Color.Black, new Point2d(args.Viewport.Bounds.Right - 75, legendValuesPosY[i]), false);
                if (_mode == FoldMode.Displacement)
                {
                    args.Display.Draw2dText("Displacement", Color.Black, new Point2d(args.Viewport.Bounds.Right - 100, 7), false);
                }
                if (_mode == FoldMode.Reaction)
                {
                    args.Display.Draw2dText("Reaction Forces", Color.Black, new Point2d(args.Viewport.Bounds.Right - 100, 10), false);
                }
                args.Display.Draw2dText(_case, Color.Black, new Point2d(args.Viewport.Bounds.Right - 100, 145), false);
            }
        }
            #endregion
    }
}