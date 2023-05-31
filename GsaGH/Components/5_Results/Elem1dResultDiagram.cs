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
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.GsaApi.Grahics;
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
using DiagramType = GsaGH.Parameters.Enums.DiagramType;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Element1D results
  /// </summary>
  public class Elem1dResultDiagram : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("7ae7ac36-f811-4c20-911f-ddb119f45644");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Elem1dDiagram;

    private string _case = "";
    private DiagramType _displayedDiagramType = DiagramType.AxialForceFx;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;
    private bool _undefinedModelLengthUnit;

    public Elem1dResultDiagram() : base("1D Element Result Diagram", "ResultElem1dDiagram",
      "Displays GSA 1D Element Result Diagram", CategoryName.Name(), SubCategoryName.Cat5()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, _spacerDescriptions);
    }

    public override bool Read(GH_IReader reader) {
      //warning - sensitive for description string! do not change description if not needed!
      _displayedDiagramType = Mappings.diagramTypeMapping
       .Where(item => item.Description == reader.GetString("diagramType"))
       .Select(item => item.GsaGhEnum).FirstOrDefault();
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
      _displayedDiagramType = (DiagramType)j;
      _selectedItems[i] = _dropDownItems[i][j];

      base.UpdateUI();
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("diagramType",
        Mappings.diagramTypeMapping.Where(item => item.GsaGhEnum == _displayedDiagramType)
         .Select(item => item.Description).FirstOrDefault());
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

      ToolStripMenuItem lengthUnitsMenu = GetToolStripMenuItem("Displacement",
        EngineeringUnits.Length, Length.GetAbbreviation(_lengthResultUnit), UpdateLength);
      ToolStripMenuItem forceUnitsMenu = GetToolStripMenuItem("Force", EngineeringUnits.Force,
        Force.GetAbbreviation(_forceUnit), UpdateForce);
      ToolStripMenuItem momentUnitsMenu = GetToolStripMenuItem("Moment", EngineeringUnits.Moment,
        Moment.GetAbbreviation(_momentUnit), UpdateMoment);
      ToolStripMenuItem stressUnitsMenu = GetToolStripMenuItem("Stress", EngineeringUnits.Stress,
        Pressure.GetAbbreviation(_stressUnit), UpdateStress);

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.Units);

      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        forceUnitsMenu,
        momentUnitsMenu,
        stressUnitsMenu,
      });

      if (_undefinedModelLengthUnit) {
        ToolStripMenuItem modelUnitsMenu = GetToolStripMenuItem("Model geometry",
          EngineeringUnits.Length, Length.GetAbbreviation(_lengthUnit), UpdateModel);

        unitsMenu.DropDownItems.Insert(0, modelUnitsMenu);
      }

      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Diagram Type",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(Mappings.diagramTypeMapping.Select(item => item.Description).ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list", "El",
        $"Filter import by list.{Environment.NewLine}Element list should take the form:{Environment.NewLine} 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1).{Environment.NewLine}Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size",
        GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Diagram lines", "DL", "Lines of the diagram",
        GH_ParamAccess.list);
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

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      _case = string.Empty;

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp) || !IsGhObjectValid(ghTyp)) {
        return;
      }

      if (ghTyp?.Value is GsaResultGoo goo) {
        result = goo.Value;
        switch (result.Type) {
          case GsaResult.CaseType.Combination when result.SelectedPermutationIds.Count > 1:
            string warningText
              = $"Combination Case {result.CaseId} contains {result.SelectedPermutationIds.Count} permutations - only one permutation can be displayed at a time.{Environment.NewLine}Displaying first permutation; please use the 'Select Results' to select other single permutations";
            this.AddRuntimeWarning(warningText.ToString());
            _case = $"C{result.CaseId}";
            break;

          case GsaResult.CaseType.Combination:
            _case = $"C{result.CaseId}";
            break;

          case GsaResult.CaseType.AnalysisCase:
            _case = $"A{result.CaseId}";
            break;
        }
      }

      string elementlist = GetElementFilters(da);

      var ghScale = new GH_Number();
      double scale = 1;
      bool autoScale = true;
      if (da.GetData(2, ref ghScale)) {
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

      double unitScale = ComputeUnitScale(autoScale);
      double computedScale = GraphicsScalar.ComputeScale(result.Model, scale, _lengthUnit, autoScale, unitScale);
      var graphic = new DiagramSpecification() {
        Elements = elementlist,
        Type = Mappings.diagramTypeMapping.Where(item => item.GsaGhEnum == _displayedDiagramType)
         .Select(item => item.GsaApiEnum).FirstOrDefault(),
        Cases = _case,
        ScaleFactor = computedScale,
        IsNormalised = autoScale,
      };

      var diagramLines = new List<DiagramLineGoo>();
      ReadOnlyCollection<GsaAPI.Line> linesFromModel = result.Model.Model.Get1dElementDiagrams(graphic).Lines;

      foreach (GsaAPI.Line item in linesFromModel) {
        double lengthScaleFactor = UnitConverter.Convert(1, Length.BaseUnit, lengthUnit);
        var startPoint = new Point3d(item.Start.X, item.Start.Y, item.Start.Z);
        var endPoint = new Point3d(item.End.X, item.End.Y, item.End.Z);
        startPoint *= lengthScaleFactor;
        endPoint *= lengthScaleFactor;

        diagramLines.Add(new DiagramLineGoo(startPoint, endPoint, (Color)item.Color));
      }

      da.SetDataList(0, diagramLines);

      //PostHog.Result(result.Type, 1, resultType, _displayedDiagramType.ToString());
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

    private static string GetElementFilters(IGH_DataAccess dataAccess) {
      string nodeList = string.Empty;
      var ghNoList = new GH_String();
      if (dataAccess.GetData(1, ref ghNoList)) {
        nodeList = GH_Convert.ToString(ghNoList, out string tempNodeList, GH_Conversion.Both) ?
          tempNodeList : string.Empty;
      }

      if (nodeList.ToLower() == "all" || string.IsNullOrEmpty(nodeList)) {
        nodeList = "All";
      }

      return nodeList;
    }



    private double ComputeUnitScale(bool autoScale) {
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
      switch (_displayedDiagramType) {
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
      switch (_displayedDiagramType) {
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
      switch (_displayedDiagramType) {
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

    private static ToolStripMenuItem GetToolStripMenuItem(
      string name, EngineeringUnits units, string unitString, Action<string> action) {
      var menu = new ToolStripMenuItem(name) {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(units)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => action(unit)) {
          Checked = unit == unitString,
          Enabled = true,
        };
        menu.DropDownItems.Add(toolStripMenuItem);
      }

      return menu;
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
