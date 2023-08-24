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
using Rhino.Commands;
using Rhino.Geometry;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to display GSA reaction forces
  /// </summary>
  public class ReactionForceDiagrams : GH_OasysDropDownComponent {
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

    public override Guid ComponentGuid => new Guid("2c9d902f-931f-4d42-904e-ea1f2448aadb");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ReactionForceDiagrams;
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
    private DisplayValue _selectedDisplayValue = DisplayValue.ResXyz;

    public ReactionForceDiagrams() : base("Reaction Force Diagrams", "ReactionForce",
      "Diplays GSA Node Reaction Force Results as Vector Diagrams", CategoryName.Name(),
      SubCategoryName.Cat6()) { }

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

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
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
      pManager.AddParameter(new GsaNodeListParameter());
      pManager.AddBooleanParameter("Annotation", "A", "Show Annotation", GH_ParamAccess.item,
        false);
      pManager.AddIntegerParameter("Significant Digits", "SD", "Round values to significant digits",
        GH_ParamAccess.item, 3);
      pManager.AddColourParameter("Colour", "Co", "[Optional] Colour to override default colour",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Scalar", "x:X",
        "Scale the result vectors to a specific size. If left empty, automatic scaling based on model size and maximum result by load cases will be computed.",
        GH_ParamAccess.item);

      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaDiagramParameter(), "Diagram lines", "Dgm", "Vectors of the GSA Result Diagram",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaAnnotationParameter(), "Annotations", 
        "An", "Annotations for the diagram", GH_ParamAccess.list);
      pManager.HideParameter(1);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      var ghObject = new GH_ObjectWrapper();

      if (!da.GetData(0, ref ghObject) || !IsGhObjectValid(ghObject)) {
        return;
      }

      result = (ghObject.Value as GsaResultGoo).Value;
      string nodeList = Inputs.GetNodeListNameForResults(this, da, 1, result.Model);

      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues
        = result.NodeReactionForceValues(nodeList, _forceUnit, _momentUnit);
      GsaResultsValues forceValues = reactionForceValues.Item1[0];
      nodeList = string.Join(" ", reactionForceValues.Item2);

      LengthUnit lengthUnit = GetLengthUnit(result);

      ReadOnlyDictionary<int, Node> gsaFilteredNodes = result.Model.Model.Nodes(nodeList);
      ConcurrentDictionary<int, GsaNodeGoo> nodes = Nodes.GetNodeDictionary(gsaFilteredNodes,
        lengthUnit, result.Model.Model.Axes());

      double scale = 1;
      if (!da.GetData(5, ref scale)) {
        scale = ComputeAutoScale(forceValues, result.Model.BoundingBox);
      }

      Color color = Color.Empty;
      da.GetData(4, ref color);

      int significantDigits = 3;
      bool _showText = true;

      da.GetData(2, ref _showText);
      da.GetData(3, ref significantDigits);

      var reactionForceVectors = new ConcurrentDictionary<int, GsaVectorDiagram>();
      var annotations = new ConcurrentDictionary<int, GsaAnnotationGoo>();
      Parallel.ForEach(nodes, node => {
        (GsaVectorDiagram reactionForceVector, GsaAnnotationGoo annotation) 
          = CreateReactionForceVectorWithAnnotations(
              node, forceValues, scale, significantDigits, color);
        if (reactionForceVector == null) {
          return;
        }

        reactionForceVectors.TryAdd(node.Key, reactionForceVector);
        annotations.TryAdd(node.Key, annotation);

        ((IGH_PreviewObject)Params.Output[1]).Hidden = !_showText;
      });

      SetOutputs(da, reactionForceVectors, annotations);
      PostHog.Result(result.Type, 0, GsaResultsValues.ResultType.Force,
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

      ;

      double factor = 0.1; // maxVector = 10% of bbox diagonal
      return bbox.Diagonal.Length * factor / maxValue;
    }

    private ToolStripMenuItem GenerateForceUnitsMenu(string menuTitle) {
      var forceUnitsMenu = new ToolStripMenuItem(menuTitle) {
        Enabled = true,
      };

      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper
       .GetFilteredAbbreviations(EngineeringUnits.Force).Select(unit
          => new ToolStripMenuItem(unit, null, (s, e) => UpdateForce(unit)) {
            Checked = unit == Force.GetAbbreviation(_forceUnit),
            Enabled = true,
          })) {
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return forceUnitsMenu;
    }

    private ToolStripMenuItem GenerateModelGeometryUnitsMenu(string menuTitle) {
      var modelUnitsMenu = new ToolStripMenuItem(menuTitle) {
        Enabled = true,
      };
      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper
       .GetFilteredAbbreviations(EngineeringUnits.Length).Select(unit
          => new ToolStripMenuItem(unit, null, (s, e) => UpdateModel(unit)) {
            Checked = unit == Length.GetAbbreviation(_lengthUnit),
            Enabled = true,
          })) {
        modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return modelUnitsMenu;
    }

    private ToolStripMenuItem GenerateMomentUnitsMenu(string menuTitle) {
      var momentUnitsMenu = new ToolStripMenuItem(menuTitle) {
        Enabled = true,
      };
      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper
       .GetFilteredAbbreviations(EngineeringUnits.Moment).Select(unit
          => new ToolStripMenuItem(unit, null, (s, e) => UpdateMoment(unit)) {
            Checked = unit == Moment.GetAbbreviation(_momentUnit),
            Enabled = true,
          })) {
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      return momentUnitsMenu;
    }

    private (GsaVectorDiagram, GsaAnnotationGoo) CreateReactionForceVectorWithAnnotations(
      KeyValuePair<int, GsaNodeGoo> node, GsaResultsValues forceValues, double scale,
      int significantDigits, Color color) {
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

      Vector3d xAxis = Vector3d.XAxis;
      Vector3d yAxis = Vector3d.YAxis;
      Vector3d zAxis = Vector3d.ZAxis;

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

      var vectorResult = new GsaVectorDiagram(node.Value.Value.Point, direction, !isForce, color);

      var annotation = new GsaAnnotationGoo(new GsaAnnotationDot(
        GenerateAnnotationPosition(vectorResult), vectorResult.Color, 
        $"{Math.Round(forceValue.Value, significantDigits)} {Message}"));

      return (vectorResult, annotation);
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

    private void SetOutputs(
      IGH_DataAccess dataAccess, ConcurrentDictionary<int, GsaVectorDiagram> reactionForceVectors,
      ConcurrentDictionary<int, GsaAnnotationGoo> annotations) {
      IOrderedEnumerable<KeyValuePair<int, GsaVectorDiagram>> orderedDict
        = reactionForceVectors.OrderBy(index => index.Key);
      IOrderedEnumerable<KeyValuePair<int, GsaAnnotationGoo>> orderedDict2
        = annotations.OrderBy(index => index.Key);
      var vectors = new List<GsaDiagramGoo>();
      var annos = new List<GsaAnnotationGoo>();

      foreach (KeyValuePair<int, GsaVectorDiagram> keyValuePair in orderedDict) {
        vectors.Add(new GsaDiagramGoo(keyValuePair.Value));
      }
      foreach (KeyValuePair<int, GsaAnnotationGoo> keyValuePair in orderedDict2) {
        annos.Add(keyValuePair.Value);
      }

      dataAccess.SetDataList(0, vectors);
      dataAccess.SetDataList(1, annos);
    }

    private Point3d GenerateAnnotationPosition(GsaVectorDiagram vector) {
      var line = new Line(vector.AnchorPoint, vector.Direction);
      line.Flip();
      line.Transform(Transform.Scale(vector.AnchorPoint, -1));
      Point3d endPoint = line.From;

      int _pixelsPerUnit = 100;
      int offset = 30;
      Vector3d direction = line.Direction;

      direction.Unitize();
      var t = Transform.Translation(direction * -1 * offset / _pixelsPerUnit);
      endPoint.Transform(t);

      return endPoint;
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
