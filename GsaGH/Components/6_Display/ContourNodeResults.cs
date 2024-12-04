using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.GUI.Gradient;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
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
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to display GSA node result contours
  /// </summary>
  public class ContourNodeResults : DrawContourBase {
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
      SpringForce
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
    private readonly List<string> _springForce = new List<string>(new[] {
      "Axial Fx",
      "Shear Fy",
      "Shear Fz",
      "Resolved |F|",
      "Torsion Mxx",
      "Moment Myy",
      "Moment Mzz",
      "Resolved |M|",
    });
    private readonly List<string> _type = new List<string>(new[] {
      "Displacement",
      "Reaction",
      "SpringForce",
      "Footfall",
    });
    private DisplayValue _disp = DisplayValue.ResXyz;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private FoldMode _mode = FoldMode.Displacement;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;

    public ContourNodeResults() : base("Contour Node Results", "NodeContour",
      "Diplays GSA Node Results as Contours", CategoryName.Name(), SubCategoryName.Cat6()) { }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));

      return base.Read(reader);
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
              if (_dropDownItems[1] != _reaction) {
                _dropDownItems[1] = _reaction;
                _selectedItems[0] = _dropDownItems[0][1];
                _selectedItems[1] = _dropDownItems[1][3];
                _disp = DisplayValue.ResXyz;
                ReactionModeClicked();
              }

              break;

            case 2:
              if (_dropDownItems[1] != _springForce) {
                _dropDownItems[1] = _springForce;
                _selectedItems[0] = _dropDownItems[0][2];
                _selectedItems[1] = _dropDownItems[1][3];
                _disp = DisplayValue.ResXyz;
                SpringForceClicked();
              }

              break;

            case 3:
              if (_dropDownItems[1] != _footfall) {
                _dropDownItems[1] = _footfall;
                _selectedItems[0] = _dropDownItems[0][3];
                _selectedItems[1] = _dropDownItems[1][0];
                _disp = DisplayValue.X;
                FootfallModeClicked();
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
        case FoldMode.SpringForce when (int)_disp < 4:
          Params.Output[2].Name = "Values [" + Force.GetAbbreviation(_forceUnit) + "]";
          break;

        case FoldMode.Reaction:
        case FoldMode.SpringForce:
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
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));

      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      ToolStripMenuItem forceUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Force", EngineeringUnits.Force,
        Force.GetAbbreviation(_forceUnit), UpdateForce);

      ToolStripMenuItem momentUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Moment",
        EngineeringUnits.Moment, Moment.GetAbbreviation(_momentUnit), UpdateMoment);

      GenerateUnitMenuItems(new List<ToolStripMenuItem>() {
        forceUnitsMenu,
        momentUnitsMenu,
      });
      base.AppendAdditionalComponentMenuItems(menu);
    }

    protected override void BeforeSolveInstance() {
      switch (_mode) {
        case FoldMode.Displacement:
          Message = (int)_disp < 4 ? Length.GetAbbreviation(_lengthResultUnit) :
            Angle.GetAbbreviation(AngleUnit.Radian);
          break;

        case FoldMode.Reaction:
        case FoldMode.SpringForce:
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
      pManager.AddGenericParameter("Result Point", "P", "Contoured Points with result values", GH_ParamAccess.list);
      base.RegisterOutputParams(pManager);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      ClearLegendGradients();
      string nodeList = "All";
      _case = string.Empty;
      _resType = string.Empty;
      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);
      GsaResult result = Inputs.GetResultInput(this, ghTyp);
      if (result == null) {
        return;
      }

      bool enveloped = Inputs.IsResultCaseEnveloped(this, result, ref _case, _envelopeType);
      List<int> permutations = result.SelectedPermutationIds;
      nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);
      ReadOnlyDictionary<int, Node> nodes = result.Model.ApiModel.Nodes(nodeList);
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
      double? dmax = 0;
      double? dmin = 0;
      var values = new ConcurrentDictionary<int, IQuantity>();
      ConcurrentDictionary<int, (double x, double y, double z)> valuesXyz = null;
      switch (_mode) {
        case FoldMode.Displacement:
          IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> displacements
            = result.NodeDisplacements.ResultSubset(nodeIds);

          if (displacements.Ids.Count == 0) {
            this.AddRuntimeError($"Model contains no results for nodes in list '{nodeList}'");
            return;
          }

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
              dmax = displacements.GetExtrema(displacements.Max.Xx).Xx.Value;
              dmin = displacements.GetExtrema(displacements.Min.Xx).Xx.Value;
              displacementSelector = (r) => r.Xx;
              break;

            case DisplayValue.Yy:
              _resType = "Rotation, Ryy";
              dmax = displacements.GetExtrema(displacements.Max.Yy).Yy.Value;
              dmin = displacements.GetExtrema(displacements.Min.Yy).Yy.Value;
              displacementSelector = (r) => r.Yy;
              break;

            case DisplayValue.Zz:
              _resType = "Rotation, Rzz";
              dmax = displacements.GetExtrema(displacements.Max.Zz).Zz.Value;
              dmin = displacements.GetExtrema(displacements.Min.Zz).Zz.Value;
              displacementSelector = (r) => r.Zz;
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Rotation, |R|";
              dmax = displacements.GetExtrema(displacements.Max.Xxyyzz).Xxyyzz.Value;
              dmin = displacements.GetExtrema(displacements.Min.Xxyyzz).Xxyyzz.Value;
              displacementSelector = (r) => r.Xxyyzz;
              break;
          }

          values = ResultsUtility.GetResultComponent(
            displacements.Subset, displacementSelector, permutations, _envelopeType);
          break;

        case FoldMode.Reaction:
          IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> reactions
            = result.NodeReactionForces.ResultSubset(nodeIds);

          if (reactions.Ids.Count == 0) {
            this.AddRuntimeError($"Model contains no results for nodes in list '{nodeList}'");
            return;
          }

          Func<IReactionForce, IQuantity> forceSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Reaction Force, Fx";
              dmax = reactions.GetExtrema(reactions.Max.X).XAs(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.X).XAs(_forceUnit);
              forceSelector = (r) => r.XToUnit(_forceUnit);
              break;

            case DisplayValue.Y:
              _resType = "Reaction Force, Fy";
              dmax = reactions.GetExtrema(reactions.Max.Y).YAs(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.Y).YAs(_forceUnit);
              forceSelector = (r) => r.YToUnit(_forceUnit);
              break;

            case DisplayValue.Z:
              _resType = "Reaction Force, Fz";
              dmax = reactions.GetExtrema(reactions.Max.Z).ZAs(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.Z).ZAs(_forceUnit);
              forceSelector = (r) => r.ZToUnit(_forceUnit);
              break;

            case DisplayValue.ResXyz:
              _resType = "Reaction Force, |F|";
              dmax = reactions.GetExtrema(reactions.Max.Xyz).XyzAs(_forceUnit);
              dmin = reactions.GetExtrema(reactions.Min.Xyz).XyzAs(_forceUnit);
              forceSelector = (r) => r.XyzToUnit(_forceUnit);
              break;

            case DisplayValue.Xx:
              _resType = "Reaction Moment, Mxx";
              dmax = reactions.GetExtrema(reactions.Max.Xx).XxAs(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Xx).XxAs(_momentUnit);
              forceSelector = (r) => r.XxToUnit(_momentUnit);
              break;

            case DisplayValue.Yy:
              _resType = "Reaction Moment, Myy";
              dmax = reactions.GetExtrema(reactions.Max.Yy).YyAs(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Yy).YyAs(_momentUnit);
              forceSelector = (r) => r.YyToUnit(_momentUnit);
              break;

            case DisplayValue.Zz:
              _resType = "Reaction Moment, Mzz";
              dmax = reactions.GetExtrema(reactions.Max.Zz).ZzAs(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Zz).ZzAs(_momentUnit);
              forceSelector = (r) => r.ZzToUnit(_momentUnit);
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Reaction Moment, |M|";
              dmax = reactions.GetExtrema(reactions.Max.Xxyyzz).XxyyzzAs(_momentUnit);
              dmin = reactions.GetExtrema(reactions.Min.Xxyyzz).XxyyzzAs(_momentUnit);
              forceSelector = (r) => r.XxyyzzToUnit(_momentUnit);
              break;
          }

          values = ResultsUtility.GetResultComponent(
            reactions.Subset, forceSelector, permutations, _envelopeType);
          break;

        case FoldMode.SpringForce:
          IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> springForces
            = result.NodeSpringForces.ResultSubset(nodeIds);

          if (springForces.Ids.Count == 0) {
            this.AddRuntimeError($"Model contains no results for nodes in list '{nodeList}'");
            return;
          }

          Func<IReactionForce, IQuantity> springSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Reaction Force, Fx";
              dmax = springForces.GetExtrema(springForces.Max.X).XAs(_forceUnit);
              dmin = springForces.GetExtrema(springForces.Min.X).XAs(_forceUnit);
              springSelector = (r) => r.XToUnit(_forceUnit);
              break;

            case DisplayValue.Y:
              _resType = "Reaction Force, Fy";
              dmax = springForces.GetExtrema(springForces.Max.Y).YAs(_forceUnit);
              dmin = springForces.GetExtrema(springForces.Min.Y).YAs(_forceUnit);
              springSelector = (r) => r.YToUnit(_forceUnit);
              break;

            case DisplayValue.Z:
              _resType = "Reaction Force, Fz";
              dmax = springForces.GetExtrema(springForces.Max.Z).ZAs(_forceUnit);
              dmin = springForces.GetExtrema(springForces.Min.Z).ZAs(_forceUnit);
              springSelector = (r) => r.ZToUnit(_forceUnit);
              break;

            case DisplayValue.ResXyz:
              _resType = "Reaction Force, |F|";
              dmax = springForces.GetExtrema(springForces.Max.Xyz).XyzAs(_forceUnit);
              dmin = springForces.GetExtrema(springForces.Min.Xyz).XyzAs(_forceUnit);
              springSelector = (r) => r.XyzToUnit(_forceUnit);
              break;

            case DisplayValue.Xx:
              _resType = "Reaction Moment, Mxx";
              dmax = springForces.GetExtrema(springForces.Max.Xx).XxAs(_momentUnit);
              dmin = springForces.GetExtrema(springForces.Min.Xx).XxAs(_momentUnit);
              springSelector = (r) => r.XxToUnit(_momentUnit);
              break;

            case DisplayValue.Yy:
              _resType = "Reaction Moment, Myy";
              dmax = springForces.GetExtrema(springForces.Max.Yy).YyAs(_momentUnit);
              dmin = springForces.GetExtrema(springForces.Min.Yy).YyAs(_momentUnit);
              springSelector = (r) => r.YyToUnit(_momentUnit);
              break;

            case DisplayValue.Zz:
              _resType = "Reaction Moment, Mzz";
              dmax = springForces.GetExtrema(springForces.Max.Zz).ZzAs(_momentUnit);
              dmin = springForces.GetExtrema(springForces.Min.Zz).ZzAs(_momentUnit);
              springSelector = (r) => r.ZzToUnit(_momentUnit);
              break;

            case DisplayValue.ResXxyyzz:
              _resType = "Reaction Moment, |M|";
              dmax = springForces.GetExtrema(springForces.Max.Xxyyzz).XxyyzzAs(_momentUnit);
              dmin = springForces.GetExtrema(springForces.Min.Xxyyzz).XxyyzzAs(_momentUnit);
              springSelector = (r) => r.XxyyzzToUnit(_momentUnit);
              break;
          }

          values = ResultsUtility.GetResultComponent(
            springForces.Subset, springSelector, permutations, _envelopeType);
          break;

        case FoldMode.Footfall:
          _resType = "Response Factor [-]";
          IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> footfall = null;
          switch (_selectedItems[1]) {
            case "Resonant":
              footfall = result.NodeResonantFootfalls.ResultSubset(nodeIds);
              break;

            case "Transient":
              footfall = result.NodeTransientFootfalls.ResultSubset(nodeIds);
              break;
          }

          if (footfall.Ids.Count == 0) {
            this.AddRuntimeError($"Model contains no results for nodes in list '{nodeList}'");
            return;
          }

          dmax = footfall.GetExtrema(footfall.Max.MaximumResponseFactor).MaximumResponseFactor;
          dmin = footfall.GetExtrema(footfall.Min.MaximumResponseFactor).MaximumResponseFactor;
          Parallel.ForEach(footfall.Subset, kvp =>
            values.TryAdd(kvp.Key,
            // take first as value as footfall cannot be from a combination case with permutations
            new Ratio(kvp.Value.FirstOrDefault().MaximumResponseFactor, RatioUnit.DecimalFraction)));
          break;
      }

      int significantDigits = 0;
      if (customMinMax != Interval.Unset) {
        dmin = customMinMax.Min;
        dmax = customMinMax.Max;
        List<double> rounded = ResultHelper.SmartRounder((double)dmax, (double)dmin);
        significantDigits = (int)rounded[2];
      } else {
        if (enveloped) {
          dmax = values.Values.Max().Value;
          dmin = values.Values.Min().Value;
        }

        List<double> rounded = ResultHelper.SmartRounder((double)dmax, (double)dmin);
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

        double tnorm = 0;
        float size = 0;
        if (t != null) {
          tnorm = (2 * (t.Value - (double)dmin) / ((double)dmax - (double)dmin)) - 1;
          size = t.Value >= 0 && dmax != 0 ? Math.Max(2, (float)(t.Value / dmax * scale)) :
            Math.Max(2, (float)(Math.Abs(t.Value) / Math.Abs((double)dmin) * scale));
        }
        Color valcol = ghGradient.ColourAt(tnorm);

        pts[kvp.Key] = new PointResultGoo(pt, t, valcol, size, kvp.Key);
      });

      int gripheight = _contourLegendMenager.GetBitmapSize().Height / ghGradient.GripCount;
      var legendValues = new List<string>();
      var _legendValuePositionsY = new List<int>();

      var ts = new List<GH_UnitNumber>();
      var cs = new List<Color>();

      for (int i = 0; i < ghGradient.GripCount; i++) {
        double t = (double)dmin + (((double)dmax - (double)dmin) / ((double)ghGradient.GripCount - 1) * i);
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
        _gradients.Add((starty, endy, gradientcolour));

        switch (_mode) {
          case FoldMode.Displacement when (int)_disp < 4:
            var displacement = new Length(t, _lengthResultUnit);
            legendValues.Add(displacement.ToString("f" + significantDigits));
            ts.Add(new GH_UnitNumber(displacement));
            break;

          case FoldMode.Displacement:
            var rotation = new Angle(t, AngleUnit.Radian);
            legendValues.Add(rotation.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(rotation));
            break;

          case FoldMode.Reaction when (int)_disp < 4:
          case FoldMode.SpringForce when (int)_disp < 4:
            var reactionForce = new Force(t, _forceUnit);
            legendValues.Add(reactionForce.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(reactionForce));
            break;

          case FoldMode.Reaction:
          case FoldMode.SpringForce:
            var reactionMoment = new Moment(t, _momentUnit);
            legendValues.Add(reactionMoment.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(reactionMoment));
            break;

          case FoldMode.Footfall:
            var responseFactor = new Ratio(t, RatioUnit.DecimalFraction);
            legendValues.Add(responseFactor.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(responseFactor));
            break;
        }

        if (Math.Abs(t) > 1) {
          legendValues[i] = legendValues[i].Replace(",", string.Empty); // remove thousand separator
        }

        _legendValuePositionsY.Add(_contourLegendMenager.GetBitmapSize().Height - starty + (gripheight / 2) - 2);
      }

      _contourLegendMenager.SetTextValues(legendValues);
      _contourLegendMenager.SetPositionYValues(_legendValuePositionsY);

      da.SetDataList(0, pts.OrderBy(x => x.Key).Select(y => y.Value).ToList());
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);

      PostHog.Result(result.CaseType, 0, _mode.ToString(), _disp.ToString());
    }

    private void DisplacementModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Displacement;
      _slider = true;
      _defScale = 100;
      ReDrawComponent();
    }

    private void ReactionModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Reaction;
      _slider = false;
      _defScale = 0;
      ReDrawComponent();
    }

    private void SpringForceClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.SpringForce;
      _slider = false;
      _defScale = 0;
      ReDrawComponent();
    }

    private void FootfallModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Footfall;
      _slider = false;
      _defScale = 0;
      ReDrawComponent();
    }

    internal void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateMoment(string unit) {
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }
  }
}
