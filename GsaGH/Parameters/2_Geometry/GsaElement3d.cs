using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Grasshopper;
using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// Element3d class, this class defines the basic properties and methods for any Gsa Element 3d
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

    #region fields
    private List<Element> _elements = new List<Element>();
    private Mesh _mesh = new Mesh();
    private List<List<int>> _topoInt; // list of topology integers referring to the topo list of points
    private List<List<int>> _faceInt; // list of face integers included in each solid mesh referring to the mesh face list
    private List<Point3d> _topo; // list of topology points for visualisation
    private List<int> _ids = new List<int>();
    private Guid _guid = Guid.NewGuid();
    private List<GsaProp3d> _props = new List<GsaProp3d>();
    private Mesh _displayMesh;
    #endregion

    #region properties
    internal List<Element> API_Elements {
      get {
        return _elements;
      }
      set {
        _elements = value;
      }
    }
    internal Element GetApiObjectClone(int i) {
      var dup = new Element() {
        Group = _elements[i].Group,
        IsDummy = _elements[i].IsDummy,
        Name = _elements[i].Name.ToString(),
        OrientationNode = _elements[i].OrientationNode,
        OrientationAngle = _elements[i].OrientationAngle,
        Offset = _elements[i].Offset,
        ParentMember = _elements[i].ParentMember,
        Property = _elements[i].Property,
        Type = _elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
      };
      dup.Topology = new ReadOnlyCollection<int>(_elements[i].Topology.ToList());
      return dup;
    }
    public int Count {
      get {
        return _elements.Count;
      }
    }
    public Mesh NgonMesh {
      get {
        return _mesh;
      }
    }
    public Mesh DisplayMesh {
      get {
        if (_displayMesh == null) {
          UpdatePreview();
        }
        return _displayMesh;
      }
    }

    public List<Point3d> Topology {
      get {
        return _topo;
      }
      set {
        _topo = value;
      }
    }
    public List<int> Ids {
      get {
        return _ids;
      }
      set {
        _ids = value;
      }
    }
    public List<List<int>> TopoInt {
      get {
        return _topoInt;
      }
      set {
        _topoInt = value;
      }
    }
    public List<List<int>> FaceInt {
      get {
        return _faceInt;
      }
      set {
        _faceInt = value;
      }
    }
    public List<GsaProp3d> Properties {
      get {
        return _props;
      }
      set {
        _props = value;
      }
    }
    public DataTree<int> TopologyIDs {
      get {
        var topos = new DataTree<int>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            topos.AddRange(_elements[i].Topology.ToList(), new Grasshopper.Kernel.Data.GH_Path(i));
        }
        return topos;
      }
    }
    public Guid Guid {
      get {
        return _guid;
      }
    }
    #region GsaAPI.Element members
    public List<Color> Colours {
      get {
        var cols = new List<Color>();
        for (int i = 0; i < _elements.Count; i++) {
          if ((Color)_elements[i].Colour == Color.FromArgb(0, 0, 0)) {
            _elements[i].Colour = Color.FromArgb(50, 150, 150, 150);
          }
          cols.Add((Color)_elements[i].Colour);

          NgonMesh.VertexColors.SetColor(i, (Color)_elements[i].Colour);
        }
        return cols;
      }
      set {
        CloneApiElements(ApiObjectMember.Colour, null, null, null, null, null, null, null, value);
      }
    }
    public List<int> Groups {
      get {
        var groups = new List<int>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            groups.Add(_elements[i].Group);
        }
        return groups;
      }
      set {
        CloneApiElements(ApiObjectMember.Group, value);
      }
    }
    public List<bool> IsDummies {
      get {
        var dums = new List<bool>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            dums.Add(_elements[i].IsDummy);
        }
        return dums;
      }
      set {
        CloneApiElements(ApiObjectMember.Dummy, null, value);
      }
    }
    public List<string> Names {
      get {
        var names = new List<string>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            names.Add(_elements[i].Name);
        }
        return names;
      }
      set {
        CloneApiElements(ApiObjectMember.Dummy, null, null, value);
      }
    }
    public List<double> OrientationAngles {
      get {
        var angles = new List<double>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            angles.Add(_elements[i].OrientationAngle);
        }
        return angles;
      }
      set {
        CloneApiElements(ApiObjectMember.Dummy, null, null, null, value);
      }
    }
    public List<GsaOffset> Offsets {
      get {
        var offs = new List<GsaOffset>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null) {
            var off = new GsaOffset(_elements[i].Offset.X1, _elements[i].Offset.X2, _elements[i].Offset.Y, _elements[i].Offset.Z);
            offs.Add(off);
          }
        }
        return offs;
      }
      set {
        CloneApiElements(ApiObjectMember.Dummy, null, null, null, null, value);
      }
    }
    public List<int> PropertyIDs {
      get {
        var propids = new List<int>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            propids.Add(_elements[i].Property);
        }
        return propids;
      }
      set {
        CloneApiElements(ApiObjectMember.Dummy, null, null, null, null, null, value);
      }
    }
    public List<ElementType> Types {
      get {
        var typs = new List<ElementType>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            typs.Add(_elements[i].Type);
        }
        return typs;
      }
      set {
        CloneApiElements(ApiObjectMember.Dummy, null, null, null, null, null, null, value);
      }
    }
    public List<int> ParentMembers {
      get {
        var pMems = new List<int>();
        for (int i = 0; i < _elements.Count; i++)
          try {
            pMems.Add(_elements[i].ParentMember.Member);
          }
          catch (Exception) {
            pMems.Add(0);
          }
        return pMems;
      }
    }
    #endregion
    #endregion

    #region constructors
    public GsaElement3d() {
      //this._props = new List<GsaProp2d>();
    }

    public GsaElement3d(Mesh mesh, int prop = 0) {
      _mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;

      _ids = new List<int>(new int[_mesh.Faces.Count()]);

      _props = new List<GsaProp3d>();
      for (int i = 0; i < _mesh.Faces.Count(); i++) {
        _props.Add(new GsaProp3d());
      }
      UpdatePreview();
    }

    internal GsaElement3d(Element element, int id, Mesh mesh, GsaProp3d prop3d) {
      _elements = new List<Element>() { element };
      _mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;
      _ids = new List<int>() { id };
      _props = new List<GsaProp3d>() { prop3d };
      UpdatePreview();
    }
    internal GsaElement3d(ConcurrentDictionary<int, Element> elements, Mesh mesh, List<GsaProp3d> prop3ds) {
      _elements = elements.Values.ToList();
      _mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;
      _ids = elements.Keys.ToList();
      _props = prop3ds;
      UpdatePreview();
    }
    #endregion

    #region methods
    public GsaElement3d Duplicate(bool cloneApiElements = false) {
      var dup = new GsaElement3d();
      dup._mesh = (Mesh)_mesh.DuplicateShallow();
      dup._guid = new Guid(_guid.ToString());
      dup._topo = _topo;
      dup._topoInt = _topoInt;
      dup._faceInt = _faceInt;
      dup._elements = _elements;
      if (cloneApiElements)
        dup.CloneApiElements();
      dup._ids = _ids.ToList();
      dup._props = _props.ConvertAll(x => x.Duplicate());
      dup.UpdatePreview();
      return dup;
    }

    public override string ToString() {
      if (!(_mesh.Ngons.Count > 0))
        return "Null";
      var types = Types.Select(t => Mappings.s_elementTypeMapping.FirstOrDefault(x => x.Value == t).Key).ToList();
      string type = string.Join("/", types.Distinct());
      string info = "N:" + NgonMesh.Vertices.Count + " E:" + API_Elements.Count;
      return string.Join(" ", type.Trim(), info.Trim()).Trim().Replace("  ", " ");
    }

    /// <summary>
    /// This method will return a copy of the existing element3d with an updated mesh
    /// </summary>
    /// <param name="updatedthis._mesh"></param>
    /// <returns></returns>
    public GsaElement3d UpdateGeometry(Mesh updatedMesh) {
      //if (mthis._mesh.Faces.Count != mthis._elements.Count) { return null; } // the logic below assumes the number of elements is equal to number of faces

      GsaElement3d dup = Duplicate(true);
      _mesh = updatedMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem3d(_mesh, 0);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _faceInt = convertMesh.Item4;
      return dup;
    }

    public GsaElement3d Transform(Transform xform) {
      if (NgonMesh == null)
        return null;

      GsaElement3d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.NgonMesh.Faces.Count()]);

      Mesh xMs = dup.NgonMesh.DuplicateMesh();
      xMs.Transform(xform);

      return dup.UpdateGeometry(xMs);
    }

    public GsaElement3d Morph(SpaceMorph xmorph) {
      if (NgonMesh == null)
        return null;

      GsaElement3d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.NgonMesh.Faces.Count()]);

      Mesh xMs = dup.NgonMesh.DuplicateMesh();
      xmorph.Morph(xMs);

      return dup.UpdateGeometry(xMs);
    }

    internal void CloneApiElements() {
      CloneApiElements(ApiObjectMember.All);
      _guid = Guid.NewGuid();
    }

    private void CloneApiElements(ApiObjectMember memType, List<int> grp = null, List<bool> dum = null, List<string> nm = null, List<double> oriA = null, List<GsaOffset> off = null, List<int> prop = null, List<ElementType> typ = null, List<Color> col = null) {
      var elems = new List<Element>();
      for (int i = 0; i < _elements.Count; i++) {
        elems.Add(new Element() {
          Group = _elements[i].Group,
          IsDummy = _elements[i].IsDummy,
          Name = _elements[i].Name.ToString(),
          OrientationNode = _elements[i].OrientationNode,
          OrientationAngle = _elements[i].OrientationAngle,
          Offset = _elements[i].Offset,
          ParentMember = _elements[i].ParentMember,
          Property = _elements[i].Property,
          Topology = new ReadOnlyCollection<int>(_elements[i].Topology.ToList()),
          Type = _elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
        });

        if (memType == ApiObjectMember.All)
          continue;

        switch (memType) {
          case ApiObjectMember.Group:
            if (grp.Count > i)
              elems[i].Group = grp[i];
            else
              elems[i].Group = grp.Last();
            break;
          case ApiObjectMember.Dummy:
            if (dum.Count > i)
              elems[i].IsDummy = dum[i];
            else
              elems[i].IsDummy = dum.Last();
            break;
          case ApiObjectMember.Name:
            if (nm.Count > i)
              elems[i].Name = nm[i];
            else
              elems[i].Name = nm.Last();
            break;
          case ApiObjectMember.OrientationAngle:
            if (oriA.Count > i)
              elems[i].OrientationAngle = oriA[i];
            else
              elems[i].OrientationAngle = oriA.Last();
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
            if (prop.Count > i)
              elems[i].Property = prop[i];
            else
              elems[i].Property = prop.Last();
            break;
          case ApiObjectMember.Type:
            if (typ.Count > i)
              elems[i].Type = typ[i];
            else
              elems[i].Type = typ.Last();
            break;
          case ApiObjectMember.Colour:
            if (col.Count > i)
              elems[i].Colour = col[i];
            else
              elems[i].Colour = col.Last();

            _mesh.VertexColors.SetColor(i, (Color)elems[i].Colour);
            break;
        }
      }
      _elements = elems;
    }

    internal void UpdatePreview() {
      _displayMesh = new Mesh();
      Mesh x = NgonMesh;

      _displayMesh.Vertices.AddVertices(x.Vertices.ToList());
      var ngons = x.GetNgonAndFacesEnumerable().ToList();

      for (int i = 0; i < ngons.Count; i++) {
        var faceindex = ngons[i].FaceIndexList().Select(u => (int)u).ToList();
        for (int j = 0; j < faceindex.Count; j++) {
          _displayMesh.Faces.AddFace(x.Faces[faceindex[j]]);
        }
      }
      _displayMesh.RebuildNormals();
    }
    #endregion
  }
}
