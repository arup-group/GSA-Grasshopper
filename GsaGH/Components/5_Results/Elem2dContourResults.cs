using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using GsaAPI;
using GsaGH.Parameters;
using System.Linq;
using Grasshopper.Kernel.Data;
using Oasys.Units;
using UnitsNet.Units;
using UnitsNet;
using GsaGH.Util.Gsa;
using System.Drawing;
using System.Windows.Forms;
using Rhino.Display;
using UnitsNet.GH;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get Element2d results
  /// </summary>
  public class Elem2dContourResults : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("935d359a-9394-42fc-a76e-ea08ccb84135");
    public Elem2dContourResults()
      : base("2D Contour Results", "ContourElem2d", "Displays GSA 2D Element Results as Contour",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override Bitmap Icon => GsaGH.Properties.Resources.Result2D;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        dropdownitems.Add(dropdowntopitems);
        dropdownitems.Add(dropdowndisplacement);
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems = new List<string>();
        selecteditems.Add(dropdownitems[0][0]);
        selecteditems.Add(dropdownitems[1][3]);
        selecteditems.Add(Units.LengthUnitGeometry.ToString());
        first = false;
      }
      m_attributes = new UI.MultiDropDownSliderComponentUI(this, SetSelected, dropdownitems, selecteditems, slider, SetVal, SetMaxMin, DefScale, MaxValue, MinValue, noDigits, spacerDescriptions);
    }

    double MinValue = 0;
    double MaxValue = 1000;
    double DefScale = 250;
    int noDigits = 0;
    bool slider = true;
    public void SetVal(double value)
    {
      DefScale = value;
    }
    public void SetMaxMin(double max, double min)
    {
      MaxValue = max;
      MinValue = min;
    }
    public void SetSelected(int dropdownlistidd, int selectedidd)
    {
      if (dropdownlistidd == 0) // if change is made to first list
      {
        if (selectedidd == 0) // displacement mode
        {
          if (dropdownitems[1] != dropdowndisplacement)
          {
            if (dropdownitems.Count == 4) // if coming from stress we remove the layer dropdown
            {
              dropdownitems.RemoveAt(2);
              selecteditems.RemoveAt(2);
              spacerDescriptions.RemoveAt(2);
            }

            dropdownitems[1] = dropdowndisplacement;
            dropdownitems[2] = Units.FilteredLengthUnits;

            selecteditems[0] = dropdownitems[0][0]; // displacement
            selecteditems[1] = dropdownitems[1][3]; // Resolved XYZ
            selecteditems[2] = lengthUnit.ToString();

            _disp = (DisplayValue)3;
            isShear = false;
            flayer = 0;
            Mode1Clicked();
          }
        }
        if (selectedidd == 1)  // force mode
        {
          if (dropdownitems[1] != dropdownforce)
          {
            if (dropdownitems.Count == 4) // if coming from stress we remove the layer dropdown
            {
              dropdownitems.RemoveAt(2);
              selecteditems.RemoveAt(2);
              spacerDescriptions.RemoveAt(2);
            }

            dropdownitems[1] = dropdownforce;

            selecteditems[0] = dropdownitems[0][1];
            selecteditems[1] = dropdownitems[1][0];
            selecteditems[2] = lengthUnit.ToString();

            _disp = 0;
            isShear = false;
            flayer = 0;
            Mode2Clicked();
          }
        }
        if (selectedidd == 2) // stress mode
        {
          if (dropdownitems[1] != dropdownstress)
          {
            if (dropdownitems.Count < 4)
            {
              dropdownitems.Insert(2, dropdownlayer); //insert layer dropdown as third dd list
              spacerDescriptions.Insert(2, "Layer");
            }

            dropdownitems[1] = dropdownstress;

            selecteditems[0] = dropdownitems[0][2];
            selecteditems[1] = dropdownitems[1][0];

            if (selecteditems.Count < 4)
              selecteditems.Insert(2, dropdownitems[2][1]);
            else
              selecteditems[2] = dropdownitems[2][1];

            selecteditems[3] = lengthUnit.ToString();

            _disp = 0;
            isShear = false;
            Mode4Clicked();
          }
        }
      }
      else if (dropdownlistidd == 1) // if change is made to second list, the type of result
      {
        bool redraw = false;
        selecteditems[1] = dropdownitems[1][selectedidd];
        if (_mode == FoldMode.Displacement)
        {
          if ((int)_disp > 3 & selectedidd < 4)
          {
            redraw = true;
            slider = true;
          }
          if ((int)_disp < 4 & selectedidd > 3)
          {
            redraw = true;
            slider = false;

          }
        }
        _disp = (DisplayValue)selectedidd;
        if (dropdownitems[1] != dropdowndisplacement)
        {
          isShear = false;
          if (_mode == FoldMode.Force)
          {
            if (selectedidd == 3 | selectedidd == 4)
            {
              _disp = (DisplayValue)selectedidd - 3;
              isShear = true;
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

        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        Params.OnParametersChanged();
        ExpireSolution(true);
      }
      else if (dropdownlistidd == 2 && _mode == FoldMode.Stress) // if change is made to third list
      {
        if (selectedidd == 0)
          flayer = 1;
        if (selectedidd == 1)
          flayer = 0;
        if (selectedidd == 2)
          flayer = -1;
        ExpireSolution(true);
      }
      else
      {
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);
      }

    }
    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    List<List<string>> dropdownitems; // list that holds all dropdown contents
    List<string> selecteditems;
    bool first = true;
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Result Type",
            "Component",
            "Geometry Unit",
            "Deform Shape"
    });
    List<string> dropdowntopitems = new List<string>(new string[]
    {
            "Displacement",
            "Force",
            "Stress"
    });
    List<string> dropdowndisplacement = new List<string>(new string[]
    {
            "Translation Ux",
            "Translation Uy",
            "Translation Uz",
            "Resolved |U|",
    });

    List<string> dropdownforce = new List<string>(new string[]
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

    List<string> dropdownstress = new List<string>(new string[]
    {
            "Stress xx",
            "Stress yy",
            "Stress zz",
            "Stress xy",
            "Stress yz",
            "Stress zx",
    });

    List<string> dropdownlayer = new List<string>(new string[]
    {
            "Top",
            "Middle",
            "Bottom"
    });
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    private LengthUnit lengthResultUnit = Units.LengthUnitResult;
    string _case = "";
    bool isShear;
    int flayer = 0;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddColourParameter("Colour", "Co", "Optional list of colours to override default colours" +
          System.Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Mesh", "M", "Mesh with coloured result values", GH_ParamAccess.item);
      pManager.AddGenericParameter("Colours", "LC", "Legend Colours", GH_ParamAccess.list);
      pManager.AddGenericParameter("Values [" + lengthunitAbbreviation + "]", "LT", "Legend Values", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        #region Inputs
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.ResultType.Combination && result.SelectedPermutationIDs.Count > 1)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Combination case contains "
                + result.SelectedPermutationIDs.Count + " - only one permutation can be displayed at a time." +
                System.Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
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
        Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = UI.Colour.Stress_Gradient(colors);

        #endregion
        // get results from results class
        GsaResultsValues res = new GsaResultsValues();
        GsaResultsValues resShear = new GsaResultsValues();
        switch (_mode)
        {
          case FoldMode.Displacement:
            res = result.Element2DDisplacementValues(elementlist, lengthUnit)[0];
            break;

          case FoldMode.Force:
            res = result.Element2DForceValues(elementlist,
                Units.ForcePerLengthUnit, Units.ForceUnit)[0];
            resShear = result.Element2DShearValues(elementlist, Units.ForcePerLengthUnit)[0];
            break;
          case FoldMode.Stress:
            res = result.Element2DStressValues(elementlist,
                flayer, Units.StressUnit)[0];
            break;
        }

        // get geometry for display from results class
        ConcurrentDictionary<int, Element> elems = new ConcurrentDictionary<int, Element>(result.Model.Elements(elementlist));
        ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(result.Model.Nodes());

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = (isShear) ? resShear.xyzResults : res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = lengthUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Force)
        {
          xyzunit = Units.ForcePerLengthUnit;
          xxyyzzunit = Units.ForceUnit;
        }
        else if (_mode == FoldMode.Stress)
        {
          xyzunit = Units.StressUnit;
          xxyyzzunit = Units.StressUnit;
        }

        double dmax_x = (isShear) ? resShear.dmax_x.As(xyzunit) : res.dmax_x.As(xyzunit);
        double dmax_y = (isShear) ? resShear.dmax_y.As(xyzunit) : res.dmax_y.As(xyzunit);
        double dmax_z = res.dmax_z.As(xyzunit);
        double dmax_xyz = (_mode == FoldMode.Displacement) ? res.dmax_xyz.As(xyzunit) : 0;
        double dmin_x = (isShear) ? resShear.dmin_x.As(xyzunit) : res.dmin_x.As(xyzunit);
        double dmin_y = (isShear) ? resShear.dmin_y.As(xyzunit) : res.dmin_y.As(xyzunit);
        double dmin_z = res.dmin_z.As(xyzunit);
        double dmin_xyz = (_mode == FoldMode.Displacement) ? res.dmin_xyz.As(xyzunit) : 0;
        double dmax_xx = (isShear) ? 0 : res.dmax_xx.As(xxyyzzunit);
        double dmax_yy = (isShear) ? 0 : res.dmax_yy.As(xxyyzzunit);
        double dmax_zz = (isShear) ? 0 : res.dmax_zz.As(xxyyzzunit);
        double dmax_xxyyzz = 0;
        double dmin_xx = (isShear) ? 0 : res.dmin_xx.As(xxyyzzunit);
        double dmin_yy = (isShear) ? 0 : res.dmin_yy.As(xxyyzzunit);
        double dmin_zz = (isShear) ? 0 : res.dmin_zz.As(xxyyzzunit);
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
            else if (_mode == FoldMode.Force & !isShear)
              resType = "2D Force, Nx";
            else if (_mode == FoldMode.Force & isShear)
              resType = "2D Shear, Qx";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, xx";
            break;
          case (DisplayValue.Y):
            dmax = dmax_y;
            dmin = dmin_y;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uy";
            else if (_mode == FoldMode.Force & !isShear)
              resType = "2D Force, Ny";
            else if (_mode == FoldMode.Force & isShear)
              resType = "2D Shear, Qy";
            else if (_mode == FoldMode.Stress)
              resType = "2D Stress, yy";
            break;
          case (DisplayValue.Z):
            dmax = dmax_z;
            dmin = dmin_z;
            if (_mode == FoldMode.Displacement)
              resType = "Translation, Uz";
            else if (_mode == FoldMode.Force & !isShear)
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
            if (_mode == FoldMode.Force & !isShear)
              resType = "2D Moment, Mx";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, xy";
            break;
          case (DisplayValue.YY):
            dmax = dmax_yy;
            dmin = dmin_yy;
            if (_mode == FoldMode.Force & !isShear)
              resType = "2D Moment, My";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, yz";
            break;
          case (DisplayValue.ZZ):
            dmax = dmax_zz;
            dmin = dmin_zz;
            if (_mode == FoldMode.Force & !isShear)
              resType = "2D Moment, Mxy";
            else if (_mode == FoldMode.Stress)
              resType = "Stress, zy";
            break;
          case (DisplayValue.resXXYYZZ):
            dmax = dmax_xxyyzz;
            dmin = dmin_xxyyzz;
            break;
        }

        List<double> rounded = Util.Gsa.ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];

        #region create mesh
        ResultMesh resultMeshes = new ResultMesh(new Mesh(), new List<List<double>>());
        ConcurrentDictionary<int, Mesh> meshes = new ConcurrentDictionary<int, Mesh>();
        meshes.AsParallel().AsOrdered();
        ConcurrentDictionary<int, List<double>> values = new ConcurrentDictionary<int, List<double>>();
        values.AsParallel().AsOrdered();

        // loop through elements
        Parallel.ForEach(elems.Keys, key => //foreach (int key in elems.Keys)
        {
          Element element = elems[key];
          if (element.Topology.Count < 3) { return; }
          Mesh tempmesh = Util.Gsa.FromGSA.ConvertElement2D(element, nodes, lengthUnit);
          if (tempmesh == null) { return; }

          List<Vector3d> transformation = null;
          List<double> vals = new List<double>();
          switch (_disp)
          {
            case (DisplayValue.X):
              vals = xyzResults[key].Select(item => item.Value.X.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = vals.Select(item => new Vector3d(item * DefScale, 0, 0)).ToList();
              break;

            case (DisplayValue.Y):
              vals = xyzResults[key].Select(item => item.Value.Y.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = vals.Select(item => new Vector3d(0, item * DefScale, 0)).ToList();
              break;

            case (DisplayValue.Z):
              vals = xyzResults[key].Select(item => item.Value.Z.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = vals.Select(item => new Vector3d(0, 0, item * DefScale)).ToList();
              break;

            case (DisplayValue.resXYZ):
              vals = xyzResults[key].Select(item => item.Value.XYZ.As(xyzunit)).ToList();
              if (_mode == FoldMode.Displacement)
                transformation = xyzResults[key].Select(item => new Vector3d(
                            item.Value.X.As(xyzunit) * DefScale,
                            item.Value.Y.As(xyzunit) * DefScale,
                            item.Value.Z.As(xyzunit) * DefScale)).ToList();
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
              Length displacement = new Length(t, lengthUnit).ToUnit(lengthResultUnit);
              legendValues.Add(displacement.ToString("f" + significantDigits));
              ts.Add(new GH_UnitNumber(displacement));
            }
            else
            {
              Angle rotation = new Angle(t, AngleUnit.Radian);
              legendValues.Add(rotation.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(rotation));
            }
          }
          if (_mode == FoldMode.Force)
          {
            if ((int)_disp < 4 | isShear)
            {
              ForcePerLength forcePerLength = new ForcePerLength(t, Units.ForcePerLengthUnit);
              legendValues.Add(forcePerLength.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(forcePerLength));
            }
            else
            {
              IQuantity length = new Length(0, lengthUnit);
              string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
              legendValues.Add(new Force(t, Units.ForceUnit).ToString("s" + significantDigits) + lengthunitAbbreviation + "/" + lengthunitAbbreviation);
              Moment moment = new Moment(t, Units.MomentUnit); // this is technically moment per length
              ts.Add(new GH_UnitNumber(moment));
            }
          }
          if (_mode == FoldMode.Stress)
          {
            Pressure stress = new Pressure(t, Units.StressUnit);
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

    #region menu override
    private enum FoldMode
    {
      Displacement,
      Force,
      Stress
    }
    private FoldMode _mode = FoldMode.Displacement;

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
    private DisplayValue _disp = DisplayValue.resXYZ;

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

      slider = true;
      DefScale = 100;

      ReDrawComponent();

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    private void Mode2Clicked()
    {
      if (_mode == FoldMode.Force)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.Force;

      slider = false;
      DefScale = 0;
      spacerDescriptions[2] = "Deform Shape";

      ReDrawComponent();

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode4Clicked()
    {
      if (_mode == FoldMode.Stress)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.Stress;

      slider = false;
      DefScale = 0;


      ReDrawComponent();

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendItem(menu, "Show Legend", ShowLegend, true, showLegend);
    }
    bool showLegend = true;
    private void ShowLegend(object sender, EventArgs e)
    {
      showLegend = !showLegend;
      this.ExpirePreview(true);
    }
    #endregion
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetInt32("Display", (int)_disp);
      writer.SetInt32("flayer", flayer);
      writer.SetBoolean("slider", slider);
      writer.SetInt32("noDec", noDigits);
      writer.SetDouble("valMax", MaxValue);
      writer.SetDouble("valMin", MinValue);
      writer.SetDouble("val", DefScale);
      writer.SetBoolean("legend", showLegend);
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      flayer = reader.GetInt32("flayer");

      slider = reader.GetBoolean("slider");
      noDigits = reader.GetInt32("noDec");
      MaxValue = reader.GetDouble("valMax");
      MinValue = reader.GetDouble("valMin");
      DefScale = reader.GetDouble("val");
      showLegend = reader.GetBoolean("legend");
      Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      first = false;

      UpdateUIFromSelectedItems();
      return base.Read(reader);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    #endregion
    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity length = new Length(0, lengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      if (_mode == FoldMode.Displacement)
      {
        if ((int)_disp < 4)
          Params.Output[2].Name = "Values [" + lengthunitAbbreviation + "]";
        else
          Params.Output[2].Name = "Values [rad]";
      }

      if (_mode == FoldMode.Force)
      {
        if ((int)_disp < 4 | isShear)
          Params.Output[2].Name = "Legend Values [" + Units.ForcePerLengthUnit + "/" + lengthunitAbbreviation + "]";
        else
          Params.Output[2].Name = "Legend Values [" + Units.ForceUnit + "·" + lengthunitAbbreviation + "/" + lengthunitAbbreviation + "]";
      }

      if (_mode == FoldMode.Stress)
      {
        Params.Output[2].Name = "Legend Values [" + Units.StressUnit + "]";
      }
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
      if (legendValues != null & showLegend)
      {
        args.Display.DrawBitmap(new DisplayBitmap(legend), args.Viewport.Bounds.Right - 110, 20);
        for (int i = 0; i < legendValues.Count; i++)
          args.Display.Draw2dText(legendValues[i], Color.Black, new Point2d(args.Viewport.Bounds.Right - 85, legendValuesPosY[i]), false);
        args.Display.Draw2dText(resType, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 7), false);
        args.Display.Draw2dText(_case, Color.Black, new Point2d(args.Viewport.Bounds.Right - 110, 145), false);
      }
    }
    #endregion
  }
}
