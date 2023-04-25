using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using DiagramType = GsaGH.Parameters.Enums.DiagramType;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get Element1D results
  /// </summary>
  public class Elem1dResultDiagram : GH_OasysDropDownComponent {
    private class DiagramLine {
      public Line Line { get; private set; }
      public Color Color { get; private set; }
      public IQuantity Quantity { get; private set; }

      public DiagramLine(Line line, Color color /*, IQuantity quantity*/) {
        Line = line;
        Color = color;
        //Quantity = quantity;
      }
    }

    public override Guid ComponentGuid => new Guid("7ae7ac36-f811-4c20-911f-ddb119f45644");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Elem1dDiagram;

    private string _case = "";
    private double _defScale = 250;
    private DiagramType _displayedDiagramType = DiagramType.AxialForceFx;
    private EnergyUnit _energyResultUnit = DefaultUnits.EnergyUnit;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthResultUnit = DefaultUnits.LengthUnitResult;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    private bool _undefinedModelLengthUnit;

    private ConcurrentDictionary<int, DiagramLine> _cachedLines;

    public Elem1dResultDiagram() : base("1D Element Result Diagram", "ResultElem1dDiagram",
      "Displays GSA 1D Element Result Diagram", CategoryName.Name(), SubCategoryName.Cat5()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, _spacerDescriptions);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);
      if (_cachedLines is null) {
        return;
      }

      foreach (DiagramLine diagramLine in _cachedLines.Values) {
        args.Display.DrawLine(diagramLine.Line, diagramLine.Color);
      }
    }

    public override bool Read(GH_IReader reader) {
      //warning - sensitive for description string! do not change description if not needed!
      _displayedDiagramType = Mappings.diagramTypeMapping
       .Where(item => item.Description == reader.GetString("DiagramType"))
       .Select(item => item.GsaGhEnum).FirstOrDefault();
      _defScale = reader.GetDouble("scale");
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("model"));
      _lengthResultUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("length"));
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), reader.GetString("force"));
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), reader.GetString("moment"));
      _energyResultUnit
        = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), reader.GetString("energy"));

      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _displayedDiagramType = (DiagramType)j;
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("DiagramType",
        Mappings.diagramTypeMapping.Where(item => item.GsaGhEnum == _displayedDiagramType)
         .Select(item => item.Description).FirstOrDefault());
      writer.SetDouble("scale", _defScale);
      writer.SetString("model", Length.GetAbbreviation(_lengthUnit));
      writer.SetString("length", Length.GetAbbreviation(_lengthResultUnit));
      writer.SetString("force", Force.GetAbbreviation(_forceUnit));
      writer.SetString("moment", Moment.GetAbbreviation(_momentUnit));
      writer.SetString("energy", Energy.GetAbbreviation(_energyResultUnit));

      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var lengthUnitsMenu = new ToolStripMenuItem("Displacement") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateLength(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthResultUnit),
          Enabled = true,
        };
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var forceUnitsMenu = new ToolStripMenuItem("Force") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateForce(unit)) {
          Checked = unit == Force.GetAbbreviation(_forceUnit),
          Enabled = true,
        };
        forceUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var momentUnitsMenu = new ToolStripMenuItem("Moment") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateMoment(unit)) {
          Checked = unit == Moment.GetAbbreviation(_momentUnit),
          Enabled = true,
        };
        momentUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var energyUnitsMenu = new ToolStripMenuItem("Energy") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Energy)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateEnergy(unit)) {
          Checked = unit == Energy.GetAbbreviation(_energyResultUnit),
          Enabled = true,
        };
        energyUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.Units);

      if (_undefinedModelLengthUnit) {
        var modelUnitsMenu = new ToolStripMenuItem("Model geometry") {
          Enabled = true,
        };
        foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
          var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateModel(unit)) {
            Checked = unit == Length.GetAbbreviation(_lengthUnit),
            Enabled = true,
          };
          modelUnitsMenu.DropDownItems.Add(toolStripMenuItem);
        }

        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
          modelUnitsMenu,
          lengthUnitsMenu,
          forceUnitsMenu,
          momentUnitsMenu,
          energyUnitsMenu,
        });
      } else {
        unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
          lengthUnitsMenu,
          forceUnitsMenu,
          momentUnitsMenu,
          energyUnitsMenu,
        });
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
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Element filter list", "El",
        "Filter import by list." + Environment.NewLine + "Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Scale", "x:X", "Scale the result display size",
        GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Result Vectors", "V", "Contoured Vectors with result values",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Curves", "e", "envelope", GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      _case = "";

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp) || !IsGhObjectValid(ghTyp)) {
        return;
      }

      if (ghTyp?.Value is GsaResultGoo goo) {
        result = goo.Value;
        switch (result.Type) {
          case GsaResult.CaseType.Combination when result.SelectedPermutationIds.Count > 1:
            this.AddRuntimeWarning("Combination Case " + result.CaseId + " contains "
              + result.SelectedPermutationIds.Count
              + " permutations - only one permutation can be displayed at a time."
              + Environment.NewLine
              + "Displaying first permutation; please use the 'Select Results' to select other single permutations");
            _case = "C" + result.CaseId;
            break;

          case GsaResult.CaseType.Combination:
            _case = "C" + result.CaseId;
            break;

          case GsaResult.CaseType.AnalysisCase:
            _case = "A" + result.CaseId;
            break;
        }
      }

      string elementlist = GetNodeFilters(da);
      var ghScale = new GH_Number();
      double scale = 1;
      bool isNormalised = !da.GetData(2, ref ghScale);
      if (!isNormalised) {
        GH_Convert.ToDouble(ghScale, out scale, GH_Conversion.Both);
      }

      //LengthUnit lengthUnit = result.Model.ModelUnit;
      //_undefinedModelLengthUnit = false;
      //if (lengthUnit == LengthUnit.Undefined) {
      //  lengthUnit = _lengthUnit;
      //  _undefinedModelLengthUnit = true;
      //  this.AddRuntimeRemark(
      //    "Model came straight out of GSA and we couldn't read the units. The geometry has been scaled to be in "
      //    + lengthUnit + ". This can be changed by right-clicking the component -> 'Select Units'");
      //}

      Tuple<List<GsaResultsValues>, List<int>> reactionForceValues
        = result.NodeReactionForceValues(elementlist, DefaultUnits.ForceUnit,
          DefaultUnits.MomentUnit);
      elementlist = string.Join(" ", reactionForceValues.Item2);

      var graphic = new GraphicSpecification() {
        Elements = elementlist,
        Type = Mappings.diagramTypeMapping.Where(item => item.GsaGhEnum == _displayedDiagramType)
         .Select(item => item.GsaApiEnum).FirstOrDefault(),
        Cases = _case,
        ScaleFactor = scale,
        IsNormalised = isNormalised,
      };

      _cachedLines = new ConcurrentDictionary<int, DiagramLine>();
      var linesFromModel = result.Model.Model.Draw(graphic).Lines.ToList();

      int i = 0;
      foreach (GsaAPI.Line item in linesFromModel) {
        var line = new Line(item.Start.X, item.Start.Y, item.Start.Z, item.End.X, item.End.Y,
          item.End.Z);
        _cachedLines.TryAdd(i++, new DiagramLine(line, (Color)item.Color));
      }

      var lines = new List<Line>();

      foreach (KeyValuePair<int, DiagramLine> keyValuePair in _cachedLines) {
        lines.Add(keyValuePair.Value.Line);
      }

      da.SetData(1, lines);

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

    private static string GetNodeFilters(IGH_DataAccess dataAccess) {
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

    private void UpdateEnergy(string unit) {
      _energyResultUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), unit);
      ExpirePreview(true);
      base.UpdateUI();
    }

    private void UpdateForce(string unit) {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), unit);
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
