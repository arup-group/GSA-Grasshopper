using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
      //public IQuantity Quantity { get; private set; } // will be used when we can get the values from graphicDrawResult

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
      return base.Write(writer);
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
      if (da.GetData(2, ref ghScale)) {
        GH_Convert.ToDouble(ghScale, out scale, GH_Conversion.Both);
      }

      var graphic = new GraphicSpecification() {
        Elements = elementlist,
        Type = Mappings.diagramTypeMapping.Where(item => item.GsaGhEnum == _displayedDiagramType)
         .Select(item => item.GsaApiEnum).FirstOrDefault(),
        Cases = _case,
        IsNormalised
          = true, // want to normalise it always! because for scaling stuff will look much better - that's why scaleFactor is outside object initialisation
      };

      _cachedLines = new ConcurrentDictionary<int, DiagramLine>();
      graphic.ScaleFactor = scale;
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

  }
}
