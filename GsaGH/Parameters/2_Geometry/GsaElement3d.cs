using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel.Data;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Element3d class, this class defines the basic properties and methods for any Gsa Element 3d
  /// </summary>
  public class GsaElement3d {
    private enum ApiObjectMember {
      All,
      Group,
      Dummy,
      Name,
      OrientationAngle,
      Offset,
      Property,
      Type,
      Colour,
    }

    public List<Color> Colours {
      get {
        var cols = new List<Color>();
        for (int i = 0; i < _elements.Count; i++) {
          if ((Color)_elements[i].Colour == Color.FromArgb(0, 0, 0))
            _elements[i]
              .Colour = Color.FromArgb(50, 150, 150, 150);
          cols.Add((Color)_elements[i]
            .Colour);

          NgonMesh.VertexColors.SetColor(i,
            (Color)_elements[i]
              .Colour);
        }

        return cols;
      }
      set
        => CloneApiElements(ApiObjectMember.Colour,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          value);
    }
    public int Count => _elements.Count;
    public Mesh DisplayMesh {
      get {
        if (_displayMesh == null)
          UpdatePreview();
        return _displayMesh;
      }
    }
    public List<List<int>> FaceInt {
      get => _faceInt;
      set => _faceInt = value;
    }
    public List<int> Groups {
      get => (from element
            in _elements
              where element != null
              select element.Group)
        .ToList();
      set => CloneApiElements(ApiObjectMember.Group, value);
    }
    public Guid Guid => _guid;
    public List<int> Ids {
      get => _ids;
      set => _ids = value;
    }
    public List<bool> IsDummies {
      get => (from element
              in _elements
              where element != null
              select element.IsDummy)
        .ToList();
      set => CloneApiElements(ApiObjectMember.Dummy, null, value);
    }
    public List<string> Names {
      get => (from element
              in _elements
              where element != null
              select element.Name)
        .ToList();
      set => CloneApiElements(ApiObjectMember.Dummy, null, null, value);
    }
    public Mesh NgonMesh => _mesh;
    public List<GsaOffset> Offsets {
      get => (from element
              in _elements
              where element != null
              select new GsaOffset(element.Offset.X1,
        element.Offset.X2,
        element.Offset.Y,
        element.Offset.Z))
        .ToList();
      set
        => CloneApiElements(ApiObjectMember.Dummy,
          null,
          null,
          null,
          null,
          value);
    }
    public List<double> OrientationAngles {
      get => (from element
            in _elements
              where element != null
              select element.OrientationAngle)
        .ToList();
      set => CloneApiElements(ApiObjectMember.Dummy, null, null, null, value);
    }
    public List<int> ParentMembers {
      get {
        var pMems = new List<int>();
        foreach (Element element in _elements)
          try {
            pMems.Add(element.ParentMember.Member);
          }
          catch (Exception) {
            pMems.Add(0);
          }

        return pMems;
      }
    }
    public List<GsaProp3d> Properties {
      get => _props;
      set => _props = value;
    }
    public List<int> PropertyIDs {
      get => (from element
              in _elements
              where element != null
              select element.Property)
        .ToList();
      set
        => CloneApiElements(ApiObjectMember.Dummy,
          null,
          null,
          null,
          null,
          null,
          value);
    }
    public List<List<int>> TopoInt {
      get => _topoInt;
      set => _topoInt = value;
    }
    public List<Point3d> Topology {
      get => _topo;
      set => _topo = value;
    }
    public DataTree<int> TopologyIDs {
      get {
        var topos = new DataTree<int>();
        for (int i = 0; i < _elements.Count; i++)
          if (_elements[i] != null)
            topos.AddRange(_elements[i]
                .Topology.ToList(),
              new GH_Path(i));
        return topos;
      }
    }
    public List<ElementType> Types {
      get => (from element
              in _elements
              where element != null
              select element.Type)
        .ToList();
      set
        => CloneApiElements(ApiObjectMember.Dummy,
          null,
          null,
          null,
          null,
          null,
          null,
          value);
    }
    internal List<Element> ApiElements {
      get => _elements;
      set => _elements = value;
    }
    private Mesh _displayMesh;
    private List<Element> _elements = new List<Element>();
    private List<List<int>> _faceInt;
    private Guid _guid = Guid.NewGuid();
    private List<int> _ids = new List<int>();
    private Mesh _mesh = new Mesh();
    private List<GsaProp3d> _props = new List<GsaProp3d>();
    // list of face integers included in each solid mesh referring to the mesh face list
    private List<Point3d> _topo;
    private List<List<int>> _topoInt;

    public GsaElement3d() {
    }

    public GsaElement3d(Mesh mesh) {
      _mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;

      _ids = new List<int>(new int[_mesh.Faces.Count]);

      _props = new List<GsaProp3d>();
      for (int i = 0; i < _mesh.Faces.Count; i++)
        _props.Add(new GsaProp3d());
      UpdatePreview();
    }

    internal GsaElement3d(Element element, int id, Mesh mesh, GsaProp3d prop3d) {
      _elements = new List<Element>() {
        element,
      };
      _mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;
      _ids = new List<int>() {
        id,
      };
      _props = new List<GsaProp3d>() {
        prop3d,
      };
      UpdatePreview();
    }

    internal GsaElement3d(
      ConcurrentDictionary<int, Element> elements,
      Mesh mesh,
      List<GsaProp3d> prop3ds) {
      _elements = elements.Values.ToList();
      _mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;
      _ids = elements.Keys.ToList();
      _props = prop3ds;
      UpdatePreview();
    }

    public GsaElement3d Duplicate(bool cloneApiElements = false) {
      var dup = new GsaElement3d {
        _mesh = (Mesh)_mesh.DuplicateShallow(),
        _guid = new Guid(_guid.ToString()),
        _topo = _topo,
        _topoInt = _topoInt,
        _faceInt = _faceInt,
        _elements = _elements,
      };
      if (cloneApiElements)
        dup.CloneApiElements();
      dup._ids = _ids.ToList();
      dup._props = _props.ConvertAll(x => x.Duplicate());
      dup.UpdatePreview();
      return dup;
    }

    public GsaElement3d Morph(SpaceMorph xmorph) {
      if (NgonMesh == null)
        return null;

      GsaElement3d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.NgonMesh.Faces.Count]);

      Mesh xMs = dup.NgonMesh.DuplicateMesh();
      xmorph.Morph(xMs);

      return dup.UpdateGeometry(xMs);
    }

    public override string ToString() {
      if (!(_mesh.Ngons.Count > 0))
        return "Null";
      var types = Types.Select(t => Mappings.s_elementTypeMapping.FirstOrDefault(x => x.Value == t)
          .Key)
        .ToList();
      string type = string.Join("/", types.Distinct());
      string info = "N:" + NgonMesh.Vertices.Count + " E:" + ApiElements.Count;
      return string.Join(" ", type.Trim(), info.Trim())
        .Trim()
        .Replace("  ", " ");
    }

    public GsaElement3d Transform(Transform xform) {
      if (NgonMesh == null)
        return null;

      GsaElement3d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.NgonMesh.Faces.Count]);

      Mesh xMs = dup.NgonMesh.DuplicateMesh();
      xMs.Transform(xform);

      return dup.UpdateGeometry(xMs);
    }

    /// <summary>
    ///   This method will return a copy of the existing element3d with an updated mesh
    /// </summary>
    /// <param name="updatedMesh"></param>
    /// <returns></returns>
    public GsaElement3d UpdateGeometry(Mesh updatedMesh) {
      GsaElement3d dup = Duplicate(true);
      _mesh = updatedMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(_mesh, 0);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;
      return dup;
    }

    internal void CloneApiElements() {
      CloneApiElements(ApiObjectMember.All);
      _guid = Guid.NewGuid();
    }

    internal Element GetApiObjectClone(int i) {
      var dup = new Element() {
        Group = _elements[i]
          .Group,
        IsDummy = _elements[i]
          .IsDummy,
        Name = _elements[i]
          .Name.ToString(),
        OrientationNode = _elements[i]
          .OrientationNode,
        OrientationAngle = _elements[i]
          .OrientationAngle,
        Offset = _elements[i]
          .Offset,
        ParentMember = _elements[i]
          .ParentMember,
        Property = _elements[i]
          .Property,
        Type = _elements[i]
          .Type,
        Topology = new ReadOnlyCollection<int>(_elements[i]
        .Topology.ToList()
      )
      };
      return dup;
    }

    internal void UpdatePreview() {
      _displayMesh = new Mesh();
      Mesh x = NgonMesh;

      _displayMesh.Vertices.AddVertices(x.Vertices.ToList());
      var ngons = x.GetNgonAndFacesEnumerable()
        .ToList();

      foreach (int faceIndex in ngons.Select(ngon => ngon
        .FaceIndexList()
        .Select(u => (int)u)
        .ToList())
        .SelectMany(faceindex => faceindex)) {
        _displayMesh.Faces.AddFace(x.Faces[faceIndex]);
      }

      _displayMesh.RebuildNormals();
    }

    // list of topology integers referring to the topo list of points
    // list of topology points for visualisation
    private void CloneApiElements(
      ApiObjectMember memType,
      IList<int> grp = null,
      IList<bool> dum = null,
      IList<string> nm = null,
      IList<double> oriA = null,
      IList<GsaOffset> off = null,
      IList<int> prop = null,
      IList<ElementType> typ = null,
      IList<Color> col = null) {
      var elems = new List<Element>();
      for (int i = 0; i < _elements.Count; i++) {
        elems.Add(new Element() {
          Group = _elements[i]
            .Group,
          IsDummy = _elements[i]
            .IsDummy,
          Name = _elements[i]
            .Name.ToString(),
          OrientationNode = _elements[i]
            .OrientationNode,
          OrientationAngle = _elements[i]
            .OrientationAngle,
          Offset = _elements[i]
            .Offset,
          ParentMember = _elements[i]
            .ParentMember,
          Property = _elements[i]
            .Property,
          Topology = new ReadOnlyCollection<int>(_elements[i]
            .Topology.ToList()),
          Type = _elements[i]
            .Type,
        });

        if (memType == ApiObjectMember.All)
          continue;

        switch (memType) {
          case ApiObjectMember.Group:
            elems[i].Group = grp.Count > i
              ? grp[i]
              : grp.Last();
            break;

          case ApiObjectMember.Dummy:
            elems[i].IsDummy = dum.Count > i
              ? dum[i]
              : dum.Last();
            break;

          case ApiObjectMember.Name:
            elems[i].Name = nm.Count > i
              ? nm[i]
              : nm.Last();
            break;

          case ApiObjectMember.OrientationAngle:
            elems[i].OrientationAngle = oriA.Count > i
              ? oriA[i]
              : oriA.Last();
            break;

          case ApiObjectMember.Offset:
            if (off.Count > i) {
              elems[i].Offset.X1 = off[i].X1.Meters;
              elems[i].Offset.X2 = off[i].X2.Meters;
              elems[i].Offset.Y = off[i].Y.Meters;
              elems[i].Offset.Z = off[i].Z.Meters;
            }
            else {
              elems[i].Offset.X1 = off.Last().X1.Meters;
              elems[i].Offset.X2 = off.Last().X2.Meters;
              elems[i].Offset.Y = off.Last().Y.Meters;
              elems[i].Offset.Z = off.Last().Z.Meters;
            }

            break;

          case ApiObjectMember.Property:
            elems[i].Property = prop.Count > i
              ? prop[i]
              : prop.Last();
            break;

          case ApiObjectMember.Type:
            elems[i].Type = typ.Count > i
              ? typ[i]
              : typ.Last();
            break;

          case ApiObjectMember.Colour:
            elems[i].Colour = col.Count > i
              ? col[i]
              : col.Last();

            _mesh.VertexColors.SetColor(i,
              (Color)elems[i]
                .Colour);
            break;
        }
      }

      _elements = elems;
    }
  }
}
