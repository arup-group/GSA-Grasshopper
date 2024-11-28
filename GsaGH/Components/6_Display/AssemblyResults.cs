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
using Grasshopper.GUI;
using Grasshopper.GUI.Gradient;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
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

using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Assembly results
  /// </summary>
  public class AssemblyResults : GH_OasysDropDownComponent {
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
      Drift,
      DriftIndex,
      Force
    }

    public override Guid ComponentGuid => new Guid("15d88d76-6dfc-434b-928f-687b91d52002");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AssemblyResults;
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
    private readonly List<string> _drift = new List<string>(new[] {
      "Drift Dx",
      "Drift Dy",
      "In-plane Drift"
    });
    private readonly List<string> _driftIndex = new List<string>(new[] {
      "Drift Index DIx",
      "Drift Index DIy",
      "In-plane Drift Index"
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
    private readonly List<string> _type = new List<string>(new[] {
      "Displacement",
      "Drift",
      "Drift Index",
      "Force"
    });
    private string _case = string.Empty;
    private double _defScale = 250;
    private DisplayValue _disp = DisplayValue.ResXyz;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private double _maxValue = 1000;
    private double _minValue;
    private FoldMode _mode = FoldMode.Displacement;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private int _noDigits;
    private string _resType;
    private string _scaleLegendTxt = string.Empty;
    private bool _slider = true;
    private EnvelopeMethod _envelopeType = EnvelopeMethod.Absolute;
    private readonly Legend _legend = new Legend() {
      Bitmap = new Bitmap(15, 120),
      Scale = 1,
      Visible = true,
    };

    public AssemblyResults() : base("Assembly Results", "Assembly",
      "Displays GSA Assembly Results as Contour", CategoryName.Name(), SubCategoryName.Cat6()) { }

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
      DrawContour.DrawViewportWires(args, _legend, _resType, _case);
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _legend.Visible = reader.GetBoolean("legend");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      if (reader.ItemExists("legendScale")) {
        _legend.Scale = reader.GetDouble("legendScale");
      }

      if (reader.ItemExists("envelope")) {
        _envelopeType = (EnvelopeMethod)Enum.Parse(
          typeof(EnvelopeMethod), reader.GetString("envelope"));
      }

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
        case 0:
          switch (j) {
            case 0:
              if (_dropDownItems[1] != _displacement) {
                _dropDownItems[1] = _displacement;

                _selectedItems[0] = _dropDownItems[0][0];
                _selectedItems[1] = _dropDownItems[1][3];

                _disp = DisplayValue.ResXyz;
                DisplacementModeClicked();
              }

              break;

            case 1:
              if (_dropDownItems[1] != _drift) {
                _dropDownItems[1] = _drift;

                _selectedItems[0] = _dropDownItems[0][1];
                _selectedItems[1] = _dropDownItems[1][0];

                _disp = DisplayValue.X;
                DriftModeClicked();
              }

              break;

            case 2:
              if (_dropDownItems[1] != _driftIndex) {
                _dropDownItems[1] = _driftIndex;

                _selectedItems[0] = _dropDownItems[0][2];
                _selectedItems[1] = _dropDownItems[1][0];

                _disp = DisplayValue.X;
                DriftIndexModeClicked();
              }

              break;

            case 3:
              if (_dropDownItems[1] != _force) {
                _dropDownItems[1] = _force;

                _selectedItems[0] = _dropDownItems[0][3];
                _selectedItems[1] = _dropDownItems[1][5];

                _disp = DisplayValue.Yy;
                ForceModeClicked();
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
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetBoolean("slider", _slider);
      writer.SetBoolean("legend", _legend.Visible);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetDouble("legendScale", _legend.Scale);
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      writer.SetString("envelope", _envelopeType.ToString());
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      ToolStripMenuItem envelopeMenu = GenerateToolStripMenuItem.GetEnvelopeSubMenuItem(_envelopeType, UpdateEnvelope);
      menu.Items.Add(envelopeMenu);

      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _legend.Visible);

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
        momentUnitsMenu
      });

      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      var legendScale = new ToolStripTextBox {
        Text = _legend.Scale.ToString(),
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

        case FoldMode.Drift:
          Message = Length.GetAbbreviation(_lengthResultUnit);
          break;

        case FoldMode.DriftIndex:
          Message = "";
          break;

        case FoldMode.Force:
          Message = (int)_disp < 4 ? Force.GetAbbreviation(_forceUnit) :
            Moment.GetAbbreviation(_momentUnit);
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
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddParameter(new GsaAssemblyListParameter());
      pManager[1].Optional = true;
      pManager.AddColourParameter("Colour", "Co",
        "[Optional] List of colours to override default colours" + Environment.NewLine
        + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[2].Optional = true;
      pManager.AddIntervalParameter("Min/Max Domain", "I",
        "Optional Domain for custom Min to Max contour colours", GH_ParamAccess.item);
      pManager[3].Optional = true;
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size",
        GH_ParamAccess.item, 10);
      pManager[4].Optional = true;
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
      GsaResult result;
      string assemblyList = "All";
      _case = string.Empty;
      _resType = string.Empty;
      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);
      result = Inputs.GetResultInput(this, ghTyp);
      if (result == null) {
        return;
      }

      bool enveloped = Inputs.IsResultCaseEnveloped(this, result, ref _case, _envelopeType);
      List<int> permutations = result.SelectedPermutationIds;
      assemblyList = Inputs.GetAssemblyListDefinition(this, da, 1, result.Model);
      ReadOnlyDictionary<int, Element> elems = result.Model.ApiModel.Elements();
      ReadOnlyDictionary<int, Node> nodes = result.Model.ApiModel.Nodes();
      ReadOnlyDictionary<int, Assembly> assemblies = result.Model.ApiModel.Assemblies();

      ReadOnlyCollection<int> assemblyIds = result.AssemblyIds(assemblyList);

      var filteredAssemblies = new Dictionary<int, Assembly>();
      foreach (KeyValuePair<int, Assembly> assembly in assemblies) {
        if (assemblyIds.Contains(assembly.Key)) {
          filteredAssemblies.Add(assembly.Key, assembly.Value);
        }
      }

      if (filteredAssemblies.Count == 0) {
        this.AddRuntimeError($"Model contains no results for assemblies in list '{assemblyList}'");
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

      double dmax = 0;
      double dmin = 0;
      ConcurrentDictionary<int, IList<IQuantity>> values = null;
      ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)> valuesXyz = null;
      switch (_mode) {
        case FoldMode.Displacement:
          Parameters.Results.AssemblyDisplacements displacements = result.AssemblyDisplacements.ResultSubset(assemblyIds);
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

              valuesXyz = ResultsUtility.GetResultResultantTranslation(
                displacements.Subset, lengthUnit, permutations, _envelopeType);
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

          values = ResultsUtility.GetResultComponent(
            displacements.Subset, displacementSelector, permutations, _envelopeType);
          break;

        case FoldMode.Drift:
          Parameters.Results.AssemblyDrifts drifts = result.AssemblyDrifts.ResultSubset(assemblyIds);
          Func<Drift, IQuantity> driftSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Drift, Dx";
              dmax = drifts.GetExtrema(drifts.Max.X).X.As(_lengthResultUnit);
              dmin = drifts.GetExtrema(drifts.Min.X).X.As(_lengthResultUnit);
              driftSelector = (r) => r.X.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.Y:
              _resType = "Drift, Dy";
              dmax = drifts.GetExtrema(drifts.Max.Y).Y.As(_lengthResultUnit);
              dmin = drifts.GetExtrema(drifts.Min.Y).Y.As(_lengthResultUnit);
              driftSelector = (r) => r.Y.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.Z:
              _resType = "In-plane Drift, Dxy";
              dmax = drifts.GetExtrema(drifts.Max.Xy).Xy.As(_lengthResultUnit);
              dmin = drifts.GetExtrema(drifts.Min.Xy).Xy.As(_lengthResultUnit);
              driftSelector = (r) => r.Xy.ToUnit(_lengthResultUnit);
              break;
          }

          values = ResultsUtility.GetResultComponent(
            drifts.Subset, driftSelector, permutations, _envelopeType);
          break;

        case FoldMode.DriftIndex:
          Parameters.Results.AssemblyDriftIndices driftIndices = result.AssemblyDriftIndices.ResultSubset(assemblyIds);
          Func<DriftIndex, IQuantity> driftIndexSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Drift Index, DIx";
              dmax = driftIndices.GetExtrema(driftIndices.Max.X).X.Value;
              dmin = driftIndices.GetExtrema(driftIndices.Min.X).X.Value;
              driftIndexSelector = (r) => r.X;
              break;

            case DisplayValue.Y:
              _resType = "Drift Index, DIy";
              dmax = driftIndices.GetExtrema(driftIndices.Max.Y).Y.Value;
              dmin = driftIndices.GetExtrema(driftIndices.Min.Y).Y.Value;
              driftIndexSelector = (r) => r.Y;
              break;

            case DisplayValue.Z:
              _resType = "In-plane Drift Index, DIxy";
              dmax = driftIndices.GetExtrema(driftIndices.Max.Xy).Xy.Value;
              dmin = driftIndices.GetExtrema(driftIndices.Min.Xy).Xy.Value;
              driftIndexSelector = (r) => r.Xy;
              break;
          }

          values = ResultsUtility.GetResultComponent(
            driftIndices.Subset, driftIndexSelector, permutations, _envelopeType);
          break;

        case FoldMode.Force:
          Parameters.Results.AssemblyForcesAndMoments forces =
          result.AssemblyForcesAndMoments.ResultSubset(assemblyIds);
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

          values = ResultsUtility.GetResultComponent(forces.Subset, forceSelector, permutations, _envelopeType);
          break;
      }

      if (values.IsNullOrEmpty()) {
        this.AddRuntimeError($"Model contains no results for assemblies in list '{assemblyList}'");
        return;
      }

      int significantDigits = 0;
      if (customMinMax != Interval.Unset) {
        dmin = customMinMax.Min;
        dmax = customMinMax.Max;
        List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
        significantDigits = (int)rounded[2];
      } else {
        if (enveloped) {
          dmax = values.Values.Select(x => x.Max()).Max().Value;
          dmin = values.Values.Select(x => x.Min()).Min().Value;
        }

        List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        significantDigits = (int)rounded[2];
      }

      var resultLines = new DataTree<LineResultGoo>();

      Parallel.ForEach(filteredAssemblies, assembly => {
        Node topology1 = nodes[assembly.Value.Topology1];
        Node topology2 = nodes[assembly.Value.Topology2];

        var ln = new Line(
          Nodes.Point3dFromNode(topology1, lengthUnit), // start point
          Nodes.Point3dFromNode(topology2, lengthUnit)); // end point

        int key = assembly.Key;

        int positionsCount = values[key].Count;

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

      int gripheight = _legend.Bitmap.Height / ghGradient.GripCount;
      _legend.Values = new List<string>();
      _legend.ValuesPositionY = new List<int>();

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
          for (int x = 0; x < _legend.Bitmap.Width; x++) {
            _legend.Bitmap.SetPixel(x, _legend.Bitmap.Height - y - 1, gradientcolour);
          }
        }

        switch (_mode) {
          case FoldMode.Displacement when (int)_disp < 4:
            var displacement = new Length(t, _lengthResultUnit);
            _legend.Values.Add(displacement.ToString("f" + significantDigits));
            ts.Add(new GH_UnitNumber(displacement));
            break;

          case FoldMode.Displacement:
            var rotation = new Angle(t, AngleUnit.Radian);
            _legend.Values.Add(rotation.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(rotation));
            break;

          case FoldMode.Drift:
            var drift = new Length(t, _lengthResultUnit);
            _legend.Values.Add(drift.ToString("f" + significantDigits));
            ts.Add(new GH_UnitNumber(drift));
            break;

          case FoldMode.DriftIndex:
            var driftIndex = new Ratio(t, RatioUnit.DecimalFraction);
            _legend.Values.Add(driftIndex.ToString("f" + significantDigits));
            ts.Add(new GH_UnitNumber(driftIndex));
            break;

          case FoldMode.Force when (int)_disp < 4:
            var force = new Force(t, _forceUnit);
            _legend.Values.Add(force.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(force));
            break;

          case FoldMode.Force:
            var moment = new Moment(t, _momentUnit);
            _legend.Values.Add(moment.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(moment));
            break;
        }

        if (Math.Abs(t) > 1) {
          // remove thousand separator
          _legend.Values[i] = _legend.Values[i].Replace(",", string.Empty);
        }

        _legend.ValuesPositionY.Add(_legend.Bitmap.Height - starty + (gripheight / 2) - 2);
      }

      da.SetDataTree(0, resultLines);
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);

      PostHog.Result(result.CaseType, 1, _mode.ToString(), _disp.ToString());
    }

    internal GH_GradientControl CreateGradient(GH_Document doc = null) {
      doc ??= OnPingDocument();
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

    private void DisplacementModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Displacement;
      _slider = true;
      _defScale = 100;
      ReDrawComponent();
    }

    private void DriftModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Drift;
      _slider = true;
      _defScale = 100;
      ReDrawComponent();
    }

    private void DriftIndexModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.DriftIndex;
      _slider = true;
      _defScale = 100;
      ReDrawComponent();
    }

    private void ForceModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Force;
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
      _legend.ToggleShowLegend();
      ExpirePreview(true);
    }

    internal void UpdateEnvelope(string type) {
      _envelopeType = (EnvelopeMethod)Enum.Parse(typeof(EnvelopeMethod), type);
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

    internal void UpdateLegendScale() {
      try {
        _legend.Scale = double.Parse(_scaleLegendTxt);
      } catch (Exception e) {
        this.AddRuntimeWarning(e.Message);
        return;
      }

      _legend.CreateNewBitmap(15, 120);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void MaintainScaleLegendText(ToolStripItem menuitem) {
      _scaleLegendTxt = menuitem.Text;
    }
  }
}
