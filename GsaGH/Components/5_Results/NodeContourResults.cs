using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers.GsaAPI;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;
using GsaGH.Helpers.GH;
using System.Collections.ObjectModel;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to display GSA node result contours
  /// </summary>
  public class NodeContourResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("742b1398-4eee-49e6-98d0-00afac6813e6");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result0D;

    public NodeContourResults() : base("Node Contour Results",
      "ContourNode",
      "Diplays GSA Node Results as Contours",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + Environment.NewLine +
          "Node list should take the form:" + Environment.NewLine +
          " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddColourParameter("Colour", "Co", "Optional list of colours to override default colours." +
          Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager.AddNumberParameter("Scalar", "x:X", "Scale the result display size", GH_ParamAccess.item, 10);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, LengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Result Point", "P", "Contoured Points with result values", GH_ParamAccess.list);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();
      this._case = "";
      this.resType = "";

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        #region Inputs
        if (gh_typ == null || gh_typ.Value == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input is null");
          return;
        }
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.CaseType.Combination && result.SelectedPermutationIDs.Count > 1)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Combination Case " + result.CaseID + " contains "
                + result.SelectedPermutationIDs.Count + " permutations - only one permutation can be displayed at a time." +
                Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
          }
          if (result.Type == GsaResult.CaseType.Combination)
            _case = "Case C" + result.CaseID + " P" + result.SelectedPermutationIDs[0];
          if (result.Type == GsaResult.CaseType.AnalysisCase)
            _case = "Case A" + result.CaseID + Environment.NewLine + result.CaseName;
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

        if (nodeList.ToLower() == "all" || nodeList == "")
          nodeList = "All";

        // Get colours
        List<GH_Colour> gh_Colours = new List<GH_Colour>();
        List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
        if (DA.GetDataList(2, gh_Colours))
        {
          for (int i = 0; i < gh_Colours.Count; i++)
          {
            Color color = new Color();
            GH_Convert.ToColor(gh_Colours[i], out color, GH_Conversion.Both);
            colors.Add(color);
          }
        }
        Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = Helpers.Graphics.Colours.Stress_Gradient(colors);

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
            Tuple<List<GsaResultsValues>, List<int>> nodedisp = result.NodeDisplacementValues(nodeList, this.LengthResultUnit);
            res = nodedisp.Item1[0];
            break;

          case FoldMode.Reaction:
            Tuple<List<GsaResultsValues>, List<int>> resultgetter = result.NodeReactionForceValues(nodeList, this.ForceUnit, this.MomentUnit);
            res = resultgetter.Item1[0];
            nodeList = string.Join(" ", resultgetter.Item2);
            break;

          case FoldMode.Footfall:
            FootfallResultType footfallType = (FootfallResultType)Enum.Parse(typeof(FootfallResultType), this.SelectedItems[1]);
            res = result.NodeFootfallValues(nodeList, footfallType);
            break;
        }

        // get geometry for display from results class
        ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes(nodeList);

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = this.LengthResultUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Reaction)
        {
          xyzunit = this.ForceUnit;
          xxyyzzunit = this.MomentUnit;
        }
        if (_mode == FoldMode.Footfall) 
        {
          xyzunit = RatioUnit.DecimalFraction;
          xxyyzzunit = AngleUnit.Radian;
          _disp = DisplayValue.X;
        }

        double dmax_x = res.dmax_x.As(xyzunit);
        double dmax_y = _mode == FoldMode.Footfall ? 0 : res.dmax_y.As(xyzunit);
        double dmax_z = _mode == FoldMode.Footfall ? 0 : res.dmax_z.As(xyzunit);
        double dmax_xyz = _mode == FoldMode.Footfall ? 0 : res.dmax_xyz.As(xyzunit);
        double dmin_x = _mode == FoldMode.Footfall ? 0 : res.dmin_x.As(xyzunit);
        double dmin_y = _mode == FoldMode.Footfall ? 0 : res.dmin_y.As(xyzunit);
        double dmin_z = _mode == FoldMode.Footfall ? 0 : res.dmin_z.As(xyzunit);
        double dmin_xyz = _mode == FoldMode.Footfall ? 0 : res.dmin_xyz.As(xyzunit);
        double dmax_xx = _mode == FoldMode.Footfall ? 0 : res.dmax_xx.As(xxyyzzunit);
        double dmax_yy = _mode == FoldMode.Footfall ? 0 : res.dmax_yy.As(xxyyzzunit);
        double dmax_zz = _mode == FoldMode.Footfall ? 0 : res.dmax_zz.As(xxyyzzunit);
        double dmax_xxyyzz = _mode == FoldMode.Footfall ? 0 : res.dmax_xxyyzz.As(xxyyzzunit);
        double dmin_xx =  _mode == FoldMode.Footfall ? 0 : res.dmin_xx.As(xxyyzzunit);
        double dmin_yy = _mode == FoldMode.Footfall ? 0 : res.dmin_yy.As(xxyyzzunit);
        double dmin_zz = _mode == FoldMode.Footfall ? 0 : res.dmin_zz.As(xxyyzzunit);
        double dmin_xxyyzz = _mode == FoldMode.Footfall ? 0 : res.dmin_xxyyzz.As(xxyyzzunit);

        #region Result point values
        // ### Coloured Result Points ###

        // round max and min to reasonable numbers
        double dmax = 0;
        double dmin = 0;
        switch (_disp)
        {
          case (DisplayValue.X):
            dmax = dmax_x;
            dmin = dmin_x;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Ux";
            else
              resType = "Reaction Force, Fx";
            break;
          case (DisplayValue.Y):
            dmax = dmax_y;
            dmin = dmin_y;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uy";
            else
              resType = "Reaction Force, Fy";
            break;
          case (DisplayValue.Z):
            dmax = dmax_z;
            dmin = dmin_z;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uz";
            else
              resType = "Reaction Force, Fz";
            break;
          case (DisplayValue.resXYZ):
            dmax = dmax_xyz;
            dmin = dmin_xyz;
            if (_mode == FoldMode.Displacement)
              resType = "Res. Trans., |U|";
            else
              resType = "Res. Rxn. Force, |F|";
            break;
          case (DisplayValue.XX):
            dmax = dmax_xx;
            dmin = dmin_xx;
            if (_mode == FoldMode.Displacement)
              resType = "Rotation, Rxx";
            else
              resType = "Reaction Moment, Mxx";
            break;
          case (DisplayValue.YY):
            dmax = dmax_yy;
            dmin = dmin_yy;
            if (_mode == FoldMode.Displacement)
              resType = "Rotation, Ryy";
            else
              resType = "Reaction Moment, Ryy";
            break;
          case (DisplayValue.ZZ):
            dmax = dmax_zz;
            dmin = dmin_zz;
            if (_mode == FoldMode.Displacement)
              resType = "Rotation, Rzz";
            else
              resType = "Reaction Moment, Rzz";
            break;
          case (DisplayValue.resXXYYZZ):
            dmax = dmax_xxyyzz;
            dmin = dmin_xxyyzz;
            if (_mode == FoldMode.Displacement)
              resType = "Res. Rot., |R|";
            else
              resType = "Res. Rxn. Mom., |M|";
            break;
        }
        if (_mode == FoldMode.Footfall)
          resType = "Response Factor [-]";

        List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];

        // Loop through nodes and set result colour into ResultPoint format
        ConcurrentDictionary<int, PointResultGoo> pts = new ConcurrentDictionary<int, PointResultGoo>();

        LengthUnit lengthUnit = result.Model.ModelUnit;
        this.undefinedModelLengthUnit = false;
        if (lengthUnit == LengthUnit.Undefined)
        {
          lengthUnit = this.LengthUnit;
          this.undefinedModelLengthUnit = true;
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in " + lengthUnit.ToString() + ". This can be changed by right-clicking the component -> 'Select Units'");
        }

        // Get nodes for point location and restraint check in case of reaction force
        ConcurrentDictionary<int, GsaNodeGoo> gsanodes = Helpers.Import.Nodes.GetNodeDictionary(nodes, lengthUnit);

        Parallel.ForEach(gsanodes, node =>
        {
          if (node.Value.Value != null)
          {
            int nodeID = node.Value.Value.Id;
            if (xyzResults.ContainsKey(nodeID))
            {
              if (!(dmin == 0 & dmax == 0))
              {
                // create deflection point
                Point3d def = new Point3d(node.Value.Value.Point);

                IQuantity t = null;
                switch (_mode)
                {
                  case FoldMode.Displacement:
                    Vector3d translation = new Vector3d(0, 0, 0);
                    // pick the right value to display
                    switch (_disp)
                    {
                      case (DisplayValue.X):
                        t = xyzResults[nodeID][0].X.ToUnit(this.LengthResultUnit);
                        translation.X = xyzResults[nodeID][0].X.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.Y):
                        t = xyzResults[nodeID][0].Y.ToUnit(this.LengthResultUnit);
                        translation.Y = xyzResults[nodeID][0].Y.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.Z):
                        t = xyzResults[nodeID][0].Z.ToUnit(this.LengthResultUnit);
                        translation.Z = xyzResults[nodeID][0].Z.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.resXYZ):
                        t = xyzResults[nodeID][0].XYZ.ToUnit(this.LengthResultUnit);
                        translation.X = xyzResults[nodeID][0].X.As(lengthUnit) * _defScale;
                        translation.Y = xyzResults[nodeID][0].Y.As(lengthUnit) * _defScale;
                        translation.Z = xyzResults[nodeID][0].Z.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.XX):
                        t = xxyyzzResults[nodeID][0].X.ToUnit(AngleUnit.Radian);
                        break;
                      case (DisplayValue.YY):
                        t = xxyyzzResults[nodeID][0].Y.ToUnit(AngleUnit.Radian);
                        break;
                      case (DisplayValue.ZZ):
                        t = xxyyzzResults[nodeID][0].Z.ToUnit(AngleUnit.Radian);
                        break;
                      case (DisplayValue.resXXYYZZ):
                        t = xxyyzzResults[nodeID][0].XYZ.ToUnit(AngleUnit.Radian);
                        break;
                    }
                    def.Transform(Transform.Translation(translation));
                    break;
                  case FoldMode.Reaction:
                    // pick the right value to display
                    switch (_disp)
                    {
                      case (DisplayValue.X):
                        t = xyzResults[nodeID][0].X.ToUnit(this.ForceUnit);
                        break;
                      case (DisplayValue.Y):
                        t = xyzResults[nodeID][0].Y.ToUnit(this.ForceUnit);
                        break;
                      case (DisplayValue.Z):
                        t = xyzResults[nodeID][0].Z.ToUnit(this.ForceUnit);
                        break;
                      case (DisplayValue.resXYZ):
                        t = xyzResults[nodeID][0].XYZ.ToUnit(this.ForceUnit);
                        break;
                      case (DisplayValue.XX):
                        t = xxyyzzResults[nodeID][0].X.ToUnit(this.MomentUnit);
                        break;
                      case (DisplayValue.YY):
                        t = xxyyzzResults[nodeID][0].Y.ToUnit(this.MomentUnit);
                        break;
                      case (DisplayValue.ZZ):
                        t = xxyyzzResults[nodeID][0].Z.ToUnit(this.MomentUnit);
                        break;
                      case (DisplayValue.resXXYYZZ):
                        t = xxyyzzResults[nodeID][0].XYZ.ToUnit(this.MomentUnit);
                        break;
                    }
                    break;
                  case FoldMode.Footfall:
                    t = xyzResults[nodeID][0].X.ToUnit(RatioUnit.DecimalFraction);
                    break;
                }

                //normalised value between -1 and 1
                double tnorm = 2 * (t.Value - dmin) / (dmax - dmin) - 1;

                // get colour for that normalised value
                Color valcol = gH_Gradient.ColourAt(tnorm);

                // set the size of the point for ResultPoint class. Size is calculated from 0-base, so not a normalised value between extremes
                float size = (t.Value >= 0 && dmax != 0) ?
                    Math.Max(2, (float)(t.Value / dmax * scale)) :
                    Math.Max(2, (float)(Math.Abs(t.Value) / Math.Abs(dmin) * scale));

                // add our special resultpoint to the list of points
                pts[nodeID] = new PointResultGoo(def, t, valcol, size);
              }
            }
          }
        });
        #endregion

        #region Legend
        // ### Legend ###
        // loop through number of grip points in gradient to create legend
        int gripheight = legend.Height / gH_Gradient.GripCount;
        legendValues = new List<string>();
        legendValuesPosY = new List<int>();

        //Find Colour and Values for legend output
        List<GH_UnitNumber> ts = new List<GH_UnitNumber>();
        List<Color> cs = new List<Color>();

        for (int i = 0; i < gH_Gradient.GripCount; i++)
        {
          double t = dmin + (dmax - dmin) / ((double)gH_Gradient.GripCount - 1) * (double)i;
          if (t > 1)
          {
            double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
            scl = Math.Max(scl, 1);
            t = scl * Math.Round(t / scl, 3);
          }
          else
            t = Math.Round(t, significantDigits);
          //t = ResultHelper.RoundToSignificantDigits(t, significantDigits);

          Color gradientcolour = gH_Gradient.ColourAt(2 * (double)i / ((double)gH_Gradient.GripCount - 1) - 1);
          cs.Add(gradientcolour);

          // create legend for viewport
          int starty = i * gripheight;
          int endy = starty + gripheight;
          for (int y = starty; y < endy; y++)
          {
            for (int x = 0; x < legend.Width; x++)
              legend.SetPixel(x, legend.Height - y - 1, gradientcolour);
          }
          if (_mode == FoldMode.Displacement)
          {
            if ((int)_disp < 4)
            {
              Length displacement = new Length(t, this.LengthResultUnit);
              legendValues.Add(displacement.ToString("f" + significantDigits));
              ts.Add(new GH_UnitNumber(displacement));
            }
            else
            {
              Angle rotation = new Angle(t, AngleUnit.Radian);
              legendValues.Add(rotation.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(rotation));
            }
          }
          if (_mode == FoldMode.Reaction)
          {
            if ((int)_disp < 4)
            {
              Force reactionForce = new Force(t, this.ForceUnit);
              legendValues.Add(reactionForce.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(reactionForce));
              this.Message = Force.GetAbbreviation(this.ForceUnit);
            }
            else
            {
              Moment reactionMoment = new Moment(t, this.MomentUnit);
              legendValues.Add(reactionMoment.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(reactionMoment));
              this.Message = Moment.GetAbbreviation(this.MomentUnit);
            }
          }
          if (_mode == FoldMode.Footfall)
          {
            Ratio responseFactor = new Ratio(t, RatioUnit.DecimalFraction);
            legendValues.Add(responseFactor.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(responseFactor));
            this.Message = "";
          }
          if (Math.Abs(t) > 1)
            legendValues[i] = legendValues[i].Replace(",", string.Empty); // remove thousand separator
          legendValuesPosY.Add(legend.Height - starty + gripheight / 2 - 2);
        }
        #endregion

        // set outputs
        DA.SetDataList(0, pts.OrderBy(x => x.Key).Select(y => y.Value).ToList());
        DA.SetDataList(1, cs);
        DA.SetDataList(2, ts);

        GsaResultsValues.ResultType resultType = _mode == FoldMode.Reaction ? GsaResultsValues.ResultType.Force : (GsaResultsValues.ResultType)Enum.Parse(typeof(GsaResultsValues.ResultType), _mode.ToString());
        Helpers.PostHogResultsHelper.PostHog(result.Type, 0, resultType, this._disp.ToString());
      }
    }

    #region Custom UI
    private enum FoldMode
    {
      Displacement,
      Reaction,
      Footfall
    }

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
    readonly List<string> _type = new List<string>(new string[]
    {
            "Displacement",
            "Reaction",
            "Footfall"
    });

    readonly List<string> _displacement = new List<string>(new string[]
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

    readonly List<string> _reaction = new List<string>(new string[]
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

    readonly List<string> _footfall = new List<string>(new string[]
    {
            "Resonant",
            "Transient"
    });

    double _minValue = 0;
    double _maxValue = 1000;
    double _defScale = 250;
    int _noDigits = 0;
    bool _slider = true;
    string _case = "";
    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    LengthUnit LengthResultUnit = DefaultUnits.LengthUnitResult;
    bool undefinedModelLengthUnit = false;
    ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    MomentUnit MomentUnit = DefaultUnits.MomentUnit;
    FoldMode _mode = FoldMode.Displacement;
    DisplayValue _disp = DisplayValue.resXYZ;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Result Type", "Component", "Deform Shape"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(this._type);
      this.SelectedItems.Add(this.DropDownItems[0][0]);

      // component
      this.DropDownItems.Add(this._displacement);
      this.SelectedItems.Add(this.DropDownItems[1][3]);

      this.IsInitialised = true;
    }
    public override void CreateAttributes()
    {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new OasysGH.UI.DropDownSliderComponentAttributes(this, SetSelected, this.DropDownItems, this.SelectedItems, this._slider, SetVal, SetMaxMin, this._defScale, this._maxValue, this._minValue, this._noDigits, this.SpacerDescriptions);
    }

    public override void SetSelected(int i, int j)
    {
      if (i == 0) // if change is made to first list
      {
        if (j == 0)
        {
          if (this.DropDownItems[1] != this._displacement)
          {
            this.DropDownItems[1] = _displacement;
            this.SelectedItems[0] = this.DropDownItems[0][0];
            this.SelectedItems[1] = this.DropDownItems[1][3];
            this.Mode1Clicked();
          }
        }
        if (j == 1)
        {
          if (this.DropDownItems[1] != this._reaction)
          {
            this.DropDownItems[1] = this._reaction;
            this.SelectedItems[0] = this.DropDownItems[0][1];
            this.SelectedItems[1] = this.DropDownItems[1][3];
            this.Mode2Clicked();
          }
        }
        if (j == 2)
        {
          if (this.DropDownItems[1] != this._footfall)
          {
            this.DropDownItems[1] = this._footfall;
            this.SelectedItems[0] = this.DropDownItems[0][2];
            this.SelectedItems[1] = this.DropDownItems[1][0];
            this.Mode3Clicked();
          }
        }
      }
      else if (i == 1)
      {
        this._disp = (DisplayValue)j;
        this.SelectedItems[1] = this.DropDownItems[1][j];
      }
      base.UpdateUI();
    }

    public void SetVal(double value)
    {
      this._defScale = value;
    }

    public void SetMaxMin(double max, double min)
    {
      this._maxValue = max;
      this._minValue = min;
    }

    public override void VariableParameterMaintenance()
    {
      if (this._mode == FoldMode.Displacement)
      {
        if ((int)_disp < 4)
          this.Params.Output[2].Name = "Values [" + Length.GetAbbreviation(this.LengthResultUnit) + "]";
        else
          this.Params.Output[2].Name = "Values [rad]";
      }

      if (this._mode == FoldMode.Reaction)
      {
        if ((int)this._disp < 4)
          this.Params.Output[2].Name = "Values [" + Force.GetAbbreviation(this.ForceUnit) + "]";
        else
          this.Params.Output[2].Name = "Values [" + Moment.GetAbbreviation(this.MomentUnit) + "]";
      }

      if (this._mode == FoldMode.Footfall)
        this.Params.Output[2].Name = "Values [-]";
    }
    #endregion

    #region menu override
    protected override void BeforeSolveInstance()
    {
      switch (this._mode)
      {
        case FoldMode.Displacement:
          if ((int)this._disp < 4)
            this.Message = Length.GetAbbreviation(this.LengthResultUnit);
          else
            this.Message = Angle.GetAbbreviation(AngleUnit.Radian);
          break;

        case FoldMode.Reaction:
          if ((int)this._disp < 4)
            this.Message = Force.GetAbbreviation(this.ForceUnit);
          else
            this.Message = Moment.GetAbbreviation(this.MomentUnit);
          break;

        case FoldMode.Footfall:
          this.Message = "";
          break;
      }
    }
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
      if (this._mode == FoldMode.Displacement)
        return;

      RecordUndoEvent(this._mode.ToString() + " Parameters");
      this._mode = FoldMode.Displacement;

      this._slider = true;
      this._defScale = 100;

      this.ReDrawComponent();
    }

    private void Mode2Clicked()
    {
      if (this._mode == FoldMode.Reaction)
        return;

      RecordUndoEvent(this._mode.ToString() + " Parameters");
      this._mode = FoldMode.Reaction;
      this._slider = false;
      this._defScale = 0;

      this.ReDrawComponent();
    }

    private void Mode3Clicked()
    {
      if (this._mode == FoldMode.Footfall)
        return;

      RecordUndoEvent(this._mode.ToString() + " Parameters");
      this._mode = FoldMode.Footfall;
      this._slider = false;
      this._defScale = 0;

      this.ReDrawComponent();
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _showLegend);

      Grasshopper.Kernel.Special.GH_GradientControl gradient = new Grasshopper.Kernel.Special.GH_GradientControl();
      gradient.CreateAttributes();
      ToolStripMenuItem extract = new ToolStripMenuItem("Extract Default Gradient", gradient.Icon_24x24, (s, e) => { CreateGradient(); });
      menu.Items.Add(extract);

      ToolStripMenuItem lengthUnitsMenu = new ToolStripMenuItem("Displacement");
      lengthUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateLength(unit); });
        toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthResultUnit);
        toolStripMenuItem.Enabled = true;
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem forceUnitsMenu = new ToolStripMenuItem("Force");
      forceUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateForce(unit); });
        toolStripMenuItem.Checked = unit == Force.GetAbbreviation(this.ForceUnit);
        toolStripMenuItem.Enabled = true;
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem momentUnitsMenu = new ToolStripMenuItem("Moment");
      momentUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateMoment(unit); });
        toolStripMenuItem.Checked = unit == Moment.GetAbbreviation(this.MomentUnit);
        toolStripMenuItem.Enabled = true;
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);

      if (undefinedModelLengthUnit)
      {
        ToolStripMenuItem modelUnitsMenu = new ToolStripMenuItem("Model geometry");
        modelUnitsMenu.Enabled = true;
        foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
        {
          ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateModel(unit); });
          toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthUnit);
          toolStripMenuItem.Enabled = true;
          modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);
        }
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { modelUnitsMenu, lengthUnitsMenu, forceUnitsMenu, momentUnitsMenu });
      }
      else
      {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { lengthUnitsMenu, forceUnitsMenu, momentUnitsMenu });
      }
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void UpdateModel(string unit)
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateLength(string unit)
    {
      this.LengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateForce(string unit)
    {
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateMoment(string unit)
    {
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }

    bool _showLegend = true;
    private void ShowLegend(object sender, EventArgs e)
    {
      _showLegend = !_showLegend;
      this.ExpirePreview(true);
    }

    private void CreateGradient()
    {
      Grasshopper.Kernel.Special.GH_GradientControl gradient = new Grasshopper.Kernel.Special.GH_GradientControl();
      gradient.CreateAttributes();

      gradient.Gradient = Helpers.Graphics.Colours.Stress_Gradient(null);
      gradient.Gradient.NormalizeGrips();
      gradient.Params.Input[0].AddVolatileData(new GH_Path(0), 0, -1);
      gradient.Params.Input[1].AddVolatileData(new GH_Path(0), 0, 1);
      gradient.Params.Input[2].AddVolatileDataList(
        new GH_Path(0),
        new List<double>() { -1, -0.666, -0.333, 0, 0.333, 0.666, 1 });

      gradient.Attributes.Pivot = new PointF(this.Attributes.Bounds.X - gradient.Attributes.Bounds.Width - 50, this.Params.Input[2].Attributes.Bounds.Y - gradient.Attributes.Bounds.Height / 4 - 6);

      Grasshopper.Instances.ActiveCanvas.Document.AddObject(gradient, false);
      this.Params.Input[2].RemoveAllSources();
      this.Params.Input[2].AddSource(gradient.Params.Output[0]);

      this.UpdateUI();
    }
    #endregion

    #region draw legend
    Bitmap legend = new Bitmap(15, 120);
    List<string> legendValues;
    List<int> legendValuesPosY;
    string resType;
    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);
      if (legendValues != null & _showLegend)
      {
        args.Display.DrawBitmap(new DisplayBitmap(legend), args.Viewport.Bounds.Right - 110, 20);
        for (int i = 0; i < legendValues.Count; i++)
          args.Display.Draw2dText(legendValues[i], Color.Black, new Point2d(args.Viewport.Bounds.Right - 85, legendValuesPosY[i]), false);
        args.Display.Draw2dText(resType, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 7), false);
        args.Display.Draw2dText(_case, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 145), false);
      }
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetBoolean("slider", _slider);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetBoolean("legend", _showLegend);
      writer.SetString("model", Length.GetAbbreviation(this.LengthUnit));
      writer.SetString("length", Length.GetAbbreviation(this.LengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(this.ForceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(this.MomentUnit));
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      _showLegend = reader.GetBoolean("legend");
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      this.LengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      return base.Read(reader);
    }
    #endregion
  }
}
