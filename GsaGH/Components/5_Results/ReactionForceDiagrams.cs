﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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

namespace GsaGH.Components {
  /// <summary>
  /// Component to display GSA reaction forces
  /// </summary>
  public class ReactionForceDiagrams : GH_OasysDropDownComponent {
    #region nested classes, enums
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private enum DisplayValue {
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
    private readonly List<string> _reactionStringList = new List<string>(new []
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
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ReactionForceDiagram;

    private ConcurrentDictionary<int, VectorResultGoo> _reactionForceVectors = new ConcurrentDictionary<int, VectorResultGoo>();
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
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
      SubCategoryName.Cat5()) { }

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + Environment.NewLine +
                                                          "Node list should take the form:" + Environment.NewLine +
                                                          " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine +
                                                          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Scalar", "x:X", "Scale the result display size", GH_ParamAccess.item, 1);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPointParameter("Anchor Point", "A", "Support Node Location", GH_ParamAccess.list);
      pManager.AddGenericParameter("Vector", "V", "Reaction Force Vector", GH_ParamAccess.list);
      pManager.AddGenericParameter("Value", "Val", "Reaction Force Value", GH_ParamAccess.list);
      pManager.HideParameter(0);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess dataAccess) {
      var gsaResult = new GsaResult();
      var ghObject = new GH_ObjectWrapper();

      if (!dataAccess.GetData(0, ref ghObject) || !IsGhObjectValid(ghObject))
        return;

      gsaResult = ((GsaResultGoo)ghObject.Value).Value;
      string filteredNodes = GetNodeFilters(dataAccess);
      double scale = GetScalarValue(dataAccess);

      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues = gsaResult.NodeReactionForceValues(filteredNodes, _forceUnit, _momentUnit);
      GsaResultsValues forceValues = reactionForceValues.Item1[0];
      filteredNodes = string.Join(" ", reactionForceValues.Item2);

      LengthUnit lengthUnit = GetLengthUnit(gsaResult);

      ReadOnlyDictionary<int, Node> gsaFilteredNodes = gsaResult.Model.Model.Nodes(filteredNodes);
      ConcurrentDictionary<int, GsaNodeGoo> nodes = Helpers.Import.Nodes.GetNodeDictionary(gsaFilteredNodes, lengthUnit);

      _reactionForceVectors = new ConcurrentDictionary<int, VectorResultGoo>();
      Parallel.ForEach(nodes, node => {
        VectorResultGoo reactionForceVector = GenerateReactionForceVector(node, forceValues, scale);
        if (reactionForceVector != null)
          _reactionForceVectors.TryAdd(node.Key, reactionForceVector);
      });

      SetOutputs(dataAccess);
      Helpers.PostHog.Result(gsaResult.Type, 0, GsaResultsValues.ResultType.Force, _selectedDisplayValue.ToString());
    }

    #region Custom UI
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
      {
        "Component"
      });

      DropDownItems = new List<List<string>> { _reactionStringList };
      SelectedItems = new List<string> { DropDownItems[0][3] };

