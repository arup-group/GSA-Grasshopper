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

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'. </para>
  /// <para>In Grasshopper, an Element2D parameter is a collection of 2D Elements (mesh faces representing <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#quad-and-triangle-elements">Quad or Triangle Elements</see>) used for FE analysis. In GSA a 2D element is just a single face, but for Rhino performance reasons we have made the Element2D parameter a mesh that can contain more than one Element/Face.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-element.html">Elements</see> to read more.</para>
  /// </summary>
  public class GsaElement2d {
    public List<Element> ApiElements { get; internal set; }
    public List<int> Ids { get; set; } = new List<int>();
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public Mesh Mesh { get; set; } = new Mesh();
    public List<List<int>> TopoInt { get; internal set; }
    public Point3dList Topology { get; internal set; }
    public List<GsaOffset> Offsets => ApiElements.Select(
      e => new GsaOffset(e.Offset.X1, e.Offset.X2, e.Offset.Y, e.Offset.Z)).ToList();
    public List<Angle> OrientationAngles => ApiElements.Select(
      e => new Angle(e.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian)).ToList();
    public List<GsaProperty2d> Prop2ds { get; set; }
    public Section3dPreview Section3dPreview { get; private set; }

    /// <summary>
    /// Empty constructor instantiating a list of new API objects
    /// </summary>
    public GsaElement2d() {
      ApiElements = new List<Element>();
    }

    /// <summary>
    /// Create new instance by casting from a Mesh
    /// </summary>
    /// <param name="mesh"></param>
    public GsaElement2d(Mesh mesh) {
      Mesh = mesh.DuplicateMesh();
      Mesh.Compact();
      Mesh.Vertices.CombineIdentical(true, false);
      Tuple<List<Element>, Point3dList, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(mesh, 0);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      Ids = new List<int>(new int[Mesh.Faces.Count]);
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaElement2d(GsaElement2d other) {
      Ids = other.Ids;
      Mesh = (Mesh)other.Mesh.DuplicateShallow();
      ApiElements = other.DuplicateApiObjects();
      Topology = other.Topology;
      TopoInt = other.TopoInt;
      Prop2ds = other.Prop2ds;
      Section3dPreview = other.Section3dPreview;
    }

    [ExcludeFromCodeCoverage]
    [Obsolete("This method is only used by obsolete components and will be removed in GsaGH 1.0")]
    public GsaElement2d(
      Brep brep, List<Curve> curves, Point3dList points, double meshSize,
      List<GsaMember1d> mem1ds, List<GsaNode> nodes, LengthUnit unit, Length tolerance,
      int prop = 0) {
      Mesh = RhinoConversions.ConvertBrepToMesh(brep, points, nodes, curves, null, mem1ds, 
        meshSize, unit, tolerance, MeshMode2d.Mixed).Item1;
      Tuple<List<Element>, Point3dList, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(Mesh, prop, true);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      Ids = new List<int>(new int[Mesh.Faces.Count]);
      var singleProp = new GsaProperty2d(prop);
      for (int i = 0; i < Mesh.Faces.Count; i++) {
        Prop2ds.Add(singleProp);
      }
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaElement2d(
      ConcurrentDictionary<int, Element> elements, Mesh mesh, ConcurrentDictionary<int, GsaProperty2d> prop2ds) {
      Mesh = mesh;
      Topology = new Point3dList(mesh.Vertices.ToPoint3dArray());
      TopoInt = RhinoConversions.ConvertMeshToElem2d(Mesh);
      ApiElements = elements.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
      Ids = elements.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key).ToList();
      if (!prop2ds.IsNullOrEmpty()) {
        Prop2ds = prop2ds.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
      }
    }

    public void CreateSection3dPreview() {
      Section3dPreview = new Section3dPreview(this);
    }

    public List<Element> DuplicateApiObjects() {
      if (ApiElements.IsNullOrEmpty()) {
        return ApiElements;
      }

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

        // workaround to handle that Color is non-nullable type
        if ((Color)ApiElements[i].Colour != Color.FromArgb(0, 0, 0)) {
          elems[i].Colour = ApiElements[i].Colour;
        }
      }

      return elems;
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
      if (!Mesh.IsValid) {
        return "Null";
      }

      string type = Mappings.elementTypeMapping.FirstOrDefault(
        x => x.Value == ApiElements.First().Type).Key;
      string info = "N:" + Mesh.Vertices.Count + " E:" + ApiElements.Count;
      return string.Join(" ", type, info).TrimSpaces();
    }

    public void UpdateMeshColours() {
      for (int i = 0; i < ApiElements.Count; i++) {
        Mesh.VertexColors.SetColor(i, (Color)ApiElements[i].Colour);
      }
    }
  }
}
