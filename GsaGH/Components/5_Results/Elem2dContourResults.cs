using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
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

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get Element2d results
    /// </summary>
    public class Elem2dContourResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e2b011dc-c5ca-46fd-87f5-b888b27ef684");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => GsaGH.Properties.Resources.Result2D;

    public Elem2dContourResults() : base("2D Contour Results",
      "ContourElem2d",
      "Displays GSA 2D Element Results as Contour",
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
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, LengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Mesh", "M", "Mesh with coloured result values", GH_ParamAccess.item);
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
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input is null");
          return;
        }
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.ResultType.Combination && result.SelectedPermutationIDs.Count > 1)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Combination Case " + result.CaseID + " contains "
                + result.SelectedPermutationIDs.Count + " permutations - only one permutation can be displayed at a time." +
                Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
          }
          if (result.Type == GsaResult.ResultType.Combination)
            _case = "Case C" + result.CaseID + " P" + result.SelectedPermutationIDs[0];
          if (result.Type == GsaResult.ResultType.AnalysisCase)
            _case = "Case A" + result.CaseID + Environment.NewLine + result.CaseName;
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
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

        #endregion
        // get results from results class
        GsaResultsValues res = new GsaResultsValues();
        GsaResultsValues resShear = new GsaResultsValues();
        switch (_mode)
        {
          case FoldMode.Displacement:
            res = result.Element2DDisplacementValues(elementlist, this.LengthResultUnit)[0];
            break;

          case FoldMode.Force:
            res = result.Element2DForceValues(elementlist,
                this.ForcePerLengthUnit, this.ForceUnit)[0];
            resShear = result.Element2DShearValues(elementlist, this.ForcePerLengthUnit)[0];
            break;
          case FoldMode.Stress:
            res = result.Element2DStressValues(elementlist,
                _flayer, this.StressUnitResult)[0];
            break;
        }

        // get geometry for display from results class
        ConcurrentDictionary<int, Element> elems = new ConcurrentDictionary<int, Element>(result.Model.Model.Elements(elementlist));
        ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(result.Model.Model.Nodes());

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = (_isShear) ? resShear.xyzResults : res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = this.LengthResultUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Force)
        {
          xyzunit = this.ForcePerLengthUnit;
          xxyyzzunit = this.ForceUnit;
        }
        else if (_mode == FoldMode.Stress)
        {
          xyzunit = this.StressUnitResult;
          xxyyzzunit = this.StressUnitResult;
        }

        if ((_isShear ? resShear.dmax_x : res.dmax_x) == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Result does not contain any values for 2D Elements");
          return;
        }

        double dmax_x = (_isShear) ? resShear.dmax_x.As(xyzunit) : res.dmax_x.As(xyzunit);
        double dmax_y = (_isShear) ? resShear.dmax_y.As(xyzunit) : res.dmax_y.As(xyzunit);
        double dmax_z = res.dmax_z.As(xyzunit);
        double dmax_xyz = (_mode == FoldMode.Displacement) ? res.dmax_xyz.As(xyzunit) : 0;
        double dmin_x = (_isShear) ? resShear.dmin_x.As(xyzunit) : res.dmin_x.As(xyzunit);
        double dmin_y = (_isShear) ? resShear.dmin_y.As(xyzunit) : res.dmin_y.As(xyzunit);
        double dmin_z = res.dmin_z.As(xyzunit);
        double dmin_xyz = (_mode == FoldMode.Displacement) ? res.dmin_xyz.As(xyzunit) : 0;
        double dmax_xx = (_isShear) ? 0 : res.dmax_xx.As(xxyyzzunit);
        double dmax_yy = (_isShear) ? 0 : res.dmax_yy.As(xxyyzzunit);
        double dmax_zz = (_isShear) ? 0 : res.dmax_zz.As(xxyyzzunit);
        double dmax_xxyyzz = 0;
        double dmin_xx = (_isShear) ? 0 : res.dmin_xx.As(xxyyzzunit);
        double dmin_yy = (_isShear) ? 0 : res.dmin_yy.As(xxyyzzunit);
        double dmin_zz = (_isShear) ? 0 : res.dmin_zz.As(xxyyzzunit);
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
            else if (_mode == FoldMode.Force & !_isShear)
              resType = "2D Force, Nx";
            else if (_mode == FoldMode.Force & _isShear)
              resType = "2D Shear, Qx";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, xx";
            break;
          case (DisplayValue.Y):
            dmax = dmax_y;
            dmin = dmin_y;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uy";
            else if (_mode == FoldMode.Force & !_isShear)
              resType = "2D Force, Ny";
            else if (_mode == FoldMode.Force & _isShear)
              resType = "2D Shear, Qy";
            else if (_mode == FoldMode.Stress)
              resType = "2D Stress, yy";
            break;
          case (DisplayValue.Z):
            dmax = dmax_z;
            dmin = dmin_z;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uz";
            else if (_mode == FoldMode.Force & !_isShear)
              resType = "2D Force, Nxy";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, zz";
            break;
          case (DisplayValue.resXYZ):
            dmax = dmax_xyz;
            dmin = dmin_xyz;
            if (_mode == FoldMode.Displacement)
              resType = "Res. Trans., |U|";
            break;
          case (DisplayValue.XX):
            dmax = dmax_xx;
            dmin = dmin_xx;
            if (_mode == FoldMode.Force & !_isShear)
              resType = "2D Moment, Mx";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, xy";
            break;
          case (DisplayValue.YY):
            dmax = dmax_yy;
            dmin = dmin_yy;
            if (_mode == FoldMode.Force & !_isShear)
              resType = "2D Moment, My";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, yz";
            break;
          case (DisplayValue.ZZ):
            dmax = dmax_zz;
            dmin = dmin_zz;
            if (_mode == FoldMode.Force & !_isShear)
              resType = "2D Moment, Mxy";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, zy";
            break;
          case (DisplayValue.resXXYYZZ):
            dmax = dmax_xxyyzz;
            dmin = dmin_xxyyzz;
            break;
        }

        List<double> rounded = Helpers.GsaAPI.ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];

        #region create mesh
        ResultMeshGoo resultMeshes = new ResultMeshGoo(new Mesh(), new List<List<double>>());
        ConcurrentDictionary<int, Mesh> meshes = new ConcurrentDictionary<int, Mesh>();
        meshes.AsParallel().AsOrdered();
        ConcurrentDictionary<int, List<double>> values = new ConcurrentDictionary<int, List<double>>();
        values.AsParallel().AsOrdered();

        LengthUnit lengthUnit = result.Model.ModelUnit;
        this.undefinedModelLengthUnit = false;
        if (lengthUnit == LengthUnit.Undefined)
        {
          lengthUnit = this.LengthUnit;
          this.undefinedModelLengthUnit = true;
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in " + lengthUnit.ToString() + ". This can be changed by right-clicking the component -> 'Select Units'");
        }
        else
          this.LengthUnit = lengthUnit;

        // loop through elements
        Parallel.ForEach(elems.Keys, key => //foreach (int key in elems.Keys)
        {
          Element element = elems[key];
          if (element.Topology.Count < 3) { return; }
          Mesh tempmesh = Helpers.Import.Elements.ConvertElement2D(element, nodes, lengthUnit);
          if (tempmesh == null) { return; }

          List<Vector3d> transformation = null;
          List<double> vals = new List<double>();
          switch (_disp)
          {
            case (DisplayValue.X):
              vals = xyzResults[key].Select(item => item.Value.X.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            item.Value.X.As(lengthUnit) * _defScale,
                            0,
                            0)).ToList();
              break;

            case (DisplayValue.Y):
              vals = xyzResults[key].Select(item => item.Value.Y.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            0,
                            item.Value.Y.As(lengthUnit) * _defScale,
                            0)).ToList();
              break;

            case (DisplayValue.Z):
              vals = xyzResults[key].Select(item => item.Value.Z.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            0,
                            0,
                            item.Value.Z.As(lengthUnit) * _defScale)).ToList();
              break;

            case (DisplayValue.resXYZ):
              vals = xyzResults[key].Select(item => item.Value.XYZ.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            item.Value.X.As(lengthUnit) * _defScale,
                            item.Value.Y.As(lengthUnit) * _defScale,
                            item.Value.Z.As(lengthUnit) * _defScale)).ToList();
              break;

            case (DisplayValue.XX):
              vals = xxyyzzResults[key].Select(item => item.Value.X.As(xxyyzzunit)).ToList();
              break;
            case (DisplayValue.YY):
              vals = xxyyzzResults[key].Select(item => item.Value.Y.As(xxyyzzunit)).ToList();
              break;
            case (DisplayValue.ZZ):
              vals = xxyyzzResults[key].Select(item => item.Value.Z.As(xxyyzzunit)).ToList();
              break;
            case (DisplayValue.resXXYYZZ):
              vals = xxyyzzResults[key].Select(item => item.Value.XYZ.As(xxyyzzunit)).ToList();
              break;
          }

          for (int i = 0; i < vals.Count - 1; i++) // start at i=0, now the last index is the centre point in GsaAPI output so to count -1
          {
            //normalised value between -1 and 1
            double tnorm = 2 * (vals[i] - dmin) / (dmax - dmin) - 1;
            Color col = (double.IsNaN(tnorm)) ? Color.Transparent : gH_Gradient.ColourAt(tnorm);
            tempmesh.VertexColors.Add(col);
            if (transformation != null)
            {
              Point3f def = tempmesh.Vertices[i];
              def.Transform(Transform.Translation(transformation[i]));
              tempmesh.Vertices[i] = def;
            }
          }
          if (tempmesh.Vertices.Count == 9) // add the value/colour at the centre point if quad-8 (as it already has a vertex here)
          {
            double tnorm = 2 * (vals.Last() - dmin) / (dmax - dmin) - 1;
            Color col = (double.IsNaN(tnorm)) ? Color.Transparent : gH_Gradient.ColourAt(tnorm);
            tempmesh.VertexColors.Add(col);
            if (transformation != null)
            {
              Point3f def = tempmesh.Vertices[8];
              def.Transform(Transform.Translation(transformation.Last()));
              tempmesh.Vertices[8] = def;
            }
          }
          if (vals.Count == 1) // if analysis settings is set to '2D element forces and 2D/3D stresses at centre only'
          {
            //normalised value between -1 and 1
            double tnorm = 2 * (vals[0] - dmin) / (dmax - dmin) - 1;
            Color col = (double.IsNaN(tnorm)) ? Color.Transparent : gH_Gradient.ColourAt(tnorm);
            for (int i = 0; i < tempmesh.Vertices.Count; i++)
              tempmesh.VertexColors.Add(col);
          }
          meshes[key] = tempmesh;
          values[key] = vals;
          #endregion
        });
        #endregion
        resultMeshes.Add(meshes.Values.ToList(), values.Values.ToList());

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
          double scl = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t))) + 1);
          scl = Math.Max(scl, 1);
          t = scl * Math.Round(t / scl, 3);

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
          if (_mode == FoldMode.Force)
          {
            if ((int)_disp < 4 | _isShear)
            {
              ForcePerLength forcePerLength = new ForcePerLength(t, this.ForcePerLengthUnit);
              legendValues.Add(forcePerLength.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(forcePerLength));
              this.Message = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);
            }
            else
            {
              legendValues.Add(new Moment(t, UnitsHelper.GetMomentUnit(this.ForceUnit, lengthUnit)).ToString("s" + significantDigits) + "/" + Length.GetAbbreviation(lengthUnit));
              Moment moment = new Moment(t, UnitsHelper.GetMomentUnit(this.ForceUnit, lengthUnit)); // this is technically moment per length
              ts.Add(new GH_UnitNumber(moment));
              this.Message = Moment.GetAbbreviation(UnitsHelper.GetMomentUnit(this.ForceUnit, lengthUnit)) + "/" + Length.GetAbbreviation(lengthUnit);
            }
          }
          if (_mode == FoldMode.Stress)
          {
            Pressure stress = new Pressure(t, this.StressUnitResult);
            legendValues.Add(stress.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(stress));
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
      }
    }

    #region Custom UI
    private enum FoldMode
    {
      Displacement,
      Force,
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
    List<string> _type = new List<string>(new string[]
    {
            "Displacement",
            "Force",
            "Stress"
    });
    List<string> _displacement = new List<string>(new string[]
    {
            "Translation Ux",
            "Translation Uy",
            "Translation Uz",
            "Resolved |U|",
    });

    List<string> _force = new List<string>(new string[]
    {
            "Force Nx",
            "Force Ny",
            "Force Nxy",
            "Shear Qx",
            "Shear Qy",
            "Moment Mx",
            "Moment My",
            "Moment Mxy"
    });

    List<string> _stress = new List<string>(new string[]
    {
            "Stress xx",
            "Stress yy",
            "Stress zz",
            "Stress xy",
            "Stress yz",
            "Stress zx",
    });

    List<string> _layer = new List<string>(new string[]
    {
            "Top",
            "Middle",
            "Bottom"
    });
    double _minValue = 0;
    double _maxValue = 1000;
    double _defScale = 250;
    int _noDigits = 0;
    bool _slider = true;
    string _case = "";
    bool _isShear;
    int _flayer = 0;

    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    LengthUnit LengthResultUnit = DefaultUnits.LengthUnitResult;
    bool undefinedModelLengthUnit = false;
    ForcePerLengthUnit ForcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;
    ForceUnit ForceUnit = DefaultUnits.ForceUnit;
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

    public override void SetSelected(int dropdownlistidd, int selectedidd)
    {
      if (dropdownlistidd == 0) // if change is made to first list
      {
        if (selectedidd == 0) // displacement mode
        {
          if (DropDownItems[1] != _displacement)
          {
            if (DropDownItems.Count == 4) // if coming from stress we remove the layer dropdown
            {
              DropDownItems.RemoveAt(2);
              SelectedItems.RemoveAt(2);
              SpacerDescriptions.RemoveAt(2);
            }

            DropDownItems[1] = _displacement;
            DropDownItems[2] = FilteredUnits.FilteredLengthUnits;

            SelectedItems[0] = DropDownItems[0][0]; // displacement
            SelectedItems[1] = DropDownItems[1][3]; // Resolved XYZ

            _disp = (DisplayValue)3;
            _isShear = false;
            _flayer = 0;
            Mode1Clicked();
          }
        }
        if (selectedidd == 1)  // force mode
        {
          if (DropDownItems[1] != _force)
          {
            if (DropDownItems.Count == 4) // if coming from stress we remove the layer dropdown
            {
              DropDownItems.RemoveAt(2);
              SelectedItems.RemoveAt(2);
              SpacerDescriptions.RemoveAt(2);
            }

            DropDownItems[1] = _force;

            SelectedItems[0] = DropDownItems[0][1];
            SelectedItems[1] = DropDownItems[1][0];

            _disp = 0;
            _isShear = false;
            _flayer = 0;
            Mode2Clicked();
          }
        }
        if (selectedidd == 2) // stress mode
        {
          if (DropDownItems[1] != _stress)
          {
            if (DropDownItems.Count < 4)
            {
              DropDownItems.Insert(2, _layer); //insert layer dropdown as third dd list
              SpacerDescriptions.Insert(2, "Layer");
            }

            DropDownItems[1] = _stress;

            SelectedItems[0] = DropDownItems[0][2];
            SelectedItems[1] = DropDownItems[1][0];

            if (SelectedItems.Count < 4)
              SelectedItems.Insert(2, DropDownItems[2][1]);
            else
              SelectedItems[2] = DropDownItems[2][1];

            _disp = 0;
            _isShear = false;
            Mode4Clicked();
          }
        }
      }
      else if (dropdownlistidd == 1) // if change is made to second list, the type of result
      {
        bool redraw = false;
        SelectedItems[1] = DropDownItems[1][selectedidd];
        if (_mode == FoldMode.Displacement)
        {
          if ((int)_disp > 3 & selectedidd < 4)
          {
            redraw = true;
            _slider = true;
          }
          if ((int)_disp < 4 & selectedidd > 3)
          {
            redraw = true;
            _slider = false;

          }
        }
        _disp = (DisplayValue)selectedidd;
        if (DropDownItems[1] != _displacement)
        {
          _isShear = false;
          if (_mode == FoldMode.Force)
          {
            if (selectedidd == 3 | selectedidd == 4)
            {
              _disp = (DisplayValue)selectedidd - 3;
              _isShear = true;
            }
            else if (selectedidd > 4)
              _disp = (DisplayValue)selectedidd - 1;

          }
          else if (_mode == FoldMode.Force)
          {
            if (selectedidd > 2)
              _disp = (DisplayValue)selectedidd + 1;
          }
        }

        if (redraw)
          ReDrawComponent();
      }
      else if (dropdownlistidd == 2 && _mode == FoldMode.Stress) // if change is made to third list
      {
        if (selectedidd == 0)
          _flayer = 1;
        if (selectedidd == 1)
          _flayer = 0;
        if (selectedidd == 2)
          _flayer = -1;
      }

      base.UpdateUI();
    }
    public void SetVal(double value)
    {
      _defScale = value;
    }
    public void SetMaxMin(double max, double min)
    {
      _maxValue = max;
      _minValue = min;
    }

    public override void VariableParameterMaintenance()
    {
      if (_mode == FoldMode.Displacement)
      {
        if ((int)_disp < 4)
          Params.Output[2].Name = "Values [" + Length.GetAbbreviation(this.LengthResultUnit) + "]";
        else
          Params.Output[2].Name = "Values [rad]";
      }

      if (_mode == FoldMode.Force)
      {
        
        if ((int)_disp < 4 | _isShear)
          Params.Output[2].Name = "Legend Values [" + ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit) + "/" + Length.GetAbbreviation(this.LengthUnit) + "]";
        else
          Params.Output[2].Name = "Legend Values [" + Force.GetAbbreviation(this.ForceUnit) + "·" + Length.GetAbbreviation(this.LengthUnit) + "/" + Length.GetAbbreviation(this.LengthUnit) + "]";
      }

      if (_mode == FoldMode.Stress)
      {
        Params.Output[2].Name = "Legend Values [" + Pressure.GetAbbreviation(this.StressUnitResult) + "]";
      }
    }
    #endregion

    #region menu override
    private void ReDrawComponent()
    {
      PointF pivot = new PointF(this.Attributes.Pivot.X, this.Attributes.Pivot.Y);
      this.CreateAttributes();
      this.Attributes.Pivot = pivot;
      this.Attributes.ExpireLayout();
      this.Attributes.PerformLayout();
    }
    private void Mode1Clicked()
    {
      if (_mode == FoldMode.Displacement)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.Displacement;

      _slider = true;
      _defScale = 100;

      ReDrawComponent();
    }
    private void Mode2Clicked()
    {
      if (_mode == FoldMode.Force)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.Force;

      _slider = false;
      _defScale = 0;
      SpacerDescriptions[2] = "Deform Shape";

      ReDrawComponent();
    }

    private void Mode4Clicked()
    {
      if (_mode == FoldMode.Stress)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.Stress;

      _slider = false;
      _defScale = 0;


      ReDrawComponent();
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

      ToolStripMenuItem forceUnitsMenu = new ToolStripMenuItem("Force");
      forceUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateForce(unit); });
        toolStripMenuItem.Checked = unit == ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);
        toolStripMenuItem.Enabled = true;
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem momentUnitsMenu = new ToolStripMenuItem("Moment");
      momentUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateMoment(unit); });
        toolStripMenuItem.Checked = unit == Force.GetAbbreviation(this.ForceUnit);
        toolStripMenuItem.Enabled = true;
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
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
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { modelUnitsMenu, lengthUnitsMenu, forceUnitsMenu, momentUnitsMenu, stressUnitsMenu });
      }
      else
      {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { lengthUnitsMenu, forceUnitsMenu, momentUnitsMenu, stressUnitsMenu });
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
    private void UpdateForce(string unit)
    {
      this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateMoment(string unit)
    {
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
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
      writer.SetInt32("flayer", _flayer);
      writer.SetBoolean("slider", _slider);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      writer.SetBoolean("legend", _showLegend);
      writer.SetString("model", Length.GetAbbreviation(this.LengthUnit));
      writer.SetString("length", Length.GetAbbreviation(this.LengthResultUnit));
      writer.SetString("force", ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit));
      writer.SetString("moment", Force.GetAbbreviation(this.ForceUnit));
      writer.SetString("stress", Pressure.GetAbbreviation(this.StressUnitResult));
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _flayer = reader.GetInt32("flayer");
      _slider = reader.GetBoolean("slider");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      _showLegend = reader.GetBoolean("legend");
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      this.LengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), reader.GetString("force"));
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("moment"));
      this.StressUnitResult = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), reader.GetString("stress"));
      return base.Read(reader);
    }
    #endregion
  }
}
