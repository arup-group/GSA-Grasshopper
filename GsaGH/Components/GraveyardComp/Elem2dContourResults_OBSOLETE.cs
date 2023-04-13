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
  // ReSharper disable once InconsistentNaming
  public class Elem2dContourResults_OBSOLETE : GH_OasysDropDownComponent {
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
    }

    public override Guid ComponentGuid => new Guid("935d359a-9394-42fc-a76e-ea08ccb84135");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Result2D;
    private readonly List<string> _displacement = new List<string>(new[] {
      "Translation Ux",
      "Translation Uy",
      "Translation Uz",
      "Resolved |U|",
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
    });
    private string _case = "";
    private double _defScale = 250;
    private DisplayValue _disp = DisplayValue.ResXyz;
    private int _flayer;
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

    public Elem2dContourResults_OBSOLETE() : base("2D Contour Results", "ContourElem2d",
      "Displays GSA 2D Element Results as Contour", CategoryName.Name(), SubCategoryName.Cat5()) { }

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
      _flayer = reader.GetInt32("flayer");
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      _showLegend = reader.GetBoolean("legend");
      return base.Read(reader);
    }

    public void SetMaxMin(double max, double min) {
      _maxValue = max;
      _minValue = min;
    }

    public override void SetSelected(int dropdownlistidd, int selectedidd) {
      switch (dropdownlistidd) {
        case 0: {
          switch (selectedidd) {
            case 0: {
              if (_dropDownItems[1] != _displacement) {
                if (_dropDownItems.Count == 4) // if coming from stress we remove the layer dropdown
                {
                  _dropDownItems.RemoveAt(2);
                  _selectedItems.RemoveAt(2);
                  _spacerDescriptions.RemoveAt(2);
                }

                _dropDownItems[1] = _displacement;
                _dropDownItems[2] = FilteredUnits.FilteredLengthUnits;

                _selectedItems[0] = _dropDownItems[0][0]; // displacement
                _selectedItems[1] = _dropDownItems[1][3]; // Resolved XYZ
                _selectedItems[2] = Length.GetAbbreviation(_lengthUnit);

                _disp = (DisplayValue)3;
                _isShear = false;
                _flayer = 0;
                Mode1Clicked();
              }

              break;
            }
            case 1: {
              if (_dropDownItems[1] != _force) {
                if (_dropDownItems.Count == 4) // if coming from stress we remove the layer dropdown
                {
                  _dropDownItems.RemoveAt(2);
                  _selectedItems.RemoveAt(2);
                  _spacerDescriptions.RemoveAt(2);
                }

                _dropDownItems[1] = _force;

                _selectedItems[0] = _dropDownItems[0][1];
                _selectedItems[1] = _dropDownItems[1][0];
                _selectedItems[2] = Length.GetAbbreviation(_lengthUnit);

                _disp = 0;
                _isShear = false;
                _flayer = 0;
                Mode2Clicked();
              }

              break;
            }
            case 2: {
              if (_dropDownItems[1] != _stress) {
                if (_dropDownItems.Count < 4) {
                  _dropDownItems.Insert(2, _layer);
                  _spacerDescriptions.Insert(2, "Layer");
                }

                _dropDownItems[1] = _stress;

                _selectedItems[0] = _dropDownItems[0][2];
                _selectedItems[1] = _dropDownItems[1][0];

                if (_selectedItems.Count < 4) {
                  _selectedItems.Insert(2, _dropDownItems[2][1]);
                } else {
                  _selectedItems[2] = _dropDownItems[2][1];
                }

                _selectedItems[3] = Length.GetAbbreviation(_lengthUnit);

                _disp = 0;
                _isShear = false;
                Mode4Clicked();
              }

              break;
            }
          }

          break;
        }
        case 1: {
          bool redraw = false;
          _selectedItems[1] = _dropDownItems[1][selectedidd];
          if (_mode == FoldMode.Displacement) {
            if ((int)_disp > 3 & selectedidd < 4) {
              redraw = true;
              _slider = true;
            }

            if ((int)_disp < 4 & selectedidd > 3) {
              redraw = true;
              _slider = false;
            }
          }

          _disp = (DisplayValue)selectedidd;
          if (_dropDownItems[1] != _displacement) {
            _isShear = false;
            if (_mode == FoldMode.Force) {
              if (selectedidd == 3 | selectedidd == 4) {
                _disp = (DisplayValue)selectedidd - 3;
                _isShear = true;
              } else if (selectedidd > 4) {
                _disp = (DisplayValue)selectedidd - 1;
              }
            } else if (_mode == FoldMode.Force) {
              if (selectedidd > 2) {
                _disp = (DisplayValue)selectedidd + 1;
              }
            }
          }

          if (redraw) {
            ReDrawComponent();
          }

          break;
        }
        case 2 when _mode == FoldMode.Stress: {
          switch (selectedidd) {
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
        default:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[2]);
          break;
      }

      base.UpdateUI();
    }

    public void SetVal(double value) {
      _defScale = value;
    }

    public override void VariableParameterMaintenance() {
      string lengthunitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      switch (_mode) {
        case FoldMode.Displacement when (int)_disp < 4:
          Params.Output[2].Name = "Values [" + lengthunitAbbreviation + "]";
          break;

        case FoldMode.Displacement:
          Params.Output[2].Name = "Values [rad]";
          break;

        case FoldMode.Force when (int)_disp < 4 | _isShear:
          Params.Output[2].Name = "Legend Values ["
            + ForcePerLength.GetAbbreviation(DefaultUnits.ForcePerLengthUnit) + "/"
            + lengthunitAbbreviation + "]";
          break;

        case FoldMode.Force:
          Params.Output[2].Name = "Legend Values [" + Force.GetAbbreviation(DefaultUnits.ForceUnit)
            + "·" + lengthunitAbbreviation + "/" + lengthunitAbbreviation + "]";
          break;

        case FoldMode.Stress:
          Params.Output[2].Name = "Legend Values ["
            + Pressure.GetAbbreviation(DefaultUnits.StressUnitResult) + "]";
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
      Menu_AppendSeparator(menu);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Result Type",
        "Component",
        "Geometry Unit",
        "Deform Shape",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_type);
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(_displacement);
      _selectedItems.Add(_dropDownItems[1][3]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list", "El",
        "Filter import by list." + Environment.NewLine + "Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item, "All");
      pManager.AddColourParameter("Colour", "Co",
        "Optional list of colours to override default colours" + Environment.NewLine
        + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      IQuantity length = new Length(0, _lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Mesh", "M", "Mesh with coloured result values",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values",
        GH_ParamAccess.list);
    }

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
              + result.SelectedPermutationIds.Count
              + " - only one permutation can be displayed at a time." + Environment.NewLine
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
      } else {
        this.AddRuntimeError("Error converting input to GSA Result");
        return;
      }

      string elementlist = "All";
      var ghType = new GH_String();
      if (da.GetData(1, ref ghType)) {
        GH_Convert.ToString(ghType, out elementlist, GH_Conversion.Both);
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

      #endregion

      var res = new GsaResultsValues();
      var resShear = new GsaResultsValues();
      switch (_mode) {
        case FoldMode.Displacement:
          res = result.Element2DDisplacementValues(elementlist, _lengthUnit)[0];
          break;

        case FoldMode.Force:
          res = result.Element2DForceValues(elementlist, DefaultUnits.ForcePerLengthUnit,
            DefaultUnits.ForceUnit)[0];
          resShear = result.Element2DShearValues(elementlist, DefaultUnits.ForcePerLengthUnit)[0];
          break;

        case FoldMode.Stress:
          res = result.Element2DStressValues(elementlist, _flayer,
            DefaultUnits.StressUnitResult)[0];
          break;
      }

      ReadOnlyDictionary<int, Element> elems = result.Model.Model.Elements(elementlist);
      ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes();

      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults
        = _isShear ? resShear.XyzResults : res.XyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults
        = res.XxyyzzResults;

      Enum xyzunit = _lengthUnit;
      Enum xxyyzzunit = AngleUnit.Radian;
      switch (_mode) {
        case FoldMode.Force:
          xyzunit = DefaultUnits.ForcePerLengthUnit;
          xxyyzzunit = DefaultUnits.ForceUnit;
          break;

        case FoldMode.Stress:
          xyzunit = DefaultUnits.StressUnitResult;
          xxyyzzunit = DefaultUnits.StressUnitResult;
          break;
      }

      double dmaxX = _isShear ? resShear.DmaxX.As(xyzunit) : res.DmaxX.As(xyzunit);
      double dmaxY = _isShear ? resShear.DmaxY.As(xyzunit) : res.DmaxY.As(xyzunit);
      double dmaxZ = res.DmaxZ.As(xyzunit);
      double dmaxXyz = (_mode == FoldMode.Displacement) ? res.DmaxXyz.As(xyzunit) : 0;
      double dminX = _isShear ? resShear.DminX.As(xyzunit) : res.DminX.As(xyzunit);
      double dminY = _isShear ? resShear.DminY.As(xyzunit) : res.DminY.As(xyzunit);
      double dminZ = res.DminZ.As(xyzunit);
      double dminXyz = (_mode == FoldMode.Displacement) ? res.DminXyz.As(xyzunit) : 0;
      double dmaxXx = _isShear ? 0 : res.DmaxXx.As(xxyyzzunit);
      double dmaxYy = _isShear ? 0 : res.DmaxYy.As(xxyyzzunit);
      double dmaxZz = _isShear ? 0 : res.DmaxZz.As(xxyyzzunit);
      double dmaxXxyyzz = 0;
      double dminXx = _isShear ? 0 : res.DminXx.As(xxyyzzunit);
      double dminYy = _isShear ? 0 : res.DminYy.As(xxyyzzunit);
      double dminZz = _isShear ? 0 : res.DminZz.As(xxyyzzunit);
      double dminXxyyzz = 0;

      #region Result mesh values

      double dmax = 0;
      double dmin = 0;
      switch (_disp) {
        case DisplayValue.X:
          dmax = dmaxX;
          dmin = dminX;
          if (_mode == FoldMode.Displacement) {
            _resType = "Translation, Ux";
          } else if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Force, Nx";
          } else if (_mode == FoldMode.Force & _isShear) {
            _resType = "2D Shear, Qx";
          } else if (_mode == FoldMode.Stress) {
            _resType = "Stress, xx";
          }

          break;

        case DisplayValue.Y:
          dmax = dmaxY;
          dmin = dminY;
          if (_mode == FoldMode.Displacement) {
            _resType = "Translation, Uy";
          } else if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Force, Ny";
          } else if (_mode == FoldMode.Force & _isShear) {
            _resType = "2D Shear, Qy";
          } else if (_mode == FoldMode.Stress) {
            _resType = "2D Stress, yy";
          }

          break;

        case DisplayValue.Z:
          dmax = dmaxZ;
          dmin = dminZ;
          if (_mode == FoldMode.Displacement) {
            _resType = "Translation, Uz";
          } else if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Force, Nxy";
          } else if (_mode == FoldMode.Stress) {
            _resType = "Stress, zz";
          }

          break;

        case DisplayValue.ResXyz:
          dmax = dmaxXyz;
          dmin = dminXyz;
          if (_mode == FoldMode.Displacement) {
            _resType = "Res. Trans., |U|";
          }

          break;

        case DisplayValue.Xx:
          dmax = dmaxXx;
          dmin = dminXx;
          if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Moment, Mx";
          } else if (_mode == FoldMode.Stress) {
            _resType = "Stress, xy";
          }

          break;

        case DisplayValue.Yy:
          dmax = dmaxYy;
          dmin = dminYy;
          if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Moment, My";
          } else if (_mode == FoldMode.Stress) {
            _resType = "Stress, yz";
          }

          break;

        case DisplayValue.Zz:
          dmax = dmaxZz;
          dmin = dminZz;
          if (_mode == FoldMode.Force & !_isShear) {
            _resType = "2D Moment, Mxy";
          } else if (_mode == FoldMode.Stress) {
            _resType = "Stress, zy";
          }

          break;

        case DisplayValue.ResXxyyzz:
          dmax = dmaxXxyyzz;
          dmin = dminXxyyzz;
          break;
      }

      List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
      dmax = rounded[0];
      dmin = rounded[1];
      int significantDigits = (int)rounded[2];

      #region create mesh

      var resultMeshes = new MeshResultGoo(new Mesh(), new List<List<IQuantity>>(),
        new List<List<Point3d>>(), new List<int>());
      var meshes = new ConcurrentDictionary<int, Mesh>();
      meshes.AsParallel().AsOrdered();
      var values = new ConcurrentDictionary<int, List<IQuantity>>();
      values.AsParallel().AsOrdered();
      var verticies = new ConcurrentDictionary<int, List<Point3d>>();
      verticies.AsParallel().AsOrdered();

      Parallel.ForEach(elems.Keys, key => {
        Element element = elems[key];
        if (element.Topology.Count < 3) {
          return;
        }

        Mesh tempmesh = Elements.ConvertElement2D(element, nodes, _lengthUnit);
        if (tempmesh == null) {
          return;
        }

        List<Vector3d> transformation = null;
        var vals = new List<IQuantity>();
        switch (_disp) {
          case DisplayValue.X:
            vals = xyzResults[key].Select(item => item.Value.X.ToUnit(xyzunit)).ToList();
            if (_mode == FoldMode.Displacement) {
              transformation = vals.Select(item => new Vector3d(item.Value * _defScale, 0, 0))
               .ToList();
            }

            break;

          case DisplayValue.Y:
            vals = xyzResults[key].Select(item => item.Value.Y.ToUnit(xyzunit)).ToList();
            if (_mode == FoldMode.Displacement) {
              transformation = vals.Select(item => new Vector3d(0, item.Value * _defScale, 0))
               .ToList();
            }

            break;

          case DisplayValue.Z:
            vals = xyzResults[key].Select(item => item.Value.Z.ToUnit(xyzunit)).ToList();
            if (_mode == FoldMode.Displacement) {
              transformation = vals.Select(item => new Vector3d(0, 0, item.Value * _defScale))
               .ToList();
            }

            break;

          case DisplayValue.ResXyz:
            vals = xyzResults[key].Select(item => item.Value.Xyz.ToUnit(xyzunit)).ToList();
            if (_mode == FoldMode.Displacement) {
              transformation = xyzResults[key].Select(item
                  => new Vector3d(item.Value.X.As(xyzunit) * _defScale,
                    item.Value.Y.As(xyzunit) * _defScale, item.Value.Z.As(xyzunit) * _defScale))
               .ToList();
            }

            break;

          case DisplayValue.Xx:
            vals = xxyyzzResults[key].Select(item => item.Value.X.ToUnit(xxyyzzunit)).ToList();
            break;

          case DisplayValue.Yy:
            vals = xxyyzzResults[key].Select(item => item.Value.Y.ToUnit(xxyyzzunit)).ToList();
            break;

          case DisplayValue.Zz:
            vals = xxyyzzResults[key].Select(item => item.Value.Z.ToUnit(xxyyzzunit)).ToList();
            break;

          case DisplayValue.ResXxyyzz:
            vals = xxyyzzResults[key].Select(item => item.Value.Xyz.ToUnit(xxyyzzunit)).ToList();
            break;
        }

        for (int i = 0; i < vals.Count - 1;
          i++) // start at i=0, now the last index is the centre point in GsaAPI output so to count -1
        {
          double tnorm = (2 * (vals[i].Value - dmin) / (dmax - dmin)) - 1;
          Color col = double.IsNaN(tnorm) ? Color.Transparent : ghGradient.ColourAt(tnorm);
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
          double tnorm = (2 * (vals.Last().Value - dmin) / (dmax - dmin)) - 1;
          Color col = double.IsNaN(tnorm) ? Color.Transparent : ghGradient.ColourAt(tnorm);
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
          double tnorm = (2 * (vals[0].Value - dmin) / (dmax - dmin)) - 1;
          Color col = double.IsNaN(tnorm) ? Color.Transparent : ghGradient.ColourAt(tnorm);
          for (int i = 0; i < tempmesh.Vertices.Count; i++) {
            tempmesh.VertexColors.Add(col);
          }
        }

        meshes[key] = tempmesh;
        values[key] = vals;
        verticies[key] = tempmesh.Vertices.Select(pt => (Point3d)pt).ToList();

        #endregion
      });

      #endregion

      resultMeshes.Add(meshes.Values.ToList(), values.Values.ToList(), verticies.Values.ToList(),
        meshes.Keys.ToList());

      #region Legend

      int gripheight = _legend.Height / ghGradient.GripCount;
      _legendValues = new List<string>();
      _legendValuesPosY = new List<int>();

      var ts = new List<GH_UnitNumber>();
      var cs = new List<Color>();

      for (int i = 0; i < ghGradient.GripCount; i++) {
        double t = dmin + ((dmax - dmin) / ((double)ghGradient.GripCount - 1) * i);
        double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
        scl = Math.Max(scl, 1);
        t = scl * Math.Round(t / scl, 3);

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
          case FoldMode.Force when (int)_disp < 4 | _isShear: {
            var forcePerLength = new ForcePerLength(t, DefaultUnits.ForcePerLengthUnit);
            _legendValues.Add(forcePerLength.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(forcePerLength));
            break;
          }
          case FoldMode.Force: {
            IQuantity length = new Length(0, _lengthUnit);
            string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
            _legendValues.Add(new Force(t, DefaultUnits.ForceUnit).ToString("s" + significantDigits)
              + lengthunitAbbreviation + "/" + lengthunitAbbreviation);
            var moment = new Moment(t, DefaultUnits.MomentUnit);
            ts.Add(new GH_UnitNumber(moment));
            break;
          }
          case FoldMode.Stress: {
            var stress = new Pressure(t, DefaultUnits.StressUnitResult);
            _legendValues.Add(stress.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(stress));
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
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[2]);
      base.UpdateUIFromSelectedItems();
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
  }
}
