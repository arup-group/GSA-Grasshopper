using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Grasshopper;
using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// Element2d class, this class defines the basic properties and methods for any Gsa Element 2d
  /// </summary>
  public class GsaElement2d {
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
    private List<GsaProp2d> _props = new List<GsaProp2d>();
    private List<List<int>> _topoInt; // list of topology integers referring to the topo list of points
    private List<Point3d> _topo; // list of topology points for visualisation
    private List<int> _ids = new List<int>();
    private Guid _guid = Guid.NewGuid();
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
      return new Element() {
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
      };
    }
    public int Count {
      get {
        return _elements.Count;
      }
    }
    public Mesh Mesh {
      get {
        return _mesh;
      }
    }
    public List<Point3d> Topology {
      get {
        return _topo;
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
    }
    public List<GsaProp2d> Properties {
      get {
        return _props;
      }
      set {
        _props = value;
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

          Mesh.VertexColors.SetColor(i, (System.Drawing.Color)_elements[i].Colour);
        }
        return cols;
      }
      set {
        CloneApiElements(ApiObjectMember.Colour, null, null, null, null, null, null, null, value);
        //for (int i = 0; i < mthis._elements.Count; i++)
        //{
        //    if (value[i] != null)
        //    {
        //        mthis._elements[i].Colour = value[i];
        //        Mesh.VertexColors.SetColor(i, (System.Drawing.Color)mthis._elements[i].Colour);
        //    }
        //}
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
        CloneApiElements(ApiObjectMember.Name, null, null, value);
      }
    }
    public List<Angle> OrientationAngles {
      get {
        var angles = new List<Angle>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            angles.Add(new Angle(_elements[i].OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian));
        }
        return angles;
      }
      set {
        CloneApiElements(ApiObjectMember.OrientationAngle, null, null, null, value);
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
        CloneApiElements(ApiObjectMember.Offset, null, null, null, null, value);
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
        CloneApiElements(ApiObjectMember.Type, null, null, null, null, null, null, value);
      }
    }

    public DataTree<int> TopologyIDs {
      get {
        var topos = new DataTree<int>();
        for (int i = 0; i < _elements.Count; i++) {
          if (_elements[i] != null)
            topos.AddRange(_elements[i].Topology.ToList(), new Grasshopper.Kernel.Data.GH_Path(Ids[i]));
        }
        return topos;
      }
    }
    #endregion
    #endregion

    #region constructors
    public GsaElement2d() {
    }

    public GsaElement2d(Mesh mesh, int prop = 0) {
      _mesh = mesh.DuplicateMesh();
      _mesh.Compact();
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(_mesh, prop);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _ids = new List<int>(new int[_mesh.Faces.Count()]);
      var singleProp = new GsaProp2d();
      for (int i = 0; i < _mesh.Faces.Count(); i++)
        _props.Add(singleProp.Duplicate());
    }

    internal GsaElement2d(Element element, int id, Mesh mesh, GsaProp2d prop2d) {
      _mesh = mesh;
      _topo = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      _topoInt = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(_mesh);
      _elements = new List<Element>() { element };
      _ids = new List<int>() { id };
      _props = new List<GsaProp2d>() { prop2d };
    }
    internal GsaElement2d(ConcurrentDictionary<int, Element> elements, Mesh mesh, List<GsaProp2d> prop2ds) {
      _mesh = mesh;
      _topo = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      _topoInt = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(_mesh);
      _elements = elements.Values.ToList();
      _ids = elements.Keys.ToList();
      _props = prop2ds;
    }

    public GsaElement2d(Brep brep, List<Curve> curves, List<Point3d> points, double meshSize, List<GsaMember1d> mem1ds, List<GsaNode> nodes, LengthUnit unit, Length tolerance, int prop = 0) {
      _mesh = Helpers.GH.RhinoConversions.ConvertBrepToMesh(brep, points, nodes, curves, null, mem1ds, meshSize, unit, tolerance).Item1;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(_mesh, prop, true);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _ids = new List<int>(new int[_mesh.Faces.Count()]);
    }
    #endregion

    #region methods
    public GsaElement2d Duplicate(bool cloneApiElements = false) {
      var dup = new GsaElement2d();
      dup._elements = _elements;
      dup._guid = new Guid(_guid.ToString());
      if (cloneApiElements)
        dup.CloneApiElements();
      dup._ids = _ids.ToList();
      dup._mesh = (Mesh)_mesh.DuplicateShallow();
      dup._props = _props.ConvertAll(x => x.Duplicate());
      dup._topo = _topo;
      dup._topoInt = _topoInt;
      return dup;
    }

    public GsaElement2d UpdateGeometry(Mesh newMesh) {
      if (_mesh.Faces.Count != _elements.Count)
        return null; // the logic below assumes the number of elements is equal to number of faces

      GsaElement2d dup = Duplicate(true);
      _mesh = newMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(_mesh, 0);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      return dup;
    }

    public GsaElement2d Transform(Transform xform) {
      if (Mesh == null)
        return null;

      GsaElement2d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count()]);

      Mesh xMs = dup.Mesh.DuplicateMesh();
      xMs.Transform(xform);

      return dup.UpdateGeometry(xMs);
    }

    public GsaElement2d Morph(SpaceMorph xmorph) {
      if (Mesh == null)
        return null;
      GsaElement2d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count()]);

      Mesh xMs = dup.Mesh.DuplicateMesh();
      xmorph.Morph(xMs);

      return dup.UpdateGeometry(xMs);
    }

    public static Tuple<GsaElement2d, List<GsaNode>, List<GsaElement1d>> GetElement2dFromBrep(Brep brep, List<Point3d> points, List<GsaNode> nodes, List<Curve> curves, List<GsaElement1d> elem1ds, List<GsaMember1d> mem1ds, double meshSize, LengthUnit unit, Length tolerance) {
      var gsaElement2D = new GsaElement2d();
      Tuple<Mesh, List<GsaNode>, List<GsaElement1d>> tuple
        = Helpers.GH.RhinoConversions.ConvertBrepToMesh(brep, points, nodes, curves, elem1ds, mem1ds, meshSize, unit, tolerance);
      gsaElement2D._mesh = tuple.Item1;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(gsaElement2D._mesh, 0, true);
      gsaElement2D._elements = convertMesh.Item1;
      gsaElement2D._topo = convertMesh.Item2;
      gsaElement2D._topoInt = convertMesh.Item3;
      gsaElement2D._ids = new List<int>(new int[gsaElement2D._mesh.Faces.Count()]);

      return new Tuple<GsaElement2d, List<GsaNode>, List<GsaElement1d>>(gsaElement2D, tuple.Item2, tuple.Item3);
    }

    public override string ToString() {
      if (!_mesh.IsValid)
        return "Null";
      string type = Mappings.s_elementTypeMapping.FirstOrDefault(x => x.Value == Types.First()).Key + " ";
      string info = "N:" + Mesh.Vertices.Count + " E:" + API_Elements.Count;
      return string.Join(" ", type.Trim(), info.Trim()).Trim().Replace("  ", " ");
    }

    private void CloneApiElements(ApiObjectMember memType, List<int> grp = null, List<bool> dum = null, List<string> nm = null, List<Angle> oriA = null, List<GsaOffset> off = null, List<int> prop = null, List<ElementType> typ = null, List<System.Drawing.Color> col = null) {
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
          Type = _elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
        });
        elems[i].Topology = new ReadOnlyCollection<int>(_elements[i].Topology.ToList());
        //if ((System.Drawing.Color)mthis._elements[i].Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        //    elems[i].Colour = mthis._elements[i].Colour;

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
              elems[i].OrientationAngle = oriA[i].Degrees;
            else
              elems[i].OrientationAngle = oriA.Last().Degrees;
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

            _mesh.VertexColors.SetColor(i, (System.Drawing.Color)elems[i].Colour);
            break;
        }
      }
      _elements = elems;
    }

    internal void CloneApiElements() {
      CloneApiElements(ApiObjectMember.All);
      _guid = Guid.NewGuid();
    }
    #endregion
  }
}