      IsInitialised = true;
    }

    public override void CreateAttributes() {
      if (!IsInitialised) {
        InitialiseDropdowns();
      }
      m_attributes = new OasysGH.UI.DropDownComponentAttributes(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }

    public override void SetSelected(int i, int j) {
      _selectedDisplayValue = (DisplayValue)j;
      SelectedItems[i] = DropDownItems[i][j];
      base.UpdateUI();
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetInt32("Display", (int)_selectedDisplayValue);
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _selectedDisplayValue = (DisplayValue)reader.GetInt32("Display");
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      return base.Read(reader);
    }
    #endregion

    #region custom preview
    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      foreach (KeyValuePair<int, VectorResultGoo> force in _reactionForceVectors) {
        force.Value.ShowText(_showText);
      }
    }

    #endregion

    #region menu override
    protected override void BeforeSolveInstance() {
      Message = (int)_selectedDisplayValue < 4
        ? Force.GetAbbreviation(_forceUnit)
        : Moment.GetAbbreviation(_momentUnit);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Text", ShowText, true, _showText);

      var unitsMenu = new ToolStripMenuItem("Select Units", Properties.Resources.Units);
      ToolStripMenuItem forceUnitsMenu = GenerateForceUnitsMenu("Force");
      ToolStripMenuItem momentUnitsMenu = GenerateMomentUnitsMenu("Moment");

      var toolStripItems = new List<ToolStripItem> { forceUnitsMenu, momentUnitsMenu };

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
      var forceUnitsMenu = new ToolStripMenuItem(menuTitle) { Enabled = true };

      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force).Select(unit => new ToolStripMenuItem(
                 unit,
                 null,
                 (s, e) => { UpdateForce(unit); }
               ) {
        Checked = unit == Force.GetAbbreviation(_forceUnit),
        Enabled = true,
      })) {
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return forceUnitsMenu;
    }

    private ToolStripMenuItem GenerateMomentUnitsMenu(string menuTitle) {
      var momentUnitsMenu = new ToolStripMenuItem(menuTitle) { Enabled = true };
      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment).Select(unit => new ToolStripMenuItem(
                 unit,
                 null,
                 (s, e) => { UpdateMoment(unit); }
               ) {
        Checked = unit == Moment.GetAbbreviation(_momentUnit),
        Enabled = true,
      })) {
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return momentUnitsMenu;
    }

    private ToolStripMenuItem GenerateModelGeometryUnitsMenu(string menuTitle) {
      var modelUnitsMenu = new ToolStripMenuItem(menuTitle) { Enabled = true };
      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length).Select(unit => new ToolStripMenuItem(unit, null, (s, e) => { UpdateModel(unit); }) {
        Checked = unit == Length.GetAbbreviation(_lengthUnit),
        Enabled = true,
      })) {
        modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return modelUnitsMenu;
    }
    #endregion

    #region Inputs ReactionForceVector methods
    private static double GetScalarValue(IGH_DataAccess dataAccess) {
      var ghScale = new GH_Number();
      dataAccess.GetData(2, ref ghScale);
      return GH_Convert.ToDouble(ghScale, out double scale, GH_Conversion.Both) ? scale : 0.0d;
    }

    private static string GetNodeFilters(IGH_DataAccess dataAccess) {
      string nodeList = string.Empty;
      var ghNoList = new GH_String();
      if (dataAccess.GetData(1, ref ghNoList)) {
        nodeList = GH_Convert.ToString(ghNoList, out string tempNodeList, GH_Conversion.Both) ? tempNodeList : string.Empty;
      }

      if (nodeList.ToLower() == "all" || string.IsNullOrEmpty(nodeList))
        nodeList = "All";

      return nodeList;
    }
    #endregion

    #region solvingInstance helpers
    private bool IsGhObjectValid(GH_ObjectWrapper ghObject) {
      bool valid = false;
      if (ghObject?.Value == null) {
        this.AddRuntimeWarning("Input is null");
      }
      else if (!(ghObject.Value is GsaResultGoo)) {
        this.AddRuntimeError("Error converting input to GSA Result");
      }
      else {
        valid = true;
      }

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
                            + lengthUnit
                            + ". This can be changed by right-clicking the component -> 'Select Units'"
      );

      return lengthUnit;
    }

    private VectorResultGoo GenerateReactionForceVector(KeyValuePair<int, GsaNodeGoo> node, GsaResultsValues forceValues, double scale) {
      int nodeId = node.Key;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults = forceValues.xyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults = forceValues.xxyyzzResults;

      if (!xyzResults.ContainsKey(nodeId))
        return null;

      var direction = new Vector3d();
      IQuantity forceValue = null;
      bool isForce = true;

      switch (_selectedDisplayValue) {
        case (DisplayValue.X):
          IQuantity xVal = xyzResults[nodeId][0].X;
          direction = new Vector3d(xVal.As(_forceUnit) * scale, 0, 0);
          forceValue = xVal.ToUnit(_forceUnit);
          break;
        case (DisplayValue.Y):
          IQuantity yVal = xyzResults[nodeId][0].Y;
          direction = new Vector3d(0, yVal.As(_forceUnit) * scale, 0);
          forceValue = yVal.ToUnit(_forceUnit);
          break;
        case (DisplayValue.Z):
          IQuantity zVal = xyzResults[nodeId][0].Z;
          direction = new Vector3d(0, 0, zVal.As(_forceUnit) * scale);
          forceValue = zVal.ToUnit(_forceUnit);
          break;
        case (DisplayValue.ResXYZ):
          direction = new Vector3d(
            xyzResults[nodeId][0].X.As(_forceUnit) * scale,
            xyzResults[nodeId][0].Y.As(_forceUnit) * scale,
            xyzResults[nodeId][0].Z.As(_forceUnit) * scale);
          forceValue = xyzResults[nodeId][0].XYZ.ToUnit(_forceUnit);
          break;
        case (DisplayValue.XX):
          isForce = false;
          IQuantity xxVal = xxyyzzResults[nodeId][0].X;
          direction = new Vector3d(xxVal.As(_momentUnit) * scale, 0, 0);
          forceValue = xxVal.ToUnit(_momentUnit);
          break;
        case (DisplayValue.YY):
          isForce = false;
          IQuantity yyVal = xxyyzzResults[nodeId][0].Y;
          direction = new Vector3d(0, yyVal.As(_momentUnit) * scale, 0);
          forceValue = yyVal.ToUnit(_momentUnit);
          break;
        case (DisplayValue.ZZ):
          isForce = false;
          IQuantity zzVal = xxyyzzResults[nodeId][0].Z;
          direction = new Vector3d(0, 0, zzVal.As(_momentUnit) * scale);
          forceValue = zzVal.ToUnit(_momentUnit);
          break;
        case (DisplayValue.ResXXYYZZ):
          isForce = false;
          direction = new Vector3d(
            xxyyzzResults[nodeId][0].X.As(_momentUnit) * scale,
            xxyyzzResults[nodeId][0].Y.As(_momentUnit) * scale,
            xxyyzzResults[nodeId][0].Z.As(_momentUnit) * scale);
          forceValue = xxyyzzResults[nodeId][0].XYZ.ToUnit(_momentUnit);
          break;
      }

      var vectorResult = new VectorResultGoo(node.Value.Value.Point, direction, forceValue);

      if (isForce)
        return vectorResult;

      return vectorResult.SetColor(Helpers.Graphics.Colours.GsaGold)
          .DrawArrowHead(true);
    }

    private void SetOutputs(IGH_DataAccess dataAccess) {
      IOrderedEnumerable<KeyValuePair<int, VectorResultGoo>> orderedDict = _reactionForceVectors.OrderBy(index => index.Key);
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
