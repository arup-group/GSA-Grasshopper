using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
using System.Collections.ObjectModel;
using Grasshopper.Kernel.Parameters;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to display GSA node result contours
  /// </summary>
  public class ReactionForceDiagrams : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("5bc139e5-614b-4f2d-887c-a980f1cbb32c");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    //protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Result0D;

    public ReactionForceDiagrams() : base("Reaction Force Diagrams",
      "ReactionForce",
      "Diplays GSA Node Reaction Force Results as Vector Diagrams",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + Environment.NewLine +
          "Node list should take the form:" + Environment.NewLine +
          " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Scalar", "x:X", "Scale the result display size", GH_ParamAccess.item, 10);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Result Point", "P", "Contoured Points with result values", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();
      this._case = "";

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
          if (result.Type == GsaResult.CaseType.Combination && result.SelectedPermutationIDs.Count > 1)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Combination Case " + result.CaseID + " contains "
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
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
          return;
        }

        // Get node filter list
        string nodeList = "All";
        GH_String gh_noList = new GH_String();
        if (DA.GetData(1, ref gh_noList))
        {
          if (GH_Convert.ToString(gh_noList, out string tempnodeList, GH_Conversion.Both))
            nodeList = tempnodeList;
        }

        if (nodeList.ToLower() == "all" || nodeList == "")
          nodeList = "All";

        // Get scalar 
        GH_Number gh_Scale = new GH_Number();
        DA.GetData(2, ref gh_Scale);
        double scale = 1;
        GH_Convert.ToDouble(gh_Scale, out scale, GH_Conversion.Both);
        #endregion

        // get stuff for drawing
        Tuple<List<GsaResultsValues>, List<int>> resultgetter = result.NodeReactionForceValues(nodeList, this.ForceUnit, this.MomentUnit);
        GsaResultsValues res = resultgetter.Item1[0];
        nodeList = string.Join(" ", resultgetter.Item2);

        // get geometry for display from results class
        ReadOnlyDictionary<int, Node> nodes = result.Model.Model.Nodes(nodeList);

        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = res.xyzResults;
        ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = res.xxyyzzResults;

        Enum xyzunit = this.ForceUnit;
        Enum xxyyzzunit = this.MomentUnit;

        #region Result point values
        // ### Coloured Result Points ###

        // round max and min to reasonable numbers
        double dmax = 0;
        double dmin = 0;
        switch (_disp)
        {
          case (DisplayValue.X):
            dmax = res.dmax_x.As(xyzunit);
            dmin = res.dmin_x.As(xyzunit);
            break;
          case (DisplayValue.Y):
            dmax = res.dmax_y.As(xyzunit);
            dmin = res.dmin_y.As(xyzunit);
            break;
          case (DisplayValue.Z):
            dmax = res.dmax_z.As(xyzunit);
            dmin = res.dmin_z.As(xyzunit);
            break;
          case (DisplayValue.resXYZ):
            dmax = res.dmax_xyz.As(xyzunit);
            dmin = res.dmin_xyz.As(xyzunit);
            break;
          case (DisplayValue.XX):
            dmax = res.dmax_xx.As(xxyyzzunit);
            dmin = res.dmin_xx.As(xxyyzzunit);
            break;
          case (DisplayValue.YY):
            dmax = res.dmax_yy.As(xxyyzzunit);
            dmin = res.dmin_yy.As(xxyyzzunit);
            break;
          case (DisplayValue.ZZ):
            dmax = res.dmax_zz.As(xxyyzzunit);
            dmin = res.dmin_zz.As(xxyyzzunit);
            break;
          case (DisplayValue.resXXYYZZ):
            dmax = res.dmax_xxyyzz.As(xxyyzzunit);
            dmin = res.dmin_xxyyzz.As(xxyyzzunit);
            break;
        }
        
        List<double> rounded = ResultHelper.SmartRounder(dmax, dmin);
        dmax = rounded[0];
        dmin = rounded[1];
        int significantDigits = (int)rounded[2];

        // Loop through nodes and set result colour into ResultPoint format
        ConcurrentDictionary<int, PointResultGoo> pts = new ConcurrentDictionary<int, PointResultGoo>();

        LengthUnit lengthUnit = result.Model.ModelUnit;
        this.undefinedModelLengthUnit = false;
        if (lengthUnit == LengthUnit.Undefined)
        {
          lengthUnit = this.LengthUnit;
          this.undefinedModelLengthUnit = true;
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in " + lengthUnit.ToString() + ". This can be changed by right-clicking the component -> 'Select Units'");
        }

        // Get nodes for point location and restraint check in case of reaction force
        ConcurrentDictionary<int, GsaNodeGoo> gsanodes = Helpers.Import.Nodes.GetNodeDictionary(nodes, lengthUnit);

        Parallel.ForEach(gsanodes, node =>
        {
          if (node.Value.Value != null)
          {
            int nodeID = node.Value.Value.Id;
            if (xyzResults.ContainsKey(nodeID))
            {
              if (!(dmin == 0 & dmax == 0))
              {
                // create vector starting point
                Point3d def = new Point3d(node.Value.Value.Point);

                // create Vector
                Vector3d vec = new Vector3d();

                IQuantity t = null;

                // pick the right value to display
                switch (_disp)
                {
                  case (DisplayValue.X):
                    vec = new Vector3d(xyzResults[nodeID][0].X.As(this.ForceUnit) * scale, 0, 0);
                    t = xyzResults[nodeID][0].X.ToUnit(this.ForceUnit);
                    break;
                  case (DisplayValue.Y):
                    vec = new Vector3d(0, xyzResults[nodeID][0].Y.As(this.ForceUnit) * scale, 0);
                    t = xyzResults[nodeID][0].Y.ToUnit(this.ForceUnit);
                    break;
                  case (DisplayValue.Z):
                    vec = new Vector3d(0, 0, xyzResults[nodeID][0].Z.As(this.ForceUnit) * scale);
                    t = xyzResults[nodeID][0].Z.ToUnit(this.ForceUnit);
                    break;
                  case (DisplayValue.resXYZ):
                    vec = new Vector3d(
                      xyzResults[nodeID][0].X.As(this.ForceUnit) * scale, 
                      xyzResults[nodeID][0].Y.As(this.ForceUnit) * scale, 
                      xyzResults[nodeID][0].Z.As(this.ForceUnit) * scale);
                    t = xyzResults[nodeID][0].XYZ.ToUnit(this.ForceUnit);
                    break;
                  case (DisplayValue.XX):
                    vec = new Vector3d(xxyyzzResults[nodeID][0].X.As(this.MomentUnit) * scale, 0, 0);
                    t = xxyyzzResults[nodeID][0].X.ToUnit(this.MomentUnit);
                    break;
                  case (DisplayValue.YY):
                    vec = new Vector3d(0, xxyyzzResults[nodeID][0].X.As(this.MomentUnit) * scale, 0);
                    t = xxyyzzResults[nodeID][0].Y.ToUnit(this.MomentUnit);
                    break;
                  case (DisplayValue.ZZ):
                    vec = new Vector3d(0, 0, xxyyzzResults[nodeID][0].X.As(this.MomentUnit) * scale);
                    t = xxyyzzResults[nodeID][0].Z.ToUnit(this.MomentUnit);
                    break;
                  case (DisplayValue.resXXYYZZ):
                    vec = new Vector3d(
                      xxyyzzResults[nodeID][0].X.As(this.MomentUnit) * scale,
                      xxyyzzResults[nodeID][0].Y.As(this.MomentUnit) * scale,
                      xxyyzzResults[nodeID][0].Z.As(this.MomentUnit) * scale);
                    t = xxyyzzResults[nodeID][0].XYZ.ToUnit(this.MomentUnit);
                    break;
                }
                
                // create custom vector display object from
                // pt
                // vector
                // t (for displaying the value as text)
              }
            }
          }
        });
        #endregion

        // set outputs
        DA.SetDataList(0, pts.OrderBy(x => x.Key).Select(y => y.Value).ToList());

        Helpers.PostHog.Result(result.Type, 0, GsaResultsValues.ResultType.Force, this._disp.ToString());
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

    readonly List<string> _reaction = new List<string>(new string[]
    {
      "Reaction Fx",
      "Reaction Fy",
      "Reaction Fz",
      "Resolved |F|",
      "Reaction Mxx",
      "Reaction Myy",
      "Reaction Mzz",
      "Resolved |M|",
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
    ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    MomentUnit MomentUnit = DefaultUnits.MomentUnit;
    DisplayValue _disp = DisplayValue.resXYZ;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Component"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // component
      this.DropDownItems.Add(this._reaction);
      this.SelectedItems.Add(this.DropDownItems[0][3]);

      this.IsInitialised = true;
    }
    public override void CreateAttributes()
    {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new OasysGH.UI.DropDownComponentAttributes(this, SetSelected, this.DropDownItems, this.SelectedItems, this.SpacerDescriptions);
    }

    public override void SetSelected(int i, int j)
    {
      this._disp = (DisplayValue)j;
      this.SelectedItems[i] = this.DropDownItems[i][j];
      base.UpdateUI();
    }
    #endregion

    #region menu override
    protected override void BeforeSolveInstance()
    {
      if ((int)this._disp < 4)
        this.Message = Force.GetAbbreviation(this.ForceUnit);
      else
        this.Message = Moment.GetAbbreviation(this.MomentUnit);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

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
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { modelUnitsMenu, forceUnitsMenu, momentUnitsMenu });
      }
      else
      {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] { forceUnitsMenu, momentUnitsMenu });
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
    private void UpdateForce(string unit)
    {
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    private void UpdateMoment(string unit)
    {
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetInt32("Display", (int)_disp);
      writer.SetString("model", Length.GetAbbreviation(this.LengthUnit));
      writer.SetString("force", Force.GetAbbreviation(this.ForceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(this.MomentUnit));
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this._disp = (DisplayValue)reader.GetInt32("Display");
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      this.LengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      return base.Read(reader);
    }
    #endregion
  }
}
