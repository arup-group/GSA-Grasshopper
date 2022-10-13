using System;
using System.Collections.Generic;
using System.Drawing;
using GsaAPI;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
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

    private int _id = 0;
    private Member _member = new Member();
    private Mesh _mesh = new Mesh();
    private GsaProp3d _prop = new GsaProp3d();
    #endregion

    #region properties
    internal Member API_Member
    {
      get
      {
        return this._member;
      }
      set
      {
        this._member = value;
      }
    }
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
    public int Id
    {
      get
      {
        return this._id;
      }
      set
      {
        this._id = value;
      }
    }
    public GsaProp3d Property
    {
      get
      {
        return this._prop;
      }
      set
      {
        this._prop = value;
      }
    }
    #region GsaAPI.Member members
    public Color Colour
    {
      get
      {
        return (Color)this._member.Colour;
      }
      set
      {
        CloneApiMember();
        this._member.Colour = value;
      }
    }
    public int Group
    {
      get
      {
        return this._member.Group;
      }
      set
      {
        CloneApiMember();
        this._member.Group = value;
      }
    }
    public bool IsDummy
    {
      get
      {
        return this._member.IsDummy;
      }
      set
      {
        CloneApiMember();
        this._member.IsDummy = value;
      }
    }
    public string Name
    {
      get
      {
        return this._member.Name;
      }
      set
      {
        CloneApiMember();
        this._member.Name = value;
      }
    }
    public Length MeshSize
    {
      get
      {
        Length l = new Length(this._member.MeshSize, LengthUnit.Meter);
        return new Length(l.As(DefaultUnits.LengthUnitGeometry), DefaultUnits.LengthUnitGeometry);
      }
      set
      {
        CloneApiMember();
        this._member.MeshSize = value.Meters;
      }
    }
    public bool MeshWithOthers
    {
      get
      {
        return this._member.IsIntersector;
      }
      set
      {
        CloneApiMember();
        this._member.IsIntersector = value;
      }
    }
    public int PropertyID
    {
      get
      {
        return this._member.Property;
      }
      set
      {
        CloneApiMember();
        this._member.Property = value;
      }
    }
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion
    #endregion

    #region constructors
    public GsaMember3d()
    {
      this._member.Type = MemberType.GENERIC_3D;
    }

    internal GsaMember3d(Member member, int id, Mesh mesh)
    {
      this._member = member;
      this._id = id;
      this._mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      this.UpdatePreview();
    }

    internal GsaMember3d(Member member, int id, Mesh mesh, GsaProp3d prop)
    {
      this._member = member;
      this._id = id;
      this._mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      this._prop = prop.Duplicate();
      this.UpdatePreview();
    }

    public GsaMember3d(Mesh mesh)
    {
      this._member = new Member
      {
        Type = MemberType.GENERIC_3D
      };
      this._mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      this.UpdatePreview();
    }

    public GsaMember3d(Brep brep)
    {
      this._member = new Member
      {
        Type = MemberType.GENERIC_3D
      };
      this._prop = new GsaProp3d();
      this._mesh = GsaGH.Util.GH.Convert.ConvertBrepToTriMeshSolid(brep);
      this.UpdatePreview();
    }
    #endregion

    #region methods
    public GsaMember3d Duplicate(bool cloneApiMember = false)
    {
      GsaMember3d dup = new GsaMember3d();
      dup._mesh = (Mesh)this._mesh.DuplicateShallow();
      dup._prop = this._prop.Duplicate();
      if (cloneApiMember)
        dup.CloneApiMember();
      else
        dup._member = this._member;
      dup._id = this._id;
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
      string idd = " " + Id.ToString();
      if (Id == 0) { idd = ""; }
      string mes = this._member.Type.ToString();
      string typeTxt = "GSA " + Char.ToUpper(mes[0]) + mes.Substring(1).ToLower().Replace("_", " ") + " Member" + idd;
      typeTxt = typeTxt.Replace("3d", "3D");
      string valid = (this.IsValid) ? "" : "Invalid ";
      return valid + typeTxt;
    }

    internal void CloneApiMember()
    {
      this._member = GetAPI_MemberClone();
    }

    internal Member GetAPI_MemberClone()
    {
      Member mem = new Member
      {
        Group = this._member.Group,
        IsDummy = this._member.IsDummy,
        MeshSize = this._member.MeshSize,
        Name = this._member.Name,
        Offset = this._member.Offset,
        OrientationAngle = this._member.OrientationAngle,
        OrientationNode = this._member.OrientationNode,
        Property = this._member.Property,
        Type = this._member.Type,
      };
      if (this._member.Topology != String.Empty)
        mem.Topology = this._member.Topology;

      if ((Color)this._member.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        mem.Colour = this._member.Colour;

      return mem;
    }

    private void UpdatePreview()
    {
      GsaGH.UI.Display.PreviewMem3d(ref this._mesh, ref this.previewHiddenLines, ref this.previewEdgeLines, ref this.previewPts);
    }
    #endregion
  }
}
