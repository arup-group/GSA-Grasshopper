using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI.Gradient;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers.GsaAPI;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;
using GsaGH.Helpers.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get Element1D results
    /// </summary>
    public class Elem1dContourResults_OBSOLETE : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("dee5c513-197e-4659-998f-09225df9beaa");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result1D;

    public Elem1dContourResults_OBSOLETE() : base("1D Contour Results",
      "ContourElem1d",
      "Displays GSA 1D Element Results as Contour",
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
      pManager.AddIntegerParameter("Intermediate Points", "nP", "Number of intermediate equidistant points (default 10)", GH_ParamAccess.item, 10);
      pManager.AddColourParameter("Colour", "Co", "[Optional] List of colours to override default colours" +
          Environment.NewLine + "A new gradient will be created from the input list of colours", GH_ParamAccess.list);
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size", GH_ParamAccess.item, 10);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, LengthResultUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Line", "L", "Contoured Line segments with result values", GH_ParamAccess.tree);
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
        if (gh_typ.Value is GsaResultGoo)
        {
          result = ((GsaResultGoo)gh_typ.Value).Value;
          if (result.Type == GsaResult.CaseType.Combination && result.SelectedPermutationIds.Count > 1)
          {
            this.AddRuntimeWarning("Combination case contains "
                + result.SelectedPermutationIds.Count + " - only one permutation can be displayed at a time." +
                Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
          }
          if (result.Type == GsaResult.CaseType.Combination)
            _case = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
          if (result.Type == GsaResult.CaseType.AnalysisCase)
            _case = "Case A" + result.CaseId + Environment.NewLine + result.CaseName;
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
        Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = Helpers.Graphics.Colours.Stress_Gradient(colors);

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
            res = result.Element1DDisplacementValues(elementlist, positionsCount, LengthUnit)[0];

            break;

          case FoldMode.Force:
            res = result.Element1DForceValues(elementlist, positionsCount,
                DefaultUnits.ForceUnit, DefaultUnits.MomentUnit)[0];
            break;
          case FoldMode.StrainEnergy:
            if (_disp == DisplayValue.X)
              res = result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, EnergyResultUnit)[0];
            else
              res = result.Element1DAverageStrainEnergyDensityValues(elementlist, EnergyResultUnit)[0];
            break;
        }

        // get geometry for display from results class
        List<int> elementIDs = new List<int>();
        if (result.Type == GsaResult.CaseType.AnalysisCase)
          elementIDs = result.ACaseElement1DResults.Values.First().Select(x => x.Key).ToList();
        else
          elementIDs = result.ComboElement1DResults.Values.First().Select(x => x.Key).ToList();
        if (elementlist.ToLower() == "all")
          elementlist = String.Join(" ", elementIDs);

        ConcurrentDictionary<int, Element> elems = new ConcurrentDictionary<int, Element>(result.Model.Model.Elements(elementlist));
        ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(result.Model.Model.Nodes());

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = LengthUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Force)
        {
          xyzunit = DefaultUnits.ForceUnit;
          xxyyzzunit = DefaultUnits.MomentUnit;
        }
        else if (_mode == FoldMode.StrainEnergy)
          xyzunit = DefaultUnits.EnergyUnit;

        double dmax_x = res.DmaxX.As(xyzunit);
        double dmax_y = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxY.As(xyzunit);
        double dmax_z = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxZ.As(xyzunit);
        double dmax_xyz = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxXyz.As(xyzunit);
        double dmin_x = _mode == FoldMode.StrainEnergy ? 0 : res.DminX.As(xyzunit);
        double dmin_y = _mode == FoldMode.StrainEnergy ? 0 : res.DminY.As(xyzunit);
        double dmin_z = _mode == FoldMode.StrainEnergy ? 0 : res.DminZ.As(xyzunit);
        double dmin_xyz = _mode == FoldMode.StrainEnergy ? 0 : res.DminXyz.As(xyzunit);
        double dmax_xx = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxXx.As(xxyyzzunit);
        double dmax_yy = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxYy.As(xxyyzzunit);
        double dmax_zz = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxZz.As(xxyyzzunit);
        double dmax_xxyyzz = _mode == FoldMode.StrainEnergy ? 0 : res.DmaxXxyyzz.As(xxyyzzunit);
        double dmin_xx = _mode == FoldMode.StrainEnergy ? 0 : res.DminXx.As(xxyyzzunit);
        double dmin_yy = _mode == FoldMode.StrainEnergy ? 0 : res.DminYy.As(xxyyzzunit);
        double dmin_zz = _mode == FoldMode.StrainEnergy ? 0 : res.DminZz.As(xxyyzzunit);
        double dmin_xxyyzz = _mode == FoldMode.StrainEnergy ? 0 : res.DminXxyyzz.As(xxyyzzunit);

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

        List<double> rounded = Helpers.GsaAPI.ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];

        // Loop through segmented lines and set result colour into ResultLine format
        DataTree<LineResultGoo> resultLines = new DataTree<LineResultGoo>();

        Parallel.ForEach(elems, element =>
        {
          ConcurrentDictionary<int, LineResultGoo> resLns = new ConcurrentDictionary<int, LineResultGoo>();

          // list for element geometry and info
          if (element.Value.IsDummy) { return; }
          if (element.Value.Type == ElementType.LINK) { return; }
          if (element.Value.Topology.Count > 2) { return; }
          Line ln = new Line(
                      Helpers.Import.Nodes.Point3dFromNode(nodes[element.Value.Topology[0]], LengthUnit), // start point
                      Helpers.Import.Nodes.Point3dFromNode(nodes[element.Value.Topology[1]], LengthUnit));// end point

          int key = element.Key;

          for (int i = 0; i < positionsCount - 1; i++)
          {
            if (!(dmin == 0 & dmax == 0))
            {
              Vector3d startTranslation = new Vector3d(0, 0, 0);
              Vector3d endTranslation = new Vector3d(0, 0, 0);

              IQuantity t1 = null;
              IQuantity t2 = null;

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
                      t1 = xyzResults[key][i].X.ToUnit(LengthUnit);
                      t2 = xyzResults[key][i + 1].X.ToUnit(LengthUnit);
                      startTranslation.X = t1.Value * _defScale;
                      endTranslation.X = t2.Value * _defScale;
                      break;
                    case (DisplayValue.Y):
                      t1 = xyzResults[key][i].Y.ToUnit(LengthUnit);
                      t2 = xyzResults[key][i + 1].Y.ToUnit(LengthUnit);
                      startTranslation.Y = t1.Value * _defScale;
                      endTranslation.Y = t2.Value * _defScale;
                      break;
                    case (DisplayValue.Z):
                      t1 = xyzResults[key][i].Z.ToUnit(LengthUnit);
                      t2 = xyzResults[key][i + 1].Z.ToUnit(LengthUnit);
                      startTranslation.Z = t1.Value * _defScale;
                      endTranslation.Z = t2.Value * _defScale;
                      break;
                    case (DisplayValue.resXYZ):
                      t1 = xyzResults[key][i].XYZ.ToUnit(LengthUnit);
                      t2 = xyzResults[key][i + 1].XYZ.ToUnit(LengthUnit);
                      startTranslation.X = xyzResults[key][i].X.As(LengthUnit) * _defScale;
                      startTranslation.Y = xyzResults[key][i].Y.As(LengthUnit) * _defScale;
                      startTranslation.Z = xyzResults[key][i].Z.As(LengthUnit) * _defScale;
                      endTranslation.X = xyzResults[key][i + 1].X.As(LengthUnit) * _defScale;
                      endTranslation.Y = xyzResults[key][i + 1].Y.As(LengthUnit) * _defScale;
                      endTranslation.Z = xyzResults[key][i + 1].Z.As(LengthUnit) * _defScale;
                      break;
                    case (DisplayValue.XX):
                      t1 = xxyyzzResults[key][i].X.ToUnit(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].X.ToUnit(AngleUnit.Radian);
                      break;
                    case (DisplayValue.YY):
                      t1 = xxyyzzResults[key][i].Y.ToUnit(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].Y.ToUnit(AngleUnit.Radian);
                      break;
                    case (DisplayValue.ZZ):
                      t1 = xxyyzzResults[key][i].Z.ToUnit(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].Z.ToUnit(AngleUnit.Radian);
                      break;
                    case (DisplayValue.resXXYYZZ):
                      t1 = xxyyzzResults[key][i].XYZ.ToUnit(AngleUnit.Radian);
                      t2 = xxyyzzResults[key][i + 1].XYZ.ToUnit(AngleUnit.Radian);
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
                    case (DisplayValue.resXYZ):
                      t1 = xyzResults[key][i].XYZ.ToUnit(DefaultUnits.ForceUnit);
                      t2 = xyzResults[key][i + 1].XYZ.ToUnit(DefaultUnits.ForceUnit);
                      break;
                    case (DisplayValue.XX):
                      t1 = xxyyzzResults[key][i].X.ToUnit(DefaultUnits.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].X.ToUnit(DefaultUnits.MomentUnit);
                      break;
                    case (DisplayValue.YY):
                      t1 = xxyyzzResults[key][i].Y.ToUnit(DefaultUnits.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].Y.ToUnit(DefaultUnits.MomentUnit);
                      break;
                    case (DisplayValue.ZZ):
                      t1 = xxyyzzResults[key][i].Z.ToUnit(DefaultUnits.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].Z.ToUnit(DefaultUnits.MomentUnit);
                      break;
                    case (DisplayValue.resXXYYZZ):
                      t1 = xxyyzzResults[key][i].XYZ.ToUnit(DefaultUnits.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].XYZ.ToUnit(DefaultUnits.MomentUnit);
                      break;
                  }
                  break;
                case FoldMode.StrainEnergy:
                  if (_disp == DisplayValue.X)
                  {
                    t1 = xyzResults[key][i].X.ToUnit(DefaultUnits.EnergyUnit);
                    t2 = xyzResults[key][i + 1].X.ToUnit(DefaultUnits.EnergyUnit);
                  }
                  else
                  {
                    t1 = xyzResults[key][i].X.ToUnit(DefaultUnits.EnergyUnit);
                    t2 = xyzResults[key][i].X.ToUnit(DefaultUnits.EnergyUnit);
                  }
                  break;
              }

              Line segmentline = new Line(start, end);

              //normalised value between -1 and 1
              double tnorm1 = 2 * (t1.Value - dmin) / (dmax - dmin) - 1;
              double tnorm2 = 2 * (t2.Value - dmin) / (dmax - dmin) - 1;

              // get colour for that normalised value
              Color valcol1 = double.IsNaN(tnorm1) ? Color.Black : gH_Gradient.ColourAt(tnorm1);
              Color valcol2 = double.IsNaN(tnorm2) ? Color.Black : gH_Gradient.ColourAt(tnorm2);

              // set the size of the line ends for ResultLine class. Size is calculated from 0-base, so not a normalised value between extremes
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

              // add our special resultline to the list of lines
              lock (resultLines)
              {
                resultLines.Add(
                        new LineResultGoo(segmentline, t1, t2, valcol1, valcol2, size1, size2),
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
              Length displacement = new Length(t, LengthUnit).ToUnit(LengthResultUnit);
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
              Force force = new Force(t, DefaultUnits.ForceUnit);
              legendValues.Add(force.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(force));
            }
            else
            {
              Moment moment = new Moment(t, DefaultUnits.MomentUnit);
              legendValues.Add(t.ToString("F" + significantDigits) + " " + Moment.GetAbbreviation(DefaultUnits.MomentUnit));
              ts.Add(new GH_UnitNumber(moment));
            }
          }
          else
          {
            Energy energy = new Energy(t, DefaultUnits.EnergyUnit);
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


    #region Custom UI
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
    private enum FoldMode
    {
      Displacement,
      Force,
      StrainEnergy
    }
    readonly List<string> _type = new List<string>(new string[]
    {
      "Displacement",
      "Force",
      "Strain Energy"
    });

    readonly List<string> _displacement = new List<string>(new string[]
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

    readonly List<string> _force = new List<string>(new string[]
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
    readonly List<string> _strainenergy = new List<string>(new string[]
    {
      "Intermediate Pts",
      "Average"
    });

    double _minValue = 0;
    double _maxValue = 1000;
    double _defScale = 250;
    int _noDigits = 0;
    bool _slider = true;
    string _case = "";
    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    LengthUnit LengthResultUnit = DefaultUnits.LengthUnitResult;
    EnergyUnit EnergyResultUnit = DefaultUnits.EnergyUnit;
    FoldMode _mode = FoldMode.Displacement;
    DisplayValue _disp = DisplayValue.resXYZ;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Result Type", "Component", "Geometry Unit", "Deform Shape"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(this._type);
      this.SelectedItems.Add(this.DropDownItems[0][0]);

      // component
      this.DropDownItems.Add(this._displacement);
      this.SelectedItems.Add(this.DropDownItems[1][3]);

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void CreateAttributes()
    {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new OasysGH.UI.DropDownSliderComponentAttributes(this, SetSelected, this.DropDownItems, this.SelectedItems, this._slider, SetVal, SetMaxMin, this._defScale, this._maxValue, this._minValue, this._noDigits, this.SpacerDescriptions);
    }

    public override void SetSelected(int dropdownlistidd, int selectedidd)
    {
      if (dropdownlistidd == 0) // if change is made to result type (displacement, force, or strain energy)
      {
        if (selectedidd == 0) // displacement selected
        {
          if (DropDownItems[1] != _displacement)
          {
            DropDownItems[1] = _displacement;

            SelectedItems[0] = DropDownItems[0][0];
            SelectedItems[1] = DropDownItems[1][3];

            _disp = DisplayValue.resXYZ;
            Mode1Clicked();
          }

        }
        if (selectedidd == 1) // force selected
        {
          if (DropDownItems[1] != _force)
          {
            DropDownItems[1] = _force;

            SelectedItems[0] = DropDownItems[0][1];
            SelectedItems[1] = DropDownItems[1][5]; // set Myy as default

            _disp = DisplayValue.YY;
            Mode2Clicked();
          }
        }
        if (selectedidd == 2) // strain energy selected
        {
          if (DropDownItems[1] != _strainenergy)
          {
            DropDownItems[1] = _strainenergy;

            SelectedItems[0] = DropDownItems[0][2];
            SelectedItems[1] = DropDownItems[1][1]; // set average as default

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
              _slider = true;
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
              _slider = false;
            }
          }
        }
        _disp = (DisplayValue)selectedidd;

        SelectedItems[1] = DropDownItems[1][selectedidd];

        if (redraw)
          ReDrawComponent();
      }
      else // change is made to the unit
      {
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[2]);
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

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      if (_mode == FoldMode.Displacement)
      {
        if ((int)_disp < 4)
          Params.Output[2].Name = "Values [" + Length.GetAbbreviation(this.LengthUnit) + "]";
        else
          Params.Output[2].Name = "Values [rad]";
      }

      if (_mode == FoldMode.Force)
      {
        if ((int)_disp < 4)
          Params.Output[2].Name = "Legend Values [" + Force.GetAbbreviation(DefaultUnits.ForceUnit) + "]";
        else
          Params.Output[2].Name = "Legend Values [" + Moment.GetAbbreviation(DefaultUnits.MomentUnit) + "]";
      }
    }
    #endregion

    #region menu override
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

      ReDrawComponent();
    }
    private void Mode3Clicked()
    {
      if (_mode == FoldMode.StrainEnergy)
        return;

      RecordUndoEvent(_mode.ToString() + " Parameters");
      _mode = FoldMode.StrainEnergy;

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
      Menu_AppendSeparator(menu);
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

      gradient.Attributes.Pivot = new PointF(this.Attributes.Bounds.X - gradient.Attributes.Bounds.Width - 50, this.Params.Input[3].Attributes.Bounds.Y - gradient.Attributes.Bounds.Height / 4 - 6);

      Grasshopper.Instances.ActiveCanvas.Document.AddObject(gradient, false);
      this.Params.Input[3].RemoveAllSources();
      this.Params.Input[3].AddSource(gradient.Params.Output[0]);

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
      writer.SetBoolean("legend", _showLegend);
      writer.SetInt32("noDec", _noDigits);
      writer.SetDouble("valMax", _maxValue);
      writer.SetDouble("valMin", _minValue);
      writer.SetDouble("val", _defScale);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");
      _disp = (DisplayValue)reader.GetInt32("Display");
      _slider = reader.GetBoolean("slider");
      _showLegend = reader.GetBoolean("legend");
      _noDigits = reader.GetInt32("noDec");
      _maxValue = reader.GetDouble("valMax");
      _minValue = reader.GetDouble("valMin");
      _defScale = reader.GetDouble("val");
      return base.Read(reader);
    }
    #endregion
  }
}
