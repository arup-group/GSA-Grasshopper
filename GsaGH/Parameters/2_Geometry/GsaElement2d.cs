using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel.Data;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Polyline = Rhino.Geometry.Polyline;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'. </para>
  /// <para>In Grasshopper, an Element2D parameter is a collection of 2D Elements (mesh faces representing <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#quad-and-triangle-elements">Quad or Triangle Elements</see>) used for FE analysis. In GSA a 2D element is just a single face, but for Rhino performance reasons we have made the Element2D parameter a mesh that can contain more than one Element/Face.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-element.html">Elements</see> to read more.</para>
  /// </summary>
  [SuppressMessage("SonarAnalyzer.CSharp", "S101", Justification = "Project-specific naming convention")]
  public class GsaElement2d : GsaGeometryBase {
    public List<GSAElement> ApiElements { get; internal set; }
    public List<int> Ids { get; set; } = new List<int>();
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public Curve Curve { get; set; } = new PolylineCurve();
    public Mesh Mesh { get; set; } = new Mesh();
    public List<List<int>> TopoInt { get; internal set; }
    public Point3dList Topology { get; internal set; }
    public List<GsaOffset> Offsets => ApiElements.Where(x => x.IsLoadPanel == false).Select(
      e => new GsaOffset(e.Offset.X1, e.Offset.X2, e.Offset.Y, e.Offset.Z)).ToList();
    public List<Angle> OrientationAngles => ApiElements.Select(
      e => new Angle(e.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian)).ToList();
    public List<GsaProperty2d> Prop2ds { get; set; }
    public Section3dPreview Section3dPreview { get; private set; }
    internal bool IsLoadPanel => (ApiElements != null) && (ApiElements.FirstOrDefault() != null) && ApiElements.FirstOrDefault().IsLoadPanel;

    /// <summary>
    /// Empty constructor instantiating a list of new API objects
    /// </summary>
    public GsaElement2d() {
      ApiElements = new List<GSAElement>();
    }

    /// <summary>
    /// Create new instance by casting from a Mesh
    /// </summary>
    /// <param name="mesh"></param>
    public GsaElement2d(Mesh mesh) {
      Mesh = mesh.DuplicateMesh();
      Mesh.Compact();
      Mesh.Vertices.CombineIdentical(true, false);
      Tuple<List<GSAElement>, Point3dList, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(mesh, false);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      Ids = new List<int>(new int[Mesh.Faces.Count]);
    }

    /// <summary>
    /// Create new instance by casting from a Polyline
    /// </summary>
    /// <param name="curve"></param>
    public GsaElement2d(Curve curve) {
      Curve = curve.DuplicateCurve();
      ApiElements = new List<GSAElement> {
        new GSAElement(new LoadPanelElement()),
      };
      Topology = RhinoConversions.LoadPanelTopo(curve);
      TopoInt = RhinoConversions.LoadPanelTopoIndices(curve);
      Ids = new List<int>(new int[1]);
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaElement2d(GsaElement2d other) : base(other.LengthUnit) {
      Ids = other.Ids;
      Mesh = (Mesh)other.Mesh.DuplicateShallow();
      Curve = other.Curve.DuplicateCurve();
      ApiElements = other.DuplicateApiObjects();
      Topology = other.Topology?.Duplicate();
      TopoInt = other.TopoInt;
      Prop2ds = other.Prop2ds;
      Section3dPreview = other.Section3dPreview?.Duplicate();
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaElement2d(
      ConcurrentDictionary<int, GSAElement> elements, Mesh mesh, ConcurrentDictionary<int, GsaProperty2d> prop2ds, LengthUnit modelUnit) : base(modelUnit) {
      Mesh = mesh;
      Topology = new Point3dList(mesh.Vertices.ToPoint3dArray());
      TopoInt = RhinoConversions.ConvertMeshToElem2d(Mesh);
      ApiElements = elements.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
      Ids = elements.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key).ToList();
      if (!prop2ds.IsNullOrEmpty()) {
        Prop2ds = prop2ds.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
      }
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaElement2d(int id, GSAElement element, Curve curve, GsaProperty2d prop2d, LengthUnit modelUnit) : base(modelUnit) {
      Curve = curve;
      ApiElements = new List<GSAElement>() { element };
      Topology = RhinoConversions.LoadPanelTopo(curve);
      TopoInt = RhinoConversions.LoadPanelTopoIndices(curve);
      Ids = new List<int>() { id };
      if (prop2d != null) {
        Prop2ds = new List<GsaProperty2d>() { prop2d };
      }
    }

    public void CreateSection3dPreview() {
      Section3dPreview = new Section3dPreview(this);
    }

    public List<GSAElement> DuplicateApiObjects() {
      if (ApiElements.IsNullOrEmpty()) {
        return ApiElements;
      }

      var elems = new List<GSAElement>();
      for (int i = 0; i < ApiElements.Count; i++) {
        GSAElement element2d = ApiElements[i];
        GSAElement element = null;
        if (element2d.IsLoadPanel) {
          element = new GSAElement(new LoadPanelElement()) {
            Group = element2d.Group,
            IsDummy = element2d.IsDummy,
            Name = element2d.Name.ToString(),
            OrientationAngle = element2d.OrientationAngle,
            ParentMember = element2d.ParentMember,
            Property = element2d.Property,
            Type = element2d.Type,
            Topology = new ReadOnlyCollection<int>(element2d.Topology.ToList()),
          };
        } else {
          element = new GSAElement(new Element()) {
            Group = element2d.Group,
            IsDummy = element2d.IsDummy,
            Name = element2d.Name.ToString(),
            OrientationNode = element2d.OrientationNode,
            OrientationAngle = element2d.OrientationAngle,
            ParentMember = element2d.ParentMember,
            Property = element2d.Property,
            Type = element2d.Type,
            Topology = new ReadOnlyCollection<int>(element2d.Topology.ToList()),
            Offset = element2d.Offset,
          };
        }
        if ((Color)element2d.Colour != Color.FromArgb(0, 0, 0)) {
          element.Colour = element2d.Colour;
        }
        elems.Add(element);
      }
      return elems;
    }

    public Point3dList GetCenterPoints() {
      var points = new Point3dList();
      int faceIndex = 0;
      for (int i = 0; i < ApiElements.Count; i++) {

        if (ApiElements[i].IsLoadPanel) {
          Curve.TryGetPolyline(out Polyline polyline);
          points.Add(polyline.CenterPoint());
        } else {
          Point3d pt = Mesh.Faces.GetFaceCenter(faceIndex);

          switch (ApiElements[i].Type) {
            case ElementType.QUAD8:
              pt = Mesh.Vertices[Mesh.Faces[faceIndex].C];
              faceIndex += 8;
              break;

            case ElementType.TRI6:
              pt = Mesh.Faces.GetFaceCenter(faceIndex + 4);
              faceIndex += 4;
              break;

            default:
              faceIndex++;
              break;
          }
          points.Add(pt);
        }
      }
      return points;
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
      string info = "";
      if (IsLoadPanel) {
        if (Curve == null || !Curve.TryGetPolyline(out Polyline polyline)) {
          return "Null";
        }
        info = "P" + polyline.Count;

      } else {
        if (!Mesh.IsValid) {
          return "Null";
        }
        info = "N:" + Mesh.Vertices.Count + " E:" + ApiElements.Count;
      }
      string type = Mappings._elementTypeMapping.FirstOrDefault(
       x => x.Value == ApiElements.First().Type).Key;
      return string.Join(" ", type, info).TrimSpaces();

    }

    public void UpdateMeshColours() {
      for (int i = 0; i < ApiElements.Count; i++) {
        Mesh.VertexColors.SetColor(i, (Color)ApiElements[i].Colour);
      }
    }

    public GeometryBase Geometry() {
      if (IsLoadPanel) {
        return Curve;
      } else {
        return Mesh;
      }
    }
  }
}
