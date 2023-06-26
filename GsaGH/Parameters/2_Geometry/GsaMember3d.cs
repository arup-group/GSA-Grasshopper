using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaApi;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Member3d class, this class defines the basic properties and methods for any Gsa Member 3d
  /// </summary>
  public class GsaMember3d {
    public Color Colour {
      get => (Color)ApiMember.Colour;
      set {
        CloneApiObject();
        ApiMember.Colour = value;
      }
    }
    public int Group {
      get => ApiMember.Group;
      set {
        CloneApiObject();
        ApiMember.Group = value;
      }
    }
    public Guid Guid => _guid;
    public int Id {
      get => _id;
      set {
        CloneApiObject();
        _id = value;
      }
    }
    public bool IsDummy {
      get => ApiMember.IsDummy;
      set {
        CloneApiObject();
        ApiMember.IsDummy = value;
      }
    }
    public double MeshSize { get; set; }
    public bool MeshWithOthers {
      get => ApiMember.IsIntersector;
      set {
        CloneApiObject();
        ApiMember.IsIntersector = value;
      }
    }
    public string Name {
      get => ApiMember.Name;
      set {
        CloneApiObject();
        ApiMember.Name = value;
      }
    }
    public GsaProp3d Prop3d { get; set; } = new GsaProp3d();
    public Mesh SolidMesh {
      get => _mesh;
      set {
        _mesh = RhinoConversions.ConvertMeshToTriMeshSolid(value);
        _guid = Guid.NewGuid();
        UpdatePreview();
      }
    }
    internal Member ApiMember { get; set; } = new Member();
    internal List<Line> _previewEdgeLines;
    internal List<Polyline> _previewHiddenLines;
    internal List<Point3d> _previewPts;
    private Guid _guid = Guid.NewGuid();
    private int _id = 0;
    private Mesh _mesh = new Mesh();

    public GsaMember3d() {
      ApiMember.Type = MemberType.GENERIC_3D;
    }

    public GsaMember3d(Mesh mesh) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_3D,
      };
      _mesh = RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
      Prop3d = new GsaProp3d(0);
      UpdatePreview();
    }

    public GsaMember3d(Brep brep) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_3D,
      };
      _mesh = RhinoConversions.ConvertBrepToTriMeshSolid(brep);
      Prop3d = new GsaProp3d(0);
      UpdatePreview();
    }

    internal GsaMember3d(Member member, int id, Mesh mesh, GsaProp3d prop, double meshSize) {
      ApiMember = member;
      _id = id;
      _mesh = RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
      Prop3d = prop;
      MeshSize = meshSize;
      UpdatePreview();
    }

    public GsaMember3d Clone() {
      var dup = new GsaMember3d {
        MeshSize = MeshSize,
        _mesh = (Mesh)_mesh.DuplicateShallow(),
        Prop3d = Prop3d.Duplicate(),
        Id = Id,
        ApiMember = GetAPI_MemberClone(),
        _guid = Guid.NewGuid()
      };

      dup.UpdatePreview();
      return dup;
    }

    public GsaMember3d Duplicate() {
      return this;
    }

    public GsaMember3d Morph(SpaceMorph xmorph) {
      if (SolidMesh == null) {
        return null;
      }

      GsaMember3d dup = Clone();
      dup.Id = 0;
      xmorph.Morph(dup.SolidMesh.Duplicate());

      return dup;
    }

    public override string ToString() {
      string idd = Id == 0 ? string.Empty : "ID:" + Id + " ";
      string type = Mappings.memberTypeMapping.FirstOrDefault(x => x.Value == ApiMember.Type).Key;
      return string.Join(" ", idd.Trim(), type.Trim()).Trim().Replace("  ", " ");
    }

    public GsaMember3d Transform(Transform xform) {
      if (SolidMesh == null) {
        return null;
      }

      GsaMember3d dup = Clone();
      dup.Id = 0;
      dup.SolidMesh.Transform(xform);

      return dup;
    }

    public GsaMember3d UpdateGeometry(Brep brep) {
      GsaMember3d dup = Clone();
      dup._mesh = RhinoConversions.ConvertBrepToTriMeshSolid(brep);
      dup.UpdatePreview();
      return dup;
    }

    public GsaMember3d UpdateGeometry(Mesh mesh) {
      GsaMember3d dup = Clone();
      dup._mesh = RhinoConversions.ConvertMeshToTriMeshSolid(mesh);
      dup.UpdatePreview();
      return dup;
    }

    internal void CloneApiObject() {
      ApiMember = GetAPI_MemberClone();
      _guid = Guid.NewGuid();
    }

    internal Member GetAPI_MemberClone() {
      var mem = new Member {
        Group = ApiMember.Group,
        IsDummy = ApiMember.IsDummy,
        IsIntersector = ApiMember.IsIntersector,
        MeshSize = ApiMember.MeshSize,
        Name = ApiMember.Name,
        Offset = ApiMember.Offset,
        OrientationAngle = ApiMember.OrientationAngle,
        OrientationNode = ApiMember.OrientationNode,
        Property = ApiMember.Property,
        Type = ApiMember.Type,
      };
      if (ApiMember.Topology != string.Empty) {
        mem.Topology = ApiMember.Topology;
      }

      if ((Color)ApiMember.Colour
        != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
      {
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    private void UpdatePreview() {
      Display.PreviewMem3d(ref _mesh, ref _previewHiddenLines, ref _previewEdgeLines,
        ref _previewPts);
    }
  }
}
