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
using GsaGH.Util.Gsa;
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
  /// Component to get Element1D results
  /// </summary>
  public class Elem1dContourResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("ce7a8f84-4c72-4fd4-a207-485e8bf7ac38");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result1D;

    public Elem1dContourResults() : base("1D Contour Results",
      "ContourElem1d",
      "Displays GSA 1D Element Results as Contour",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
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
            res = result.Element1DDisplacementValues(elementlist, positionsCount, this.LengthResultUnit)[0];
            break;

          case FoldMode.Force:
            res = result.Element1DForceValues(elementlist, positionsCount,
                this.ForceUnit, this.MomentUnit)[0];
            break;
          case FoldMode.StrainEnergy:
            if (_disp == DisplayValue.X)
              res = result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, EnergyResultUnit)[0];
            else
              res = result.Element1DStrainEnergyDensityValues(elementlist, EnergyResultUnit)[0];
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

        ConcurrentDictionary<int, Element> elems = new ConcurrentDictionary<int, Element>(result.Model.Model.Elements(elementlist));
        ConcurrentDictionary<int, Node> nodes = new ConcurrentDictionary<int, Node>(result.Model.Model.Nodes());

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = this.LengthResultUnit;
        Enum xxyyzzunit = AngleUnit.Radian;
        if (_mode == FoldMode.Force)
        {
          xyzunit = this.ForceUnit;
          xxyyzzunit = this.MomentUnit;
        }
        else if (_mode == FoldMode.StrainEnergy)
          xyzunit = DefaultUnits.EnergyUnit;

        if (res.dmax_x == null)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Result does not contain any values for 1D Elements");
          return;
        }

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
        DataTree<ResultLineGoo> resultLines = new DataTree<ResultLineGoo>();
        LengthUnit lengthUnit = result.Model.ModelUnit;
        this.undefinedModelLengthUnit = false;
        if (lengthUnit == LengthUnit.Undefined)
        {
          lengthUnit = this.LengthUnit;
          this.undefinedModelLengthUnit = true;
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in " + lengthUnit.ToString() + ". This can be changed by right-clicking the component -> 'Select Units'");
        }

        Parallel.ForEach(elems, element =>
        {
          ConcurrentDictionary<int, ResultLineGoo> resLns = new ConcurrentDictionary<int, ResultLineGoo>();

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
                      t1 = xyzResults[key][i].X.As(this.LengthResultUnit);
                      t2 = xyzResults[key][i + 1].X.As(this.LengthResultUnit);
                      startTranslation.X = xyzResults[key][i].X.As(lengthUnit) * _defScale;
                      endTranslation.X = xyzResults[key][i + 1].X.As(lengthUnit) * _defScale;
                      break;
                    case (DisplayValue.Y):
                      t1 = xyzResults[key][i].Y.As(this.LengthResultUnit);
                      t2 = xyzResults[key][i + 1].Y.As(this.LengthResultUnit);
                      startTranslation.X = xyzResults[key][i].Y.As(lengthUnit) * _defScale;
                      endTranslation.X = xyzResults[key][i + 1].Y.As(lengthUnit) * _defScale;
                      break;
                    case (DisplayValue.Z):
                      t1 = xyzResults[key][i].Z.As(this.LengthResultUnit);
                      t2 = xyzResults[key][i + 1].Z.As(this.LengthResultUnit);
                      startTranslation.X = xyzResults[key][i].Z.As(lengthUnit) * _defScale;
                      endTranslation.X = xyzResults[key][i + 1].Z.As(lengthUnit) * _defScale;
                      break;
                    case (DisplayValue.resXYZ):
                      t1 = xyzResults[key][i].XYZ.As(this.LengthResultUnit);
                      t2 = xyzResults[key][i + 1].XYZ.As(this.LengthResultUnit);
                      startTranslation.X = xyzResults[key][i].X.As(lengthUnit) * _defScale;
                      startTranslation.Y = xyzResults[key][i].Y.As(lengthUnit) * _defScale;
                      startTranslation.Z = xyzResults[key][i].Z.As(lengthUnit) * _defScale;
                      endTranslation.X = xyzResults[key][i + 1].X.As(lengthUnit) * _defScale;
                      endTranslation.Y = xyzResults[key][i + 1].Y.As(lengthUnit) * _defScale;
                      endTranslation.Z = xyzResults[key][i + 1].Z.As(lengthUnit) * _defScale;
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
                      t1 = xyzResults[key][i].X.As(this.ForceUnit);
                      t2 = xyzResults[key][i + 1].X.As(this.ForceUnit);
                      break;
                    case (DisplayValue.Y):
                      t1 = xyzResults[key][i].Y.As(this.ForceUnit);
                      t2 = xyzResults[key][i + 1].Y.As(this.ForceUnit);
                      break;
                    case (DisplayValue.Z):
                      t1 = xyzResults[key][i].Z.As(this.ForceUnit);
                      t2 = xyzResults[key][i + 1].Z.As(this.ForceUnit);
                      break;
                    case (DisplayValue.resXYZ):
                      t1 = xyzResults[key][i].XYZ.As(this.ForceUnit);
                      t2 = xyzResults[key][i + 1].XYZ.As(this.ForceUnit);
                      break;
                    case (DisplayValue.XX):
                      t1 = xxyyzzResults[key][i].X.As(this.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].X.As(this.MomentUnit);
                      break;
                    case (DisplayValue.YY):
                      t1 = xxyyzzResults[key][i].Y.As(this.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].Y.As(this.MomentUnit);
                      break;
                    case (DisplayValue.ZZ):
                      t1 = xxyyzzResults[key][i].Z.As(this.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].Z.As(this.MomentUnit);
                      break;
                    case (DisplayValue.resXXYYZZ):
                      t1 = xxyyzzResults[key][i].XYZ.As(this.MomentUnit);
                      t2 = xxyyzzResults[key][i + 1].XYZ.As(this.MomentUnit);
                      break;
                  }
                  break;
                case FoldMode.StrainEnergy:
                  if (_disp == DisplayValue.X)
                  {
                    t1 = xyzResults[key][i].X.As(DefaultUnits.EnergyUnit);
                    t2 = xyzResults[key][i + 1].X.As(DefaultUnits.EnergyUnit);
                  }
                  else
                  {
                    t1 = xyzResults[key][i].X.As(DefaultUnits.EnergyUnit);
                    t2 = xyzResults[key][i].X.As(DefaultUnits.EnergyUnit);
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
                        new ResultLineGoo(segmentline, t1, t2, valcol1, valcol2, size1, size2),
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
          else if (_mode == FoldMode.Force)
          {
            if ((int)_disp < 4)
            {
              Force force = new Force(t, this.ForceUnit);
              legendValues.Add(force.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(force));
              this.Message = Force.GetAbbreviation(this.ForceUnit);
            }
            else
            {
              Moment moment = new Moment(t, this.MomentUnit);
              legendValues.Add(moment.ToString("s" + significantDigits));
              ts.Add(new GH_UnitNumber(moment));
              this.Message = Moment.GetAbbreviation(this.MomentUnit);
            }
          }
          else
          {
            Energy energy = new Energy(t, this.EnergyResultUnit);
            legendValues.Add(energy.ToString("s" + significantDigits));
            ts.Add(new GH_UnitNumber(energy));
            this.Message = Energy.GetAbbreviation(this.EnergyResultUnit);
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
    
    LengthUnit LengthResultUnit = DefaultUnits.LengthUnitResult;
    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    bool undefinedModelLengthUnit = false;
    EnergyUnit EnergyResultUnit = DefaultUnits.EnergyUnit;
    ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    MomentUnit MomentUnit = DefaultUnits.MomentUnit;
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
        if ((int)_disp < 4)
          Params.Output[2].Name = "Legend Values [" + Force.GetAbbreviation(this.ForceUnit) + "]";
        else
          Params.Output[2].Name = "Legend Values [" + Moment.GetAbbreviation(this.MomentUnit) + "]";
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
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateForce(unit); });
        toolStripMenuItem.Checked = unit == Force.GetAbbreviation(this.ForceUnit);
        toolStripMenuItem.Enabled = true;
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem momentUnitsMenu = new ToolStripMenuItem("Moment");
      momentUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateMoment(unit); });
        toolStripMenuItem.Checked = unit == Moment.GetAbbreviation(this.MomentUnit);
        toolStripMenuItem.Enabled = true;
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      ToolStripMenuItem energyUnitsMenu = new ToolStripMenuItem("Energy");
      energyUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Energy))
      {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateEnergy(unit); });
        toolStripMenuItem.Checked = unit == Energy.GetAbbreviation(this.EnergyResultUnit);
        toolStripMenuItem.Enabled = true;
        energyUnitsMenu.DropDownItems.Add(toolStripMenuItem);
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
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { modelUnitsMenu, lengthUnitsMenu, forceUnitsMenu, momentUnitsMenu, energyUnitsMenu });
      }
      else
      {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { lengthUnitsMenu, forceUnitsMenu, momentUnitsMenu, energyUnitsMenu });
      }
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void UpdateModel(string unit)
    {
      this.LengthUnit = Length.ParseUnit(unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateLength(string unit)
    {
      this.LengthResultUnit = Length.ParseUnit(unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateForce(string unit)
    {
      this.ForceUnit = Force.ParseUnit(unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateMoment(string unit)
    {
      this.MomentUnit = Moment.ParseUnit(unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateEnergy(string unit)
    {
      this.EnergyResultUnit = Energy.ParseUnit(unit);
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

      gradient.Gradient = UI.Colour.Stress_Gradient(null);
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
      writer.SetString("model", Length.GetAbbreviation(this.LengthUnit));
      writer.SetString("length", Length.GetAbbreviation(this.LengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(this.ForceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(this.MomentUnit));
      writer.SetString("energy", Energy.GetAbbreviation(this.EnergyResultUnit));
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
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      this.LengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      this.EnergyResultUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), reader.GetString("energy"));
      return base.Read(reader);
    }
    #endregion
  }
}
