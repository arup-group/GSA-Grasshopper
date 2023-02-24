using Grasshopper.Kernel;
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
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GsaGH.Components
{
  class ReactionForceVector
  {
    public readonly int NodeId;
    public readonly Point3d StartingPoint;
    public readonly Vector3d ForceVector;
    public readonly IQuantity ForceValue;

    public ReactionForceVector(int nodeId, Point3d startingPoint, Vector3d forceVector, IQuantity forceValue)
    {
      this.NodeId = nodeId;
      this.StartingPoint = startingPoint;
      this.ForceVector = forceVector;
      this.ForceValue = forceValue;
    }
  }

  /// <summary>
  /// Component to display GSA node result contours
  /// </summary>
  public class ReactionForceDiagrams : GH_OasysDropDownComponent
  {
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
    DisplayValue _disp = DisplayValue.ResXYZ;
    List<ReactionForceVector> _reactionForceVectors = new List<ReactionForceVector>();
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
      pManager.AddPointParameter("Anchor Point", "A", "Support Node Location", GH_ParamAccess.list);
      pManager.AddVectorParameter("Vector", "V", "Reaction Force Vector", GH_ParamAccess.list);
      pManager.AddGenericParameter("Value", "Val", "Reaction Force Value", GH_ParamAccess.list);
      pManager.HideParameter(0);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess dataAccess)
    {
      // Result to work on
      var gsaResult = new GsaResult();

      // Get Model
      var ghObject = new GH_ObjectWrapper();
      if (!dataAccess.GetData(0, ref ghObject) || !IsGhObjectValid(ghObject)) return;

      _reactionForceVectors.Clear();

      #region get input values
      gsaResult = (ghObject.Value as GsaResultGoo).Value;
      this._case = GetCase(gsaResult);
      string filteredNodes = GetNodeFilters(dataAccess);
      double scale = GetScalarValue(dataAccess);
      #endregion

      // get stuff for drawing
      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues = gsaResult.NodeReactionForceValues(filteredNodes, this.ForceUnit, this.MomentUnit);
      GsaResultsValues forceValues = reactionForceValues.Item1[0];
      filteredNodes = string.Join(" ", reactionForceValues.Item2);

      // get geometry for display from results class
      var xyzResults = forceValues.xyzResults;
      var xxyyzzResults = forceValues.xxyyzzResults;

      #region Result point values

      LengthUnit lengthUnit = GetLenghtUnit(gsaResult);

      // Get nodes for point location and restraint check in case of reaction force
      ReadOnlyDictionary<int, Node> gsaFilteredNodes = gsaResult.Model.Model.Nodes(filteredNodes);
      ConcurrentDictionary<int, GsaNodeGoo> nodes = Helpers.Import.Nodes.GetNodeDictionary(gsaFilteredNodes, lengthUnit);

      // Lists to collect results
      Parallel.ForEach(nodes, node =>
      {
        int nodeId = node.Key;
        if (!xyzResults.ContainsKey(nodeId)) return;

        var vector3d = new Vector3d();// create Vector
        IQuantity forceValue = null;

        // pick the right value to display
        switch (_disp)
        {
          case (DisplayValue.X):
            var xVal = xyzResults[nodeId][0].X;
            vector3d = new Vector3d(xVal.As(this.ForceUnit) * scale, 0, 0);
            forceValue = xVal.ToUnit(this.ForceUnit);
            break;
          case (DisplayValue.Y):
            var yVal = xyzResults[nodeId][0].Y;
            vector3d = new Vector3d(0, yVal.As(this.ForceUnit) * scale, 0);
            forceValue = yVal.ToUnit(this.ForceUnit);
            break;
          case (DisplayValue.Z):
            var zVal = xyzResults[nodeId][0].Z;
            vector3d = new Vector3d(0, 0, zVal.As(this.ForceUnit) * scale);
            forceValue = zVal.ToUnit(this.ForceUnit);
            break;
          case (DisplayValue.ResXYZ):
            vector3d = new Vector3d(
              xyzResults[nodeId][0].X.As(this.ForceUnit) * scale,
              xyzResults[nodeId][0].Y.As(this.ForceUnit) * scale,
              xyzResults[nodeId][0].Z.As(this.ForceUnit) * scale);
            forceValue = xyzResults[nodeId][0].XYZ.ToUnit(this.ForceUnit);
            break;
          case (DisplayValue.XX):
            var xxVal = xxyyzzResults[nodeId][0].X;
            vector3d = new Vector3d(xxVal.As(this.MomentUnit) * scale, 0, 0);
            forceValue = xxVal.ToUnit(this.MomentUnit);
            break;
          case (DisplayValue.YY):
            var yyVal = xxyyzzResults[nodeId][0].Y;
            vector3d = new Vector3d(0, yyVal.As(this.MomentUnit) * scale, 0);
            forceValue = yyVal.ToUnit(this.MomentUnit);
            break;
          case (DisplayValue.ZZ):
            var zzVal = xxyyzzResults[nodeId][0].Z;
            vector3d = new Vector3d(0, 0, zzVal.As(this.MomentUnit) * scale);
            forceValue = zzVal.ToUnit(this.MomentUnit);
            break;
          case (DisplayValue.ResXXYYZZ):
            vector3d = new Vector3d(
              xxyyzzResults[nodeId][0].X.As(this.MomentUnit) * scale,
              xxyyzzResults[nodeId][0].Y.As(this.MomentUnit) * scale,
              xxyyzzResults[nodeId][0].Z.As(this.MomentUnit) * scale);
            forceValue = xxyyzzResults[nodeId][0].XYZ.ToUnit(this.MomentUnit);
            break;
        }

        var reactionVector = new ReactionForceVector(nodeId, node.Value.Value.Point, vector3d, forceValue);
        _reactionForceVectors.Add(reactionVector);
      });
      #endregion

      // set outputs
      var orderedReactionForceVectors = _reactionForceVectors.OrderBy(x => x.NodeId);
      foreach (var forceVector in orderedReactionForceVectors)
      {
        dataAccess.SetData(0, forceVector.StartingPoint);
        dataAccess.SetData(1, forceVector.ForceVector);
        dataAccess.SetData(2, forceVector.ForceValue);

      }
      //GH_UnitNumber
      Helpers.PostHog.Result(gsaResult.Type, 0, GsaResultsValues.ResultType.Force, this._disp.ToString());
    }

    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      _reactionForceVectors.ForEach(force =>
      {
        var line = new Line(force.StartingPoint, force.ForceVector);
        line.Flip();
        args.Display.DrawArrow(line, Color.Red);
        //args.Display.DrawArrowHead(force.StartingPoint, force.ForceVector, Color.Red, 50.0, 1.0);
      });

    }

    private static void GenerateReactionForceVectors()
    {

    }

    private bool IsGhObjectValid(GH_ObjectWrapper ghObject)
    {
      var valid = false;
      if (ghObject == null || ghObject.Value == null)
      {
        this.AddRuntimeWarning("Input is null");
      }
      else if (!(ghObject.Value is GsaResultGoo))
      {
        this.AddRuntimeError("Error converting input to GSA Result");
      }
      else
      {
        valid = true;
      }

      return valid;
    }

    #region Inputs ReactionForceVector methods
    private static double GetScalarValue(IGH_DataAccess dataAccess)
    {
      var ghScale = new GH_Number();
      dataAccess.GetData(2, ref ghScale);
      return GH_Convert.ToDouble(ghScale, out var scale, GH_Conversion.Both) ? scale : 0.0d;
    }

    private static string GetNodeFilters(IGH_DataAccess dataAccess)
    {
      var nodeList = string.Empty;
      var ghNoList = new GH_String();
      if (dataAccess.GetData(1, ref ghNoList))
      {
        nodeList = GH_Convert.ToString(ghNoList, out string tempNodeList, GH_Conversion.Both) ? tempNodeList : string.Empty;
      }

      if (nodeList.ToLower() == "all" || string.IsNullOrEmpty(nodeList))
        nodeList = "All";

      return nodeList;
    }

    private string GetCase(GsaResult gsaResult)
    {
      var caseName = string.Empty;
      switch (gsaResult.Type)
      {
        case GsaResult.CaseType.Combination when gsaResult.SelectedPermutationIDs.Count > 1:
          this.AddRuntimeWarning("Combination Case " + gsaResult.CaseID + " contains "
                                 + gsaResult.SelectedPermutationIDs.Count + " permutations - only one permutation can be displayed at a time." +
                                 Environment.NewLine + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
          break;
        case GsaResult.CaseType.Combination:
          caseName = "Case C" + gsaResult.CaseID + " P" + gsaResult.SelectedPermutationIDs[0];
          break;
        case GsaResult.CaseType.AnalysisCase:
          caseName = "Case A" + gsaResult.CaseID + Environment.NewLine + gsaResult.CaseName;
          break;
        default:
          caseName = string.Empty;
          break;
      }

      return caseName;
    }
    #endregion

    private LengthUnit GetLenghtUnit(GsaResult gsaResult)
    {
      var lengthUnit = gsaResult.Model.ModelUnit;
      var isUndefined = lengthUnit == LengthUnit.Undefined;

      this.undefinedModelLengthUnit = isUndefined;

      if (isUndefined)
      {
        lengthUnit = this.LengthUnit;
        this.AddRuntimeRemark("Model came straight out of GSA and we couldn't read the units. "
                              + "The geometry has been scaled to be in "
                              + lengthUnit.ToString()
                              + ". This can be changed by right-clicking the component -> 'Select Units'"
        );
      }

      return lengthUnit;
    }

    #region Custom UI
    private enum DisplayValue
    {
      X,
      Y,
      Z,
      ResXYZ,
      XX,
      YY,
      ZZ,
      ResXXYYZZ
    }

    private readonly List<string> _reactionStringList = new List<string>(new string[]
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

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Component"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // component
      this.DropDownItems.Add(this._reactionStringList);
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
      this.Message = (int)this._disp < 4
        ? Force.GetAbbreviation(this.ForceUnit)
        : Moment.GetAbbreviation(this.MomentUnit);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      var forceUnitsMenu = new ToolStripMenuItem("Force");
      forceUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force))
      {
        var toolStripMenuItem = new ToolStripMenuItem(
          unit,
          null,
          (s, e) => { UpdateForce(unit); }
        )
        {
          Checked = unit == Force.GetAbbreviation(this.ForceUnit),
          Enabled = true,
        };
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var momentUnitsMenu = new ToolStripMenuItem("Moment");
      momentUnitsMenu.Enabled = true;
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment))
      {
        var toolStripMenuItem = new ToolStripMenuItem(
          unit,
          null,
          (s, e) => { UpdateMoment(unit); }
          )
        {
          Checked = unit == Moment.GetAbbreviation(this.MomentUnit),
          Enabled = true,
        };
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);

      if (undefinedModelLengthUnit)
      {
        var modelUnitsMenu = new ToolStripMenuItem("Model geometry") { Enabled = true };
        foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length))
        {
          var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { UpdateModel(unit); })
          {
            Checked = unit == Length.GetAbbreviation(this.LengthUnit),
            Enabled = true,
          };
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
      writer.SetString("length", Length.GetAbbreviation(this.LengthResultUnit));
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
