using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel.Data;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;

using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'. </para>
  /// <para>In Grasshopper, an Element3D is a collection of 3D Elements (mesh solids representing <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#brick-wedge-pyramid-and-tetra-elements">Brick, Wedge, Pyramid or Tetra Elements</see>) used for FE analysis. In GSA, a 3D Element is just a single closed mesh, but for Rhino performance reasons we have made Element3D an <see href="https://docs.mcneel.com/rhino/7/help/en-us/popup_moreinformation/ngon.htm">Ngon Mesh</see> that can contain more than one closed mesh.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-element.html">Elements</see> to read more.</para>
  ///
  /// </summary>
  public class GsaElement3d {
    public List<GSAElement> ApiElements { get; internal set; }
    public List<int> Ids { get; set; } = new List<int>();
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public Mesh NgonMesh { get; internal set; } = new Mesh();
    public List<List<int>> FaceInt { get; internal set; }
    public List<List<int>> TopoInt { get; internal set; }
    public Point3dList Topology { get; internal set; }
    public List<GsaProperty3d> Prop3ds { get; set; }
    public Mesh DisplayMesh {
      get {
        if (_displayMesh == null) {
          CreateDisplayMesh();
        }

        return _displayMesh;
      }
    }
    private Mesh _displayMesh;

    /// <summary>
    /// Empty constructor instantiating a list of new API objects
    /// </summary>
    public GsaElement3d() {
      ApiElements = new List<GSAElement>();
    }

    /// <summary>
    /// Create new instance by casting from a Mesh
    /// </summary>
    /// <param name="mesh"></param>
    public GsaElement3d(Mesh mesh) {
      if (mesh.IsClosed) {
        NgonMesh = mesh;
      } else {
        Mesh m = mesh.DuplicateMesh();
        m.FillHoles();
        NgonMesh = m;
      }

      InitVariablesFromMesh(mesh, true);
      Ids = new List<int>(new int[NgonMesh.Faces.Count]);
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaElement3d(GsaElement3d other) {
      Ids = other.Ids;
      NgonMesh = (Mesh)other.NgonMesh.DuplicateShallow();
      ApiElements = other.DuplicateApiObjects();
      Topology = other.Topology?.Duplicate();
      TopoInt = other.TopoInt;
      FaceInt = other.FaceInt;
      Prop3ds = other.Prop3ds;
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaElement3d(ConcurrentDictionary<int, GSAElement> elements, Mesh mesh,
      ConcurrentDictionary<int, GsaProperty3d> prop3ds) {
      NgonMesh = mesh;
      InitVariablesFromMesh(mesh, false);
      ApiElements = elements.OrderBy(kvp => kvp.Key).Select(kvp => DuplicateFrom(kvp.Value)).ToList();
      Ids = elements.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key).ToList();
      if (!prop3ds.IsNullOrEmpty()) {
        Prop3ds = prop3ds.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
      }
      UpdateMeshColours();
    }

    public List<GSAElement> DuplicateApiObjects() {
      if (ApiElements.IsNullOrEmpty()) {
        return ApiElements;
      }

      return ApiElements.Select(DuplicateFrom).ToList();
    }

    private static GSAElement DuplicateFrom(GSAElement source) {
      var element = new GSAElement(new Element()) {
        Group = source.Group,
        IsDummy = source.IsDummy,
        Name = source.Name.ToString(),
        OrientationNode = source.OrientationNode,
        OrientationAngle = source.OrientationAngle,
        ParentMember = source.ParentMember,
        Property = source.Property,
        Type = source.Type,
        Topology = new ReadOnlyCollection<int>(source.Topology.ToList()),
      };

      element.Offset.X1 = source.Offset.X1;
      element.Offset.X2 = source.Offset.X2;
      element.Offset.Y = source.Offset.Y;
      element.Offset.Z = source.Offset.Z;

      if ((Color)source.Colour != Color.FromArgb(0, 0, 0)) {
        element.Colour = source.Colour;
      }

      return element;
    }

    public DataTree<int> GetTopologyIDs() {
      var topos = new DataTree<int>();
      for (int i = 0; i < ApiElements.Count; i++) {
        if (ApiElements[i] != null) {
          topos.AddRange(ApiElements[i].Topology.ToList(), new GH_Path(Ids[i]));
        }
      }

      return topos;
    }

    public override string ToString() {
      if (NgonMesh == null || NgonMesh.Ngons.Count == 0) {
        return "Invalid 3D Element";
      }

      var types = ApiElements.Select(t => Mappings._elementTypeMapping.FirstOrDefault(
        x => x.Value == t.Type).Key).ToList();
      string type = string.Join("/", types.Distinct());
      string info = "N:" + NgonMesh.Vertices.Count + " E:" + ApiElements.Count;
      return string.Join(" ", type, info).TrimSpaces();
    }

    public void UpdateMeshColours() {
      for (int i = 0; i < ApiElements.Count; i++) {
        NgonMesh.VertexColors.SetColor(i, (Color)ApiElements[i].Colour);
      }
    }

    private void CreateDisplayMesh() {
      _displayMesh = new Mesh();
      Mesh x = NgonMesh;

      _displayMesh.Vertices.AddVertices(x.Vertices.ToList());
      var ngons = x.GetNgonAndFacesEnumerable().ToList();

      foreach (int faceIndex in ngons
       .Select(ngon => ngon.FaceIndexList().Select(u => (int)u).ToList())
       .SelectMany(faceindex => faceindex)) {
        _displayMesh.Faces.AddFace(x.Faces[faceIndex]);
      }

      _displayMesh.RebuildNormals();
    }

    private void InitVariablesFromMesh(Mesh mesh, bool setApiElements) {
      Tuple<List<GSAElement>, Point3dList, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(mesh);
      if (setApiElements) {
        ApiElements = convertMesh.Item1;
      }

      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      FaceInt = convertMesh.Item4;
    }

  }
}
