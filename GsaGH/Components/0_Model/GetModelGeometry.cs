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
using Grasshopper.Kernel.Data;

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

using Rhino.Display;
using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Utility = GsaGH.Helpers.Utility;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve geometric objects from a GSA model
  /// </summary>
  public class GetModelGeometry : GH_OasysTaskCapableComponent<GetModelGeometry.SolveResults>,
    IGH_VariableParameterComponent {
    public class SolveResults {
      internal ConcurrentBag<GsaAssemblyGoo> Assemblies { get; set; }
      internal ConcurrentBag<GsaNodeGoo> DisplaySupports { get; set; }
      internal ConcurrentBag<GsaElement1dGoo> Elem1ds { get; set; }
      internal ConcurrentBag<GsaElement2dGoo> Elem2ds { get; set; }
      internal ConcurrentBag<GsaElement3dGoo> Elem3ds { get; set; }
      internal ConcurrentBag<GsaMember1dGoo> Mem1ds { get; set; }
      internal ConcurrentBag<GsaMember2dGoo> Mem2ds { get; set; }
      internal ConcurrentBag<GsaMember3dGoo> Mem3ds { get; set; }
      internal ConcurrentBag<GsaNodeGoo> Nodes { get; set; }
    }

    private enum FoldMode {
      Graft,
      List,
    }

    public override BoundingBox ClippingBox => _boundingBox;
    public override Guid ComponentGuid => new Guid("7a5b627e-067e-4f77-9bb3-d528e686238c");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public List<List<string>> _dropDownItems;
    public bool _isInitialised;
    public List<string> _selectedItems;
    public List<string> _spacerDescriptions;
    protected override Bitmap Icon => Resources.GetModelGeometry;
    private BoundingBox _boundingBox;
    private ConcurrentBag<GeometryBase> _cachedDisplayGeometryWithoutParent;
    private ConcurrentBag<GeometryBase> _cachedDisplayGeometryWithParent;
    private ConcurrentBag<GeometryBase> _cachedDisplayNgonMeshWithoutParent;
    private ConcurrentBag<GeometryBase> _cachedDisplayNgonMeshWithParent;

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private FoldMode _mode = FoldMode.List;
    private ConcurrentBag<GsaNodeGoo> _supportNodes;
    private ConcurrentBag<AssemblyPreview> _assemblyPreviews = new ConcurrentBag<AssemblyPreview>();
    private bool _showSupports = true;
    private SolveResults _results;

    public GetModelGeometry() : base("Get Model Geometry", "GetGeo",
      "Get nodes, elements, members and assemblies from GSA model", CategoryName.Name(),
      SubCategoryName.Cat0()) { }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, _spacerDescriptions);
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
      base.DrawViewportMeshes(args);
      DrawGraphicMesh(args, _cachedDisplayGeometryWithoutParent);
      DrawGraphicMesh(args, _cachedDisplayNgonMeshWithoutParent);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      DrawGraphicWire(args, _cachedDisplayGeometryWithParent, Colours.GsaLightGrey, 1);
      DrawGraphicWire(args, _cachedDisplayNgonMeshWithParent, Colours.GsaLightGrey, 1);

      if (Attributes.Selected) {
        DrawGraphicWire(args, _cachedDisplayGeometryWithoutParent, Colours.Element2dEdgeSelected, 2);
        DrawGraphicWire(args, _cachedDisplayNgonMeshWithoutParent, Colours.Element2dEdgeSelected, 2);
      } else {
        DrawGraphicWire(args, _cachedDisplayGeometryWithoutParent, Colours.Element2dEdge, 1);
        DrawGraphicWire(args, _cachedDisplayNgonMeshWithoutParent, Colours.Element2dEdge, 1);
      }

      if (_supportNodes == null) {
        return;
      }

      foreach (AssemblyPreview preview in _assemblyPreviews) {
        args.Display.DrawLines(preview.Outlines, Colours.Assembly);
      }

      foreach (GsaNodeGoo node in _supportNodes) {
        if (node.Value.Point.IsValid) {
          if (!Attributes.Selected) {
            if ((Color)node.Value.ApiNode.Colour != Color.FromArgb(0, 0, 0)) {
              args.Display.DrawPoint(node.Value.Point, PointStyle.RoundSimple, 3,
                (Color)node.Value.ApiNode.Colour);
            } else {
              Color col = Colours.Node;
              args.Display.DrawPoint(node.Value.Point, PointStyle.RoundSimple, 3, col);
            }
          } else {
            args.Display.DrawPoint(node.Value.Point, PointStyle.RoundControlPoint, 3,
              Colours.NodeSelected);
          }

          if (node.Value.SupportPreview != null) {
            if (!Attributes.Selected) {
              if (node.Value.SupportPreview.SupportSymbol != null) {
                args.Display.DrawBrepShaded(node.Value.SupportPreview.SupportSymbol, Colours.SupportSymbol);
              }

              if (node.Value.SupportPreview.Text != null) {
                args.Display.Draw3dText(node.Value.SupportPreview.Text, Colours.Support);
              }
            } else {
              if (node.Value.SupportPreview.SupportSymbol != null) {
                args.Display.DrawBrepShaded(node.Value.SupportPreview.SupportSymbol, Colours.SupportSymbolSelected);
              }

              if (node.Value.SupportPreview.Text != null) {
                args.Display.Draw3dText(node.Value.SupportPreview.Text, Colours.NodeSelected);
              }
            }

            if (node.Value.SupportPreview.Xaxis != null) {
              args.Display.DrawLine(node.Value.SupportPreview.Xaxis,
                Color.FromArgb(255, 244, 96, 96), 1);
              args.Display.DrawLine(node.Value.SupportPreview.Yaxis,
                Color.FromArgb(255, 96, 244, 96), 1);
              args.Display.DrawLine(node.Value.SupportPreview.Zaxis,
                Color.FromArgb(255, 96, 96, 234), 1);
            }
          }
        }
      }
    }

    public void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      ReadDropDownComponents(ref reader, ref _dropDownItems, ref _selectedItems,
        ref _spacerDescriptions);
      _isInitialised = true;
      UpdateUiFromSelectedItems();
      return base.Read(reader);
    }

    public void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      UpdateUi();
    }

    public virtual void UpdateUi() {
      ((IGH_VariableParameterComponent)this).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      OnDisplayExpired(true);
    }

    public void UpdateUiFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      CreateAttributes();
      UpdateUi();
    }

    public void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      int i = 0;
      Params.Output[i++].Name = "Nodes in [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "1D Elements in [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "2D Elements in [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "3D Elements in [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "1D Members in [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "2D Members in [" + unitAbbreviation + "]";
      Params.Output[i].Name = "3D Members in [" + unitAbbreviation + "]";

      for (int j = 1; j < 8; j++) {
        Params.Output[j].Access
          = _mode == FoldMode.List ? GH_ParamAccess.list : GH_ParamAccess.tree;
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      WriteDropDownComponents(ref writer, _dropDownItems, _selectedItems, _spacerDescriptions);

      return base.Write(writer);
    }

    internal static void ReadDropDownComponents(
      ref GH_IReader reader, ref List<List<string>> dropDownItems, ref List<string> selectedItems,
      ref List<string> spacerDescriptions) {
      if (reader.GetBoolean("dropdown")) {
        int dropdownCount = reader.GetInt32("dropdownCount");
        dropDownItems = new List<List<string>>();
        for (int i = 0; i < dropdownCount; i++) {
          int dropdowncontentsCount = reader.GetInt32("dropdowncontentsCount" + i);
          var tempcontent = new List<string>();
          for (int j = 0; j < dropdowncontentsCount; j++) {
            tempcontent.Add(reader.GetString("dropdowncontents" + i + j));
          }

          dropDownItems.Add(tempcontent);
        }
      } else {
        throw new Exception("Component doesnt have 'dropdown' content stored");
      }

      if (reader.GetBoolean("spacer")) {
        int dropdownspacerCount = reader.GetInt32("spacerCount");
        spacerDescriptions = new List<string>();
        for (int i = 0; i < dropdownspacerCount; i++) {
          spacerDescriptions.Add(reader.GetString("spacercontents" + i));
        }
      }

      if (!reader.GetBoolean("select")) {
        return;
      }

      int selectionsCount = reader.GetInt32("selectionCount");
      selectedItems = new List<string>();
      for (int i = 0; i < selectionsCount; i++) {
        selectedItems.Add(reader.GetString("selectioncontents" + i));
      }
    }

    internal static GH_IWriter WriteDropDownComponents(
      ref GH_IWriter writer, List<List<string>> dropDownItems, List<string> selectedItems,
      List<string> spacerDescriptions) {
      bool dropdown = false;
      if (dropDownItems != null) {
        writer.SetInt32("dropdownCount", dropDownItems.Count);
        for (int i = 0; i < dropDownItems.Count; i++) {
          writer.SetInt32("dropdowncontentsCount" + i, dropDownItems[i].Count);
          for (int j = 0; j < dropDownItems[i].Count; j++) {
            writer.SetString("dropdowncontents" + i + j, dropDownItems[i][j]);
          }
        }

        dropdown = true;
      }

      writer.SetBoolean("dropdown", dropdown);

      bool spacer = false;
      if (spacerDescriptions != null) {
        writer.SetInt32("spacerCount", spacerDescriptions.Count);
        for (int i = 0; i < spacerDescriptions.Count; i++) {
          writer.SetString("spacercontents" + i, spacerDescriptions[i]);
        }

        spacer = true;
      }

      writer.SetBoolean("spacer", spacer);

      bool select = false;
      if (selectedItems != null) {
        writer.SetInt32("selectionCount", selectedItems.Count);
        for (int i = 0; i < selectedItems.Count; i++) {
          writer.SetString("selectioncontents" + i, selectedItems[i]);
        }

        select = true;
      }

      writer.SetBoolean("select", select);

      return writer;
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendItem(menu, "Graft by Property", GraftModeClicked, true, _mode == FoldMode.Graft);
      Menu_AppendItem(menu, "List", ListModeClicked, true, _mode == FoldMode.List);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some geometry", GH_ParamAccess.item);
      pManager.AddParameter(new GsaNodeListParameter());
      pManager.AddParameter(new GsaElementMemberListParameter(), "Element filter list",
        "El", $"Filter the Elements by list. (by default 'all'){Environment.NewLine}" +
        $"Element/Member list should take the form:{Environment.NewLine}" +
        $" 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" +
        $"{Environment.NewLine}Refer to GSA help file for definition of lists and full vocabulary."
        + $"{Environment.NewLine}You can input a member list to get child elements.",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMemberListParameter());
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddParameter(new GsaNodeParameter(), "Nodes in [" + unitAbbreviation + "]", "No",
        "Nodes from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaElement1dParameter(), "1D Elements in [" + unitAbbreviation + "]", "E1D",
        "1D Elements (Analysis Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElement2dParameter(), "2D Elements in [" + unitAbbreviation + "]", "E2D",
        "2D Elements (Analysis Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElement3dParameter(), "3D Elements in [" + unitAbbreviation + "]", "E3D",
        "3D Elements (Analysis Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaMember1dParameter(), "1D Members in [" + unitAbbreviation + "]", "M1D",
        "1D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMember2dParameter(), "2D Members in [" + unitAbbreviation + "]", "M2D",
        "2D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
      pManager.AddParameter(new GsaMember3dParameter(), "3D Members in [" + unitAbbreviation + "]", "M3D",
        "3D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
      pManager.AddParameter(new GsaAssemblyParameter(), "Assemblies", "As",
        "Assemblies from GSA Model", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess data) {
      if (InPreSolve) {
        GsaModelGoo modelGoo = null;
        data.GetData(0, ref modelGoo);

        if (modelGoo == null) {
          return;
        }

        bool nodeFilterHasInput = false;
        bool elementFilterHasInput = false;
        bool memberFilterHasInput = false;

        GsaListGoo nodeListGoo = null;
        string nodeList = "all";
        if (data.GetData(1, ref nodeListGoo)) {
          if (!nodeListGoo.IsValid) {
            return;
          }

          nodeList = Inputs.GetNodeListDefinition(this, data, 1, modelGoo.Value);
          nodeFilterHasInput = true;
        }

        GsaListGoo elementListGoo = null;
        string elemList = "all";
        if (data.GetData(2, ref elementListGoo)) {
          if (!elementListGoo.IsValid) {
            return;
          }

          elemList = Inputs.GetElementListDefinition(this, data, 2, modelGoo.Value);
          elementFilterHasInput = true;
        }

        GsaListGoo memberListGoo = null;
        string memList = "all";
        if (data.GetData(3, ref memberListGoo)) {
          if (!memberListGoo.IsValid) {
            return;
          }

          memList = memberListGoo.Value.Definition;
          memberFilterHasInput = true;
        }

        UpdateHiddenOutputs(nodeFilterHasInput, elementFilterHasInput, memberFilterHasInput);

        Task<SolveResults> tsk = null;
        tsk = Task.Run(
          () => Compute(modelGoo.Value, nodeList, elemList, memList), CancelToken);

        TaskList.Add(tsk);
        return;
      }

      if (!GetSolveResults(data, out SolveResults results)) {
        GsaModelGoo modelGoo = null;
        data.GetData(0, ref modelGoo);

        if (modelGoo == null) {
          return;
        }

        GsaListGoo nodeListGoo = null;
        string nodeList = "all";
        if (data.GetData(1, ref nodeListGoo)) {
          if (!nodeListGoo.IsValid) {
            return;
          }

          nodeList = nodeListGoo.Value.Definition;
        }

        GsaListGoo elementListGoo = null;
        string elemList = "all";
        if (data.GetData(2, ref elementListGoo)) {
          if (!elementListGoo.IsValid) {
            return;
          }

          elemList = elementListGoo.Value.Definition;
        }

        GsaListGoo memberListGoo = null;
        string memList = "all";
        if (data.GetData(3, ref memberListGoo)) {
          if (!memberListGoo.IsValid) {
            return;
          }

          memList = memberListGoo.Value.Definition;
        }

        results = Compute(modelGoo.Value, nodeList, elemList, memList);
      }

      if (results is null) {
        return;
      }

      _supportNodes = new ConcurrentBag<GsaNodeGoo>();
      if (!(results.Nodes is null)) {
        data.SetDataList(0, results.Nodes.OrderBy(item => item.Value.Id));
        if (_showSupports) {
          _supportNodes = results.DisplaySupports;
        }
      }

      if (!(results.Mem1ds is null) || results.Mem1ds.Count == 0) {
        var invalid1dMem = results.Mem1ds.Where(x => !x.IsValid).Select(x => x.Value.Id).ToList();
        if (invalid1dMem.Count > 0) {
          string ids = string.Join(Environment.NewLine, invalid1dMem.OrderBy(x => x));
          string err = $" Invalid definition for 1D Members ID(s):{Environment.NewLine}{ids}";
          this.AddRuntimeWarning(err);
        }

        if (_mode == FoldMode.List) {
          data.SetDataList(4, results.Mem1ds.OrderBy(item => item.Value.Id));
        } else {
          var tree = new DataTree<GsaMember1dGoo>();
          foreach (GsaMember1dGoo element in results.Mem1ds) {
            tree.Add(element, new GH_Path(element.Value.Section.Id));
          }

          data.SetDataTree(4, tree);
        }
      }

      if (!(results.Elem1ds is null) || results.Elem1ds.Count == 0) {
        var invalid1dElem = results.Elem1ds.Where(x => !x.IsValid).Select(x => x.Value.Id).ToList();
        if (invalid1dElem.Count > 0) {
          string ids = string.Join(Environment.NewLine, invalid1dElem.OrderBy(x => x));
          string err = $" Invalid definition for 1D Elements ID(s):{Environment.NewLine}{ids}";
          this.AddRuntimeWarning(err);
        }

        if (_mode == FoldMode.List) {
          data.SetDataList(1, results.Elem1ds.OrderBy(item => item.Value.Id));
        } else {
          var tree = new DataTree<GsaElement1dGoo>();
          foreach (GsaElement1dGoo element in results.Elem1ds) {
            tree.Add(element, new GH_Path(element.Value.Section.Id));
          }

          data.SetDataTree(1, tree);
        }
      }

      var member2dKeys = new List<int>();
      if (!(results.Mem2ds is null) || results.Mem2ds.Count == 0) {
        var invalid2dMem = results.Mem2ds.Where(x => !x.IsValid).Select(x => x.Value.Id).ToList();
        if (invalid2dMem.Count > 0) {
          string ids = string.Join(Environment.NewLine, invalid2dMem.OrderBy(x => x));
          string err = $" Invalid definition for 2D Member ID(s):{Environment.NewLine}{ids}";
          this.AddRuntimeWarning(err);
        }

        if (!((IGH_PreviewObject)Params.Output[5]).Hidden) {
          member2dKeys = results.Mem2ds.Select(item => item.Value.Id).ToList();
        }

        if (_mode == FoldMode.List) {
          data.SetDataList(5, results.Mem2ds.OrderBy(item => item.Value.Id));
        } else {
          var tree = new DataTree<GsaMember2dGoo>();
          foreach (GsaMember2dGoo element in results.Mem2ds) {
            tree.Add(element, new GH_Path(element.Value.Prop2d.Id));
          }

          data.SetDataTree(5, tree);
        }
      }

      _cachedDisplayGeometryWithParent = new ConcurrentBag<GeometryBase>();
      _cachedDisplayGeometryWithoutParent = new ConcurrentBag<GeometryBase>();
      if (!(results.Elem2ds is null) || results.Elem2ds.Count == 0) {
        if (_mode == FoldMode.List) {
          data.SetDataList(2, results.Elem2ds.OrderBy(item => item.Value.Ids.First()));
        } else {
          var tree = new DataTree<GsaElement2dGoo>();
          foreach (GsaElement2dGoo element in results.Elem2ds) {
            tree.Add(element, new GH_Path(element.Value.ApiElements.First().Property));
          }

          data.SetDataTree(2, tree);
        }

        if (!((IGH_PreviewObject)Params.Output[5]).Hidden) {
          member2dKeys = results.Mem2ds.Select(item => item.Value.Id).ToList();
          Parallel.ForEach(results.Elem2ds, elem => {
            int parent = elem.Value.ApiElements[0].ParentMember.Member;
            if (parent > 0 && member2dKeys.Contains(parent)) {
              if (elem.Value.IsLoadPanel) {
                _cachedDisplayGeometryWithParent.Add(elem.Value.Curve);
              } else {
                _cachedDisplayGeometryWithParent.Add(elem.Value.Mesh);
              }
            } else {
              if (elem.Value.IsLoadPanel) {
                _cachedDisplayGeometryWithoutParent.Add(elem.Value.Curve);
              } else {
                _cachedDisplayGeometryWithoutParent.Add(elem.Value.Mesh);
              }
            }
          });
        }
      }

      var member3dKeys = new List<int>();
      if (!(results.Mem3ds is null) || results.Mem3ds.Count == 0) {
        var invalid3dMem = results.Mem3ds.Where(x => !x.IsValid).Select(x => x.Value.Id).ToList();
        if (invalid3dMem.Count > 0) {
          string ids = string.Join(Environment.NewLine, invalid3dMem.OrderBy(x => x));
          string err = $" Invalid definition for 3D Members ID(s):{Environment.NewLine}{ids}";
          this.AddRuntimeWarning(err);
        }

        if (!((IGH_PreviewObject)Params.Output[6]).Hidden) {
          member3dKeys = results.Mem3ds.Select(item => item.Value.Id).ToList();
        }

        if (_mode == FoldMode.List) {
          data.SetDataList(6, results.Mem3ds.OrderBy(item => item.Value.Id));
        } else {
          var tree = new DataTree<GsaMember3dGoo>();
          foreach (GsaMember3dGoo element in results.Mem3ds) {
            tree.Add(element, new GH_Path(element.Value.Prop3d.Id));
          }

          data.SetDataTree(6, tree);
        }
      }

      _cachedDisplayNgonMeshWithParent = new ConcurrentBag<GeometryBase>();
      _cachedDisplayNgonMeshWithoutParent = new ConcurrentBag<GeometryBase>();
      if (!(results.Elem3ds is null) || results.Elem3ds.Count == 0) {
        if (_mode == FoldMode.List) {
          data.SetDataList(3, results.Elem3ds.OrderBy(item => item.Value.Ids.First()));
        } else {
          var tree = new DataTree<GsaElement3dGoo>();
          foreach (GsaElement3dGoo element in results.Elem3ds) {
            tree.Add(element, new GH_Path(element.Value.ApiElements.First().Property));
          }

          data.SetDataTree(3, tree);
        }

        if (!((IGH_PreviewObject)Params.Output[6]).Hidden) {
          var element3dsShaded = new ConcurrentBag<GsaElement3dGoo>();
          var element3dsNotShaded = new ConcurrentBag<GsaElement3dGoo>();
          Parallel.ForEach(results.Elem3ds, elem => {
            try {
              int parent = elem.Value.ApiElements[0].ParentMember.Member;
              if (parent > 0 && member3dKeys.Contains(parent)) {
                _cachedDisplayNgonMeshWithParent.Add(elem.Value.DisplayMesh);
              } else {
                _cachedDisplayNgonMeshWithoutParent.Add(elem.Value.DisplayMesh);
              }
            } catch (Exception) {
              _cachedDisplayNgonMeshWithoutParent.Add(elem.Value.DisplayMesh);
            }
          });
        }
      }

      if (!results.Assemblies.IsNullOrEmpty()) {
        GsaModelGoo modelGoo = null;
        data.GetData(0, ref modelGoo);

        ReadOnlyDictionary<int, Node> nodes = modelGoo.Value.ApiModel.Nodes();
        var gridPlanes = new ReadOnlyCollection<GridPlane>(modelGoo.Value.ApiModel.GridPlanes().Values.ToList());
        foreach (GsaAssemblyGoo assemblyGoo in results.Assemblies) {
          Assembly assembly = assemblyGoo.Value.ApiAssembly;
          var preview = new AssemblyPreview(assembly, nodes[assembly.Topology1], nodes[assembly.Topology2], nodes[assembly.OrientationNode], gridPlanes);
          _assemblyPreviews.Add(preview);
        }

        data.SetDataList(7, results.Assemblies.OrderBy(item => item.Value.Id));
      }
    }

    private SolveResults Compute(GsaModel model, string nodeList, string elemList, string memList) {
      _results = new SolveResults();
      var steps = new List<int> {
        0, 1, 2,
      };

      if (model.ModelUnit != _lengthUnit) {
        model.ModelUnit = _lengthUnit;
      }
      Utility.ModelGeometryLengthUnit = model.ModelUnit;
      _boundingBox = model.BoundingBox;

      try {
        Parallel.ForEach(steps, i => {
          switch (i) {
            case 0:
              _results.Nodes = Nodes.GetNodes(
                nodeList.ToLower() == "all" ? model.ApiNodes : model.ApiModel.Nodes(nodeList),
                model.ModelUnit,
                model.ApiAxis,
                model.SpringProps);
              _results.DisplaySupports
                = new ConcurrentBag<GsaNodeGoo>(_results.Nodes.Where(n => n.Value.IsSupport));
              break;

            case 1:
              var elements = new Elements(model, elemList);
              _results.Elem1ds = elements.Element1ds;
              _results.Elem2ds = elements.Element2ds;
              _results.Elem3ds = elements.Element3ds;
              _results.Assemblies = elements.Assemblies;
              break;

            case 2:
              var members = new Members(model, this, memList);
              _results.Mem1ds = members.Member1ds;
              _results.Mem2ds = members.Member2ds;
              _results.Mem3ds = members.Member3ds;
              break;
          }
        });
      } catch (Exception e) {
        this.AddRuntimeWarning(e.InnerException?.Message);
      }

      return _results;
    }

    internal void GraftModeClicked(object sender, EventArgs e) {
      if (_mode == FoldMode.Graft) {
        return;
      }

      RecordUndoEvent("Graft by Property");
      _mode = FoldMode.Graft;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      Message = "Graft by Property";
      ExpireSolution(true);
    }

    internal void ListModeClicked(object sender, EventArgs e) {
      if (_mode == FoldMode.List) {
        return;
      }

      RecordUndoEvent("List");
      _mode = FoldMode.List;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      Message = "Import as List";
      ExpireSolution(true);
    }

    private void UpdateHiddenOutputs(bool nodeFilter, bool elementFilter, bool memberFilter) {
      if (!nodeFilter && !elementFilter && !memberFilter) {
        // set default display options
        _showSupports = true;
        ((IGH_PreviewObject)Params.Output[0]).Hidden = true;
        ((IGH_PreviewObject)Params.Output[1]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[2]).Hidden = true;
        ((IGH_PreviewObject)Params.Output[3]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[4]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[5]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[6]).Hidden = false;
      } else {
        // set all to hidden
        _showSupports = false;
        for (int i = 0; i < Params.Output.Count - 1; i++) {
          ((IGH_PreviewObject)Params.Output[i]).Hidden = true;
        }
      }

      if (nodeFilter) {
        ((IGH_PreviewObject)Params.Output[0]).Hidden = false;
      }

      if (elementFilter) {
        ((IGH_PreviewObject)Params.Output[1]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[2]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[3]).Hidden = false;
      }

      if (memberFilter) {
        ((IGH_PreviewObject)Params.Output[4]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[5]).Hidden = false;
        ((IGH_PreviewObject)Params.Output[6]).Hidden = false;
      }
    }

    private void DrawGraphicMesh(IGH_PreviewArgs args, ConcurrentBag<GeometryBase> geometryEntities) {
      if (geometryEntities.IsNullOrEmpty()) {
        return;
      }

      foreach (GeometryBase entity in geometryEntities.Where(entity => entity != null)) {
        switch (entity) {
          case Curve curve:
            double tollerance = Rhino.RhinoDoc.ActiveDoc != null ? Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance :
              0.001;
            Brep[] PlanerBrep = Brep.CreatePlanarBreps(curve, tollerance);
            foreach (Brep brep in PlanerBrep) {
              DisplayMaterial displayMaterialLoadPanel = Attributes.Selected ? Colours.Element2dFaceSelectedLoadPanel : Colours.Element2dFaceLoadPanel;
              args.Display.DrawBrepShaded(brep, displayMaterialLoadPanel);
            }
            break;
          case Mesh mesh:
            DisplayMaterial displayMaterialFEA
              = Attributes.Selected ? Colours.Element2dFaceSelected : Colours.Element2dFace;
            args.Display.DrawMeshShaded(mesh, displayMaterialFEA);
            break;
          default: break;
        }
      }
    }

    private static void DrawGraphicWire(IGH_PreviewArgs args, ConcurrentBag<GeometryBase> geometryEntities, Color colour, int wireDensity = -1) {
      if (geometryEntities.IsNullOrEmpty()) {
        return;
      }
      foreach (GeometryBase entity in geometryEntities.Where(entity => entity != null)) {
        switch (entity) {
          case Curve curve:
            args.Display.DrawCurve(curve, colour, wireDensity);
            break;
          case Mesh mesh:
            args.Display.DrawMeshWires(mesh, colour, wireDensity);
            break;
          default: break;
        }
      }
    }

  }
}
