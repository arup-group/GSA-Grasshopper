using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
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

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get Element3d results
  /// </summary>
  public class Elem3dContourResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("033035c6-16d9-4cc7-b2b3-cf5d5fac4108");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => GsaGH.Properties.Resources.Result3D;

    public Elem3dContourResults() : base("3D Contour Results",
      "ContourElem3d",
      "Displays GSA 3D Element Results as Contour",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + Environment.NewLine +
          "Element list should take the form:" + Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddColourParameter("Colour", "Co", "Optional list of colours to override default colours" +
          Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager.AddIntervalParameter("Min/Max Domain", "I", "Opitonal Domain for custom Min to Max contour colours", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, LengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Ngon Mesh", "M", "Ngon Mesh with coloured result values", GH_ParamAccess.item);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        #region Inputs
        if (gh_typ == null || gh_typ.Value == null)
        {
          this.AddRuntimeWarning("Input is null");
          return;
        }
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.CaseType.Combination && result.SelectedPermutationIDs.Count > 1)
          {
            this.AddRuntimeWarning("Combination Case " + result.CaseID + " contains "
                + result.SelectedPermutationIDs.Count + " permutations - only one permutation can be displayed at a time." +
                Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
          }
          if (result.Type == GsaResult.CaseType.Combination)
            _case = "Case C" + result.CaseID + " P" + result.SelectedPermutationIDs[0];
          if (result.Type == GsaResult.CaseType.AnalysisCase)
            _case = "Case A" + result.CaseID + Environment.NewLine + result.CaseName;
        }
        else
        {
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
        }

        // Get elemnt list filter
        string elementlist = "All";
        GH_String gh_Type = new GH_String();
        if (DA.GetData(1, ref gh_Type))
          GH_Convert.ToString(gh_Type, out elementlist, GH_Conversion.Both);

        if (elementlist.ToLower() == "all" || elementlist == "")
          elementlist = "All";

        // Get colours
        List<GH_Colour> gh_Colours = new List<GH_Colour>();
        List<Color> colors = new List<Color>();
        if (DA.GetDataList(2, gh_Colours))
        {
          for (int i = 0; i < gh_Colours.Count; i++)
          {
            Color color = new Color();
            GH_Convert.ToColor(gh_Colours[i], out color, GH_Conversion.Both);
            colors.Add(color);
          }
        }
        Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = Helpers.Graphics.Colours.Stress_Gradient(colors);

        // Get interval min/max
        GH_Interval gH_Interval = new GH_Interval();
        Interval customMinMax = Interval.Unset;
        if (DA.GetData(3, ref gH_Interval))
          GH_Convert.ToInterval(gH_Interval, ref customMinMax, GH_Conversion.Both);
        #endregion
        // get results from results class
        GsaResultsValues res = new GsaResultsValues();
        switch (_mode)
        {
          case FoldMode.Displacement:
            res = result.Element3DDisplacementValues(elementlist, this.LengthResultUnit)[0];
            break;

          case FoldMode.Stress:
            res = result.Element3DStressValues(elementlist, this.StressUnitResult)[0];
            break;
        }

        // get geometry for display from results class
        ReadOnlyDictionary<int, Element> elems = result.Model.Model.Elements(elementlist);
        ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes();

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = this.LengthResultUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Stress)
        {
          xyzunit = this.StressUnitResult;
          xxyyzzunit = this.StressUnitResult;
        }

        double dmax_x = res.dmax_x.As(xyzunit);
        double dmax_y = res.dmax_y.As(xyzunit);
        double dmax_z = res.dmax_z.As(xyzunit);
        double dmax_xyz = (_mode == FoldMode.Displacement) ? res.dmax_xyz.As(xyzunit) : 0;
        double dmin_x = res.dmin_x.As(xyzunit);
        double dmin_y = res.dmin_y.As(xyzunit);
        double dmin_z = res.dmin_z.As(xyzunit);
        double dmin_xyz = (_mode == FoldMode.Displacement) ? res.dmin_xyz.As(xyzunit) : 0;
        double dmax_xx = (_mode == FoldMode.Displacement) ? 0 : res.dmax_xx.As(xxyyzzunit);
        double dmax_yy = (_mode == FoldMode.Displacement) ? 0 : res.dmax_yy.As(xxyyzzunit);
        double dmax_zz = (_mode == FoldMode.Displacement) ? 0 : res.dmax_zz.As(xxyyzzunit);
        double dmax_xxyyzz = 0;
        double dmin_xx = (_mode == FoldMode.Displacement) ? 0 : res.dmin_xx.As(xxyyzzunit);
        double dmin_yy = (_mode == FoldMode.Displacement) ? 0 : res.dmin_yy.As(xxyyzzunit);
        double dmin_zz = (_mode == FoldMode.Displacement) ? 0 : res.dmin_zz.As(xxyyzzunit);
        double dmin_xxyyzz = 0;

        #region Result mesh values
        // ### Coloured Result Meshes ###

        double dmax = 0;
        double dmin = 0;
        switch (_disp)
        {
          case (DisplayValue.X):
            dmax = dmax_x;
            dmin = dmin_x;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Ux";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, xx";
            break;
          case (DisplayValue.Y):
            dmax = dmax_y;
            dmin = dmin_y;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uy";
            else if (_mode == FoldMode.Stress)
              resType = "2D Stress, yy";
            break;
          case (DisplayValue.Z):
            dmax = dmax_z;
            dmin = dmin_z;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uz";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, zz";
            break;
          case (DisplayValue.resXYZ):
            dmax = dmax_xyz;
            dmin = dmin_xyz;
            resType = "Res. Trans., |U|";
            break;
          case (DisplayValue.XX):
            dmax = dmax_xx;
            dmin = dmin_xx;
            resType = "Stress, xy";
            break;
          case (DisplayValue.YY):
            dmax = dmax_yy;
            dmin = dmin_yy;
            resType = "Stress, yz";
            break;
          case (DisplayValue.ZZ):
            dmax = dmax_zz;
            dmin = dmin_zz;
            resType = "Stress, zy";
            break;
          case (DisplayValue.resXXYYZZ):
            dmax = dmax_xxyyzz;
            dmin = dmin_xxyyzz;
            break;
        }
        if (customMinMax != Interval.Unset)
        {
          dmin = customMinMax.Min;
          dmax = customMinMax.Max;
        }
        List<double> rounded = Helpers.GsaAPI.ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];
        if (customMinMax != Interval.Unset)
        {
          dmin = customMinMax.Min;
          dmax = customMinMax.Max;
        }
        #region create mesh
        MeshResultGoo resultMeshes = new MeshResultGoo(new Mesh(), new List<List<IQuantity>>(), new List<List<Point3d>>());
        ConcurrentDictionary<int, Mesh> meshes = new ConcurrentDictionary<int, Mesh>();
        meshes.AsParallel().AsOrdered();
        ConcurrentDictionary<int, List<IQuantity>> values = new ConcurrentDictionary<int, List<IQuantity>>();
        values.AsParallel().AsOrdered();
        ConcurrentDictionary<int, List<Point3d>> verticies = new ConcurrentDictionary<int, List<Point3d>>();
        verticies.AsParallel().AsOrdered();

        LengthUnit lengthUnit = result.Model.ModelUnit;
        this.undefinedModelLengthUnit = false;
        if (lengthUnit == LengthUnit.Undefined)
        {
          lengthUnit = this.LengthUnit;
          this.undefinedModelLengthUnit = true;
          this.AddRuntimeRemarkMsg("Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in " + lengthUnit.ToString() + ". This can be changed by right-clicking the component -> 'Select Units'");
        }
        else
          this.LengthUnit = lengthUnit;

        // loop through elements
        Parallel.ForEach(elems.Keys, key => //foreach (int key in elems.Keys)
        {
          Element element = elems[key];
          if (element.Topology.Count < 5) { return; }
          Mesh tempmesh = Helpers.Import.Elements.ConvertElement3D(element, nodes, lengthUnit);
          if (tempmesh == null) { return; }

          List<Vector3d> transformation = null;
          List<IQuantity> vals = new List<IQuantity>();
          switch (_disp)
          {
            case (DisplayValue.X):
              vals = xyzResults[key].Select(item => item.Value.X.ToUnit(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            item.Value.X.As(lengthUnit) * _defScale,
                            0,
                            0)).ToList();
              break;

            case (DisplayValue.Y):
              vals = xyzResults[key].Select(item => item.Value.Y.ToUnit(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            0,
                            item.Value.Y.As(lengthUnit) * _defScale,
                            0)).ToList();
              break;

            case (DisplayValue.Z):
              vals = xyzResults[key].Select(item => item.Value.Z.ToUnit(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            0,
                            0,
                            item.Value.Z.As(lengthUnit) * _defScale)).ToList();
              break;

            case (DisplayValue.resXYZ):
              vals = xyzResults[key].Select(item => item.Value.XYZ.ToUnit(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            item.Value.X.As(lengthUnit) * _defScale,
                            item.Value.Y.As(lengthUnit) * _defScale,
                            item.Value.Z.As(lengthUnit) * _defScale)).ToList();
              break;

            case (DisplayValue.XX):
              vals = xxyyzzResults[key].Select(item => item.Value.X.ToUnit(xxyyzzunit)).ToList();
              break;
            case (DisplayValue.YY):
              vals = xxyyzzResults[key].Select(item => item.Value.Y.ToUnit(xxyyzzunit)).ToList();
              break;
            case (DisplayValue.ZZ):
              vals = xxyyzzResults[key].Select(item => item.Value.Z.ToUnit(xxyyzzunit)).ToList();
              break;
            case (DisplayValue.resXXYYZZ):
              vals = xxyyzzResults[key].Select(item => item.Value.XYZ.ToUnit(xxyyzzunit)).ToList();
              break;
          }

          for (int i = 0; i < vals.Count - 1; i++) // start at i=0, now the last index is the centre point in GsaAPI output so to count -1
          {
            //normalised value between -1 and 1
            double tnorm = 2 * (vals[i].Value - dmin) / (dmax - dmin) - 1;
            Color col = (double.IsNaN(tnorm)) ? Color.Transparent : gH_Gradient.ColourAt(tnorm);
            tempmesh.VertexColors.Add(col);
            if (transformation != null)
            {
              Point3f def = tempmesh.Vertices[i];
              def.Transform(Transform.Translation(transformation[i]));
              tempmesh.Vertices[i] = def;
            }
          }
          if (vals.Count == 1) // if analysis settings is set to '2D element forces and 2D/3D stresses at centre only'
          {
            //normalised value between -1 and 1
            double tnorm = 2 * (vals[0].Value - dmin) / (dmax - dmin) - 1;
            Color col = (double.IsNaN(tnorm)) ? Color.Transparent : gH_Gradient.ColourAt(tnorm);
            for (int i = 0; i < tempmesh.Vertices.Count; i++)
              tempmesh.VertexColors.SetColor(i, col);

            verticies[key] = new List<Point3d>()
              {
                new Point3d(
                  tempmesh.Vertices.Select(pt => pt.X).Average(),
                  tempmesh.Vertices.Select(pt => pt.Y).Average(),
                  tempmesh.Vertices.Select(pt => pt.Z).Average()
                )
              };
          }
          else
            verticies[key] = tempmesh.Vertices.Select(pt => (Point3d)pt).ToList();

          meshes[key] = tempmesh;
          values[key] = vals;
          #endregion
        });
        #endregion
        resultMeshes.Add(meshes.Values.ToList(), values.Values.ToList(), verticies.Values.ToList());

        #region Legend
        // ### Legend ###
        // loop through number of grip points in gradient to create legend
        int gripheight = legend.Height / gH_Gradient.GripCount;
        legendValues = new List<string>();
        legendValuesPosY = new List<int>();

        //Find Colour and Values for legend output
        List<GH_UnitNumber> ts = new List<GH_UnitNumber>();
        List<Color> cs = new List<Color>();

        for (int i = 0; i < gH_Gradient.GripCount; i++)
        {
          double t = dmin + (dmax - dmin) / ((double)gH_Gradient.GripCount - 1) * (double)i;
          if (t > 1)
          {
            double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
            scl = Math.Max(scl, 1);
            t = scl * Math.Round(t / scl, 3);
          }
          else
            t = Math.Round(t, significantDigits);

          Color gradientcolour = gH_Gradient.ColourAt(2 * (double)i / ((double)gH_Gradient.GripCount - 1) - 1);
          cs.Add(gradientcolour);

          // create legend for viewport
          int starty = i * gripheight;
          int endy = starty + gripheight;
          for (int y = starty; y < endy; y++)
          {
            for (int x = 0; x < legend.Width; x++)
              legend.SetPixel(x, legend.Height - y - 1, gradientcolour);
          }
          if (_mode == FoldMode.Displacement)
          {
            if ((int)_disp < 4)
            {
              Length displacement = new Length(t, this.LengthResultUnit);
              legendValues.Add(displacement.ToString("f" + significantDigits));
              ts.Add(new GH_UnitNumber(displacement));
              this.Message = Length.GetAbbreviation(this.LengthResultUnit);
            }
            else
            {
              Angle rotation = new Angle(t, AngleUnit.Radian);
              legendValues.Add(rotation.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(rotation));
              this.Message = Angle.GetAbbreviation(AngleUnit.Radian);
            }
          }
          if (_mode == FoldMode.Stress)
          {
            Pressure stress = new Pressure(t, this.StressUnitResult);
            legendValues.Add(stress.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(stress));
            this.Message = Pressure.GetAbbreviation(this.StressUnitResult);
          }
          if (Math.Abs(t) > 1)
            legendValues[i] = legendValues[i].Replace(",", string.Empty); // remove thousand separator
          legendValuesPosY.Add(legend.Height - starty + gripheight / 2 - 2);
        }
        #endregion

        // set outputs
        DA.SetData(0, resultMeshes);
        DA.SetDataList(1, cs);
        DA.SetDataList(2, ts);

        GsaResultsValues.ResultType resultType = (GsaResultsValues.ResultType)Enum.Parse(typeof(GsaResultsValues.ResultType), _mode.ToString());
        Helpers.PostHog.Result(result.Type, 3, resultType, this._disp.ToString());
      }
    }

    #region Custom UI
    private enum FoldMode
    {
      Displacement,
      Stress
    }

    private enum DisplayValue
    {
      X,
      Y,
      Z,
      resXYZ,
      XX,
      YY,
      ZZ,
      resXXYYZZ
    }

    readonly List<string> _type = new List<string>(new string[]
    {
      "Displacement",
      "Stress"
    });
    readonly List<string> _displacement = new List<string>(new string[]
    {
      "Translation Ux",
      "Translation Uy",
      "Translation Uz",
      "Resolved |U|",
    });
    readonly List<string> _stress = new List<string>(new string[]
    {
      "Stress xx",
      "Stress yy",
      "Stress zz",
      "Stress xy",
      "Stress yz",
      "Stress zx",
    });

    double _minValue = 0;
    double _maxValue = 1000;
    double _defScale = 250;
    int _noDigits = 0;
    bool _slider = true;
    string _case = "";

    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    LengthUnit LengthResultUnit = DefaultUnits.LengthUnitResult;
    bool undefinedModelLengthUnit = false;
    PressureUnit StressUnitResult = DefaultUnits.StressUnitResult;
    FoldMode _mode = FoldMode.Displacement;
    DisplayValue _disp = DisplayValue.resXYZ;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Result Type", "Component", "Deform Shape"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(this._type);
      this.SelectedItems.Add(this.DropDownItems[0][0]);

      // component
      this.DropDownItems.Add(this._displacement);
      this.SelectedItems.Add(this.DropDownItems[1][3]);

      this.IsInitialised = true;
    }
    public override void CreateAttributes()
    {
      if (!this.IsInitialised)
        this.InitialiseDropdowns();
      this.m_attributes = new OasysGH.UI.DropDownSliderComponentAttributes(this, SetSelected, this.DropDownItems, this.SelectedItems, this._slider, SetVal, SetMaxMin, this._defScale, this._maxValue, this._minValue, this._noDigits, this.SpacerDescriptions);
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0) // if change is made to first list
      {
        if (j == 0) // displacement mode
        {
          if (this.DropDownItems[1] != this._displacement)
          {
            this.DropDownItems[1] = this._displacement;
            this.SelectedItems[1] = this.DropDownItems[1][3]; // Resolved XYZ

            this._disp = (DisplayValue)3; // Resolved XYZ
            this.DeformationModeClicked();
          }
        }
        if (j == 1) // stress mode
        {
          if (this.DropDownItems[1] != this._stress)
          {
            this.DropDownItems[1] = this._stress;
            this.SelectedItems[1] = this.DropDownItems[1][2];

            this._disp = (DisplayValue)2;
            this.StressModeClicked();
          }
        }
      }
      else if (i == 1) // if change is made to display mode of result
      {
        bool redraw = false;
        this.SelectedItems[1] = this.DropDownItems[1][j];
        if (this._mode == FoldMode.Displacement)
        {
          if ((int)this._disp > 3 & j < 4)
          {
            redraw = true;
            this._slider = true;
          }
          if ((int)this._disp < 4 & j > 3)
          {
            redraw = true;
            this._slider = false;
          }
          this._disp = (DisplayValue)j;
        }
        else
        {
          if (j < 3)
            this._disp = (DisplayValue)j;
          else
            this._disp = (DisplayValue)(j + 1);
        }

        if (redraw)
          this.ReDrawComponent();
      }
      base.UpdateUI();
    }
    public void SetVal(double value)
    {
      this._defScale = value;
    }
    public void SetMaxMin(double max, double min)
    {
      this._maxValue = max;
      this._minValue = min;
    }

    public override void VariableParameterMaintenance()
    {
      if (this.Params.Input.Count != 4)
      {
        this.Params.RegisterInputParam(new Param_Interval());
        this.Params.Input[3].Name = "Min/Max Domain";
        this.Params.Input[3].NickName = "I";
        this.Params.Input[3].Description = "Opitonal Domain for custom Min to Max contour colours";
        this.Params.Input[3].Optional = true;
        this.Params.Input[3].Access = GH_ParamAccess.item;
      }

      if (this._mode == FoldMode.Displacement)
      {
        if ((int)_disp < 4)
          this.Params.Output[2].Name = "Values [" + Length.GetAbbreviation(this.LengthResultUnit) + "]";
        else
          this.Params.Output[2].Name = "Values [rad]";
      }

      if (this._mode == FoldMode.Stress)
      {
        this.Params.Output[2].Name = "Legend Values [" + Pressure.GetAbbreviation(this.StressUnitResult) + "]";
      }
    }
    #endregion

    #region menu override
    protected override void BeforeSolveInstance()
    {
      switch (_mode)
      {
        case FoldMode.Displacement:
          this.Message = Length.GetAbbreviation(this.LengthResultUnit);
          break;

        case FoldMode.Stress:
          this.Message = Pressure.GetAbbreviation(this.StressUnitResult);
          break;
      }
    }
    private void ReDrawComponent()
    {
      PointF pivot = new PointF(this.Attributes.Pivot.X, this.Attributes.Pivot.Y);
      this.CreateAttributes();
      this.Attributes.Pivot = pivot;
      this.Attributes.ExpireLayout();
      this.Attributes.PerformLayout();
    }
    private void DeformationModeClicked()
    {
      if (this._mode == FoldMode.Displacement)
        return;

      RecordUndoEvent(this._mode.ToString() + " Parameters");
      this._mode = FoldMode.Displacement;

      this._slider = true;
      this._defScale = 100;

      ReDrawComponent();
    }

    private void StressModeClicked()
    {
      if (this._mode == FoldMode.Stress)
        return;

      RecordUndoEvent(this._mode.ToString() + " Parameters");
      this._mode = FoldMode.Stress;

      this._slider = false;
      this._defScale = 0;

      this.ReDrawComponent();
    }
    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, _showLegend);

      Grasshopper.Kernel.Special.GH_GradientControl gradient = new Grasshopper.Kernel.Special.GH_GradientControl();
      gradient.CreateAttributes();
      ToolStripMenuItem extract = new ToolStripMenuItem("Extract Default Gradient", gradient.Icon_24x24, (s, e) => { CreateGradient(); });
      menu.Items.Add(extract);

      ToolStripMenuItem lengthUnitsMenu = new ToolStripMenuItem("Displacement");
      lengthUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateLength(unit); });
        toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthResultUnit);
        toolStripMenuItem.Enabled = true;
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem stressUnitsMenu = new ToolStripMenuItem("Stress");
      stressUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateStress(unit); });
        toolStripMenuItem.Checked = unit == Pressure.GetAbbreviation(this.StressUnitResult);
        toolStripMenuItem.Enabled = true;
        stressUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);

      if (undefinedModelLengthUnit)
      {
        ToolStripMenuItem modelUnitsMenu = new ToolStripMenuItem("Model geometry");
        modelUnitsMenu.Enabled = true;
        foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
        {
          ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateModel(unit); });
          toolStripMenuItem.Checked = unit == Length.GetAbbreviation(this.LengthUnit);
          toolStripMenuItem.Enabled = true;
          modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);
        }
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { modelUnitsMenu, lengthUnitsMenu, stressUnitsMenu });
      }
      else
      {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { lengthUnitsMenu, stressUnitsMenu });
      }
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void UpdateModel(string unit)
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateLength(string unit)
    {
      this.LengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateStress(string unit)
    {
      this.StressUnitResult = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    bool _showLegend = true;
    private void ShowLegend(object sender, EventArgs e)
    {
      _showLegend = !_showLegend;
      this.ExpirePreview(true);
    }

    private void CreateGradient()
    {
      Grasshopper.Kernel.Special.GH_GradientControl gradient = new Grasshopper.Kernel.Special.GH_GradientControl();
      gradient.CreateAttributes();

      gradient.Gradient = Helpers.Graphics.Colours.Stress_Gradient(null);
      gradient.Gradient.NormalizeGrips();
      gradient.Params.Input[0].AddVolatileData(new GH_Path(0), 0, -1);
      gradient.Params.Input[1].AddVolatileData(new GH_Path(0), 0, 1);
      gradient.Params.Input[2].AddVolatileDataList(
        new GH_Path(0),
        new List<double>() { -1, -0.666, -0.333, 0, 0.333, 0.666, 1 });

      gradient.Attributes.Pivot = new PointF(this.Attributes.Bounds.X - gradient.Attributes.Bounds.Width - 50, this.Params.Input[2].Attributes.Bounds.Y - gradient.Attributes.Bounds.Height / 4 - 6);

      Grasshopper.Instances.ActiveCanvas.Document.AddObject(gradient, false);
      this.Params.Input[2].RemoveAllSources();
      this.Params.Input[2].AddSource(gradient.Params.Output[0]);

      this.UpdateUI();
    }
    #endregion

    #region draw legend
    Bitmap legend = new Bitmap(15, 120);
    List<string> legendValues;
    List<int> legendValuesPosY;
    string resType;
    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);
      if (legendValues != null & _showLegend)
      {
        args.Display.DrawBitmap(new DisplayBitmap(legend), args.Viewport.Bounds.Right - 110, 20);
        for (int i = 0; i < legendValues.Count; i++)
          args.Display.Draw2dText(legendValues[i], Color.Black, new Point2d(args.Viewport.Bounds.Right - 85, legendValuesPosY[i]), false);
        args.Display.Draw2dText(resType, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 7), false);
        args.Display.Draw2dText(_case, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 145), false);
      }
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetBoolean("slider", _slider);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetBoolean("legend", _showLegend);
      writer.SetString("model", Length.GetAbbreviation(this.LengthUnit));
      writer.SetString("length", Length.GetAbbreviation(this.LengthResultUnit));
      writer.SetString("stress", Pressure.GetAbbreviation(this.StressUnitResult));
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      _showLegend = reader.GetBoolean("legend");
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      this.LengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      this.StressUnitResult = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), reader.GetString("stress"));
      return base.Read(reader);
    }
    #endregion
  }
}
