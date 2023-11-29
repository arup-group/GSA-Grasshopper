﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Grasshopper.Kernel.Geometry.SpatialTrees;
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
using GsaGH.Parameters.Results;
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
  public class Contour1dResults : GH_OasysDropDownComponent {
    private enum DisplayValue {
      X,
      Y,
      Z,
      ResXyz,
      Xx,
      Yy,
      Zz,
      ResXxyyzz,
      Extra
    }

    private enum FoldMode {
      Displacement,
      Force,
      ProjectedStress,
      DerivedStress,
      StrainEnergy,
      Footfall,
    }

    public override Guid ComponentGuid => new Guid("ce7a8f84-4c72-4fd4-a207-485e8bf7ac38");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Contour1dResults;
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
    private readonly List<string> _projStress = new List<string>(new[] {
      "Axial, A",
      "Shear, Sy",
      "Shear, Sz",
      "Bending, yy+",
      "Bending, yy-",
      "Bending, zz+",
      "Bending, zz-",
      "Combined, C1",
      "Combined, C2",
    });
    private readonly List<string> _derivStress = new List<string>(new[] {
      "Elastic Shear Y",
      "Elastic Shear Z",
      "Torsional",
      "Von Mises",
    });
    private readonly List<string> _type = new List<string>(new[] {
      "Displacement",
      "Force",
      "Stress",
      "Derived Stress",
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
    private double _maxValue = 1000;
    private double _minValue;
    private FoldMode _mode = FoldMode.Displacement;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private int _noDigits;
    private string _resType;
    private string _scaleLegendTxt = string.Empty;
    private bool _showLegend = true;
    private bool _slider = true;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;

    public Contour1dResults() : base("Contour 1D Results", "Contour1d",
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

      _stressUnit = reader.ItemExists("stress")
        ? (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), reader.GetString("stress"))
        : DefaultUnits.StressUnitResult;

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
        case 0:
          switch (j) {
            case 0: 
                if (_dropDownItems[1] != _displacement) {
                  _dropDownItems[1] = _displacement;

                  _selectedItems[0] = _dropDownItems[0][0];
                  _selectedItems[1] = _dropDownItems[1][3];

                  _disp = DisplayValue.ResXyz;
                  Mode1Clicked();
                }

                break;
              
            case 1: 
                if (_dropDownItems[1] != _force) {
                  _dropDownItems[1] = _force;

                  _selectedItems[0] = _dropDownItems[0][1];
                  _selectedItems[1] = _dropDownItems[1][5]; // set Myy as default

                  _disp = DisplayValue.Yy;
                  Mode2Clicked();
                }

                break;

            case 2:
              if (_dropDownItems[1] != _projStress) {
                _dropDownItems[1] = _projStress;

                _selectedItems[0] = _dropDownItems[0][2];
                _selectedItems[1] = _dropDownItems[1][7]; // set C1 as default

                _disp = DisplayValue.ResXxyyzz;
                Mode3Clicked();
              }

              break;

            case 3:
              if (_dropDownItems[1] != _derivStress) {
                _dropDownItems[1] = _derivStress;

                _selectedItems[0] = _dropDownItems[0][3];
                _selectedItems[1] = _dropDownItems[1][3]; // set von Mises as default

                _disp = DisplayValue.ResXyz;
                Mode4Clicked();
              }

              break;

            case 4: 
                if (_dropDownItems[1] != _strainenergy) {
                  _dropDownItems[1] = _strainenergy;

                  _selectedItems[0] = _dropDownItems[0][4];
                  _selectedItems[1] = _dropDownItems[1][1]; // set average as default

                  _disp = DisplayValue.Y;
                  Mode5Clicked();
                }

                break;
              
            case 5: 
                if (_dropDownItems[1] != _footfall) {
                  _dropDownItems[1] = _footfall;

                  _selectedItems[0] = _dropDownItems[0][5];
                  _selectedItems[1] = _dropDownItems[1][0];

                  _disp = DisplayValue.X;
                  Mode6Clicked();
                }

                break;
              
          }

          break;

        case 1:
          bool redraw = false;

          if (j < 4) {
            if ((int)_disp > 3) // check if we are coming from other half of display modes
            {
              if (_mode == FoldMode.Displacement) {
                redraw = true;
                _slider = true;
              }
            }
          } else {
            if ((int)_disp < 4) // check if we are coming from other half of display modes
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
        Params.Input[4].Description = "Optional Domain for custom Min to Max contour colours";
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

        case FoldMode.ProjectedStress:
        case FoldMode.DerivedStress:
          Params.Output[2].Name = "Legend Values [" + Pressure.GetAbbreviation(_stressUnit) + "]";
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
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("stress", Pressure.GetAbbreviation(_stressUnit));
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

      ToolStripMenuItem stressUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Stress",
        EngineeringUnits.Stress, Pressure.GetAbbreviation(_stressUnit), UpdateStress);

      ToolStripMenuItem energyUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Energy",
        EngineeringUnits.Energy, Energy.GetAbbreviation(_energyResultUnit), UpdateEnergy);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        forceUnitsMenu,
        momentUnitsMenu,
        stressUnitsMenu,
        energyUnitsMenu,
      });

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

        case FoldMode.ProjectedStress:
        case FoldMode.DerivedStress:
          Message = Pressure.GetAbbreviation(_stressUnit);
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
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 10)", GH_ParamAccess.item, 10);
      pManager[2].Optional = true;
      pManager.AddColourParameter("Colour", "Co",
        "[Optional] List of colours to override default colours" + Environment.NewLine
        + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[3].Optional = true;
      pManager.AddIntervalParameter("Min/Max Domain", "I",
        "Optional Domain for custom Min to Max contour colours", GH_ParamAccess.item);
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

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult2 result;
      string elementlist = "All";
      _case = string.Empty;
      _resType = string.Empty;

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      switch (ghTyp?.Value) {
        case GsaResultGoo goo:
          result = new GsaResult2((GsaResult)goo.Value);
          elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
          switch (result.CaseType) {
            case CaseType.CombinationCase when result.SelectedPermutationIds.Count > 1:
              this.AddRuntimeWarning("Combination Case " + result.CaseId + " contains "
                + result.SelectedPermutationIds.Count
                + " permutations - only one permutation can be displayed at a time."
                + Environment.NewLine
                + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
              _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
              break;

            case CaseType.CombinationCase:
              _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
              break;

            case CaseType.AnalysisCase:
              _case = "Case A" + result.CaseId + Environment.NewLine + result.CaseName;
              break;
          }
          break;

        case null:
          this.AddRuntimeWarning("Input is null");
          return;

        default:
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
      }

      ReadOnlyDictionary<int, Element> elems = result.Model.Model.Elements(elementlist);
      ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes();
      if (elems.Count == 0) {
        this.AddRuntimeError($"Model contains no results for elements in list '{elementlist}'");
        return;
      }

      LengthUnit lengthUnit = result.Model.ModelUnit;

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

      ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist);
      int permutation = result.SelectedPermutationIds == null
        ? 0 : result.SelectedPermutationIds[0] - 1;
      double dmax = 0;
      double dmin = 0;
      ConcurrentDictionary<int, List<IQuantity>> values = null;
      ConcurrentDictionary<int, (List<double> x, List<double> y, List<double> z)> valuesXyz = null;
      switch (_mode) {
        case FoldMode.Displacement:
          result.Element1dDisplacements.ResultSubset(elementIds, positionsCount);
          IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>> displacements = Quarterion.CoordinateTransformationTo(
            result.Element1dDisplacements.ResultSubset(elementIds, positionsCount).Subset, 
            Plane.WorldXY, result.Model.Model);
          Func<IDisplacement, IQuantity> displacementSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Translation, Ux";
              dmax = displacements.GetExtrema(displacements.Max.X).X.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.X).X.As(_lengthResultUnit);
              displacementSelector = (r) => r.X.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.Y:
              _resType = "Translation, Uy";
              dmax = displacements.GetExtrema(displacements.Max.Y).Y.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Y).Y.As(_lengthResultUnit);
              displacementSelector = (r) => r.Y.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.Z:
              _resType = "Translation, Uz";
              dmax = displacements.GetExtrema(displacements.Max.Z).Z.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Z).Z.As(_lengthResultUnit);
              displacementSelector = (r) => r.Z.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.ResXyz:
              _resType = "Translation, |U|";
              dmax = displacements.GetExtrema(displacements.Max.Xyz).Xyz.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Xyz).Xyz.As(_lengthResultUnit);
              displacementSelector = (r) => r.Xyz.ToUnit(_lengthResultUnit);
              valuesXyz = ResultsUtility.GetResultResultanTranslation(
                displacements.Subset, lengthUnit, permutation);
              break;

            case DisplayValue.Xx:
              _resType = "Rotation, Rxx";
              dmax = displacements.GetExtrema(displacements.Max.Xx).Xx.Radians;
              dmin = displacements.GetExtrema(displacements.Min.Xx).Xx.Radians;
              displacementSelector = (r) => r.Xx;
              break;

            case DisplayValue.Yy:
              _resType = "Rotation, Ryy";
              dmax = displacements.GetExtrema(displacements.Max.Yy).Yy.Radians;
              dmin = displacements.GetExtrema(displacements.Min.Yy).Yy.Radians;
              displacementSelector = (r) => r.Yy;
              break;

            case DisplayValue.Zz:
              _resType = "Rotation, Rzz";
              dmax = displacements.GetExtrema(displacements.Max.Zz).Zz.Radians;
              dmin = displacements.GetExtrema(displacements.Min.Zz).Zz.Radians;
              displacementSelector = (r) => r.Zz;
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Rotation, |R|";
              dmax = displacements.GetExtrema(displacements.Max.Xxyyzz).Xxyyzz.Radians;
              dmin = displacements.GetExtrema(displacements.Min.Xxyyzz).Xxyyzz.Radians;
              displacementSelector = (r) => r.Xxyyzz;
              break;
          }

          values = ResultsUtility.GetResultComponent(displacements.Subset, displacementSelector, permutation);
          break;

        case FoldMode.Force:
          IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> forces =
          result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);
          Func<IInternalForce, IQuantity> forceSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Axial Force, Fx";
              dmax = forces.GetExtrema(forces.Max.X).X.As(_forceUnit);
              dmin = forces.GetExtrema(forces.Min.X).X.As(_forceUnit);
              forceSelector = (r) => r.X.ToUnit(_forceUnit);
              break;

            case DisplayValue.Y:
              _resType = "Shear Force, Fy";
              dmax = forces.GetExtrema(forces.Max.Y).Y.As(_forceUnit);
              dmin = forces.GetExtrema(forces.Min.Y).Y.As(_forceUnit);
              forceSelector = (r) => r.Y.ToUnit(_forceUnit);
              break;

            case DisplayValue.Z:
              _resType = "Shear Force, Fz";
              dmax = forces.GetExtrema(forces.Max.Z).Z.As(_forceUnit);
              dmin = forces.GetExtrema(forces.Min.Z).Z.As(_forceUnit);
              forceSelector = (r) => r.Z.ToUnit(_forceUnit);
              break;

            case DisplayValue.ResXyz:
              _resType = "Shear Force, |Fyz|";
              dmax = forces.GetExtrema(forces.Max.Xyz).Xyz.As(_forceUnit);
              dmin = forces.GetExtrema(forces.Min.Xyz).Xyz.As(_forceUnit);
              forceSelector = (r) => r.Xyz.ToUnit(_forceUnit);
              break;

            case DisplayValue.Xx:
              _resType = "Torsion, Mxx";
              dmax = forces.GetExtrema(forces.Max.Xx).Xx.As(_momentUnit);
              dmin = forces.GetExtrema(forces.Min.Xx).Xx.As(_momentUnit);
              forceSelector = (r) => r.Xx.ToUnit(_momentUnit);
              break;

            case DisplayValue.Yy:
              _resType = "Moment, Myy";
              dmax = forces.GetExtrema(forces.Max.Yy).Yy.As(_momentUnit);
              dmin = forces.GetExtrema(forces.Min.Yy).Yy.As(_momentUnit);
              forceSelector = (r) => r.Yy.ToUnit(_momentUnit);
              break;

            case DisplayValue.Zz:
              _resType = "Moment, Mzz";
              dmax = forces.GetExtrema(forces.Max.Zz).Zz.As(_momentUnit);
              dmin = forces.GetExtrema(forces.Min.Zz).Zz.As(_momentUnit);
              forceSelector = (r) => r.Zz.ToUnit(_momentUnit);
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Moment, |Myz|";
              dmax = forces.GetExtrema(forces.Max.Xxyyzz).Xxyyzz.As(_momentUnit);
              dmin = forces.GetExtrema(forces.Min.Xxyyzz).Xxyyzz.As(_momentUnit);
              forceSelector = (r) => r.Xxyyzz.ToUnit(_momentUnit);
              break;
          }

          values = ResultsUtility.GetResultComponent(forces.Subset, forceSelector, permutation);
          break;

        case FoldMode.ProjectedStress:
          IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> stresses =
          result.Element1dStresses.ResultSubset(elementIds, positionsCount);
          Func<IStress1d, IQuantity> stressSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Axial Stress, SA";
              dmax = stresses.GetExtrema(stresses.Max.Axial).Axial.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.Axial).Axial.As(_stressUnit);
              stressSelector = (r) => r.Axial.ToUnit(_stressUnit);
              break;

            case DisplayValue.Y:
              _resType = "Shear Stress, Sy";
              dmax = stresses.GetExtrema(stresses.Max.ShearY).ShearY.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.ShearY).ShearY.As(_stressUnit);
              stressSelector = (r) => r.ShearY.ToUnit(_stressUnit);
              break;

            case DisplayValue.Z:
              _resType = "Shear Stress, Sz";
              dmax = stresses.GetExtrema(stresses.Max.ShearZ).ShearZ.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.ShearZ).ShearZ.As(_stressUnit);
              stressSelector = (r) => r.ShearZ.ToUnit(_stressUnit);
              break;

            case DisplayValue.ResXyz:
              _resType = "Bending Stress, By +ve z";
              dmax = stresses.GetExtrema(stresses.Max.BendingYyPositiveZ).BendingYyPositiveZ.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.BendingYyPositiveZ).BendingYyPositiveZ.As(_stressUnit);
              stressSelector = (r) => r.BendingYyPositiveZ.ToUnit(_stressUnit);
              break;

            case DisplayValue.Xx:
              _resType = "Bending Stress, By -ve z";
              dmax = stresses.GetExtrema(stresses.Max.BendingYyNegativeZ).BendingYyNegativeZ.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.BendingYyNegativeZ).BendingYyNegativeZ.As(_stressUnit);
              stressSelector = (r) => r.BendingYyNegativeZ.ToUnit(_stressUnit);
              break;

            case DisplayValue.Yy:
              _resType = "Bending Stress, Bz +ve y";
              dmax = stresses.GetExtrema(stresses.Max.BendingZzPositiveY).BendingZzPositiveY.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.BendingZzPositiveY).BendingZzPositiveY.As(_stressUnit);
              stressSelector = (r) => r.BendingZzPositiveY.ToUnit(_stressUnit);
              break;

            case DisplayValue.Zz:
              _resType = "Bending Stress, Bz -ve y";
              dmax = stresses.GetExtrema(stresses.Max.BendingZzNegativeY).BendingZzNegativeY.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.BendingZzNegativeY).BendingZzNegativeY.As(_stressUnit);
              stressSelector = (r) => r.BendingZzNegativeY.ToUnit(_stressUnit);
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Combined Stress, C1";
              dmax = stresses.GetExtrema(stresses.Max.CombinedC1).CombinedC1.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.CombinedC1).CombinedC1.As(_stressUnit);
              stressSelector = (r) => r.CombinedC1.ToUnit(_stressUnit);
              break;

            case DisplayValue.Extra:
              _resType = "Combined Stress, C2";
              dmax = stresses.GetExtrema(stresses.Max.CombinedC2).CombinedC2.As(_stressUnit);
              dmin = stresses.GetExtrema(stresses.Min.CombinedC2).CombinedC2.As(_stressUnit);
              stressSelector = (r) => r.CombinedC2.ToUnit(_stressUnit);
              break;
          }

          values = ResultsUtility.GetResultComponent(stresses.Subset, stressSelector, permutation);
          break;

        case FoldMode.DerivedStress:
          IEntity1dResultSubset<IEntity1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> derivedStresses =
          result.Element1dDerivedStresses.ResultSubset(elementIds, positionsCount);
          Func<IStress1dDerived, IQuantity> derivedStressSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Elastic Shear, SEy";
              dmax = derivedStresses.GetExtrema(derivedStresses.Max.ElasticShearY).ElasticShearY.As(_stressUnit);
              dmin = derivedStresses.GetExtrema(derivedStresses.Min.ElasticShearY).ElasticShearY.As(_stressUnit);
              derivedStressSelector = (r) => r.ElasticShearY.ToUnit(_stressUnit);
              break;

            case DisplayValue.Y:
              _resType = "Elastic Shear, SEy";
              dmax = derivedStresses.GetExtrema(derivedStresses.Max.ElasticShearZ).ElasticShearZ.As(_stressUnit);
              dmin = derivedStresses.GetExtrema(derivedStresses.Min.ElasticShearZ).ElasticShearZ.As(_stressUnit);
              derivedStressSelector = (r) => r.ElasticShearZ.ToUnit(_stressUnit);
              break;

            case DisplayValue.Z:
              _resType = "Torsional Stress, St";
              dmax = derivedStresses.GetExtrema(derivedStresses.Max.Torsional).Torsional.As(_stressUnit);
              dmin = derivedStresses.GetExtrema(derivedStresses.Min.Torsional).Torsional.As(_stressUnit);
              derivedStressSelector = (r) => r.Torsional.ToUnit(_stressUnit);
              break;

            case DisplayValue.ResXyz:
              _resType = "Von Mises Stress";
              dmax = derivedStresses.GetExtrema(derivedStresses.Max.VonMises).VonMises.As(_stressUnit);
              dmin = derivedStresses.GetExtrema(derivedStresses.Min.VonMises).VonMises.As(_stressUnit);
              derivedStressSelector = (r) => r.VonMises.ToUnit(_stressUnit);
              break;
          }

          values = ResultsUtility.GetResultComponent(derivedStresses.Subset, derivedStressSelector, permutation);
          break;

        case FoldMode.StrainEnergy:
          switch (_disp) {
            case DisplayValue.X:
              IEntity1dResultSubset<IEntity1dStrainEnergyDensity, IEnergyDensity, Entity1dExtremaKey> strainEnergies =
                result.Element1dStrainEnergyDensities.ResultSubset(elementIds, positionsCount);
              _resType = "Strain Energy Density";
              dmax = strainEnergies.GetExtrema(strainEnergies.Max).EnergyDensity.As(_energyResultUnit);
              dmin = strainEnergies.GetExtrema(strainEnergies.Min).EnergyDensity.As(_energyResultUnit);
              Func<IEnergyDensity, IQuantity> strainEnergySelector = (r) => r.EnergyDensity.ToUnit(_energyResultUnit);
              values = ResultsUtility.GetResultComponent(strainEnergies.Subset, strainEnergySelector, permutation);
              break;

            default:
              INodeResultSubset<IEnergyDensity, NodeExtremaKey> averageStrainEnergies =
                result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);
              _resType = "Average Strain E Dens.";
              dmax = averageStrainEnergies.GetExtrema(averageStrainEnergies.Max).EnergyDensity.As(_energyResultUnit);
              dmin = averageStrainEnergies.GetExtrema(averageStrainEnergies.Min).EnergyDensity.As(_energyResultUnit);
              positionsCount = 2;
              values = new ConcurrentDictionary<int, List<IQuantity>>();
              Parallel.ForEach(averageStrainEnergies.Subset, kvp => values.TryAdd(kvp.Key, new List<IQuantity>() {
                  kvp.Value[permutation].EnergyDensity.ToUnit(_energyResultUnit),
                  kvp.Value[permutation].EnergyDensity.ToUnit(_energyResultUnit),
                }));
              break;
          }
          break;

        case FoldMode.Footfall:
          positionsCount = 2;
          _resType = "Response Factor [-]";
          INodeResultCache<IFootfall, ResultFootfall<NodeExtremaKey>> nodeFootfallCache
          = _selectedItems[1] == "Resonant" 
            ? result.NodeResonantFootfalls
            : result.NodeTransientFootfalls;
          Func<IFootfall, IQuantity> footfallSelector = 
            (r) => new Ratio(r.MaximumResponseFactor, RatioUnit.DecimalFraction);
          ICollection<int> nodeIds;
          (values, nodeIds) = ResultsUtility.MapNodeResultToElements(
            elems, nodeFootfallCache, footfallSelector, permutation);
          INodeResultSubset<IFootfall, ResultFootfall<NodeExtremaKey>> nodeFootfall 
            = nodeFootfallCache.ResultSubset(nodeIds);
          dmax = nodeFootfall.GetExtrema(nodeFootfall.Max.MaximumResponseFactor).MaximumResponseFactor;
          dmin = nodeFootfall.GetExtrema(nodeFootfall.Min.MaximumResponseFactor).MaximumResponseFactor;
          break;
      }

      if (values.IsNullOrEmpty()) {
        this.AddRuntimeError($"Model contains no results for elements in list '{elementlist}'");
        return;
      }

      int significantDigits = 0;
      if (customMinMax != Interval.Unset) {
        dmin = customMinMax.Min;
        dmax = customMinMax.Max;
        List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
        significantDigits = (int)rounded[2];
      } else {
        List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        significantDigits = (int)rounded[2];
      }

      var resultLines = new DataTree<LineResultGoo>();

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

          IQuantity t1 = values[key][i];
          IQuantity t2 = values[key][i + 1];

          var start = new Point3d(ln.PointAt((double)i / (positionsCount - 1)));
          var end = new Point3d(ln.PointAt((double)(i + 1) / (positionsCount - 1)));

          if (_mode == FoldMode.Displacement && (int)_disp < 4) {
            switch (_disp) {
              case DisplayValue.X:
                startTranslation.X = t1.As(lengthUnit) * _defScale;
                endTranslation.X = t2.As(lengthUnit) * _defScale;
                break;

              case DisplayValue.Y:
                startTranslation.Y = t1.As(lengthUnit) * _defScale;
                endTranslation.Y = t2.As(lengthUnit) * _defScale;
                break;

              case DisplayValue.Z:
                startTranslation.Z = t1.As(lengthUnit) * _defScale;
                endTranslation.Z = t2.As(lengthUnit) * _defScale;
                break;

              case DisplayValue.ResXyz:
                startTranslation.X = valuesXyz[key].x[i] * _defScale;
                startTranslation.Y = valuesXyz[key].y[i] * _defScale;
                startTranslation.Z = valuesXyz[key].z[i] * _defScale;
                endTranslation.X = valuesXyz[key].x[i + 1] * _defScale;
                endTranslation.Y = valuesXyz[key].y[i + 1] * _defScale;
                endTranslation.Z = valuesXyz[key].z[i + 1] * _defScale;
                break;
            }

            start.Transform(Transform.Translation(startTranslation));
            end.Transform(Transform.Translation(endTranslation));
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
          case FoldMode.Displacement when (int)_disp < 4: 
              var displacement = new Length(t, _lengthResultUnit);
              _legendValues.Add(displacement.ToString("f" + significantDigits));
              ts.Add(new GH_UnitNumber(displacement));
              break;
            
          case FoldMode.Displacement: 
              var rotation = new Angle(t, AngleUnit.Radian);
              _legendValues.Add(rotation.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(rotation));
              break;
            
          case FoldMode.Force when (int)_disp < 4: 
              var force = new Force(t, _forceUnit);
              _legendValues.Add(force.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(force));
              break;
            
          case FoldMode.Force: 
              var moment = new Moment(t, _momentUnit);
              _legendValues.Add(moment.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(moment));
              break;

          case FoldMode.ProjectedStress:
          case FoldMode.DerivedStress:
            var stress = new Pressure(t, _stressUnit);
            _legendValues.Add(stress.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(stress));
            break;

          case FoldMode.StrainEnergy: 
              var energy = new Energy(t, _energyResultUnit);
              _legendValues.Add(energy.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(energy));
              break;
            
          case FoldMode.Footfall: 
              var responseFactor = new Ratio(t, RatioUnit.DecimalFraction);
              _legendValues.Add(responseFactor.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(responseFactor));
              break;
        }

        if (Math.Abs(t) > 1) {
          // remove thousand separator
          _legendValues[i] = _legendValues[i].Replace(",", string.Empty); 
        }

        _legendValuesPosY.Add(_legend.Height - starty + (gripheight / 2) - 2);
      }

      da.SetDataTree(0, resultLines);
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);

      PostHog.Result(result.CaseType, 1, _mode.ToString(), _disp.ToString());
    }

    internal GH_GradientControl CreateGradient(GH_Document doc = null) {
      doc ??= Instances.ActiveCanvas.Document;
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

      doc.AddObject(gradient, false);
      Params.Input[3].RemoveAllSources();
      Params.Input[3].AddSource(gradient.Params.Output[0]);

      UpdateUI();
      return gradient;
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
      if (_mode == FoldMode.ProjectedStress) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.ProjectedStress;

      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    private void Mode4Clicked() {
      if (_mode == FoldMode.DerivedStress) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.DerivedStress;

      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    private void Mode5Clicked() {
      if (_mode == FoldMode.StrainEnergy) {
        return;
      }

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.StrainEnergy;

      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }

    private void Mode6Clicked() {
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

    internal void ShowLegend(object sender, EventArgs e) {
      _showLegend = !_showLegend;
      ExpirePreview(true);
    }

    internal void UpdateEnergy(string unit) {
      _energyResultUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateLength(string unit) {
      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateMoment(string unit) {
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateStress(string unit) {
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateLegendScale() {
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

    internal void MaintainScaleLegendText(ToolStripItem menuitem) {
      _scaleLegendTxt = menuitem.Text;
    }
  }
}
