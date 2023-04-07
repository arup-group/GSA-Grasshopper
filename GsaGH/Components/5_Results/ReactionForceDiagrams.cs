using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to display GSA reaction forces
  /// </summary>
  public class ReactionForceDiagrams : GH_OasysDropDownComponent {
    public ReactionForceDiagrams() : base("Reaction Force Diagrams",
      "ReactionForce",
      "Diplays GSA Node Reaction Force Results as Vector Diagrams",
      CategoryName.Name(),
      SubCategoryName.Cat5()) { }

    protected override void SolveInstance(IGH_DataAccess dataAccess) {
      var gsaResult = new GsaResult();
      var ghObject = new GH_ObjectWrapper();

      if (!dataAccess.GetData(0, ref ghObject) || !IsGhObjectValid(ghObject))
        return;

      gsaResult = (ghObject.Value as GsaResultGoo).Value;
      string filteredNodes = GetNodeFilters(dataAccess);

      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues
        = gsaResult.NodeReactionForceValues(filteredNodes, _forceUnit, _momentUnit);
      GsaResultsValues forceValues = reactionForceValues.Item1[0];
      filteredNodes = string.Join(" ", reactionForceValues.Item2);

      LengthUnit lengthUnit = GetLengthUnit(gsaResult);

      ReadOnlyDictionary<int, Node> gsaFilteredNodes = gsaResult.Model.Model.Nodes(filteredNodes);
      ConcurrentDictionary<int, GsaNodeGoo> nodes
        = Nodes.GetNodeDictionary(gsaFilteredNodes, lengthUnit, gsaResult.Model.Model.Axes());

      double scale = 1;
      if (!dataAccess.GetData(2, ref scale))
        scale = ComputeScale(forceValues, gsaResult.Model.BoundingBox);

      _reactionForceVectors = new ConcurrentDictionary<int, VectorResultGoo>();
      Parallel.ForEach(nodes,
        node => {
          VectorResultGoo reactionForceVector
            = GenerateReactionForceVector(node, forceValues, scale);
          if (reactionForceVector != null)
            _reactionForceVectors.TryAdd(node.Key, reactionForceVector);
        });

      SetOutputs(dataAccess);
      PostHog.Result(gsaResult.Type,
        0,
        GsaResultsValues.ResultType.Force,
        _selectedDisplayValue.ToString());
    }

    #region custom preview

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      foreach (KeyValuePair<int, VectorResultGoo> force in _reactionForceVectors)
        force.Value.ShowText(_showText);
    }

    #endregion

    #region Inputs ReactionForceVector methods

    private static string GetNodeFilters(IGH_DataAccess dataAccess) {
      string nodeList = string.Empty;
      var ghNoList = new GH_String();
      if (dataAccess.GetData(1, ref ghNoList))
        nodeList = GH_Convert.ToString(ghNoList, out string tempNodeList, GH_Conversion.Both)
          ? tempNodeList
          : string.Empty;

      if (nodeList.ToLower() == "all" || string.IsNullOrEmpty(nodeList))
        nodeList = "All";

      return nodeList;
    }

    #endregion

    #region nested classes, enums

    private enum DisplayValue {
      X,
      Y,
      Z,
      ResXyz,
      Xx,
      Yy,
      Zz,
      ResXxyyzz,
    }

    #endregion

    #region fields and properties

    private readonly List<string> _reactionStringList = new List<string>(new[] {
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
    protected override Bitmap Icon => Resources.ReactionForceDiagram;

    private ConcurrentDictionary<int, VectorResultGoo> _reactionForceVectors
      = new ConcurrentDictionary<int, VectorResultGoo>();

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private DisplayValue _selectedDisplayValue = DisplayValue.ResXyz;
    private bool _showText = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(),
        "Result",
        "Res",
        "GSA Result",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list",
        "No",
        "Filter results by list."
        + Environment.NewLine
        + "Node list should take the form:"
        + Environment.NewLine
        + " 1 11 to 72 step 2 not (XY3 31 to 45)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item,
        "All");
      pManager.AddNumberParameter("Scalar",
        "x:X",
        "Scale the result vectors to a specific size. If left empty, automatic scaling based on model size and maximum result by load cases will be computed.",
        GH_ParamAccess.item);
      pManager[1]
        .Optional = true;
      pManager[2]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPointParameter("Anchor Point", "A", "Support Node Location", GH_ParamAccess.list);
      pManager.AddGenericParameter("Vector", "V", "Reaction Force Vector", GH_ParamAccess.list);
      pManager.AddGenericParameter("Value", "Val", "Reaction Force Value", GH_ParamAccess.list);
      pManager.HideParameter(0);
    }

    #endregion

    #region Custom UI

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Component",
      });

      _dropDownItems = new List<List<string>> {
        _reactionStringList,
      };
      _selectedItems = new List<string> {
        _dropDownItems[0][3],
      };

      _isInitialised = true;
    }

    public override void CreateAttributes() {
      if (!_isInitialised)
        InitialiseDropdowns();
      m_attributes = new DropDownComponentAttributes(this,
        SetSelected,
        _dropDownItems,
        _selectedItems,
        _spacerDescriptions);
    }

    public override void SetSelected(int i, int j) {
      _selectedDisplayValue = (DisplayValue)j;
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    #endregion

    #region (de)serialization

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Display", (int)_selectedDisplayValue);
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      _selectedDisplayValue = (DisplayValue)reader.GetInt32("Display");
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      return base.Read(reader);
    }

    #endregion

    #region menu override

    protected override void BeforeSolveInstance()
      => Message = (int)_selectedDisplayValue < 4
        ? Force.GetAbbreviation(_forceUnit)
        : Moment.GetAbbreviation(_momentUnit);

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Text", ShowText, true, _showText);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.Units);
      ToolStripMenuItem forceUnitsMenu = GenerateForceUnitsMenu("Force");
      ToolStripMenuItem momentUnitsMenu = GenerateMomentUnitsMenu("Moment");

      var toolStripItems = new List<ToolStripItem> {
        forceUnitsMenu,
        momentUnitsMenu,
      };

      if (_lengthUnit == LengthUnit.Undefined) {
        ToolStripMenuItem modelUnitsMenu = GenerateModelGeometryUnitsMenu("Model geometry");
        toolStripItems.Insert(0, modelUnitsMenu);
      }

      unitsMenu.DropDownItems.AddRange(toolStripItems.ToArray());
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);
      Menu_AppendSeparator(menu);
    }

    #endregion

    #region menu helpers

    private void UpdateModel(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateMoment(string unit) {
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void ShowText(object sender, EventArgs e) {
      _showText = !_showText;
      ExpirePreview(true);
      base.UpdateUI();
    }

    private ToolStripMenuItem GenerateForceUnitsMenu(string menuTitle) {
      var forceUnitsMenu = new ToolStripMenuItem(menuTitle) {
        Enabled = true,
      };

      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper
        .GetFilteredAbbreviations(EngineeringUnits.Force)
        .Select(unit => new ToolStripMenuItem(unit, null, (s, e) => UpdateForce(unit)) {
          Checked = unit == Force.GetAbbreviation(_forceUnit),
          Enabled = true,
        }))
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);

      return forceUnitsMenu;
    }

    private ToolStripMenuItem GenerateMomentUnitsMenu(string menuTitle) {
      var momentUnitsMenu = new ToolStripMenuItem(menuTitle) {
        Enabled = true,
      };
      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper
        .GetFilteredAbbreviations(EngineeringUnits.Moment)
        .Select(unit => new ToolStripMenuItem(unit, null, (s, e) => UpdateMoment(unit)) {
          Checked = unit == Moment.GetAbbreviation(_momentUnit),
          Enabled = true,
        }))
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);

      return momentUnitsMenu;
    }

    private ToolStripMenuItem GenerateModelGeometryUnitsMenu(string menuTitle) {
      var modelUnitsMenu = new ToolStripMenuItem(menuTitle) {
        Enabled = true,
      };
      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper
        .GetFilteredAbbreviations(EngineeringUnits.Length)
        .Select(unit => new ToolStripMenuItem(unit, null, (s, e) => UpdateModel(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        }))
        modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);

      return modelUnitsMenu;
    }

    #endregion

    #region solvingInstance helpers

    private bool IsGhObjectValid(GH_ObjectWrapper ghObject) {
      bool valid = false;
      if (ghObject == null || ghObject.Value == null)
        this.AddRuntimeWarning("Input is null");
      else if (!(ghObject.Value is GsaResultGoo))
        this.AddRuntimeError("Error converting input to GSA Result");
      else
        valid = true;

      return valid;
    }

    private LengthUnit GetLengthUnit(GsaResult gsaResult) {
      LengthUnit lengthUnit = gsaResult.Model.ModelUnit;
      bool isUndefined = lengthUnit == LengthUnit.Undefined;

      if (!isUndefined)
        return lengthUnit;

      lengthUnit = _lengthUnit;
      this.AddRuntimeRemark("Model came straight out of GSA and we couldn't read the units. "
        + "The geometry has been scaled to be in "
        + lengthUnit.ToString()
        + ". This can be changed by right-clicking the component -> 'Select Units'");

      return lengthUnit;
    }

    private double ComputeScale(GsaResultsValues forceValues, BoundingBox bbox) {
      var values = new List<double>(8);
      switch (_selectedDisplayValue) {
        case (DisplayValue.X):
        case (DisplayValue.Y):
        case (DisplayValue.Z):
        case (DisplayValue.ResXyz):
          values = new List<double>() {
            forceValues.DmaxX.As(_forceUnit),
            forceValues.DmaxY.As(_forceUnit),
            forceValues.DmaxZ.As(_forceUnit),
            forceValues.DmaxXyz.As(_forceUnit),
            forceValues.DminXyz.As(_forceUnit),
            Math.Abs(forceValues.DminX.As(_forceUnit)),
            Math.Abs(forceValues.DminY.As(_forceUnit)),
            Math.Abs(forceValues.DminZ.As(_forceUnit)),
          };
          break;

        case (DisplayValue.Xx):
        case (DisplayValue.Yy):
        case (DisplayValue.Zz):
        case (DisplayValue.ResXxyyzz):
          values = new List<double>() {
            forceValues.DmaxXx.As(_momentUnit),
            forceValues.DmaxYy.As(_momentUnit),
            forceValues.DmaxZz.As(_momentUnit),
            forceValues.DmaxXxyyzz.As(_momentUnit),
            forceValues.DminXxyyzz.As(_momentUnit),
            Math.Abs(forceValues.DminXx.As(_momentUnit)),
            Math.Abs(forceValues.DminYy.As(_momentUnit)),
            Math.Abs(forceValues.DminZz.As(_momentUnit)),
          };
          break;
      }

      double factor = 0.000001;
      return bbox.Diagonal.Length * values.Max() * factor;
    }

    private VectorResultGoo GenerateReactionForceVector(
      KeyValuePair<int, GsaNodeGoo> node,
      GsaResultsValues forceValues,
      double scale) {
      int nodeId = node.Key;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults
        = forceValues.XyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults
        = forceValues.XxyyzzResults;

      if (!xyzResults.ContainsKey(nodeId))
        return null;

      var direction = new Vector3d();
      IQuantity forceValue = null;
      bool isForce = true;

      switch (_selectedDisplayValue) {
        case (DisplayValue.X):
          IQuantity xVal = xyzResults[nodeId][0]
            .X;
          direction = new Vector3d(xVal.As(_forceUnit) * scale, 0, 0);
          forceValue = xVal.ToUnit(_forceUnit);
          break;
        case (DisplayValue.Y):
          IQuantity yVal = xyzResults[nodeId][0]
            .Y;
          direction = new Vector3d(0, yVal.As(_forceUnit) * scale, 0);
          forceValue = yVal.ToUnit(_forceUnit);
          break;
        case (DisplayValue.Z):
          IQuantity zVal = xyzResults[nodeId][0]
            .Z;
          direction = new Vector3d(0, 0, zVal.As(_forceUnit) * scale);
          forceValue = zVal.ToUnit(_forceUnit);
          break;
        case (DisplayValue.ResXyz):
          direction = new Vector3d(xyzResults[nodeId][0]
              .X.As(_forceUnit)
            * scale,
            xyzResults[nodeId][0]
              .Y.As(_forceUnit)
            * scale,
            xyzResults[nodeId][0]
              .Z.As(_forceUnit)
            * scale);
          forceValue = xyzResults[nodeId][0]
            .Xyz.ToUnit(_forceUnit);
          break;
        case (DisplayValue.Xx):
          isForce = false;
          IQuantity xxVal = xxyyzzResults[nodeId][0]
            .X;
          direction = new Vector3d(xxVal.As(_momentUnit) * scale, 0, 0);
          forceValue = xxVal.ToUnit(_momentUnit);
          break;
        case (DisplayValue.Yy):
          isForce = false;
          IQuantity yyVal = xxyyzzResults[nodeId][0]
            .Y;
          direction = new Vector3d(0, yyVal.As(_momentUnit) * scale, 0);
          forceValue = yyVal.ToUnit(_momentUnit);
          break;
        case (DisplayValue.Zz):
          isForce = false;
          IQuantity zzVal = xxyyzzResults[nodeId][0]
            .Z;
          direction = new Vector3d(0, 0, zzVal.As(_momentUnit) * scale);
          forceValue = zzVal.ToUnit(_momentUnit);
          break;
        case (DisplayValue.ResXxyyzz):
          isForce = false;
          direction = new Vector3d(xxyyzzResults[nodeId][0]
              .X.As(_momentUnit)
            * scale,
            xxyyzzResults[nodeId][0]
              .Y.As(_momentUnit)
            * scale,
            xxyyzzResults[nodeId][0]
              .Z.As(_momentUnit)
            * scale);
          forceValue = xxyyzzResults[nodeId][0]
            .Xyz.ToUnit(_momentUnit);
          break;
      }

      if (!node.Value.Value.IsGlobalAxis()) {
        var rotation = Transform.PlaneToPlane(node.Value.Value.LocalAxis, Plane.WorldXY);
        direction.Transform(rotation);
      }

      var vectorResult = new VectorResultGoo(node.Value.Value.Point, direction, forceValue, nodeId);

      return isForce
        ? vectorResult
        : vectorResult.SetColor(Colours.GsaGold)
          .DrawArrowHead(true);
    }

    private void SetOutputs(IGH_DataAccess dataAccess) {
      IOrderedEnumerable<KeyValuePair<int, VectorResultGoo>> orderedDict
        = _reactionForceVectors.OrderBy(index => index.Key);
      var startingPoints = new List<Point3d>();
      var vectors = new List<VectorResultGoo>();
      var forceValues = new List<IQuantity>();

      foreach (KeyValuePair<int, VectorResultGoo> keyValuePair in orderedDict) {
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
