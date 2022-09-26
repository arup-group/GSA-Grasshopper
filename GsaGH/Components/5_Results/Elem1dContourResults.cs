﻿using System;
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
using GsaGH.Parameters;
using GsaGH.Util.Gsa;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get Element1D results
  /// </summary>
  public class Elem1dContourResults : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("dee5c513-197e-4659-998f-09225df9beaa");
    public Elem1dContourResults()
      : base("1D Contour Results", "ContourElem1d", "Displays GSA 1D Element Results as Contour",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result1D;
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
      if (dropdownlistidd == 0) // if change is made to result type (displacement, force, or strain energy)
      {
        if (selectedidd == 0) // displacement selected
        {
          if (dropdownitems[1] != dropdowndisplacement)
          {
            dropdownitems[1] = dropdowndisplacement;

            selecteditems[0] = dropdownitems[0][0];
            selecteditems[1] = dropdownitems[1][3];

            _disp = DisplayValue.resXYZ;
            Mode1Clicked();
          }

        }
        if (selectedidd == 1) // force selected
        {
          if (dropdownitems[1] != dropdownforce)
          {
            dropdownitems[1] = dropdownforce;

            selecteditems[0] = dropdownitems[0][1];
            selecteditems[1] = dropdownitems[1][5]; // set Myy as default

            _disp = DisplayValue.YY;
            Mode2Clicked();
          }
        }
        if (selectedidd == 2) // strain energy selected
        {
          if (dropdownitems[1] != dropdownstrainenergy)
          {
            dropdownitems[1] = dropdownstrainenergy;

            selecteditems[0] = dropdownitems[0][2];
            selecteditems[1] = dropdownitems[1][1]; // set average as default

            _disp = DisplayValue.Y;
            Mode3Clicked();
          }
        }
      } // if changes to the selected result type
      else if (dropdownlistidd == 1)
      {
        bool redraw = false;

        if (selectedidd < 4)
        {
          if ((int)_disp > 3) // chekc if we are coming from other half of display modes
          {
            if (_mode == FoldMode.Displacement)
            {
              redraw = true;
              slider = true;
            }
          }
        }
        else
        {
          if ((int)_disp < 4) // chekc if we are coming from other half of display modes
          {
            if (_mode == FoldMode.Displacement)
            {
              redraw = true;
              slider = false;
            }
          }
        }
        _disp = (DisplayValue)selectedidd;

        selecteditems[1] = dropdownitems[1][selectedidd];

        if (redraw)
          ReDrawComponent();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        Params.OnParametersChanged();
        ExpireSolution(true);
      }
      else // change is made to the unit
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
    readonly List<string> dropdowntopitems = new List<string>(new string[]
    {
            "Displacement",
            "Force",
            "Strain Energy"
    });

    readonly List<string> dropdowndisplacement = new List<string>(new string[]
    {
            "Translation Ux",
            "Translation Uy",
            "Translation Uz",
            "Resolved |U|",
            "Rotation Rxx",
            "Rotation Ryy",
            "Rotation Rzz",
            "Resolved |R|"
    });

    readonly List<string> dropdownforce = new List<string>(new string[]
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
    readonly List<string> dropdownstrainenergy = new List<string>(new string[]
    {
            "Intermediate Pts",
            "Average"
    });

    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    private LengthUnit lengthResultUnit = Units.LengthUnitResult;
    private EnergyUnit energyResultUnit = Units.EnergyUnit;
    string _case = "";
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddIntegerParameter("Intermediate Points", "nP", "Number of intermediate equidistant points (default 10)", GH_ParamAccess.item, 10);
      pManager.AddColourParameter("Colour", "Co", "[Optional] List of colours to override default colours" +
          System.Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size", GH_ParamAccess.item, 10);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Line", "L", "Contoured Line segments with result values", GH_ParamAccess.tree);
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

        // Get number of divisions
        GH_Integer gh_Div = new GH_Integer();
        DA.GetData(2, ref gh_Div);
        GH_Convert.ToInt32(gh_Div, out int positionsCount, GH_Conversion.Both);
        positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

        // Get colours
        List<GH_Colour> gh_Colours = new List<GH_Colour>();
        List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
        if (DA.GetDataList(3, gh_Colours))
        {
          for (int i = 0; i < gh_Colours.Count; i++)
          {
            System.Drawing.Color color = new System.Drawing.Color();
            GH_Convert.ToColor(gh_Colours[i], out color, GH_Conversion.Both);
            colors.Add(color);
          }
        }
        Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = UI.Colour.Stress_Gradient(colors);

        // Get scalar 
        GH_Number gh_Scale = new GH_Number();
        DA.GetData(4, ref gh_Scale);
        double scale = 1;
        GH_Convert.ToDouble(gh_Scale, out scale, GH_Conversion.Both);
        #endregion

        // get results from results class
        GsaResultsValues res = new GsaResultsValues();

        switch (_mode)
        {
          case FoldMode.Displacement:
            res = result.Element1DDisplacementValues(elementlist, positionsCount, lengthUnit)[0];

            break;

          case FoldMode.Force:
            res = result.Element1DForceValues(elementlist, positionsCount,
                Units.ForceUnit, Units.MomentUnit)[0];
            break;
          case FoldMode.StrainEnergy:
            if (_disp == DisplayValue.X)
              res = result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, energyResultUnit)[0];
            else
              res = result.Element1DStrainEnergyDensityValues(elementlist, energyResultUnit)[0];
            break;
        }

        // get geometry for display from results class
        List<int> elementIDs = new List<int>();
        if (result.Type == GsaResult.ResultType.AnalysisCase)
          elementIDs = result.ACaseElement1DResults.Values.First().Select(x => x.Key).ToList();
        else
          elementIDs = result.ComboElement1DResults.Values.First().Select(x => x.Key).ToList();
        if (elementlist.ToLower() == "all")
          elementlist = String.Join(" ", elementIDs);

        ConcurrentDictionary<int, Element> elems = new ConcurrentDictionary<int, Element>(result.Model.Elements(elementlist));
        ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(result.Model.Nodes());

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = lengthUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Force)
        {
          xyzunit = Units.ForceUnit;
          xxyyzzunit = Units.MomentUnit;
        }
        else if (_mode == FoldMode.StrainEnergy)
          xyzunit = Units.EnergyUnit;

        double dmax_x = res.dmax_x.As(xyzunit);
        double dmax_y = _mode == FoldMode.StrainEnergy ? 0 : res.dmax_y.As(xyzunit);
        double dmax_z = _mode == FoldMode.StrainEnergy ? 0 : res.dmax_z.As(xyzunit);
        double dmax_xyz = _mode == FoldMode.StrainEnergy ? 0 : res.dmax_xyz.As(xyzunit);
        double dmin_x = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_x.As(xyzunit);
        double dmin_y = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_y.As(xyzunit);
        double dmin_z = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_z.As(xyzunit);
        double dmin_xyz = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_xyz.As(xyzunit);
        double dmax_xx = _mode == FoldMode.StrainEnergy ? 0 : res.dmax_xx.As(xxyyzzunit);
        double dmax_yy = _mode == FoldMode.StrainEnergy ? 0 : res.dmax_yy.As(xxyyzzunit);
        double dmax_zz = _mode == FoldMode.StrainEnergy ? 0 : res.dmax_zz.As(xxyyzzunit);
        double dmax_xxyyzz = _mode == FoldMode.StrainEnergy ? 0 : res.dmax_xxyyzz.As(xxyyzzunit);
        double dmin_xx = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_xx.As(xxyyzzunit);
        double dmin_yy = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_yy.As(xxyyzzunit);
        double dmin_zz = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_zz.As(xxyyzzunit);
        double dmin_xxyyzz = _mode == FoldMode.StrainEnergy ? 0 : res.dmin_xxyyzz.As(xxyyzzunit);

        #region Result line values
        // ### Coloured Result Lines ###

        // round max and min to reasonable numbers
        double dmax = 0;
        double dmin = 0;
        if (_mode == FoldMode.StrainEnergy)
        {
          dmax = dmax_x;
          dmin = dmin_x;
          if (_disp == DisplayValue.X)
            resType = "Strain Energy Density";
          else
          {
            positionsCount = 2;
            resType = "Average Strain E Dens.";
          }
        }
        else
        {
          switch (_disp)
          {
            case (DisplayValue.X):
              dmax = dmax_x;
              dmin = dmin_x;
              if (_mode == FoldMode.Displacement)
                resType = "Elem. Trans., Ux";
              else
                resType = "Axial Force, Fx";
              break;
            case (DisplayValue.Y):
              dmax = dmax_y;
              dmin = dmin_y;
              if (_mode == FoldMode.Displacement)
                resType = "Elem. Trans., Uy";
              else
                resType = "Shear Force, Fy";
              break;
            case (DisplayValue.Z):
              dmax = dmax_z;
              dmin = dmin_z;
              if (_mode == FoldMode.Displacement)
                resType = "Elem. Trans., Uz";
              else
                resType = "Shear Force, Fz";
              break;
            case (DisplayValue.resXYZ):
              dmax = dmax_xyz;
              dmin = dmin_xyz;
              if (_mode == FoldMode.Displacement)
                resType = "Res. Trans., |U|";
              else
                resType = "Res. Shear, |Fyz|";
              break;
            case (DisplayValue.XX):
              dmax = dmax_xx;
              dmin = dmin_xx;
              if (_mode == FoldMode.Displacement)
                resType = "Elem. Rot., Rxx";
              else
                resType = "Torsion, Mxx";
              break;
            case (DisplayValue.YY):
              dmax = dmax_yy;
              dmin = dmin_yy;
              if (_mode == FoldMode.Displacement)
                resType = "Elem. Rot., Ryy";
              else
                resType = "Moment, Myy";
              break;
            case (DisplayValue.ZZ):
              dmax = dmax_zz;
              dmin = dmin_zz;
              if (_mode == FoldMode.Displacement)
                resType = "Elem. Rot., Rzz";
              else
                resType = "Moment, Mzz";
              break;
            case (DisplayValue.resXXYYZZ):
              dmax = dmax_xxyyzz;
              dmin = dmin_xxyyzz;
              if (_mode == FoldMode.Displacement)
                resType = "Res. Rot., |U|";
              else
                resType = "Res. Moment, |Myz|";
              break;
          }
        }

        List<double> rounded = Util.Gsa.ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];

        // Loop through segmented lines and set result colour into ResultLine format
        DataTree<ResultLine> resultLines = new DataTree<ResultLine>();

        Parallel.ForEach(elems, element =>
        {
          ConcurrentDictionary<int, ResultLine> resLns = new ConcurrentDictionary<int, ResultLine>();

          // list for element geometry and info
          if (element.Value.IsDummy) { return; }
          if (element.Value.Type == ElementType.LINK) { return; }
          if (element.Value.Topology.Count > 2) { return; }
          Line ln = new Line(
                      FromGSA.Point3dFromNode(nodes[element.Value.Topology[0]], lengthUnit), // start point
                      FromGSA.Point3dFromNode(nodes[element.Value.Topology[1]], lengthUnit));// end point

          int key = element.Key;

          for (int i = 0; i < positionsCount - 1; i++)
          {
            if (!(dmin == 0 & dmax == 0))
            {
              Vector3d startTranslation = new Vector3d(0, 0, 0);
              Vector3d endTranslation = new Vector3d(0, 0, 0);

              double t1 = 0;
              double t2 = 0;

              Point3d start = new Point3d(ln.PointAt((double)i / (positionsCount - 1)));
              Point3d end = new Point3d(ln.PointAt((double)(i + 1) / (positionsCount - 1)));

              // pick the right value to display
              switch (_mode)
              {
                case FoldMode.Displacement:
                  Vector3d translation = new Vector3d(0, 0, 0);
                  // pick the right value to display
                  switch (_disp)
                  {
                    case (DisplayValue.X):
                      t1 = xyzResults[key][i].X.As(lengthUnit);
                      t2 = xyzResults[key][i + 1].X.As(lengthUnit);
                      startTranslation.X = t1 * DefScale;
                      endTranslation.X = t2 * DefScale;
                      break;
                    case (DisplayValue.Y):
                      t1 = xyzResults[key][i].Y.As(lengthUnit);
                      t2 = xyzResults[key][i + 1].Y.As(lengthUnit);
                      startTranslation.Y = t1 * DefScale;
                      endTranslation.Y = t2 * DefScale;
                      break;
                    case (DisplayValue.Z):
                      t1 = xyzResults[key][i].Z.As(lengthUnit);
                      t2 = xyzResults[key][i + 1].Z.As(lengthUnit);
                      startTranslation.Z = t1 * DefScale;
                      endTranslation.Z = t2 * DefScale;
                      break;
                    case (DisplayValue.resXYZ):
                      t1 = xyzResults[key][i].XYZ.As(lengthUnit);
                      t2 = xyzResults[key][i + 1].XYZ.As(lengthUnit);
                      startTranslation.X = t1 * DefScale;
                      endTranslation.X = t2 * DefScale;
                      startTranslation.X = xyzResults[key][i].X.As(lengthUnit) * DefScale;
                      startTranslation.Y = xyzResults[key][i].Y.As(lengthUnit) * DefScale;
                      startTranslation.Z = xyzResults[key][i].Z.As(lengthUnit) * DefScale;
                      endTranslation.X = xyzResults[key][i + 1].X.As(lengthUnit) * DefScale;
                      endTranslation.Y = xyzResults[key][i + 1].Y.As(lengthUnit) * DefScale;
                      endTranslation.Z = xyzResults[key][i + 1].Z.As(lengthUnit) * DefScale;
                      break;
                    case (DisplayValue.XX):
                      t1 = xxyyzzResults[key][i].X.As(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].X.As(AngleUnit.Radian);
                      break;
                    case (DisplayValue.YY):
                      t1 = xxyyzzResults[key][i].Y.As(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].Y.As(AngleUnit.Radian);
                      break;
                    case (DisplayValue.ZZ):
                      t1 = xxyyzzResults[key][i].Z.As(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].Z.As(AngleUnit.Radian);
                      break;
                    case (DisplayValue.resXXYYZZ):
                      t1 = xxyyzzResults[key][i].XYZ.As(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].XYZ.As(AngleUnit.Radian);
                      break;
                  }
                  start.Transform(Transform.Translation(startTranslation));
                  end.Transform(Transform.Translation(endTranslation));
                  break;
                case FoldMode.Force:
                  // pick the right value to display
                  switch (_disp)
                  {
                    case (DisplayValue.X):
                      t1 = xyzResults[key][i].X.As(Units.ForceUnit);
                      t2 = xyzResults[key][i + 1].X.As(Units.ForceUnit);
                      break;
                    case (DisplayValue.Y):
                      t1 = xyzResults[key][i].Y.As(Units.ForceUnit);
                      t2 = xyzResults[key][i + 1].Y.As(Units.ForceUnit);
                      break;
                    case (DisplayValue.Z):
                      t1 = xyzResults[key][i].Z.As(Units.ForceUnit);
                      t2 = xyzResults[key][i + 1].Z.As(Units.ForceUnit);
                      break;
                    case (DisplayValue.resXYZ):
                      t1 = xyzResults[key][i].XYZ.As(Units.ForceUnit);
                      t2 = xyzResults[key][i + 1].XYZ.As(Units.ForceUnit);
                      break;
                    case (DisplayValue.XX):
                      t1 = xxyyzzResults[key][i].X.As(Units.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].X.As(Units.MomentUnit);
                      break;
                    case (DisplayValue.YY):
                      t1 = xxyyzzResults[key][i].Y.As(Units.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].Y.As(Units.MomentUnit);
                      break;
                    case (DisplayValue.ZZ):
                      t1 = xxyyzzResults[key][i].Z.As(Units.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].Z.As(Units.MomentUnit);
                      break;
                    case (DisplayValue.resXXYYZZ):
                      t1 = xxyyzzResults[key][i].XYZ.As(Units.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].XYZ.As(Units.MomentUnit);
                      break;
                  }
                  break;
                case FoldMode.StrainEnergy:
                  if (_disp == DisplayValue.X)
                  {
                    t1 = xyzResults[key][i].X.As(Units.EnergyUnit);
                    t2 = xyzResults[key][i + 1].X.As(Units.EnergyUnit);
                  }
                  else
                  {
                    t1 = xyzResults[key][i].X.As(Units.EnergyUnit);
                    t2 = xyzResults[key][i].X.As(Units.EnergyUnit);
                  }
                  break;
              }

              Line segmentline = new Line(start, end);

              //normalised value between -1 and 1
              double tnorm1 = 2 * (t1 - dmin) / (dmax - dmin) - 1;
              double tnorm2 = 2 * (t2 - dmin) / (dmax - dmin) - 1;

              // get colour for that normalised value
              Color valcol1 = double.IsNaN(tnorm1) ? Color.Black : gH_Gradient.ColourAt(tnorm1);
              Color valcol2 = double.IsNaN(tnorm2) ? Color.Black : gH_Gradient.ColourAt(tnorm2);

              // set the size of the line ends for ResultLine class. Size is calculated from 0-base, so not a normalised value between extremes
              float size1 = (t1 >= 0 && dmax != 0) ?
                          Math.Max(2, (float)(t1 / dmax * scale)) :
                          Math.Max(2, (float)(Math.Abs(t1) / Math.Abs(dmin) * scale));
              if (double.IsNaN(size1))
                size1 = 1;
              float size2 = (t2 >= 0 && dmax != 0) ?
                          Math.Max(2, (float)(t2 / dmax * scale)) :
                          Math.Max(2, (float)(Math.Abs(t2) / Math.Abs(dmin) * scale));
              if (double.IsNaN(size2))
                size2 = 1;

              // add our special resultline to the list of lines
              lock (resultLines)
              {
                resultLines.Add(
                        new ResultLine(segmentline, t1, t2, valcol1, valcol2, size1, size2),
                        new GH_Path(key));
              }
            }
          }
        });

        #endregion

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
          else if (_mode == FoldMode.Force)
          {
            if ((int)_disp < 4)
            {
              Force force = new Force(t, Units.ForceUnit);
              legendValues.Add(force.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(force));
            }
            else
            {
              Moment moment = new Moment(t, Units.MomentUnit);
              legendValues.Add(t.ToString("F" + significantDigits) + " " + Moment.GetAbbreviation(Units.MomentUnit));
              ts.Add(new GH_UnitNumber(moment));
            }
          }
          else
          {
            Energy energy = new Energy(t, Units.EnergyUnit);
            legendValues.Add(energy.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(energy));
          }

          if (Math.Abs(t) > 1)
            legendValues[i] = legendValues[i].Replace(",", string.Empty); // remove thousand separator
          legendValuesPosY.Add(legend.Height - starty + gripheight / 2 - 2);
        }
        #endregion

        // set outputs
        DA.SetDataTree(0, resultLines);
        DA.SetDataList(1, cs);
        DA.SetDataList(2, ts);
      }
    }



    #region menu override
    private enum FoldMode
    {
      Displacement,
      Force,
      StrainEnergy
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
      System.Drawing.PointF pivot = new System.Drawing.PointF(this.Attributes.Pivot.X, this.Attributes.Pivot.Y);
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

      ReDrawComponent();

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    private void Mode3Clicked()
    {
      if (_mode == FoldMode.StrainEnergy)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.StrainEnergy;

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
      writer.SetBoolean("slider", slider);
      writer.SetBoolean("legend", showLegend);
      writer.SetInt32("noDec", noDigits);
      writer.SetDouble("valMax", MaxValue);
      writer.SetDouble("valMin", MinValue);
      writer.SetDouble("val", DefScale);
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");

      slider = reader.GetBoolean("slider");
      showLegend = reader.GetBoolean("legend");
      noDigits = reader.GetInt32("noDec");
      MaxValue = reader.GetDouble("valMax");
      MinValue = reader.GetDouble("valMin");
      DefScale = reader.GetDouble("val");
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
        if ((int)_disp < 4)
          Params.Output[2].Name = "Legend Values [" + Units.ForceUnit + "]";
        else
          Params.Output[2].Name = "Legend Values [" + Units.MomentUnit + "]";
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
