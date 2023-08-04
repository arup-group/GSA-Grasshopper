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
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to display GSA reaction forces
  /// </summary>
  public class ReactionForceDiagrams_OBSOLETE : GH_OasysDropDownComponent {
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

    public override Guid ComponentGuid => new Guid("5bc139e5-614b-4f2d-887c-a980f1cbb32c");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ReactionForceDiagram;
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
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private ConcurrentDictionary<int, (VectorDiagram, IQuantity)> _reactionForceVectors
      = new ConcurrentDictionary<int, (VectorDiagram, IQuantity)>();

    private DisplayValue _selectedDisplayValue = DisplayValue.ResXyz;
    private bool _showText = true;

    public ReactionForceDiagrams_OBSOLETE() : base("Reaction Force Diagrams", "ReactionForce",
      "Diplays GSA Node Reaction Force Results as Vector Diagrams", CategoryName.Name(),
      SubCategoryName.Cat5()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, _spacerDescriptions);
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

    public override void SetSelected(int i, int j) {
      _selectedDisplayValue = (DisplayValue)j;
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Display", (int)_selectedDisplayValue);
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Show Text", ShowText, true, _showText);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.Units);
      ToolStripMenuItem forceUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Force",
        EngineeringUnits.Force, Force.GetAbbreviation(_forceUnit), UpdateForce);
      ToolStripMenuItem momentUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Moment",
        EngineeringUnits.Moment, Moment.GetAbbreviation(_momentUnit), UpdateMoment);

      var toolStripItems = new List<ToolStripItem> {
        forceUnitsMenu,
        momentUnitsMenu,
      };

      if (_lengthUnit == LengthUnit.Undefined) {
        ToolStripMenuItem modelUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem(
          "Model geometry", EngineeringUnits.Length, Length.GetAbbreviation(_lengthUnit),
          UpdateModel);
        toolStripItems.Insert(0, modelUnitsMenu);
      }

      unitsMenu.DropDownItems.AddRange(toolStripItems.ToArray());
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);
      Menu_AppendSeparator(menu);
    }

    protected override void BeforeSolveInstance() {
      Message = (int)_selectedDisplayValue < 4 ? Force.GetAbbreviation(_forceUnit) :
        Moment.GetAbbreviation(_momentUnit);
    }

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

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Node filter list", "No",
        "Filter results by list (by default 'all')" + Environment.NewLine
        + "Input a GSA List or a text string taking the form:" + Environment.NewLine
        + " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager.AddNumberParameter("Scalar", "x:X",
        "Scale the result vectors to a specific size. If left empty, automatic scaling based on model size and maximum result by load cases will be computed.",
        GH_ParamAccess.item);
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPointParameter("Anchor Point", "A", "Support Node Location", GH_ParamAccess.list);
      pManager.AddGenericParameter("Vector", "V", "Reaction Force Vector", GH_ParamAccess.list);
      pManager.AddGenericParameter("Value", "Val", "Reaction Force Value", GH_ParamAccess.list);
      pManager.HideParameter(0);
    }

    protected override void SolveInstance(IGH_DataAccess dataAccess) {
      var gsaResult = new GsaResult();
      var ghObject = new GH_ObjectWrapper();

      if (!dataAccess.GetData(0, ref ghObject) || !IsGhObjectValid(ghObject)) {
        return;
      }

      gsaResult = (ghObject.Value as GsaResultGoo).Value;
      string filteredNodes = Inputs.GetNodeListNameForesults(this, dataAccess, 1);
      if (string.IsNullOrEmpty(filteredNodes)) {
        return;
      }

      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues
        = gsaResult.NodeReactionForceValues(filteredNodes, _forceUnit, _momentUnit);
      GsaResultsValues forceValues = reactionForceValues.Item1[0];
      filteredNodes = string.Join(" ", reactionForceValues.Item2);

      LengthUnit lengthUnit = GetLengthUnit(gsaResult);

      ReadOnlyDictionary<int, Node> gsaFilteredNodes = gsaResult.Model.Model.Nodes(filteredNodes);
      ConcurrentDictionary<int, GsaNodeGoo> nodes = Nodes.GetNodeDictionary(gsaFilteredNodes,
        lengthUnit, gsaResult.Model.Model.Axes());

      double scale = 1;
      if (!dataAccess.GetData(2, ref scale)) {
        scale = ComputeAutoScale(forceValues, gsaResult.Model.BoundingBox);
      }

      _reactionForceVectors = new ConcurrentDictionary<int, (VectorDiagram, IQuantity)>();
      Parallel.ForEach(nodes, node => {
        (VectorDiagram reactionForceVector, IQuantity forceValue)
          = CreateReactionForceVector(node, forceValues, scale);
        if (reactionForceVector != null) {
          _reactionForceVectors.TryAdd(node.Key, (reactionForceVector, forceValue));
        }
      });

      SetOutputs(dataAccess);
      PostHog.Result(gsaResult.Type, 0, GsaResultsValues.ResultType.Force,
        _selectedDisplayValue.ToString());
    }

    private double ComputeAutoScale(GsaResultsValues forceValues, BoundingBox bbox) {
      double maxValue = 0;
      switch (_selectedDisplayValue) {
        case DisplayValue.X:
          maxValue = Math.Max(forceValues.DmaxX.As(_forceUnit),
            Math.Abs(forceValues.DminX.As(_forceUnit)));
          break;
        case DisplayValue.Y:
          maxValue = Math.Max(forceValues.DmaxY.As(_forceUnit),
            Math.Abs(forceValues.DminY.As(_forceUnit)));
          break;
        case DisplayValue.Z:
          maxValue = Math.Max(forceValues.DmaxZ.As(_forceUnit),
            Math.Abs(forceValues.DminZ.As(_forceUnit)));
          break;
        case DisplayValue.ResXyz:
          maxValue = Math.Max(forceValues.DmaxXyz.As(_forceUnit),
            Math.Abs(forceValues.DminXyz.As(_forceUnit)));
          break;

        case DisplayValue.Xx:
          maxValue = Math.Max(forceValues.DmaxXx.As(_momentUnit),
            Math.Abs(forceValues.DminXx.As(_momentUnit)));
          break;
        case DisplayValue.Yy:
          maxValue = Math.Max(forceValues.DmaxYy.As(_momentUnit),
            Math.Abs(forceValues.DminYy.As(_momentUnit)));
          break;
        case DisplayValue.Zz:
          maxValue = Math.Max(forceValues.DmaxZz.As(_momentUnit),
            Math.Abs(forceValues.DminZz.As(_momentUnit)));
          break;
        case DisplayValue.ResXxyyzz:
          maxValue = Math.Max(forceValues.DmaxXxyyzz.As(_momentUnit),
            Math.Abs(forceValues.DminXxyyzz.As(_momentUnit)));
          break;
      }

      double factor = 0.1; // maxVector = 10% of bbox diagonal
      return bbox.Diagonal.Length * factor / maxValue;
    }

    private (VectorDiagram diagram, IQuantity quantity) CreateReactionForceVector(
      KeyValuePair<int, GsaNodeGoo> node, GsaResultsValues forceValues, double scale) {
      int nodeId = node.Key;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xyzResults
        = forceValues.XyzResults;
      ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>> xxyyzzResults
        = forceValues.XxyyzzResults;

      if (!xyzResults.ContainsKey(nodeId)) {
        return (null, null);
      }

      bool isForce = (int)_selectedDisplayValue < 4;
      GsaResultQuantity quantity;
      Enum unit;

      if (isForce) {
        quantity = xyzResults[nodeId][0];
        unit = _forceUnit;
      } else {
        quantity = xxyyzzResults[nodeId][0];
        unit = _momentUnit;
      }

      var direction = new Vector3d();
      IQuantity forceValue = null;

      Vector3d xAxis = node.Value.Value.IsGlobalAxis() ? Vector3d.XAxis :
        node.Value.Value.LocalAxis.XAxis;
      xAxis.Unitize();
      Vector3d yAxis = node.Value.Value.IsGlobalAxis() ? Vector3d.YAxis :
        node.Value.Value.LocalAxis.YAxis;
      yAxis.Unitize();
      Vector3d zAxis = node.Value.Value.IsGlobalAxis() ? Vector3d.ZAxis :
        node.Value.Value.LocalAxis.ZAxis;
      zAxis.Unitize();

      xAxis *= quantity.X.As(unit) * scale;
      yAxis *= quantity.Y.As(unit) * scale;
      zAxis *= quantity.Z.As(unit) * scale;

      switch (_selectedDisplayValue) {
        case DisplayValue.X:
        case DisplayValue.Xx:
          direction = xAxis;
          forceValue = quantity.X.ToUnit(unit);
          break;

        case DisplayValue.Y:
        case DisplayValue.Yy:
          direction = yAxis;
          forceValue = quantity.Y.ToUnit(unit);
          break;

        case DisplayValue.Z:
        case DisplayValue.Zz:
          direction = zAxis;
          forceValue = quantity.Z.ToUnit(unit);
          break;

        case DisplayValue.ResXyz:
        case DisplayValue.ResXxyyzz:
          direction = xAxis + yAxis + zAxis;
          forceValue = quantity.Xyz.ToUnit(unit);
          break;
      }

      return (new VectorDiagram(node.Value.Value.Point, direction, !isForce, Color.Empty), forceValue);
    }

    private LengthUnit GetLengthUnit(GsaResult gsaResult) {
      LengthUnit lengthUnit = gsaResult.Model.ModelUnit;
      bool isUndefined = lengthUnit == LengthUnit.Undefined;

      if (!isUndefined) {
        return lengthUnit;
      }

      lengthUnit = _lengthUnit;
      this.AddRuntimeRemark("Model came straight out of GSA and we couldn't read the units. "
        + "The geometry has been scaled to be in " + lengthUnit.ToString()
        + ". This can be changed by right-clicking the component -> 'Select Units'");

      return lengthUnit;
    }

    private bool IsGhObjectValid(GH_ObjectWrapper ghObject) {
      bool valid = false;
      if (ghObject == null || ghObject.Value == null) {
        this.AddRuntimeWarning("Input is null");
      } else if (!(ghObject.Value is GsaResultGoo)) {
        this.AddRuntimeError("Error converting input to GSA Result");
      } else {
        valid = true;
      }

      return valid;
    }

    private void SetOutputs(IGH_DataAccess dataAccess) {
      IOrderedEnumerable<KeyValuePair<int, (VectorDiagram, IQuantity)>> orderedDict
        = _reactionForceVectors.OrderBy(index => index.Key);
      var startingPoints = new List<Point3d>();
      var vectors = new List<IGsaDiagram>();
      var forces = new List<IQuantity>();

      foreach (KeyValuePair<int, (VectorDiagram diagram, IQuantity force)> keyValuePair in
        orderedDict) {
        startingPoints.Add(keyValuePair.Value.diagram.AnchorPoint);
        vectors.Add(keyValuePair.Value.diagram);
        forces.Add(keyValuePair.Value.force);
      }

      dataAccess.SetDataList(0, startingPoints);
      dataAccess.SetDataList(1, vectors);
      dataAccess.SetDataList(2, forces);
    }

    private void ShowText(object sender, EventArgs e) {
      _showText = !_showText;
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateModel(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateMoment(string unit) {
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }
  }
}
