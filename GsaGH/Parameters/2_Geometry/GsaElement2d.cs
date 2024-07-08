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
using OasysGH.UI;
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
    public List<object> ApiElements { get; internal set; }
    public List<int> Ids { get; set; } = new List<int>();
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public Mesh Mesh { get; set; } = new Mesh();
    public List<List<int>> TopoInt { get; internal set; }
    public Point3dList Topology { get; internal set; }

    public List<GsaOffset> Offsets => ElementHelper.GsaGhOffset(ApiElements);
    public List<Angle> OrientationAngles => ElementHelper.OasysOrientationAngle(ApiElements);
    public List<GsaProperty2d> Prop2ds { get; set; }
    public Section3dPreview Section3dPreview { get; private set; }

    /// <summary>
    /// Empty constructor instantiating a list of new API objects
    /// </summary>
    public GsaElement2d() {
      ApiElements = new List<object>();
    }

    /// <summary>
    /// Create new instance by casting from a Mesh
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="isLoadPanel"></param>
    public GsaElement2d(Mesh mesh, bool isLoadPanel = false) {
      Mesh = mesh.DuplicateMesh();
      Mesh.Compact();
      Mesh.Vertices.CombineIdentical(true, false);
      Tuple<List<object>, Point3dList, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(mesh, isLoadPanel);
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
      Topology = other.Topology?.Duplicate();
      TopoInt = other.TopoInt;
      Prop2ds = other.Prop2ds;
      Section3dPreview = other.Section3dPreview?.Duplicate();
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaElement2d(
      ConcurrentDictionary<int, object> elements, Mesh mesh, ConcurrentDictionary<int, GsaProperty2d> prop2ds) {
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

    public List<object> DuplicateApiObjects() {
      if (ApiElements.IsNullOrEmpty()) {
        return ApiElements;
      }

      var elems = new List<object>();
      for (int i = 0; i < ApiElements.Count; i++) {
        object genericElement = ApiElements[i];
        if ((genericElement as Element) != null) {
          var element2d = genericElement as Element;
          var feElement = new Element() {
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
          if ((Color)element2d.Colour != Color.FromArgb(0, 0, 0)) {
            feElement.Colour = element2d.Colour;
          }
          elems.Add(feElement);
        }
        else {
          var element2d = genericElement as LoadPanelElement;
          var laodPanel = new LoadPanelElement() {
            Group = element2d.Group,
            IsDummy = element2d.IsDummy,
            Name = element2d.Name.ToString(),
            OrientationAngle = element2d.OrientationAngle,
            ParentMember = element2d.ParentMember,
            Property = element2d.Property,
            Topology = new ReadOnlyCollection<int>(element2d.Topology.ToList()),
          };
          if ((Color)element2d.Colour != Color.FromArgb(0, 0, 0)) {
            laodPanel.Colour = element2d.Colour;
          }
          elems.Add(laodPanel);
        }
      }
      return elems;
    }

    public Point3dList GetCenterPoints() {
      var points = new Point3dList();
      int faceIndex = 0;
      for (int i = 0; i < ApiElements.Count; i++) {
        Point3d pt = Mesh.Faces.GetFaceCenter(faceIndex);
        int index = 0;
        ElementType elementType = ElementType.BEAM;
        object genericElement = ApiElements[i];
        if ((genericElement as Element) != null) {
          var element2d = genericElement as Element;
          elementType = element2d.Type;
        }

        switch (elementType) {
          case ElementType.QUAD8:
            index = TopoInt[i][0];
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
      return points;
    }

    public DataTree<int> GetTopologyIDs() {
      var topos = new DataTree<int>();
      for (int i = 0; i < ApiElements.Count; i++) {
        if (ApiElements[i] != null) {
          object genericElement = ApiElements[i];
          if ((genericElement as Element) != null) {
            var element2d = genericElement as Element;
            topos.AddRange(element2d.Topology.ToList(), new GH_Path(Ids[i]));
          }
          else {
            var element2d = genericElement as LoadPanelElement;
            topos.AddRange(element2d.Topology.ToList(), new GH_Path(Ids[i]));
          }
        }
      }

      return topos;
    }

    public override string ToString() {
      if (!Mesh.IsValid) {
        return "Null";
      }
      string type = "";
      object genericElement = ApiElements.First();
      if ((genericElement as Element) != null) {
        var element2d = genericElement as Element;
        type = Mappings._elementTypeMapping.FirstOrDefault(
        x => x.Value == element2d.Type).Key;
      }

      string info = "N:" + Mesh.Vertices.Count + " E:" + ApiElements.Count;
      return string.Join(" ", type, info).TrimSpaces();
    }

    public void UpdateMeshColours() {
      for (int i = 0; i < ApiElements.Count; i++) {
        object genericElement = ApiElements[i];
        if ((genericElement as Element) != null) {
          var element2d = genericElement as Element;
          Mesh.VertexColors.SetColor(i, (Color)element2d.Colour);
        }
        else {
          var element2d = genericElement as LoadPanelElement;
          Mesh.VertexColors.SetColor(i, (Color)element2d.Colour);
        }
      }
    }
  }
}
