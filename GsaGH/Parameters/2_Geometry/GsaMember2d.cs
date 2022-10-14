using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Member2d class, this class defines the basic properties and methods for any Gsa Member 2d
  /// </summary>
  public class GsaMember2d
  {
    #region fields
    private int _id = 0;
    private Member _member = new Member();
    private GsaProp2d _prop = new GsaProp2d();

    private Brep _brep; // brep for visualisation /member2d

    private PolyCurve _edgeCrv; // Polyline for visualisation /member1d/member2d
    private List<Point3d> _edgeCrvTopo; // list of topology points for visualisation /member1d/member2d
    private List<string> _edgeCrvTopoType; // list of polyline curve type (arch or line) for member1d/2d

    private List<PolyCurve> _voidCrvs; //converted edgecurve /member2d
    private List<List<Point3d>> _voidCrvsTopo; // list of lists of void points /member2d
    private List<List<string>> _voidCrvsTopoType; // list of polyline curve type (arch or line) for void /member2d

    private List<PolyCurve> _inclCrvs; // converted inclusion lines /member2d
    private List<List<Point3d>> _inclCrvsTopo; // list of lists of line inclusion topology points /member2d
    private List<List<string>> _inclCrvsTopoType; // list of polyline curve type (arch or line) for inclusion /member2d

    private List<Point3d> _inclPts; //slist of points for inclusion /member2d
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

    public PolyCurve PolyCurve
    {
      get
      {
        return this._edgeCrv;
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
    public Brep Brep
    {
      get
      {
        return this._brep;
      }
    }
    public List<Point3d> Topology
    {
      get
      {
        return this._edgeCrvTopo;
      }
    }
    public List<string> TopologyType
    {
      get
      {
        return this._edgeCrvTopoType;
      }
    }
    public List<List<Point3d>> VoidTopology
    {
      get
      {
        return this._voidCrvsTopo;
      }
    }
    public List<List<string>> VoidTopologyType
    {
      get
      {
        return this._voidCrvsTopoType;
      }
    }
    public List<PolyCurve> InclusionLines
    {
      get
      {
        return this._inclCrvs;
      }
    }
    public List<List<Point3d>> IncLinesTopology
    {
      get
      {
        return this._inclCrvsTopo;
      }
    }
    public List<List<string>> IncLinesTopologyType
    {
      get
      {
        return this._inclCrvsTopoType;
      }
    }
    public List<Point3d> InclusionPoints
    {
      get
      {
        return this._inclPts;
      }
    }
    public GsaProp2d Property
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
        this.CloneApiMember();
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
        this.CloneApiMember();
        this._member.Group = value;
      }
    }
    public bool IsDummy
    {
      get
      {
        return _member.IsDummy;
      }
      set
      {
        this.CloneApiMember();
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
        this.CloneApiMember();
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
        this.CloneApiMember();
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
        this.CloneApiMember();
        this._member.IsIntersector = value;
      }
    }
    public GsaOffset Offset
    {
      get
      {
        return new GsaOffset(this._member.Offset.X1, this._member.Offset.X2, this._member.Offset.Y, this._member.Offset.Z);
      }
      set
      {
        this.CloneApiMember();
        this._member.Offset.X1 = value.X1.Meters;
        this._member.Offset.X2 = value.X2.Meters;
        this._member.Offset.Y = value.Y.Meters;
        this._member.Offset.Z = value.Z.Meters;
      }
    }
    public double OrientationAngle
    {
      get
      {
        return this._member.OrientationAngle;
      }
      set
      {
        this.CloneApiMember();
        this._member.OrientationAngle = value;
      }
    }
    public int OrientationNode
    {
      get
      {
        return this._member.OrientationNode;
      }
      set
      {
        this.CloneApiMember();
        this._member.OrientationNode = value;
      }
    }

    public MemberType Type
    {
      get
      {
        return this._member.Type;
      }
      set
      {
        this.CloneApiMember();
        this._member.Type = value;
      }
    }
    public AnalysisOrder Type2D
    {
      get
      {
        return this._member.Type2D;
      }
      set
      {
        this.CloneApiMember();
        this._member.Type2D = value;
      }
    }

    internal void CloneApiMember()
    {
      this._member = this.GetAPI_MemberClone();
    }
    #endregion
    public bool IsValid
    {
      get
      {
        if (this._brep == null | this._edgeCrv == null)
          return false;
        return true;
      }
    }
    #endregion

    #region constructors
    public GsaMember2d()
    {
    }

    public GsaMember2d(Brep brep, List<Curve> includeCurves = null, List<Point3d> includePoints = null, int prop = 0)
    {
      this._member = new Member
      {
        Type = MemberType.GENERIC_2D,
        Property = prop
      };

      Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>> convertBrepInclusion = Util.GH.Convert.ConvertPolyBrepInclusion(brep, includeCurves, includePoints);

      Tuple<PolyCurve, List<Point3d>, List<string>> edgeTuple = convertBrepInclusion.Item1;
      Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>> voidTuple = convertBrepInclusion.Item2;
      Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>> inclTuple = convertBrepInclusion.Item3;

      this._edgeCrv = edgeTuple.Item1;
      this._edgeCrvTopo = edgeTuple.Item2;
      this._edgeCrvTopoType = edgeTuple.Item3;
      this._voidCrvs = voidTuple.Item1;
      this._voidCrvsTopo = voidTuple.Item2;
      this._voidCrvsTopoType = voidTuple.Item3;
      this._inclCrvs = inclTuple.Item1;
      this._inclCrvsTopo = inclTuple.Item2;
      this._inclCrvsTopoType = inclTuple.Item3;
      this._inclPts = inclTuple.Item4;

      this._brep = Util.GH.Convert.BuildBrep(_edgeCrv, _voidCrvs);
      if (this._brep == null)
        throw new Exception(" Error with Mem2D: Unable to build Brep, please verify input geometry is valid and tolerance is set accordingly with your geometry under GSA Plugin Unit Settings or if unset under Rhino unit settings");
    }

    internal GsaMember2d(Member member, int id, List<Point3d> topology, List<string> topologyType, List<List<Point3d>> voidTopology, List<List<string>> voidTopologyType, List<List<Point3d>> inlcusionLinesTopology, List<List<string>> inclusionTopologyType, List<Point3d> includePoints, GsaProp2d prop, GH_Component owner = null)
    {
      this._member = member;
      this._id = id;

      if (topology.Count == 0)
      {
        this._brep = null;
        this._edgeCrv = null;
        this._inclPts = null;
        return;
      }

      if (topology.Count < 3) // we need minimum 3 nodes to create a 2D member
      {
        this._brep = null;
        this._edgeCrv = null;
        this._inclPts = null;
        string error = "Invalid topology Mem2D ID: " + id + ".";
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
        topologyType.Add("");
      }

      this._edgeCrv = Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(topology, topologyType);
      this._edgeCrvTopo = topology;
      this._edgeCrvTopoType = topologyType;

      if (voidTopology != null)
      {
        if (this._voidCrvs == null)
        {
          this._voidCrvs = new List<PolyCurve>();
        }
        for (int i = 0; i < voidTopology.Count; i++)
        {
          // void curves must be closed, check that topogylist is ending with start point
          if (voidTopology[i][0] != voidTopology[i][voidTopology[i].Count - 1])
          {
            voidTopology[i].Add(voidTopology[i][0]);
            voidTopologyType[i].Add("");
          }
          if (voidTopologyType != null)
            this._voidCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i], voidTopologyType[i]));
          else
            this._voidCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i]));
        }
      }
      this._voidCrvsTopo = voidTopology;
      this._voidCrvsTopoType = voidTopologyType;

      if (inlcusionLinesTopology != null)
      {
        if (this._inclCrvs == null)
        {
          this._inclCrvs = new List<PolyCurve>();
        }
        for (int i = 0; i < inlcusionLinesTopology.Count; i++)
        {
          if (inclusionTopologyType != null)
            this._inclCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i], inclusionTopologyType[i]));
          else
            this._inclCrvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i]));
        }
      }
      this._inclCrvsTopo = inlcusionLinesTopology;
      this._inclCrvsTopoType = inclusionTopologyType;

      this._inclPts = includePoints;

      this._brep = Util.GH.Convert.BuildBrep(this._edgeCrv, this._voidCrvs, new Length(0.25, LengthUnit.Meter).As(DefaultUnits.LengthUnitGeometry)); // use relative high tolerance as if the member worked in GSA we want to import it even if warped
      //if (m_brep == null)
      //{
      //  string error = " Error with Mem2D ID: " + id + ". Unable to build Brep, please verify input geometry is valid and tolerance is set to something reasonable." +
      //      System.Environment.NewLine + "It may be that the topology list is invalid, please check your input. Member " + id + " has not been imported!";
      //  if (owner == null)
      //  {
      //    throw new Exception(error);
      //  }
      //  else
      //    owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, error);
      //}

      this._prop = prop;
    }
    #endregion

    #region methods
    public GsaMember2d Duplicate(bool cloneApiMember = false)
    {
      GsaMember2d dup = new GsaMember2d();
      dup._id = this._id;
      dup._member = this._member;
      if (cloneApiMember)
        dup.CloneApiMember();
      dup._prop = this._prop.Duplicate();

      if (this._brep == null) 
        return dup;

      dup._brep = (Brep)this._brep.DuplicateShallow();

      dup._edgeCrv = (PolyCurve)this._edgeCrv.DuplicateShallow();
      dup._edgeCrvTopo = this._edgeCrvTopo;
      dup._edgeCrvTopoType = this._edgeCrvTopoType;

      List<PolyCurve> dupVoidCrvs = new List<PolyCurve>();
      for (int i = 0; i < this._voidCrvs.Count; i++)
        dupVoidCrvs.Add((PolyCurve)this._voidCrvs[i].DuplicateShallow());
      dup._voidCrvs = dupVoidCrvs;
      dup._voidCrvsTopo = this._voidCrvsTopo;
      dup._voidCrvsTopoType = this._voidCrvsTopoType;

      List<PolyCurve> dupInclCrvs = new List<PolyCurve>();
      for (int i = 0; i < this._inclCrvs.Count; i++)
        dupInclCrvs.Add((PolyCurve)this._inclCrvs[i].DuplicateShallow());
      dup._inclCrvs = dupInclCrvs;
      dup._inclCrvsTopo = this._inclCrvsTopo;
      dup._inclCrvsTopoType = this._inclCrvsTopoType;

      dup._inclPts = this._inclPts;

      return dup;
    }

    internal Member GetAPI_MemberClone()
    {
      Member mem = new Member
      {
        Group = _member.Group,
        IsDummy = _member.IsDummy,
        MeshSize = _member.MeshSize,
        Name = _member.Name.ToString(),
        Offset = _member.Offset,
        OrientationAngle = _member.OrientationAngle,
        OrientationNode = _member.OrientationNode,
        Property = _member.Property,
        Type = _member.Type,
        Type2D = _member.Type2D
      };
      if (this._member.Topology != String.Empty)
        mem.Topology = this._member.Topology;

      if ((Color)_member.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        mem.Colour = this._member.Colour;

      return mem;
    }

    public GsaMember2d UpdateGeometry(Brep brep = null, List<Curve> inclCrvs = null, List<Point3d> inclPts = null)
    {
      if (brep == null)
        brep = this._brep.DuplicateBrep();
      if (inclCrvs == null)
        inclCrvs = this._inclCrvs.Select(x => (Curve)x).ToList();
      if (inclPts == null)
        inclPts = this._inclPts.ToList();

      GsaMember2d dup = new GsaMember2d(brep, inclCrvs, inclPts);
      dup._id = this._id;
      dup._member = this._member;
      dup._prop = this._prop.Duplicate();

      return dup;
    }

    public GsaMember2d Transform(Transform xform)
    {
      GsaMember2d dup = this.Duplicate(true);
      dup.Id = 0;

      // Brep
      if (dup._brep != null)
        dup._brep.Transform(xform);

      // Edge curve
      if (dup._edgeCrv != null)
        dup._edgeCrv.Transform(xform);
      dup._edgeCrvTopo = xform.TransformList(dup._edgeCrvTopo).ToList();

      // Void curves
      for (int i = 0; i < _voidCrvs.Count; i++)
      {
        if (dup._voidCrvs[i] != null)
          dup._voidCrvs[i].Transform(xform);
        dup._voidCrvsTopo[i] = xform.TransformList(dup._voidCrvsTopo[i]).ToList();
      }

      // Incl. curves
      for (int i = 0; i < _inclCrvs.Count; i++)
      {
        if (dup._inclCrvs[i] != null)
          dup._inclCrvs[i].Transform(xform);
        dup._inclCrvsTopo[i] = xform.TransformList(dup._inclCrvsTopo[i]).ToList();
      }

      // Incl. points
      dup._inclPts = xform.TransformList(dup._inclPts).ToList();

      return dup;
    }
    public GsaMember2d Morph(SpaceMorph xmorph)
    {
      GsaMember2d dup = this.Duplicate(true);
      dup.Id = 0;

      // Brep
      if (dup._brep != null)
        xmorph.Morph(dup._brep);

      // Edge curve
      if (dup._edgeCrv != null)
        xmorph.Morph(_edgeCrv);
      for (int i = 0; i < dup._edgeCrvTopo.Count; i++)
        dup._edgeCrvTopo[i] = xmorph.MorphPoint(dup._edgeCrvTopo[i]);

      // Void curves
      for (int i = 0; i < _voidCrvs.Count; i++)
      {
        if (dup._voidCrvs[i] != null)
          xmorph.Morph(_voidCrvs[i]);
        for (int j = 0; j < dup._voidCrvsTopo[i].Count; j++)
          dup._voidCrvsTopo[i][j] = xmorph.MorphPoint(dup._voidCrvsTopo[i][j]);
      }

      // Incl. curves
      for (int i = 0; i < _inclCrvs.Count; i++)
      {
        if (dup._inclCrvs[i] != null)
          xmorph.Morph(_inclCrvs[i]);
        for (int j = 0; j < dup._inclCrvsTopo[i].Count; j++)
          dup._inclCrvsTopo[i][j] = xmorph.MorphPoint(dup._inclCrvsTopo[i][j]);
      }

      // Incl. points
      for (int i = 0; i < dup._inclPts.Count; i++)
        dup._inclPts[i] = xmorph.MorphPoint(dup._inclPts[i]);

      return dup;
    }

    public override string ToString()
    {
      string idd = " " + Id.ToString();
      if (this.Id == 0)
      {
        idd = "";
      }
      string mes = _member.Type.ToString();
      string typeTxt = "GSA " + Char.ToUpper(mes[0]) + mes.Substring(1).ToLower().Replace("_", " ") + " Member" + idd;
      typeTxt = typeTxt.Replace("2d", "2D");
      string incl = "";
      if (!(this._inclCrvs == null & this._inclPts == null))
        if (this._inclCrvs.Count > 0 | this._inclPts.Count > 0)
          incl = " {Contains ";
      if (this._inclCrvs != null)
      {
        if (this._inclCrvs.Count > 0)
          incl = incl + this._inclCrvs.Count + " inclusion line";
        if (this._inclCrvs.Count > 1)
          incl += "s";
      }

      if (this._inclCrvs != null & this._inclPts != null)
        if (this._inclCrvs.Count > 0 & this._inclPts.Count > 0)
          incl += " and ";

      if (this._inclPts != null)
      {
        if (this._inclPts.Count > 0)
          incl = incl + this._inclPts.Count + " inclusion point";
        if (this._inclPts.Count > 1)
          incl += "s";
      }
      if (incl != "")
        incl += "}";
      string valid = (this.IsValid) ? "" : "Invalid ";
      return valid + typeTxt + incl;
    }
    #endregion
  }
}
