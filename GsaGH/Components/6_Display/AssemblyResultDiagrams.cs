using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.GsaApi.Grahics;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using DiagramType = GsaAPI.DiagramType;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = GsaAPI.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Assembly results
  /// </summary>
  public class AssemblyResultDiagrams : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("1b059edf-69aa-4c54-bcd6-01e3def03ef1");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => null;

    private string _case = string.Empty;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;

    public AssemblyResultDiagrams() : base("Assembly Result Diagrams", "AssemblyResultDiagram",
      "Displays GSA Assembly Result Diagram", CategoryName.Name(), SubCategoryName.Cat6()) { }

    public override bool Read(GH_IReader reader) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0) {
        if (j == 0) {
          if (_dropDownItems[1] != Mappings._diagramTypeMappingAssemblyForce.Select(item => item.Description)
           .ToList()) {
            _dropDownItems[1] = Mappings._diagramTypeMappingAssemblyForce.Select(item => item.Description)
             .ToList();
            _selectedItems[1] = _dropDownItems[1][5]; // Myy
          }
        }
      }

      base.UpdateUI();
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      ToolStripMenuItem forceUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Force",
        EngineeringUnits.Force, Force.GetAbbreviation(_forceUnit), UpdateForce);
      ToolStripMenuItem momentUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Moment",
        EngineeringUnits.Moment, Moment.GetAbbreviation(_momentUnit), UpdateMoment);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        forceUnitsMenu,
        momentUnitsMenu
      });

      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;
      menu.Items.Add(unitsMenu);
      Menu_AppendSeparator(menu);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Diagram",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>() {
        "Force"
      });
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(Mappings._diagramTypeMappingAssemblyForce.Select(item => item.Description)
       .ToList());
      _selectedItems.Add(_dropDownItems[1][5]); // Myy

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaAssemblyListParameter());
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
      pManager.AddParameter(new GsaDiagramParameter(), "Diagram lines", "Dgm", "Lines of the GSA Result Diagram",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaAnnotationParameter(), "Annotations",
        "An", "Annotations for the diagram", GH_ParamAccess.list);
      pManager.HideParameter(1);
    }

    protected override void BeforeSolveInstance() {
      if (IsForce()) {
        Message = Force.GetAbbreviation(_forceUnit);
      } else if (IsMoment()) {
        Message = Moment.GetAbbreviation(_momentUnit);
      } else {
        Message = "Error";
        this.AddRuntimeError("Cannot get unit for selected diagramType!");
      }
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result = null;
      _case = string.Empty;

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp) || !IsGhObjectValid(ghTyp)) {
        return;
      }

      if (ghTyp?.Value is GsaResultGoo goo) {
        result = (GsaResult)goo.Value;
        switch (result.CaseType) {
          case CaseType.CombinationCase when result.SelectedPermutationIds.Count > 1:
            this.AddRuntimeRemark(
              $"Combination Case {result.CaseId} contains {result.SelectedPermutationIds.Count}"
              + $" permutations and diagrams will show on top of eachother for each permutaion."
              + Environment.NewLine
              + $"To select a single permutation use the 'Select Results' component.");
            _case = $"C{result.CaseId}";
            break;

          case CaseType.CombinationCase:
            _case = $"C{result.CaseId}";
            break;

          case CaseType.AnalysisCase:
            _case = $"A{result.CaseId}";
            break;
        }
      }

      EntityList list = Inputs.GetAssemblyList(this, da, 1);

      var ghScale = new GH_Number();
      double scale = 1;
      bool autoScale = true;
      if (da.GetData(5, ref ghScale)) {
        GH_Convert.ToDouble(ghScale, out scale, GH_Conversion.Both);
        autoScale = false;
      }

      LengthUnit lengthUnit = result.Model.ModelUnit;

      DiagramType type = _selectedItems[0] == "Force"
        ? Mappings._diagramTypeMappingAssemblyForce.Where(item => item.Description == _selectedItems[1])
          .Select(item => item.GsaApiEnum).FirstOrDefault()
        : Mappings._diagramTypeMappingStress.Where(item => item.Description == _selectedItems[1])
          .Select(item => item.GsaApiEnum).FirstOrDefault();

      double unitScale = ComputeUnitScale(autoScale);
      double computedScale
        = GraphicsScalar.ComputeScale(result.Model, scale, lengthUnit, autoScale, unitScale);
      var graphic = new DiagramSpecification() {
        ListDefinition = list.Definition,
        ListType = list.Type,
        Type = type,
        Cases = _case,
        ScaleFactor = computedScale,
        IsNormalised = autoScale,
      };

      var diagramLines = new List<GsaDiagramGoo>();
      var diagramAnnotations = new List<GsaAnnotationGoo>();

      GraphicDrawResult diagramResults = result.Model.Model.GetDiagrams(graphic);
      ReadOnlyCollection<Line> linesFromModel = diagramResults.Lines;

      Color color = Color.Empty;
      da.GetData(4, ref color);

      double lengthScaleFactor = UnitConverter.Convert(1, Length.BaseUnit, lengthUnit);
      foreach (Line item in linesFromModel) {
        diagramLines.Add(new GsaDiagramGoo(new GsaLineDiagram(item, lengthScaleFactor, color)));
      }

      bool showAnnotations = true;
      int significantDigits = 3;

      da.GetData(2, ref showAnnotations);
      da.GetData(3, ref significantDigits);

      diagramAnnotations = GenerateAnnotations(diagramResults.Annotations, lengthScaleFactor,
        significantDigits, color);

      ((IGH_PreviewObject)Params.Output[1]).Hidden = !showAnnotations;

      da.SetDataList(0, diagramLines);
      da.SetDataList(1, diagramAnnotations);

      PostHog.Diagram("Result", result.CaseType, _selectedItems[0], type.ToString(), Parameters.EntityType.Element);
    }

    private List<GsaAnnotationGoo> GenerateAnnotations(
      IReadOnlyCollection<Annotation> annotationsFromModel, double lengthScaleFactor,
      int significantDigits, Color color) {
      var diagramAnnotations = new List<GsaAnnotationGoo>();

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

          if (color == Color.Empty) {
            color = (Color)annotation.Colour;
          }

          diagramAnnotations.Add(new GsaAnnotationGoo(
            new GsaAnnotationDot(location, color, valueToAnnotate)));
        }
      }

      return diagramAnnotations;
    }

    private bool IsGhObjectValid(GH_ObjectWrapper ghObject) {
      bool valid = false;
      if (ghObject?.Value == null) {
        this.AddRuntimeWarning("Input is null");
      } else if (!(ghObject.Value is GsaResultGoo)) {
        this.AddRuntimeError("Error converting input to GSA Result");
      } else {
        valid = true;
      }

      return valid;
    }

    private double ComputeUnitScale(bool autoScale = false) {
      double unitScaleFactor = 1.0d;
      if (!autoScale) {
        if (IsForce()) {
          unitScaleFactor = UnitConverter.Convert(1, Force.BaseUnit, _forceUnit);
        } else if (IsMoment()) {
          unitScaleFactor = UnitConverter.Convert(1, Moment.BaseUnit, _momentUnit);
        } else {
          this.AddRuntimeError("Not supported diagramType!");
        }
      }

      return unitScaleFactor;
    }

    private bool IsForce() {
      bool isForce = false;
      DiagramType type = _selectedItems[0] == "Force" ?
        Mappings._diagramTypeMappingAssemblyForce.Where(item => item.Description == _selectedItems[1])
         .Select(item => item.GsaApiEnum).FirstOrDefault() : Mappings._diagramTypeMappingStress
         .Where(item => item.Description == _selectedItems[1]).Select(item => item.GsaApiEnum)
         .FirstOrDefault();
      switch (type) {
        case DiagramType.AssemblyAxialForceFx:
        case DiagramType.AssemblyShearForceFy:
        case DiagramType.AssemblyShearForceFz:
          isForce = true;
          break;
      }

      return isForce;
    }

    private bool IsMoment() {
      bool isMoment = false;
      DiagramType type = _selectedItems[0] == "Force" ?
        Mappings._diagramTypeMappingAssemblyForce.Where(item => item.Description == _selectedItems[1])
         .Select(item => item.GsaApiEnum).FirstOrDefault() : Mappings._diagramTypeMappingStress
         .Where(item => item.Description == _selectedItems[1]).Select(item => item.GsaApiEnum)
         .FirstOrDefault();
      switch (type) {
        case DiagramType.AssemblyMomentMyy:
        case DiagramType.AssemblyMomentMzz:
        case DiagramType.AssemblyTorsionMxx:
          isMoment = true;
          break;
      }

      return isMoment;
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
  }
}
