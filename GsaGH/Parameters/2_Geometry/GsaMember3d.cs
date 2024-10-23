using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;
using Polyline = Rhino.Geometry.Polyline;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Members in GSA are geometrical objects used in the Design Layer. Members can automatically intersection with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. </para>
  /// <para>A Member3D is the volumetric geometry resembling for instance soil. It can be defined geometrically by a closed Solid (either Mesh or Brep).</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-member.html">Members</see> to read more.</para>
  /// </summary>
  public class GsaMember3d {
    public Member ApiMember { get; internal set; }
    public int Id { get; set; } = 0;
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public Mesh SolidMesh { get; private set; }
    public List<Line> PreviewEdgeLines { get; private set; }
    public List<Polyline> PreviewHiddenLines { get; private set; }
    public Point3dList PreviewPts { get; private set; }
    public GsaProperty3d Prop3d { get; set; }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaMember3d() {
      ApiMember = new Member() {
        Type = MemberType.GENERIC_3D,
        Group = GsaMemberDefaults.GroupValue,
      };
    }

    /// <summary>
    /// Create new instance by casting from a Mesh
    /// </summary>
    public GsaMember3d(Mesh mesh) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_3D,
        Group = GsaMemberDefaults.GroupValue,
      };
      SolidMesh = RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
      UpdatePreview();
    }

    /// <summary>
    /// Create new instance by casting from a Brep
    /// </summary>
    public GsaMember3d(Brep brep) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_3D,
        Group = GsaMemberDefaults.GroupValue,
      };
      SolidMesh = RhinoConversions.ConvertBrepToTriMeshSolid(brep);
      UpdatePreview();
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaMember3d(GsaMember3d other) {
      Id = other.Id;
      ApiMember = other.DuplicateApiObject();
      SolidMesh = (Mesh)other.SolidMesh?.DuplicateShallow();
      PreviewEdgeLines = other.PreviewEdgeLines;
      PreviewHiddenLines = other.PreviewHiddenLines;
      PreviewPts = other.PreviewPts;
      Prop3d = other.Prop3d;
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaMember3d(Member member, int id, Mesh mesh, GsaProperty3d prop, LengthUnit modelUnit) {
      ApiMember = member;
      Id = id;
      SolidMesh = RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
      Prop3d = prop;
      ApiMember.MeshSize = new Length(member.MeshSize, LengthUnit.Meter).As(modelUnit);
      ApiMember.Group = member.Group;
      UpdatePreview();
    }

    public Member DuplicateApiObject() {
      var mem = new Member {
        Group = ApiMember.Group,
        IsDummy = ApiMember.IsDummy,
        IsIntersector = ApiMember.IsIntersector,
        MeshSize = ApiMember.MeshSize,
        Name = ApiMember.Name,
        OrientationAngle = ApiMember.OrientationAngle,
        OrientationNode = ApiMember.OrientationNode,
        Property = ApiMember.Property,
        Type = ApiMember.Type,
      };
      if (ApiMember.Topology != string.Empty) {
        mem.Topology = ApiMember.Topology;
      }

      mem.Offset.X1 = ApiMember.Offset.X1;
      mem.Offset.X2 = ApiMember.Offset.X2;
      mem.Offset.Y = ApiMember.Offset.Y;
      mem.Offset.Z = ApiMember.Offset.Z;

      if ((Color)ApiMember.Colour
        != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
      {
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    public override string ToString() {
      string id = Id > 0 ? $"ID:{Id}" : string.Empty;
      string type = Mappings._memberTypeMapping.FirstOrDefault(x => x.Value == ApiMember.Type).Key;
      return string.Join(" ", id, type).TrimSpaces();
    }

    public void UpdateGeometry(Brep brep) {
      SolidMesh = RhinoConversions.ConvertBrepToTriMeshSolid(brep);
      UpdatePreview();
    }

    public void UpdateGeometry(Mesh mesh) {
      SolidMesh = RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
      UpdatePreview();
    }

    public void UpdatePreview() {
      MeshTopologyEdgeList alledges = SolidMesh.TopologyEdges;
      if (SolidMesh.FaceNormals.Count < SolidMesh.Faces.Count) {
        SolidMesh.FaceNormals.ComputeFaceNormals();
      }

      PreviewHiddenLines = new List<Polyline>();
      PreviewEdgeLines = new List<Line>();
      for (int i = 0; i < alledges.Count; i++) {
        int[] faceId = alledges.GetConnectedFaces(i);
        Vector3d vec1 = SolidMesh.FaceNormals[faceId[0]];
        Vector3d vec2 = SolidMesh.FaceNormals[faceId[1]];
        vec1.Unitize();
        vec2.Unitize();
        if (!vec1.Equals(vec2) || faceId.Length > 2) {
          PreviewEdgeLines.Add(alledges.EdgeLine(i));
        } else {
          var hidden = new Polyline {
            alledges.EdgeLine(i).PointAt(0),
            alledges.EdgeLine(i).PointAt(1),
          };
          PreviewHiddenLines.Add(hidden);
        }
      }

      PreviewPts = new Point3dList(SolidMesh.Vertices.ToPoint3dArray());
    }
  }
}
