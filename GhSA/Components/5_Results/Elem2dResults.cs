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
    public class Elem2DResults : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("c2787348-ee8d-489c-bcb2-32a674c19267");
        public Elem2DResults()
          : base("2D Element Results", "Elem2dResults", "Get 2D Element Results from GSA",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.Elem2dResults;
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
                        if (dropdowncontents.Count > 2)
                            dropdowncontents.RemoveAt(2);
                        _disp = (DisplayValue)3;
                        flayer = 0;
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
                        selections[1] = dropdowncontents[1][0];
                        if (dropdowncontents.Count > 2)
                            dropdowncontents.RemoveAt(2);
                        _disp = 0;
                        flayer = 0;
                        getresults = true;
                        Mode2Clicked();
                    }
                }
                if (selectedidd == 2)
                {
                    if (dropdowncontents[1] != dropdownshear)
                    {
                        dropdowncontents[1] = dropdownshear;
                        selections[0] = dropdowncontents[0][1];
                        selections[1] = dropdowncontents[1][0];
                        if (dropdowncontents.Count > 2)
                            dropdowncontents.RemoveAt(2);
                        _disp = 0;
                        flayer = 0;
                        getresults = true;
                        Mode3Clicked();
                    }
                }
                if (selectedidd == 3)
                {
                    if (dropdowncontents[1] != dropdownstress)
                    {
                        dropdowncontents[1] = dropdownstress;
                        selections[0] = dropdowncontents[0][1];
                        selections[1] = dropdowncontents[1][0];
                        
                        if (dropdowncontents.Count < 3)
                            dropdowncontents.Add(dropdownlayer);

                        if (selections.Count < 3)
                            selections.Add(dropdowncontents[2][1]);
                        else
                            selections[2] = dropdowncontents[2][1];

                        _disp = 0;
                        getresults = true;
                        Mode4Clicked();
                    }
                }
            }

            if (dropdownlistidd == 1) // if change is made to second list
            {
                selections[1] = dropdowncontents[1][selectedidd];
                _disp = (DisplayValue)selectedidd;
                if (dropdowncontents[1] != dropdowndisplacement)
                    if (selectedidd > 2)
                        _disp = (DisplayValue)selectedidd + 1;
                
                (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
                Params.OnParametersChanged();
                ExpireSolution(true);
            }

            if (dropdownlistidd == 2) // if change is made to third list
            {
                if (selectedidd == 0)
                    flayer = 1;
                if (selectedidd == 1)
                    flayer = 0;
                if (selectedidd == 2)
                    flayer = -1;
                getresults = true;
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
            "Layer"
        });
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Displacement",
            "Force/Moment",
            "Shear",
            "Stress"
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
            "Force Nx",
            "Force Ny",
            "Force Nxy",
            "Moment Mx",
            "Moment My",
            "Moment Mxy"
        });
        
        readonly List<string> dropdownshear = new List<string>(new string[]
        {
            "Shear Qx",
            "Shear Qy"
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

        readonly List<string> dropdownlayer = new List<string>(new string[]
        {
            "Top",
            "Middle",
            "Bottom"
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
            pManager.AddVectorParameter("Rotation", "R\u0305", "XX, YY, ZZ rotation values(" + Units.Angle + ")", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Mesh", "M", "Mesh with result values", GH_ParamAccess.list);
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
        int flayer = 0;
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
                    IReadOnlyDictionary<int, Element2DResult> globalResults = analysisCaseResult.Element2DResults(elemList, flayer);
                    
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
                        Element2DResult elementResults;
                        if (globalResults.TryGetValue(key, out elementResults))
                        {
                            List<Vector3d> xyz = new List<Vector3d>();
                            List<Vector3d> xxyyzz = new List<Vector3d>();

                            // set the result type dependent on user selection in dropdown
                            switch (_mode)
                            {
                                case (FoldMode.Displacement):
                                    unitfactorxyz = 0.001;
                                    unitfactorxxyyzz = 1;

                                    List<Double6> values = elementResults.Displacement.ToList();
                                    foreach (Double6 val in values)
                                    {
                                        Vector3d valxyz = new Vector3d
                                        {
                                            X = val.X / unitfactorxyz,
                                            Y = val.Y / unitfactorxyz,
                                            Z = val.Z / unitfactorxyz
                                        };

                                        Vector3d valxxyyzz = new Vector3d
                                        {
                                            X = val.XX / unitfactorxxyyzz,
                                            Y = val.YY / unitfactorxxyyzz,
                                            Z = val.ZZ / unitfactorxxyyzz
                                        };

                                        xyz.Add(valxyz);
                                        xxyyzz.Add(valxxyyzz);
                                    }
                                    break;
                                case (FoldMode.Force):
                                    unitfactorxyz = 1000;

                                    List<Tensor2> forces = elementResults.Force.ToList();

                                    foreach (Tensor2 force in forces)
                                    {
                                        Vector3d valxyz = new Vector3d
                                        {
                                            X = force.XX / unitfactorxyz,
                                            Y = force.YY / unitfactorxyz,
                                            Z = force.XY / unitfactorxyz
                                        };

                                        xyz.Add(valxyz);
                                    }

                                    unitfactorxxyyzz = 1000;
                                    List<Tensor2> moments = elementResults.Moment.ToList();
                                    foreach (Tensor2 moment in moments)
                                    {
                                        Vector3d valxxyyzz = new Vector3d
                                        {
                                            X = moment.XX / unitfactorxxyyzz,
                                            Y = moment.YY / unitfactorxxyyzz,
                                            Z = moment.XY / unitfactorxxyyzz
                                        };

                                        xxyyzz.Add(valxxyyzz);
                                    }
                                    break;
                                case (FoldMode.Shear):
                                    unitfactorxyz = 1000;
                                    unitfactorxxyyzz = 1000;
                                    List<Vector2> shears = elementResults.Shear.ToList();
                                    foreach (Vector2 shear in shears)
                                    {
                                        Vector3d valxyz = new Vector3d
                                        {
                                            X = shear.X / unitfactorxyz,
                                            Y = shear.Y / unitfactorxyz,
                                            Z = 0,
                                        };

                                        xyz.Add(valxyz);
                                    }
                                    break;
                                case (FoldMode.Stress):
                                    unitfactorxyz = 1000000;
                                    unitfactorxxyyzz = 1000000;

                                    List<Tensor3> stresses = elementResults.Stress.ToList();
                                    foreach (Tensor3 stress in stresses)
                                    {
                                        Vector3d valxyz = new Vector3d
                                        {
                                            X = stress.XX / unitfactorxyz,
                                            Y = stress.YY / unitfactorxyz,
                                            Z = stress.ZZ / unitfactorxyz
                                        };

                                        Vector3d valxxyyzz = new Vector3d
                                        {
                                            X = stress.XY / unitfactorxxyyzz,
                                            Y = stress.YZ / unitfactorxxyyzz,
                                            Z = stress.ZX / unitfactorxxyyzz
                                        };

                                        xyz.Add(valxyz);
                                        xxyyzz.Add(valxxyyzz);
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

                            if (_mode != FoldMode.Shear)
                            {
                                dmax_xx = Math.Max(xxyyzz.Max(val => val.X), dmax_xx);
                                dmax_yy = Math.Max(xxyyzz.Max(val => val.Y), dmax_yy);
                                dmax_zz = Math.Max(xxyyzz.Max(val => val.Z), dmax_zz);

                                dmax_xxyyzz = Math.Max(
                                    xxyyzz.Max(val =>
                                    Math.Sqrt(
                                        Math.Pow(val.X, 2) +
                                        Math.Pow(val.Y, 2) +
                                        Math.Pow(val.Z, 2))),
                                    dmax_xxyyzz);
                            }

                            dmin_x = Math.Min(xyz.Min(val => val.X), dmin_x);
                            dmin_y = Math.Min(xyz.Min(val => val.Y), dmin_y);
                            dmin_z = Math.Min(xyz.Min(val => val.Z), dmin_z);

                            dmin_xx = Math.Min(xxyyzz.Min(val => val.X), dmin_xx);
                            dmin_yy = Math.Min(xxyyzz.Min(val => val.Y), dmin_yy);
                            dmin_zz = Math.Min(xxyyzz.Min(val => val.Z), dmin_zz);

                            // add vector lists to main lists
                            xyz_out.AddRange(xyz, new GH_Path(key - 1));
                            xxyyzz_out.AddRange(xxyyzz, new GH_Path(key - 1));
                        }
                    }

                    getresults = false;
                }
                #endregion

                #region Result mesh values
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

                #region create mesh
                // create mesh

                // get elements and nodes from model
                elemList = string.Join(" ", keys.ToList());
                IReadOnlyDictionary<int, Element> elems = gsaModel.Model.Elements(elemList);
                IReadOnlyDictionary<int, Node> nodes = gsaModel.Model.Nodes();

                List<int> elemID = new List<int>();
                List<int> parentMember = new List<int>();
                List<ResultMesh> resultMeshes = new List<ResultMesh>();
                List<Mesh> meshes = new List<Mesh>();

                // loop through elements
                foreach (int key in elems.Keys)
                {
                    elems.TryGetValue(key, out Element element);

                    // create empty mesh and add vertices
                    Mesh tempmesh = new Mesh();
                    for (int i = 0; i < element.Topology.Count; i++)
                    {
                        int nodeid = element.Topology[i];
                        nodes.TryGetValue(nodeid, out Node node);
                        Point3d pt = new Point3d(
                            node.Position.X,
                            node.Position.Y,
                            node.Position.Z
                            );
                        tempmesh.Vertices.Add(pt);
                    }

                    // Create mesh face (Tri- or Quad, or if GSA Tri-6 or Quad-8 we create multiple faces with a new vertex in the middle):
                    switch (tempmesh.Vertices.Count)
                    {
                        case (3):
                            tempmesh.Faces.AddFace(0, 1, 2);
                            break;
                        case (4):
                            tempmesh.Faces.AddFace(0, 1, 2, 3);
                            break;
                        case (6):
                            tempmesh.Vertices.Add(new Point3d(
                                tempmesh.Vertices.Average(pt => pt.X),
                                tempmesh.Vertices.Average(pt => pt.Y),
                                tempmesh.Vertices.Average(pt => pt.Z)
                                ));
                            tempmesh.Faces.AddFace(0, 3, 6);
                            tempmesh.Faces.AddFace(3, 1, 6);
                            tempmesh.Faces.AddFace(1, 4, 6);
                            tempmesh.Faces.AddFace(4, 2, 6);
                            tempmesh.Faces.AddFace(2, 5, 6);
                            tempmesh.Faces.AddFace(5, 0, 6);
                            break;
                        case (8):
                            tempmesh.Vertices.Add(new Point3d(
                                tempmesh.Vertices.Average(pt => pt.X),
                                tempmesh.Vertices.Average(pt => pt.Y),
                                tempmesh.Vertices.Average(pt => pt.Z)
                                ));
                            tempmesh.Faces.AddFace(0, 4, 8, 7);
                            tempmesh.Faces.AddFace(1, 5, 8, 4);
                            tempmesh.Faces.AddFace(2, 6, 8, 5);
                            tempmesh.Faces.AddFace(3, 7, 8, 6);
                            break;
                    }

                    // add mesh colour
                    List<double> vals = new List<double>();

                    GH_Path path = new GH_Path(key - 1);

                    List<Vector3d> tempXYZ = xyz_out.Branch(path);
                    List<Vector3d> tempXXYYZZ = xxyyzz_out.Branch(path);
                    switch (_disp)
                    {
                        case (DisplayValue.X):
                            vals = tempXYZ.ConvertAll(val => val.X);
                            break;
                        case (DisplayValue.Y):
                            vals = tempXYZ.ConvertAll(val => val.Y);
                            break;
                        case (DisplayValue.Z):
                            vals = tempXYZ.ConvertAll(val => val.Z);
                            break;
                        case (DisplayValue.resXYZ):
                            vals = tempXYZ.ConvertAll(val => (
                            Math.Sqrt(
                                    Math.Pow(val.X, 2) +
                                    Math.Pow(val.Y, 2) +
                                    Math.Pow(val.Z, 2))));
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
                    }
                    if (tempmesh.Vertices.Count > 4) // add the value/colour at the centre point if tri-6 or quad-8
                    {
                        double tnorm = 2 * (vals[0] - dmin) / (dmax - dmin) - 1;
                        System.Drawing.Color col = (double.IsNaN(tnorm)) ? System.Drawing.Color.Transparent : gH_Gradient.ColourAt(tnorm);
                        tempmesh.VertexColors.Add(col);
                    }

                    ResultMesh resultMesh = new ResultMesh(tempmesh, vals);
                    meshes.Add(tempmesh);
                    resultMeshes.Add(resultMesh);
                    #endregion
                    elemID.Add(key);
                    parentMember.Add(element.ParentMember.Member);
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
                DA.SetDataList(2, resultMeshes);
                DA.SetDataList(3, cs);
                DA.SetDataList(4, ts);
            }
        }



        #region menu override
        private enum FoldMode
        {
            Displacement,
            Force,
            Shear,
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
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.Shear)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Shear;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode4Clicked()
        {
            if (_mode == FoldMode.Stress)
                return;

            RecordUndoEvent(_mode.ToString() + " Parameters");
            _mode = FoldMode.Stress;

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
            writer.SetInt32("flayer", flayer);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            _disp = (DisplayValue)reader.GetInt32("Display");
            flayer = reader.GetInt32("flayer");

            dropdowncontents = new List<List<string>>();
            dropdowncontents.Add(dropdownitems);
            if (_mode == FoldMode.Displacement)
                dropdowncontents.Add(dropdowndisplacement);
            if (_mode == FoldMode.Force)
                dropdowncontents.Add(dropdownforce);
            if (_mode == FoldMode.Shear)
                dropdowncontents.Add(dropdownshear);
            if (_mode == FoldMode.Stress)
            {
                dropdowncontents.Add(dropdownstress);
                dropdowncontents.Add(dropdownlayer);
            }

            selections = new List<string>();
            selections.Add(dropdowncontents[0][(int)_mode]);
            selections.Add(dropdowncontents[1][(int)_disp]);
            if (_mode == FoldMode.Stress)
            {
                if (flayer == 1)
                    selections.Add(dropdowncontents[2][0]);
                if (flayer == 0)
                    selections.Add(dropdowncontents[2][1]);
                if (flayer == -1)
                    selections.Add(dropdowncontents[2][2]);
            }
                
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

                Params.Output[1].NickName = "R\u0305";
                Params.Output[1].Name = "Rotation";
                Params.Output[1].Description = "Rotation Vector [Rxx, Ryy, Rzz] (" + Units.Angle + ")"
                    + System.Environment.NewLine + "Values order: [Centre, Vertex(0), Vertex(1), ..., Vertex(i)]";

                if ((int)_disp < 4)
                    Params.Output[4].Description = "Legend Values (" + Units.LengthSmall + ")";
                else
                    Params.Output[4].Description = "Legend Values (" + Units.Angle + ")";

            }

            if (_mode == FoldMode.Force)
            {
                Params.Output[0].NickName = "F\u0305";
                Params.Output[0].Name = "Force";
                Params.Output[0].Description = "Force vector [Fx, Fy, Fxy] (" + Units.Force + ")"
                    + System.Environment.NewLine + "Values order: [Centre, Vertex(0), Vertex(1), ..., Vertex(i)]";

                Params.Output[1].NickName = "M\u0305";
                Params.Output[1].Name = "Moment";
                Params.Output[1].Description = "Moment vector [Mxx, Myy, Mxy] (" + Units.Force + "/" + Units.LengthLarge + ")"
                    + System.Environment.NewLine + "Values order: [Centre, Vertex(0), Vertex(1), ..., Vertex(i)]";

                if ((int)_disp < 4)
                    Params.Output[4].Description = "Legend Values (" + Units.Force + ")";
                else
                    Params.Output[4].Description = "Legend Values (" + Units.Force + "/" + Units.LengthLarge + ")";
            }

            if ( _mode == FoldMode.Shear)
            {
                Params.Output[0].NickName = "V\u0305";
                Params.Output[0].Name = "Shear 2D-Vector";
                Params.Output[0].Description = "Shear 2D-Vector [Vx, Vy, --] (" + Units.Force + ")"
                    + System.Environment.NewLine + "Values order: [Centre, Vertex(0), Vertex(1), ..., Vertex(i)]";

                Params.Output[1].NickName = "-";
                Params.Output[1].Name = "(empty)";
                Params.Output[1].Description = "No output for the selected result type";

                if ((int)_disp < 4)
                    Params.Output[4].Description = "Legend Values (" + Units.Force + ")";
                else
                    Params.Output[4].Description = "Legend Values (" + Units.Force + "/" + Units.LengthLarge + ")";
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