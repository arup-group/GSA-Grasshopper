using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI.Gradient;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaAPI;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to display GSA node result contours
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class NodeContourResults_OBSOLETE : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp))
        return;

      #region Inputs

      if (ghTyp.Value is GsaResultGoo goo) {
        result = goo.Value;
        switch (result.Type) {
          case GsaResult.CaseType.Combination when result.SelectedPermutationIds.Count > 1:
            this.AddRuntimeWarning("Combination case contains "
              + result.SelectedPermutationIds.Count
              + " - only one permutation can be displayed at a time."
              + Environment.NewLine
              + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
            _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
            break;
          case GsaResult.CaseType.Combination:
            _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
            break;
          case GsaResult.CaseType.AnalysisCase:
            _case = "Case A" + result.CaseId + Environment.NewLine + result.CaseName;
            break;
        }
      }
      else {
        this.AddRuntimeError("Error converting input to GSA Result");
        return;
      }

      string nodeList = "All";
      var ghNoList = new GH_String();
      if (da.GetData(1, ref ghNoList))
        if (GH_Convert.ToString(ghNoList, out string tempnodeList, GH_Conversion.Both))
          nodeList = tempnodeList;

      var ghColours = new List<GH_Colour>();
      var colors = new List<Color>();
      if (da.GetDataList(2, ghColours))
        foreach (GH_Colour t in ghColours) {
          GH_Convert.ToColor(t, out Color color, GH_Conversion.Both);
          colors.Add(color);
        }

      GH_Gradient ghGradient = Colours.Stress_Gradient(colors);

      var ghScale = new GH_Number();
      da.GetData(3, ref ghScale);
      GH_Convert.ToDouble(ghScale, out double scale, GH_Conversion.Both);

      #endregion

      var res = new GsaResultsValues();
      switch (_mode) {
        case FoldMode.Displacement:
          Tuple<List<GsaResultsValues>, List<int>> nodedisp
            = result.NodeDisplacementValues(nodeList, _lengthUnit);
          res = nodedisp.Item1[0];
          break;

        case FoldMode.Reaction:
          Tuple<List<GsaResultsValues>, List<int>> resultgetter
            = result.NodeReactionForceValues(nodeList,
              DefaultUnits.ForceUnit,
              DefaultUnits.MomentUnit);
          res = resultgetter.Item1[0];
          nodeList = string.Join(" ", resultgetter.Item2);
          break;
      }

      ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes(nodeList);

      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults
        = res.XyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults
        = res.XxyyzzResults;

      Enum xyzunit = _lengthUnit;
      Enum xxyyzzunit = AngleUnit.Radian;
      if (_mode == FoldMode.Reaction) {
        xyzunit = DefaultUnits.ForceUnit;
        xxyyzzunit = DefaultUnits.MomentUnit;
      }

      double dmaxX = res.DmaxX.As(xyzunit);
      double dmaxY = res.DmaxY.As(xyzunit);
      double dmaxZ = res.DmaxZ.As(xyzunit);
      double dmaxXyz = res.DmaxXyz.As(xyzunit);
      double dminX = res.DminX.As(xyzunit);
      double dminY = res.DminY.As(xyzunit);
      double dminZ = res.DminZ.As(xyzunit);
      double dminXyz = res.DminXyz.As(xyzunit);
      double dmaxXx = res.DmaxXx.As(xxyyzzunit);
      double dmaxYy = res.DmaxYy.As(xxyyzzunit);
      double dmaxZz = res.DmaxZz.As(xxyyzzunit);
      double dmaxXxyyzz = res.DmaxXxyyzz.As(xxyyzzunit);
      double dminXx = res.DminXx.As(xxyyzzunit);
      double dminYy = res.DminYy.As(xxyyzzunit);
      double dminZz = res.DminZz.As(xxyyzzunit);
      double dminXxyyzz = res.DminXxyyzz.As(xxyyzzunit);

      #region Result point values

      ConcurrentDictionary<int, GsaNodeGoo> gsanodes = Nodes.GetNodeDictionary(nodes, _lengthUnit);
      double dmax = 0;
      double dmin = 0;
      switch (_disp) {
        case (DisplayValue.X):
          dmax = dmaxX;
          dmin = dminX;
          _resType = _mode == FoldMode.Displacement
            ? "Translation, Ux"
            : "Reaction Force, Fx";
          break;
        case (DisplayValue.Y):
          dmax = dmaxY;
          dmin = dminY;
          _resType = _mode == FoldMode.Displacement
            ? "Translation, Uy"
            : "Reaction Force, Fy";
          break;
        case (DisplayValue.Z):
          dmax = dmaxZ;
          dmin = dminZ;
          _resType = _mode == FoldMode.Displacement
            ? "Translation, Uz"
            : "Reaction Force, Fz";
          break;
        case (DisplayValue.ResXyz):
          dmax = dmaxXyz;
          dmin = dminXyz;
          _resType = _mode == FoldMode.Displacement
            ? "Res. Trans., |U|"
            : "Res. Rxn. Force, |F|";
          break;
        case (DisplayValue.Xx):
          dmax = dmaxXx;
          dmin = dminXx;
          _resType = _mode == FoldMode.Displacement
            ? "Rotation, Rxx"
            : "Reaction Moment, Mxx";
          break;
        case (DisplayValue.Yy):
          dmax = dmaxYy;
          dmin = dminYy;
          _resType = _mode == FoldMode.Displacement
            ? "Rotation, Ryy"
            : "Reaction Moment, Ryy";
          break;
        case (DisplayValue.Zz):
          dmax = dmaxZz;
          dmin = dminZz;
          _resType = _mode == FoldMode.Displacement
            ? "Rotation, Rzz"
            : "Reaction Moment, Rzz";
          break;
        case (DisplayValue.ResXxyyzz):
          dmax = dmaxXxyyzz;
          dmin = dminXxyyzz;
          _resType = _mode == FoldMode.Displacement
            ? "Res. Rot., |R|"
            : "Res. Rxn. Mom., |M|";
          break;
      }

      List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
      dmax = rounded[0];
      dmin = rounded[1];
      int significantDigits = (int)rounded[2];

      var pointResultGoos = new ConcurrentDictionary<int, PointResultGoo>();

      Parallel.ForEach(gsanodes,
        node => {
          if (node.Value.Value == null)
            return;

          int nodeId = node.Value.Value.Id;
          if (!xyzResults.ContainsKey(nodeId))
            return;

          if (dmin == 0 & dmax == 0)
            return;

          var def = new Point3d(node.Value.Value.Point);
          IQuantity t = null;
          switch (_mode) {
            case FoldMode.Displacement:
              var translation = new Vector3d(0, 0, 0);
              switch (_disp) {
                case (DisplayValue.X):
                  t = xyzResults[nodeId][0]
                    .X.ToUnit(_lengthUnit);
                  translation.X = t.Value * _defScale;
                  break;
                case (DisplayValue.Y):
                  t = xyzResults[nodeId][0]
                    .Y.ToUnit(_lengthUnit);
                  translation.Y = t.Value * _defScale;
                  break;
                case (DisplayValue.Z):
                  t = xyzResults[nodeId][0]
                    .Z.ToUnit(_lengthUnit);
                  translation.Z = t.Value * _defScale;
                  break;
                case (DisplayValue.ResXyz):
                  t = xyzResults[nodeId][0]
                    .Xyz.ToUnit(_lengthUnit);
                  translation.X = xyzResults[nodeId][0]
                      .X.As(_lengthUnit)
                    * _defScale;
                  translation.Y = xyzResults[nodeId][0]
                      .Y.As(_lengthUnit)
                    * _defScale;
                  translation.Z = xyzResults[nodeId][0]
                      .Z.As(_lengthUnit)
                    * _defScale;
                  break;
                case (DisplayValue.Xx):
                  t = xxyyzzResults[nodeId][0]
                    .X.ToUnit(AngleUnit.Radian);
                  break;
                case (DisplayValue.Yy):
                  t = xxyyzzResults[nodeId][0]
                    .Y.ToUnit(AngleUnit.Radian);
                  break;
                case (DisplayValue.Zz):
                  t = xxyyzzResults[nodeId][0]
                    .Z.ToUnit(AngleUnit.Radian);
                  break;
                case (DisplayValue.ResXxyyzz):
                  t = xxyyzzResults[nodeId][0]
                    .Xyz.ToUnit(AngleUnit.Radian);
                  break;
              }

              def.Transform(Transform.Translation(translation));
              break;
            case FoldMode.Reaction:
              switch (_disp) {
                case (DisplayValue.X):
                  t = xyzResults[nodeId][0]
                    .X.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.Y):
                  t = xyzResults[nodeId][0]
                    .Y.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.Z):
                  t = xyzResults[nodeId][0]
                    .Z.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.ResXyz):
                  t = xyzResults[nodeId][0]
                    .Xyz.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.Xx):
                  t = xxyyzzResults[nodeId][0]
                    .X.ToUnit(DefaultUnits.MomentUnit);
                  break;
                case (DisplayValue.Yy):
                  t = xxyyzzResults[nodeId][0]
                    .Y.ToUnit(DefaultUnits.MomentUnit);
                  break;
                case (DisplayValue.Zz):
                  t = xxyyzzResults[nodeId][0]
                    .Z.ToUnit(DefaultUnits.MomentUnit);
                  break;
                case (DisplayValue.ResXxyyzz):
                  t = xxyyzzResults[nodeId][0]
                    .Xyz.ToUnit(DefaultUnits.MomentUnit);
                  break;
              }

              break;
          }

          double tnorm = 2 * (t.Value - dmin) / (dmax - dmin) - 1;
          Color valcol = ghGradient.ColourAt(tnorm);
          float size = (t.Value >= 0 && dmax != 0)
            ? Math.Max(2, (float)(t.Value / dmax * scale))
            : Math.Max(2, (float)(Math.Abs(t.Value) / Math.Abs(dmin) * scale));

          pointResultGoos[nodeId] = new PointResultGoo(def, t, valcol, size, nodeId);
        });

      #endregion

      #region Legend

      int gripheight = _legend.Height / ghGradient.GripCount;
      _legendValues = new List<string>();
      _legendValuesPosY = new List<int>();

      var ts = new List<GH_UnitNumber>();
      var cs = new List<Color>();

      for (int i = 0; i < ghGradient.GripCount; i++) {
        double t = dmin + (dmax - dmin) / ((double)ghGradient.GripCount - 1) * i;
        t = ResultHelper.RoundToSignificantDigits(t, significantDigits);

        Color gradientcolour
          = ghGradient.ColourAt(2 * (double)i / ((double)ghGradient.GripCount - 1) - 1);
        cs.Add(gradientcolour);

        int starty = i * gripheight;
        int endy = starty + gripheight;
        for (int y = starty; y < endy; y++)
          for (int x = 0; x < _legend.Width; x++)
            _legend.SetPixel(x, _legend.Height - y - 1, gradientcolour);
        switch (_mode) {
          case FoldMode.Displacement when (int)_disp < 4: {
              Length displacement = new Length(t, _lengthUnit).ToUnit(_lengthResultUnit);
              _legendValues.Add(displacement.ToString("f" + significantDigits));
              ts.Add(new GH_UnitNumber(displacement));
              break;
            }
          case FoldMode.Displacement: {
              var rotation = new Angle(t, AngleUnit.Radian);
              _legendValues.Add(rotation.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(rotation));
              break;
            }
          case FoldMode.Reaction when (int)_disp < 4: {
              var reactionForce = new Force(t, DefaultUnits.ForceUnit);
              _legendValues.Add(reactionForce.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(reactionForce));
              break;
            }
          case FoldMode.Reaction: {
              var reactionMoment = new Moment(t, DefaultUnits.MomentUnit);
              _legendValues.Add(t.ToString("F" + significantDigits)
                + " "
                + Moment.GetAbbreviation(DefaultUnits.MomentUnit));
              ts.Add(new GH_UnitNumber(reactionMoment));
              break;
            }
        }

        if (Math.Abs(t) > 1)
          _legendValues[i] = _legendValues[i]
            .Replace(",", string.Empty); // remove thousand separator
        _legendValuesPosY.Add(_legend.Height - starty + gripheight / 2 - 2);
      }

      #endregion

      da.SetDataList(0,
        pointResultGoos.OrderBy(x => x.Key)
          .Select(y => y.Value)
          .ToList());
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("47053884-2c22-4f2c-b092-8531fa5751e1");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Result0D;

    public NodeContourResults_OBSOLETE() : base("Node Contour Results",
      "ContourNode",
      "Diplays GSA Node Results as Contours",
      CategoryName.Name(),
      SubCategoryName.Cat5()) { }

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(),
        "Result",
        "Res",
        "GSA Result",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list",
        "No",
        "Filter results by list."
        + Environment.NewLine
        + "Node list should take the form:"
        + Environment.NewLine
        + " 1 11 to 72 step 2 not (XY3 31 to 45)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item,
        "All");
      pManager.AddColourParameter("Colour",
        "Co",
        "Optional list of colours to override default colours."
        + Environment.NewLine
        + "A new gradient will be created from the input list of colours",
        GH_ParamAccess.list);
      pManager.AddNumberParameter("Scalar",
        "x:X",
        "Scale the result display size",
        GH_ParamAccess.item,
        10);
      pManager[1]
        .Optional = true;
      pManager[2]
        .Optional = true;
      pManager[3]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      IQuantity length = new Length(0, _lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString()
        .Where(char.IsLetter));

      pManager.AddGenericParameter("Point",
        "P",
        "Contoured Points with result values",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]",
        "LT",
        "Legend Values",
        GH_ParamAccess.list);
    }

    #endregion

    #region Custom UI

    private enum FoldMode {
      Displacement,
      Reaction,
    }

    private enum DisplayValue {
      X,
      Y,
      Z,
      ResXyz,
      Xx,
      Yy,
      Zz,
      ResXxyyzz,
    }

    private readonly List<string> _type = new List<string>(new[] {
      "Displacement",
      "Reaction",
    });

    private readonly List<string> _displacement = new List<string>(new[] {
      "Translation Ux",
      "Translation Uy",
      "Translation Uz",
      "Resolved |U|",
      "Rotation Rxx",
      "Rotation Ryy",
      "Rotation Rzz",
      "Resolved |R|",
    });

    private readonly List<string> _reaction = new List<string>(new[] {
      "Reaction Fx",
      "Reaction Fy",
      "Reaction Fz",
      "Resolved |F|",
      "Reaction Mxx",
      "Reaction Myy",
      "Reaction Mzz",
      "Resolved |M|",
    });

    private double _minValue;
    private double _maxValue = 1000;
    private double _defScale = 250;
    private int _noDigits;
    private bool _slider = true;
    private string _case = "";
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private FoldMode _mode = FoldMode.Displacement;
    private DisplayValue _disp = DisplayValue.ResXyz;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Result Type",
        "Component",
        "Geometry Unit",
        "Deform Shape",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(_type);
      SelectedItems.Add(DropDownItems[0][0]);

      DropDownItems.Add(_displacement);
      SelectedItems.Add(DropDownItems[1][3]);

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void CreateAttributes() {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new DropDownSliderComponentAttributes(this,
        SetSelected,
        DropDownItems,
        SelectedItems,
        _slider,
        SetVal,
        SetMaxMin,
        _defScale,
        _maxValue,
        _minValue,
        _noDigits,
        SpacerDescriptions);
    }

    public override void SetSelected(int dropdownlistidd, int selectedidd) {
      switch (dropdownlistidd) {
        case 0: {
            switch (selectedidd) {
              case 0: {
                  if (DropDownItems[1] != _displacement) {
                    DropDownItems[1] = _displacement;
                    SelectedItems[0] = DropDownItems[0][0];
                    SelectedItems[1] = DropDownItems[1][3];
                    Mode1Clicked();
                  }

                  break;
                }
              case 1: {
                  if (DropDownItems[1] != _reaction) {
                    DropDownItems[1] = _reaction;
                    SelectedItems[0] = DropDownItems[0][1];
                    SelectedItems[1] = DropDownItems[1][3];
                    Mode2Clicked();
                  }

                  break;
                }
            }

            break;
          }
        case 1:
          _disp = (DisplayValue)selectedidd;
          SelectedItems[1] = DropDownItems[1][selectedidd];
          break;
        default:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[2]);
          break;
      }

      base.UpdateUI();
    }

    public void SetVal(double value) => _defScale = value;

    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      switch (_mode) {
        case FoldMode.Displacement when (int)_disp < 4: {
            IQuantity length = new Length(0, _lengthUnit);
            string lengthunitAbbreviation = string.Concat(length.ToString()
              .Where(char.IsLetter));
            Params.Output[2]
              .Name = "Values [" + lengthunitAbbreviation + "]";
            break;
          }
        case FoldMode.Displacement:
          Params.Output[2]
            .Name = "Values [rad]";
          break;
        case FoldMode.Reaction when (int)_disp < 4: {
            IQuantity force = new Force(0, DefaultUnits.ForceUnit);
            string forceunitAbbreviation = string.Concat(force.ToString()
              .Where(char.IsLetter));
            Params.Output[2]
              .Name = "Values [" + forceunitAbbreviation + "]";
            break;
          }
        case FoldMode.Reaction: {
            string momentunitAbbreviation = Moment.GetAbbreviation(DefaultUnits.MomentUnit);
            Params.Output[2]
              .Name = "Values [" + momentunitAbbreviation + "]";
            break;
          }
      }
    }

    #endregion

    #region menu override

    private void ReDrawComponent() {
      var pivot = new PointF(Attributes.Pivot.X, Attributes.Pivot.Y);
      CreateAttributes();
      Attributes.Pivot = pivot;
      Attributes.ExpireLayout();
      Attributes.PerformLayout();
    }

    private void Mode1Clicked() {
      if (_mode == FoldMode.Displacement)
        return;

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Displacement;

      _slider = true;
      _defScale = 100;

      ReDrawComponent();
    }

    private void Mode2Clicked() {
      if (_mode == FoldMode.Reaction)
        return;

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Reaction;
      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _showLegend);

      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();
      var extract = new ToolStripMenuItem("Extract Default Gradient",
        gradient.Icon_24x24,
        (s, e) => CreateGradient());
      menu.Items.Add(extract);
      Menu_AppendSeparator(menu);
    }

    private bool _showLegend = true;

    private void ShowLegend(object sender, EventArgs e) {
      _showLegend = !_showLegend;
      ExpirePreview(true);
    }

    private void CreateGradient() {
      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();

      gradient.Gradient = Colours.Stress_Gradient(null);
      gradient.Gradient.NormalizeGrips();
      gradient.Params.Input[0]
        .AddVolatileData(new GH_Path(0), 0, -1);
      gradient.Params.Input[1]
        .AddVolatileData(new GH_Path(0), 0, 1);
      gradient.Params.Input[2]
        .AddVolatileDataList(new GH_Path(0),
          new List<double>() {
            -1,
            -0.666,
            -0.333,
            0,
            0.333,
            0.666,
            1,
          });

      gradient.Attributes.Pivot = new PointF(
        Attributes.Bounds.X - gradient.Attributes.Bounds.Width - 50,
        Params.Input[2]
          .Attributes.Bounds.Y
        - gradient.Attributes.Bounds.Height / 4
        - 6);

      Instances.ActiveCanvas.Document.AddObject(gradient, false);
      Params.Input[2]
        .RemoveAllSources();
      Params.Input[2]
        .AddSource(gradient.Params.Output[0]);

      UpdateUI();
    }

    #endregion

    #region draw _legend

    private readonly Bitmap _legend = new Bitmap(15, 120);
    private List<string> _legendValues;
    private List<int> _legendValuesPosY;
    private string _resType;

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);
      if (!(_legendValues != null & _showLegend))
        return;

      args.Display.DrawBitmap(new DisplayBitmap(_legend), args.Viewport.Bounds.Right - 110, 20);
      for (int i = 0; i < _legendValues.Count; i++)
        args.Display.Draw2dText(_legendValues[i],
          Color.Black,
          new Point2d(args.Viewport.Bounds.Right - 85, _legendValuesPosY[i]),
          false);
      args.Display.Draw2dText(_resType,
        Color.Black,
        new Point2d(args.Viewport.Bounds.Right - 110, 7),
        false);
      args.Display.Draw2dText(_case,
        Color.Black,
        new Point2d(args.Viewport.Bounds.Right - 110, 145),
        false);
    }

    #endregion

    #region (de)serialization

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetBoolean("slider", _slider);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetBoolean("legend", _showLegend);
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      _showLegend = reader.GetBoolean("legend");
      return base.Read(reader);
    }

    #endregion
  }
}
