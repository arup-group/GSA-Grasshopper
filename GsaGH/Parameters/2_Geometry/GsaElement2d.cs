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
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Element2d class, this class defines the basic properties and methods for any Gsa Element 2d
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

    public List<Color> Colours {
      get {
        var cols = new List<Color>();
        for (int i = 0; i < _elements.Count; i++) {
          if ((Color)_elements[i].Colour == Color.FromArgb(0, 0, 0))
            _elements[i]
              .Colour = Color.FromArgb(50, 150, 150, 150);
          cols.Add((Color)_elements[i].Colour);

          Mesh.VertexColors.SetColor(i, (Color)_elements[i].Colour);
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
    public Mesh Mesh => _mesh;
    public List<string> Names {
      get => (from element
          in _elements
              where element != null
              select element.Name)
        .ToList();
      set => CloneApiElements(ApiObjectMember.Name, null, null, value);
    }
    public List<GsaOffset> Offsets {
      get => (from element
              in _elements
              where element != null
              select new GsaOffset(element.Offset.X1,
                element.Offset.X2,
                element.Offset.Y,
                element.Offset.Z)).ToList();
      set
        => CloneApiElements(ApiObjectMember.Offset,
          null,
          null,
          null,
          null,
          value);
    }
    public List<Angle> OrientationAngles {
      get => (from element
            in _elements
              where element != null
              select new Angle(element.OrientationAngle, AngleUnit.Degree)
          .ToUnit(AngleUnit.Radian))
        .ToList();
      set => CloneApiElements(ApiObjectMember.OrientationAngle, null, null, null, value);
    }
    public List<int> ParentMembers {
      get {
        var pMems = new List<int>();
        foreach (Element element in _elements)
          try {
            pMems.Add(element
              .ParentMember.Member);
          }
          catch (Exception) {
            pMems.Add(0);
          }

        return pMems;
      }
    }
    public List<GsaProp2d> Properties {
      get => _props;
      set => _props = value;
    }
    public List<List<int>> TopoInt => _topoInt;
    public List<Point3d> Topology => _topo;
    public DataTree<int> TopologyIDs {
      get {
        var topos = new DataTree<int>();
        for (int i = 0; i < _elements.Count; i++)
          if (_elements[i] != null)
            topos.AddRange(_elements[i]
                .Topology.ToList(),
              new GH_Path(Ids[i]));
        return topos;
      }
    }
    public List<ElementType> Types {
      get => (from t
              in _elements
              where t != null
              select t.Type)
        .ToList();
      set
        => CloneApiElements(ApiObjectMember.Type,
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
    private List<Element> _elements = new List<Element>();
    private Guid _guid = Guid.NewGuid();
    private List<int> _ids = new List<int>();
    private Mesh _mesh = new Mesh();
    private List<GsaProp2d> _props = new List<GsaProp2d>();
    // list of topology points for visualisation
    private List<Point3d> _topo;
    // list of topology integers referring to the topo list of points
    private List<List<int>> _topoInt;

    public GsaElement2d() { }

    public GsaElement2d(Mesh mesh, int prop = 0) {
      _mesh = mesh.DuplicateMesh();
      _mesh.Compact();
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(_mesh, prop);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _ids = new List<int>(new int[_mesh.Faces.Count]);
      var singleProp = new GsaProp2d();
      for (int i = 0; i < _mesh.Faces.Count; i++)
        _props.Add(singleProp.Duplicate());
    }

    public GsaElement2d(
      Brep brep,
      List<Curve> curves,
      List<Point3d> points,
      double meshSize,
      List<GsaMember1d> mem1ds,
      List<GsaNode> nodes,
      LengthUnit unit,
      Length tolerance,
      int prop = 0) {
      _mesh = RhinoConversions.ConvertBrepToMesh(brep,
          points,
          nodes,
          curves,
          null,
          mem1ds,
          meshSize,
          unit,
          tolerance)
        .Item1;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(_mesh, prop, true);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      _ids = new List<int>(new int[_mesh.Faces.Count]);
    }

    internal GsaElement2d(Element element, int id, Mesh mesh, GsaProp2d prop2d) {
      _mesh = mesh;
      _topo = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      _topoInt = RhinoConversions.ConvertMeshToElem2d(_mesh);
      _elements = new List<Element> {
        element,
      };
      _ids = new List<int> {
        id,
      };
      _props = new List<GsaProp2d> {
        prop2d,
      };
    }

    internal GsaElement2d(
      ConcurrentDictionary<int, Element> elements,
      Mesh mesh,
      List<GsaProp2d> prop2ds) {
      _mesh = mesh;
      _topo = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      _topoInt = RhinoConversions.ConvertMeshToElem2d(_mesh);
      _elements = elements.Values.ToList();
      _ids = elements.Keys.ToList();
      _props = prop2ds;
    }

    public static Tuple<GsaElement2d, List<GsaNode>, List<GsaElement1d>> GetElement2dFromBrep(
              Brep brep,
      List<Point3d> points,
      List<GsaNode> nodes,
      List<Curve> curves,
      List<GsaElement1d> elem1ds,
      List<GsaMember1d> mem1ds,
      double meshSize,
      LengthUnit unit,
      Length tolerance) {
      var gsaElement2D = new GsaElement2d();
      Tuple<Mesh, List<GsaNode>, List<GsaElement1d>> tuple
        = RhinoConversions.ConvertBrepToMesh(brep,
          points,
          nodes,
          curves,
          elem1ds,
          mem1ds,
          meshSize,
          unit,
          tolerance);
      gsaElement2D._mesh = tuple.Item1;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(gsaElement2D._mesh, 0, true);
      gsaElement2D._elements = convertMesh.Item1;
      gsaElement2D._topo = convertMesh.Item2;
      gsaElement2D._topoInt = convertMesh.Item3;
      gsaElement2D._ids = new List<int>(new int[gsaElement2D._mesh.Faces.Count]);

      return new Tuple<GsaElement2d, List<GsaNode>, List<GsaElement1d>>(gsaElement2D,
        tuple.Item2,
        tuple.Item3);
    }

    public GsaElement2d Duplicate(bool cloneApiElements = false) {
      var dup = new GsaElement2d {
        _elements = _elements,
        _guid = new Guid(_guid.ToString()),
      };
      if (cloneApiElements)
        dup.CloneApiElements();
      dup._ids = _ids.ToList();
      dup._mesh = (Mesh)_mesh.DuplicateShallow();
      dup._props = _props.ConvertAll(x => x.Duplicate());
      dup._topo = _topo;
      dup._topoInt = _topoInt;
      return dup;
    }

    public GsaElement2d Morph(SpaceMorph xmorph) {
      if (Mesh == null)
        return null;
      GsaElement2d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count]);

      Mesh xMs = dup.Mesh.DuplicateMesh();
      xmorph.Morph(xMs);

      return dup.UpdateGeometry(xMs);
    }

    public override string ToString() {
      if (!_mesh.IsValid)
        return "Null";
      string type = Mappings.s_elementTypeMapping.FirstOrDefault(x => x.Value == Types.First())
          .Key
        + " ";
      string info = "N:" + Mesh.Vertices.Count + " E:" + ApiElements.Count;
      return string.Join(" ", type.Trim(), info.Trim())
        .Trim()
        .Replace("  ", " ");
    }

    public GsaElement2d Transform(Transform xform) {
      if (Mesh == null)
        return null;

      GsaElement2d dup = Duplicate(true);
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count]);

      Mesh xMs = dup.Mesh.DuplicateMesh();
      xMs.Transform(xform);

      return dup.UpdateGeometry(xMs);
    }

    public GsaElement2d UpdateGeometry(Mesh newMesh) {
      if (_mesh.Faces.Count != _elements.Count)
        return null; // the logic below assumes the number of elements is equal to number of faces

      GsaElement2d dup = Duplicate(true);
      _mesh = newMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(_mesh, 0);
      _elements = convertMesh.Item1;
      _topo = convertMesh.Item2;
      _topoInt = convertMesh.Item3;
      return dup;
    }

    internal void CloneApiElements() {
      CloneApiElements(ApiObjectMember.All);
      _guid = Guid.NewGuid();
    }

    internal Element GetApiObjectClone(int i)
      => new Element() {
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
      };

    private void CloneApiElements(
      ApiObjectMember memType,
      IList<int> grp = null,
      IList<bool> dum = null,
      IList<string> nm = null,
      IList<Angle> oriA = null,
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
          Type = _elements[i]
            .Type,
        });
        elems[i]
          .Topology = new ReadOnlyCollection<int>(_elements[i]
          .Topology.ToList());

        if (memType == ApiObjectMember.All)
          continue;

        switch (memType) {
          case ApiObjectMember.Group:
            elems[i]
                .Group = grp.Count > i ? grp[i] : grp.Last();
            break;

          case ApiObjectMember.Dummy:
            elems[i]
                .IsDummy = dum.Count > i ? dum[i] : dum.Last();
            break;

          case ApiObjectMember.Name:
            elems[i]
                .Name = nm.Count > i ? nm[i] : nm.Last();
            break;

          case ApiObjectMember.OrientationAngle:
            elems[i]
                .OrientationAngle = oriA.Count > i
              ? oriA[i]
                .Degrees
              : oriA.Last()
                .Degrees;
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
            elems[i].Property = prop.Count > i ? prop[i] : prop.Last();
            break;

          case ApiObjectMember.Type:
            elems[i].Type = typ.Count > i ? typ[i] : typ.Last();
            break;

          case ApiObjectMember.Colour:
            elems[i].Colour = col.Count > i ? col[i] : (ValueType)col.Last();

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
