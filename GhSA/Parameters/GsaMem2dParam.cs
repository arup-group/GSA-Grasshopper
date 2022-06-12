using System;
using System.Collections.Generic;
using System.Linq;

using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using UnitsNet;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Member2d class, this class defines the basic properties and methods for any Gsa Member 2d
  /// </summary>
  public class GsaMember2d
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
        Name = m_member.Name.ToString(),
        Offset = m_member.Offset,
        OrientationAngle = m_member.OrientationAngle,
        OrientationNode = m_member.OrientationNode,
        Property = m_member.Property,
        Topology = m_member.Topology.ToString(),
        Type = m_member.Type,
        Type2D = m_member.Type2D
      };

      if ((System.Drawing.Color)m_member.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        mem.Colour = m_member.Colour;

      return mem;
    }
    public PolyCurve PolyCurve
    {
      get { return m_edgeCrv; }
      //set { m_crv = Util.GH.Convert.ConvertCurveMem2d(value); }
    }
    public int ID
    {
      get { return m_id; }
      set { m_id = value; }
    }
    public Brep Brep
    {
      get { return m_brep; }
    }
    public List<Point3d> Topology
    {
      get { return m_edgeCrv_topo; }
      //set { m_topo = value; }
    }
    public List<string> TopologyType
    {
      get { return m_edgeCrv_topoType; }
      //set { m_topoType = value; }
    }
    public List<List<Point3d>> VoidTopology
    {
      get { return m_voidCrvs_topo; }
      //set { m_void_topo = value; }
    }
    public List<List<string>> VoidTopologyType
    {
      get { return m_voidCrvs_topoType; }
      //set { m_void_topoType = value; }
    }
    public List<PolyCurve> InclusionLines
    {
      get { return m_inclCrvs; }
      //set { m_incl_Lines = value; }
    }
    public List<List<Point3d>> IncLinesTopology
    {
      get { return m_inclCrvs_topo; }
      //set { m_incLines_topo = value; }
    }
    public List<List<string>> IncLinesTopologyType
    {
      get { return m_inclCrvs_topoType; }
      //set { m_inclLines_topoType = value; }
    }
    public List<Point3d> InclusionPoints
    {
      get { return m_inclPts; }
      //set { m_incl_pts = value; }
    }
    public GsaProp2d Property
    {
      get { return m_prop; }
      set { m_prop = value; }
    }
    #region GsaAPI.Member members
    public System.Drawing.Color Colour
    {
      get
      {
        //if ((System.Drawing.Color)m_member.Colour == System.Drawing.Color.FromArgb(0, 0, 0))
        //    m_member.Colour = UI.Colour.Member1d;
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
        Length l = new Length(m_member.MeshSize, UnitsNet.Units.LengthUnit.Meter);
        return new Length(l.As(Units.LengthUnitGeometry), Units.LengthUnitGeometry);
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
    public int OrientationNode
    {
      get { return m_member.OrientationNode; }
      set
      {
        CloneApiMember();
        m_member.OrientationNode = value;
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
    public AnalysisOrder Type2D
    {
      get { return m_member.Type2D; }
      set
      {
        CloneApiMember();
        m_member.Type2D = value;
      }
    }
    private void CloneApiMember()
    {
      Member mem = new Member
      {
        Group = m_member.Group,
        IsDummy = m_member.IsDummy,
        IsIntersector = m_member.IsIntersector,
        MeshSize = m_member.MeshSize,
        LateralTorsionalBucklingFactor = m_member.LateralTorsionalBucklingFactor,
        MomentAmplificationFactorStrongAxis = m_member.MomentAmplificationFactorStrongAxis,
        MomentAmplificationFactorWeakAxis = m_member.MomentAmplificationFactorWeakAxis,
        Name = m_member.Name.ToString(),
        Offset = m_member.Offset,
        OrientationAngle = m_member.OrientationAngle,
        OrientationNode = m_member.OrientationNode,
        Property = m_member.Property,
        Type = m_member.Type,
        Type2D = m_member.Type2D
      };
      if (m_member.Topology != String.Empty)
        mem.Topology = m_member.Topology;

      if ((System.Drawing.Color)m_member.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        mem.Colour = m_member.Colour;

      m_member = mem;
    }
    #endregion

    #region fields
    private Member m_member;
    private int m_id = 0;

    private Brep m_brep; //brep for visualisation /member2d

    private PolyCurve m_edgeCrv; //Polyline for visualisation /member1d/member2d
    private List<Point3d> m_edgeCrv_topo; // list of topology points for visualisation /member1d/member2d
    private List<string> m_edgeCrv_topoType; //list of polyline curve type (arch or line) for member1d/2d

    private List<PolyCurve> m_voidCrvs; //converted edgecurve /member2d
    private List<List<Point3d>> m_voidCrvs_topo; //list of lists of void points /member2d
    private List<List<string>> m_voidCrvs_topoType; ////list of polyline curve type (arch or line) for void /member2d

    private List<PolyCurve> m_inclCrvs; //converted inclusion lines /member2d
    private List<List<Point3d>> m_inclCrvs_topo; //list of lists of line inclusion topology points /member2d
    private List<List<string>> m_inclCrvs_topoType; ////list of polyline curve type (arch or line) for inclusion /member2d

    private List<Point3d> m_inclPts; //list of points for inclusion /member2d

    private GsaProp2d m_prop;
    #endregion

    #region constructors
    public GsaMember2d()
    {
      m_member = new Member();
      m_prop = new GsaProp2d();
    }

    public GsaMember2d(Brep brep, List<Curve> includeCurves = null, List<Point3d> includePoints = null, int prop = 0)
    {
      m_member = new Member
      {
        Type = MemberType.GENERIC_2D,
        Property = prop
      };
      m_prop = new GsaProp2d();

      Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>>
          convertBrepInclusion = Util.GH.Convert.ConvertPolyBrepInclusion(brep, includeCurves, includePoints);

      Tuple<PolyCurve, List<Point3d>, List<string>> edgeTuple = convertBrepInclusion.Item1;
      Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>> voidTuple = convertBrepInclusion.Item2;
      Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>> inclTuple = convertBrepInclusion.Item3;

      m_edgeCrv = edgeTuple.Item1;
      m_edgeCrv_topo = edgeTuple.Item2;
      m_edgeCrv_topoType = edgeTuple.Item3;
      m_voidCrvs = voidTuple.Item1;
      m_voidCrvs_topo = voidTuple.Item2;
      m_voidCrvs_topoType = voidTuple.Item3;
      m_inclCrvs = inclTuple.Item1;
      m_inclCrvs_topo = inclTuple.Item2;
      m_inclCrvs_topoType = inclTuple.Item3;
      m_inclPts = inclTuple.Item4;

      m_brep = Util.GH.Convert.BuildBrep(m_edgeCrv, m_voidCrvs);
      if (m_brep == null)
        throw new Exception(" Error with Mem2D: Unable to build Brep, please verify input geometry is valid and tolerance is set accordingly with your geometry under GSA Plugin Unit Settings or if unset under Rhino unit settings");

    }

    internal GsaMember2d(Member member, int id, List<Point3d> topology,
        List<string> topology_type,
        List<List<Point3d>> void_topology,
        List<List<string>> void_topology_type,
        List<List<Point3d>> inlcusion_lines_topology,
        List<List<string>> inclusion_topology_type,
        List<Point3d> includePoints,
        GsaProp2d prop, GH_Component owner = null)
    {
      m_member = member;
      m_id = id;

      if (topology.Count == 0)
      {
        m_brep = null;
        m_edgeCrv = null;
        m_inclPts = null;
        return;
      }

      if (topology.Count < 3) // we need minimum 3 nodes to create a 2D member
      {
        m_brep = null;
        m_edgeCrv = null;
        m_inclPts = null;
        string error = " Invalid topology Mem2D ID: " + id + ".";
        if (owner == null)
        {
          throw new Exception(error);
        }
        else
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, error);
        return;
      }

      if (topology[0] != topology[topology.Count - 1])
      {
        topology.Add(topology[0]);
        topology_type.Add("");
      }

      m_edgeCrv = Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(topology, topology_type);
      m_edgeCrv_topo = topology;
      m_edgeCrv_topoType = topology_type;

      if (void_topology != null)
      {
        if (m_voidCrvs == null) { m_voidCrvs = new List<PolyCurve>(); }
        for (int i = 0; i < void_topology.Count; i++)
        {
          // void curves must be closed, check that topogylist is ending with start point
          if (void_topology[i][0] != void_topology[i][void_topology[i].Count - 1])
          {
            void_topology[i].Add(void_topology[i][0]);
            void_topology_type[i].Add("");
          }
          if (void_topology_type != null)
            m_voidCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(void_topology[i], void_topology_type[i]));
          else
            m_voidCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(void_topology[i]));
        }
      }
      m_voidCrvs_topo = void_topology;
      m_voidCrvs_topoType = void_topology_type;

      if (inlcusion_lines_topology != null)
      {
        if (m_inclCrvs == null) { m_inclCrvs = new List<PolyCurve>(); }
        for (int i = 0; i < inlcusion_lines_topology.Count; i++)
        {
          if (inclusion_topology_type != null)
            m_inclCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(inlcusion_lines_topology[i], inclusion_topology_type[i]));
          else
            m_inclCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(inlcusion_lines_topology[i]));
        }
      }
      m_inclCrvs_topo = inlcusion_lines_topology;
      m_inclCrvs_topoType = inclusion_topology_type;

      m_inclPts = includePoints;

      m_brep = Util.GH.Convert.BuildBrep(m_edgeCrv, m_voidCrvs);
      if (m_brep == null)
      {
        string error = " Error with Mem2D ID: " + id + ". Unable to build Brep, please verify input geometry is valid and tolerance is set to something reasonable." +
            System.Environment.NewLine + "It may be that the topology list is invalid, please check your input. Member " + id + " has not been imported!";
        if (owner == null)
        {
          throw new Exception(error);
        }
        else
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, error);
      }

      m_prop = prop;
    }
    public GsaMember2d Duplicate(bool cloneApiMember = false)
    {
      if (this == null) { return null; }
      GsaMember2d dup = new GsaMember2d();
      dup.m_id = m_id;
      dup.m_member = m_member;
      if (cloneApiMember)
        dup.CloneApiMember();
      dup.m_prop = m_prop.Duplicate();

      dup.m_brep = (Brep)m_brep.DuplicateShallow();

      dup.m_edgeCrv = (PolyCurve)m_edgeCrv.DuplicateShallow();
      dup.m_edgeCrv_topo = m_edgeCrv_topo;
      dup.m_edgeCrv_topoType = m_edgeCrv_topoType;

      List<PolyCurve> dupVoidCrvs = new List<PolyCurve>();
      for (int i = 0; i < m_voidCrvs.Count; i++)
        dupVoidCrvs.Add((PolyCurve)m_voidCrvs[i].DuplicateShallow());
      dup.m_voidCrvs = dupVoidCrvs;
      dup.m_voidCrvs_topo = m_voidCrvs_topo;
      dup.m_voidCrvs_topoType = m_voidCrvs_topoType;

      List<PolyCurve> dupInclCrvs = new List<PolyCurve>();
      for (int i = 0; i < m_inclCrvs.Count; i++)
        dupInclCrvs.Add((PolyCurve)m_inclCrvs[i].DuplicateShallow());
      dup.m_inclCrvs = dupInclCrvs;
      dup.m_inclCrvs_topo = m_inclCrvs_topo;
      dup.m_inclCrvs_topoType = m_inclCrvs_topoType;

      dup.m_inclPts = m_inclPts;

      return dup;
    }
    public GsaMember2d UpdateGeometry(Brep brep = null, List<Curve> inclCrvs = null, List<Point3d> inclPts = null)
    {
      if (this == null) { return null; }

      if (brep == null)
        brep = m_brep.DuplicateBrep();
      if (inclCrvs == null)
        inclCrvs = m_inclCrvs.Select(x => (Curve)x).ToList();
      if (inclPts == null)
        inclPts = m_inclPts.ToList();

      GsaMember2d dup = new GsaMember2d(brep, inclCrvs, inclPts);
      dup.m_id = m_id;
      dup.m_member = m_member;
      dup.m_prop = m_prop.Duplicate();

      return dup;
    }
    public GsaMember2d Transform(Transform xform)
    {
      if (this == null) { return null; }
      GsaMember2d dup = this.Duplicate(true);
      dup.ID = 0;

      // Brep
      if (dup.m_brep != null)
        dup.m_brep.Transform(xform);

      // Edge curve
      if (dup.m_edgeCrv != null)
        dup.m_edgeCrv.Transform(xform);
      dup.m_edgeCrv_topo = xform.TransformList(dup.m_edgeCrv_topo).ToList();

      // Void curves
      for (int i = 0; i < m_voidCrvs.Count; i++)
      {
        if (dup.m_voidCrvs[i] != null)
          dup.m_voidCrvs[i].Transform(xform);
        dup.m_voidCrvs_topo[i] = xform.TransformList(dup.m_voidCrvs_topo[i]).ToList();
      }

      // Incl. curves
      for (int i = 0; i < m_inclCrvs.Count; i++)
      {
        if (dup.m_inclCrvs[i] != null)
          dup.m_inclCrvs[i].Transform(xform);
        dup.m_inclCrvs_topo[i] = xform.TransformList(dup.m_inclCrvs_topo[i]).ToList();
      }

      // Incl. points
      dup.m_inclPts = xform.TransformList(dup.m_inclPts).ToList();

      return dup;
    }
    public GsaMember2d Morph(SpaceMorph xmorph)
    {
      if (this == null) { return null; }
      GsaMember2d dup = this.Duplicate(true);
      dup.ID = 0;

      // Brep
      if (dup.m_brep != null)
        xmorph.Morph(dup.m_brep);

      // Edge curve
      if (dup.m_edgeCrv != null)
        xmorph.Morph(m_edgeCrv);
      for (int i = 0; i < dup.m_edgeCrv_topo.Count; i++)
        dup.m_edgeCrv_topo[i] = xmorph.MorphPoint(dup.m_edgeCrv_topo[i]);

      // Void curves
      for (int i = 0; i < m_voidCrvs.Count; i++)
      {
        if (dup.m_voidCrvs[i] != null)
          xmorph.Morph(m_voidCrvs[i]);
        for (int j = 0; j < dup.m_voidCrvs_topo[i].Count; j++)
          dup.m_voidCrvs_topo[i][j] = xmorph.MorphPoint(dup.m_voidCrvs_topo[i][j]);
      }

      // Incl. curves
      for (int i = 0; i < m_inclCrvs.Count; i++)
      {
        if (dup.m_inclCrvs[i] != null)
          xmorph.Morph(m_inclCrvs[i]);
        for (int j = 0; j < dup.m_inclCrvs_topo[i].Count; j++)
          dup.m_inclCrvs_topo[i][j] = xmorph.MorphPoint(dup.m_inclCrvs_topo[i][j]);
      }

      // Incl. points
      for (int i = 0; i < dup.m_inclPts.Count; i++)
        dup.m_inclPts[i] = xmorph.MorphPoint(dup.m_inclPts[i]);

      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (m_brep == null | m_edgeCrv == null)
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
      typeTxt = typeTxt.Replace("2d", "2D");
      string incl = "";
      if (!(m_inclCrvs == null & m_inclPts == null))
        if (m_inclCrvs.Count > 0 | m_inclPts.Count > 0)
          incl = " {Contains ";
      if (m_inclCrvs != null)
      {
        if (m_inclCrvs.Count > 0)
          incl = incl + m_inclCrvs.Count + " inclusion line";
        if (m_inclCrvs.Count > 1)
          incl += "s";
      }

      if (m_inclCrvs != null & m_inclPts != null)
        if (m_inclCrvs.Count > 0 & m_inclPts.Count > 0)
          incl += " and ";

      if (m_inclPts != null)
      {
        if (m_inclPts.Count > 0)
          incl = incl + m_inclPts.Count + " inclusion point";
        if (m_inclPts.Count > 1)
          incl += "s";
      }
      if (incl != "")
        incl += "}";
      string valid = (this.IsValid) ? "" : "Invalid ";
      return valid + typeTxt + incl;
    }

    #endregion
  }

  /// <summary>
  /// GsaMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
  /// </summary>
  public class GsaMember2dGoo : GH_GeometricGoo<GsaMember2d>, IGH_PreviewData
  {
    #region constructors
    public GsaMember2dGoo()
    {
      this.Value = new GsaMember2d();
    }
    public GsaMember2dGoo(GsaMember2d member)
    {
      if (member == null)
        member = new GsaMember2d();
      this.Value = member; //member.Duplicate();
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return DuplicateGsaMember2d();
    }
    public GsaMember2dGoo DuplicateGsaMember2d()
    {
      return new GsaMember2dGoo(Value == null ? new GsaMember2d() : Value); //Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid
    {
      get
      {
        if (Value == null) { return false; }
        if (Value.Brep == null & Value.PolyCurve == null) { return false; }
        return true;
      }
    }
    public override string IsValidWhyNot
    {
      get
      {
        //if (Value == null) { return "No internal GsaMember instance"; }
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }
    public override string ToString()
    {
      if (Value == null)
        return "Null Member2D";
      else
        return Value.ToString();
    }
    public override string TypeName
    {
      get { return ("Member 2D"); }
    }
    public override string TypeDescription
    {
      get { return ("GSA 2D Member"); }
    }

    public override BoundingBox Boundingbox
    {
      get
      {
        if (Value == null) { return BoundingBox.Empty; }
        if (Value.Brep == null & Value.PolyCurve == null) { return BoundingBox.Empty; }
        if (Value.Brep != null) { return Value.Brep.GetBoundingBox(false); }
        return Value.PolyCurve.GetBoundingBox(false);
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      if (Value == null) { return BoundingBox.Empty; }
      if (Value.Brep == null & Value.PolyCurve == null) { return BoundingBox.Empty; }
      if (Value.Brep != null) { return Value.Brep.GetBoundingBox(xform); }
      return Value.PolyCurve.GetBoundingBox(xform);
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(out Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaMember into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(GsaMember2d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(Member)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.GetAPI_MemberClone();
        return true;
      }

      //Cast to Curve
      if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Curve(Value.PolyCurve.DuplicatePolyCurve());
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(PolyCurve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(Polyline)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(Line)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.PolyCurve.ToPolyline(GsaGH.Units.Tolerance.As(Units.LengthUnitGeometry), 2, 0, 0);
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }

      //Cast to Brep
      if (typeof(Q).IsAssignableFrom(typeof(Brep)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.Brep.DuplicateBrep();
          if (Value.Brep == null)
            return false;
        }

        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Brep)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Brep(Value.Brep.DuplicateBrep());
          if (Value.Brep == null)
            return false;
        }
        return true;
      }

      //Cast to Points
      if (typeof(Q).IsAssignableFrom(typeof(List<Point3d>)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Topology.ToList();
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(List<GH_Point>)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Topology.ToList();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Integer ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaMember.

      if (source == null) { return false; }

      //Cast from GsaMember
      if (typeof(GsaMember2d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaMember2d)source;
        return true;
      }

      //Cast from GsaAPI Member
      //if (typeof(Member).IsAssignableFrom(source.GetType()))
      //{
      //    Value.Member = (Member)source;
      //    return true;
      //}

      //Cast from Brep
      Brep brep = new Brep();
      if (GH_Convert.ToBrep(source, ref brep, GH_Conversion.Both))
      {
        List<Point3d> pts = new List<Point3d>();
        List<Curve> crvs = new List<Curve>();
        GsaMember2d mem = new GsaMember2d(brep, crvs, pts);
        //GsaProp2d prop2d = new GsaProp2d();
        //prop2d.ID = 1;
        //mem.Property = prop2d;
        this.Value = mem;
        return true;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      return new GsaMember2dGoo(Value.Transform(xform));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      return new GsaMember2dGoo(Value.Morph(xmorph));
    }

    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      if (Value.Brep != null)
      {
        if (Value.Type == MemberType.VOID_CUTTER_2D)
        {
          if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            args.Pipeline.DrawBrepShaded(Value.Brep, UI.Colour.Member2dVoidCutterFace); //UI.Colour.Member2dFace
        }
        else
        {
          if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            args.Pipeline.DrawBrepShaded(Value.Brep, UI.Colour.Member2dFace); //UI.Colour.Member2dFace
          else
            args.Pipeline.DrawBrepShaded(Value.Brep, UI.Colour.Member2dFaceSelected);
        }
      }
    }
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      // Draw shape edge
      if (Value.Brep != null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if (Value.Type == MemberType.VOID_CUTTER_2D)
          {
            args.Pipeline.DrawBrepWires(Value.Brep, UI.Colour.VoidCutter, -1);
          }
          else if (!Value.IsDummy)
            args.Pipeline.DrawBrepWires(Value.Brep, UI.Colour.Member2dEdge, -1);
        }
        else
          args.Pipeline.DrawBrepWires(Value.Brep, UI.Colour.Member2dEdgeSelected, -1);
      }

      //Draw lines
      if (Value.PolyCurve != null & Value.Brep == null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedPolyline(Value.Topology, UI.Colour.Dummy1D, false);
          else
          {
            if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
              args.Pipeline.DrawCurve(Value.PolyCurve, Value.Colour, 2);
            else
            {
              System.Drawing.Color col = UI.Colour.Member2dEdge;
              args.Pipeline.DrawCurve(Value.PolyCurve, col, 2);
            }
          }
        }
        else
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedPolyline(Value.Topology, UI.Colour.Member1dSelected, false);
          else
            args.Pipeline.DrawCurve(Value.PolyCurve, UI.Colour.Member1dSelected, 2);
        }
      }

      if (Value.InclusionLines != null)
      {
        for (int i = 0; i < Value.InclusionLines.Count; i++)
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedPolyline(Value.IncLinesTopology[i], UI.Colour.Member1dSelected, false);
          else
            args.Pipeline.DrawCurve(Value.InclusionLines[i], UI.Colour.Member2dInclLn, 2);
        }
      }

      //Draw points.
      if (Value.Topology != null)
      {
        List<Point3d> pts = Value.Topology;
        for (int i = 0; i < pts.Count; i++)
        {
          if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
          {
            if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 3, (Value.IsDummy) ? UI.Colour.Dummy1D : UI.Colour.Member1dNode);
            else
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 2, (Value.IsDummy) ? UI.Colour.Dummy1D : UI.Colour.Member1dNode);
          }
          else
          {
            if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Member1dNodeSelected);
            else
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 2, UI.Colour.Member1dNodeSelected);
          }
        }
      }
      if (Value.InclusionPoints != null)
      {
        for (int i = 0; i < Value.InclusionPoints.Count; i++)
          args.Pipeline.DrawPoint(Value.InclusionPoints[i], Rhino.Display.PointStyle.RoundSimple, 3, (Value.IsDummy) ? UI.Colour.Dummy1D : UI.Colour.Member2dInclPt);
      }
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the Data_GsaMember2d type.
  /// </summary>
  public class GsaMember2dParameter : GH_PersistentGeometryParam<GsaMember2dGoo>, IGH_PreviewObject
  {
    public GsaMember2dParameter()
      : base(new GH_InstanceDescription("2D Member", "M2D", "Maintains a collection of GSA 2D Member data.", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    {
    }

    public override Guid ComponentGuid => new Guid("fa512c2d-4767-49f1-a574-32bf66a66568");

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Mem2dParam;

    //We do not allow users to pick parameter, 
    //therefore the following 4 methods disable all this ui.
    protected override GH_GetterResult Prompt_Plural(ref List<GsaMember2dGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref GsaMember2dGoo value)
    {
      return GH_GetterResult.cancel;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    #region preview methods
    public BoundingBox ClippingBox
    {
      get
      {
        return Preview_ComputeClippingBox();
      }
    }
    public void DrawViewportMeshes(IGH_PreviewArgs args)
    {
      //Use a standard method to draw gunk, you don't have to specifically implement this.
      Preview_DrawMeshes(args);
    }
    public void DrawViewportWires(IGH_PreviewArgs args)
    {
      //Use a standard method to draw gunk, you don't have to specifically implement this.
      Preview_DrawWires(args);
    }

    private bool m_hidden = false;
    public bool Hidden
    {
      get { return m_hidden; }
      set { m_hidden = value; }
    }
    public bool IsPreviewCapable
    {
      get { return true; }
    }
    #endregion
  }
}
