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
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaApi;
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

using Rhino.Collections;
using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Element3d results
  /// </summary>
  public class Contour3dResults : GH_OasysDropDownComponent {
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
      Stress,
    }

    public override Guid ComponentGuid => new Guid("033035c6-16d9-4cc7-b2b3-cf5d5fac4108");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Contour3dResults;
    private readonly List<string> _displacement = new List<string>(new[] {
      "Translation Ux",
      "Translation Uy",
      "Translation Uz",
      "Resolved |U|",
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
      "Stress",
    });
    private string _case = string.Empty;
    private double _defScale = 250;
    private DisplayValue _disp = DisplayValue.ResXyz;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private double _maxValue = 1000;
    private double _minValue;
    private FoldMode _mode = FoldMode.Displacement;
    private int _noDigits;
    private string _resType;
    private bool _slider = true;
    private PressureUnit _stressUnitResult = DefaultUnits.StressUnitResult;
    private EnvelopeMethod _envelopeType = EnvelopeMethod.Absolute;
    private readonly ContourLegendManager _contourLegendMenager = new ContourLegendManager();
    private List<(int startY, int endY, Color gradientColor)> _gradients
      = new List<(int startY, int endY, Color gradientColor)>();

    public Contour3dResults() : base("Contour 3D Results", "Contour3D", "Displays GSA 3D Element Results as Contour",
      CategoryName.Name(), SubCategoryName.Cat6()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownSliderComponentAttributes(this, SetSelected, _dropDownItems, _selectedItems, _slider,
        SetVal, SetMaxMin, _defScale, _maxValue, _minValue, _noDigits, _spacerDescriptions);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);
      _contourLegendMenager.DrawLegend(args, _resType, _case, _gradients);
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");

      if (reader.ItemExists("envelope")) {
        _envelopeType = (EnvelopeMethod)Enum.Parse(typeof(EnvelopeMethod), reader.GetString("envelope"));
      }

      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _stressUnitResult = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), reader.GetString("stress"));

      _contourLegendMenager.DeserialiseLegendState(reader);

      return base.Read(reader);
    }

    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 0:
          switch (j) {
            case 0:
              if (_dropDownItems[1] != _displacement) {
                _dropDownItems[1] = _displacement;
                _selectedItems[1] = _dropDownItems[1][3]; // Resolved XYZ

                _disp = (DisplayValue)3; // Resolved XYZ
                DeformationModeClicked();
              }

              break;

            case 1:
              if (_dropDownItems[1] != _stress) {
                _dropDownItems[1] = _stress;
                _selectedItems[1] = _dropDownItems[1][2];

                _disp = (DisplayValue)2;
                StressModeClicked();
              }

              break;
          }

          break;

        case 1:
          bool redraw = false;
          _selectedItems[1] = _dropDownItems[1][j];
          if (_mode == FoldMode.Displacement) {
            if (((int)_disp > 3) & (j < 4)) {
              redraw = true;
              _slider = true;
            }

            if (((int)_disp < 4) & (j > 3)) {
              redraw = true;
              _slider = false;
            }

            _disp = (DisplayValue)j;
          } else {
            _disp = j < 3 ? (DisplayValue)j : (DisplayValue)(j + 1);
          }

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
      if (Params.Input.Count != 4) {
        Params.RegisterInputParam(new Param_Interval());
        Params.Input[3].Name = "Min/Max Domain";
        Params.Input[3].NickName = "I";
        Params.Input[3].Description = "Optional Domain for custom Min to Max contour colours";
        Params.Input[3].Optional = true;
        Params.Input[3].Access = GH_ParamAccess.item;
      }

      switch (_mode) {
        case FoldMode.Displacement when (int)_disp < 4:
          Params.Output[2].Name = "Values [" + Length.GetAbbreviation(_lengthResultUnit) + "]";
          break;

        case FoldMode.Displacement:
          Params.Output[2].Name = "Values [rad]";
          break;

        case FoldMode.Stress:
          Params.Output[2].Name = "Legend Values [" + Pressure.GetAbbreviation(_stressUnitResult) + "]";
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
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("stress", Pressure.GetAbbreviation(_stressUnitResult));
      writer.SetString("envelope", _envelopeType.ToString());

      _contourLegendMenager.SerialiseLegendState(writer);

      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      ToolStripMenuItem envelopeMenu = GenerateToolStripMenuItem.GetEnvelopeSubMenuItem(_envelopeType, UpdateEnvelope);
      menu.Items.Add(envelopeMenu);

      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _contourLegendMenager.IsLegendVisible);

      var gradient = new GH_GradientControl();
      gradient.CreateAttributes();
      var extract = new ToolStripMenuItem("Extract Default Gradient", gradient.Icon_24x24, (s, e) => CreateGradient());
      menu.Items.Add(extract);

      ToolStripMenuItem lengthUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Displacement",
        EngineeringUnits.Length, Length.GetAbbreviation(_lengthResultUnit), UpdateLength);

      ToolStripMenuItem stressUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Stress", EngineeringUnits.Stress,
        Pressure.GetAbbreviation(_stressUnitResult), UpdateStress);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        stressUnitsMenu,
      });
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
      menu.Items.Add(unitsMenu);
      ToolStripMenuItem legendScaleMenu = _contourLegendMenager.CreateMenu(this, () => base.UpdateUI());
      menu.Items.Add(legendScaleMenu);

      Menu_AppendSeparator(menu);
    }

    protected override void BeforeSolveInstance() {
      switch (_mode) {
        case FoldMode.Displacement:
          Message = Length.GetAbbreviation(_lengthResultUnit);
          break;

        case FoldMode.Stress:
          Message = Pressure.GetAbbreviation(_stressUnitResult);
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
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager[1].Optional = true;
      pManager.AddColourParameter("Colour", "Co",
        "Optional list of colours to override default colours" + Environment.NewLine
        + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[2].Optional = true;
      pManager.AddIntervalParameter("Min/Max Domain", "I", "Optional Domain for custom Min to Max contour colours",
        GH_ParamAccess.item);
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      IQuantity length = new Length(0, _lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Ngon Mesh", "M", "Ngon Mesh with coloured result values", GH_ParamAccess.item);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values",
        GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      _gradients = new List<(int startY, int endY, Color gradientColor)>();
      GsaResult result;
      string elementlist = "All";
      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);
      result = Inputs.GetResultInput(this, ghTyp);
      if (result == null) {
        return;
      }

      bool enveloped = Inputs.IsResultCaseEnveloped(this, result, ref _case, _envelopeType);
      List<int> permutations = result.SelectedPermutationIds;
      elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
      ReadOnlyDictionary<int, Element> elems = result.Model.ApiModel.Elements(elementlist);
      ReadOnlyDictionary<int, Node> nodes = result.Model.ApiModel.Nodes();
      if (elems.Count == 0) {
        this.AddRuntimeError($"Model contains no results for elements in list '{elementlist}'");
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

      ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 3);
      double dmax = 0;
      double dmin = 0;
      ConcurrentDictionary<int, IList<IQuantity>> values = null;
      ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)> valuesXyz = null;
      switch (_mode) {
        case FoldMode.Displacement:
          IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>>
            displacements = result.Element3dDisplacements.ResultSubset(elementIds);
          Func<ITranslation, IQuantity> translationSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Translation, Ux";
              dmax = displacements.GetExtrema(displacements.Max.X).X.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.X).X.As(_lengthResultUnit);
              translationSelector = (r) => r.X.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.Y:
              _resType = "Translation, Uy";
              dmax = displacements.GetExtrema(displacements.Max.Y).Y.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Y).Y.As(_lengthResultUnit);
              translationSelector = (r) => r.Y.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.Z:
              _resType = "Translation, Uz";
              dmax = displacements.GetExtrema(displacements.Max.Z).Z.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Z).Z.As(_lengthResultUnit);
              translationSelector = (r) => r.Z.ToUnit(_lengthResultUnit);
              break;

            case DisplayValue.ResXyz:
              _resType = "Translation, |U|";
              dmax = displacements.GetExtrema(displacements.Max.Xyz).Xyz.As(_lengthResultUnit);
              dmin = displacements.GetExtrema(displacements.Min.Xyz).Xyz.As(_lengthResultUnit);
              translationSelector = (r) => r.Xyz.ToUnit(_lengthResultUnit);
              valuesXyz = ResultsUtility.GetResultResultantTranslation(displacements.Subset, lengthUnit, permutations,
                _envelopeType);
              break;
          }

          values = ResultsUtility.GetResultComponent(displacements.Subset, translationSelector, permutations,
            _envelopeType);
          break;

        case FoldMode.Stress:
          IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> stresses
            = result.Element3dStresses.ResultSubset(elementIds);
          Func<IStress, IQuantity> stressSelector = null;
          switch (_disp) {
            case DisplayValue.X:
              _resType = "Stress, xx";
              dmax = stresses.GetExtrema(stresses.Max.Xx).Xx.As(_stressUnitResult);
              dmin = stresses.GetExtrema(stresses.Min.Xx).Xx.As(_stressUnitResult);
              stressSelector = (r) => r.Xx.ToUnit(_stressUnitResult);
              break;

            case DisplayValue.Y:
              _resType = "Stress, yy";
              dmax = stresses.GetExtrema(stresses.Max.Yy).Yy.As(_stressUnitResult);
              dmin = stresses.GetExtrema(stresses.Min.Yy).Yy.As(_stressUnitResult);
              stressSelector = (r) => r.Yy.ToUnit(_stressUnitResult);
              break;

            case DisplayValue.Z:
              _resType = "Stress, zz";
              dmax = stresses.GetExtrema(stresses.Max.Zz).Zz.As(_stressUnitResult);
              dmin = stresses.GetExtrema(stresses.Min.Zz).Zz.As(_stressUnitResult);
              stressSelector = (r) => r.Zz.ToUnit(_stressUnitResult);
              break;

            case DisplayValue.Xx:
              _resType = "Stress, xy";
              dmax = stresses.GetExtrema(stresses.Max.Xy).Xy.As(_stressUnitResult);
              dmin = stresses.GetExtrema(stresses.Min.Xy).Xy.As(_stressUnitResult);
              stressSelector = (r) => r.Xx.ToUnit(_stressUnitResult);
              break;

            case DisplayValue.Yy:
              _resType = "Stress, yz";
              dmax = stresses.GetExtrema(stresses.Max.Yz).Yz.As(_stressUnitResult);
              dmin = stresses.GetExtrema(stresses.Min.Yz).Yz.As(_stressUnitResult);
              stressSelector = (r) => r.Yz.ToUnit(_stressUnitResult);
              break;

            case DisplayValue.Zz:
              _resType = "Stress, zx";
              dmax = stresses.GetExtrema(stresses.Max.Zx).Zx.As(_stressUnitResult);
              dmin = stresses.GetExtrema(stresses.Min.Zx).Zx.As(_stressUnitResult);
              stressSelector = (r) => r.Zx.ToUnit(_stressUnitResult);
              break;
          }

          values = ResultsUtility.GetResultComponent(stresses.Subset, stressSelector, permutations, _envelopeType);
          break;
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

      var resultMeshes = new MeshResultGoo(new Mesh(), new List<IList<IQuantity>>(), new List<Point3dList>(),
        new List<int>());
      var meshes = new ConcurrentDictionary<int, Mesh>();
      meshes.AsParallel().AsOrdered();
      values.AsParallel().AsOrdered();
      var verticies = new ConcurrentDictionary<int, Point3dList>();
      verticies.AsParallel().AsOrdered();

      Parallel.ForEach(elems.Keys, key => {
        var element = new GSAElement(elems[key]);
        if (element.Topology.Count < 5) {
          return;
        }

        Mesh tempmesh = GsaElementFactory.GetMeshFromApiElement3d(element, nodes, lengthUnit);
        if (tempmesh == null) {
          return;
        }

        List<Vector3d> transformation = null;
        if (_mode == FoldMode.Displacement) {
          switch (_disp) {
            case DisplayValue.X:
              transformation = values[key].Select(item => new Vector3d(item.As(lengthUnit) * _defScale, 0, 0)).ToList();
              break;

            case DisplayValue.Y:
              transformation = values[key].Select(item => new Vector3d(0, item.As(lengthUnit) * _defScale, 0)).ToList();
              break;

            case DisplayValue.Z:
              transformation = values[key].Select(item => new Vector3d(0, 0, item.As(lengthUnit) * _defScale)).ToList();
              break;

            case DisplayValue.ResXyz:
              transformation = new List<Vector3d>();
              for (int i = 0; i < valuesXyz[key].x.Count; i++) {
                transformation.Add(new Vector3d(valuesXyz[key].x[i] * _defScale, valuesXyz[key].y[i] * _defScale,
                  valuesXyz[key].z[i] * _defScale));
              }

              break;
          }
        }

        for (int i = 0; i < tempmesh.Vertices.Count; i++) {
          double tnorm = (2 * (values[key][i].Value - dmin) / (dmax - dmin)) - 1;
          Color col = double.IsNaN(tnorm) ? Color.Transparent : ghGradient.ColourAt(tnorm);
          tempmesh.VertexColors.Add(col);
          if (transformation == null) {
            continue;
          }

          Point3f def = tempmesh.Vertices[i];
          def.Transform(Transform.Translation(transformation[i]));
          tempmesh.Vertices[i] = def;
        }

        if (values[key].Count == 1) {
          // if analysis settings is set to '2D element forces and 2D/3D stresses at centre only'
          double tnorm = (2 * (values[key][0].Value - dmin) / (dmax - dmin)) - 1;
          Color col = double.IsNaN(tnorm) ? Color.Transparent : ghGradient.ColourAt(tnorm);
          for (int i = 0; i < tempmesh.Vertices.Count; i++) {
            tempmesh.VertexColors.SetColor(i, col);
          }

          verticies[key] = new Point3dList() {
            new Point3d(tempmesh.Vertices.Select(pt => pt.X).Average(), tempmesh.Vertices.Select(pt => pt.Y).Average(),
              tempmesh.Vertices.Select(pt => pt.Z).Average()),
          };
        } else {
          verticies[key] = new Point3dList(tempmesh.Vertices.Select(pt => (Point3d)pt).ToList());
        }

        meshes[key] = tempmesh;
      });

      resultMeshes.AddRange(meshes.Values.ToList(), values.Values.ToList(), verticies.Values.ToList(),
        meshes.Keys.ToList());

      int gripheight = _contourLegendMenager.GetBitmapSize().Height / ghGradient.GripCount;
      var legendValues = new List<string>();
      var legendValuePositionsY = new List<int>();

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

        Color gradientcolour = ghGradient.ColourAt((2 * (double)i / ((double)ghGradient.GripCount - 1)) - 1);
        cs.Add(gradientcolour);

        int starty = i * gripheight;
        int endy = starty + gripheight;
        _gradients.Add((starty, endy, gradientcolour));

        switch (_mode) {
          case FoldMode.Displacement when (int)_disp < 4: {
            var displacement = new Length(t, _lengthResultUnit);
            legendValues.Add(displacement.ToString("f" + significantDigits));
            ts.Add(new GH_UnitNumber(displacement));
            break;
          }
          case FoldMode.Displacement: {
            var rotation = new Angle(t, AngleUnit.Radian);
            legendValues.Add(rotation.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(rotation));
            break;
          }
          case FoldMode.Stress: {
            var stress = new Pressure(t, _stressUnitResult);
            legendValues.Add(stress.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(stress));
            break;
          }
        }

        if (Math.Abs(t) > 1) {
          legendValues[i] = legendValues[i].Replace(",", string.Empty); // remove thousand separator
        }

        legendValuePositionsY.Add(_contourLegendMenager.GetBitmapSize().Height - starty + (gripheight / 2) - 2);
      }

      _contourLegendMenager.SetTextValues(legendValues);
      _contourLegendMenager.SetPositionYValues(legendValuePositionsY);

      da.SetData(0, resultMeshes);
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);

      PostHog.Result(result.CaseType, 3, _mode.ToString(), _disp.ToString());
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

      gradient.Attributes.Pivot = new PointF(Attributes.Bounds.X - gradient.Attributes.Bounds.Width - 50,
        Params.Input[2].Attributes.Bounds.Y - (gradient.Attributes.Bounds.Height / 4) - 6);

      doc.AddObject(gradient, false);
      Params.Input[2].RemoveAllSources();
      Params.Input[2].AddSource(gradient.Params.Output[0]);

      UpdateUI();
      return gradient;
    }

    private void DeformationModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Displacement;
      _slider = true;
      _defScale = 100;
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
      _contourLegendMenager.ToggleVisibility();
      ExpirePreview(true);
    }

    internal void UpdateEnvelope(string type) {
      _envelopeType = (EnvelopeMethod)Enum.Parse(typeof(EnvelopeMethod), type);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void StressModeClicked() {
      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Stress;
      _slider = false;
      _defScale = 0;
      ReDrawComponent();
    }

    internal void UpdateLength(string unit) {
      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateStress(string unit) {
      _stressUnitResult = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }
  }
}
