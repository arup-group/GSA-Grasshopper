using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to get Element1D results
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class Elem1dContourResults_OBSOLETE : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("dee5c513-197e-4659-998f-09225df9beaa");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Properties.Resources.Result1D;

    public Elem1dContourResults_OBSOLETE() : base("1D Contour Results",
      "ContourElem1d",
      "Displays GSA 1D Element Results as Contour",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + Environment.NewLine +
          "Element list should take the form:" + Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddIntegerParameter("Intermediate Points", "nP", "Number of intermediate equidistant points (default 10)", GH_ParamAccess.item, 10);
      pManager.AddColourParameter("Colour", "Co", "[Optional] List of colours to override default colours" +
          Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size", GH_ParamAccess.item, 10);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      IQuantity length = new Length(0, _lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Line", "L", "Contoured Line segments with result values", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      #region Inputs
      if (ghTyp.Value is GsaResultGoo goo) {
        result = goo.Value;
        switch (result.Type) {
          case GsaResult.CaseType.Combination when result.SelectedPermutationIds.Count > 1:
            this.AddRuntimeWarning("Combination case contains "
                                   + result.SelectedPermutationIds.Count + " - only one permutation can be displayed at a time." +
                                   Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
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

      string elementlist = "All";
      var ghType = new GH_String();
      if (da.GetData(1, ref ghType))
        GH_Convert.ToString(ghType, out elementlist, GH_Conversion.Both);

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
      Grasshopper.GUI.Gradient.GH_Gradient ghGradient = Helpers.Graphics.Colours.Stress_Gradient(colors);

      var ghScale = new GH_Number();
      da.GetData(4, ref ghScale);
      GH_Convert.ToDouble(ghScale, out double scale, GH_Conversion.Both);
      #endregion

      var res = new GsaResultsValues();

      switch (_mode) {
        case FoldMode.Displacement:
          res = result.Element1DDisplacementValues(elementlist, positionsCount, _lengthUnit)[0];

          break;

        case FoldMode.Force:
          res = result.Element1DForceValues(elementlist, positionsCount,
            DefaultUnits.ForceUnit, DefaultUnits.MomentUnit)[0];
          break;
        case FoldMode.StrainEnergy:
          res = _disp == DisplayValue.X
            ? result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, _energyResultUnit)[0]
            : result.Element1DAverageStrainEnergyDensityValues(elementlist, _energyResultUnit)[0];
          break;
      }

      var elementIDs = new List<int>();
      elementIDs = result.Type == GsaResult.CaseType.AnalysisCase
        ? result.ACaseElement1DResults.Values.First().Select(x => x.Key).ToList()
        : result.ComboElement1DResults.Values.First().Select(x => x.Key).ToList();
      if (elementlist.ToLower() == "all")
        elementlist = string.Join(" ", elementIDs);

      var elems = new ConcurrentDictionary<int, Element>(result.Model.Model.Elements(elementlist));
      var nodes = new ConcurrentDictionary<int, Node>(result.Model.Model.Nodes());

      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

      Enum xyzunit = _lengthUnit;
      Enum xxyyzzunit = AngleUnit.Radian;
      switch (_mode) {
        case FoldMode.Force:
          xyzunit = DefaultUnits.ForceUnit;
          xxyyzzunit = DefaultUnits.MomentUnit;
          break;
        case FoldMode.StrainEnergy:
          xyzunit = DefaultUnits.EnergyUnit;
          break;
      }

      double dmaxX = res.DmaxX.As(xyzunit);
      double dmaxY = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxY.As(xyzunit);
      double dmaxZ = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxZ.As(xyzunit);
      double dmaxXyz = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxXyz.As(xyzunit);
      double dminX = _mode == FoldMode.StrainEnergy ? 0 : res.DminX.As(xyzunit);
      double dminY = _mode == FoldMode.StrainEnergy ? 0 : res.DminY.As(xyzunit);
      double dminZ = _mode == FoldMode.StrainEnergy ? 0 : res.DminZ.As(xyzunit);
      double dminXyz = _mode == FoldMode.StrainEnergy ? 0 : res.DminXyz.As(xyzunit);
      double dmaxXx = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxXx.As(xxyyzzunit);
      double dmaxYy = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxYy.As(xxyyzzunit);
      double dmaxZz = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxZz.As(xxyyzzunit);
      double dmaxXxyyzz = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxXxyyzz.As(xxyyzzunit);
      double dminXx = _mode == FoldMode.StrainEnergy ? 0 : res.DminXx.As(xxyyzzunit);
      double dminYy = _mode == FoldMode.StrainEnergy ? 0 : res.DminYy.As(xxyyzzunit);
      double dminZz = _mode == FoldMode.StrainEnergy ? 0 : res.DminZz.As(xxyyzzunit);
      double dminXxyyzz = _mode == FoldMode.StrainEnergy ? 0 : res.DminXxyyzz.As(xxyyzzunit);

      #region Result line values
      double dmax = 0;
      double dmin = 0;
      if (_mode == FoldMode.StrainEnergy) {
        dmax = dmaxX;
        dmin = dminX;
        if (_disp == DisplayValue.X)
          _resType = "Strain Energy Density";
        else {
          positionsCount = 2;
          _resType = "Average Strain E Dens.";
        }
      }
      else {
        switch (_disp) {
          case (DisplayValue.X):
            dmax = dmaxX;
            dmin = dminX;
            _resType = _mode == FoldMode.Displacement ? "Elem. Trans., Ux" : "Axial Force, Fx";
            break;
          case (DisplayValue.Y):
            dmax = dmaxY;
            dmin = dminY;
            _resType = _mode == FoldMode.Displacement ? "Elem. Trans., Uy" : "Shear Force, Fy";
            break;
          case (DisplayValue.Z):
            dmax = dmaxZ;
            dmin = dminZ;
            _resType = _mode == FoldMode.Displacement ? "Elem. Trans., Uz" : "Shear Force, Fz";
            break;
          case (DisplayValue.ResXyz):
            dmax = dmaxXyz;
            dmin = dminXyz;
            _resType = _mode == FoldMode.Displacement ? "Res. Trans., |U|" : "Res. Shear, |Fyz|";
            break;
          case (DisplayValue.Xx):
            dmax = dmaxXx;
            dmin = dminXx;
            _resType = _mode == FoldMode.Displacement ? "Elem. Rot., Rxx" : "Torsion, Mxx";
            break;
          case (DisplayValue.Yy):
            dmax = dmaxYy;
            dmin = dminYy;
            _resType = _mode == FoldMode.Displacement ? "Elem. Rot., Ryy" : "Moment, Myy";
            break;
          case (DisplayValue.Zz):
            dmax = dmaxZz;
            dmin = dminZz;
            _resType = _mode == FoldMode.Displacement ? "Elem. Rot., Rzz" : "Moment, Mzz";
            break;
          case (DisplayValue.ResXxyyzz):
            dmax = dmaxXxyyzz;
            dmin = dminXxyyzz;
            _resType = _mode == FoldMode.Displacement ? "Res. Rot., |U|" : "Res. Moment, |Myz|";
            break;
        }
      }

      List<double> rounded = Helpers.GsaAPI.ResultHelper.SmartRounder(dmax, dmin);
      dmax = rounded[0];
      dmin = rounded[1];
      int significantDigits = (int)rounded[2];

      var resultLines = new DataTree<LineResultGoo>();

      Parallel.ForEach(elems, element => {
        if (element.Value.IsDummy) {
          return;
        }
        if (element.Value.Type == ElementType.LINK) {
          return;
        }
        if (element.Value.Topology.Count > 2) {
          return;
        }
        var ln = new Line(
          Helpers.Import.Nodes.Point3dFromNode(nodes[element.Value.Topology[0]], _lengthUnit), // start point
          Helpers.Import.Nodes.Point3dFromNode(nodes[element.Value.Topology[1]], _lengthUnit));// end point

        int key = element.Key;

        for (int i = 0; i < positionsCount - 1; i++) {
          if (dmin == 0 & dmax == 0) {
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
                case (DisplayValue.X):
                  t1 = xyzResults[key][i].X.ToUnit(_lengthUnit);
                  t2 = xyzResults[key][i + 1].X.ToUnit(_lengthUnit);
                  startTranslation.X = t1.Value * _defScale;
                  endTranslation.X = t2.Value * _defScale;
                  break;
                case (DisplayValue.Y):
                  t1 = xyzResults[key][i].Y.ToUnit(_lengthUnit);
                  t2 = xyzResults[key][i + 1].Y.ToUnit(_lengthUnit);
                  startTranslation.Y = t1.Value * _defScale;
                  endTranslation.Y = t2.Value * _defScale;
                  break;
                case (DisplayValue.Z):
                  t1 = xyzResults[key][i].Z.ToUnit(_lengthUnit);
                  t2 = xyzResults[key][i + 1].Z.ToUnit(_lengthUnit);
                  startTranslation.Z = t1.Value * _defScale;
                  endTranslation.Z = t2.Value * _defScale;
                  break;
                case (DisplayValue.ResXyz):
                  t1 = xyzResults[key][i].XYZ.ToUnit(_lengthUnit);
                  t2 = xyzResults[key][i + 1].XYZ.ToUnit(_lengthUnit);
                  startTranslation.X = xyzResults[key][i].X.As(_lengthUnit) * _defScale;
                  startTranslation.Y = xyzResults[key][i].Y.As(_lengthUnit) * _defScale;
                  startTranslation.Z = xyzResults[key][i].Z.As(_lengthUnit) * _defScale;
                  endTranslation.X = xyzResults[key][i + 1].X.As(_lengthUnit) * _defScale;
                  endTranslation.Y = xyzResults[key][i + 1].Y.As(_lengthUnit) * _defScale;
                  endTranslation.Z = xyzResults[key][i + 1].Z.As(_lengthUnit) * _defScale;
                  break;
                case (DisplayValue.Xx):
                  t1 = xxyyzzResults[key][i].X.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].X.ToUnit(AngleUnit.Radian);
                  break;
                case (DisplayValue.Yy):
                  t1 = xxyyzzResults[key][i].Y.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].Y.ToUnit(AngleUnit.Radian);
                  break;
                case (DisplayValue.Zz):
                  t1 = xxyyzzResults[key][i].Z.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].Z.ToUnit(AngleUnit.Radian);
                  break;
                case (DisplayValue.ResXxyyzz):
                  t1 = xxyyzzResults[key][i].XYZ.ToUnit(AngleUnit.Radian);
                  t2 = xxyyzzResults[key][i + 1].XYZ.ToUnit(AngleUnit.Radian);
                  break;
              }
              start.Transform(Transform.Translation(startTranslation));
              end.Transform(Transform.Translation(endTranslation));
              break;
            case FoldMode.Force:
              switch (_disp) {
                case (DisplayValue.X):
                  t1 = xyzResults[key][i].X.ToUnit(DefaultUnits.ForceUnit);
                  t2 = xyzResults[key][i + 1].X.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.Y):
                  t1 = xyzResults[key][i].Y.ToUnit(DefaultUnits.ForceUnit);
                  t2 = xyzResults[key][i + 1].Y.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.Z):
                  t1 = xyzResults[key][i].Z.ToUnit(DefaultUnits.ForceUnit);
                  t2 = xyzResults[key][i + 1].Z.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.ResXyz):
                  t1 = xyzResults[key][i].XYZ.ToUnit(DefaultUnits.ForceUnit);
                  t2 = xyzResults[key][i + 1].XYZ.ToUnit(DefaultUnits.ForceUnit);
                  break;
                case (DisplayValue.Xx):
                  t1 = xxyyzzResults[key][i].X.ToUnit(DefaultUnits.MomentUnit);
                  t2 = xxyyzzResults[key][i + 1].X.ToUnit(DefaultUnits.MomentUnit);
                  break;
                case (DisplayValue.Yy):
                  t1 = xxyyzzResults[key][i].Y.ToUnit(DefaultUnits.MomentUnit);
                  t2 = xxyyzzResults[key][i + 1].Y.ToUnit(DefaultUnits.MomentUnit);
                  break;
                case (DisplayValue.Zz):
                  t1 = xxyyzzResults[key][i].Z.ToUnit(DefaultUnits.MomentUnit);
                  t2 = xxyyzzResults[key][i + 1].Z.ToUnit(DefaultUnits.MomentUnit);
                  break;
                case (DisplayValue.ResXxyyzz):
                  t1 = xxyyzzResults[key][i].XYZ.ToUnit(DefaultUnits.MomentUnit);
                  t2 = xxyyzzResults[key][i + 1].XYZ.ToUnit(DefaultUnits.MomentUnit);
                  break;
              }
              break;
            case FoldMode.StrainEnergy:
              if (_disp == DisplayValue.X) {
                t1 = xyzResults[key][i].X.ToUnit(DefaultUnits.EnergyUnit);
                t2 = xyzResults[key][i + 1].X.ToUnit(DefaultUnits.EnergyUnit);
              }
              else {
                t1 = xyzResults[key][i].X.ToUnit(DefaultUnits.EnergyUnit);
                t2 = xyzResults[key][i].X.ToUnit(DefaultUnits.EnergyUnit);
              }
              break;
          }

          var segmentline = new Line(start, end);
          double tnorm1 = 2 * (t1.Value - dmin) / (dmax - dmin) - 1;
          double tnorm2 = 2 * (t2.Value - dmin) / (dmax - dmin) - 1;

          Color valcol1 = double.IsNaN(tnorm1) ? Color.Black : ghGradient.ColourAt(tnorm1);
          Color valcol2 = double.IsNaN(tnorm2) ? Color.Black : ghGradient.ColourAt(tnorm2);

          float size1 = (t1.Value >= 0 && dmax != 0) ?
            Math.Max(2, (float)(t1.Value / dmax * scale)) :
            Math.Max(2, (float)(Math.Abs(t1.Value) / Math.Abs(dmin) * scale));
          if (double.IsNaN(size1))
            size1 = 1;
          float size2 = (t2.Value >= 0 && dmax != 0) ?
            Math.Max(2, (float)(t2.Value / dmax * scale)) :
            Math.Max(2, (float)(Math.Abs(t2.Value) / Math.Abs(dmin) * scale));
          if (double.IsNaN(size2))
            size2 = 1;

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
        double t = dmin + (dmax - dmin) / ((double)ghGradient.GripCount - 1) * (double)i;
        double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
        scl = Math.Max(scl, 1);
        t = scl * Math.Round(t / scl, 3);

        Color gradientcolour = ghGradient.ColourAt(2 * (double)i / ((double)ghGradient.GripCount - 1) - 1);
        cs.Add(gradientcolour);

        int starty = i * gripheight;
        int endy = starty + gripheight;
        for (int y = starty; y < endy; y++) {
          for (int x = 0; x < _legend.Width; x++)
            _legend.SetPixel(x, _legend.Height - y - 1, gradientcolour);
        }
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
          case FoldMode.Force when (int)_disp < 4: {
              var force = new Force(t, DefaultUnits.ForceUnit);
              _legendValues.Add(force.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(force));
              break;
            }
          case FoldMode.Force: {
              var moment = new Moment(t, DefaultUnits.MomentUnit);
              _legendValues.Add(t.ToString("F" + significantDigits) + " " + Moment.GetAbbreviation(DefaultUnits.MomentUnit));
              ts.Add(new GH_UnitNumber(moment));
              break;
            }
          default: {
              var energy = new Energy(t, DefaultUnits.EnergyUnit);
              _legendValues.Add(energy.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(energy));
              break;
            }
        }

        if (Math.Abs(t) > 1)
          _legendValues[i] = _legendValues[i].Replace(",", string.Empty); // remove thousand separator
        _legendValuesPosY.Add(_legend.Height - starty + gripheight / 2 - 2);
      }
      #endregion

      da.SetDataTree(0, resultLines);
      da.SetDataList(1, cs);
      da.SetDataList(2, ts);
    }

    #region Custom UI
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
    }

    private readonly List<string> _type = new List<string>(new[]
    {
      "Displacement",
      "Force",
      "Strain Energy",
    });
    private readonly List<string> _displacement = new List<string>(new[]
    {
      "Translation Ux",
      "Translation Uy",
      "Translation Uz",
      "Resolved |U|",
      "Rotation Rxx",
      "Rotation Ryy",
      "Rotation Rzz",
      "Resolved |R|",
    });
    private readonly List<string> _force = new List<string>(new[]
    {
      "Axial Force Fx",
      "Shear Force Fy",
      "Shear Force Fz",
      "Res. Shear |Fyz|",
      "Torsion Mxx",
      "Moment Myy",
      "Moment Mzz",
      "Res. Moment |Myz|",
    });
    private readonly List<string> _strainenergy = new List<string>(new[]
    {
      "Intermediate Pts",
      "Average",
    });
    private double _minValue;
    private double _maxValue = 1000;
    private double _defScale = 250;
    private int _noDigits;
    private bool _slider = true;
    private string _case = "";
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private EnergyUnit _energyResultUnit = DefaultUnits.EnergyUnit;
    private FoldMode _mode = FoldMode.Displacement;
    private DisplayValue _disp = DisplayValue.ResXyz;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
        {
          "Result Type",
          "Component",
          "Geometry Unit",
          "Deform Shape",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // type
      DropDownItems.Add(_type);
      SelectedItems.Add(DropDownItems[0][0]);

      // component
      DropDownItems.Add(_displacement);
      SelectedItems.Add(DropDownItems[1][3]);

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void CreateAttributes() {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new OasysGH.UI.DropDownSliderComponentAttributes(this, SetSelected, DropDownItems, SelectedItems, _slider, SetVal, SetMaxMin, _defScale, _maxValue, _minValue, _noDigits, SpacerDescriptions);
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

                    _disp = DisplayValue.ResXyz;
                    Mode1Clicked();
                  }

                  break;
                }
              case 1: {
                  if (DropDownItems[1] != _force) {
                    DropDownItems[1] = _force;

                    SelectedItems[0] = DropDownItems[0][1];
                    SelectedItems[1] = DropDownItems[1][5]; // set Myy as default

                    _disp = DisplayValue.Yy;
                    Mode2Clicked();
                  }

                  break;
                }
              case 2: {
                  if (DropDownItems[1] != _strainenergy) {
                    DropDownItems[1] = _strainenergy;

                    SelectedItems[0] = DropDownItems[0][2];
                    SelectedItems[1] = DropDownItems[1][1]; // set average as default

                    _disp = DisplayValue.Y;
                    Mode3Clicked();
                  }

                  break;
                }
            }

            break;
          }
        case 1: {
            bool redraw = false;

            if (selectedidd < 4) {
              if ((int)_disp > 3) // chekc if we are coming from other half of display modes
              {
                if (_mode == FoldMode.Displacement) {
                  redraw = true;
                  _slider = true;
                }
              }
            }
            else {
              if ((int)_disp < 4) // chekc if we are coming from other half of display modes
              {
                if (_mode == FoldMode.Displacement) {
                  redraw = true;
                  _slider = false;
                }
              }
            }
            _disp = (DisplayValue)selectedidd;

            SelectedItems[1] = DropDownItems[1][selectedidd];

            if (redraw)
              ReDrawComponent();
            break;
          }
        default:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[2]);
          break;
      }

      base.UpdateUI();
    }
    public void SetVal(double value) {
      _defScale = value;
    }
    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      if (_mode == FoldMode.Displacement) {
        if ((int)_disp < 4)
          Params.Output[2].Name = "Values [" + Length.GetAbbreviation(_lengthUnit) + "]";
        else
          Params.Output[2].Name = "Values [rad]";
      }

      if (_mode != FoldMode.Force) {
        return;
      }

      if ((int)_disp < 4)
        Params.Output[2].Name = "Legend Values [" + Force.GetAbbreviation(DefaultUnits.ForceUnit) + "]";
      else
        Params.Output[2].Name = "Legend Values [" + Moment.GetAbbreviation(DefaultUnits.MomentUnit) + "]";
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
      if (_mode == FoldMode.Force)
        return;

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.Force;

      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }
    private void Mode3Clicked() {
      if (_mode == FoldMode.StrainEnergy)
        return;

      RecordUndoEvent(_mode + " Parameters");
      _mode = FoldMode.StrainEnergy;

      _slider = false;
      _defScale = 0;

      ReDrawComponent();
    }
    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _showLegend);

      var gradient = new Grasshopper.Kernel.Special.GH_GradientControl();
      gradient.CreateAttributes();
      var extract = new ToolStripMenuItem("Extract Default Gradient", gradient.Icon_24x24, (s, e) => { CreateGradient(); });
      menu.Items.Add(extract);
      Menu_AppendSeparator(menu);
    }

    private bool _showLegend = true;
    private void ShowLegend(object sender, EventArgs e) {
      _showLegend = !_showLegend;
      ExpirePreview(true);
    }
    private void CreateGradient() {
      var gradient = new Grasshopper.Kernel.Special.GH_GradientControl();
      gradient.CreateAttributes();

      gradient.Gradient = Helpers.Graphics.Colours.Stress_Gradient();
      gradient.Gradient.NormalizeGrips();
      gradient.Params.Input[0].AddVolatileData(new GH_Path(0), 0, -1);
      gradient.Params.Input[1].AddVolatileData(new GH_Path(0), 0, 1);
      gradient.Params.Input[2].AddVolatileDataList(
        new GH_Path(0),
        new List<double>() { -1, -0.666, -0.333, 0, 0.333, 0.666, 1 });

      gradient.Attributes.Pivot = new PointF(Attributes.Bounds.X - gradient.Attributes.Bounds.Width - 50, Params.Input[3].Attributes.Bounds.Y - gradient.Attributes.Bounds.Height / 4 - 6);

      Grasshopper.Instances.ActiveCanvas.Document.AddObject(gradient, false);
      Params.Input[3].RemoveAllSources();
      Params.Input[3].AddSource(gradient.Params.Output[0]);

      UpdateUI();
    }
    #endregion

    #region draw _legend
    private Bitmap _legend = new Bitmap(15, 120);
    private List<string> _legendValues;
    private List<int> _legendValuesPosY;
    private string _resType;
    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);
      if (!(_legendValues != null & _showLegend)) {
        return;
      }

      args.Display.DrawBitmap(new DisplayBitmap(_legend), args.Viewport.Bounds.Right - 110, 20);
      for (int i = 0; i < _legendValues.Count; i++)
        args.Display.Draw2dText(_legendValues[i], Color.Black, new Point2d(args.Viewport.Bounds.Right - 85, _legendValuesPosY[i]), false);
      args.Display.Draw2dText(_resType, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 7), false);
      args.Display.Draw2dText(_case, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 145), false);
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetBoolean("slider", _slider);
      writer.SetBoolean("_legend", _showLegend);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _showLegend = reader.GetBoolean("_legend");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      return base.Read(reader);
    }
    #endregion
  }
}
