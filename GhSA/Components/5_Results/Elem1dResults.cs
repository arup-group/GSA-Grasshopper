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
using Grasshopper.Kernel.Data;

namespace GhSA.Components
{
    /// <summary>
    /// Component to get Element1D results
    /// </summary>
    public class Elem1DResults : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("79cd1187-2b27-4bee-a77f-4f94c98e73c3");
        public Elem1DResults()
          : base("1D Element Results", "Elem1dResults", "Get 1D Element Results from GSA",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.Elem1dResults;
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
                        _disp = DisplayValue.resXYZ;
                        getresults = true;
                        Mode1Clicked();
                    }
                    
                }
                if (selectedidd == 1)
                {
                    if (dropdowncontents[1] != dropdownforce)
                    {
                        dropdowncontents[1] = dropdownforce;
                        selections[0] = dropdowncontents[0][1];
                        selections[1] = dropdowncontents[1][5];
                        _disp = DisplayValue.YY;
                        getresults = true;
                        Mode2Clicked();
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
            "Force",
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

        readonly List<string> dropdownforce = new List<string>(new string[]
        {
            "Axial Force Fx",
            "Shear Force Fy",
            "Shear Force Fz",
            "Resolved |F|",
            "Torsion Mxx",
            "Moment Myy",
            "Moment Mzz",
            "Resolved |M|",
        });
        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Load Case", "LC", "Load Case (default 1)", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + System.Environment.NewLine +
                "Element list should take the form:" + System.Environment.NewLine +
                " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
                "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
            pManager.AddIntegerParameter("No. Positions", "nP", "Number of results (positions) for each line", GH_ParamAccess.item, 10);
            pManager.AddColourParameter("Colour", "Co", "Optional list of colours to override default colours" +
                System.Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size", GH_ParamAccess.item, 10);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Translation", "U\u0305", "X, Y, Z translation values (" + Units.LengthSmall + ")", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Rotation", "R\u0305", "XX, YY, ZZ rotation values (" + Units.Angle + ")", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Line", "L", "Line with result values", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Result Colour", "Co", "Colours representing the result value at each point", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
            pManager.AddGenericParameter("Values", "LT", "Legend Values (" + Units.LengthSmall + ")", GH_ParamAccess.list);
        }

        #region fields
        // new lists of vectors to output results in:
        DataTree<Vector3d> xyz_out = new DataTree<Vector3d>();
        DataTree<Vector3d> xxyyzz_out = new DataTree<Vector3d>();
        DataTree<Line> segmentlines = new DataTree<Line>();
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
        string elemList = "";
        int positionsCount = 0;

        GsaModel gsaModel;
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Model to work on
            GsaModel in_Model = new GsaModel();
            
            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                #region Inputs
                if (gh_typ.Value is GsaModelGoo)
                {
                    gh_typ.CastTo(ref in_Model);
                    if (gsaModel != null)
                    {
                        if (in_Model.GUID != gsaModel.GUID)
                        {
                            gsaModel = in_Model;
                            getresults = true;
                        }
                    }
                    else
                        gsaModel = in_Model;
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                    return;
                }

                // Get analysis case 
                GH_Integer gh_aCase = new GH_Integer();
                DA.GetData(1, ref gh_aCase);
                GH_Convert.ToInt32(gh_aCase, out int tempanalCase, GH_Conversion.Both);

                // Get element filter list
                GH_String gh_elList = new GH_String();
                DA.GetData(2, ref gh_elList);
                GH_Convert.ToString(gh_elList, out string tempelemList, GH_Conversion.Both);

                // Get number of divisions
                GH_Integer gh_Div = new GH_Integer();
                DA.GetData(3, ref gh_Div);
                GH_Convert.ToInt32(gh_Div, out int temppositionsCount, GH_Conversion.Both);

                // Get colours
                List<Grasshopper.Kernel.Types.GH_Colour> gh_Colours = new List<Grasshopper.Kernel.Types.GH_Colour>();
                List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
                if (DA.GetDataList(4, gh_Colours))
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
                DA.GetData(5, ref gh_Scale);
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

                if (elemList == "" || elemList != tempelemList)
                {
                    elemList = tempelemList;
                    getresults = true;
                }

                if (positionsCount == 0 || positionsCount != temppositionsCount)
                {
                    positionsCount = temppositionsCount;
                    getresults = true;
                }
                #endregion

                #region Create results output
                if (getresults)
                {
                    #region Get results from GSA
                    // ### Get results ###
                    //Get analysis case from model
                    AnalysisCaseResult analysisCaseResult = null;
                    gsaModel.Model.Results().TryGetValue(analCase, out analysisCaseResult);
                    if (analysisCaseResult == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No results exist for Analysis Case " + analCase + " in file");
                        return;
                    }
                    IReadOnlyDictionary<int, Element1DResult> globalResults = analysisCaseResult.Element1DResults(elemList, positionsCount);
                    IReadOnlyDictionary<int, Element> elems = gsaModel.Model.Elements(elemList);
                    IReadOnlyDictionary<int, Node> nodes = gsaModel.Model.Nodes();
                    #endregion


                    // ### Loop through results ###
                    // clear existing result lists
                    xyz_out = new DataTree<Vector3d>();
                    xxyyzz_out = new DataTree<Vector3d>();
                    segmentlines = new DataTree<Line>();

                    List<int> elemID = new List<int>();
                    List<int> parentMember = new List<int>();

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

                    foreach (int key in globalResults.Keys)
                    {
                        // lists for results
                        Element1DResult elementResults;
                        globalResults.TryGetValue(key, out elementResults);
                        List<Double6> values = new List<Double6>();
                        List<Vector3d> xyz = new List<Vector3d>();
                        List<Vector3d> xxyyzz = new List<Vector3d>();

                        // list for element geometry and info
                        Element element = new Element();
                        elems.TryGetValue(key, out element);
                        Node start = new Node();
                        nodes.TryGetValue(element.Topology[0], out start);
                        Node end = new Node();
                        nodes.TryGetValue(element.Topology[1], out end);
                        Line ln = new Line(
                            new Point3d(start.Position.X, start.Position.Y, start.Position.Z),
                            new Point3d(end.Position.X, end.Position.Y, end.Position.Z));
                        elemID.Add(key);
                        parentMember.Add(element.ParentMember.Member);

                        // set the result type dependent on user selection in dropdown
                        switch (_mode)
                        {
                            case (FoldMode.Displacement):
                                values = elementResults.Displacement.ToList();
                                unitfactorxyz = 0.001;
                                unitfactorxxyyzz = 1;
                                break;
                            case (FoldMode.Force):
                                values = elementResults.Force.ToList();
                                unitfactorxyz = 1000;
                                unitfactorxxyyzz = 1000;
                                break;
                        }

                        // prepare the line segments
                        int segments = Math.Max(1, values.Count - 1); // number of segment lines is 1 less than number of points
                        int segment = 0; // counter for segments
                        List<Line> segmentedlines = new List<Line>();

                        // loop through the results
                        foreach (Double6 result in values)
                        {
                            // update max and min values
                            if (result.X / unitfactorxyz > dmax_x)
                                dmax_x = result.X / unitfactorxyz;
                            if (result.Y / unitfactorxyz > dmax_y)
                                dmax_y = result.Y / unitfactorxyz;
                            if (result.Z / unitfactorxyz > dmax_z)
                                dmax_z = result.Z / unitfactorxyz;
                            if (Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2)) / unitfactorxyz > dmax_xyz)
                                dmax_xyz = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2)) / unitfactorxyz;

                            if (result.XX / unitfactorxxyyzz > dmax_xx)
                                dmax_xx = result.XX / unitfactorxxyyzz;
                            if (result.YY / unitfactorxxyyzz > dmax_yy)
                                dmax_yy = result.YY / unitfactorxxyyzz;
                            if (result.ZZ / unitfactorxxyyzz > dmax_zz)
                                dmax_zz = result.ZZ / unitfactorxxyyzz;
                            if (Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2)) / unitfactorxxyyzz > dmax_xxyyzz)
                                dmax_xxyyzz = Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2)) / unitfactorxxyyzz;

                            if (result.X / unitfactorxyz < dmin_x)
                                dmin_x = result.X / unitfactorxyz;
                            if (result.Y / unitfactorxyz < dmin_y)
                                dmin_y = result.Y / unitfactorxyz;
                            if (result.Z / unitfactorxyz < dmin_z)
                                dmin_z = result.Z / unitfactorxyz;
                            if (Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2)) / unitfactorxyz < dmin_xyz)
                                dmin_xyz = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2)) / unitfactorxyz;

                            if (result.XX / unitfactorxxyyzz < dmin_xx)
                                dmin_xx = result.XX / unitfactorxxyyzz;
                            if (result.YY / unitfactorxxyyzz < dmin_yy)
                                dmin_yy = result.YY / unitfactorxxyyzz;
                            if (result.ZZ / unitfactorxxyyzz < dmin_zz)
                                dmin_zz = result.ZZ / unitfactorxxyyzz;
                            if (Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2)) / unitfactorxxyyzz < dmin_xxyyzz)
                                dmin_xxyyzz = Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2)) / unitfactorxxyyzz;

                            // add the values to the vector lists
                            xyz.Add(new Vector3d(result.X / unitfactorxyz, result.Y / unitfactorxyz, result.Z / unitfactorxyz));
                            xxyyzz.Add(new Vector3d(result.XX / unitfactorxxyyzz, result.YY / unitfactorxxyyzz, result.ZZ / unitfactorxxyyzz));

                            // create ResultLines
                            if (segment < segments)
                            {
                                Line segmentline = new Line(
                                    ln.PointAt((double)segment / segments),
                                    ln.PointAt((double)(segment + 1) / segments)
                                    );
                                segment++;
                                segmentedlines.Add(segmentline);
                            }
                        }
                        // add the vector list to the out tree
                        xyz_out.AddRange(xyz, new GH_Path(key));
                        xxyyzz_out.AddRange(xxyyzz, new GH_Path(key));
                        segmentlines.AddRange(segmentedlines, new GH_Path(key));
                    }
                    getresults = false;
                }
                #endregion

                #region Result line values
                // ### Coloured Result Lines ###

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

                // Loop through segmented lines and set result colour into ResultLine format
                DataTree<ResultLine> lines_out = new DataTree<ResultLine>();
                DataTree<System.Drawing.Color> col_out = new DataTree<System.Drawing.Color>();


                foreach (GH_Path path in segmentlines.Paths)
                {
                    List<ResultLine> lns = new List<ResultLine>();
                    List<System.Drawing.Color> col = new List<System.Drawing.Color>();

                    List<Line> segmentedlines = segmentlines.Branch(path);

                    for (int j = 0; j < segmentedlines.Count; j++)
                    {
                        if (!(dmin == 0 & dmax == 0))
                        {
                            Line segmentline = segmentedlines[j];
                            int nextj = j + 1;

                            double t1 = 0;
                            double t2 = 0;

                            // pick the right value to display
                            switch (_disp)
                            {
                                case (DisplayValue.X):
                                    t1 = xyz_out[path, j].X;
                                    t2 = xyz_out[path, j + 1].X;
                                    break;
                                case (DisplayValue.Y):
                                    t1 = xyz_out[path, j].Y;
                                    t2 = xyz_out[path, j + 1].Y;
                                    break;
                                case (DisplayValue.Z):
                                    t1 = xyz_out[path, j].Z;
                                    t2 = xyz_out[path, j + 1].Z;
                                    break;
                                case (DisplayValue.resXYZ):
                                    t1 = Math.Sqrt(Math.Pow(xyz_out[path, j].X, 2) + Math.Pow(xyz_out[path, j].Y, 2) + Math.Pow(xyz_out[path, j].Z, 2));
                                    t2 = Math.Sqrt(Math.Pow(xyz_out[path, j + 1].X, 2) + Math.Pow(xyz_out[path, j + 1].Y, 2) + Math.Pow(xyz_out[path, j + 1].Z, 2));
                                    break;
                                case (DisplayValue.XX):
                                    t1 = xxyyzz_out[path, j].X;
                                    t2 = xxyyzz_out[path, nextj].X;
                                    break;
                                case (DisplayValue.YY):
                                    t1 = xxyyzz_out[path, j].Y;
                                    t2 = xxyyzz_out[path, nextj].Y;
                                    break;
                                case (DisplayValue.ZZ):
                                    t1 = xxyyzz_out[path, j].Z;
                                    t2 = xxyyzz_out[path, nextj].Z;
                                    break;
                                case (DisplayValue.resXXYYZZ):
                                    t1 = Math.Sqrt(Math.Pow(xxyyzz_out[path, j].X, 2) + Math.Pow(xxyyzz_out[path, j].Y, 2) + Math.Pow(xxyyzz_out[path, j].Z, 2));
                                    t2 = Math.Sqrt(Math.Pow(xxyyzz_out[path, nextj].X, 2) + Math.Pow(xxyyzz_out[path, nextj].Y, 2) + Math.Pow(xxyyzz_out[path, nextj].Z, 2));
                                    break;
                            }

                            //normalised value between -1 and 1
                            double tnorm1 = 2 * (t1 - dmin) / (dmax - dmin) - 1;
                            double tnorm2 = 2 * (t2 - dmin) / (dmax - dmin) - 1;

                            // get colour for that normalised value
                        
                            System.Drawing.Color valcol1 = double.IsNaN(tnorm1) ? System.Drawing.Color.Black : gH_Gradient.ColourAt(tnorm1);
                            System.Drawing.Color valcol2 = double.IsNaN(tnorm2) ? System.Drawing.Color.Black : gH_Gradient.ColourAt(tnorm2);

                            // set the size of the line ends for ResultLine class. Size is calculated from 0-base, so not a normalised value between extremes
                            float size1 = (t1 >= 0 && dmax != 0) ?
                                Math.Max(2, (float)(t1 / dmax * scale)) :
                                Math.Max(2, (float)(Math.Abs(t1) / Math.Abs(dmin) * scale));
                            if (double.IsNaN(size1))
                                size1 = 1;
                            float size2 = (t2 >= 0 && dmax != 0) ?
                                Math.Max(2, (float)(t2 / dmax * scale)) :
                                Math.Max(2, (float)(Math.Abs(t2) / Math.Abs(dmin) * scale));
                            if (double.IsNaN(size2))
                                size2 = 1;

                            // add our special resultline to the list of lines
                            lns.Add(new ResultLine(segmentline, t1, t2, valcol1, valcol2, size1, size2));

                            // add the colour to the colours list
                            col.Add(valcol1);
                            if (j == segmentedlines.Count - 1)
                                col.Add(valcol2);
                        }
                    }
                    lines_out.AddRange(lns, path);
                    col_out.AddRange(col, path);
                }
                #endregion 

                #region Legend
                // ### Legend ###
                // loop through number of grip points in gradient to create legend

                //Find Colour and Values for legend output
                List<double> ts = new List<double>();
                List<System.Drawing.Color> cs = new List<System.Drawing.Color>();

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
                DA.SetDataTree(0, xyz_out);
                DA.SetDataTree(1, xxyyzz_out);
                DA.SetDataTree(2, lines_out);
                DA.SetDataTree(3, col_out);
                DA.SetDataList(4, cs);
                DA.SetDataList(5, ts);
            }
        }



        #region menu override
        private enum FoldMode
        {
            Displacement,
            Force
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
            if (_mode == FoldMode.Force)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Force;

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
            if (_mode == FoldMode.Force)
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
                Params.Output[0].NickName = "U\u0305";
                Params.Output[0].Name = "Translation";
                Params.Output[0].Description = "Translation Vector [Ux, Uy, Uz] (" + Units.LengthSmall + ")";

                Params.Output[1].NickName = "R\u0305";
                Params.Output[1].Name = "Rotation";
                Params.Output[1].Description = "Rotation Vector [Rxx, Ryy, Rzz] (" + Units.Angle + ")";

                if ((int)_disp < 4)
                    Params.Output[5].Description = "Legend Values (" + Units.LengthSmall + ")";
                else
                    Params.Output[5].Description = "Legend Values (" + Units.Angle + ")";

            }

            if (_mode == FoldMode.Force)
            {
                Params.Output[0].NickName = "F\u0305";
                Params.Output[0].Name = "Force";
                Params.Output[0].Description = "Force Vector [Nx, Vy, Vz] (" + Units.Force + ")";

                Params.Output[1].NickName = "M\u0305";
                Params.Output[1].Name = "Moment";
                Params.Output[1].Description = "Moment Vector [Mxx, Myy, Mzz] (" + Units.Force + "/" + Units.LengthLarge + ")";

                if ((int)_disp < 4)
                    Params.Output[5].Description = "Legend Values (" + Units.Force + ")";
                else
                    Params.Output[5].Description = "Legend Values (" + Units.Force + "/" + Units.LengthLarge + ")";
            }
        }
        #endregion  
    }
}