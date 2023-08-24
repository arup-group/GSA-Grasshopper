using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI;
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
using AngleUnit = OasysUnits.Units.AngleUnit;
using EnergyUnit = OasysUnits.Units.EnergyUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Element1D results
  /// </summary>
  public class Elem1dContourResults : GH_OasysDropDownComponent {
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
      StrainEnergy,
      Footfall,
    }

    public override Guid ComponentGuid => new Guid("ce7a8f84-4c72-4fd4-a207-485e8bf7ac38");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources._1DContourResults;
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
    private readonly List<string> _force = new List<string>(new[] {
      "Axial Force Fx",
      "Shear Force Fy",
      "Shear Force Fz",
      "Res. Shear |Fyz|",
      "Torsion Mxx",
      "Moment Myy",
      "Moment Mzz",
      "Res. Moment |Myz|",
    });
    private readonly List<string> _strainenergy = new List<string>(new[] {
      "Intermediate Pts",
      "Average",
    });
    private readonly List<string> _type = new List<string>(new[] {
      "Displacement",
      "Force",
      "Strain Energy",
      "Footfall",
    });
    private string _case = string.Empty;
    private double _defScale = 250;
    private DisplayValue _disp = DisplayValue.ResXyz;
    private EnergyUnit _energyResultUnit = DefaultUnits.EnergyUnit;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private Bitmap _legend = new Bitmap(15, 120);
    private double _legendScale = 1;
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
    private string _scaleLegendTxt = string.Empty;
    private bool _showLegend = true;
    private bool _slider = true;
    private bool _undefinedModelLengthUnit;

    public Elem1dContourResults() : base("1D Contour Results", "Contour1d",
      "Displays GSA 1D Element Results as Contour", CategoryName.Name(), SubCategoryName.Cat6()) { }

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
      if (!((_legendValues != null) & _showLegend)) {
        return;
      }

      int defaultTextHeight = 12;
      args.Display.DrawBitmap(new DisplayBitmap(_legend),
        args.Viewport.Bounds.Right - (int)(110 * _legendScale), (int)(20 * _legendScale));
      for (int i = 0; i < _legendValues.Count; i++) {
        args.Display.Draw2dText(_legendValues[i], Color.Black,
          new Point2d(args.Viewport.Bounds.Right - (int)(85 * _legendScale), _legendValuesPosY[i]),
          false, (int)(defaultTextHeight * _legendScale));
      }

      args.Display.Draw2dText(_resType, Color.Black,
        new Point2d(args.Viewport.Bounds.Right - (int)(110 * _legendScale),
          (int)(7 * _legendScale)), false, (int)(defaultTextHeight * _legendScale));
      args.Display.Draw2dText(_case, Color.Black,
        new Point2d(args.Viewport.Bounds.Right - (int)(110 * _legendScale),
          (int)(145 * _legendScale)), false, (int)(defaultTextHeight * _legendScale));
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _showLegend = reader.GetBoolean("legend");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      if (reader.ItemExists("legendScale")) {
        _legendScale = reader.GetDouble("legendScale");
      }

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      _energyResultUnit
        = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), reader.GetString("energy"));
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

                    _disp = DisplayValue.ResXyz;
                    Mode1Clicked();
                  }

                  break;
                }
              case 1: {
                  if (_dropDownItems[1] != _force) {
                    _dropDownItems[1] = _force;

                    _selectedItems[0] = _dropDownItems[0][1];
                    _selectedItems[1] = _dropDownItems[1][5]; // set Myy as default

                    _disp = DisplayValue.Yy;
                    Mode2Clicked();
                  }

                  break;
                }
              case 2: {
                  if (_dropDownItems[1] != _strainenergy) {
                    _dropDownItems[1] = _strainenergy;

                    _selectedItems[0] = _dropDownItems[0][2];
                    _selectedItems[1] = _dropDownItems[1][1]; // set average as default

                    _disp = DisplayValue.Y;
                    Mode3Clicked();
                  }

                  break;
                }
              case 3: {
                  if (_dropDownItems[1] != _footfall) {
                    _dropDownItems[1] = _footfall;

                    _selectedItems[0] = _dropDownItems[0][3];
                    _selectedItems[1] = _dropDownItems[1][0];

                    _disp = DisplayValue.X;
                    Mode4Clicked();
                  }

                  break;
                }
            }

            break;
          }
        case 1: {
            bool redraw = false;

            if (j < 4) {
              if ((int)_disp > 3) // chekc if we are coming from other half of display modes
              {
                if (_mode == FoldMode.Displacement) {
                  redraw = true;
                  _slider = true;
                }
              }
            } else {
              if ((int)_disp < 4) // chekc if we are coming from other half of display modes
              {
                if (_mode == FoldMode.Displacement) {
                  redraw = true;
                  _slider = false;
                }
              }
            }

            _disp = (DisplayValue)j;

            _selectedItems[1] = _dropDownItems[1][j];

            if (redraw) {
              ReDrawComponent();
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
      if (Params.Input.Count != 6) {
        var scale = (Param_Number)Params.Input[4];
        Params.UnregisterInputParameter(Params.Input[4], false);
        Params.RegisterInputParam(new Param_Interval());
        Params.Input[4].Name = "Min/Max Domain";
        Params.Input[4].NickName = "I";
        Params.Input[4].Description = "Opitonal Domain for custom Min to Max contour colours";
        Params.Input[4].Optional = true;
        Params.Input[4].Access = GH_ParamAccess.item;
        Params.RegisterInputParam(scale);
      }

      switch (_mode) {
        case FoldMode.Displacement when (int)_disp < 4:
          Params.Output[2].Name = "Values [" + Length.GetAbbreviation(_lengthResultUnit) + "]";
          break;

        case FoldMode.Displacement:
          Params.Output[2].Name = "Values [rad]";
          break;

        case FoldMode.Force when (int)_disp < 4:
          Params.Output[2].Name = "Legend Values [" + Force.GetAbbreviation(_forceUnit) + "]";
          break;

        case FoldMode.Force:
          Params.Output[2].Name = "Legend Values [" + Moment.GetAbbreviation(_momentUnit) + "]";
          break;

        case FoldMode.StrainEnergy:
          Params.Output[2].Name
            = "Legend Values [" + Energy.GetAbbreviation(_energyResultUnit) + "]";
          break;

        case FoldMode.Footfall:
          Params.Output[2].Name = "Legend Values [-]";
          break;
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetBoolean("slider", _slider);
      writer.SetBoolean("legend", _showLegend);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetDouble("legendScale", _legendScale);
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      writer.SetString("energy", Energy.GetAbbreviation(_energyResultUnit));
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

      ToolStripMenuItem lengthUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Displacement",
        EngineeringUnits.Length, Length.GetAbbreviation(_lengthResultUnit), UpdateLength);

      ToolStripMenuItem forceUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Force",
        EngineeringUnits.Force, Force.GetAbbreviation(_forceUnit), UpdateForce);

      ToolStripMenuItem momentUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Moment",
        EngineeringUnits.Moment, Moment.GetAbbreviation(_momentUnit), UpdateMoment);

      ToolStripMenuItem energyUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Energy",
        EngineeringUnits.Energy, Energy.GetAbbreviation(_energyResultUnit), UpdateEnergy);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        forceUnitsMenu,
        momentUnitsMenu,
        energyUnitsMenu,
      });

      if (_undefinedModelLengthUnit) {
        ToolStripMenuItem modelUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem(
          "Model geometry", EngineeringUnits.Length, Length.GetAbbreviation(_lengthUnit),
          UpdateModel);

        unitsMenu.DropDownItems.Insert(0, modelUnitsMenu);
      }

      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      var legendScale = new ToolStripTextBox {
        Text = _legendScale.ToString(),
      };
      legendScale.TextChanged += (s, e) => MaintainScaleLegendText(legendScale);
      var legendScaleMenu = new ToolStripMenuItem("Scale Legend") {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      var menu2 = new GH_MenuCustomControl(legendScaleMenu.DropDown, legendScale.Control, true,
        200);
      legendScaleMenu.DropDownItems[1].MouseUp += (s, e) => {
        UpdateLegendScale();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        ExpireSolution(true);
      };
      menu.Items.Add(legendScaleMenu);

      Menu_AppendSeparator(menu);
    }

    protected override void BeforeSolveInstance() {
      switch (_mode) {
        case FoldMode.Displacement:
          Message = (int)_disp < 4 ? Length.GetAbbreviation(_lengthResultUnit) :
            Angle.GetAbbreviation(AngleUnit.Radian);
          break;

        case FoldMode.Force:
          Message = (int)_disp < 4 ? Force.GetAbbreviation(_forceUnit) :
            Moment.GetAbbreviation(_momentUnit);
          break;

        case FoldMode.StrainEnergy:
          Message = Energy.GetAbbreviation(_energyResultUnit);
          break;

        case FoldMode.Footfall:
          Message = string.Empty;
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
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaElementListParameter());
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 10)", GH_ParamAccess.item, 10);
      pManager[2].Optional = true;
      pManager.AddColourParameter("Colour", "Co",
        "[Optional] List of colours to override default colours" + Environment.NewLine
        + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[3].Optional = true;
      pManager.AddIntervalParameter("Min/Max Domain", "I",
        "Opitonal Domain for custom Min to Max contour colours", GH_ParamAccess.item);
      pManager[4].Optional = true;
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size",
        GH_ParamAccess.item, 10);
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      IQuantity length = new Length(0, _lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Result Line", "L", "Contoured Line segments with result values",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      _case = string.Empty;
      _resType = string.Empty;

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
            switch (result.Type) {
              case CaseType.Combination when result.SelectedPermutationIds.Count > 1:
                this.AddRuntimeWarning("Combination Case " + result.CaseId + " contains "
                  + result.SelectedPermutationIds.Count
                  + " permutations - only one permutation can be displayed at a time."
                  + Environment.NewLine
                  + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
                _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
                break;

              case CaseType.Combination:
                _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
                break;

              case CaseType.AnalysisCase:
                _case = "Case A" + result.CaseId + Environment.NewLine + result.CaseName;
                break;
            }

            break;
          }
        default:
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
      }

      string elementlist = Inputs.GetElementListNameForResults(this, da, 1, result.Model);

      var ghDiv = new GH_Integer();
      da.GetData(2, ref ghDiv);
      GH_Convert.ToInt32(ghDiv, out int positionsCount, GH_Conversion.Both);
      positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

      var ghColours = new List<GH_Colour>();
      var colors = new List<Color>();
      if (da.GetDataList(3, ghColours)) {
        foreach (GH_Colour t in ghColours) {
          GH_Convert.ToColor(t, out Color color, GH_Conversion.Both);
          colors.Add(color);
        }
      }

      GH_Gradient ghGradient = Colours.Stress_Gradient(colors);

      var ghInterval = new GH_Interval();
      Interval customMinMax = Interval.Unset;
      if (da.GetData(4, ref ghInterval)) {
        GH_Convert.ToInterval(ghInterval, ref customMinMax, GH_Conversion.Both);
      }

      var ghScale = new GH_Number();
      da.GetData(5, ref ghScale);
      GH_Convert.ToDouble(ghScale, out double scale, GH_Conversion.Both);

      #endregion

      var res = new GsaResultsValues();

      switch (_mode) {
        case FoldMode.Displacement:
          res = result.Element1DDisplacementValues(elementlist, positionsCount, 0, _lengthResultUnit)[0];
          break;

        case FoldMode.Force:
          res = result.Element1DForceValues(elementlist, positionsCount, 0, _forceUnit, _momentUnit)[0];
          break;

        case FoldMode.StrainEnergy:
          res = _disp == DisplayValue.X ?
            result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, 0,
              _energyResultUnit)[0] :
            result.Element1DAverageStrainEnergyDensityValues(elementlist, 0, _energyResultUnit)[0];
          break;

        case FoldMode.Footfall:
          var footfallType
            = (FootfallResultType)Enum.Parse(typeof(FootfallResultType), _selectedItems[1]);
          res = result.Element1DFootfallValues(elementlist, footfallType)[0];
          break;
      }

      var elems = new ConcurrentDictionary<int, Element>(result.Model.Model.Elements(elementlist));
      var nodes = new ConcurrentDictionary<int, Node>(result.Model.Model.Nodes());

      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults
        = res.XyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults
        = res.XxyyzzResults;

      Enum xyzunit = _lengthResultUnit;
      Enum xxyyzzunit = AngleUnit.Radian;
      switch (_mode) {
        case FoldMode.Force:
          xyzunit = _forceUnit;
          xxyyzzunit = _momentUnit;
          break;

        case FoldMode.StrainEnergy:
          xyzunit = DefaultUnits.EnergyUnit;
          break;

        case FoldMode.Footfall:
          _disp = DisplayValue.X;
          xyzunit = RatioUnit.DecimalFraction;
          break;
      }

      if (res.DmaxX == null) {
        string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
        string filter = string.Empty;
        if (elementlist.ToLower() != "all") {
          filter = " for element list " + elementlist;
        }

        this.AddRuntimeWarning("Case " + acase + " contains no Element1D results" + filter);
        return;
      }

      double dmaxX = res.DmaxX.As(xyzunit);
      double dmaxY = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DmaxY.As(xyzunit);
      double dmaxZ = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DmaxZ.As(xyzunit);
      double dmaxXyz = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DmaxXyz.As(xyzunit);
      double dminX = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminX.As(xyzunit);
      double dminY = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminY.As(xyzunit);
      double dminZ = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminZ.As(xyzunit);
      double dminXyz = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminXyz.As(xyzunit);
      double dmaxXx = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DmaxXx.As(xxyyzzunit);
      double dmaxYy = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DmaxYy.As(xxyyzzunit);
      double dmaxZz = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DmaxZz.As(xxyyzzunit);
      double dmaxXxyyzz = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DmaxXxyyzz.As(xxyyzzunit);
      double dminXx = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminXx.As(xxyyzzunit);
      double dminYy = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminYy.As(xxyyzzunit);
      double dminZz = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminZz.As(xxyyzzunit);
      double dminXxyyzz = _mode == FoldMode.StrainEnergy || _mode == FoldMode.Footfall ? 0 :
        res.DminXxyyzz.As(xxyyzzunit);

      #region Result line values

      double dmax = 0;
      double dmin = 0;
      switch (_mode) {
        case FoldMode.StrainEnergy: {
            dmax = dmaxX;
            dmin = dminX;
            if (_disp == DisplayValue.X) {
              _resType = "Strain Energy Density";
            } else {
              positionsCount = 2;
              _resType = "Average Strain E Dens.";
            }

            break;
          }
        case FoldMode.Footfall:
          dmax = dmaxX;
          dmin = dminX;
          positionsCount = 2;
          _resType = "Response Factor [-]";
          break;

        default:
          switch (_disp) {
            case DisplayValue.X:
              dmax = dmaxX;
              dmin = dminX;
              _resType = _mode == FoldMode.Displacement ? "Elem. Trans., Ux" : "Axial Force, Fx";
              break;

            case DisplayValue.Y:
              dmax = dmaxY;
              dmin = dminY;
              _resType = _mode == FoldMode.Displacement ? "Elem. Trans., Uy" : "Shear Force, Fy";
              break;

            case DisplayValue.Z:
              dmax = dmaxZ;
              dmin = dminZ;
              _resType = _mode == FoldMode.Displacement ? "Elem. Trans., Uz" : "Shear Force, Fz";
              break;

            case DisplayValue.ResXyz:
              dmax = dmaxXyz;
              dmin = dminXyz;
              _resType = _mode == FoldMode.Displacement ? "Res. Trans., |U|" : "Res. Shear, |Fyz|";
              break;

            case DisplayValue.Xx:
              dmax = dmaxXx;
              dmin = dminXx;
              _resType = _mode == FoldMode.Displacement ? "Elem. Rot., Rxx" : "Torsion, Mxx";
              break;

            case DisplayValue.Yy:
              dmax = dmaxYy;
              dmin = dminYy;
              _resType = _mode == FoldMode.Displacement ? "Elem. Rot., Ryy" : "Moment, Myy";
              break;

            case DisplayValue.Zz:
              dmax = dmaxZz;
              dmin = dminZz;
              _resType = _mode == FoldMode.Displacement ? "Elem. Rot., Rzz" : "Moment, Mzz";
              break;

            case DisplayValue.ResXxyyzz:
              dmax = dmaxXxyyzz;
              dmin = dminXxyyzz;
              _resType = _mode == FoldMode.Displacement ? "Res. Rot., |U|" : "Res. Moment, |Myz|";
              break;
          }

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

      var resultLines = new DataTree<LineResultGoo>();
      LengthUnit lengthUnit = result.Model.ModelUnit;
      _undefinedModelLengthUnit = false;
      if (lengthUnit == LengthUnit.Undefined) {
        lengthUnit = _lengthUnit;
        _undefinedModelLengthUnit = true;
        this.AddRuntimeRemark(
          "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in "
          + lengthUnit + ". This can be changed by right-clicking the component -> 'Select Units'");
      }

      Parallel.ForEach(elems, element => {
        if (element.Value.IsDummy || element.Value.Type == ElementType.LINK
          || element.Value.Topology.Count > 2) {
          return;
        }

        var ln = new Line(
          Nodes.Point3dFromNode(nodes[element.Value.Topology[0]], lengthUnit), // start point
          Nodes.Point3dFromNode(nodes[element.Value.Topology[1]], lengthUnit)); // end point

        int key = element.Key;

        for (int i = 0; i < positionsCount - 1; i++) {
          if ((dmin == 0) & (dmax == 0)) {
            continue;
          }

          var startTranslation = new Vector3d(0, 0, 0);
          var endTranslation = new Vector3d(0, 0, 0);

          IQuantity t1 = null;
          IQuantity t2 = null;

          var start = new Point3d(ln.PointAt((double)i / (positionsCount - 1)));
          var end = new Point3d(ln.PointAt((double)(i + 1) / (positionsCount - 1)));

          switch (_mode) {
            case FoldMode.Displacement:
              switch (_disp) {
                case DisplayValue.X:
                  t1 = xyzResults[key][i].X.ToUnit(_lengthResultUnit);
                  t2 = xyzResults[key][i + 1].X.ToUnit(_lengthResultUnit);
                  startTranslation.X = xyzResults[key][i].X.As(lengthUnit) * _defScale;
                  endTranslation.X = xyzResults[key][i + 1].X.As(lengthUnit) * _defScale;
                  break;

                case DisplayValue.Y:
                  t1 = xyzResults[key][i].Y.ToUnit(_lengthResultUnit);
                  t2 = xyzResults[key][i + 1].Y.ToUnit(_lengthResultUnit);
                  startTranslation.Y = xyzResults[key][i].Y.As(lengthUnit) * _defScale;
                  endTranslation.Y = xyzResults[key][i + 1].Y.As(lengthUnit) * _defScale;
                  break;

                case DisplayValue.Z:
                  t1 = xyzResults[key][i].Z.ToUnit(_lengthResultUnit);
                  t2 = xyzResults[key][i + 1].Z.ToUnit(_lengthResultUnit);
                  startTranslation.Z = xyzResults[key][i].Z.As(lengthUnit) * _defScale;
                  endTranslation.Z = xyzResults[key][i + 1].Z.As(lengthUnit) * _defScale;
                  break;

                case DisplayValue.ResXyz:
                  t1 = xyzResults[key][i].Xyz.ToUnit(_lengthResultUnit);
                  t2 = xyzResults[key][i + 1].Xyz.ToUnit(_lengthResultUnit);
                  startTranslation.X = xyzResults[key][i].X.As(lengthUnit) * _defScale;
                  startTranslation.Y = xyzResults[key][i].Y.As(lengthUnit) * _defScale;
                  startTranslation.Z = xyzResults[key][i].Z.As(lengthUnit) * _defScale;
                  endTranslation.X = xyzResults[key][i + 1].X.As(lengthUnit) * _defScale;
                  endTranslation.Y = xyzResults[key][i + 1].Y.As(lengthUnit) * _defScale;
                  endTranslation.Z = xyzResults[key][i + 1].Z.As(lengthUnit) * _defScale;
                  break;

                case DisplayValue.Xx:
                  t1 = xxyyzzResults[key][i].X.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].X.ToUnit(AngleUnit.Radian);
                  break;

                case DisplayValue.Yy:
                  t1 = xxyyzzResults[key][i].Y.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].Y.ToUnit(AngleUnit.Radian);
                  break;

                case DisplayValue.Zz:
                  t1 = xxyyzzResults[key][i].Z.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].Z.ToUnit(AngleUnit.Radian);
                  break;

                case DisplayValue.ResXxyyzz:
                  t1 = xxyyzzResults[key][i].Xyz.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].Xyz.ToUnit(AngleUnit.Radian);
                  break;
              }

              start.Transform(Transform.Translation(startTranslation));
              end.Transform(Transform.Translation(endTranslation));
              break;

            case FoldMode.Force:
              switch (_disp) {
                case DisplayValue.X:
                  t1 = xyzResults[key][i].X.ToUnit(_forceUnit);
                  t2 = xyzResults[key][i + 1].X.ToUnit(_forceUnit);
                  break;

                case DisplayValue.Y:
                  t1 = xyzResults[key][i].Y.ToUnit(_forceUnit);
                  t2 = xyzResults[key][i + 1].Y.ToUnit(_forceUnit);
                  break;

                case DisplayValue.Z:
                  t1 = xyzResults[key][i].Z.ToUnit(_forceUnit);
                  t2 = xyzResults[key][i + 1].Z.ToUnit(_forceUnit);
                  break;

                case DisplayValue.ResXyz:
                  t1 = xyzResults[key][i].Xyz.ToUnit(_forceUnit);
                  t2 = xyzResults[key][i + 1].Xyz.ToUnit(_forceUnit);
                  break;

                case DisplayValue.Xx:
                  t1 = xxyyzzResults[key][i].X.ToUnit(_momentUnit);
                  t2 = xxyyzzResults[key][i + 1].X.ToUnit(_momentUnit);
                  break;

                case DisplayValue.Yy:
                  t1 = xxyyzzResults[key][i].Y.ToUnit(_momentUnit);
                  t2 = xxyyzzResults[key][i + 1].Y.ToUnit(_momentUnit);
                  break;

                case DisplayValue.Zz:
                  t1 = xxyyzzResults[key][i].Z.ToUnit(_momentUnit);
                  t2 = xxyyzzResults[key][i + 1].Z.ToUnit(_momentUnit);
                  break;

                case DisplayValue.ResXxyyzz:
                  t1 = xxyyzzResults[key][i].Xyz.ToUnit(_momentUnit);
                  t2 = xxyyzzResults[key][i + 1].Xyz.ToUnit(_momentUnit);
                  break;
              }

              break;

            case FoldMode.StrainEnergy:
              if (_disp == DisplayValue.X) {
                t1 = xyzResults[key][i].X.ToUnit(_energyResultUnit);
                t2 = xyzResults[key][i + 1].X.ToUnit(_energyResultUnit);
              } else {
                t1 = xyzResults[key][i].X.ToUnit(_energyResultUnit);
                t2 = xyzResults[key][i].X.ToUnit(_energyResultUnit);
              }

              break;

            case FoldMode.Footfall:
              t1 = xyzResults[key][i].X.ToUnit(RatioUnit.DecimalFraction);
              t2 = xyzResults[key][i].X.ToUnit(RatioUnit.DecimalFraction);
              break;
          }

          var segmentline = new Line(start, end);

          double tnorm1 = (2 * (t1.Value - dmin) / (dmax - dmin)) - 1;
          double tnorm2 = (2 * (t2.Value - dmin) / (dmax - dmin)) - 1;

          Color valcol1 = double.IsNaN(tnorm1) ? Color.Black : ghGradient.ColourAt(tnorm1);
          Color valcol2 = double.IsNaN(tnorm2) ? Color.Black : ghGradient.ColourAt(tnorm2);

          float size1 = t1.Value >= 0 && dmax != 0 ? Math.Max(2, (float)(t1.Value / dmax * scale)) :
            Math.Max(2, (float)(Math.Abs(t1.Value) / Math.Abs(dmin) * scale));
          if (double.IsNaN(size1)) {
            size1 = 1;
          }

          float size2 = t2.Value >= 0 && dmax != 0 ? Math.Max(2, (float)(t2.Value / dmax * scale)) :
            Math.Max(2, (float)(Math.Abs(t2.Value) / Math.Abs(dmin) * scale));
          if (double.IsNaN(size2)) {
            size2 = 1;
          }

          lock (resultLines) {
            resultLines.Add(
              new LineResultGoo(segmentline, t1, t2, valcol1, valcol2, size1, size2, key),
              new GH_Path(key));
          }
        }
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
          case FoldMode.Force when (int)_disp < 4: {
              var force = new Force(t, _forceUnit);
              _legendValues.Add(force.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(force));
              break;
            }
          case FoldMode.Force: {
              var moment = new Moment(t, _momentUnit);
              _legendValues.Add(moment.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(moment));
              break;
            }
          case FoldMode.StrainEnergy: {
              var energy = new Energy(t, _energyResultUnit);
              _legendValues.Add(energy.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(energy));
              break;
            }
          default: {
              var responseFactor = new Ratio(t, RatioUnit.DecimalFraction);
              _legendValues.Add(responseFactor.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(responseFactor));
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

      da.SetDataTree(0, resultLines);
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);

      var resultType
        = (GsaResultsValues.ResultType)Enum.Parse(typeof(GsaResultsValues.ResultType),
          _mode.ToString());
      PostHog.Result(result.Type, 1, resultType, _disp.ToString());
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
        Params.Input[3].Attributes.Bounds.Y - (gradient.Attributes.Bounds.Height / 4) - 6);

      Instances.ActiveCanvas.Document.AddObject(gradient, false);
      Params.Input[3].RemoveAllSources();
      Params.Input[3].AddSource(gradient.Params.Output[0]);

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

      ReDrawComponent();
    }

    private void Mode3Clicked() {
      if (_mode == FoldMode.StrainEnergy) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.StrainEnergy;

      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    private void Mode4Clicked() {
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

    private void UpdateEnergy(string unit) {
      _energyResultUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
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

    private void UpdateLegendScale() {
      try {
        _legendScale = double.Parse(_scaleLegendTxt);
      } catch (Exception e) {
        this.AddRuntimeWarning(e.Message);
        return;
      }

      _legend = new Bitmap((int)(15 * _legendScale), (int)(120 * _legendScale));

      ExpirePreview(true);
      base.UpdateUI();
    }

    private void MaintainScaleLegendText(ToolStripItem menuitem) {
      _scaleLegendTxt = menuitem.Text;
    }
  }
}
