﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Member3d class, this class defines the basic properties and methods for any Gsa Member 3d
  /// </summary>
  public class GsaMember3d
  {
    #region fields
    internal List<Polyline> previewHiddenLines;
    internal List<Line> previewEdgeLines;
    internal List<Point3d> previewPts;

    private Mesh _mesh = new Mesh();
    #endregion

    #region properties
    internal Member ApiMember { get; set; } = new Member();
    public int Id { get; set; } = 0;
    public double MeshSize { get; set; }
    public GsaProp3d Property { get; set; } = new GsaProp3d();
    public Mesh SolidMesh
    {
      get
      {
        return this._mesh;
      }
      set
      {
        this._mesh = Util.GH.Convert.ConvertMeshToTriMeshSolid(value);
        this.UpdatePreview();
      }
    }
    public Color Colour
    {
      get
      {
        return (Color)this.ApiMember.Colour;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Colour = value;
      }
    }
    public int Group
    {
      get
      {
        return this.ApiMember.Group;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Group = value;
      }
    }
    public bool IsDummy
    {
      get
      {
        return this.ApiMember.IsDummy;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.IsDummy = value;
      }
    }
    public string Name
    {
      get
      {
        return this.ApiMember.Name;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Name = value;
      }
    }
    public bool MeshWithOthers
    {
      get
      {
        return this.ApiMember.IsIntersector;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.IsIntersector = value;
      }
    }
    public int PropertyID
    {
      get
      {
        return this.ApiMember.Property;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Property = value;
      }
    }
    #endregion

    #region constructors
    public GsaMember3d()
    {
      this.ApiMember.Type = MemberType.GENERIC_3D;
    }

    internal GsaMember3d(Member member, int id, Mesh mesh)
    {
      this.ApiMember = member;
      this.Id = id;
      this._mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      this.UpdatePreview();
    }

    internal GsaMember3d(Member member, int id, Mesh mesh, GsaProp3d prop)
    {
      this.ApiMember = member;
      this.Id = id;
      this._mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      this.Property = prop.Duplicate();
      this.UpdatePreview();
    }

    public GsaMember3d(Mesh mesh)
    {
      this.ApiMember = new Member
      {
        Type = MemberType.GENERIC_3D
      };
      this._mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      this.UpdatePreview();
    }

    public GsaMember3d(Brep brep)
    {
      this.ApiMember = new Member
      {
        Type = MemberType.GENERIC_3D
      };
      this.Property = new GsaProp3d();
      this._mesh = GsaGH.Util.GH.Convert.ConvertBrepToTriMeshSolid(brep);
      this.UpdatePreview();
    }
    #endregion

    #region methods
    public GsaMember3d Duplicate(bool cloneApiMember = false)
    {
      GsaMember3d dup = new GsaMember3d();
      dup.MeshSize = this.MeshSize;
      dup._mesh = (Mesh)this._mesh.DuplicateShallow();
      dup.Property = this.Property.Duplicate();
      if (cloneApiMember)
        dup.CloneApiObject();
      else
        dup.ApiMember = this.ApiMember;
      dup.Id = this.Id;
      dup.UpdatePreview();
      return dup;
    }

    public GsaMember3d UpdateGeometry(Brep brep)
    {
      GsaMember3d dup = this.Duplicate();
      dup._mesh = GsaGH.Util.GH.Convert.ConvertBrepToTriMeshSolid(brep);
      dup.UpdatePreview();
      return dup;
    }

    public GsaMember3d UpdateGeometry(Mesh mesh)
    {
      GsaMember3d dup = this.Duplicate();
      dup._mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      dup.UpdatePreview();
      return dup;
    }

    public override string ToString()
    {
      string idd = this.Id == 0 ? "" : "ID:" + Id + " ";
      string type = Helpers.Mappings.MemberTypeMapping.FirstOrDefault(x => x.Value == this.ApiMember.Type).Key;
      return string.Join(" ", idd.Trim(), type.Trim()).Trim().Replace("  ", " ");
    }

    internal void CloneApiObject()
    {
      this.ApiMember = GetAPI_MemberClone();
    }

    internal Member GetAPI_MemberClone()
    {
      Member mem = new Member
      {
        Group = this.ApiMember.Group,
        IsDummy = this.ApiMember.IsDummy,
        MeshSize = this.ApiMember.MeshSize,
        Name = this.ApiMember.Name,
        Offset = this.ApiMember.Offset,
        OrientationAngle = this.ApiMember.OrientationAngle,
        OrientationNode = this.ApiMember.OrientationNode,
        Property = this.ApiMember.Property,
        Type = this.ApiMember.Type,
      };
      if (this.ApiMember.Topology != String.Empty)
        mem.Topology = this.ApiMember.Topology;

      if ((Color)this.ApiMember.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        mem.Colour = this.ApiMember.Colour;

      return mem;
    }

    private void UpdatePreview()
    {
      GsaGH.UI.Display.PreviewMem3d(ref this._mesh, ref this.previewHiddenLines, ref this.previewEdgeLines, ref this.previewPts);
    }
    #endregion
  }
}
