using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.GsaApi.Grahics;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using Rhino.Geometry;

using DiagramType = GsaAPI.DiagramType;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = GsaAPI.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Load Diagram
  /// </summary>
  public class LoadDiagrams : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("5ea823af-a567-40a6-8e82-0e14eb8dda0e");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.LoadDiagrams;
    private string _caseId = "L1";
    private List<string> _2dDiagramTypes;
    private List<string> _3dDiagramTypes;
    private List<string> _beamDiagramTypes;
    private List<string> _gridDiagramTypes;
    private List<string> _nodalDiagramTypes;
    private GsaModel _gsaModel;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private List<string> _loadTypes = new List<string>() {
      "All",
      "Nodal",
      "Beam",
      "2D",
      "3D",
      "Grid",
    };
    private Guid _modelGuid = new Guid();

    public LoadDiagrams() : base("Load Diagrams", "LoadDiagram", "Displays GSA Load Diagram",
      CategoryName.Name(), SubCategoryName.Cat6()) { }

    public override bool Read(GH_IReader reader) {
      //warning - sensitive for description string! do not change description if not needed!
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      switch (i) {
        case 0: { // Cases
            if (_selectedItems[i].ToLower() == "all") {
              _caseId = "all";
            } else if (!_selectedItems[i].Equals(_caseId)) {
              _caseId = _selectedItems[i];
            }

            break;
          }
        case 1: { // Load Type
            if (_selectedItems[i] == "All") {
              if (_dropDownItems.Count == 3) {
                _dropDownItems.RemoveAt(2);
                _selectedItems.RemoveAt(2);
              }

              _selectedItems[1] = "All";
              break;
            }

            int k = 2;
            if (_dropDownItems.Count == 2) {
              _dropDownItems.Add(new List<string>());
              _selectedItems.Add(string.Empty);
            }

            switch (_selectedItems[i]) {
              case "Grid": {
                  _dropDownItems[k] = _gridDiagramTypes;
                  break;
                }
              case "Nodal": {
                  _dropDownItems[k] = _nodalDiagramTypes;
                  break;
                }
              case "Beam": {
                  _dropDownItems[k] = _beamDiagramTypes;
                  break;
                }
              case "2D": {
                  _dropDownItems[k] = _2dDiagramTypes;
                  break;
                }
              case "3D": {
                  _dropDownItems[k] = _3dDiagramTypes;
                  break;
                }
              default: break;
            }

            _selectedItems[k] = _dropDownItems[k][0];

            break;
          }
      }

      base.UpdateUI();
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      ToolStripMenuItem forceUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Force",
        EngineeringUnits.Force, Force.GetAbbreviation(_forceUnit), UpdateForce);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        forceUnitsMenu,
      });

      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
      menu.Items.Add(unitsMenu);
      Menu_AppendSeparator(menu);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Case ID",
        "Load Type",
        "Diagram Type",
      });

      PreparedDropdownLists();
      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      //cases
      _dropDownItems.Add(new List<string>() {
        string.Empty,
      });
      _selectedItems.Add(string.Empty);

      //Load Types
      _dropDownItems.Add(_loadTypes);
      _selectedItems.Add(_dropDownItems[1][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA model", "GSA",
        "GSA model containing some Analysis Cases and Tasks", GH_ParamAccess.item);
      pManager.AddGenericParameter("Case filter list", "C",
        $"Filter import by list.{Environment.NewLine}The case list should take the form:" +
        $"{Environment.NewLine} 1 L1 M1 A1 C1 C2p1 A3 to A5 T1.", GH_ParamAccess.item);
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager.AddBooleanParameter("Annotation", "A", "Show Annotation", GH_ParamAccess.item,
        false);
      pManager.AddIntegerParameter("Significant Digits", "SD", "Round values to significant digits",
        GH_ParamAccess.item, 3);
      pManager.AddColourParameter("Colour", "Co", "[Optional] Colour to override default colour",
        GH_ParamAccess.item);
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size",
        GH_ParamAccess.item);

      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaDiagramParameter(), "Diagram lines", "Dgm",
        "Lines and arrowheads of the GSA Load Diagram", GH_ParamAccess.list);
      pManager.AddParameter(new GsaAnnotationParameter(), "Annotations",
        "An", "Annotations for the diagram", GH_ParamAccess.list);
      pManager.HideParameter(1);
    }

    protected override void BeforeSolveInstance() {
      Message = Force.GetAbbreviation(_forceUnit);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      if (!da.GetData(0, ref modelGoo) || !IsGhObjectValid(modelGoo)) {
        _gsaModel = null;
        return;
      }

      _gsaModel = modelGoo.Value;
      if (_gsaModel.Guid != _modelGuid) {
        UpdateCaseDropdown();
        _modelGuid = _gsaModel.Guid;
      }

      string caseList = string.Empty;
      if (da.GetData(1, ref caseList)) {
        _selectedItems[0] = caseList;
        _caseId = caseList;
      }

      EntityList list = Inputs.GetElementOrMemberList(this, da, 2);

      var ghScale = new GH_Number();
      double scale = 1;
      bool autoScale = true;
      if (da.GetData(6, ref ghScale)) {
        GH_Convert.ToDouble(ghScale, out scale, GH_Conversion.Both);
        autoScale = false;
      }

      bool showAnnotations = true;
      da.GetData(3, ref showAnnotations);
      ((IGH_PreviewObject)Params.Output[1]).Hidden = !showAnnotations;
      int significantDigits = 3;
      da.GetData(4, ref significantDigits);

      Color color = Color.Empty;
      da.GetData(5, ref color);

      double unitScale = ComputeUnitScale(autoScale);
      double unitScaleFactor = UnitConverter.Convert(1, Force.BaseUnit, _forceUnit);
      double computedScale
        = GraphicsScalar.ComputeScale(_gsaModel, scale, autoScale, unitScale);
      var diagramLines = new ConcurrentBag<GsaDiagramGoo>();
      var diagramAnnotations = new ConcurrentBag<GsaAnnotationGoo>();

      double arrowScale = _gsaModel.BoundingBox.Diagonal.Length *
      (autoScale ? 0.00025 : computedScale);

      List<DiagramType> types = GetSelectedDiagramTypes();
      Parallel.ForEach(types, type => {
        var graphic = new DiagramSpecification() {
          ListDefinition = list.Definition,
          ListType = list.Type,
          Type = type,
          Cases = _caseId,
          ScaleFactor = computedScale,
          IsNormalised = autoScale,
          StructuralScale = arrowScale,
        };

        GraphicDrawResult diagramResults = _gsaModel.ApiModel.GetDiagrams(graphic);
        ReadOnlyCollection<Line> linesFromModel = diagramResults.Lines;

        double lengthScaleFactor = UnitConverter.Convert(1, Length.BaseUnit, _gsaModel.ModelUnit);
        foreach (Line item in linesFromModel) {
          diagramLines.Add(new GsaDiagramGoo(
            new GsaLineDiagram(item, lengthScaleFactor, color)));
        }

        if (diagramResults.Triangles.Count > 0) {
          diagramLines.Add(new GsaDiagramGoo(
            new GsaArrowheadDiagram(diagramResults.Triangles, lengthScaleFactor, color)));
        }

        foreach (Annotation annotation in diagramResults.Annotations) {
          diagramAnnotations.Add(GenerateAnnotation(
          annotation, lengthScaleFactor, significantDigits, color, unitScaleFactor));
        }
      });

      da.SetDataList(0, diagramLines);
      da.SetDataList(1, diagramAnnotations);

      PostHog.Diagram("Load", _caseId, _selectedItems[1], types, Parameters.EntityType.Element);
    }

    private bool IsGhObjectValid(GsaModelGoo modelGoo) {
      bool valid = false;
      if (modelGoo?.Value == null) {
        this.AddRuntimeWarning("Input is null");
      } else {
        valid = true;
      }

      return valid;
    }

    private GsaAnnotationGoo GenerateAnnotation(
      Annotation annotation, double lengthScaleFactor,
      int significantDigits, Color color, double valueScaleFactor) {

      //move position
      var location = new Point3d(annotation.Position.X, annotation.Position.Y,
        annotation.Position.Z);
      location *= lengthScaleFactor;

      string valueToAnnotate = annotation.String;
      if (double.TryParse(annotation.String, out double valResult)) {
        //convert annotation value
        valueToAnnotate
          = $"{Math.Round(valResult * valueScaleFactor, significantDigits)} {Message}";
      }

      if (color == Color.Empty) {
        color = (Color)annotation.Colour;
      }

      return new GsaAnnotationGoo(new GsaAnnotationDot(location, color, valueToAnnotate));
    }

    private double ComputeUnitScale(bool autoScale = false) {
      double unitScaleFactor = 1.0d;
      if (!autoScale) {
        unitScaleFactor = UnitConverter.Convert(1, Force.BaseUnit, _forceUnit);
      }

      return unitScaleFactor;
    }

    private List<DiagramType> GetSelectedDiagramTypes() {
      var types = new List<DiagramType>();
      if (_selectedItems[1] == "All") {
        types.AddRange(Mappings._diagramTypeMappingLoads.Select(x => x.GsaApiEnum).ToList());
      } else {
        if (_selectedItems[2] == "All") {
          types.AddRange(Mappings._diagramTypeMappingLoads.Where(
            item => item.Description.StartsWith(
              _selectedItems[1])).Select(item => item.GsaApiEnum).ToList());
        } else {
          string type = $"{_selectedItems[1]} {_selectedItems[2]}";
          types.Add(Mappings._diagramTypeMappingLoads.Where(
            item => item.Description.Contains(type))
            .Select(item => item.GsaApiEnum).FirstOrDefault());
        }
      }
      return types;
    }

    private void PreparedDropdownLists() {
      PopulateTypeNames(ref _nodalDiagramTypes, "Nodal");
      PopulateTypeNames(ref _beamDiagramTypes, "Beam");
      PopulateTypeNames(ref _2dDiagramTypes, "2d");
      PopulateTypeNames(ref _3dDiagramTypes, "3d");
      PopulateTypeNames(ref _gridDiagramTypes, "Grid");
    }

    private void PopulateTypeNames(ref List<string> list, string typeIdentifier) {
      list = Mappings._diagramTypeMappingLoads
       .Where(item => item.Description.Contains(typeIdentifier))
       .Select(item => item.Description).ToList();
      list = list.Select(x => x.Replace(typeIdentifier + " ", string.Empty)).ToList();
      list.Insert(0, "All");
    }

    internal void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    internal void UpdateModel(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateCaseDropdown() {
      if (_gsaModel == null) {
        return;
      }

      Tuple<List<string>, List<int>, DataTree<int?>> modelResults
        = ResultHelper.GetAvalailableResults(_gsaModel);
      var cases = new List<string>();

      List<int> caseIds = _gsaModel.GetLoadCases();
      foreach (int caseId in caseIds) {
        cases.Add($"L{caseId}");
      }

      for (int i = 0; i < modelResults.Item1.Count; i++) {
        if (modelResults.Item1[i] != "Analysis") {
          continue;
        }

        cases.Add($"A{modelResults.Item2[i]}");
      }

      for (int i = 0; i < modelResults.Item1.Count; i++) {
        if (modelResults.Item1[i] != "Combination") {
          continue;
        }

        cases.Add($"C{modelResults.Item2[i]}");
      }

      if (caseIds.Count == 0) {
        return;
      }

      _dropDownItems[0] = cases;
      if (!_dropDownItems[0].Contains(_selectedItems[0])) {
        _selectedItems[0] = _dropDownItems[0][0];
      }

      _caseId = _selectedItems[0];
    }
  }
}
