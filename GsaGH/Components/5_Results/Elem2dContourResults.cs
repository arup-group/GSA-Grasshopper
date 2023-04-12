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
  ///   Component to get Element2d results
  /// </summary>
  public class Elem2dContourResults : GH_OasysDropDownComponent {
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
      Force,
      Stress,
      Footfall,
    }

    public override Guid ComponentGuid => new Guid("e2b011dc-c5ca-46fd-87f5-b888b27ef684");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Result2D;
    private readonly List<string> _displacement = new List<string>(new[] {
      "Translation Ux",
      "Translation Uy",
      "Translation Uz",
      "Resolved |U|",
    });
    private readonly List<string> _footfall = new List<string>(new[] {
      "Resonant",
      "Transient",
    });
    private readonly List<string> _force = new List<string>(new[] {
      "Force Nx",
      "Force Ny",
      "Force Nxy",
      "Shear Qx",
      "Shear Qy",
      "Moment Mx",
      "Moment My",
      "Moment Mxy",
      "Wood-Armer M*x",
      "Wood-Armer M*y",
    });
    private readonly List<string> _layer = new List<string>(new[] {
      "Top",
      "Middle",
      "Bottom",
    });
    private readonly List<string> _stress = new List<string>(new[] {
      "Stress xx",
      "Stress yy",
      "Stress zz",
      "Stress xy",
      "Stress yz",
      "Stress zx",
    });
    private readonly List<string> _type = new List<string>(new[] {
      "Displacement",
      "Force",
      "Stress",
      "Footfall",
    });
    private string _case = "";
    private double _defScale = 250;
    private DisplayValue _disp = DisplayValue.ResXyz;
    private int _flayer;
    private ForcePerLengthUnit _forcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private bool _isShear;
    private Bitmap _legend = new Bitmap(15, 120);
    private List<string> _legendValues;
    private List<int> _legendValuesPosY;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private double _maxValue = 1000;
    private double _minValue;
    private FoldMode _mode = FoldMode.Displacement;
    private int _noDigits;
    private string _resType;
    private bool _showLegend = true;
    private bool _slider = true;
    private PressureUnit _stressUnitResult = DefaultUnits.StressUnitResult;
    private bool _undefinedModelLengthUnit;

    public Elem2dContourResults() : base("2D Contour Results",
                                                                                                                      "ContourElem2d",
      "Displays GSA 2D Element Results as Contour",
      CategoryName.Name(),
      SubCategoryName.Cat5()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownSliderComponentAttributes(this,
        SetSelected,
        _dropDownItems,
        _selectedItems,
        _slider,
        SetVal,
        SetMaxMin,
        _defScale,
        _maxValue,
        _minValue,
        _noDigits,
        _spacerDescriptions);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);
      if (!(_legendValues != null & _showLegend)) {
        return;
      }

      args.Display.DrawBitmap(new DisplayBitmap(_legend), args.Viewport.Bounds.Right - 110, 20);
      for (int i = 0; i < _legendValues.Count; i++) {
        args.Display.Draw2dText(_legendValues[i],
          Color.Black,
          new Point2d(args.Viewport.Bounds.Right - 85, _legendValuesPosY[i]),
          false);
      }

      args.Display.Draw2dText(_resType,
        Color.Black,
        new Point2d(args.Viewport.Bounds.Right - 110, 7),
        false);
      args.Display.Draw2dText(_case,
        Color.Black,
        new Point2d(args.Viewport.Bounds.Right - 110, 145),
        false);
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _flayer = reader.GetInt32("flayer");
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      _showLegend = reader.GetBoolean("legend");

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forcePerLengthUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit),
          reader.GetString("force"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("moment"));
      _stressUnitResult
        = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), reader.GetString("stress"));
      return base.Read(reader);
    }

    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 0: {
            switch (j) {
              case 0: {
                  if (_dropDownItems[1] != _displacement) {
                    while (
                      _dropDownItems.Count > 2) // if coming from stress we remove the layer dropdown
                    {
                      _dropDownItems.RemoveAt(2);
                      _selectedItems.RemoveAt(2);
                      _spacerDescriptions.RemoveAt(2);
                    }

                    _dropDownItems[1] = _displacement;
                    _selectedItems[1] = _dropDownItems[1][3]; // Resolved XYZ

                    _disp = (DisplayValue)3;
                    _isShear = false;
                    _flayer = 0;
                    Mode1Clicked();
                  }

                  break;
                }
              case 1: {
                  if (_dropDownItems[1] != _force) {
                    while (
                      _dropDownItems.Count > 2) // if coming from stress we remove the layer dropdown
                    {
                      _dropDownItems.RemoveAt(2);
                      _selectedItems.RemoveAt(2);
                      _spacerDescriptions.RemoveAt(2);
                    }

                    _dropDownItems[1] = _force;
                    _selectedItems[1] = _dropDownItems[1][0];

                    _disp = 0;
                    _isShear = false;
                    _flayer = 0;
                    Mode2Clicked();
                  }

                  break;
                }
              case 2: {
                  if (_dropDownItems[1] != _stress) {
                    if (_dropDownItems.Count < 3) {
                      _dropDownItems.Insert(2, _layer); //insert layer dropdown as third dd list
                      _spacerDescriptions.Insert(2, "Layer");
                    }

                    _dropDownItems[1] = _stress;
                    _selectedItems[1] = _dropDownItems[1][0];

                    if (_selectedItems.Count < 3) {
                      _selectedItems.Insert(2, _dropDownItems[2][1]);
                    }
                    else {
                      _selectedItems[2] = _dropDownItems[2][1];
                    }

                    _disp = 0;
                    _isShear = false;
                    Mode4Clicked();
                  }

                  break;
                }
              case 3: {
                  if (_dropDownItems[1] != _footfall) {
                    while (
                      _dropDownItems.Count > 2) // if coming from stress we remove the layer dropdown
                    {
                      _dropDownItems.RemoveAt(2);
                      _selectedItems.RemoveAt(2);
                      _spacerDescriptions.RemoveAt(2);
                    }

                    _dropDownItems[1] = _footfall;
                    _selectedItems[1] = _dropDownItems[1][0];

                    _disp = 0;
                    _isShear = false;
                    _flayer = 0;
                    Mode5Clicked();
                  }

                  break;
                }
            }

            break;
          }
        case 1: {
            bool redraw = false;
            _selectedItems[1] = _dropDownItems[1][j];
            if (_mode == FoldMode.Displacement) {
              if ((int)_disp > 3 & j < 4) {
                redraw = true;
                _slider = true;
              }

              if ((int)_disp < 4 & j > 3) {
                redraw = true;
                _slider = false;
              }
            }

            _disp = (DisplayValue)j;
            if (_dropDownItems[1] != _displacement) {
              _isShear = false;
              if (_mode == FoldMode.Force) {
                if (j == 3 | j == 4) {
                  _disp = (DisplayValue)j - 3;
                  _isShear = true;
                }
                else if (j > 4) {
                  _disp = (DisplayValue)j - 1;
                }

                switch (j) {
                  case 8:
                    _disp = DisplayValue.ResXyz;
                    break;

                  case 9:
                    _disp = DisplayValue.ResXxyyzz;
                    break;
                }
              }
              else if (_mode == FoldMode.Force || _mode == FoldMode.Stress) {
                if (j > 2) {
                  _disp = (DisplayValue)j + 1;
                }
              }
            }

            if (redraw) {
              ReDrawComponent();
            }

            break;
          }
        case 2 when _mode == FoldMode.Stress: {
            switch (j) {
              case 0:
                _flayer = 1;
                break;

              case 1:
                _flayer = 0;
                break;

              case 2:
                _flayer = -1;
                break;
            }

            break;
          }
      }

      base.UpdateUI();
    }

    public void SetVal(double value) {
      _defScale = value;
    }

    public override void VariableParameterMaintenance() {
      if (Params.Input.Count != 4) {
        Params.RegisterInputParam(new Param_Interval());
        Params.Input[3]
          .Name = "Min/Max Domain";
        Params.Input[3]
          .NickName = "I";
        Params.Input[3]
          .Description = "Opitonal Domain for custom Min to Max contour colours";
        Params.Input[3]
          .Optional = true;
        Params.Input[3]
          .Access = GH_ParamAccess.item;
      }

      switch (_mode) {
        case FoldMode.Displacement when (int)_disp < 4:
          Params.Output[2]
            .Name = "Values [" + Length.GetAbbreviation(_lengthResultUnit) + "]";
          break;

        case FoldMode.Displacement:
          Params.Output[2]
            .Name = "Values [rad]";
          break;

        case FoldMode.Force when (int)_disp < 4 | _isShear:
          Params.Output[2]
              .Name = "Legend Values ["
            + ForcePerLength.GetAbbreviation(_forcePerLengthUnit)
            + "/"
            + Length.GetAbbreviation(_lengthUnit)
            + "]";
          break;

        case FoldMode.Force:
          Params.Output[2]
              .Name = "Legend Values ["
            + Force.GetAbbreviation(_forceUnit)
            + "·"
            + Length.GetAbbreviation(_lengthUnit)
            + "/"
            + Length.GetAbbreviation(_lengthUnit)
            + "]";
          break;

        case FoldMode.Stress:
          Params.Output[2]
            .Name = "Legend Values [" + Pressure.GetAbbreviation(_stressUnitResult) + "]";
          break;

        case FoldMode.Footfall:
          Params.Output[2]
            .Name = "Legend Values [-]";
          break;
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetInt32("flayer", _flayer);
      writer.SetBoolean("slider", _slider);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetBoolean("legend", _showLegend);
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", ForcePerLength.GetAbbreviation(_forcePerLengthUnit));
      writer.SetString("moment", Force.GetAbbreviation(_forceUnit));
      writer.SetString("stress", Pressure.GetAbbreviation(_stressUnitResult));
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
      var extract = new ToolStripMenuItem("Extract Default Gradient", gradient.Icon_24x24, (s, e) => CreateGradient());
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
      foreach (string unit in
        UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateForce(unit)) {
          Checked = unit == ForcePerLength.GetAbbreviation(_forcePerLengthUnit),
          Enabled = true,
        };
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var momentUnitsMenu = new ToolStripMenuItem("Moment") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateMoment(unit)) {
          Checked = unit == Force.GetAbbreviation(_forceUnit),
          Enabled = true,
        };
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var stressUnitsMenu = new ToolStripMenuItem("Stress") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateStress(unit)) {
          Checked = unit == Pressure.GetAbbreviation(_stressUnitResult),
          Enabled = true,
        };
        stressUnitsMenu.DropDownItems.Add(toolStripMenuItem);
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
          stressUnitsMenu,
        });
      }
      else {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
          lengthUnitsMenu,
          forceUnitsMenu,
          momentUnitsMenu,
          stressUnitsMenu,
        });
      }

      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    protected override void BeforeSolveInstance() {
      switch (_mode) {
        case FoldMode.Displacement:
          Message = (int)_disp < 4
            ? Length.GetAbbreviation(_lengthResultUnit)
            : Angle.GetAbbreviation(AngleUnit.Radian);
          break;

        case FoldMode.Force:
          Message = (int)_disp < 4
            ? ForcePerLength.GetAbbreviation(_forcePerLengthUnit)
            : Force.GetAbbreviation(_forceUnit)
            + "·"
            + Length.GetAbbreviation(_lengthUnit)
            + "/"
            + Length.GetAbbreviation(_lengthUnit);
          break;

        case FoldMode.Stress:
          Message = Pressure.GetAbbreviation(_stressUnitResult);
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
      pManager.AddParameter(new GsaResultsParameter(),
        "Result",
        "Res",
        "GSA Result",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list",
        "El",
        "Filter import by list."
        + Environment.NewLine
        + "Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item,
        "All");
      pManager.AddColourParameter("Colour",
        "Co",
        "Optional list of colours to override default colours"
        + Environment.NewLine
        + "A new gradient will be created from the input list of colours",
        GH_ParamAccess.list);
      pManager.AddIntervalParameter("Min/Max Domain",
        "I",
        "Opitonal Domain for custom Min to Max contour colours",
        GH_ParamAccess.item);
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

      pManager.AddGenericParameter("Result Mesh",
        "M",
        "Mesh with coloured result values",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]",
        "LT",
        "Legend Values",
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

        case GsaResultGoo goo:
          result = goo.Value;
          switch (result.Type) {
            case GsaResult.CaseType.Combination when result.SelectedPermutationIds.Count > 1:
              this.AddRuntimeWarning("Combination Case "
                + result.CaseId
                + " contains "
                + result.SelectedPermutationIds.Count
                + " permutations - only one permutation can be displayed at a time."
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

          break;

        default:
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
      }

      string elementlist = "All";
      var ghType = new GH_String();
      if (da.GetData(1, ref ghType)) {
        GH_Convert.ToString(ghType, out elementlist, GH_Conversion.Both);
      }

      if (elementlist.ToLower() == "all" || elementlist == "") {
        elementlist = "All";
      }

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

      #endregion

      var res = new GsaResultsValues();
      var resShear = new GsaResultsValues();
      switch (_mode) {
        case FoldMode.Displacement:
          res = result.Element2DDisplacementValues(elementlist, _lengthResultUnit)[0];
          break;

        case FoldMode.Force:
          res = result.Element2DForceValues(elementlist, _forcePerLengthUnit, _forceUnit)[0];
          resShear = result.Element2DShearValues(elementlist, _forcePerLengthUnit)[0];
          break;

        case FoldMode.Stress:
          res = result.Element2DStressValues(elementlist, _flayer, _stressUnitResult)[0];
          break;

        case FoldMode.Footfall:
          var footfallType
            = (FootfallResultType)Enum.Parse(typeof(FootfallResultType), _selectedItems[1]);
          res = result.Element2DFootfallValues(elementlist, footfallType)[0];
          break;
      }

      ReadOnlyDictionary<int, Element> elems = result.Model.Model.Elements(elementlist);
      ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes();

      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults
        = _isShear
          ? resShear.XyzResults
          : res.XyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults
        = res.XxyyzzResults;

      Enum xyzunit = _lengthResultUnit;
      Enum xxyyzzunit = AngleUnit.Radian;
      switch (_mode) {
        case FoldMode.Force:
          xyzunit = _forcePerLengthUnit;
          xxyyzzunit = _forceUnit;
          break;

        case FoldMode.Stress:
          xyzunit = _stressUnitResult;
          xxyyzzunit = _stressUnitResult;
          break;

        case FoldMode.Footfall:
          xyzunit = RatioUnit.DecimalFraction;
          _disp = DisplayValue.X;
          break;
      }

      if ((_isShear
          ? resShear.DmaxX
          : res.DmaxX)
        == null) {
        string acase = result.ToString()
          .Replace('}', ' ')
          .Replace('{', ' ');
        this.AddRuntimeWarning("Case " + acase + " contains no Element2D results.");
        return;
      }

      double dmaxX = _isShear
        ? resShear.DmaxX.As(xyzunit)
        : res.DmaxX.As(xyzunit);
      double dmaxY = 0;
      double dmaxZ = 0;
      double dmaxXyz = 0;
      double dminX = 0;
      double dminY = 0;
      double dminZ = 0;
      double dminXyz = 0;
      double dmaxXx = 0;
      double dmaxYy = 0;
      double dmaxZz = 0;
      double dmaxXxyyzz = 0;
      double dminXx = 0;
      double dminYy = 0;
      double dminZz = 0;
      double dminXxyyzz = 0;
      if (_mode != FoldMode.Footfall) {
        dmaxY = _isShear
          ? resShear.DmaxY.As(xyzunit)
          : res.DmaxY.As(xyzunit);
        dmaxZ = res.DmaxZ.As(xyzunit);
        dmaxXyz = (_mode == FoldMode.Displacement)
          ? res.DmaxXyz.As(xyzunit)
          : 0;
        dminX = _isShear
          ? resShear.DminX.As(xyzunit)
          : res.DminX.As(xyzunit);
        dminY = _isShear
          ? resShear.DminY.As(xyzunit)
          : res.DminY.As(xyzunit);
        dminZ = res.DminZ.As(xyzunit);
        dminXyz = (_mode == FoldMode.Displacement)
          ? res.DminXyz.As(xyzunit)
          : 0;
        dmaxXx = _isShear
          ? 0
          : res.DmaxXx.As(xxyyzzunit);
        dmaxYy = _isShear
          ? 0
          : res.DmaxYy.As(xxyyzzunit);
        dmaxZz = _isShear
          ? 0
          : res.DmaxZz.As(xxyyzzunit);
        dmaxXxyyzz = (_mode == FoldMode.Force)
          ? res.DmaxXxyyzz.As(xxyyzzunit)
          : 0;
        dminXx = _isShear
          ? 0
          : res.DminXx.As(xxyyzzunit);
        dminYy = _isShear
          ? 0
          : res.DminYy.As(xxyyzzunit);
        dminZz = _isShear
          ? 0
          : res.DminZz.As(xxyyzzunit);
        dminXxyyzz = (_mode == FoldMode.Force)
          ? res.DminXxyyzz.As(xxyyzzunit)
          : 0;
      }

      if (_mode == FoldMode.Force && _disp == DisplayValue.ResXyz) {
        dmaxXyz = res.DmaxXyz.As(xxyyzzunit);
        dminXyz = res.DminXyz.As(xxyyzzunit);
      }

      #region Result mesh values

      double dmax = 0;
      double dmin = 0;
      switch (_disp) {
        case DisplayValue.X:
          dmax = dmaxX;
          dmin = dminX;
          if (_mode == FoldMode.Displacement) {
            _resType = "Translation, Ux";
          }
          else if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Force, Nx";
          }
          else if (_mode == FoldMode.Force & _isShear) {
            _resType = "2D Shear, Qx";
          }
          else if (_mode == FoldMode.Stress) {
            _resType = "Stress, xx";
          }
          else if (_mode == FoldMode.Footfall) {
            _resType = "Response Factor [-]";
          }

          break;

        case DisplayValue.Y:
          dmax = dmaxY;
          dmin = dminY;
          if (_mode == FoldMode.Displacement) {
            _resType = "Translation, Uy";
          }
          else if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Force, Ny";
          }
          else if (_mode == FoldMode.Force & _isShear) {
            _resType = "2D Shear, Qy";
          }
          else if (_mode == FoldMode.Stress) {
            _resType = "2D Stress, yy";
          }

          break;

        case DisplayValue.Z:
          dmax = dmaxZ;
          dmin = dminZ;
          if (_mode == FoldMode.Displacement) {
            _resType = "Translation, Uz";
          }
          else if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Force, Nxy";
          }
          else if (_mode == FoldMode.Stress) {
            _resType = "Stress, zz";
          }

          break;

        case DisplayValue.ResXyz:
          dmax = dmaxXyz;
          dmin = dminXyz;
          switch (_mode) {
            case FoldMode.Displacement:
              _resType = "Res. Trans., |U|";
              break;

            case FoldMode.Force:
              _resType = "2D Moment, Mx+sgn(Mx)|Mxy|";
              break;
          }

          break;

        case DisplayValue.Xx:
          dmax = dmaxXx;
          dmin = dminXx;
          if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Moment, Mx";
          }
          else if (_mode == FoldMode.Stress) {
            _resType = "Stress, xy";
          }

          break;

        case DisplayValue.Yy:
          dmax = dmaxYy;
          dmin = dminYy;
          if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Moment, My";
          }
          else if (_mode == FoldMode.Stress) {
            _resType = "Stress, yz";
          }

          break;

        case DisplayValue.Zz:
          dmax = dmaxZz;
          dmin = dminZz;
          if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Moment, Mxy";
          }
          else if (_mode == FoldMode.Stress) {
            _resType = "Stress, zy";
          }

          break;

        case DisplayValue.ResXxyyzz:
          if (_mode == FoldMode.Force) {
            _resType = "2D Moment, My+sgn(My)|Mxy|";
          }

          dmax = dmaxXxyyzz;
          dmin = dminXxyyzz;
          break;
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

      #region create mesh

      var resultMeshes = new MeshResultGoo(new Mesh(),
        new List<List<IQuantity>>(),
        new List<List<Point3d>>(),
        new List<int>());
      var meshes = new ConcurrentDictionary<int, Mesh>();
      meshes.AsParallel()
        .AsOrdered();
      var values = new ConcurrentDictionary<int, List<IQuantity>>();
      values.AsParallel()
        .AsOrdered();
      var verticies = new ConcurrentDictionary<int, List<Point3d>>();
      verticies.AsParallel()
        .AsOrdered();

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
      else {
        _lengthUnit = lengthUnit;
      }

      Parallel.ForEach(elems.Keys,
        key => {
          Element element = elems[key];
          if (element.Topology.Count < 3) {
            return;
          }

          Mesh tempmesh = Elements.ConvertElement2D(element, nodes, lengthUnit);
          if (tempmesh == null) {
            return;
          }

          List<Vector3d> transformation = null;
          var vals = new List<IQuantity>();
          switch (_disp) {
            case DisplayValue.X:
              vals = xyzResults[key]
                .Select(item => item.Value.X.ToUnit(xyzunit))
                .ToList();
              if (_mode == FoldMode.Displacement) {
                transformation = xyzResults[key]
                  .Select(item => new Vector3d(item.Value.X.As(lengthUnit) * _defScale, 0, 0))
                  .ToList();
              }

              break;

            case DisplayValue.Y:
              vals = xyzResults[key]
                .Select(item => item.Value.Y.ToUnit(xyzunit))
                .ToList();
              if (_mode == FoldMode.Displacement) {
                transformation = xyzResults[key]
                  .Select(item => new Vector3d(0, item.Value.Y.As(lengthUnit) * _defScale, 0))
                  .ToList();
              }

              break;

            case DisplayValue.Z:
              vals = xyzResults[key]
                .Select(item => item.Value.Z.ToUnit(xyzunit))
                .ToList();
              if (_mode == FoldMode.Displacement) {
                transformation = xyzResults[key]
                  .Select(item => new Vector3d(0, 0, item.Value.Z.As(lengthUnit) * _defScale))
                  .ToList();
              }

              break;

            case DisplayValue.ResXyz:
              vals = xyzResults[key]
                .Select(item => item.Value.Xyz.ToUnit(_mode == FoldMode.Force
                  ? xxyyzzunit
                  : xyzunit))
                .ToList();
              if (_mode == FoldMode.Displacement) {
                transformation = xyzResults[key]
                  .Select(item => new Vector3d(item.Value.X.As(lengthUnit) * _defScale,
                    item.Value.Y.As(lengthUnit) * _defScale,
                    item.Value.Z.As(lengthUnit) * _defScale))
                  .ToList();
              }

              break;

            case DisplayValue.Xx:
              vals = xxyyzzResults[key]
                .Select(item => item.Value.X.ToUnit(xxyyzzunit))
                .ToList();
              break;

            case DisplayValue.Yy:
              vals = xxyyzzResults[key]
                .Select(item => item.Value.Y.ToUnit(xxyyzzunit))
                .ToList();
              break;

            case DisplayValue.Zz:
              vals = xxyyzzResults[key]
                .Select(item => item.Value.Z.ToUnit(xxyyzzunit))
                .ToList();
              break;

            case DisplayValue.ResXxyyzz:
              vals = xxyyzzResults[key]
                .Select(item => item.Value.Xyz.ToUnit(xxyyzzunit))
                .ToList();
              break;
          }

          for (int i = 0;
            i < vals.Count - 1;
            i++) // start at i=0, now the last index is the centre point in GsaAPI output so to count -1
          {
            double tnorm = (2
              * (vals[i]
                  .Value
                - dmin)
              / (dmax - dmin))
              - 1;
            Color col = double.IsNaN(tnorm)
              ? Color.Transparent
              : ghGradient.ColourAt(tnorm);
            tempmesh.VertexColors.Add(col);
            if (transformation == null) {
              continue;
            }

            Point3f def = tempmesh.Vertices[i];
            def.Transform(Transform.Translation(transformation[i]));
            tempmesh.Vertices[i] = def;
          }

          if (
            tempmesh.Vertices.Count
            == 9) // add the value/colour at the centre point if quad-8 (as it already has a vertex here)
          {
            double tnorm = (2
              * (vals.Last()
                  .Value
                - dmin)
              / (dmax - dmin))
              - 1;
            Color col = double.IsNaN(tnorm)
              ? Color.Transparent
              : ghGradient.ColourAt(tnorm);
            tempmesh.VertexColors.Add(col);
            if (transformation != null) {
              Point3f def = tempmesh.Vertices[8];
              def.Transform(Transform.Translation(transformation.Last()));
              tempmesh.Vertices[8] = def;
            }
          }

          if (
            vals.Count
            == 1) // if analysis settings is set to '2D element forces and 2D/3D stresses at centre only'
          {
            double tnorm = (2
              * (vals[0]
                  .Value
                - dmin)
              / (dmax - dmin))
              - 1;
            Color col = double.IsNaN(tnorm)
              ? Color.Transparent
              : ghGradient.ColourAt(tnorm);
            for (int i = 0; i < tempmesh.Vertices.Count; i++) {
              tempmesh.VertexColors.SetColor(i, col);
            }

            verticies[key] = tempmesh.Ngons.Count == 0
              ? new List<Point3d>() {
                new Point3d(tempmesh.Vertices.Select(pt => pt.X)
                    .Average(),
                  tempmesh.Vertices.Select(pt => pt.Y)
                    .Average(),
                  tempmesh.Vertices.Select(pt => pt.Z)
                    .Average()),
              }
              : new List<Point3d>() {
                new Point3d(tempmesh.Vertices.Last()
                    .X,
                  tempmesh.Vertices.Last()
                    .Y,
                  tempmesh.Vertices.Last()
                    .Z),
              };
          }
          else {
            verticies[key] = tempmesh.Vertices.Select(pt => (Point3d)pt)
              .ToList();
          }

          meshes[key] = tempmesh;
          values[key] = vals;

          #endregion
        });

      #endregion

      resultMeshes.Add(meshes.Values.ToList(),
        values.Values.ToList(),
        verticies.Values.ToList(),
        meshes.Keys.ToList());

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
        }
        else {
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
              Message = Length.GetAbbreviation(_lengthResultUnit);
              break;
            }
          case FoldMode.Displacement: {
              var rotation = new Angle(t, AngleUnit.Radian);
              _legendValues.Add(rotation.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(rotation));
              Message = Angle.GetAbbreviation(AngleUnit.Radian);
              break;
            }
          case FoldMode.Force when (int)_disp < 4 | _isShear: {
              var forcePerLength = new ForcePerLength(t, _forcePerLengthUnit);
              _legendValues.Add(forcePerLength.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(forcePerLength));
              Message = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);
              break;
            }
          case FoldMode.Force: {
              _legendValues.Add(
                new Moment(t, UnitsHelper.GetMomentUnit(_forceUnit, lengthUnit)).ToString(
                  "s" + significantDigits)
                + "/"
                + Length.GetAbbreviation(lengthUnit));
              var moment = new Moment(t, UnitsHelper.GetMomentUnit(_forceUnit, lengthUnit));
              ts.Add(new GH_UnitNumber(moment));
              Message = Moment.GetAbbreviation(UnitsHelper.GetMomentUnit(_forceUnit, lengthUnit))
                + "/"
                + Length.GetAbbreviation(lengthUnit);
              break;
            }
          case FoldMode.Stress: {
              var stress = new Pressure(t, _stressUnitResult);
              _legendValues.Add(stress.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(stress));
              Message = Pressure.GetAbbreviation(_stressUnitResult);
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

      da.SetData(0, resultMeshes);
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);

      var resultType
        = (GsaResultsValues.ResultType)Enum.Parse(typeof(GsaResultsValues.ResultType),
          _mode.ToString());
      PostHog.Result(result.Type, 2, resultType, _disp.ToString());
    }

    private void CreateGradient() {
      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();

      gradient.Gradient = Colours.Stress_Gradient();
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
        - (gradient.Attributes.Bounds.Height / 4)
        - 6);

      Instances.ActiveCanvas.Document.AddObject(gradient, false);
      Params.Input[2]
        .RemoveAllSources();
      Params.Input[2]
        .AddSource(gradient.Params.Output[0]);

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
      if (_mode == FoldMode.Force) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Force;

      _slider = false;
      _defScale = 0;
      _spacerDescriptions[2] = "Deform Shape";

      ReDrawComponent();
    }

    private void Mode4Clicked() {
      if (_mode == FoldMode.Stress) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Stress;

      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    private void Mode5Clicked() {
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
      _forcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), unit);
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
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateStress(string unit) {
      _stressUnitResult = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }
  }
}
