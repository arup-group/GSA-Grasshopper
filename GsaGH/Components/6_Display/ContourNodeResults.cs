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
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Types.Transforms;
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
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to display GSA node result contours
  /// </summary>
  public class ContourNodeResults : GH_OasysDropDownComponent {
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
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ContourNodeResults;
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
    private string _case = string.Empty;
    private double _defScale = 250;
    private DisplayValue _disp = DisplayValue.ResXyz;
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

    public ContourNodeResults() : base("Contour Node Results", "NodeContour",
      "Diplays GSA Node Results as Contours", CategoryName.Name(), SubCategoryName.Cat6()) { }

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
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      if (reader.ItemExists("legendScale")) {
        _legendScale = reader.GetDouble("legendScale");
      }

      _showLegend = reader.GetBoolean("legend");
      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      _legend = new Bitmap((int)(15 * _legendScale), (int)(120 * _legendScale));
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
              if (_dropDownItems[1] != _reaction) {
                _dropDownItems[1] = _reaction;
                _selectedItems[0] = _dropDownItems[0][1];
                _selectedItems[1] = _dropDownItems[1][3];
                _disp = DisplayValue.ResXyz;
                Mode2Clicked();
              }

              break;

            case 2:
              if (_dropDownItems[1] != _footfall) {
                _dropDownItems[1] = _footfall;
                _selectedItems[0] = _dropDownItems[0][2];
                _selectedItems[1] = _dropDownItems[1][0];
                _disp = DisplayValue.X;
                Mode3Clicked();
              }

              break;

          }

          break;

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
        Params.Input[3].Description = "Optional Domain for custom Min to Max contour colours";
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
      writer.SetDouble("legendScale", _legendScale);
      writer.SetBoolean("legend", _showLegend);
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

      ToolStripMenuItem lengthUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Displacement",
        EngineeringUnits.Length, Length.GetAbbreviation(_lengthResultUnit), UpdateLength);

      ToolStripMenuItem forceUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Force",
        EngineeringUnits.Force, Force.GetAbbreviation(_forceUnit), UpdateForce);

      ToolStripMenuItem momentUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Moment",
        EngineeringUnits.Moment, Moment.GetAbbreviation(_momentUnit), UpdateMoment);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        forceUnitsMenu,
        momentUnitsMenu,
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

        case FoldMode.Reaction:
          Message = (int)_disp < 4 ? Force.GetAbbreviation(_forceUnit) :
            Moment.GetAbbreviation(_momentUnit);
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
      pManager.AddParameter(new GsaNodeListParameter());
      pManager[1].Optional = true;
      pManager.AddColourParameter("Colour", "Co",
        "Optional list of colours to override default colours." + Environment.NewLine
        + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[2].Optional = true;
      pManager.AddIntervalParameter("Min/Max Domain", "I",
        "Optional Domain for custom Min to Max contour colours", GH_ParamAccess.item);
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

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult2 result;
      string nodeList = "All";
      _case = string.Empty;
      _resType = string.Empty;

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      switch (ghTyp?.Value) {
        case GsaResultGoo goo:
          result = new GsaResult2((GsaResult)goo.Value);
          nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);
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

      ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes(nodeList);
      if (nodes.Count == 0) {
        this.AddRuntimeError($"Model contains no results for nodes in list '{nodeList}'");
        return;
      }
      LengthUnit lengthUnit = result.Model.ModelUnit;

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

      ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
      int permutation = result.SelectedPermutationIds == null
        ? 0 : result.SelectedPermutationIds[0] - 1;
      double dmax = 0;
      double dmin = 0;
      var values = new ConcurrentDictionary<int, IQuantity>();
      ConcurrentDictionary<int, (double x, double y, double z)> valuesXyz = null;
      switch (_mode) {
        case FoldMode.Displacement:
          INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> displacements 
            = result.NodeDisplacements.ResultSubset(nodeIds);

          if (displacements.Ids.Count == 0) {
            this.AddRuntimeWarning($"{result.CaseType} {result.CaseId} contains no Displacement results.");
            return;
          }

          switch (_disp) {
            case DisplayValue.X:
              _resType = "Translation, Ux";
              dmax = displacements.GetExtrema(displacements.Max.X).X.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.X).X.As(_lengthResultUnit);
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].X.ToUnit(_lengthResultUnit)));
              break;

            case DisplayValue.Y:
              _resType = "Translation, Uy";
              dmax = displacements.GetExtrema(displacements.Max.Y).Y.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Y).Y.As(_lengthResultUnit);
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Y.ToUnit(_lengthResultUnit)));
              break;

            case DisplayValue.Z:
              _resType = "Translation, Uz";
              dmax = displacements.GetExtrema(displacements.Max.Z).Z.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Z).Z.As(_lengthResultUnit);
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Z.ToUnit(_lengthResultUnit)));
              break;

            case DisplayValue.ResXyz:
              _resType = "Translation, |U|";
              dmax = displacements.GetExtrema(displacements.Max.Xyz).Xyz.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Xyz).Xyz.As(_lengthResultUnit);
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Xyz.ToUnit(_lengthResultUnit)));
              valuesXyz = new ConcurrentDictionary<int, (double x, double y, double z)>();
              Parallel.ForEach(displacements.Subset, kvp =>
                valuesXyz.TryAdd(kvp.Key, (
                kvp.Value[permutation].X.As(lengthUnit),
                kvp.Value[permutation].Y.As(lengthUnit),
                kvp.Value[permutation].Z.As(lengthUnit))));
              break;

            case DisplayValue.Xx:
              _resType = "Rotation, Rxx";
              dmax = displacements.GetExtrema(displacements.Max.Xx).Xx.Value;
              dmin = displacements.GetExtrema(displacements.Min.Xx).Xx.Value;
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Xx));
              break;

            case DisplayValue.Yy:
              _resType = "Rotation, Ryy";
              dmax = displacements.GetExtrema(displacements.Max.Yy).Yy.Value;
              dmin = displacements.GetExtrema(displacements.Min.Yy).Yy.Value;
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Yy));
              break;

            case DisplayValue.Zz:
              _resType = "Rotation, Rzz";
              dmax = displacements.GetExtrema(displacements.Max.Zz).Zz.Value;
              dmin = displacements.GetExtrema(displacements.Min.Zz).Zz.Value;
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Zz));
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Rotation, |R|";
              dmax = displacements.GetExtrema(displacements.Max.Xxyyzz).Xxyyzz.Value;
              dmin = displacements.GetExtrema(displacements.Min.Xxyyzz).Xxyyzz.Value;
              Parallel.ForEach(displacements.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Xxyyzz));
              break;
          }

          break;

        case FoldMode.Reaction:
          INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> reactions
            = result.NodeReactionForces.ResultSubset(nodeIds);

          if (reactions.Ids.Count == 0) {
            this.AddRuntimeWarning($"{result.CaseType} {result.CaseId} contains no Reaction force results.");
            return;
          }

          switch (_disp) {
            case DisplayValue.X:
              _resType = "Reaction Force, Fx";
              dmax = reactions.GetExtrema(reactions.Max.X).X.As(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.X).X.As(_forceUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].X.ToUnit(_forceUnit)));
              break;

            case DisplayValue.Y:
              _resType = "Reaction Force, Fy";
              dmax = reactions.GetExtrema(reactions.Max.Y).Y.As(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.Y).Y.As(_forceUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Y.ToUnit(_forceUnit)));
              break;

            case DisplayValue.Z:
              _resType = "Reaction Force, Fz";
              dmax = reactions.GetExtrema(reactions.Max.Z).Z.As(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.Z).Z.As(_forceUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Z.ToUnit(_forceUnit)));
              break;

            case DisplayValue.ResXyz:
              _resType = "Reaction Force, |F|";
              dmax = reactions.GetExtrema(reactions.Max.Xyz).Xyz.As(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.Xyz).Xyz.As(_forceUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Xyz.ToUnit(_forceUnit)));
              break;

            case DisplayValue.Xx:
              _resType = "Reaction Moment, Mxx";
              dmax = reactions.GetExtrema(reactions.Max.Xx).Xx.As(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Xx).Xx.As(_momentUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Xx.ToUnit(_momentUnit)));
              break;

            case DisplayValue.Yy:
              _resType = "Reaction Moment, Myy";
              dmax = reactions.GetExtrema(reactions.Max.Yy).Yy.As(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Yy).Yy.As(_momentUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Yy.ToUnit(_momentUnit)));
              break;

            case DisplayValue.Zz:
              _resType = "Reaction Moment, Mzz";
              dmax = reactions.GetExtrema(reactions.Max.Zz).Zz.As(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Zz).Zz.As(_momentUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Zz.ToUnit(_momentUnit)));
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Reaction Moment, |M|";
              dmax = reactions.GetExtrema(reactions.Max.Xxyyzz).Xxyyzz.As(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Xxyyzz).Xxyyzz.As(_momentUnit);
              Parallel.ForEach(reactions.Subset, kvp =>
                values.TryAdd(kvp.Key, kvp.Value[permutation].Xxyyzz.ToUnit(_momentUnit)));
              break;
          }

          break;

        case FoldMode.Footfall:
          _resType = "Response Factor [-]";
          INodeResultSubset<IFootfall, ResultFootfall<NodeExtremaKey>> footfall = null;
          switch (_selectedItems[1]) {
            case "Resonant":
              footfall = result.NodeResonantFootfalls.ResultSubset(nodeIds);
              break;

            case "Transient":
              footfall = result.NodeTransientFootfalls.ResultSubset(nodeIds);
              break;
          }

          if (footfall.Ids.Count == 0) {
            this.AddRuntimeWarning($"{result.CaseType} {result.CaseId} contains no Footfall results.");
            return;
          }

          dmax = footfall.GetExtrema(footfall.Max.MaximumResponseFactor).MaximumResponseFactor;
          dmin = footfall.GetExtrema(footfall.Min.MaximumResponseFactor).MaximumResponseFactor;
          Parallel.ForEach(footfall.Subset, kvp =>
            values.TryAdd(kvp.Key, 
            new Ratio(kvp.Value[permutation].MaximumResponseFactor, RatioUnit.DecimalFraction)));
          break;
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

      var pts = new ConcurrentDictionary<int, PointResultGoo>();
      ConcurrentDictionary<int, GsaNodeGoo> gsanodes = Nodes.GetNodeDictionary(nodes, lengthUnit);

      Parallel.ForEach(gsanodes, kvp => {
        if (kvp.Value.Value == null) {
          return;
        }

        if (!values.ContainsKey(kvp.Key) || (dmin == 0) & (dmax == 0)) {
          return;
        }

        var pt = new Point3d(kvp.Value.Value.Point);
        IQuantity t = values[kvp.Key];
        if (_mode == FoldMode.Displacement) {
          var translation = new Vector3d(0, 0, 0);
          switch (_disp) {
            case DisplayValue.X:
              translation.X = values[kvp.Key].As(lengthUnit) * _defScale;
              break;

            case DisplayValue.Y:
              translation.Y = values[kvp.Key].As(lengthUnit) * _defScale;
              break;

            case DisplayValue.Z:
              translation.Z = values[kvp.Key].As(lengthUnit) * _defScale;
              break;

            case DisplayValue.ResXyz:
              translation.X = valuesXyz[kvp.Key].x * _defScale;
              translation.Y = valuesXyz[kvp.Key].y * _defScale;
              translation.Z = valuesXyz[kvp.Key].z * _defScale;
              break;
          }

          pt.Transform(Transform.Translation(translation));
        }

        double tnorm = (2 * (t.Value - dmin) / (dmax - dmin)) - 1;
        Color valcol = ghGradient.ColourAt(tnorm);
        float size = t.Value >= 0 && dmax != 0 ? Math.Max(2, (float)(t.Value / dmax * scale)) :
          Math.Max(2, (float)(Math.Abs(t.Value) / Math.Abs(dmin) * scale));

        pts[kvp.Key] = new PointResultGoo(pt, t, valcol, size, kvp.Key);
      });


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
              Message = string.Empty;
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
      PostHog.Result(result.CaseType, 0, resultType, _disp.ToString());
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
        Params.Input[2].Attributes.Bounds.Y - (gradient.Attributes.Bounds.Height / 4) - 6);

      doc.AddObject(gradient, false);
      Params.Input[2].RemoveAllSources();
      Params.Input[2].AddSource(gradient.Params.Output[0]);

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

    internal void ShowLegend(object sender, EventArgs e) {
      _showLegend = !_showLegend;
      ExpirePreview(true);
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
