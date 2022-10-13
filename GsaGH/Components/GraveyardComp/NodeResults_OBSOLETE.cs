using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.Gsa;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Prop2d
  /// </summary>
  public class NodeResults_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7a8250b8-781e-4aeb-a05b-1b1fce9fbe3b");
    public NodeResults_OBSOLETE()
      : base("Node Results", "NodeResult", "Get GSA Node Results",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result0D;
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
      if (dropdownlistidd == 0) // if change is made to first list
      {
        if (selectedidd == 0)
        {
          if (dropdowncontents[1] != dropdowndisplacement)
          {
            dropdowncontents[1] = dropdowndisplacement;
            dropdowncontents[2] = FilteredUnits.FilteredLengthUnits;
            selections[0] = dropdowncontents[0][0];
            selections[1] = dropdowncontents[1][3];
            selections[2] = resultLengthUnit.ToString();
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
            selections[2] = forceUnit.ToString(); // default when changing is |F|
            getresults = true;
            Mode2Clicked();
          }

        }
        if (selectedidd == 2) // this should currently not be hit
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
      else if (dropdownlistidd == 1)
      {
        if (_mode == FoldMode.Reaction)
        {
          if ((int)_disp > 3 && selectedidd < 4)
          {
            dropdowncontents[2] = FilteredUnits.FilteredForceUnits;
            selections[2] = forceUnit.ToString();
          }
          if ((int)_disp < 4 && selectedidd > 3)
          {
            dropdowncontents[2] = FilteredUnits.FilteredMomentUnits;
            selections[2] = momentUnit.ToString();
          }
        }
        _disp = (DisplayValue)selectedidd;
        selections[1] = dropdowncontents[1][selectedidd];

        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        Params.OnParametersChanged();
        ExpireSolution(true);
      }
      else // change is made to the unit
      {
        if (dropdownlistidd == 2) // result units
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
        else // geometry unit
          geometryLengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selections[3]);
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
            "Reaction",
      //"Spring Force"
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

    private MomentUnit momentUnit = DefaultUnits.MomentUnit;
    private ForceUnit forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit resultLengthUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit geometryLengthUnit = DefaultUnits.LengthUnitGeometry;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Load Case", "LC", "Load Case (default 1)", GH_ParamAccess.item, 1);
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
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, resultLengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddVectorParameter("Translation [" + lengthunitAbbreviation + "]", "U\u0305", "(X, Y, Z) Translation Vector", GH_ParamAccess.list);
      pManager.AddVectorParameter("Rotations [rad]", "R\u0305", "(XX, YY, ZZ) Rotation Vector", GH_ParamAccess.list);
      pManager.AddGenericParameter("Point", "P", "Point with result value", GH_ParamAccess.list);
      pManager.AddGenericParameter("Result Colour", "Co", "Colours representing the result value at each point", GH_ParamAccess.list);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
    }

    #region fields
    ConcurrentDictionary<int, Vector3d> xyz = new ConcurrentDictionary<int, Vector3d>();
    ConcurrentDictionary<int, Vector3d> xxyyzz = new ConcurrentDictionary<int, Vector3d>();

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

        // Get node filter list
        GH_String gh_noList = new GH_String();
        DA.GetData(2, ref gh_noList);
        GH_Convert.ToString(gh_noList, out string tempnodeList, GH_Conversion.Both);

        // Get colours
        List<GH_Colour> gh_Colours = new List<GH_Colour>();
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
        ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(gsaModel.Model.Nodes(nodeList));

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
          ConcurrentDictionary<int, NodeResult> results = new ConcurrentDictionary<int, NodeResult>(analysisCaseResult.NodeResults(nodeList));

          #endregion

          #region Create results output
          // ### Loop through results ###
          // clear any existing lists of vectors to output results in:
          xyz = new ConcurrentDictionary<int, Vector3d>();
          xxyyzz = new ConcurrentDictionary<int, Vector3d>();

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

          // if reaction type, then we reuse the nodeList to filter support nodes from the rest
          if (_mode == FoldMode.Reaction)
            nodeList = "";


          Parallel.ForEach(results.Keys, nodeID =>
          {
            NodeResult result;
            Double6 values = null;

            if (_mode == FoldMode.Reaction)
            {
              bool isSupport = false;
              Node node = new Node();
              nodes.TryGetValue(nodeID, out node);
              NodalRestraint rest = node.Restraint;
              if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ)
                isSupport = true;
              if (!isSupport)
                return;
              else
              {
                if (nodeList == "")
                  nodeList = nodeID.ToString();
                else
                  nodeList += " " + nodeID;
              }
            }

            results.TryGetValue(nodeID, out result);
            switch (_mode)
            {
              case (FoldMode.Displacement):
                values = result.Displacement;
                //unitfactorxyz = 0.001;
                //unitfactorxxyyzz = 1;
                break;
              case (FoldMode.Reaction):
                values = result.Reaction;
                //unitfactorxyz = 1000;
                //unitfactorxxyyzz = 1000;
                break;
              case (FoldMode.SpringForce):
                values = result.SpringForce;
                //unitfactorxyz = 1000;
                //unitfactorxxyyzz = 1000;
                break;
              case (FoldMode.Constraint):
                values = result.Constraint;
                break;
            }

            // add the values to the vector lists
            if (_mode == FoldMode.Displacement)
            {
              xyz[nodeID] = ResultHelper.GetResult(values, resultLengthUnit);
              xxyyzz[nodeID] = ResultHelper.GetResult(values, AngleUnit.Radian);
            }
            else
            {
              xyz[nodeID] = ResultHelper.GetResult(values, forceUnit);
              xxyyzz[nodeID] = ResultHelper.GetResult(values, momentUnit);
            }
          });
          #endregion

          // update max and min values
          dmax_x = xyz.AsParallel().Select(res => res.Value.X).Max();
          dmax_y = xyz.AsParallel().Select(res => res.Value.Y).Max();
          dmax_z = xyz.AsParallel().Select(res => res.Value.Z).Max();
          dmax_xyz = xyz.AsParallel().Select(res =>
              Math.Sqrt(Math.Pow(res.Value.X, 2) + Math.Pow(res.Value.Y, 2) + Math.Pow(res.Value.Z, 2))
              ).Max();
          dmin_x = xyz.AsParallel().Select(res => res.Value.X).Min();
          dmin_y = xyz.AsParallel().Select(res => res.Value.Y).Min();
          dmin_z = xyz.AsParallel().Select(res => res.Value.Z).Min();
          dmin_xyz = xyz.AsParallel().Select(res =>
              Math.Sqrt(Math.Pow(res.Value.X, 2) + Math.Pow(res.Value.Y, 2) + Math.Pow(res.Value.Z, 2))
              ).Min();
          dmax_xx = xxyyzz.AsParallel().Select(res => res.Value.X).Max();
          dmax_yy = xxyyzz.AsParallel().Select(res => res.Value.Y).Max();
          dmax_zz = xxyyzz.AsParallel().Select(res => res.Value.Z).Max();
          dmax_xxyyzz = xxyyzz.AsParallel().Select(res =>
              Math.Sqrt(Math.Pow(res.Value.X, 2) + Math.Pow(res.Value.Y, 2) + Math.Pow(res.Value.Z, 2))
              ).Max();
          dmin_xx = xxyyzz.AsParallel().Select(res => res.Value.X).Min();
          dmin_yy = xxyyzz.AsParallel().Select(res => res.Value.Y).Min();
          dmin_zz = xxyyzz.AsParallel().Select(res => res.Value.Z).Min();
          dmin_xxyyzz = xyz.AsParallel().Select(res =>
              Math.Sqrt(Math.Pow(res.Value.X, 2) + Math.Pow(res.Value.Y, 2) + Math.Pow(res.Value.Z, 2))
              ).Min();

          getresults = false;
        }


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
        ConcurrentDictionary<int, ResultPointGoo> pts = new ConcurrentDictionary<int, ResultPointGoo>();
        ConcurrentDictionary<int, System.Drawing.Color> col = new ConcurrentDictionary<int, System.Drawing.Color>();

        Parallel.ForEach(gsanodes, node =>
        {
          if (node.Value.Value != null)
          {
            int nodeID = node.Value.Value.Id;
            if (xyz.ContainsKey(nodeID))
            {
              if (!(dmin == 0 & dmax == 0))
              {
                double t = 0;
                Vector3d translation = new Vector3d(0, 0, 0);
                // pick the right value to display
                switch (_disp)
                {
                  case (DisplayValue.X):
                    t = xyz[nodeID].X;
                    translation.X = t * DefScale / 1000;
                    break;
                  case (DisplayValue.Y):
                    t = xyz[nodeID].Y;
                    translation.Y = t * DefScale / 1000;
                    break;
                  case (DisplayValue.Z):
                    t = xyz[nodeID].Z;
                    translation.Z = t * DefScale / 1000;
                    break;
                  case (DisplayValue.resXYZ):
                    t = Math.Sqrt(Math.Pow(xyz[nodeID].X, 2) + Math.Pow(xyz[nodeID].Y, 2) + Math.Pow(xyz[nodeID].Z, 2));
                    translation.X = xyz[nodeID].X * DefScale / 1000;
                    translation.Y = xyz[nodeID].Y * DefScale / 1000;
                    translation.Z = xyz[nodeID].Z * DefScale / 1000;
                    break;
                  case (DisplayValue.XX):
                    t = xxyyzz[nodeID].X;
                    break;
                  case (DisplayValue.YY):
                    t = xxyyzz[nodeID].Y;
                    break;
                  case (DisplayValue.ZZ):
                    t = xxyyzz[nodeID].Z;
                    break;
                  case (DisplayValue.resXXYYZZ):
                    t = Math.Sqrt(Math.Pow(xxyyzz[nodeID].X, 2) + Math.Pow(xxyyzz[nodeID].Y, 2) + Math.Pow(xxyyzz[nodeID].Z, 2));
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

                // create deflection point
                Point3d def = new Point3d(node.Value.Value.Point);
                def.Transform(Transform.Translation(translation));

                // add our special resultpoint to the list of points
                pts[nodeID] = new ResultPointGoo(def, t, valcol, size);

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
        DA.SetDataList(0, xyz.OrderBy(x => x.Key).Select(y => y.Value).ToList());
        DA.SetDataList(1, xxyyzz.OrderBy(x => x.Key).Select(y => y.Value).ToList());
        DA.SetDataList(2, pts.OrderBy(x => x.Key).Select(y => y.Value).ToList());
        DA.SetDataList(3, col.OrderBy(x => x.Key).Select(y => y.Value).ToList());
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
    private void Mode3Clicked()
    {
      if (_mode == FoldMode.SpringForce)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.SpringForce;
      slider = false;
      DefScale = 0;

      ReDrawComponent();

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
      if (_mode == FoldMode.Reaction)
        dropdowncontents.Add(dropdownreaction);
      if (_mode == FoldMode.SpringForce)
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

      if (_mode == FoldMode.Reaction | _mode == FoldMode.SpringForce)
      {
        Params.Output[0].NickName = "F\u0305";
        Params.Output[0].Name = "Forces [" + forceunitAbbreviation + "]";
        Params.Output[0].Description = "(Nx, Vy, Vz) Force Vector ";

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