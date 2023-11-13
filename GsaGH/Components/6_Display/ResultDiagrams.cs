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
  ///   Component to get Element1D results
  /// </summary>
  public class ResultDiagrams : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("7ae7ac36-f811-4c20-911f-ddb119f45644");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ResultDiagrams;

    private string _case = string.Empty;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;
    private bool _undefinedModelLengthUnit;

    public ResultDiagrams() : base("Result Diagrams", "ResultDiagram",
      "Displays GSA 1D Element Result Diagram", CategoryName.Name(), SubCategoryName.Cat6()) { }

    public override bool Read(GH_IReader reader) {
      //warning - sensitive for description string! do not change description if not needed!
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      _stressUnit
        = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), reader.GetString("stress"));
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0) {
        if (j == 0) {
          if (_dropDownItems[1] != Mappings.diagramTypeMappingForce.Select(item => item.Description)
           .ToList()) {
            _dropDownItems[1] = Mappings.diagramTypeMappingForce.Select(item => item.Description)
             .ToList();
            _selectedItems[1] = _dropDownItems[1][5]; // Myy
          }
        } else {
          if (_dropDownItems[1] != Mappings.diagramTypeMappingStress
           .Select(item => item.Description).ToList()) {
            _dropDownItems[1] = Mappings.diagramTypeMappingStress.Select(item => item.Description)
             .ToList();
            _selectedItems[1] = _dropDownItems[1][7]; // Combined C1
          }
        }
      }

      base.UpdateUI();
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      writer.SetString("stress", Pressure.GetAbbreviation(_stressUnit));
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      ToolStripMenuItem lengthUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Displacement",
        EngineeringUnits.Length, Length.GetAbbreviation(_lengthResultUnit), UpdateLength);
      ToolStripMenuItem forceUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Force",
        EngineeringUnits.Force, Force.GetAbbreviation(_forceUnit), UpdateForce);
      ToolStripMenuItem momentUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Moment",
        EngineeringUnits.Moment, Moment.GetAbbreviation(_momentUnit), UpdateMoment);
      ToolStripMenuItem stressUnitsMenu = GenerateToolStripMenuItem.GetSubMenuItem("Stress",
        EngineeringUnits.Stress, Pressure.GetAbbreviation(_stressUnit), UpdateStress);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        forceUnitsMenu,
        momentUnitsMenu,
        stressUnitsMenu,
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
        "Type",
        "Diagram",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>() {
        "Force",
        "Stress",
      });
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(Mappings.diagramTypeMappingForce.Select(item => item.Description)
       .ToList());
      _selectedItems.Add(_dropDownItems[1][5]); // Myy

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
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
      } else if (IsStress()) {
        Message = Pressure.GetAbbreviation(_stressUnit);
      } else {
        Message = "Error";
        this.AddRuntimeError("Cannot get unit for selected diagramType!");
      }
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var result = new GsaResult();
      _case = string.Empty;

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp) || !IsGhObjectValid(ghTyp)) {
        return;
      }

      if (ghTyp?.Value is GsaResultGoo goo) {
        result = (GsaResult)goo.Value;
        switch (result.CaseType) {
          case CaseType.CombinationCase when result.SelectedPermutationIds.Count > 1:
            string warningText
              = $"Combination Case {result.CaseId} contains {result.SelectedPermutationIds.Count} permutations - only one permutation can be displayed at a time.{Environment.NewLine}Displaying first permutation; please use the 'Select Results' to select other single permutations";
            this.AddRuntimeWarning(warningText);
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

      EntityList list = Inputs.GetElementOrMemberList(this, da, 1, result.Model);

      var ghScale = new GH_Number();
      double scale = 1;
      bool autoScale = true;
      if (da.GetData(5, ref ghScale)) {
        GH_Convert.ToDouble(ghScale, out scale, GH_Conversion.Both);
        autoScale = false;
      }

      LengthUnit lengthUnit = result.Model.ModelUnit;
      _undefinedModelLengthUnit = false;
      if (lengthUnit == LengthUnit.Undefined) {
        lengthUnit = _lengthUnit;
        _undefinedModelLengthUnit = true;
        this.AddRuntimeRemark(
          $"Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in {lengthUnit}. This can be changed by right-clicking the component -> 'Select Units'");
      }

      DiagramType type = _selectedItems[0] == "Force" 
        ? Mappings.diagramTypeMappingForce.Where(item => item.Description == _selectedItems[1])
          .Select(item => item.GsaApiEnum).FirstOrDefault() 
        : Mappings.diagramTypeMappingStress.Where(item => item.Description == _selectedItems[1])
          .Select(item => item.GsaApiEnum).FirstOrDefault();

      double unitScale = ComputeUnitScale(autoScale);
      double computedScale
        = GraphicsScalar.ComputeScale(result.Model, scale, _lengthUnit, autoScale, unitScale);
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
        } else if (IsStress()) {
          unitScaleFactor = UnitConverter.Convert(1, Pressure.BaseUnit, _stressUnit);
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
        Mappings.diagramTypeMappingForce.Where(item => item.Description == _selectedItems[1])
         .Select(item => item.GsaApiEnum).FirstOrDefault() : Mappings.diagramTypeMappingStress
         .Where(item => item.Description == _selectedItems[1]).Select(item => item.GsaApiEnum)
         .FirstOrDefault();
      switch (type) {
        case DiagramType.AxialForceFx:
        case DiagramType.ShearForceFy:
        case DiagramType.ShearForceFz:
        case DiagramType.ResolvedShearFyz:
          isForce = true;
          break;
      }

      return isForce;
    }

    private bool IsMoment() {
      bool isMoment = false;
      DiagramType type = _selectedItems[0] == "Force" ?
        Mappings.diagramTypeMappingForce.Where(item => item.Description == _selectedItems[1])
         .Select(item => item.GsaApiEnum).FirstOrDefault() : Mappings.diagramTypeMappingStress
         .Where(item => item.Description == _selectedItems[1]).Select(item => item.GsaApiEnum)
         .FirstOrDefault();
      switch (type) {
        case DiagramType.MomentMyy:
        case DiagramType.MomentMzz:
        case DiagramType.ResolvedMomentMyz:
        case DiagramType.TorsionMxx:
          isMoment = true;
          break;
      }

      return isMoment;
    }

    private bool IsStress() {
      bool isStress = false;
      DiagramType type = _selectedItems[0] == "Force" ?
        Mappings.diagramTypeMappingForce.Where(item => item.Description == _selectedItems[1])
         .Select(item => item.GsaApiEnum).FirstOrDefault() : Mappings.diagramTypeMappingStress
         .Where(item => item.Description == _selectedItems[1]).Select(item => item.GsaApiEnum)
         .FirstOrDefault();
      switch (type) {
        case DiagramType.AxialStressA:
        case DiagramType.BendingStressByNegativeZ:
        case DiagramType.BendingStressByPositiveZ:
        case DiagramType.BendingStressBzNegativeY:
        case DiagramType.BendingStressBzPositiveY:
        case DiagramType.CombinedStressC1:
        case DiagramType.CombinedStressC2:
        case DiagramType.ShearStressSz:
        case DiagramType.ShearStressSy:
          isStress = true;
          break;
      }

      return isStress;
    }

    private void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateStress(string unit) {
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateLength(string unit) {
      _lengthResultUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
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
