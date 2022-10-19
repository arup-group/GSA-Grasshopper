using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.Gsa;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get Element1D results
  /// </summary>
  public class Elem1DResults_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("79cd1187-2b27-4bee-a77f-4f94c98e73c3");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result1D;

    public Elem1DResults_OBSOLETE()
      : base("1D Element Results", "Elem1dResults", "Get 1D Element Results from GSA",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    {
    }

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
        dropdowncontents.Add(FilteredUnits.FilteredLengthUnits);
        dropdowncontents.Add(FilteredUnits.FilteredLengthUnits);
        selections = new List<string>();
        selections.Add(dropdowncontents[0][0]);
        selections.Add(dropdowncontents[1][3]);
        selections.Add(DefaultUnits.LengthUnitResult.ToString());
        selections.Add(DefaultUnits.LengthUnitGeometry.ToString());
        first = false;
      }
      m_attributes = new UI.MultiDropDownSliderComponentUI(this, SetSelected, dropdowncontents, selections, slider, SetVal, SetMaxMin, DefScale, MaxValue, MinValue, noDigits, spacertext);
    }

    double MinValue = 0;
    double MaxValue = 1000;
    double DefScale = 100;
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
      if (dropdownlistidd == 0) // if change is made to result type (displacement or force)
      {
        if (selectedidd == 0) // displacement selected
        {
          if (dropdowncontents[1] != dropdowndisplacement)
          {
            dropdowncontents[1] = dropdowndisplacement;
            dropdowncontents[2] = FilteredUnits.FilteredLengthUnits;

            selections[0] = dropdowncontents[0][0];
            selections[1] = dropdowncontents[1][3];
            selections[2] = resultLengthUnit.ToString(); // displacement

            _disp = DisplayValue.resXYZ;
            getresults = true;
            Mode1Clicked();
          }

        }
        if (selectedidd == 1) // force selected
        {
          if (dropdowncontents[1] != dropdownforce)
          {
            dropdowncontents[1] = dropdownforce;
            dropdowncontents[2] = FilteredUnits.FilteredForcePerLengthUnits;

            selections[0] = dropdowncontents[0][1];
            selections[1] = dropdowncontents[1][5];
            selections[2] = momentUnit.ToString(); // default when changing is Myy

            _disp = DisplayValue.YY;
            getresults = true;
            Mode2Clicked();
          }
        }
      } // if changes to the selected result type
      else if (dropdownlistidd == 1)
      {
        bool redraw = false;

        if (selectedidd < 4)
        {
          if ((int)_disp > 3) // chekc if we are coming from other half of display modes
          {
            if (_mode == FoldMode.Displacement)
            {
              redraw = true;
              slider = true;
            }
            else
            {
              dropdowncontents[2] = FilteredUnits.FilteredForceUnits;
              selections[2] = forceUnit.ToString();
            }
          }
        }
        else
        {
          if ((int)_disp < 4) // chekc if we are coming from other half of display modes
          {
            if (_mode == FoldMode.Displacement)
            {
              redraw = true;
              slider = false;
            }
            else
            {
              dropdowncontents[2] = FilteredUnits.FilteredForcePerLengthUnits;
              selections[2] = momentUnit.ToString();
            }
          }
        }
        _disp = (DisplayValue)selectedidd;

        selections[1] = dropdowncontents[1][selectedidd];

        if (redraw)
          ReDrawComponent();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        Params.OnParametersChanged();
        ExpireSolution(true);
      }
      else // change is made to the unit
      {
        if (dropdownlistidd == 2)
        {
          if (_mode == FoldMode.Displacement)
            resultLengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selections[2]);
          else
          {
            if ((int)_disp < 4)
              forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selections[2]);
            else
              momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), selections[2]);
          }
        }
        else
          geometryLengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selections[3]);
        getresults = true;
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
            "Result Unit",
            "Geometry Unit",
            "Deform Shape"
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

    private MomentUnit momentUnit = DefaultUnits.MomentUnit;
    private ForceUnit forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit resultLengthUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit geometryLengthUnit = DefaultUnits.LengthUnitGeometry;
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
      IQuantity length = new Length(0, resultLengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddVectorParameter("Translations [" + lengthunitAbbreviation + "]", "U\u0305", "(X, Y, Z) Translation Vector", GH_ParamAccess.tree);
      pManager.AddVectorParameter("Rotations [rad]", "R\u0305", "(XX, YY, ZZ) Rotation Vector", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Line", "L", "Line with coloured result values", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Result Colour", "Co", "Colours representing the result value at each point", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
    }

    #region fields
    // new lists of vectors to output results in:
    ConcurrentDictionary<int, ConcurrentDictionary<int, Vector3d>> xyzResults = new ConcurrentDictionary<int, ConcurrentDictionary<int, Vector3d>>();
    ConcurrentDictionary<int, ConcurrentDictionary<int, Vector3d>> xxyyzzResults = new ConcurrentDictionary<int, ConcurrentDictionary<int, Vector3d>>();
    ConcurrentDictionary<int, ConcurrentDictionary<int, Line>> lineResults = new ConcurrentDictionary<int, ConcurrentDictionary<int, Line>>();
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
            if (in_Model.Guid != gsaModel.Guid) // only get results if GUID is not similar
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
        if (temppositionsCount < 2)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Number of positions must be at least 2, one for start and end of line"
              + System.Environment.NewLine + "Number of positions has been set to 2.");
          temppositionsCount = 2;
        }

        // Get colours
        List<GH_Colour> gh_Colours = new List<GH_Colour>();
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
          //CombinationCaseResult combinationCaseResult = null;
          //gsaModel.Model.CombinationCaseResults().TryGetValue(analCase, out combinationCaseResult);
          if (analysisCaseResult == null)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No results exist for Analysis Case " + analCase + " in file");
            return;
          }
          ConcurrentDictionary<int, Element> elems = new ConcurrentDictionary<int, Element>(gsaModel.Model.Elements(elemList));
          if (elems.Count == 0)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No elements in list: " + elemList + " found");
            return;
          }
          ConcurrentDictionary<int, Element1DResult> globalResults = new ConcurrentDictionary<int, Element1DResult>(analysisCaseResult.Element1DResults(elemList, positionsCount));
          ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(gsaModel.Model.Nodes());
          #endregion


          // ### Loop through results ###
          // clear existing result lists
          xyzResults = new ConcurrentDictionary<int, ConcurrentDictionary<int, Vector3d>>();
          xyzResults.AsParallel().AsOrdered();
          xxyyzzResults = new ConcurrentDictionary<int, ConcurrentDictionary<int, Vector3d>>();
          xxyyzzResults.AsParallel().AsOrdered();
          lineResults = new ConcurrentDictionary<int, ConcurrentDictionary<int, Line>>();
          lineResults.AsParallel().AsOrdered();

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

          //double unitfactorxyz = 1;
          //double unitfactorxxyyzz = 1;

          Parallel.ForEach(globalResults.Keys, key =>
          {
            // list for element geometry and info
            Element element = new Element();
            elems.TryGetValue(key, out element);
            if (element.IsDummy) { return; }
            Node start = new Node();
            nodes.TryGetValue(element.Topology[0], out start);
            Point3d pt1 = FromGSA.Point3dFromNode(start, geometryLengthUnit);
            Node end = new Node();
            nodes.TryGetValue(element.Topology[1], out end);
            Point3d pt2 = FromGSA.Point3dFromNode(end, geometryLengthUnit);
            Line ln = new Line(pt1, pt2);

            // lists for results
            Element1DResult elementResults;
            globalResults.TryGetValue(key, out elementResults);
            List<Double6> values = new List<Double6>();
            ConcurrentDictionary<int, Vector3d> xyzRes = new ConcurrentDictionary<int, Vector3d>();
            xyzRes.AsParallel().AsOrdered();
            ConcurrentDictionary<int, Vector3d> xxyyzzRes = new ConcurrentDictionary<int, Vector3d>();
            xxyyzzRes.AsParallel().AsOrdered();
            ConcurrentDictionary<int, Line> lineRes = new ConcurrentDictionary<int, Line>();
            lineRes.AsParallel().AsOrdered();

            // set the result type dependent on user selection in dropdown
            switch (_mode)
            {
              case (FoldMode.Displacement):
                values = elementResults.Displacement.ToList();
                //unitfactorxyz = 0.001;
                //unitfactorxxyyzz = 1;
                break;
              case (FoldMode.Force):
                values = elementResults.Force.ToList();
                //unitfactorxyz = 1000;
                //unitfactorxxyyzz = 1000;
                break;
            }

            // prepare the line segments
            int segments = Math.Max(1, values.Count - 1); // number of segment lines is 1 less than number of points

            // loop through the results
            Parallel.For(0, values.Count, i =>
                      {
                Double6 result = values[i];

                        // add the values to the vector lists
                if (_mode == FoldMode.Displacement)
                {
                  xyzRes[i] = ResultHelper.GetResult(result, resultLengthUnit);
                  xxyyzzRes[i] = ResultHelper.GetResult(result, AngleUnit.Radian);
                }
                else
                {
                  xyzRes[i] = ResultHelper.GetResult(result, forceUnit);
                  xxyyzzRes[i] = ResultHelper.GetResult(result, momentUnit);
                }

                        // create ResultLines
                if (i < segments)
                {
                  Line segmentline = new Line(
                                ln.PointAt((double)i / segments),
                                ln.PointAt((double)(i + 1) / segments)
                                );
                  lineRes[i] = segmentline;
                }
              });
            // add the vector list to the out tree
            xyzResults[key] = xyzRes;
            xxyyzzResults[key] = xxyyzzRes;
            lineResults[key] = lineRes;
          });

          // update max and min values
          dmax_x = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
          dmax_y = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
          dmax_z = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
          dmax_xyz = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
              Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))
              ).Max()).Max();
          dmin_x = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
          dmin_y = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
          dmin_z = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
          dmin_xyz = xyzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
              Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))
              ).Min()).Min();
          dmax_xx = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
          dmax_yy = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
          dmax_zz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
          dmax_xxyyzz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
              Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))
              ).Max()).Max();
          dmin_xx = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
          dmin_yy = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
          dmin_zz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
          dmin_xxyyzz = xxyyzzResults.AsParallel().Select(list => list.Value.Values.Select(res =>
              Math.Sqrt(Math.Pow(res.X, 2) + Math.Pow(res.Y, 2) + Math.Pow(res.Z, 2))
              ).Min()).Min();

          if (dmax_x == 0 &
          dmax_y == 0 &
          dmax_z == 0 &
          dmax_xx == 0 &
          dmax_yy == 0 &
          dmax_zz == 0 &
          dmin_x == 0 &
          dmin_y == 0 &
          dmin_z == 0 &
          dmin_xx == 0 &
          dmin_yy == 0 &
          dmin_zz == 0)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "All results are zero");
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
        ConcurrentDictionary<int, ConcurrentDictionary<int, ResultLineGoo>> resultLines = new ConcurrentDictionary<int, ConcurrentDictionary<int, ResultLineGoo>>();
        DataTree<ResultLineGoo> lines_out = new DataTree<ResultLineGoo>();
        DataTree<System.Drawing.Color> col_out = new DataTree<System.Drawing.Color>();

        Parallel.ForEach(lineResults.Keys, key =>
        {
          GH_Path path = new GH_Path(key);
          ConcurrentDictionary<int, ResultLineGoo> resLns = new ConcurrentDictionary<int, ResultLineGoo>();
          ConcurrentDictionary<int, System.Drawing.Color> resCol = new ConcurrentDictionary<int, System.Drawing.Color>();

          ConcurrentDictionary<int, Line> lineRes = lineResults[key];

          List<Line> segmentedlines = lineRes.Values.ToList();

          for (int j = 0; j < segmentedlines.Count; j++)
          {
            if (!(dmin == 0 & dmax == 0))
            {
              Vector3d startTranslation = new Vector3d(0, 0, 0);
              Vector3d endTranslation = new Vector3d(0, 0, 0);

              double t1 = 0;
              double t2 = 0;

              // pick the right value to display
              switch (_disp)
              {
                case (DisplayValue.X):
                  t1 = xyzResults[key][j].X;
                  t2 = xyzResults[key][j + 1].X;
                  startTranslation.X = t1 * DefScale / 1000;
                  endTranslation.X = t2 * DefScale / 1000;
                  break;
                case (DisplayValue.Y):
                  t1 = xyzResults[key][j].Y;
                  t2 = xyzResults[key][j + 1].Y;
                  startTranslation.Y = t1 * DefScale / 1000;
                  endTranslation.Y = t2 * DefScale / 1000;
                  break;
                case (DisplayValue.Z):
                  t1 = xyzResults[key][j].Z;
                  t2 = xyzResults[key][j + 1].Z;
                  startTranslation.Z = t1 * DefScale / 1000;
                  endTranslation.Z = t2 * DefScale / 1000;
                  break;
                case (DisplayValue.resXYZ):
                  t1 = Math.Sqrt(Math.Pow(xyzResults[key][j].X, 2) + Math.Pow(xyzResults[key][j].Y, 2) + Math.Pow(xyzResults[key][j].Z, 2));
                  t2 = Math.Sqrt(Math.Pow(xyzResults[key][j + 1].X, 2) + Math.Pow(xyzResults[key][j + 1].Y, 2) + Math.Pow(xyzResults[key][j + 1].Z, 2));
                  startTranslation.X = xyzResults[key][j].X * DefScale / 1000;
                  endTranslation.X = xyzResults[key][j + 1].X * DefScale / 1000;
                  startTranslation.Y = xyzResults[key][j].Y * DefScale / 1000;
                  endTranslation.Y = xyzResults[key][j + 1].Y * DefScale / 1000;
                  startTranslation.Z = xyzResults[key][j].Z * DefScale / 1000;
                  endTranslation.Z = xyzResults[key][j + 1].Z * DefScale / 1000;
                  break;
                case (DisplayValue.XX):
                  t1 = xxyyzzResults[key][j].X;
                  t2 = xxyyzzResults[key][j + 1].X;
                  break;
                case (DisplayValue.YY):
                  t1 = xxyyzzResults[key][j].Y;
                  t2 = xxyyzzResults[key][j + 1].Y;
                  break;
                case (DisplayValue.ZZ):
                  t1 = xxyyzzResults[key][j].Z;
                  t2 = xxyyzzResults[key][j + 1].Z;
                  break;
                case (DisplayValue.resXXYYZZ):
                  t1 = Math.Sqrt(Math.Pow(xxyyzzResults[key][j].X, 2) + Math.Pow(xxyyzzResults[key][j].Y, 2) + Math.Pow(xxyyzzResults[key][j].Z, 2));
                  t2 = Math.Sqrt(Math.Pow(xxyyzzResults[key][j + 1].X, 2) + Math.Pow(xxyyzzResults[key][j + 1].Y, 2) + Math.Pow(xxyyzzResults[key][j + 1].Z, 2));
                  break;
              }
              Point3d start = new Point3d(segmentedlines[j].PointAt(0));
              start.Transform(Transform.Translation(startTranslation));
              Point3d end = new Point3d(segmentedlines[j].PointAt(1));
              end.Transform(Transform.Translation(endTranslation));
              Line segmentline = new Line(start, end);

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
              resLns[j] = new ResultLineGoo(segmentline, t1, t2, valcol1, valcol2, size1, size2);

              // add the colour to the colours list
              resCol[j] = valcol1;
              if (j == segmentedlines.Count - 1)
                resCol[j + 1] = valcol2;
            }
          }
          lock (lines_out)
          {
            lines_out.AddRange(resLns.Values.ToList(), path);
          }
          lock (col_out)
          {
            col_out.AddRange(resCol.Values.ToList(), path);
          }

        });

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

        // convert result libraries to datatrees
        DataTree<Vector3d> xyz_out = new DataTree<Vector3d>();
        DataTree<Vector3d> xxyyzz_out = new DataTree<Vector3d>();
        Parallel.ForEach(xyzResults.Keys, path =>
        {
          xyzResults.TryGetValue(path, out ConcurrentDictionary<int, Vector3d> xyzRes);
          lock (xyz_out)
          {
            xyz_out.AddRange(xyzRes.Values.ToList(), new GH_Path(path));
          }
          xxyyzzResults.TryGetValue(path, out ConcurrentDictionary<int, Vector3d> xxyyzzRes);
          lock (xxyyzz_out)
          {
            xxyyzz_out.AddRange(xxyyzzRes.Values.ToList(), new GH_Path(path));
          }
        });

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
      if (_mode == FoldMode.Force)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.Force;

      slider = false;
      DefScale = 0;

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
      writer.SetDouble("val", DefScale);
      writer.SetString("selectedUnit", selections[2]);
      writer.SetString("resultLengthUnit", resultLengthUnit.ToString());
      writer.SetString("forceUnit", forceUnit.ToString());
      writer.SetString("momentUnit", momentUnit.ToString());
      writer.SetString("geometryLengthUnit", geometryLengthUnit.ToString());
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
      DefScale = reader.GetDouble("val");

      dropdowncontents = new List<List<string>>();
      dropdowncontents.Add(dropdownitems);
      if (_mode == FoldMode.Displacement)
        dropdowncontents.Add(dropdowndisplacement);
      if (_mode == FoldMode.Force)
        dropdowncontents.Add(dropdownforce);

      selections = new List<string>();
      selections.Add(dropdowncontents[0][(int)_mode]);
      selections.Add(dropdowncontents[1][(int)_disp]);
      try
      {
        selections.Add(reader.GetString("selectedUnit"));
        resultLengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), reader.GetString("resultLengthUnit"));
        forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), reader.GetString("forceUnit"));
        momentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), reader.GetString("momentUnit"));
        geometryLengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), reader.GetString("geometryLengthUnit"));
      }
      catch (Exception) // if user has old component this will fail and we set it to kN, kN/m or mm
      {
        resultLengthUnit = LengthUnit.Millimeter;
        forceUnit = ForceUnit.Kilonewton;
        momentUnit = MomentUnit.KilonewtonMeter;
        geometryLengthUnit = LengthUnit.Meter;
        if (_mode == FoldMode.Displacement)
          selections.Add(resultLengthUnit.ToString());
        else
        {
          if ((int)_disp < 4)
            selections.Add(forceUnit.ToString());
          else
            selections.Add(momentUnit.ToString());
        }
      }
      selections.Add(geometryLengthUnit.ToString());
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
      string momentunitAbbreviation = Moment.GetAbbreviation(momentUnit);
      IQuantity force = new Force(0, forceUnit);
      string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      IQuantity length = new Length(0, resultLengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));


      if (_mode == FoldMode.Displacement)
      {
        Params.Output[0].NickName = "U\u0305";
        Params.Output[0].Name = "Translations [" + lengthunitAbbreviation + "]";
        Params.Output[0].Description = "(X, Y, Z) Translation Vector";

        Params.Output[1].NickName = "R\u0305";
        Params.Output[1].Name = "Rotations [rad]";
        Params.Output[1].Description = "(XX, YY, ZZ) Rotation Vector";

        if ((int)_disp < 4)
          Params.Output[5].Name = "Values [" + lengthunitAbbreviation + "]";
        else
          Params.Output[5].Name = "Values [rad]";
      }

      if (_mode == FoldMode.Force)
      {
        Params.Output[0].NickName = "F\u0305";
        Params.Output[0].Name = "Forces [" + forceunitAbbreviation + "]";
        Params.Output[0].Description = "(Nx, Vy, Vz) Force Vector";

        Params.Output[1].NickName = "M\u0305";
        Params.Output[1].Name = "Moments [" + momentunitAbbreviation + "]";
        Params.Output[1].Description = "(Mxx, Myy, Mzz) Moment Vector";

        if ((int)_disp < 4)
          Params.Output[5].Name = "Legend Values [" + forceunitAbbreviation + "]";
        else
          Params.Output[5].Name = "Legend Values [" + momentunitAbbreviation + "]";
      }
    }
    #endregion
  }
}