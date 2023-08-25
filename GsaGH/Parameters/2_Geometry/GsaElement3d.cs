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
  /// <para>Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'. </para>
  /// <para>In Grasshopper, an Element3D is a collection of 3D Elements (mesh solids representing <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#brick-wedge-pyramid-and-tetra-elements">Brick, Wedge, Pyramid or Tetra Elements</see>) used for FE analysis. In GSA, a 3D Element is just a single closed mesh, but for Rhino performance reasons we have made Element3D an <see href="https://docs.mcneel.com/rhino/7/help/en-us/popup_moreinformation/ngon.htm">Ngon Mesh</see> that can contain more than one closed mesh.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-element.html">Elements</see> to read more.</para>
  /// 
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
        for (int i = 0; i < ApiElements.Count; i++) {
          if ((Color)ApiElements[i].Colour == Color.FromArgb(0, 0, 0)) {
            ApiElements[i].Colour = Color.FromArgb(50, 150, 150, 150);
          }

          cols.Add((Color)ApiElements[i].Colour);

          NgonMesh.VertexColors.SetColor(i, (Color)ApiElements[i].Colour);
        }

        return cols;
      }
      set
        => CloneApiElements(ApiObjectMember.Colour, null, null, null, null, null, null, null,
          value);
    }
    public int Count => ApiElements.Count;
    public Mesh DisplayMesh {
      get {
        if (_displayMesh == null) {
          UpdatePreview();
        }

        return _displayMesh;
      }
    }
    public List<List<int>> FaceInt { get; set; }
    public List<int> Groups {
      get => (from element in ApiElements where element != null select element.Group).ToList();
      set => CloneApiElements(ApiObjectMember.Group, value);
    }
    public Guid Guid => _guid;
    public List<int> Ids { get; set; } = new List<int>();
    public List<bool> IsDummies {
      get => (from element in ApiElements where element != null select element.IsDummy).ToList();
      set => CloneApiElements(ApiObjectMember.Dummy, null, value);
    }
    public List<string> Names {
      get => (from element in ApiElements where element != null select element.Name).ToList();
      set => CloneApiElements(ApiObjectMember.Name, null, null, value);
    }
    public Mesh NgonMesh { get; private set; } = new Mesh();
    public List<GsaOffset> Offsets {
      get
        => (from element in ApiElements where element != null
            select new GsaOffset(element.Offset.X1, element.Offset.X2, element.Offset.Y,
              element.Offset.Z)).ToList();
      set => CloneApiElements(ApiObjectMember.Offset, null, null, null, null, value);
    }
    public List<double> OrientationAngles {
      get
        => (from element in ApiElements where element != null select element.OrientationAngle)
         .ToList();
      set => CloneApiElements(ApiObjectMember.OrientationAngle, null, null, null, value);
    }
    public List<int> ParentMembers {
      get {
        var pMems = new List<int>();
        foreach (Element element in ApiElements) {
          try {
            pMems.Add(element.ParentMember.Member);
          } catch (Exception) {
            pMems.Add(0);
          }
        }

        return pMems;
      }
    }
    public List<GsaProperty3d> Prop3ds { get; set; } = new List<GsaProperty3d>();
    public List<int> PropertyIDs {
      get => (from element in ApiElements where element != null select element.Property).ToList();
      set => CloneApiElements(ApiObjectMember.Property, null, null, null, null, null, value);
    }
    public List<List<int>> TopoInt { get; set; }
    public List<Point3d> Topology { get; set; }
    public DataTree<int> TopologyIDs {
      get {
        var topos = new DataTree<int>();
        for (int i = 0; i < ApiElements.Count; i++) {
          if (ApiElements[i] != null) {
            topos.AddRange(ApiElements[i].Topology.ToList(), new GH_Path(i));
          }
        }

        return topos;
      }
    }
    public List<ElementType> Types {
      get => (from element in ApiElements where element != null select element.Type).ToList();
      set => CloneApiElements(ApiObjectMember.Type, null, null, null, null, null, null, value);
    }
    internal List<Element> ApiElements { get; set; } = new List<Element>();
    private Mesh _displayMesh;
    private Guid _guid = Guid.NewGuid();

    public GsaElement3d() { }

    public GsaElement3d(Mesh mesh) {
      NgonMesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      FaceInt = convertMesh.Item4;

      Ids = new List<int>(new int[NgonMesh.Faces.Count]);

      Prop3ds = new List<GsaProperty3d>();
      for (int i = 0; i < NgonMesh.Faces.Count; i++) {
        Prop3ds.Add(new GsaProperty3d(0));
      }

      UpdatePreview();
    }

    internal GsaElement3d(Element element, int id, Mesh mesh, GsaProperty3d prop3d) {
      ApiElements = new List<Element>() {
        element,
      };
      NgonMesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      FaceInt = convertMesh.Item4;
      Ids = new List<int>() {
        id,
      };
      Prop3ds = new List<GsaProperty3d>() {
        prop3d,
      };
      UpdatePreview();
    }

    internal GsaElement3d(
      ConcurrentDictionary<int, Element> elements, Mesh mesh, ConcurrentDictionary<int, GsaProperty3d> prop3ds) {
      NgonMesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      FaceInt = convertMesh.Item4;
      ApiElements = elements.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
      Ids = elements.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key).ToList();
      Prop3ds = prop3ds.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList(); ;
      UpdatePreview();
    }

    public GsaElement3d Clone() {
      var dup = new GsaElement3d {
        NgonMesh = (Mesh)NgonMesh.DuplicateShallow(),
        Topology = Topology,
        TopoInt = TopoInt,
        FaceInt = FaceInt,
        ApiElements = ApiElements,
      };
      dup.CloneApiElements();

      dup.Ids = Ids.ToList();
      dup.Prop3ds = Prop3ds.ConvertAll(x => x.Duplicate());
      dup.UpdatePreview();
      return dup;
    }

    public GsaElement3d Duplicate() {
      return this;
    }

    public GsaElement3d Morph(SpaceMorph xmorph) {
      if (NgonMesh == null) {
        return null;
      }

      GsaElement3d dup = Clone();
      dup.Ids = new List<int>(new int[dup.NgonMesh.Faces.Count]);

      Mesh xMs = dup.NgonMesh.DuplicateMesh();
      xmorph.Morph(xMs);

      return dup.UpdateGeometry(xMs);
    }

    public override string ToString() {
      if (!(NgonMesh.Ngons.Count > 0)) {
        return "Null";
      }

      var types = Types.Select(t => Mappings.elementTypeMapping.FirstOrDefault(x => x.Value == t)
       .Key).ToList();
      string type = string.Join("/", types.Distinct());
      string info = "N:" + NgonMesh.Vertices.Count + " E:" + ApiElements.Count;
      return string.Join(" ", type.Trim(), info.Trim()).Trim().Replace("  ", " ");
    }

    public GsaElement3d Transform(Transform xform) {
      if (NgonMesh == null) {
        return null;
      }

      GsaElement3d dup = Clone();
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
      GsaElement3d dup = Clone();
      NgonMesh = updatedMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem3d(NgonMesh, 0);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      FaceInt = convertMesh.Item4;
      return dup;
    }

    internal void CloneApiElements() {
      CloneApiElements(ApiObjectMember.All);
      _guid = Guid.NewGuid();
    }

    internal Element GetApiObjectClone(int i) {
      var dup = new Element() {
        Group = ApiElements[i].Group,
        IsDummy = ApiElements[i].IsDummy,
        Name = ApiElements[i].Name.ToString(),
        OrientationNode = ApiElements[i].OrientationNode,
        OrientationAngle = ApiElements[i].OrientationAngle,
        Offset = ApiElements[i].Offset,
        ParentMember = ApiElements[i].ParentMember,
        Property = ApiElements[i].Property,
        Type = ApiElements[i].Type,
        Topology = new ReadOnlyCollection<int>(ApiElements[i].Topology.ToList()),
      };
      return dup;
    }

    internal void UpdatePreview() {
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

    // list of topology integers referring to the topo list of points
    // list of topology points for visualisation
    private void CloneApiElements(
      ApiObjectMember memType, IList<int> grp = null, IList<bool> dum = null,
      IList<string> nm = null, IList<double> oriA = null, IList<GsaOffset> off = null,
      IList<int> prop = null, IList<ElementType> typ = null, IList<Color> col = null) {
      var elems = new List<Element>();
      for (int i = 0; i < ApiElements.Count; i++) {
        elems.Add(new Element() {
          Group = ApiElements[i].Group,
          IsDummy = ApiElements[i].IsDummy,
          Name = ApiElements[i].Name.ToString(),
          OrientationNode = ApiElements[i].OrientationNode,
          OrientationAngle = ApiElements[i].OrientationAngle,
          ParentMember = ApiElements[i].ParentMember,
          Property = ApiElements[i].Property,
          Topology = new ReadOnlyCollection<int>(ApiElements[i].Topology.ToList()),
          Type = ApiElements[i].Type,
        });

        elems[i].Offset.X1 = ApiElements[i].Offset.X1;
        elems[i].Offset.X2 = ApiElements[i].Offset.X2;
        elems[i].Offset.Y = ApiElements[i].Offset.Y;
        elems[i].Offset.Z = ApiElements[i].Offset.Z;

        if (memType == ApiObjectMember.All) {
          continue;
        }

        switch (memType) {
          case ApiObjectMember.Group:
            elems[i].Group = grp.Count > i ? grp[i] : grp.Last();
            break;

          case ApiObjectMember.Dummy:
            elems[i].IsDummy = dum.Count > i ? dum[i] : dum.Last();
            break;

          case ApiObjectMember.Name:
            elems[i].Name = nm.Count > i ? nm[i] : nm.Last();
            break;

          case ApiObjectMember.OrientationAngle:
            elems[i].OrientationAngle = oriA.Count > i ? oriA[i] : oriA.Last();
            break;

          case ApiObjectMember.Offset:
            if (off.Count > i) {
              elems[i].Offset.X1 = off[i].X1.Meters;
              elems[i].Offset.X2 = off[i].X2.Meters;
              elems[i].Offset.Y = off[i].Y.Meters;
              elems[i].Offset.Z = off[i].Z.Meters;
            } else {
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
            elems[i].Colour = col.Count > i ? col[i] : col.Last();

            NgonMesh.VertexColors.SetColor(i, (Color)elems[i].Colour);
            break;
        }
      }

      ApiElements = elems;
    }
  }
}
