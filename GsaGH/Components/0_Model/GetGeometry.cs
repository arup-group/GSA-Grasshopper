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
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using Newtonsoft.Json;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Serialization.JsonNet;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve geometric objects from a GSA model
  /// </summary>
  public class GetGeometry : GH_OasysTaskCapableComponent<GetGeometry.SolveResults>,
    IGH_VariableParameterComponent {
    internal static GH_IWriter WriteDropDownComponents(
      ref GH_IWriter writer,
      List<List<string>> dropDownItems,
      List<string> selectedItems,
      List<string> spacerDescriptions) {
      bool dropdown = false;
      if (dropDownItems != null) {
        writer.SetInt32("dropdownCount", dropDownItems.Count);
        for (int i = 0; i < dropDownItems.Count; i++) {
          writer.SetInt32("dropdowncontentsCount" + i,
            dropDownItems[i]
              .Count);
          for (int j = 0;
            j
            < dropDownItems[i]
              .Count;
            j++)
            writer.SetString("dropdowncontents" + i + j, dropDownItems[i][j]);
        }

        dropdown = true;
      }

      writer.SetBoolean("dropdown", dropdown);

      bool spacer = false;
      if (spacerDescriptions != null) {
        writer.SetInt32("spacerCount", spacerDescriptions.Count);
        for (int i = 0; i < spacerDescriptions.Count; i++)
          writer.SetString("spacercontents" + i, spacerDescriptions[i]);
        spacer = true;
      }

      writer.SetBoolean("spacer", spacer);

      bool select = false;
      if (selectedItems != null) {
        writer.SetInt32("selectionCount", selectedItems.Count);
        for (int i = 0; i < selectedItems.Count; i++)
          writer.SetString("selectioncontents" + i, selectedItems[i]);
        select = true;
      }

      writer.SetBoolean("select", select);

      return writer;
    }

    internal static void ReadDropDownComponents(
      ref GH_IReader reader,
      ref List<List<string>> dropDownItems,
      ref List<string> selectedItems,
      ref List<string> spacerDescriptions) {
      if (reader.GetBoolean("dropdown")) {
        int dropdownCount = reader.GetInt32("dropdownCount");
        dropDownItems = new List<List<string>>();
        for (int i = 0; i < dropdownCount; i++) {
          int dropdowncontentsCount = reader.GetInt32("dropdowncontentsCount" + i);
          var tempcontent = new List<string>();
          for (int j = 0; j < dropdowncontentsCount; j++)
            tempcontent.Add(reader.GetString("dropdowncontents" + i + j));
          dropDownItems.Add(tempcontent);
        }
      }
      else
        throw new Exception("Component doesnt have 'dropdown' content stored");

      if (reader.GetBoolean("spacer")) {
        int dropdownspacerCount = reader.GetInt32("spacerCount");
        spacerDescriptions = new List<string>();
        for (int i = 0; i < dropdownspacerCount; i++)
          spacerDescriptions.Add(reader.GetString("spacercontents" + i));
      }

      if (!reader.GetBoolean("select"))
        return;

      int selectionsCount = reader.GetInt32("selectionCount");
      selectedItems = new List<string>();
      for (int i = 0; i < selectionsCount; i++)
        selectedItems.Add(reader.GetString("selectioncontents" + i));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("6c4cb686-a6d1-4a79-b01b-fadc5d6da520");

    public GetGeometry() : base("Get Model Geometry",
      "GetGeo",
      "Get nodes, elements and members from GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0()) { }

    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetGeometry;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(),
        "GSA Model",
        "GSA",
        "GSA model containing some geometry",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list",
        "No",
        "Filter import by list."
        + Environment.NewLine
        + "Node list should take the form:"
        + Environment.NewLine
        + " 1 11 to 72 step 2 not (XY3 31 to 45)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item,
        "All");
      pManager.AddTextParameter("Element filter list",
        "El",
        "Filter import by list."
        + Environment.NewLine
        + "Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item,
        "All");
      pManager.AddTextParameter("Member filter list",
        "Me",
        "Filter import by list."
        + Environment.NewLine
        + "Member list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item,
        "All");
      pManager[1]
        .Optional = true;
      pManager[2]
        .Optional = true;
      pManager[3]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGenericParameter("Nodes [" + unitAbbreviation + "]",
        "No",
        "Nodes from GSA Model",
        GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.AddGenericParameter("1D Elements [" + unitAbbreviation + "]",
        "E1D",
        "1D Elements (Analysis Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Elements [" + unitAbbreviation + "]",
        "E2D",
        "2D Elements (Analysis Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Elements [" + unitAbbreviation + "]",
        "E3D",
        "3D Elements (Analysis Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.list);
      pManager.HideParameter(2);
      pManager.AddGenericParameter("1D Members [" + unitAbbreviation + "]",
        "M1D",
        "1D Members (Design Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("2D Members [" + unitAbbreviation + "]",
        "M2D",
        "2D Members (Design Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("3D Members [" + unitAbbreviation + "]",
        "M3D",
        "3D Members (Design Layer) from GSA Model imported to selected unit",
        GH_ParamAccess.tree);
    }

    #endregion

    #region solve

    public class SolveResults {
      internal ConcurrentBag<GsaNodeGoo> Nodes { get; set; }
      internal ConcurrentBag<GsaNodeGoo> DisplaySupports { get; set; }
      internal ConcurrentBag<GsaElement1dGoo> Elem1ds { get; set; }
      internal ConcurrentBag<GsaElement2dGoo> Elem2ds { get; set; }
      internal ConcurrentBag<GsaElement3dGoo> Elem3ds { get; set; }
      internal ConcurrentBag<GsaMember1dGoo> Mem1ds { get; set; }
      internal ConcurrentBag<GsaMember2dGoo> Mem2ds { get; set; }
      internal ConcurrentBag<GsaMember3dGoo> Mem3ds { get; set; }
    }

    private SolveResults Compute(
      ReadOnlyDictionary<int, Node> allnDict,
      ReadOnlyDictionary<int, Axis> axDict,
      ReadOnlyDictionary<int, Node> nDict,
      ReadOnlyDictionary<int, Element> eDict,
      ReadOnlyDictionary<int, Member> mDict,
      ReadOnlyDictionary<int, Section> sDict,
      ReadOnlyDictionary<int, SectionModifier> modDict,
      ReadOnlyDictionary<int, Prop2D> pDict,
      ReadOnlyDictionary<int, Prop3D> p3Dict,
      ReadOnlyDictionary<int, AnalysisMaterial> matDict,
      Dictionary<int, ReadOnlyCollection<double>> elementLocalAxesDict,
      Dictionary<int, ReadOnlyCollection<double>> memberLocalAxesDict) {
      var results = new SolveResults();
      var steps = new List<int> {
        0,
        1,
        2,
      };

      try {
        Parallel.ForEach(steps,
          i => {
            switch (i) {
              case 0:
                results.Nodes = Nodes.GetNodes(nDict, _lengthUnit, axDict);
                results.DisplaySupports
                  = new ConcurrentBag<GsaNodeGoo>(results.Nodes.Where(n => n.Value.IsSupport));
                break;
              case 1:
                Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>,
                  ConcurrentBag<GsaElement3dGoo>> elementTuple = Elements.GetElements(eDict,
                  allnDict,
                  sDict,
                  pDict,
                  p3Dict,
                  matDict,
                  modDict,
                  elementLocalAxesDict,
                  axDict,
                  _lengthUnit,
                  false);

                results.Elem1ds = elementTuple.Item1;
                results.Elem2ds = elementTuple.Item2;
                results.Elem3ds = elementTuple.Item3;
                break;
              case 2:
                Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>,
                  ConcurrentBag<GsaMember3dGoo>> memberTuple = Members.GetMembers(mDict,
                  allnDict,
                  sDict,
                  pDict,
                  p3Dict,
                  matDict,
                  modDict,
                  memberLocalAxesDict,
                  axDict,
                  _lengthUnit,
                  false,
                  this);

                results.Mem1ds = memberTuple.Item1;
                results.Mem2ds = memberTuple.Item2;
                results.Mem3ds = memberTuple.Item3;
                break;
            }
          });
      }
      catch (Exception e) {
        this.AddRuntimeWarning(e.InnerException?.Message);
      }

      return results;
    }

    protected override void SolveInstance(IGH_DataAccess data) {
      var memberKeys = new List<int>();
      if (InPreSolve) {
        var gsaModel = new GsaModel();
        var ghTyp = new GH_ObjectWrapper();
        Task<SolveResults> tsk = null;
        if (data.GetData(0, ref ghTyp)) {
          if (ghTyp.Value is GsaModelGoo)
            ghTyp.CastTo(ref gsaModel);
          else {
            this.AddRuntimeError("Error converting input to GSA Model");
            return;
          }

          string nodeList = "all";
          data.GetData(1, ref nodeList);
          string elemList = "all";
          data.GetData(2, ref elemList);
          string memList = "all";
          data.GetData(3, ref memList);

          Model model = gsaModel.Model;
          ReadOnlyDictionary<int, Node> nDict = model.Nodes();
          ReadOnlyDictionary<int, Axis> axDict = model.Axes();
          ReadOnlyDictionary<int, Node> allNDict = (nodeList.ToLower() == "all")
            ? nDict
            : model.Nodes(nodeList);
          ReadOnlyDictionary<int, Element> eDict = model.Elements(elemList);
          ReadOnlyDictionary<int, Member> mDict = model.Members(memList);
          ReadOnlyDictionary<int, Section> sDict = model.Sections();
          ReadOnlyDictionary<int, SectionModifier> modDict = model.SectionModifiers();
          ReadOnlyDictionary<int, Prop2D> pDict = model.Prop2Ds();
          ReadOnlyDictionary<int, Prop3D> p3Dict = model.Prop3Ds();
          ReadOnlyDictionary<int, AnalysisMaterial> amDict = model.AnalysisMaterials();

          var elementLocalAxesDict
            = eDict.Keys.ToDictionary(id => id, id => model.ElementDirectionCosine(id));
          var memberLocalAxesDict
            = mDict.Keys.ToDictionary(id => id, id => model.MemberDirectionCosine(id));

          tsk = Task.Run(() => Compute(nDict,
              axDict,
              allNDict,
              eDict,
              mDict,
              sDict,
              modDict,
              pDict,
              p3Dict,
              amDict,
              elementLocalAxesDict,
              memberLocalAxesDict),
            CancelToken);
        }

        TaskList.Add(tsk);
        return;
      }

      if (!GetSolveResults(data, out SolveResults results)) {
        var gsaModel = new GsaModel();
        var ghTyp = new GH_ObjectWrapper();
        if (data.GetData(0, ref ghTyp)) {
          if (ghTyp.Value is GsaModelGoo)
            ghTyp.CastTo(ref gsaModel);
          else {
            this.AddRuntimeError("Error converting input to GSA Model");
            return;
          }

          string nodeList = "all";
          data.GetData(1, ref nodeList);
          string elemList = "all";
          data.GetData(2, ref elemList);
          string memList = "all";
          data.GetData(3, ref memList);

          Model model = gsaModel.Model;
          ReadOnlyDictionary<int, Node> nDict = model.Nodes();
          ReadOnlyDictionary<int, Axis> axDict = model.Axes();
          ReadOnlyDictionary<int, Node> allNDict = (nodeList.ToLower() == "all")
            ? nDict
            : model.Nodes(nodeList);
          ReadOnlyDictionary<int, Element> eDict = model.Elements(elemList);
          ReadOnlyDictionary<int, Member> mDict = model.Members(memList);
          memberKeys = mDict.Keys.ToList();
          ReadOnlyDictionary<int, Section> sDict = model.Sections();
          ReadOnlyDictionary<int, SectionModifier> modDict = model.SectionModifiers();
          ReadOnlyDictionary<int, Prop2D> pDict = model.Prop2Ds();
          ReadOnlyDictionary<int, Prop3D> p3Dict = model.Prop3Ds();
          ReadOnlyDictionary<int, AnalysisMaterial> amDict = model.AnalysisMaterials();

          var elementLocalAxesDict
            = eDict.Keys.ToDictionary(id => id, id => model.ElementDirectionCosine(id));
          var memberLocalAxesDict
            = mDict.Keys.ToDictionary(id => id, id => model.MemberDirectionCosine(id));

          results = Compute(nDict,
            axDict,
            allNDict,
            eDict,
            mDict,
            sDict,
            modDict,
            pDict,
            p3Dict,
            amDict,
            elementLocalAxesDict,
            memberLocalAxesDict);
        }
        else
          return;
      }

      if (results is null)
        return;

      if (!(results.Nodes is null)) {
        data.SetDataList(0, results.Nodes.OrderBy(item => item.Value.Id));
        _supportNodes = results.DisplaySupports;
        _boundingBox = new BoundingBox(results.Nodes.Select(n => n.Value.Point)
          .ToArray());
      }

      if (!(results.Elem1ds is null)) {
        var invalid1dElem = results.Elem1ds.Where(x => !x.IsValid)
          .Select(x => x.Value.Id)
          .ToList();
        if (invalid1dElem.Count > 0) {
          this.AddRuntimeWarning("Invalid Element1D definition for Element IDs:");
          this.AddRuntimeWarning(string.Join(" ", invalid1dElem.OrderBy(x => x)));
        }

        if (_mode == FoldMode.List)
          data.SetDataList(1, results.Elem1ds.OrderBy(item => item.Value.Id));
        else {
          var tree = new DataTree<GsaElement1dGoo>();
          foreach (GsaElement1dGoo element in results.Elem1ds)
            tree.Add(element, new GH_Path(element.Value.Section.Id));
          data.SetDataTree(1, tree);
        }
      }

      if (!(results.Elem2ds is null)) {
        if (_mode == FoldMode.List)
          data.SetDataList(2, results.Elem2ds.OrderBy(item => item.Value.Ids.First()));
        else {
          var tree = new DataTree<GsaElement2dGoo>();
          foreach (GsaElement2dGoo element in results.Elem2ds)
            tree.Add(element,
              new GH_Path(element.Value.Properties.First()
                .Id));
          data.SetDataTree(2, tree);
        }

        _element2ds = results.Elem2ds;

        var element2dsShaded = new ConcurrentBag<GsaElement2dGoo>();
        var element2dsNotShaded = new ConcurrentBag<GsaElement2dGoo>();
        Parallel.ForEach(_element2ds,
          elem => {
            try {
              int parent = elem.Value.ApiElements[0]
                .ParentMember.Member;
              if (parent > 0 && memberKeys.Contains(parent))
                element2dsShaded.Add(elem);
              else
                element2dsNotShaded.Add(elem);
            }
            catch (Exception) {
              element2dsNotShaded.Add(elem);
            }
          });
        _cachedDisplayMeshWithParent = new Mesh();
        _cachedDisplayMeshWithParent.Append(element2dsShaded.Select(e => e.Value.Mesh));
        _cachedDisplayMeshWithoutParent = new Mesh();
        _cachedDisplayMeshWithoutParent.Append(element2dsNotShaded.Select(e => e.Value.Mesh));
      }

      if (!(results.Elem3ds is null)) {
        if (_mode == FoldMode.List)
          data.SetDataList(3, results.Elem3ds.OrderBy(item => item.Value.Ids.First()));
        else {
          var tree = new DataTree<GsaElement3dGoo>();
          foreach (GsaElement3dGoo element in results.Elem3ds)
            tree.Add(element, new GH_Path(element.Value.PropertyIDs.First()));
          data.SetDataTree(3, tree);
        }

        _element3ds = results.Elem3ds;
        var element3dsShaded = new ConcurrentBag<GsaElement3dGoo>();
        var element3dsNotShaded = new ConcurrentBag<GsaElement3dGoo>();
        Parallel.ForEach(_element3ds,
          elem => {
            try {
              int parent = elem.Value.ApiElements[0]
                .ParentMember.Member;
              if (parent > 0 && memberKeys.Contains(parent))
                element3dsShaded.Add(elem);
              else
                element3dsNotShaded.Add(elem);
            }
            catch (Exception) {
              element3dsNotShaded.Add(elem);
            }
          });
        _cachedDisplayNgonMeshWithParent = new Mesh();
        _cachedDisplayNgonMeshWithParent.Append(element3dsShaded.Select(e => e.Value.DisplayMesh));
        _cachedDisplayNgonMeshWithoutParent = new Mesh();
        _cachedDisplayNgonMeshWithoutParent.Append(
          element3dsNotShaded.Select(e => e.Value.DisplayMesh));
      }

      if (!(results.Mem1ds is null)) {
        var invalid1dMem = results.Mem1ds.Where(x => !x.IsValid)
          .Select(x => x.Value.Id)
          .ToList();
        if (invalid1dMem.Count > 0) {
          this.AddRuntimeWarning("Invalid Member1D definition for Member IDs:");
          this.AddRuntimeWarning(string.Join(" ", invalid1dMem.OrderBy(x => x)));
        }

        if (_mode == FoldMode.List)
          data.SetDataList(4, results.Mem1ds.OrderBy(item => item.Value.Id));
        else {
          var tree = new DataTree<GsaMember1dGoo>();
          foreach (GsaMember1dGoo element in results.Mem1ds)
            tree.Add(element, new GH_Path(element.Value.Section.Id));
          data.SetDataTree(4, tree);
        }
      }

      if (!(results.Mem2ds is null)) {
        var invalid2dMem = results.Mem2ds.Where(x => !x.IsValid)
          .Select(x => x.Value.Id)
          .ToList();
        if (invalid2dMem.Count > 0) {
          this.AddRuntimeWarning("Invalid Member2D definition for Member IDs:");
          this.AddRuntimeWarning(string.Join(" ", invalid2dMem.OrderBy(x => x)));
        }

        if (_mode == FoldMode.List)
          data.SetDataList(5, results.Mem2ds.OrderBy(item => item.Value.Id));
        else {
          var tree = new DataTree<GsaMember2dGoo>();
          foreach (GsaMember2dGoo element in results.Mem2ds)
            tree.Add(element, new GH_Path(element.Value.Property.Id));
          data.SetDataTree(5, tree);
        }
      }

      if (results.Mem3ds is null)
        return;

      {
        var invalid3dMem = results.Mem3ds.Where(x => !x.IsValid)
          .Select(x => x.Value.Id)
          .ToList();
        if (invalid3dMem.Count > 0) {
          this.AddRuntimeWarning("Invalid Member3D definition for Member IDs:");
          this.AddRuntimeWarning(string.Join(" ", invalid3dMem.OrderBy(x => x)));
        }

        if (_mode == FoldMode.List)
          data.SetDataList(6, results.Mem3ds.OrderBy(item => item.Value.Id));
        else {
          var tree = new DataTree<GsaMember3dGoo>();
          foreach (GsaMember3dGoo element in results.Mem3ds)
            tree.Add(element, new GH_Path(element.Value.Prop3d.Id));
          data.SetDataTree(6, tree);
        }
      }
    }

    #endregion

    #region custom preview

    private BoundingBox _boundingBox;
    private ConcurrentBag<GsaElement2dGoo> _element2ds;
    private ConcurrentBag<GsaElement3dGoo> _element3ds;
    private Mesh _cachedDisplayMeshWithParent;
    private Mesh _cachedDisplayMeshWithoutParent;
    private Mesh _cachedDisplayNgonMeshWithParent;
    private Mesh _cachedDisplayNgonMeshWithoutParent;
    private ConcurrentBag<GsaNodeGoo> _supportNodes;
    public override BoundingBox ClippingBox => _boundingBox;

    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
      base.DrawViewportMeshes(args);
      if (Attributes.Selected) {
        if (_cachedDisplayMeshWithoutParent != null)
          args.Display.DrawMeshShaded(_cachedDisplayMeshWithoutParent, Colours.Element2dFace);
        if (_cachedDisplayNgonMeshWithoutParent != null)
          args.Display.DrawMeshShaded(_cachedDisplayNgonMeshWithoutParent, Colours.Element2dFace);
      }
      else {
        if (_cachedDisplayMeshWithoutParent != null)
          args.Display.DrawMeshShaded(_cachedDisplayMeshWithoutParent,
            Colours.Element2dFaceSelected);
        if (_cachedDisplayNgonMeshWithoutParent != null)
          args.Display.DrawMeshShaded(_cachedDisplayNgonMeshWithoutParent,
            Colours.Element2dFaceSelected);
      }
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      if (_cachedDisplayMeshWithParent != null)
        args.Display.DrawMeshWires(_cachedDisplayMeshWithParent,
          Color.FromArgb(255, 229, 229, 229),
          1);

      if (_cachedDisplayNgonMeshWithParent != null)
        args.Display.DrawMeshWires(_cachedDisplayNgonMeshWithParent,
          Color.FromArgb(255, 229, 229, 229),
          1);

      if (_cachedDisplayMeshWithoutParent != null) {
        if (Attributes.Selected)
          args.Display.DrawMeshWires(_cachedDisplayMeshWithoutParent,
            Colours.Element2dEdgeSelected,
            2);
        else
          args.Display.DrawMeshWires(_cachedDisplayMeshWithoutParent, Colours.Element2dEdge, 1);
      }

      if (_cachedDisplayNgonMeshWithoutParent != null) {
        if (Attributes.Selected)
          args.Display.DrawMeshWires(_cachedDisplayNgonMeshWithoutParent,
            Colours.Element2dEdgeSelected,
            2);
        else
          args.Display.DrawMeshWires(_cachedDisplayNgonMeshWithoutParent, Colours.Element2dEdge, 1);
      }

      if (_supportNodes == null)
        return;

      foreach (GsaNodeGoo node in _supportNodes) {
        if (node.Value.Point.IsValid) {
          if (!Attributes.Selected) {
            if (node.Value.Colour != Color.FromArgb(0, 0, 0))
              args.Display.DrawPoint(node.Value.Point,
                PointStyle.RoundSimple,
                3,
                node.Value.Colour);
            else {
              Color col = Colours.Node;
              args.Display.DrawPoint(node.Value.Point, PointStyle.RoundSimple, 3, col);
            }

            if (node.Value._previewSupportSymbol != null)
              args.Display.DrawBrepShaded(node.Value._previewSupportSymbol, Colours.SupportSymbol);
            if (node.Value._previewText != null)
              args.Display.Draw3dText(node.Value._previewText, Colours.Support);
          }
          else {
            args.Display.DrawPoint(node.Value.Point,
              PointStyle.RoundControlPoint,
              3,
              Colours.NodeSelected);
            if (node.Value._previewSupportSymbol != null)
              args.Display.DrawBrepShaded(node.Value._previewSupportSymbol,
                Colours.SupportSymbolSelected);
            if (node.Value._previewText != null)
              args.Display.Draw3dText(node.Value._previewText, Colours.NodeSelected);
          }

          if (!(node.Value.LocalAxis != Plane.WorldXY
            & node.Value.LocalAxis != new Plane()
            & node.Value.LocalAxis != Plane.Unset))
            continue;

          args.Display.DrawLine(node.Value._previewXaxis, Color.FromArgb(255, 244, 96, 96), 1);
          args.Display.DrawLine(node.Value._previewYaxis, Color.FromArgb(255, 96, 244, 96), 1);
          args.Display.DrawLine(node.Value._previewZaxis, Color.FromArgb(255, 96, 96, 234), 1);
        }
      }
    }

    #endregion

    #region custom UI

    public List<List<string>> DropDownItems;

    public List<string> SelectedItems;

    public List<string> SpacerDescriptions;

    public bool IsInitialised;

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void CreateAttributes() {
      if (!IsInitialised)
        InitialiseDropdowns();

      m_attributes = new DropDownComponentAttributes(this,
        SetSelected,
        DropDownItems,
        SelectedItems,
        SpacerDescriptions);
    }

    public void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);
      UpdateUi();
    }

    public void UpdateUiFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[0]);
      CreateAttributes();
      UpdateUi();
    }

    public virtual void UpdateUi() {
      ((IGH_VariableParameterComponent)this).VariableParameterMaintenance();
      ExpireSolution(recompute: true);
      Params.OnParametersChanged();
      OnDisplayExpired(redraw: true);
    }

    void IGH_VariableParameterComponent.VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      int i = 0;
      Params.Output[i++]
        .Name = "Nodes [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "1D Elements [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "2D Elements [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "3D Elements [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "1D Members [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "2D Members [" + unitAbbreviation + "]";
      Params.Output[i]
        .Name = "3D Members [" + unitAbbreviation + "]";

      i = 1;
      for (int j = 1; j < 7; j++)
        Params.Output[i]
          .Access = _mode == FoldMode.List
          ? GH_ParamAccess.list
          : GH_ParamAccess.tree;
    }

    #endregion

    #region right-click menu item

    private enum FoldMode {
      Graft,
      List,
    }

    private FoldMode _mode = FoldMode.List;

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }
      Menu_AppendItem(menu, "Graft by Property", GraftModeClicked, true, _mode == FoldMode.Graft);
      Menu_AppendItem(menu, "List", ListModeClicked, true, _mode == FoldMode.List);
    }

    private void GraftModeClicked(object sender, EventArgs e) {
      if (_mode == FoldMode.Graft)
        return;

      RecordUndoEvent("Graft by Property");
      _mode = FoldMode.Graft;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      Message = "Graft by Property";
      ExpireSolution(true);
    }

    private void ListModeClicked(object sender, EventArgs e) {
      if (_mode == FoldMode.List)
        return;

      RecordUndoEvent("List");
      _mode = FoldMode.List;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      Message = "Import as List";
      ExpireSolution(true);
    }

    #endregion

    #region expire downstream

    private static readonly OasysUnitsIQuantityJsonConverter s_converter
      = new OasysUnitsIQuantityJsonConverter();

    public bool AlwaysExpireDownStream;

    public Dictionary<int, List<string>> ExistingOutputsSerialized
      = new Dictionary<int, List<string>>();

    private Dictionary<int, List<bool>> _outputsAreExpired = new Dictionary<int, List<bool>>();

    private Dictionary<int, bool> _outputIsExpired = new Dictionary<int, bool>();

    protected override void ExpireDownStreamObjects() {
      if (AlwaysExpireDownStream) {
        base.ExpireDownStreamObjects();
        return;
      }

      SetExpireDownStream();
      if (_outputIsExpired.Count > 0) {
        for (int i = 0; i < Params.Output.Count; i++)
          if (_outputIsExpired[i])
            Params.Output[i]
              .ExpireSolution(recompute: false);
      }
      else
        base.ExpireDownStreamObjects();
    }

    private void SetExpireDownStream() {
      if (_outputsAreExpired == null || _outputsAreExpired.Count <= 0)
        return;

      _outputIsExpired = new Dictionary<int, bool>();
      for (int i = 0; i < Params.Output.Count; i++)
        if (_outputsAreExpired.ContainsKey(i))
          _outputIsExpired.Add(i,
            _outputsAreExpired[i]
              .Any(c => c));
        else
          _outputIsExpired.Add(i, value: true);
    }

    public void OutputChanged<T>(T data, int outputIndex, int index) where T : IGH_Goo {
      if (!ExistingOutputsSerialized.ContainsKey(outputIndex)) {
        ExistingOutputsSerialized.Add(outputIndex, new List<string>());
        _outputsAreExpired.Add(outputIndex, new List<bool>());
      }

      string text;
      if (data.GetType() == typeof(GH_UnitNumber))
        text = JsonConvert.SerializeObject(((GH_UnitNumber)(object)data).Value, s_converter);
      else {
        object value = data.ScriptVariable();
        try {
          text = JsonConvert.SerializeObject(value);
        }
        catch (Exception) {
          text = data.GetHashCode()
            .ToString();
        }
      }

      if (ExistingOutputsSerialized[outputIndex]
          .Count
        == index) {
        ExistingOutputsSerialized[outputIndex]
          .Add(text);
        _outputsAreExpired[outputIndex]
          .Add(item: true);
      }
      else if (ExistingOutputsSerialized[outputIndex][index] != text) {
        ExistingOutputsSerialized[outputIndex][index] = text;
        _outputsAreExpired[outputIndex][index] = true;
      }
      else
        _outputsAreExpired[outputIndex][index] = false;
    }

    #endregion

    #region deserialization

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      WriteDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);

      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      ReadDropDownComponents(ref reader,
        ref DropDownItems,
        ref SelectedItems,
        ref SpacerDescriptions);
      IsInitialised = true;
      UpdateUiFromSelectedItems();
      return base.Read(reader);
    }

    #endregion

    #region variable component null implementation

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
      => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
      => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
      => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;

    #endregion
  }
}
