﻿using System;
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
using GsaGH.Util.Gsa;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;

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
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
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
      IQuantity length = new Length(0, LengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Point", "P", "Contoured Points with result values", GH_ParamAccess.list);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

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
          if (result.Type == GsaResult.ResultType.Combination && result.SelectedPermutationIDs.Count > 1)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Combination case contains "
                + result.SelectedPermutationIDs.Count + " - only one permutation can be displayed at a time." +
                System.Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
          }
          if (result.Type == GsaResult.ResultType.Combination)
            _case = "Case C" + result.CaseID + " P" + result.SelectedPermutationIDs[0];
          if (result.Type == GsaResult.ResultType.AnalysisCase)
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
        Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = UI.Colour.Stress_Gradient(colors);

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
        }

        // get geometry for display from results class
        ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(result.Model.Model.Nodes(nodeList));

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = this.LengthResultUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Reaction)
        {
          xyzunit = this.ForceUnit;
          xxyyzzunit = this.MomentUnit;
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

        List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];

        // Loop through nodes and set result colour into ResultPoint format
        ConcurrentDictionary<int, ResultPointGoo> pts = new ConcurrentDictionary<int, ResultPointGoo>();

        LengthUnit lengthUnit = result.Model.ModelUnit;
        this.undefinedModelLengthUnit = false;
        if (lengthUnit == LengthUnit.Undefined)
        {
          lengthUnit = this.LengthUnit;
          this.undefinedModelLengthUnit = true;
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in " + lengthUnit.ToString() + ". This can be changed by right-clicking the component -> 'Select Units'");
        }

        // Get nodes for point location and restraint check in case of reaction force
        ConcurrentDictionary<int, GsaNodeGoo> gsanodes = Util.Gsa.FromGSA.GetNodeDictionary(nodes, lengthUnit);

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
                        t = xyzResults[nodeID][0].X.As(this.LengthResultUnit);
                        translation.X = xyzResults[nodeID][0].X.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.Y):
                        t = xyzResults[nodeID][0].Y.As(this.LengthResultUnit);
                        translation.Y = xyzResults[nodeID][0].Y.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.Z):
                        t = xyzResults[nodeID][0].Z.As(this.LengthResultUnit);
                        translation.Z = xyzResults[nodeID][0].Z.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.resXYZ):
                        t = xyzResults[nodeID][0].XYZ.As(this.LengthResultUnit);
                        translation.X = xyzResults[nodeID][0].X.As(lengthUnit) * _defScale;
                        translation.Y = xyzResults[nodeID][0].Y.As(lengthUnit) * _defScale;
                        translation.Z = xyzResults[nodeID][0].Z.As(lengthUnit) * _defScale;
                        break;
                      case (DisplayValue.XX):
                        t = xxyyzzResults[nodeID][0].X.As(AngleUnit.Radian);
                        break;
                      case (DisplayValue.YY):
                        t = xxyyzzResults[nodeID][0].Y.As(AngleUnit.Radian);
                        break;
                      case (DisplayValue.ZZ):
                        t = xxyyzzResults[nodeID][0].Z.As(AngleUnit.Radian);
                        break;
                      case (DisplayValue.resXXYYZZ):
                        t = xxyyzzResults[nodeID][0].XYZ.As(AngleUnit.Radian);
                        break;
                    }
                    def.Transform(Transform.Translation(translation));
                    break;
                  case FoldMode.Reaction:
                    // pick the right value to display
                    switch (_disp)
                    {
                      case (DisplayValue.X):
                        t = xyzResults[nodeID][0].X.As(this.ForceUnit);
                        break;
                      case (DisplayValue.Y):
                        t = xyzResults[nodeID][0].Y.As(this.ForceUnit);
                        break;
                      case (DisplayValue.Z):
                        t = xyzResults[nodeID][0].Z.As(this.ForceUnit);
                        break;
                      case (DisplayValue.resXYZ):
                        t = xyzResults[nodeID][0].XYZ.As(this.ForceUnit);
                        break;
                      case (DisplayValue.XX):
                        t = xxyyzzResults[nodeID][0].X.As(this.MomentUnit);
                        break;
                      case (DisplayValue.YY):
                        t = xxyyzzResults[nodeID][0].Y.As(this.MomentUnit);
                        break;
                      case (DisplayValue.ZZ):
                        t = xxyyzzResults[nodeID][0].Z.As(this.MomentUnit);
                        break;
                      case (DisplayValue.resXXYYZZ):
                        t = xxyyzzResults[nodeID][0].XYZ.As(this.MomentUnit);
                        break;
                    }
                    break;
                }

                //normalised value between -1 and 1
                double tnorm = 2 * (t - dmin) / (dmax - dmin) - 1;

                // get colour for that normalised value
                Color valcol = gH_Gradient.ColourAt(tnorm);

                // set the size of the point for ResultPoint class. Size is calculated from 0-base, so not a normalised value between extremes
                float size = (t >= 0 && dmax != 0) ?
                    Math.Max(2, (float)(t / dmax * scale)) :
                    Math.Max(2, (float)(Math.Abs(t) / Math.Abs(dmin) * scale));

                // add our special resultpoint to the list of points
                pts[nodeID] = new ResultPointGoo(def, t, valcol, size);
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
          t = ResultHelper.RoundToSignificantDigits(t, significantDigits);

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
              this.Message = Length.GetAbbreviation(this.LengthResultUnit);
            }
            else
            {
              Angle rotation = new Angle(t, AngleUnit.Radian);
              legendValues.Add(rotation.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(rotation));
              this.Message = Angle.GetAbbreviation(AngleUnit.Radian);
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
          if (Math.Abs(t) > 1)
            legendValues[i] = legendValues[i].Replace(",", string.Empty); // remove thousand separator
          legendValuesPosY.Add(legend.Height - starty + gripheight / 2 - 2);
        }
        #endregion

        // set outputs
        DA.SetDataList(0, pts.OrderBy(x => x.Key).Select(y => y.Value).ToList());
        DA.SetDataList(1, cs);
        DA.SetDataList(2, ts);
      }
    }

    #region Custom UI
    private enum FoldMode
    {
      Displacement,
      Reaction
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
            "Reaction"
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

    readonly List<string> _force = new List<string>(new string[]
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

    public override void SetSelected(int dropdownlistidd, int selectedidd)
    {
      if (dropdownlistidd == 0) // if change is made to first list
      {
        if (selectedidd == 0)
        {
          if (DropDownItems[1] != _displacement)
          {
            DropDownItems[1] = _displacement;
            SelectedItems[0] = DropDownItems[0][0];
            SelectedItems[1] = DropDownItems[1][3];
            Mode1Clicked();
          }
        }
        if (selectedidd == 1)
        {
          if (DropDownItems[1] != _reaction)
          {
            DropDownItems[1] = _reaction;
            SelectedItems[0] = DropDownItems[0][1];
            SelectedItems[1] = DropDownItems[1][3];
            Mode2Clicked();
          }
        }
      }
      else if (dropdownlistidd == 1)
      {
        _disp = (DisplayValue)selectedidd;
        SelectedItems[1] = DropDownItems[1][selectedidd];
      }
      base.UpdateUI();
    }

    public void SetVal(double value)
    {
      _defScale = value;
    }

    public void SetMaxMin(double max, double min)
    {
      _maxValue = max;
      _minValue = min;
    }

    public override void VariableParameterMaintenance()
    {
      if (_mode == FoldMode.Displacement)
      {
        if ((int)_disp < 4)
          Params.Output[2].Name = "Values [" + Length.GetAbbreviation(this.LengthResultUnit) + "]";
        else
          Params.Output[2].Name = "Values [rad]";
      }

      if (_mode == FoldMode.Reaction)
      {
        if ((int)_disp < 4)
          Params.Output[2].Name = "Values [" + Force.GetAbbreviation(this.ForceUnit) + "]";
        else
          Params.Output[2].Name = "Values [" + Moment.GetAbbreviation(this.MomentUnit) + "]";
      }
    }
    #endregion

    #region menu override
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

      _slider = true;
      _defScale = 100;

      ReDrawComponent();
    }

    private void Mode2Clicked()
    {
      if (_mode == FoldMode.Reaction)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.Reaction;
      _slider = false;
      _defScale = 0;

      ReDrawComponent();
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

      gradient.Gradient = UI.Colour.Stress_Gradient(null);
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
