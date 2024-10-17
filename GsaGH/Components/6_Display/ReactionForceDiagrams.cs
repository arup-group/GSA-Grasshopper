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
using GsaGH.Parameters.Results;
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

    private static Point3d GenerateAnnotationPosition(GsaVectorDiagram vector) {
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

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);
      GsaResult result = Inputs.GetResultInput(this, ghTyp);
      if (result == null) {
        return;
      }

      string nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);

      ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> forceValues
        = result.NodeReactionForces.ResultSubset(nodeIds);
      nodeList = string.Join(" ", forceValues.Ids);
      LengthUnit lengthUnit = GetLengthUnit(result);

      int permutation = result.SelectedPermutationIds == null
        ? 0 : result.SelectedPermutationIds[0] - 1;

      ReadOnlyDictionary<int, Node> gsaFilteredNodes = result.Model.ApiModel.Nodes(nodeList);
      ConcurrentDictionary<int, GsaNodeGoo> nodes = Nodes.GetNodeDictionary(gsaFilteredNodes,
        lengthUnit, result.Model.ApiModel.Axes());

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
              node, forceValues.Subset, permutation, scale, significantDigits, color);
        if (reactionForceVector == null) {
          return;
        }

        reactionForceVectors.TryAdd(node.Key, reactionForceVector);
        annotations.TryAdd(node.Key, annotation);

        ((IGH_PreviewObject)Params.Output[1]).Hidden = !_showText;
      });

      SetOutputs(da, reactionForceVectors, annotations);
      PostHog.Diagram("Result", result.CaseType, "ReactionForce", _selectedDisplayValue.ToString(), Parameters.EntityType.Node);
    }

    private double ComputeAutoScale(IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> forceValues, BoundingBox bbox) {
      double? max = 0;
      double? min = 0;
      switch (_selectedDisplayValue) {
        case DisplayValue.X:
          max = forceValues.GetExtrema(forceValues.Max.X).XAs(_forceUnit);
          min = forceValues.GetExtrema(forceValues.Min.X).XAs(_forceUnit);
          break;
        case DisplayValue.Y:
          max = forceValues.GetExtrema(forceValues.Max.Y).YAs(_forceUnit);
          min = forceValues.GetExtrema(forceValues.Min.Y).YAs(_forceUnit);
          break;
        case DisplayValue.Z:
          max = forceValues.GetExtrema(forceValues.Max.Z).ZAs(_forceUnit);
          min = forceValues.GetExtrema(forceValues.Min.Z).ZAs(_forceUnit);
          break;
        case DisplayValue.ResXyz:
          max = forceValues.GetExtrema(forceValues.Max.Xyz).XyzAs(_forceUnit);
          min = forceValues.GetExtrema(forceValues.Min.Xyz).XyzAs(_forceUnit);
          break;

        case DisplayValue.Xx:
          max = forceValues.GetExtrema(forceValues.Max.Xx).XxAs(_momentUnit);
          min = forceValues.GetExtrema(forceValues.Min.Xx).XxAs(_momentUnit);
          break;
        case DisplayValue.Yy:
          max = forceValues.GetExtrema(forceValues.Max.Yy).YyAs(_momentUnit);
          min = forceValues.GetExtrema(forceValues.Min.Yy).YyAs(_momentUnit);
          break;
        case DisplayValue.Zz:
          max = forceValues.GetExtrema(forceValues.Max.Zz).ZzAs(_momentUnit);
          min = forceValues.GetExtrema(forceValues.Min.Zz).ZzAs(_momentUnit);
          break;
        case DisplayValue.ResXxyyzz:
          max = forceValues.GetExtrema(forceValues.Max.Xxyyzz).XxyyzzAs(_momentUnit);
          min = forceValues.GetExtrema(forceValues.Min.Xxyyzz).XxyyzzAs(_momentUnit);
          break;
      }

      max ??= 0;
      min ??= 0;

      double maxValue = Math.Max((double)max, Math.Abs((double)min));
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
      KeyValuePair<int, GsaNodeGoo> node, IDictionary<int, IList<IReactionForce>> forceValues,
      int permutation, double scale, int significantDigits, Color color) {
      int nodeId = node.Key;

      if (!forceValues.ContainsKey(nodeId)) {
        return (null, null);
      }

      IReactionForce result = forceValues[nodeId][permutation];
      Vector3d? direction = new Vector3d();
      IQuantity forceValue = null;

      switch (_selectedDisplayValue) {
        case DisplayValue.X:
          direction = Vector3d.XAxis * result.XAs(_forceUnit) * scale;
          forceValue = result.XToUnit(_forceUnit);
          break;

        case DisplayValue.Y:
          direction = Vector3d.YAxis * result.YAs(_forceUnit) * scale;
          forceValue = result.YToUnit(_forceUnit);
          break;

        case DisplayValue.Z:
          direction = Vector3d.ZAxis * result.ZAs(_forceUnit) * scale;
          forceValue = result.ZToUnit(_forceUnit);
          break;

        case DisplayValue.ResXyz:
          Vector3d? xAxis = Vector3d.XAxis * result.XAs(_forceUnit) * scale;
          Vector3d? yAxis = Vector3d.YAxis * result.YAs(_forceUnit) * scale;
          Vector3d? zAxis = Vector3d.ZAxis * result.ZAs(_forceUnit) * scale;
          direction = xAxis + yAxis + zAxis;
          forceValue = result.XyzToUnit(_forceUnit);
          break;

        case DisplayValue.Xx:
          direction = Vector3d.XAxis * result.XxAs(_momentUnit) * scale;
          forceValue = result.XxToUnit(_momentUnit);
          break;

        case DisplayValue.Yy:
          direction = Vector3d.YAxis * result.YyAs(_momentUnit) * scale;
          forceValue = result.YyToUnit(_momentUnit);
          break;

        case DisplayValue.Zz:
          direction = Vector3d.ZAxis * result.ZzAs(_momentUnit) * scale;
          forceValue = result.ZzToUnit(_momentUnit);
          break;

        case DisplayValue.ResXxyyzz:
          Vector3d? xxAxis = Vector3d.XAxis * result.XxAs(_momentUnit) * scale;
          Vector3d? yyAxis = Vector3d.YAxis * result.YyAs(_momentUnit) * scale;
          Vector3d? zzAxis = Vector3d.ZAxis * result.ZzAs(_momentUnit) * scale;
          direction = xxAxis + yyAxis + zzAxis;
          forceValue = result.XxyyzzToUnit(_momentUnit);
          break;
      }

      if (direction == null) {
        direction = Vector3d.Zero;
      }

      bool isForce = (int)_selectedDisplayValue < 4;
      var vectorResult = new GsaVectorDiagram(node.Value.Value.Point, (Vector3d)direction, !isForce, color);

      string text = string.Empty;
      if (forceValue != null) {
        text = $"{Math.Round(forceValue.Value, significantDigits)} {Message}";
      }

      var annotation = new GsaAnnotationGoo(new GsaAnnotationDot(
        GenerateAnnotationPosition(vectorResult), vectorResult.Color, text));

      return (vectorResult, annotation);
    }

    private LengthUnit GetLengthUnit(GsaResult gsaResult) {
      return gsaResult.Model.ModelUnit;
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
