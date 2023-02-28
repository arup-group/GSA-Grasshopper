using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to display GSA node result contours
  /// </summary>
  public class ReactionForceDiagrams : GH_OasysDropDownComponent
  {
    #region nested classes, enums
    private class ReactionForceVector
    {
      public readonly int NodeId;
      public readonly Point3d StartingPoint;
      public readonly Vector3d ForceVector;
      public readonly IQuantity ForceValue;
      public readonly DisplayValue Type;

      public ReactionForceVector(int nodeId, Point3d startingPoint, Vector3d forceVector, IQuantity forceValue, DisplayValue type)
      {
        this.NodeId = nodeId;
        this.StartingPoint = startingPoint;
        this.ForceVector = forceVector;
        this.ForceValue = forceValue;
        this.Type = type;
      }
    }

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
    #endregion

    #region readonly fields
    private readonly List<ReactionForceVector> _reactionForceVectors = new List<ReactionForceVector>();

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

    #endregion

    #region fields and properties

      #region public/internal/protected internal
    public override Guid ComponentGuid => new Guid("5bc139e5-614b-4f2d-887c-a980f1cbb32c");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    #endregion

      #region protected and private
    private string _case = "";
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private bool _undefinedModelLengthUnit = false;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private DisplayValue _selectedDisplayValue = DisplayValue.ResXYZ;
    #endregion

    #endregion

    #region public methods
    public ReactionForceDiagrams() : base("Reaction Force Diagrams",
      "ReactionForce",
      "Diplays GSA Node Reaction Force Results as Vector Diagrams",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { }

      #region core
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + Environment.NewLine +
                                                          "Node list should take the form:" + Environment.NewLine +
                                                          " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine +
                                                          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Scalar", "x:X", "Scale the result display size", GH_ParamAccess.item, 1);
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

    protected override void SolveInstance(IGH_DataAccess dataAccess)
    {
      var gsaResult = new GsaResult();
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
      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues = gsaResult.NodeReactionForceValues(filteredNodes, this._forceUnit, this._momentUnit);
      GsaResultsValues forceValues = reactionForceValues.Item1[0];
      filteredNodes = string.Join(" ", reactionForceValues.Item2);

      #region Result point values
      LengthUnit lengthUnit = GetLengthUnit(gsaResult);

      // Get nodes for point location and restraint check in case of reaction force
      ReadOnlyDictionary<int, Node> gsaFilteredNodes = gsaResult.Model.Model.Nodes(filteredNodes);
      ConcurrentDictionary<int, GsaNodeGoo> nodes = Helpers.Import.Nodes.GetNodeDictionary(gsaFilteredNodes, lengthUnit);

      Parallel.ForEach(nodes, node =>
      {
        var reactionForceVector = GenerateReactionForceVector(node, forceValues, scale);
        if (reactionForceVector != null) _reactionForceVectors.Add(reactionForceVector);
      });
      #endregion

      this.SetOutputs(dataAccess);
      Helpers.PostHog.Result(gsaResult.Type, 0, GsaResultsValues.ResultType.Force, this._selectedDisplayValue.ToString());
    }

    #endregion

    #region Custom UI
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new[]
      {
        "Component"
      });

      this.DropDownItems = new List<List<string>> { this._reactionStringList };
      this.SelectedItems = new List<string> { this.DropDownItems[0][3] };

      this.IsInitialised = true;
    }

    public override void CreateAttributes()
    {
      if (!IsInitialised)
      {
        InitialiseDropdowns();
      }
      m_attributes = new OasysGH.UI.DropDownComponentAttributes(this, SetSelected, this.DropDownItems, this.SelectedItems, this.SpacerDescriptions);
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedDisplayValue = (DisplayValue)j;
      this.SelectedItems[i] = this.DropDownItems[i][j];
      base.UpdateUI();
    }
    #endregion

      #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetInt32("Display", (int)_selectedDisplayValue);
      writer.SetString("model", Length.GetAbbreviation(this._lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(this._lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(this._forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(this._momentUnit));
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this._selectedDisplayValue = (DisplayValue)reader.GetInt32("Display");
      this._lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      this._lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      this._forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      this._momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      return base.Read(reader);
    }
    #endregion
    
    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      _reactionForceVectors.ForEach(force =>
      {
        var line = new Line(force.StartingPoint, force.ForceVector);
        line.Flip();
        line.Transform(Transform.Scale(force.StartingPoint, -1));

        Point3d p = new Point3d(line.To);
        Vector3d motion = line.Direction;
        //motion.Transform(Transform.Scale(force.StartingPoint, -1));
        motion.Unitize();
        Transform t = Transform.Translation(motion * 0.05);
        p.Transform(t);

        args.Display.DrawArrowHead(p, force.ForceVector, Color.Red, 20, 0);
        args.Display.DrawArrow(line, Color.Blue);

        if (force.Type == DisplayValue.ResXYZ || force.Type == DisplayValue.X || force.Type == DisplayValue.Y || force.Type == DisplayValue.Z)
          args.Display.DrawArrow(line, Helpers.Graphics.Colours.GsaDarkPurple);
        else //moments
          args.Display.DrawArrow(line, Helpers.Graphics.Colours.GsaGold);
      });
    }

    #endregion

    #region protected methods
      #region menu override
    protected override void BeforeSolveInstance()
    {
      this.Message = (int)this._selectedDisplayValue < 4
        ? Force.GetAbbreviation(this._forceUnit)
        : Moment.GetAbbreviation(this._momentUnit);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);
      var forceUnitsMenu = GenerateForceUnitsMenu("Force");
      var momentUnitsMenu = GenerateMomentUnitsMenu("Moment");
      
      var toolStripItems = new List<ToolStripItem> { forceUnitsMenu, momentUnitsMenu };

      if (_undefinedModelLengthUnit)
      {
        var modelUnitsMenu = GenerateModelGeometryUnitsMenu("Model geometry");
        toolStripItems.Insert(0, modelUnitsMenu);
      }
      unitsMenu.DropDownItems.AddRange(toolStripItems.ToArray());
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);
      Menu_AppendSeparator(menu);
    }

    #endregion
    #endregion

    #region private methods

      #region menu helpers
    private void UpdateModel(string unit)
    {
      this._lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateForce(string unit)
    {
      this._forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateMoment(string unit)
    {
      this._momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      this.ExpirePreview(true);
      base.UpdateUI();
    }

    private ToolStripMenuItem GenerateForceUnitsMenu(string menuTitle)
    {
      var forceUnitsMenu = new ToolStripMenuItem(menuTitle) { Enabled = true };

      foreach (var toolStripMenuItem in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force).Select(unit => new ToolStripMenuItem(
                 unit,
                 null,
                 (s, e) => { UpdateForce(unit); }
               )
               {
                 Checked = unit == Force.GetAbbreviation(this._forceUnit),
                 Enabled = true,
               }))
      {
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return forceUnitsMenu;
    }

    private ToolStripMenuItem GenerateMomentUnitsMenu(string menuTitle)
    {
      var momentUnitsMenu = new ToolStripMenuItem(menuTitle) { Enabled = true };
      foreach (var toolStripMenuItem in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment).Select(unit => new ToolStripMenuItem(
                 unit,
                 null,
                 (s, e) => { UpdateMoment(unit); }
               )
               {
                 Checked = unit == Moment.GetAbbreviation(this._momentUnit),
                 Enabled = true,
               }))
      {
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return momentUnitsMenu;
    }

    private ToolStripMenuItem GenerateModelGeometryUnitsMenu(string menuTitle)
    {
      var modelUnitsMenu = new ToolStripMenuItem(menuTitle) { Enabled = true };
      foreach (var toolStripMenuItem in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length).Select(unit => new ToolStripMenuItem(unit, null, (s, e) => { UpdateModel(unit); })
               {
                 Checked = unit == Length.GetAbbreviation(this._lengthUnit),
                 Enabled = true,
               }))
      {
        modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return modelUnitsMenu;
    }
    #endregion

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

      #region solvingInstance helpers
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

    private LengthUnit GetLengthUnit(GsaResult gsaResult)
    {
      var lengthUnit = gsaResult.Model.ModelUnit;
      var isUndefined = lengthUnit == LengthUnit.Undefined;

      this._undefinedModelLengthUnit = isUndefined;

      if (!isUndefined) return lengthUnit;

      lengthUnit = this._lengthUnit;
      this.AddRuntimeRemark("Model came straight out of GSA and we couldn't read the units. "
                            + "The geometry has been scaled to be in "
                            + lengthUnit.ToString()
                            + ". This can be changed by right-clicking the component -> 'Select Units'"
      );

      return lengthUnit;
    }
  
    private ReactionForceVector GenerateReactionForceVector(KeyValuePair<int,GsaNodeGoo> node, GsaResultsValues forceValues, double scale)
    {
      var nodeId = node.Key;
      var xyzResults = forceValues.xyzResults;
      var xxyyzzResults = forceValues.xxyyzzResults;
      
      if (!xyzResults.ContainsKey(nodeId)) return null;

      var vector3d = new Vector3d();
      IQuantity forceValue = null;
      
      switch (_selectedDisplayValue)
      {
        case (DisplayValue.X):
          var xVal = xyzResults[nodeId][0].X;
          vector3d = new Vector3d(xVal.As(this._forceUnit) * scale, 0, 0);
          forceValue = xVal.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.Y):
          var yVal = xyzResults[nodeId][0].Y;
          vector3d = new Vector3d(0, yVal.As(this._forceUnit) * scale, 0);
          forceValue = yVal.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.Z):
          var zVal = xyzResults[nodeId][0].Z;
          vector3d = new Vector3d(0, 0, zVal.As(this._forceUnit) * scale);
          forceValue = zVal.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.ResXYZ):
          vector3d = new Vector3d(
            xyzResults[nodeId][0].X.As(this._forceUnit) * scale,
            xyzResults[nodeId][0].Y.As(this._forceUnit) * scale,
            xyzResults[nodeId][0].Z.As(this._forceUnit) * scale);
          forceValue = xyzResults[nodeId][0].XYZ.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.XX):
          var xxVal = xxyyzzResults[nodeId][0].X;
          vector3d = new Vector3d(xxVal.As(this._momentUnit) * scale, 0, 0);
          forceValue = xxVal.ToUnit(this._momentUnit);
          break;
        case (DisplayValue.YY):
          var yyVal = xxyyzzResults[nodeId][0].Y;
          vector3d = new Vector3d(0, yyVal.As(this._momentUnit) * scale, 0);
          forceValue = yyVal.ToUnit(this._momentUnit);
          break;
        case (DisplayValue.ZZ):
          var zzVal = xxyyzzResults[nodeId][0].Z;
          vector3d = new Vector3d(0, 0, zzVal.As(this._momentUnit) * scale);
          forceValue = zzVal.ToUnit(this._momentUnit);
          break;
        case (DisplayValue.ResXXYYZZ):
          vector3d = new Vector3d(
            xxyyzzResults[nodeId][0].X.As(this._momentUnit) * scale,
            xxyyzzResults[nodeId][0].Y.As(this._momentUnit) * scale,
            xxyyzzResults[nodeId][0].Z.As(this._momentUnit) * scale);
          forceValue = xxyyzzResults[nodeId][0].XYZ.ToUnit(this._momentUnit);
          break;
      }

      return new ReactionForceVector(nodeId, node.Value.Value.Point, vector3d, forceValue, _selectedDisplayValue);
    }

    private void SetOutputs(IGH_DataAccess dataAccess)
    {
      var orderedReactionForceVectors = _reactionForceVectors.OrderBy(x => x.NodeId);
      dataAccess.SetDataList(0, orderedReactionForceVectors.Select(a => a.StartingPoint));
      dataAccess.SetDataList(1, orderedReactionForceVectors.Select(a => a.ForceVector));
      dataAccess.SetDataList(2, orderedReactionForceVectors.Select(a => a.ForceValue));
    }
    #endregion

    #endregion
  }
}
