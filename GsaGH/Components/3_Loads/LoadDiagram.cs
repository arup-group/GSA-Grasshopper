using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
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
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = GsaAPI.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Load Diagram
  /// </summary>
  public class LoadDiagram : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("5ea823af-a567-40a6-8e82-0e14eb8dda0e");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ShowLoadDiagrams;
    private List<string> _2dDiagramTypes = Mappings.diagramTypeMappingLoads
     .Where(item => item.Description.Contains("2d")).Select(item => item.Description).ToList();
    private List<string> _3dDiagramTypes = Mappings.diagramTypeMappingLoads
     .Where(item => item.Description.Contains("3d")).Select(item => item.Description).ToList();
    private List<string> _allDiagramTypes = Mappings.diagramTypeMappingLoads
     .Select(item => item.Description.ToString()).ToList();
    private List<string> _beamDiagramTypes = Mappings.diagramTypeMappingLoads
     .Where(item => item.Description.Contains("1d")).Select(item => item.Description).ToList();
    private string _caseId = "A1";

    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private List<string> _gravityDiagramTypes = Mappings.diagramTypeMappingLoads
     .Where(item => item.Description.Contains("Gravity")).Select(item => item.Description).ToList();
    private List<string> _gridDiagramTypes = Mappings.diagramTypeMappingLoads
     .Where(item => item.Description.Contains("Grid")).Select(item => item.Description).ToList();
    private GsaModel _gsaModel;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private List<string> _loadTypes = new List<string>() {
      "All",
      "Gravity",
      "Grid",
      "Nodal",
      "Beam",
      "2D",
      "3D",
    };
    private List<string> _nodalDiagramTypes = Mappings.diagramTypeMappingLoads
     .Where(item => item.Description.Contains("Nodal")).Select(item => item.Description).ToList();
    private bool _undefinedModelLengthUnit;

    public LoadDiagram() : base("Load Diagram", "LoadDiagram", "Displays GSA Load Diagram",
      CategoryName.Name(), SubCategoryName.Cat3()) { }

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

          UpdateCaseDropdown();
          break;
        }
        case 1: { // Load Type
          int k = 2;
          switch (_selectedItems[i]) {
            case "All": {
              _dropDownItems[k] = _allDiagramTypes;
              break;
            }
            case "Gravity": {
              _dropDownItems[k] = _gravityDiagramTypes;
              break;
            }
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

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.Units);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        forceUnitsMenu,
      });

      if (_undefinedModelLengthUnit) {
        ToolStripMenuItem modelUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem(
          "Model geometry", EngineeringUnits.Length, Length.GetAbbreviation(_lengthUnit),
          UpdateModel);

        unitsMenu.DropDownItems.Insert(0, modelUnitsMenu);
      }

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

      //Diagram Types
      _dropDownItems.Add(Mappings.diagramTypeMappingLoads.Select(item => item.Description)
       .ToList());
      _selectedItems.Add(_dropDownItems[2][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA model", "GSA",
        "GSA model containing some Analysis Cases and Tasks", GH_ParamAccess.item);
      pManager.AddGenericParameter("Case filter list", "C",
        $"Filter import by list.{Environment.NewLine}The case list should take the form:{Environment.NewLine} 1 L1 M1 A1 C1 C2p1 A3 to A5 T1.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Element filter list", "El",
        $"Filter import by list.{Environment.NewLine}Element list should take the form:{Environment.NewLine} 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1).{Environment.NewLine}Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
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
      pManager.AddGenericParameter("Diagram lines", "L", "Lines of the diagram",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Annotations", "Val", "Annotations for the diagram",
        GH_ParamAccess.list);
      pManager.HideParameter(1);
    }

    protected override void BeforeSolveInstance() {
      Message = Force.GetAbbreviation(_forceUnit);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      if (!da.GetData(0, ref modelGoo) || !IsGhObjectValid(modelGoo)) {
        _gsaModel = null;
        return;
      }

      _gsaModel = modelGoo.Value;
      UpdateCaseDropdown();

      string caseList = string.Empty;
      if (da.GetData(1, ref caseList)) {
        _selectedItems[0] = caseList;
      }

      string elementlist = Inputs.GetElementListNameForesults(this, da, 2);
      if (string.IsNullOrEmpty(elementlist)) {
        return;
      }

      var ghScale = new GH_Number();
      double scale = 1;
      bool autoScale = true;
      if (da.GetData(6, ref ghScale)) {
        GH_Convert.ToDouble(ghScale, out scale, GH_Conversion.Both);
        autoScale = false;
      }

      LengthUnit lengthUnit = _gsaModel.ModelUnit;
      _undefinedModelLengthUnit = false;
      if (lengthUnit == LengthUnit.Undefined) {
        lengthUnit = _lengthUnit;
        _undefinedModelLengthUnit = true;
        this.AddRuntimeRemark(
          $"Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in {lengthUnit}. This can be changed by right-clicking the component -> 'Select Units'");
      }

      DiagramType type = Mappings.diagramTypeMappingLoads
       .Where(item => item.Description == _selectedItems[2]).Select(item => item.GsaApiEnum)
       .FirstOrDefault();

      double unitScale = ComputeUnitScale(autoScale);
      double computedScale
        = GraphicsScalar.ComputeScale(_gsaModel, scale, _lengthUnit, autoScale, unitScale);
      var graphic = new DiagramSpecification() {
        ListDefinition = elementlist,
        Type = type,
        Cases = _caseId,
        ScaleFactor = computedScale,
        IsNormalised = autoScale,
      };

      var diagramLines = new List<DiagramGoo>();
      var diagramAnnotations = new List<AnnotationGoo>();

      GraphicDrawResult diagramResults = _gsaModel.Model.GetDiagrams(graphic);
      ReadOnlyCollection<Line> linesFromModel = diagramResults.Lines;

      Color color = Colours.GsaDarkPurple;
      da.GetData(5, ref color);

      double lengthScaleFactor = UnitConverter.Convert(1, Length.BaseUnit, lengthUnit);
      foreach (Line item in linesFromModel) {
        var startPoint = new Point3d(item.Start.X, item.Start.Y, item.Start.Z);
        var endPoint = new Point3d(item.End.X, item.End.Y, item.End.Z);
        startPoint *= lengthScaleFactor;
        endPoint *= lengthScaleFactor;
        color = color != Color.Empty ? color : (Color)item.Colour;

        var line = new Rhino.Geometry.Line(startPoint, endPoint);
        line.Flip();

        diagramLines.Add(
          new DiagramGoo(endPoint, line.Direction * -1, ArrowMode.OneArrow).SetColor(color));
      }

      bool showAnnotations = true;
      int significantDigits = 3;

      da.GetData(3, ref showAnnotations);
      da.GetData(4, ref significantDigits);

      diagramAnnotations = GenerateAnnotations(diagramResults.Annotations, lengthScaleFactor,
        significantDigits, color);

      ((IGH_PreviewObject)Params.Output[1]).Hidden = !showAnnotations;

      da.SetDataList(0, diagramLines);
      da.SetDataList(1, diagramAnnotations);

      //PostHog.Result(modelGoo.Value.Model.case, 1, "Diagram", type.ToString());
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

    private List<AnnotationGoo> GenerateAnnotations(
      IReadOnlyCollection<Annotation> annotationsFromModel, double lengthScaleFactor,
      int significantDigits, Color color) {
      var diagramAnnotations = new List<AnnotationGoo>();

      foreach (Annotation annotation in annotationsFromModel) {
        {
          //move position
          var location = new Point3d(annotation.Position.X, annotation.Position.Y,
            annotation.Position.Z);
          location *= lengthScaleFactor;

          string valueToAnnotate = annotation.String;
          if (double.TryParse(annotation.String, out double valResult)) {
            //convert annotation value
            double valueScaleFactor = ComputeUnitScale();
            valueToAnnotate
              = $"{Math.Round(valResult * valueScaleFactor, significantDigits)} {Message}";
          }

          diagramAnnotations.Add(new AnnotationGoo(location, color, valueToAnnotate));
        }
      }

      return diagramAnnotations;
    }

    private double ComputeUnitScale(bool autoScale = false) {
      double unitScaleFactor = 1.0d;
      if (!autoScale) {
        unitScaleFactor = UnitConverter.Convert(1, Force.BaseUnit, _forceUnit);
      }

      return unitScaleFactor;
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

    private void UpdateCaseDropdown() {
      if (_gsaModel == null) {
        return;
      }

      Tuple<List<string>, List<int>, DataTree<int?>> modelResults
        = ResultHelper.GetAvalailableResults(_gsaModel);
      string type = "Analysis";

      var cases = new List<string>();
      for (int i = 0; i < modelResults.Item1.Count; i++) {
        if (modelResults.Item1[i] != type) {
          continue;
        }

        cases.Add(type[0] + modelResults.Item2[i].ToString());
      }

      _dropDownItems[0] = cases;
      _selectedItems[0] = _caseId;
    }
  }
}
