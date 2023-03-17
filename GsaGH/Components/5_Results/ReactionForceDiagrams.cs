using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Types.Transforms;
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
  /// Component to display GSA reaction forces
  /// </summary>
  public class ReactionForceDiagrams : GH_OasysDropDownComponent
  {
    #region nested classes, enums
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

    #region fields and properties
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

    public override Guid ComponentGuid => new Guid("5bc139e5-614b-4f2d-887c-a980f1cbb32c");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ReactionForceDiagram;

    private ConcurrentDictionary<int, VectorResultGoo> _reactionForceVectors = new ConcurrentDictionary<int, VectorResultGoo>();
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private bool _undefinedModelLengthUnit = false;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private DisplayValue _selectedDisplayValue = DisplayValue.ResXYZ;
    private bool _showText = true;
    #endregion

    public ReactionForceDiagrams() : base("Reaction Force Diagrams",
      "ReactionForce",
      "Diplays GSA Node Reaction Force Results as Vector Diagrams",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { }

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + Environment.NewLine +
                                                          "Node list should take the form:" + Environment.NewLine +
                                                          " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine +
                                                          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Scalar", "x:X", "Scale the result display size to a specific size. Autoscaling by default.", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Anchor Point", "A", "Support Node Location", GH_ParamAccess.list);
      pManager.AddGenericParameter("Vector", "V", "Reaction Force Vector", GH_ParamAccess.list);
      pManager.AddGenericParameter("Value", "Val", "Reaction Force Value", GH_ParamAccess.list);
      pManager.HideParameter(0);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess dataAccess)
    {
      var gsaResult = new GsaResult();
      var ghObject = new GH_ObjectWrapper();

      if (!dataAccess.GetData(0, ref ghObject) || !IsGhObjectValid(ghObject)) return;

      gsaResult = (ghObject.Value as GsaResultGoo).Value;
      string filteredNodes = GetNodeFilters(dataAccess);

      // get stuff for drawing
      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues = gsaResult.NodeReactionForceValues(filteredNodes, this._forceUnit, this._momentUnit);
      GsaResultsValues forceValues = reactionForceValues.Item1[0];
      filteredNodes = string.Join(" ", reactionForceValues.Item2);

      LengthUnit lengthUnit = GetLengthUnit(gsaResult);

      // Get nodes for point location and restraint check in case of reaction force
      ReadOnlyDictionary<int, Node> gsaFilteredNodes = gsaResult.Model.Model.Nodes(filteredNodes);
      ConcurrentDictionary<int, GsaNodeGoo> nodes = Helpers.Import.Nodes.GetNodeDictionary(gsaFilteredNodes, lengthUnit);

      // get scale or compute autoscale
      double scale = 1;
      if (!dataAccess.GetData(2, ref scale))
        scale = ComputeScale(forceValues, gsaResult.Model.BoundingBox);

      _reactionForceVectors = new ConcurrentDictionary<int, VectorResultGoo>();
      Parallel.ForEach(nodes, node =>
      {
          var reactionForceVector = GenerateReactionForceVector(node, forceValues, scale);
          if (reactionForceVector != null) _reactionForceVectors.TryAdd(node.Key, reactionForceVector);
      });

      this.SetOutputs(dataAccess);
      Helpers.PostHog.Result(gsaResult.Type, 0, GsaResultsValues.ResultType.Force, this._selectedDisplayValue.ToString());
    }

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

    #region custom preview
    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      foreach (var force in _reactionForceVectors)
      {
        force.Value.ShowText(_showText);
      };
    }

    #endregion

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
      Menu_AppendItem(menu, "Show Text", ShowText, true, _showText);

      var unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);
      var forceUnitsMenu = GenerateForceUnitsMenu("Force");
      var momentUnitsMenu = GenerateMomentUnitsMenu("Moment");

      var toolStripItems = new List<ToolStripItem> { forceUnitsMenu, momentUnitsMenu };

      if (this._lengthUnit == LengthUnit.Undefined)
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

    private void ShowText(object sender, EventArgs e)
    {
      _showText = !_showText;
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

    private double ComputeScale(GsaResultsValues forceValues, BoundingBox bbox)
    {
      double abs = 1;
      switch (_selectedDisplayValue)
      {
        case (DisplayValue.X):
          abs = Math.Max(
            forceValues.DmaxX.As(this._forceUnit),
            Math.Abs(forceValues.DminX.As(this._forceUnit)));
          break;
        case (DisplayValue.Y):
          abs = Math.Max(
            forceValues.DmaxY.As(this._forceUnit),
            Math.Abs(forceValues.DminY.As(this._forceUnit)));
          break;
        case (DisplayValue.Z):
          abs = Math.Max(
            forceValues.DmaxZ.As(this._forceUnit),
            Math.Abs(forceValues.DminZ.As(this._forceUnit)));
          break;
        case (DisplayValue.ResXYZ):
          abs = Math.Max(
            forceValues.DmaxXyz.As(this._forceUnit),
            Math.Abs(forceValues.DminXyz.As(this._forceUnit)));
          break;
        case (DisplayValue.XX):
          abs = Math.Max(
            forceValues.DmaxXx.As(this._forceUnit),
            Math.Abs(forceValues.DminXx.As(this._forceUnit)));
          break;
        case (DisplayValue.YY):
          abs = Math.Max(
            forceValues.DmaxYy.As(this._forceUnit),
            Math.Abs(forceValues.DminYy.As(this._forceUnit)));
          break;
        case (DisplayValue.ZZ):
          abs = Math.Max(
            forceValues.DmaxZz.As(this._forceUnit),
            Math.Abs(forceValues.DminZz.As(this._forceUnit)));
          break;
        case (DisplayValue.ResXXYYZZ):
          abs = Math.Max(
            forceValues.DmaxXxyyzz.As(this._forceUnit),
            Math.Abs(forceValues.DminXxyyzz.As(this._forceUnit)));
          break;
      }
      double factor = 0.000001;
      return bbox.Area * abs * factor;
    }

    private VectorResultGoo GenerateReactionForceVector(KeyValuePair<int, GsaNodeGoo> node, GsaResultsValues forceValues, double scale)
    {
      var nodeId = node.Key;
      var xyzResults = forceValues.xyzResults;
      var xxyyzzResults = forceValues.xxyyzzResults;

      if (!xyzResults.ContainsKey(nodeId)) return null;

      var direction = new Vector3d();
      IQuantity forceValue = null;
      var isForce = true;

      switch (_selectedDisplayValue)
      {
        case (DisplayValue.X):
          var xVal = xyzResults[nodeId][0].X;
          direction = new Vector3d(xVal.As(this._forceUnit) * scale, 0, 0);
          forceValue = xVal.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.Y):
          var yVal = xyzResults[nodeId][0].Y;
          direction = new Vector3d(0, yVal.As(this._forceUnit) * scale, 0);
          forceValue = yVal.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.Z):
          var zVal = xyzResults[nodeId][0].Z;
          direction = new Vector3d(0, 0, zVal.As(this._forceUnit) * scale);
          forceValue = zVal.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.ResXYZ):
          direction = new Vector3d(
            xyzResults[nodeId][0].X.As(this._forceUnit) * scale,
            xyzResults[nodeId][0].Y.As(this._forceUnit) * scale,
            xyzResults[nodeId][0].Z.As(this._forceUnit) * scale);
          forceValue = xyzResults[nodeId][0].XYZ.ToUnit(this._forceUnit);
          break;
        case (DisplayValue.XX):
          isForce = false;
          var xxVal = xxyyzzResults[nodeId][0].X;
          direction = new Vector3d(xxVal.As(this._momentUnit) * scale, 0, 0);
          forceValue = xxVal.ToUnit(this._momentUnit);
          break;
        case (DisplayValue.YY):
          isForce = false;
          var yyVal = xxyyzzResults[nodeId][0].Y;
          direction = new Vector3d(0, yyVal.As(this._momentUnit) * scale, 0);
          forceValue = yyVal.ToUnit(this._momentUnit);
          break;
        case (DisplayValue.ZZ):
          isForce = false;
          var zzVal = xxyyzzResults[nodeId][0].Z;
          direction = new Vector3d(0, 0, zzVal.As(this._momentUnit) * scale);
          forceValue = zzVal.ToUnit(this._momentUnit);
          break;
        case (DisplayValue.ResXXYYZZ):
          isForce = false;
          direction = new Vector3d(
            xxyyzzResults[nodeId][0].X.As(this._momentUnit) * scale,
            xxyyzzResults[nodeId][0].Y.As(this._momentUnit) * scale,
            xxyyzzResults[nodeId][0].Z.As(this._momentUnit) * scale);
          forceValue = xxyyzzResults[nodeId][0].XYZ.ToUnit(this._momentUnit);
          break;
      }

      var vectorResult = new VectorResultGoo(node.Value.Value.Point, direction, forceValue);

      if (isForce) return vectorResult;

      return vectorResult.SetColor(Helpers.Graphics.Colours.GsaGold)
          .DrawArrowHead(true);
    }

    private void SetOutputs(IGH_DataAccess dataAccess)
    {
      var orderedDict = _reactionForceVectors.OrderBy(index => index.Key);
      var startingPoints = new List<Point3d>();
      var vectors = new List<VectorResultGoo>();
      var forceValues = new List<IQuantity>();

      foreach (var keyValuePair in orderedDict)
      {
        startingPoints.Add(keyValuePair.Value.StartingPoint);
        vectors.Add(keyValuePair.Value);
        forceValues.Add(keyValuePair.Value.ForceValue);
      }

      dataAccess.SetDataList(0, startingPoints);
      dataAccess.SetDataList(1, vectors);
      dataAccess.SetDataList(2, forceValues);
    }

    #endregion
  }
}
