using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Member1d class, this class defines the basic properties and methods for any Gsa Member 1d
  /// </summary>
  public class GsaMember1d
  {
    public PolyCurve PolyCurve
    {
      get { return m_crv; }
      set
      {
        Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Util.GH.Convert.ConvertMem1dCrv(value);
        m_crv = convertCrv.Item1;
        m_topo = convertCrv.Item2;
        m_topoType = convertCrv.Item3;
        UpdatePreview();
      }
    }
    public int Id
    {
      get { return m_id; }
      set { m_id = value; }
    }

    public List<Point3d> Topology
    {
      get { return m_topo; }
    }

    public List<string> TopologyType
    {
      get { return m_topoType; }
    }

    public GsaBool6 ReleaseStart
    {
      get
      {
        return new GsaBool6(m_member.GetEndRelease(0).Releases);
      }
      set
      {
        m_rel1 = value;
        if (m_rel1 == null) { m_rel1 = new GsaBool6(); }
        CloneApiMember();
        m_member.SetEndRelease(0, new EndRelease(m_rel1._bool6));
        UpdatePreview();
      }
    }
    public GsaBool6 ReleaseEnd
    {
      get
      {
        return new GsaBool6(m_member.GetEndRelease(1).Releases);
      }
      set
      {
        m_rel2 = value;
        if (m_rel2 == null) { m_rel2 = new GsaBool6(); }
        CloneApiMember();
        m_member.SetEndRelease(1, new EndRelease(m_rel2._bool6));
        UpdatePreview();
      }
    }
    internal Tuple<Vector3d, Vector3d, Vector3d> LocalAxes
    {
      get
      {
        return UI.Display.GetLocalPlane(m_crv, m_crv.GetLength() / 2, m_member.OrientationAngle * Math.PI / 180.0);
      }
    }
    public GsaSection Section
    {
      get { return m_section; }
      set { m_section = value; }
    }
    #region GsaAPI.Member members
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
        IsIntersector = m_member.IsIntersector,
        LateralTorsionalBucklingFactor = m_member.LateralTorsionalBucklingFactor,
        MeshSize = m_member.MeshSize,
        MomentAmplificationFactorStrongAxis = m_member.MomentAmplificationFactorStrongAxis,
        MomentAmplificationFactorWeakAxis = m_member.MomentAmplificationFactorWeakAxis,
        Name = m_member.Name.ToString(),
        Offset = m_member.Offset,
        OrientationAngle = m_member.OrientationAngle,
        OrientationNode = m_member.OrientationNode,
        Property = m_member.Property,
        Type = m_member.Type,
        Type1D = m_member.Type1D
      };
      if (m_member.Topology != String.Empty)
        mem.Topology = m_member.Topology;
      
      mem.SetEndRelease(0, m_member.GetEndRelease(0));
      mem.SetEndRelease(1, m_member.GetEndRelease(1));

      if ((System.Drawing.Color)m_member.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        mem.Colour = m_member.Colour;

      return mem;
    }
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
        UpdatePreview();
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
        UpdatePreview();
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
    public GsaOffset Offset
    {
      get
      {
        return new GsaOffset(
            m_member.Offset.X1,
            m_member.Offset.X2,
            m_member.Offset.Y,
            m_member.Offset.Z);
      }
      set
      {
        CloneApiMember();
        m_member.Offset.X1 = value.X1.Meters;
        m_member.Offset.X2 = value.X2.Meters;
        m_member.Offset.Y = value.Y.Meters;
        m_member.Offset.Z = value.Z.Meters;
      }
    }
    public double OrientationAngle
    {
      get { return m_member.OrientationAngle; }
      set
      {
        CloneApiMember();
        m_member.OrientationAngle = value;
      }
    }
    public GsaNode OrientationNode
    {
      get { return m_orientationNode; }
      set
      {
        CloneApiMember();
        m_orientationNode = value;
      }
    }
    public MemberType Type
    {
      get { return m_member.Type; }
      set
      {
        CloneApiMember();
        m_member.Type = value;
      }
    }
    public ElementType Type1D
    {
      get { return m_member.Type1D; }
      set
      {
        CloneApiMember();
        m_member.Type1D = value;
      }
    }
    internal void CloneApiMember()
    {
      m_member = this.GetAPI_MemberClone();
    }
    #endregion
    #region preview
    #region preview lines
    private Line previewSX1;
    private Line previewSX2;
    private Line previewSY1;
    private Line previewSY2;
    private Line previewSY3;
    private Line previewSY4;
    private Line previewSZ1;
    private Line previewSZ2;
    private Line previewSZ3;
    private Line previewSZ4;
    private Line previewEX1;
    private Line previewEX2;
    private Line previewEY1;
    private Line previewEY2;
    private Line previewEY3;
    private Line previewEY4;
    private Line previewEZ1;
    private Line previewEZ2;
    private Line previewEZ3;
    private Line previewEZ4;
    private Line previewSXX;
    private Line previewSYY1;
    private Line previewSYY2;
    private Line previewSZZ1;
    private Line previewSZZ2;
    private Line previewEXX;
    private Line previewEYY1;
    private Line previewEYY2;
    private Line previewEZZ1;
    private Line previewEZZ2;

    internal List<Line> previewGreenLines;
    internal List<Line> previewRedLines;
    #endregion
    private void UpdatePreview()
    {
      if (m_rel1 != null & m_rel2 != null)
      {
        if (m_rel1.X || m_rel1.Y || m_rel1.Z || m_rel1.XX || m_rel1.YY || m_rel1.ZZ ||
        m_rel2.X || m_rel2.Y || m_rel2.Z || m_rel2.XX || m_rel2.YY || m_rel2.ZZ)
        {
          #region add lines
          previewGreenLines = new List<Line>();
          previewGreenLines.Add(previewSX1);
          previewGreenLines.Add(previewSX2);
          previewGreenLines.Add(previewSY1);
          previewGreenLines.Add(previewSY2);
          previewGreenLines.Add(previewSY3);
          previewGreenLines.Add(previewSY4);
          previewGreenLines.Add(previewSZ1);
          previewGreenLines.Add(previewSZ2);
          previewGreenLines.Add(previewSZ3);
          previewGreenLines.Add(previewSZ4);
          previewGreenLines.Add(previewEX1);
          previewGreenLines.Add(previewEX2);
          previewGreenLines.Add(previewEY1);
          previewGreenLines.Add(previewEY2);
          previewGreenLines.Add(previewEY3);
          previewGreenLines.Add(previewEY4);
          previewGreenLines.Add(previewEZ1);
          previewGreenLines.Add(previewEZ2);
          previewGreenLines.Add(previewEZ3);
          previewGreenLines.Add(previewEZ4);
          previewRedLines = new List<Line>();
          previewRedLines.Add(previewSXX);
          previewRedLines.Add(previewSYY1);
          previewRedLines.Add(previewSYY2);
          previewRedLines.Add(previewSZZ1);
          previewRedLines.Add(previewSZZ2);
          previewRedLines.Add(previewEXX);
          previewRedLines.Add(previewEYY1);
          previewRedLines.Add(previewEYY2);
          previewRedLines.Add(previewEZZ1);
          previewRedLines.Add(previewEZZ2);
          #endregion
          GsaGH.UI.Display.Preview1D(m_crv, m_member.OrientationAngle * Math.PI / 180.0, m_rel1, m_rel2,
          ref previewGreenLines, ref previewRedLines);
        }
        else
          previewGreenLines = null;
      }
    }
    #endregion
    #region fields
    private Member m_member;
    private int m_id = 0;

    private PolyCurve m_crv; //Polyline for visualisation /member1d/member2d
    private List<Point3d> m_topo; // list of topology points for visualisation /member1d/member2d
    private List<string> m_topoType; //list of polyline curve type (arch or line) for member1d/2d
    private GsaBool6 m_rel1;
    private GsaBool6 m_rel2;
    private GsaSection m_section;
    private GsaNode m_orientationNode;
    #endregion

    #region constructors
    public GsaMember1d()
    {
      m_member = new Member();
      m_crv = new PolyCurve();
      m_section = new GsaSection();
    }

    internal GsaMember1d(Member member, int id, List<Point3d> topology, List<string> topo_type, GsaSection section, GsaNode orientationNode)
    {
      m_member = member;
      m_id = id;
      m_crv = Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(topology, topo_type);
      m_topo = topology;
      m_topoType = topo_type;
      m_section = section;
      m_rel1 = new GsaBool6(m_member.GetEndRelease(0).Releases);
      m_rel2 = new GsaBool6(m_member.GetEndRelease(1).Releases);
      if (orientationNode != null)
        m_orientationNode = orientationNode;
      UpdatePreview();
    }

    public GsaMember1d(Curve crv, int prop = 0)
    {
      m_member = new Member
      {
        Type = MemberType.GENERIC_1D,
        Property = prop
      };
      Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Util.GH.Convert.ConvertMem1dCrv(crv);
      m_crv = convertCrv.Item1;
      m_topo = convertCrv.Item2;
      m_topoType = convertCrv.Item3;

      m_section = new GsaSection();
      UpdatePreview();
    }
    public GsaMember1d Duplicate(bool cloneApiMember = false)
    {
      if (this == null) { return null; }

      GsaMember1d dup = new GsaMember1d();
      dup.m_id = m_id;
      dup.m_member = m_member;
      if (cloneApiMember)
        dup.CloneApiMember();
      dup.m_crv = (PolyCurve)m_crv.DuplicateShallow();
      if (m_rel1 != null)
        dup.m_rel1 = m_rel1.Duplicate();
      if (m_rel2 != null)
        dup.m_rel2 = m_rel2.Duplicate();
      dup.m_section = m_section.Duplicate();
      dup.m_topo = m_topo;
      dup.m_topoType = m_topoType;
      if (m_orientationNode != null)
        dup.m_orientationNode = m_orientationNode.Duplicate(cloneApiMember);
      dup.UpdatePreview();
      return dup;
    }
    public GsaMember1d Transform(Transform xform)
    {
      if (this == null) { return null; }

      GsaMember1d dup = this.Duplicate(true);
      dup.Id = 0;

      List<Point3d> pts = m_topo.ToList();
      Point3dList xpts = new Point3dList(pts);
      xpts.Transform(xform);
      dup.m_topo = xpts.ToList();

      if (m_crv != null)
      {
        PolyCurve crv = m_crv.DuplicatePolyCurve();
        crv.Transform(xform);
        dup.m_crv = crv;
      }
      dup.UpdatePreview();
      return dup;
    }
    public GsaMember1d Morph(SpaceMorph xmorph)
    {
      if (this == null) { return null; }

      GsaMember1d dup = this.Duplicate(true);
      dup.Id = 0;

      List<Point3d> pts = m_topo.ToList();
      for (int i = 0; i < pts.Count; i++)
        pts[i] = xmorph.MorphPoint(pts[i]);
      dup.m_topo = pts;

      if (m_crv != null)
      {
        PolyCurve crv = m_crv.DuplicatePolyCurve();
        xmorph.Morph(crv);
        dup.m_crv = crv;
      }
      dup.UpdatePreview();
      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (m_crv == null)
          return false;
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string idd = " " + Id.ToString();
      if (Id == 0) { idd = ""; }
      string mes = m_member.Type.ToString();
      string typeTxt = "GSA " + Char.ToUpper(mes[0]) + mes.Substring(1).ToLower().Replace("_", " ") + " Member" + idd;
      typeTxt = typeTxt.Replace("1d", "1D");
      string valid = (this.IsValid) ? "" : "Invalid ";
      return valid + typeTxt;
    }

    #endregion
  }
}
