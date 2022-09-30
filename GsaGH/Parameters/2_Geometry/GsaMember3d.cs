using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
    internal Member API_Member
    {
      get { return m_member; }
      set { m_member = value; }
    }

    internal Member GetAPI_MemberClone()
    {
      Member mem = new Member
      {
        Group = m_member.Group,
        IsDummy = m_member.IsDummy,
        MeshSize = m_member.MeshSize,
        Name = m_member.Name,
        Offset = m_member.Offset,
        OrientationAngle = m_member.OrientationAngle,
        OrientationNode = m_member.OrientationNode,
        Property = m_member.Property,
        Type = m_member.Type,
      };
      if (m_member.Topology != String.Empty)
        mem.Topology = m_member.Topology;

      if ((System.Drawing.Color)m_member.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        mem.Colour = m_member.Colour;

      return mem;
    }

    public Mesh SolidMesh
    {
      get { return m_mesh; }
      set
      {
        m_mesh = Util.GH.Convert.ConvertMeshToTriMeshSolid(value);
        UpdatePreview();
      }
    }

    public int ID
    {
      get { return m_id; }
      set { m_id = value; }
    }

    public GsaProp3d Property
    {
      get { return m_prop; }
      set
      {
        if (m_prop == null)
          PropertyID = 0;
        m_prop = value;
      }
    }

    #region GsaAPI.Member members
    public System.Drawing.Color Colour
    {
      get
      {
        return (System.Drawing.Color)m_member.Colour;
      }
      set
      {
        CloneApiMember();
        m_member.Colour = value;
      }
    }
    public int Group
    {
      get { return m_member.Group; }
      set
      {
        CloneApiMember();
        m_member.Group = value;
      }
    }
    public bool IsDummy
    {
      get { return m_member.IsDummy; }
      set
      {
        CloneApiMember();
        m_member.IsDummy = value;
      }
    }
    public string Name
    {
      get { return m_member.Name; }
      set
      {
        CloneApiMember();
        m_member.Name = value;
      }
    }
    public Length MeshSize
    {
      get
      {
        Length l = new Length(m_member.MeshSize, LengthUnit.Meter);
        return new Length(l.As(DefaultUnits.LengthUnitGeometry), DefaultUnits.LengthUnitGeometry);
      }
      set
      {
        CloneApiMember();
        m_member.MeshSize = value.Meters;
      }
    }
    public bool MeshWithOthers
    {
      get
      {
        return m_member.IsIntersector;
      }
      set
      {
        CloneApiMember();
        m_member.IsIntersector = value;
      }
    }
    public int PropertyID
    {
      get { return m_member.Property; }
      set
      {
        CloneApiMember();
        m_member.Property = value;
      }
    }
    internal void CloneApiMember()
    {
      m_member = GetAPI_MemberClone();
    }
    #endregion
    #region preview
    internal List<Polyline> previewHiddenLines;
    internal List<Line> previewEdgeLines;
    internal List<Point3d> previewPts;
    private void UpdatePreview()
    {
      GsaGH.UI.Display.PreviewMem3d(ref m_mesh, ref previewHiddenLines, ref previewEdgeLines, ref previewPts);
    }
    #endregion
    #region fields
    private Member m_member;
    private int m_id = 0;

    private Mesh m_mesh;
    private GsaProp3d m_prop;
    #endregion

    #region constructors
    public GsaMember3d()
    {
      m_member = new Member();
      m_member.Type = MemberType.GENERIC_3D;
      m_mesh = new Mesh();
      m_prop = new GsaProp3d();
    }

    internal GsaMember3d(Member member, int id, Mesh mesh)
    {
      m_member = member;
      m_id = id;
      m_mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      m_prop = new GsaProp3d();
      UpdatePreview();
    }

    internal GsaMember3d(Member member, int id, Mesh mesh, GsaProp3d prop)
    {
      m_member = member;
      m_id = id;
      m_mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      m_prop = prop.Duplicate();
      UpdatePreview();
    }

    public GsaMember3d(Mesh mesh)
    {
      m_member = new Member
      {
        Type = MemberType.GENERIC_3D
      };
      m_prop = new GsaProp3d();
      m_mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      if (m_mesh == null)
      {
        throw new Exception("Unable to convert Mesh to solid mesh");
      }
      else
      {
        UpdatePreview();
      }
    }

    public GsaMember3d(Brep brep)
    {
      m_member = new Member
      {
        Type = MemberType.GENERIC_3D
      };
      m_prop = new GsaProp3d();
      m_mesh = GsaGH.Util.GH.Convert.ConvertBrepToTriMeshSolid(brep);
      if (m_mesh == null)
      {
        throw new Exception("Unable to convert Brep to solid mesh");
      }
      else
      {
        UpdatePreview();
      }
    }

    public GsaMember3d Duplicate(bool cloneApiMember = false)
    {
      GsaMember3d dup = new GsaMember3d();
      dup.m_mesh = (Mesh)this.m_mesh.DuplicateShallow();
      dup.m_prop = this.m_prop.Duplicate();
      if (cloneApiMember)
        dup.CloneApiMember();
      else
        dup.m_member = this.m_member;
      dup.m_id = this.m_id;
      dup.UpdatePreview();
      return dup;
    }

    public GsaMember3d UpdateGeometry(Brep brep)
    {
      if (this == null) { return null; }
      GsaMember3d dup = this.Duplicate();
      dup.m_mesh = GsaGH.Util.GH.Convert.ConvertBrepToTriMeshSolid(brep);
      dup.UpdatePreview();
      return dup;
    }

    public GsaMember3d UpdateGeometry(Mesh mesh)
    {
      if (this == null) { return null; }
      GsaMember3d dup = this.Duplicate();
      dup.m_mesh = GsaGH.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
      dup.UpdatePreview();
      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (m_mesh == null)
          return false;
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string idd = " " + ID.ToString();
      if (ID == 0) { idd = ""; }
      string mes = m_member.Type.ToString();
      string typeTxt = "GSA " + Char.ToUpper(mes[0]) + mes.Substring(1).ToLower().Replace("_", " ") + " Member" + idd;
      typeTxt = typeTxt.Replace("3d", "3D");
      string valid = (this.IsValid) ? "" : "Invalid ";
      return valid + typeTxt;
    }
    #endregion
  }
}
