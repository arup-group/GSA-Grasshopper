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
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaApi;
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
  public class NodeContourResults : GH_OasysDropDownComponent {
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

    private enum FoldMode {
      Displacement,
      Reaction,
      Footfall,
    }

    public override Guid ComponentGuid => new Guid("742b1398-4eee-49e6-98d0-00afac6813e6");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Result0D;
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
    private readonly List<string> _footfall = new List<string>(new[] {
      "Resonant",
      "Transient",
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
    private readonly List<string> _type = new List<string>(new[] {
      "Displacement",
      "Reaction",
      "Footfall",
    });
    private string _case = "";
    private double _defScale = 250;
    private DisplayValue _disp = DisplayValue.ResXyz;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private Bitmap _legend = new Bitmap(15, 120);
    private List<string> _legendValues;
    private List<int> _legendValuesPosY;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private double _maxValue = 1000;
    private double _minValue;
    private FoldMode _mode = FoldMode.Displacement;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private int _noDigits;
    private string _resType;
    private bool _showLegend = true;
    private bool _slider = true;
    private bool _undefinedModelLengthUnit;

    public NodeContourResults() : base("Node Contour Results", "ContourNode",
      "Diplays GSA Node Results as Contours", CategoryName.Name(), SubCategoryName.Cat5()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownSliderComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, _slider, SetVal, SetMaxMin, _defScale, _maxValue, _minValue, _noDigits,
        _spacerDescriptions);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);
      if (!(_legendValues != null & _showLegend)) {
        return;
      }

      args.Display.DrawBitmap(new DisplayBitmap(_legend), args.Viewport.Bounds.Right - 110, 20);
      for (int i = 0; i < _legendValues.Count; i++) {
        args.Display.Draw2dText(_legendValues[i], Color.Black,
          new Point2d(args.Viewport.Bounds.Right - 85, _legendValuesPosY[i]), false);
      }

      args.Display.Draw2dText(_resType, Color.Black,
        new Point2d(args.Viewport.Bounds.Right - 110, 7), false);
      args.Display.Draw2dText(_case, Color.Black,
        new Point2d(args.Viewport.Bounds.Right - 110, 145), false);
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
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      return base.Read(reader);
    }

    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public override void SetSelected(int i, int j) {
      switch (i) {
        case 0: {
          switch (j) {
            case 0: {
              if (_dropDownItems[1] != _displacement) {
                _dropDownItems[1] = _displacement;
                _selectedItems[0] = _dropDownItems[0][0];
                _selectedItems[1] = _dropDownItems[1][3];
                Mode1Clicked();
              }

              break;
            }
            case 1: {
              if (_dropDownItems[1] != _reaction) {
                _dropDownItems[1] = _reaction;
                _selectedItems[0] = _dropDownItems[0][1];
                _selectedItems[1] = _dropDownItems[1][3];
                Mode2Clicked();
              }

              break;
            }
            case 2: {
              if (_dropDownItems[1] != _footfall) {
                _dropDownItems[1] = _footfall;
                _selectedItems[0] = _dropDownItems[0][2];
                _selectedItems[1] = _dropDownItems[1][0];
                Mode3Clicked();
              }

              break;
            }
          }

          break;
        }
        case 1:
          _disp = (DisplayValue)j;
          _selectedItems[1] = _dropDownItems[1][j];
          break;
      }

      base.UpdateUI();
    }

    public void SetVal(double value) {
      _defScale = value;
    }

    public override void VariableParameterMaintenance() {
      if (Params.Input.Count != 5) {
        var scale = (Param_Number)Params.Input[3];
        Params.UnregisterInputParameter(Params.Input[3], false);
        Params.RegisterInputParam(new Param_Interval());
        Params.Input[3].Name = "Min/Max Domain";
        Params.Input[3].NickName = "I";
        Params.Input[3].Description = "Opitonal Domain for custom Min to Max contour colours";
        Params.Input[3].Optional = true;
        Params.Input[3].Access = GH_ParamAccess.item;
        Params.RegisterInputParam(scale);
      }

      switch (_mode) {
        case FoldMode.Displacement when (int)_disp < 4:
          Params.Output[2].Name = "Values [" + Length.GetAbbreviation(_lengthResultUnit) + "]";
          break;

        case FoldMode.Displacement:
          Params.Output[2].Name = "Values [rad]";
          break;

        case FoldMode.Reaction when (int)_disp < 4:
          Params.Output[2].Name = "Values [" + Force.GetAbbreviation(_forceUnit) + "]";
          break;

        case FoldMode.Reaction:
          Params.Output[2].Name = "Values [" + Moment.GetAbbreviation(_momentUnit) + "]";
          break;

        case FoldMode.Footfall:
          Params.Output[2].Name = "Values [-]";
          break;
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetBoolean("slider", _slider);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetBoolean("legend", _showLegend);
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _showLegend);

      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();
      var extract = new ToolStripMenuItem("Extract Default Gradient", gradient.Icon_24x24,
        (s, e) => CreateGradient());
      menu.Items.Add(extract);

      var lengthUnitsMenu = new ToolStripMenuItem("Displacement") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateLength(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthResultUnit),
          Enabled = true,
        };
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var forceUnitsMenu = new ToolStripMenuItem("Force") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateForce(unit)) {
          Checked = unit == Force.GetAbbreviation(_forceUnit),
          Enabled = true,
        };
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var momentUnitsMenu = new ToolStripMenuItem("Moment") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateMoment(unit)) {
          Checked = unit == Moment.GetAbbreviation(_momentUnit),
          Enabled = true,
        };
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.Units);

      if (_undefinedModelLengthUnit) {
        var modelUnitsMenu = new ToolStripMenuItem("Model geometry") {
          Enabled = true,
        };
        foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
          var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateModel(unit)) {
            Checked = unit == Length.GetAbbreviation(_lengthUnit),
            Enabled = true,
          };
          modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);
        }

        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
          modelUnitsMenu,
          lengthUnitsMenu,
          forceUnitsMenu,
          momentUnitsMenu,
        });
      } else {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
          lengthUnitsMenu,
          forceUnitsMenu,
          momentUnitsMenu,
        });
      }

      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    protected override void BeforeSolveInstance() {
      switch (_mode) {
        case FoldMode.Displacement:
          Message = (int)_disp < 4 ? Length.GetAbbreviation(_lengthResultUnit) :
            Angle.GetAbbreviation(AngleUnit.Radian);
          break;

        case FoldMode.Reaction:
          Message = (int)_disp < 4 ? Force.GetAbbreviation(_forceUnit) :
            Moment.GetAbbreviation(_momentUnit);
          break;

        case FoldMode.Footfall:
          Message = "";
          break;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Result Type",
        "Component",
        "Deform Shape",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_type);
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(_displacement);
      _selectedItems.Add(_dropDownItems[1][3]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Node filter list", "No",
        "Filter results by list (by default 'all')" + Environment.NewLine
        + "Input a GSA List or a text string taking the form:" + Environment.NewLine
        + " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager.AddColourParameter("Colour", "Co",
        "Optional list of colours to override default colours." + Environment.NewLine
        + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[2].Optional = true;
      pManager.AddIntervalParameter("Min/Max Domain", "I",
        "Opitonal Domain for custom Min to Max contour colours", GH_ParamAccess.item);
      pManager[3].Optional = true;
      pManager.AddNumberParameter("Scalar", "x:X", "Scale the result display size",
        GH_ParamAccess.item, 10);
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      IQuantity length = new Length(0, _lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Result Point", "P", "Contoured Points with result values",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      _case = "";
      _resType = "";

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      #region Inputs

      switch (ghTyp?.Value) {
        case null:
          this.AddRuntimeWarning("Input is null");
          return;

        case GsaResultGoo goo: {
          result = goo.Value;
          if (result.Type == GsaResult.CaseType.Combination
            && result.SelectedPermutationIds.Count > 1) {
            this.AddRuntimeWarning("Combination Case " + result.CaseId + " contains "
              + result.SelectedPermutationIds.Count
              + " permutations - only one permutation can be displayed at a time."
              + Environment.NewLine
              + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
            _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
          }

          if (result.Type == GsaResult.CaseType.Combination) {
            _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
          }

          if (result.Type == GsaResult.CaseType.AnalysisCase) {
            _case = "Case A" + result.CaseId + Environment.NewLine + result.CaseName;
          }

          break;
        }
        default:
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
      }

      string nodeList = Inputs.GetNodeListNameForesults(this, da, 1);
      if (string.IsNullOrEmpty(nodeList)) { return; }

      var ghColours = new List<GH_Colour>();
      var colors = new List<Color>();
      if (da.GetDataList(2, ghColours)) {
        foreach (GH_Colour t in ghColours) {
          GH_Convert.ToColor(t, out Color color, GH_Conversion.Both);
          colors.Add(color);
        }
      }

      GH_Gradient ghGradient = Colours.Stress_Gradient(colors);

      var ghInterval = new GH_Interval();
      Interval customMinMax = Interval.Unset;
      if (da.GetData(3, ref ghInterval)) {
        GH_Convert.ToInterval(ghInterval, ref customMinMax, GH_Conversion.Both);
      }

      var ghScale = new GH_Number();
      da.GetData(4, ref ghScale);
      GH_Convert.ToDouble(ghScale, out double scale, GH_Conversion.Both);

      #endregion

      var res = new GsaResultsValues();
      switch (_mode) {
        case FoldMode.Displacement:
          Tuple<List<GsaResultsValues>, List<int>> nodedisp
            = result.NodeDisplacementValues(nodeList, _lengthResultUnit);
          res = nodedisp.Item1[0];
          break;

        case FoldMode.Reaction:
          Tuple<List<GsaResultsValues>, List<int>> resultgetter
            = result.NodeReactionForceValues(nodeList, _forceUnit, _momentUnit);
          res = resultgetter.Item1[0];
          nodeList = string.Join(" ", resultgetter.Item2);
          break;

        case FoldMode.Footfall:
          var footfallType
            = (FootfallResultType)Enum.Parse(typeof(FootfallResultType), _selectedItems[1]);
          res = result.NodeFootfallValues(nodeList, footfallType);
          break;
      }

      ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes(nodeList);

      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults
        = res.XyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults
        = res.XxyyzzResults;

      Enum xyzunit = _lengthResultUnit;
      Enum xxyyzzunit = AngleUnit.Radian;
      switch (_mode) {
        case FoldMode.Reaction:
          xyzunit = _forceUnit;
          xxyyzzunit = _momentUnit;
          break;

        case FoldMode.Footfall:
          xyzunit = RatioUnit.DecimalFraction;
          xxyyzzunit = AngleUnit.Radian;
          _disp = DisplayValue.X;
          break;
      }

      double dmaxX = res.DmaxX.As(xyzunit);
      double dmaxY = _mode == FoldMode.Footfall ? 0 : res.DmaxY.As(xyzunit);
      double dmaxZ = _mode == FoldMode.Footfall ? 0 : res.DmaxZ.As(xyzunit);
      double dmaxXyz = _mode == FoldMode.Footfall ? 0 : res.DmaxXyz.As(xyzunit);
      double dminX = _mode == FoldMode.Footfall ? 0 : res.DminX.As(xyzunit);
      double dminY = _mode == FoldMode.Footfall ? 0 : res.DminY.As(xyzunit);
      double dminZ = _mode == FoldMode.Footfall ? 0 : res.DminZ.As(xyzunit);
      double dminXyz = _mode == FoldMode.Footfall ? 0 : res.DminXyz.As(xyzunit);
      double dmaxXx = _mode == FoldMode.Footfall ? 0 : res.DmaxXx.As(xxyyzzunit);
      double dmaxYy = _mode == FoldMode.Footfall ? 0 : res.DmaxYy.As(xxyyzzunit);
      double dmaxZz = _mode == FoldMode.Footfall ? 0 : res.DmaxZz.As(xxyyzzunit);
      double dmaxXxyyzz = _mode == FoldMode.Footfall ? 0 : res.DmaxXxyyzz.As(xxyyzzunit);
      double dminXx = _mode == FoldMode.Footfall ? 0 : res.DminXx.As(xxyyzzunit);
      double dminYy = _mode == FoldMode.Footfall ? 0 : res.DminYy.As(xxyyzzunit);
      double dminZz = _mode == FoldMode.Footfall ? 0 : res.DminZz.As(xxyyzzunit);
      double dminXxyyzz = _mode == FoldMode.Footfall ? 0 : res.DminXxyyzz.As(xxyyzzunit);

      #region Result point values

      double dmax = 0;
      double dmin = 0;
      switch (_disp) {
        case DisplayValue.X:
          dmax = dmaxX;
          dmin = dminX;
          _resType = _mode == FoldMode.Displacement ? "Translation, Ux" : "Reaction Force, Fx";
          break;

        case DisplayValue.Y:
          dmax = dmaxY;
          dmin = dminY;
          _resType = _mode == FoldMode.Displacement ? "Translation, Uy" : "Reaction Force, Fy";
          break;

        case DisplayValue.Z:
          dmax = dmaxZ;
          dmin = dminZ;
          _resType = _mode == FoldMode.Displacement ? "Translation, Uz" : "Reaction Force, Fz";
          break;

        case DisplayValue.ResXyz:
          dmax = dmaxXyz;
          dmin = dminXyz;
          _resType = _mode == FoldMode.Displacement ? "Res. Trans., |U|" : "Res. Rxn. Force, |F|";
          break;

        case DisplayValue.Xx:
          dmax = dmaxXx;
          dmin = dminXx;
          _resType = _mode == FoldMode.Displacement ? "Rotation, Rxx" : "Reaction Moment, Mxx";
          break;

        case DisplayValue.Yy:
          dmax = dmaxYy;
          dmin = dminYy;
          _resType = _mode == FoldMode.Displacement ? "Rotation, Ryy" : "Reaction Moment, Ryy";
          break;

        case DisplayValue.Zz:
          dmax = dmaxZz;
          dmin = dminZz;
          _resType = _mode == FoldMode.Displacement ? "Rotation, Rzz" : "Reaction Moment, Rzz";
          break;

        case DisplayValue.ResXxyyzz:
          dmax = dmaxXxyyzz;
          dmin = dminXxyyzz;
          _resType = _mode == FoldMode.Displacement ? "Res. Rot., |R|" : "Res. Rxn. Mom., |M|";
          break;
      }

      if (_mode == FoldMode.Footfall) {
        _resType = "Response Factor [-]";
      }

      if (customMinMax != Interval.Unset) {
        dmin = customMinMax.Min;
        dmax = customMinMax.Max;
      }

      List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
      dmax = rounded[0];
      dmin = rounded[1];
      int significantDigits = (int)rounded[2];
      if (customMinMax != Interval.Unset) {
        dmin = customMinMax.Min;
        dmax = customMinMax.Max;
      }

      var pts = new ConcurrentDictionary<int, PointResultGoo>();
      LengthUnit lengthUnit = result.Model.ModelUnit;
      _undefinedModelLengthUnit = false;
      if (lengthUnit == LengthUnit.Undefined) {
        lengthUnit = _lengthUnit;
        _undefinedModelLengthUnit = true;
        this.AddRuntimeRemark(
          "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in "
          + lengthUnit.ToString()
          + ". This can be changed by right-clicking the component -> 'Select Units'");
      }

      ConcurrentDictionary<int, GsaNodeGoo> gsanodes = Nodes.GetNodeDictionary(nodes, lengthUnit);

      Parallel.ForEach(gsanodes, node => {
        if (node.Value.Value == null) {
          return;
        }

        int nodeId = node.Value.Value.Id;
        if (!xyzResults.ContainsKey(nodeId) || (dmin == 0 & dmax == 0)) {
          return;
        }

        var def = new Point3d(node.Value.Value.Point);

        IQuantity t = null;
        switch (_mode) {
          case FoldMode.Displacement:
            var translation = new Vector3d(0, 0, 0);
            switch (_disp) {
              case DisplayValue.X:
                t = xyzResults[nodeId][0].X.ToUnit(_lengthResultUnit);
                translation.X = xyzResults[nodeId][0].X.As(lengthUnit) * _defScale;
                break;

              case DisplayValue.Y:
                t = xyzResults[nodeId][0].Y.ToUnit(_lengthResultUnit);
                translation.Y = xyzResults[nodeId][0].Y.As(lengthUnit) * _defScale;
                break;

              case DisplayValue.Z:
                t = xyzResults[nodeId][0].Z.ToUnit(_lengthResultUnit);
                translation.Z = xyzResults[nodeId][0].Z.As(lengthUnit) * _defScale;
                break;

              case DisplayValue.ResXyz:
                t = xyzResults[nodeId][0].Xyz.ToUnit(_lengthResultUnit);
                translation.X = xyzResults[nodeId][0].X.As(lengthUnit) * _defScale;
                translation.Y = xyzResults[nodeId][0].Y.As(lengthUnit) * _defScale;
                translation.Z = xyzResults[nodeId][0].Z.As(lengthUnit) * _defScale;
                break;

              case DisplayValue.Xx:
                t = xxyyzzResults[nodeId][0].X.ToUnit(AngleUnit.Radian);
                break;

              case DisplayValue.Yy:
                t = xxyyzzResults[nodeId][0].Y.ToUnit(AngleUnit.Radian);
                break;

              case DisplayValue.Zz:
                t = xxyyzzResults[nodeId][0].Z.ToUnit(AngleUnit.Radian);
                break;

              case DisplayValue.ResXxyyzz:
                t = xxyyzzResults[nodeId][0].Xyz.ToUnit(AngleUnit.Radian);
                break;
            }

            def.Transform(Transform.Translation(translation));
            break;

          case FoldMode.Reaction:
            switch (_disp) {
              case DisplayValue.X:
                t = xyzResults[nodeId][0].X.ToUnit(_forceUnit);
                break;

              case DisplayValue.Y:
                t = xyzResults[nodeId][0].Y.ToUnit(_forceUnit);
                break;

              case DisplayValue.Z:
                t = xyzResults[nodeId][0].Z.ToUnit(_forceUnit);
                break;

              case DisplayValue.ResXyz:
                t = xyzResults[nodeId][0].Xyz.ToUnit(_forceUnit);
                break;

              case DisplayValue.Xx:
                t = xxyyzzResults[nodeId][0].X.ToUnit(_momentUnit);
                break;

              case DisplayValue.Yy:
                t = xxyyzzResults[nodeId][0].Y.ToUnit(_momentUnit);
                break;

              case DisplayValue.Zz:
                t = xxyyzzResults[nodeId][0].Z.ToUnit(_momentUnit);
                break;

              case DisplayValue.ResXxyyzz:
                t = xxyyzzResults[nodeId][0].Xyz.ToUnit(_momentUnit);
                break;
            }

            break;

          case FoldMode.Footfall:
            t = xyzResults[nodeId][0].X.ToUnit(RatioUnit.DecimalFraction);
            break;
        }

        double tnorm = (2 * (t.Value - dmin) / (dmax - dmin)) - 1;
        Color valcol = ghGradient.ColourAt(tnorm);
        float size = (t.Value >= 0 && dmax != 0) ? Math.Max(2, (float)(t.Value / dmax * scale)) :
          Math.Max(2, (float)(Math.Abs(t.Value) / Math.Abs(dmin) * scale));

        pts[nodeId] = new PointResultGoo(def, t, valcol, size, nodeId);
      });

      #endregion

      #region Legend

      int gripheight = _legend.Height / ghGradient.GripCount;
      _legendValues = new List<string>();
      _legendValuesPosY = new List<int>();

      var ts = new List<GH_UnitNumber>();
      var cs = new List<Color>();

      for (int i = 0; i < ghGradient.GripCount; i++) {
        double t = dmin + ((dmax - dmin) / ((double)ghGradient.GripCount - 1) * i);
        if (t > 1) {
          double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
          scl = Math.Max(scl, 1);
          t = scl * Math.Round(t / scl, 3);
        } else {
          t = Math.Round(t, significantDigits);
        }

        Color gradientcolour
          = ghGradient.ColourAt((2 * (double)i / ((double)ghGradient.GripCount - 1)) - 1);
        cs.Add(gradientcolour);

        int starty = i * gripheight;
        int endy = starty + gripheight;
        for (int y = starty; y < endy; y++) {
          for (int x = 0; x < _legend.Width; x++) {
            _legend.SetPixel(x, _legend.Height - y - 1, gradientcolour);
          }
        }

        switch (_mode) {
          case FoldMode.Displacement when (int)_disp < 4: {
            var displacement = new Length(t, _lengthResultUnit);
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
            var reactionForce = new Force(t, _forceUnit);
            _legendValues.Add(reactionForce.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(reactionForce));
            Message = Force.GetAbbreviation(_forceUnit);
            break;
          }
          case FoldMode.Reaction: {
            var reactionMoment = new Moment(t, _momentUnit);
            _legendValues.Add(reactionMoment.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(reactionMoment));
            Message = Moment.GetAbbreviation(_momentUnit);
            break;
          }
          case FoldMode.Footfall: {
            var responseFactor = new Ratio(t, RatioUnit.DecimalFraction);
            _legendValues.Add(responseFactor.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(responseFactor));
            Message = "";
            break;
          }
        }

        if (Math.Abs(t) > 1) {
          _legendValues[i] = _legendValues[i]
           .Replace(",", string.Empty); // remove thousand separator
        }

        _legendValuesPosY.Add(_legend.Height - starty + (gripheight / 2) - 2);
      }

      #endregion

      da.SetDataList(0, pts.OrderBy(x => x.Key).Select(y => y.Value).ToList());
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);

      GsaResultsValues.ResultType resultType = _mode == FoldMode.Reaction ?
        GsaResultsValues.ResultType.Force :
        (GsaResultsValues.ResultType)Enum.Parse(typeof(GsaResultsValues.ResultType),
          _mode.ToString());
      PostHog.Result(result.Type, 0, resultType, _disp.ToString());
    }

    private void CreateGradient() {
      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();

      gradient.Gradient = Colours.Stress_Gradient();
      gradient.Gradient.NormalizeGrips();
      gradient.Params.Input[0].AddVolatileData(new GH_Path(0), 0, -1);
      gradient.Params.Input[1].AddVolatileData(new GH_Path(0), 0, 1);
      gradient.Params.Input[2].AddVolatileDataList(new GH_Path(0), new List<double>() {
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
        Params.Input[2].Attributes.Bounds.Y - (gradient.Attributes.Bounds.Height / 4) - 6);

      Instances.ActiveCanvas.Document.AddObject(gradient, false);
      Params.Input[2].RemoveAllSources();
      Params.Input[2].AddSource(gradient.Params.Output[0]);

      UpdateUI();
    }

    private void Mode1Clicked() {
      if (_mode == FoldMode.Displacement) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Displacement;

      _slider = true;
      _defScale = 100;

      ReDrawComponent();
    }

    private void Mode2Clicked() {
      if (_mode == FoldMode.Reaction) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Reaction;
      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    private void Mode3Clicked() {
      if (_mode == FoldMode.Footfall) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Footfall;
      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    private void ReDrawComponent() {
      var pivot = new PointF(Attributes.Pivot.X, Attributes.Pivot.Y);
      CreateAttributes();
      Attributes.Pivot = pivot;
      Attributes.ExpireLayout();
      Attributes.PerformLayout();
    }

    private void ShowLegend(object sender, EventArgs e) {
      _showLegend = !_showLegend;
      ExpirePreview(true);
    }

    private void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateLength(string unit) {
      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateModel(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateMoment(string unit) {
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }
  }
}
