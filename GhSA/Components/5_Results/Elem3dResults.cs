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
    /// Component to get Element2d results
    /// </summary>
    public class Elem3dResults : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("aad5e74b-fbe5-4f6e-9673-28fbc6c1f635");
        public Elem3dResults()
          : base("3D Element Results", "Elem3dResults", "Get 3D Element Results from GSA",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.Elem3dResults;
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
            m_attributes = new UI.MultiDropDownSliderComponentUI(this, SetSelected, dropdowncontents, selections, slider, SetVal, SetMaxMin, Value, MaxValue, MinValue, noDigits, spacertext);
        }

        double MinValue = 0;
        double MaxValue = 100;
        double Value = 50;
        int noDigits = 0;
        bool slider = true;
        public void SetVal(double value)
        {
            Value = value;
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
                    if (dropdowncontents[1] != dropdowndisplacement)
                    {
                        dropdowncontents[1] = dropdowndisplacement;
                        selections[0] = dropdowncontents[0][0];
                        selections[1] = dropdowncontents[1][3];

                        _disp = (DisplayValue)3;
                        getresults = true;
                        Mode1Clicked();
                    }
                }
                if (selectedidd == 1)
                {
                    if (dropdowncontents[1] != dropdownstress)
                    {
                        dropdowncontents[1] = dropdownstress;
                        selections[0] = dropdowncontents[0][1];
                        selections[1] = dropdowncontents[1][0];

                        _disp = (DisplayValue)0;
                        getresults = true;
                        Mode2Clicked();
                    }
                }
            }

            if (dropdownlistidd == 1) // if change is made to second list
            {
                bool redraw = false;
                selections[1] = dropdowncontents[1][selectedidd];
                if (_mode == FoldMode.Displacement)
                {
                    if ((int)_disp > 3 & selectedidd < 4)
                    {
                        redraw = true;
                        slider = true;
                    }
                    if ((int)_disp < 4 & selectedidd > 3)
                    {
                        redraw = true;
                        slider = false;

                    }
                }

                _disp = (DisplayValue)selectedidd;
                if (dropdowncontents[1] != dropdowndisplacement)
                    if (selectedidd > 2)
                        _disp = (DisplayValue)selectedidd + 1;

                if (redraw)
                    ReDrawComponent();

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
            "Deform Shape"
        });
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Displacement",
            "Stress"
        });

        readonly List<string> dropdowndisplacement = new List<string>(new string[]
        {
            "Translation Ux",
            "Translation Uy",
            "Translation Uz",
            "Resolved |U|",
        });

        readonly List<string> dropdownstress = new List<string>(new string[]
        {
            "Stress xx",
            "Stress yy",
            "Stress zz",
            "Stress xy",
            "Stress yz",
            "Stress zx",
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
            pManager.AddColourParameter("Colour", "Co", "Optional list of colours to override default colours" +
                System.Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Translation", "U\u0305", "X, Y, Z translation values (" + Units.LengthSmall + ")", GH_ParamAccess.tree);
            //pManager.AddVectorParameter("Stress", "σ\u0305", "XX, YY, ZZ stress values(" + Units.Stress + ")", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Mesh", "M", "Mesh with result values", GH_ParamAccess.item);
            pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
            pManager.AddGenericParameter("Values", "LT", "Legend Values (" + Units.LengthSmall + ")", GH_ParamAccess.list);
        }

        #region fields
        // new lists of vectors to output results in:
        DataTree<Vector3d> xyz_out = new DataTree<Vector3d>();
        DataTree<Vector3d> xxyyzz_out = new DataTree<Vector3d>();
        List<ResultMesh> resultMeshes = new List<ResultMesh>();
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
        List<int> keys;

        int analCase = 0;
        string elemList = "";
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
                    IReadOnlyDictionary<int, Element3DResult> globalResults = analysisCaseResult.Element3DResults(elemList);
                    
                    #endregion

                    // ### Loop through results ###
                    // clear existing result lists
                    xyz_out = new DataTree<Vector3d>();
                    xxyyzz_out = new DataTree<Vector3d>();

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
                    keys = new List<int>();

                    double unitfactorxyz = 1;
                    double unitfactorxxyyzz = 1;

                    foreach (int key in globalResults.Keys)
                    {
                        keys.Add(key);
                        
                        // lists for results
                        Element3DResult elementResults;
                        if (globalResults.TryGetValue(key, out elementResults))
                        {
                            List<Vector3d> xyz = new List<Vector3d>();
                            List<Vector3d> xxyyzz = new List<Vector3d>();

                            switch (_mode)
                            {
                                case FoldMode.Displacement:
                                    unitfactorxyz = 0.001;
                                    List<Double3> trans_vals = elementResults.Displacement.ToList();
                                    foreach (Double3 val in trans_vals)
                                    {
                                        Vector3d valxyz = new Vector3d
                                        {
                                            X = val.X / unitfactorxyz,
                                            Y = val.Y / unitfactorxyz,
                                            Z = val.Z / unitfactorxyz
                                        };
                                        xyz.Add(valxyz);
                                    }
                                    break;

                                case FoldMode.Stress:
                                    unitfactorxxyyzz = 1000000;

                                    List<Tensor3> stress_vals = elementResults.Stress.ToList();
                                    foreach (Tensor3 val in stress_vals)
                                    {
                                        Vector3d valxxyyzz = new Vector3d
                                        {
                                            X = val.XX / unitfactorxxyyzz,
                                            Y = val.YY / unitfactorxxyyzz,
                                            Z = val.ZZ / unitfactorxxyyzz
                                        };

                                        Vector3d valxyyzzx = new Vector3d
                                        {
                                            X = val.XY / unitfactorxxyyzz,
                                            Y = val.YZ / unitfactorxxyyzz,
                                            Z = val.ZX / unitfactorxxyyzz
                                        };

                                        xyz.Add(valxxyyzz);
                                        xxyyzz.Add(valxyyzzx);
                                    }
                                    break;
                            }

                            // update max and min values
                            dmax_x = Math.Max(xyz.Max(val => val.X), dmax_x);
                            dmax_y = Math.Max(xyz.Max(val => val.Y), dmax_y);
                            dmax_z = Math.Max(xyz.Max(val => val.Z), dmax_z);

                            dmax_xyz = Math.Max(
                                xyz.Max(val =>
                                Math.Sqrt(
                                    Math.Pow(val.X, 2) +
                                    Math.Pow(val.Y, 2) +
                                    Math.Pow(val.Z, 2))),
                                dmax_xyz);

                            dmin_x = Math.Min(xyz.Min(val => val.X), dmin_x);
                            dmin_y = Math.Min(xyz.Min(val => val.Y), dmin_y);
                            dmin_z = Math.Min(xyz.Min(val => val.Z), dmin_z);

                            if (_mode == FoldMode.Stress)
                            {
                                dmax_xx = Math.Max(xxyyzz.Max(val => val.X), dmax_xx);
                                dmax_yy = Math.Max(xxyyzz.Max(val => val.Y), dmax_yy);
                                dmax_zz = Math.Max(xxyyzz.Max(val => val.Z), dmax_zz);

                                dmin_xx = Math.Min(xxyyzz.Min(val => val.X), dmin_xx);
                                dmin_yy = Math.Min(xxyyzz.Min(val => val.Y), dmin_yy);
                                dmin_zz = Math.Min(xxyyzz.Min(val => val.Z), dmin_zz);
                            }

                            // add vector lists to main lists
                            xyz_out.AddRange(xyz, new GH_Path(key));
                            xxyyzz_out.AddRange(xxyyzz, new GH_Path(key));
                        }
                    }
                    getresults = false;
                }
                #endregion

                #region Result mesh values
                // ### Coloured Result Meshes ###

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

                #region create mesh
                // create mesh

                // get elements and nodes from model
                elemList = string.Join(" ", keys.ToList());
                IReadOnlyDictionary<int, Element> elems = gsaModel.Model.Elements(elemList);
                IReadOnlyDictionary<int, Node> nodes = gsaModel.Model.Nodes();

                List<int> elemID = new List<int>();
                List<int> parentMember = new List<int>();
                ResultMesh resultMeshes = new ResultMesh(new Mesh(), new List<List<double>>());
                List<Mesh> meshes = new List<Mesh>();

                // loop through elements
                foreach (int key in elems.Keys)
                {
                    elems.TryGetValue(key, out Element element);

                    Mesh tempmesh = GhSA.Util.Gsa.FromGSA.ConvertElement3D(element, nodes);
                    if (tempmesh == null) { continue; }

                    List<Vector3d> transformation = null;
                    // add mesh colour
                    List<double> vals = new List<double>();

                    GH_Path path = new GH_Path(key);

                    List<Vector3d> tempXYZ = xyz_out.Branch(path);
                    List<Vector3d> tempXXYYZZ = xxyyzz_out.Branch(path);
                    switch (_disp)
                    {
                        case (DisplayValue.X):
                            vals = tempXYZ.ConvertAll(val => val.X);
                            transformation = new List<Vector3d>();
                            for (int i = 0; i < vals.Count; i++)
                            {
                                transformation.Add(new Vector3d(vals[i] * Value / 1000, 0, 0));
                            }
                            break;
                        case (DisplayValue.Y):
                            vals = tempXYZ.ConvertAll(val => val.Y);
                            transformation = new List<Vector3d>();
                            for (int i = 0; i < vals.Count; i++)
                            {
                                transformation.Add(new Vector3d(0, vals[i] * Value / 1000, 0));
                            }
                            break;
                        case (DisplayValue.Z):
                            vals = tempXYZ.ConvertAll(val => val.Z);
                            transformation = new List<Vector3d>();
                            for (int i = 0; i < vals.Count; i++)
                            {
                                transformation.Add(new Vector3d(0, 0, vals[i] * Value / 1000));
                            }
                            break;
                        case (DisplayValue.resXYZ):
                            vals = tempXYZ.ConvertAll(val => (
                            Math.Sqrt(
                                    Math.Pow(val.X, 2) +
                                    Math.Pow(val.Y, 2) +
                                    Math.Pow(val.Z, 2))));
                            transformation = tempXYZ.ConvertAll(vec => Vector3d.Multiply(Value / 1000, vec));
                            break;
                        case (DisplayValue.XX):
                            vals = tempXXYYZZ.ConvertAll(val => val.X);
                            break;
                        case (DisplayValue.YY):
                            vals = tempXXYYZZ.ConvertAll(val => val.Y);
                            break;
                        case (DisplayValue.ZZ):
                            vals = tempXXYYZZ.ConvertAll(val => val.Z);
                            break;
                        case (DisplayValue.resXXYYZZ):
                            vals = tempXXYYZZ.ConvertAll(val => (
                            Math.Sqrt(
                                    Math.Pow(val.X, 2) +
                                    Math.Pow(val.Y, 2) +
                                    Math.Pow(val.Z, 2))));
                            break;
                    }
                    
                    for (int i = 1; i < vals.Count; i++) // start at i=1 as the first index is the centre point in GsaAPI output
                    {
                        //normalised value between -1 and 1
                        double tnorm = 2 * (vals[i] - dmin) / (dmax - dmin) - 1;
                        System.Drawing.Color col = (double.IsNaN(tnorm)) ? System.Drawing.Color.Transparent : gH_Gradient.ColourAt(tnorm);
                        tempmesh.VertexColors.Add(col);
                        if (transformation != null)
                        {
                            Point3f def = tempmesh.Vertices[i - 1];
                            def.Transform(Transform.Translation(transformation[i]));
                            tempmesh.Vertices[i - 1] = def;
                        }
                    }
                    if (vals.Count == 1) // if analysis settings is set to '2D element forces and 2D/3D stresses at centre only'
                    {
                        //normalised value between -1 and 1
                        double tnorm = 2 * (vals[0] - dmin) / (dmax - dmin) - 1;
                        System.Drawing.Color col = (double.IsNaN(tnorm)) ? System.Drawing.Color.Transparent : gH_Gradient.ColourAt(tnorm);
                        for (int i = 0; i < tempmesh.Vertices.Count; i++)
                            tempmesh.VertexColors.Add(col);
                    }

                    resultMeshes.Add(tempmesh, vals);
                    //meshes.Add(tempmesh);
                    //resultMeshes.Add(resultMesh);
                    #endregion
                    elemID.Add(key);
                    parentMember.Add(element.ParentMember.Member);
                }
                resultMeshes.Finalise();
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
                int outind = 0;
                DA.SetDataTree(outind++, xyz_out);
                if (_mode == FoldMode.Stress)
                    DA.SetDataTree(outind++, xxyyzz_out);
                DA.SetData(outind++, resultMeshes);
                DA.SetDataList(outind++, cs);
                DA.SetDataList(outind++, ts);
            }
        }



        #region menu override
        private enum FoldMode
        {
            Displacement,
            Stress
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

            Params.UnregisterOutputParameter(Params.Output[1], true);

            slider = true;
            Value = 50;

            ReDrawComponent();

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Stress)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Stress;

            Params.RegisterOutputParam(new Param_Vector(), 1);

            slider = false;
            Value = 0;

            ReDrawComponent();

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
            writer.SetBoolean("slider", slider);
            writer.SetInt32("noDec", noDigits);
            writer.SetDouble("valMax", MaxValue);
            writer.SetDouble("valMin", MinValue);
            writer.SetDouble("val", Value);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            _disp = (DisplayValue)reader.GetInt32("Display");
            
            slider = reader.GetBoolean("slider");
            noDigits = reader.GetInt32("noDec");
            MaxValue = reader.GetDouble("valMax");
            MinValue = reader.GetDouble("valMin");
            Value = reader.GetDouble("val");

            dropdowncontents = new List<List<string>>();
            dropdowncontents.Add(dropdownitems);
            if (_mode == FoldMode.Displacement)
                dropdowncontents.Add(dropdowndisplacement);
            if (_mode == FoldMode.Stress)
            {
                dropdowncontents.Add(dropdownstress);
            }

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
                Params.Output[0].Description = "Translation Vector [Ux, Uy, Uz] (" + Units.LengthSmall + ")"
                    + System.Environment.NewLine + "Values order: [Centre, Vertex(0), Vertex(1), ..., Vertex(i)]";

                Params.Output[3].Description = "Legend Values (" + Units.LengthSmall + ")";
            }

            if (_mode == FoldMode.Stress)
            {
                Params.Output[0].NickName = "σ\u0305";
                Params.Output[0].Name = "Projected Stress Vector";
                Params.Output[0].Description = "Stress Vector [σxx, σyy, σzz] (" + Units.Stress + ")"
                    + System.Environment.NewLine + "Value order: [Centre, Vertex(0), Vertex(1), ..., Vertex(i)]";

                Params.Output[1].NickName = "τ\u0305";
                Params.Output[1].Name = "Projected Shear Stress Vector";
                Params.Output[1].Description = "Shear Stress Vector [τxy, τyz, τzx] (" + Units.Stress + ")"
                    + System.Environment.NewLine + "Values order: [Centre, Vertex(0), Vertex(1), ..., Vertex(i)]";

                Params.Output[4].Description = "Legend Values (" + Units.Stress + ")";
            }
        }
        #endregion  
    }
}