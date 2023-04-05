using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Member2d class, this class defines the basic properties and methods for any Gsa Member 2d
  /// </summary>
  public class GsaMember2d {
    #region fields

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

    private Guid _guid = Guid.NewGuid();
    private int _id = 0;

    #endregion

    #region properties

    public int Id {
      get => _id;
      set {
        CloneApiObject();
        _id = value;
      }
    }

    internal Member ApiMember { get; set; } = new Member() {
      Type = MemberType.GENERIC_2D,
    };

    // mesh size in Rhino/Grasshopper world, might be different to internal GSA mesh size
    public double MeshSize { get; set; } = 0;
    public GsaProp2d Property { get; set; } = new GsaProp2d();
    public PolyCurve PolyCurve => _edgeCrv;
    public Brep Brep => _brep;
    public List<Point3d> Topology => _edgeCrvTopo;
    public List<string> TopologyType => _edgeCrvTopoType;
    public List<List<Point3d>> VoidTopology => _voidCrvsTopo;
    public List<List<string>> VoidTopologyType => _voidCrvsTopoType;
    public List<PolyCurve> InclusionLines => _inclCrvs ?? new List<PolyCurve>();
    public List<List<Point3d>> IncLinesTopology => _inclCrvsTopo;
    public List<List<string>> IncLinesTopologyType => _inclCrvsTopoType;
    public List<Point3d> InclusionPoints => _inclPts;
    public Guid Guid => _guid;

    #region GsaAPI.Member members

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

    public bool IsDummy {
      get => ApiMember.IsDummy;
      set {
        CloneApiObject();
        ApiMember.IsDummy = value;
      }
    }

    public string Name {
      get => ApiMember.Name;
      set {
        CloneApiObject();
        ApiMember.Name = value;
      }
    }

    public bool MeshWithOthers {
      get => ApiMember.IsIntersector;
      set {
        CloneApiObject();
        ApiMember.IsIntersector = value;
      }
    }

    public GsaOffset Offset {
      get
        => new GsaOffset(ApiMember.Offset.X1,
          ApiMember.Offset.X2,
          ApiMember.Offset.Y,
          ApiMember.Offset.Z);
      set {
        CloneApiObject();
        ApiMember.Offset.X1 = value.X1.Meters;
        ApiMember.Offset.X2 = value.X2.Meters;
        ApiMember.Offset.Y = value.Y.Meters;
        ApiMember.Offset.Z = value.Z.Meters;
      }
    }

    public bool AutomaticInternalOffset {
      get => ApiMember.AutomaticOffset.Internal;
      set {
        CloneApiObject();
        ApiMember.AutomaticOffset.Internal = value;
      }
    }

    public Angle OrientationAngle {
      get => new Angle(ApiMember.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      set {
        CloneApiObject();
        ApiMember.OrientationAngle = value.Degrees;
      }
    }

    public int OrientationNode {
      get => ApiMember.OrientationNode;
      set {
        CloneApiObject();
        ApiMember.OrientationNode = value;
      }
    }

    public MemberType Type {
      get => ApiMember.Type;
      set {
        CloneApiObject();
        ApiMember.Type = value;
      }
    }

    public AnalysisOrder Type2D {
      get => ApiMember.Type2D;
      set {
        CloneApiObject();
        ApiMember.Type2D = value;
      }
    }

    internal void CloneApiObject() {
      ApiMember = GetAPI_MemberClone();
      _guid = Guid.NewGuid();
    }

    #endregion

    #endregion

    #region constructors

    public GsaMember2d() { }

    public GsaMember2d(
      Brep brep,
      List<Curve> includeCurves = null,
      List<Point3d> includePoints = null,
      int prop = 0) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_2D,
        Property = prop,
      };

      (Tuple<PolyCurve,
            List<Point3d>,
            List<string>> edgeTuple,
            Tuple<List<PolyCurve>,
            List<List<Point3d>>,
            List<List<string>>> voidTuple, Tuple<List<PolyCurve>,
            List<List<Point3d>>,
            List<List<string>>,
            List<Point3d>> inclTuple)
        = RhinoConversions.ConvertPolyBrepInclusion(brep, includeCurves, includePoints);

      _edgeCrv = edgeTuple.Item1;
      _edgeCrvTopo = edgeTuple.Item2;
      _edgeCrvTopoType = edgeTuple.Item3;
      _voidCrvs = voidTuple.Item1;
      _voidCrvsTopo = voidTuple.Item2;
      _voidCrvsTopoType = voidTuple.Item3;
      _inclCrvs = inclTuple.Item1;
      _inclCrvsTopo = inclTuple.Item2;
      _inclCrvsTopoType = inclTuple.Item3;
      _inclPts = inclTuple.Item4;

      _brep = RhinoConversions.BuildBrep(_edgeCrv, _voidCrvs);
      if (_brep == null)
        throw new Exception(
          " Error with Mem2D: Unable to build Brep, "
          + "please verify input geometry is valid and tolerance "
          + "is set accordingly with your geometry under GSA Plugin Unit "
          + "Settings or if unset under Rhino unit settings");
    }

    internal GsaMember2d(
      Member member,
      int id,
      List<Point3d> topology,
      List<string> topologyType,
      List<List<Point3d>> voidTopology,
      List<List<string>> voidTopologyType,
      List<List<Point3d>> inlcusionLinesTopology,
      List<List<string>> inclusionTopologyType,
      List<Point3d> includePoints,
      IReadOnlyDictionary<int, Prop2D> properties,
      IReadOnlyDictionary<int, AnalysisMaterial> materials,
      IReadOnlyDictionary<int, Axis> axDict,
      LengthUnit modelUnit) {
      ApiMember = member;
      MeshSize = new Length(member.MeshSize, LengthUnit.Meter).As(modelUnit);
      _id = id;

      if (topology[0] != topology[topology.Count - 1]) // add last point to close boundary
      {
        topology.Add(topology[0]);
        topologyType.Add("");
      }

      _edgeCrv = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topology, topologyType);
      _edgeCrvTopo = topology;
      _edgeCrvTopoType = topologyType;

      if (voidTopology != null) {
        if (_voidCrvs == null)
          _voidCrvs = new List<PolyCurve>();
        for (int i = 0; i < voidTopology.Count; i++) {
          if (voidTopology[i][0] != voidTopology[i][voidTopology[i].Count - 1]) {
            voidTopology[i].Add(voidTopology[i][0]);
            voidTopologyType[i].Add("");
          }

          if (voidTopologyType != null)
            _voidCrvs.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i],
                voidTopologyType[i]));
          else
            _voidCrvs.Add(RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i]));
        }
      }

      _voidCrvsTopo = voidTopology;
      _voidCrvsTopoType = voidTopologyType;

      if (inlcusionLinesTopology != null) {
        if (_inclCrvs == null)
          _inclCrvs = new List<PolyCurve>();
        for (int i = 0; i < inlcusionLinesTopology.Count; i++)
          if (inclusionTopologyType != null)
            _inclCrvs.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i],
                inclusionTopologyType[i]));
          else
            _inclCrvs.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i]));
      }

      _inclCrvsTopo = inlcusionLinesTopology;
      _inclCrvsTopoType = inclusionTopologyType;
      _inclPts = includePoints;

      _brep = RhinoConversions.BuildBrep(_edgeCrv,
        _voidCrvs,
        new Length(0.001, LengthUnit.Meter).As(DefaultUnits.LengthUnitGeometry));

      Property = new GsaProp2d(properties, ApiMember.Property, materials, axDict, modelUnit);
    }

    #endregion

    #region methods

    public GsaMember2d Duplicate(bool cloneApiMember = false) {
      var dup = new GsaMember2d {
        Id = Id,
        MeshSize = MeshSize,
        _guid = new Guid(_guid.ToString()),
        ApiMember = ApiMember,
      };
      if (cloneApiMember)
        dup.CloneApiObject();
      dup.Property = Property.Duplicate();

      if (_brep == null)
        return dup;

      dup._brep = (Brep)_brep.DuplicateShallow();

      dup._edgeCrv = (PolyCurve)_edgeCrv.DuplicateShallow();
      dup._edgeCrvTopo = _edgeCrvTopo;
      dup._edgeCrvTopoType = _edgeCrvTopoType;

      var dupVoidCrvs = _voidCrvs.Select(t => (PolyCurve)t.DuplicateShallow())
        .ToList();
      dup._voidCrvs = dupVoidCrvs;
      dup._voidCrvsTopo = _voidCrvsTopo;
      dup._voidCrvsTopoType = _voidCrvsTopoType;

      var dupInclCrvs = _inclCrvs.Select(t => (PolyCurve)t.DuplicateShallow())
        .ToList();
      dup._inclCrvs = dupInclCrvs;
      dup._inclCrvsTopo = _inclCrvsTopo;
      dup._inclCrvsTopoType = _inclCrvsTopoType;

      dup._inclPts = _inclPts;

      return dup;
    }

    internal Member GetAPI_MemberClone() {
      var mem = new Member {
        Group = ApiMember.Group,
        IsDummy = ApiMember.IsDummy,
        MeshSize = ApiMember.MeshSize,
        Name = ApiMember.Name.ToString(),
        Offset = ApiMember.Offset,
        OrientationAngle = ApiMember.OrientationAngle,
        OrientationNode = ApiMember.OrientationNode,
        Property = ApiMember.Property,
        Type = ApiMember.Type,
        Type2D = ApiMember.Type2D,
        AutomaticOffset = ApiMember.AutomaticOffset,
      };
      if (ApiMember.Topology != string.Empty)
        mem.Topology = ApiMember.Topology;

      if ((Color)ApiMember.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        mem.Colour = ApiMember.Colour;

      return mem;
    }

    public GsaMember2d UpdateGeometry(
      Brep brep = null,
      List<Curve> inclCrvs = null,
      List<Point3d> inclPts = null) {
      if (brep == null && _brep != null)
        brep = _brep.DuplicateBrep();
      if (inclCrvs == null && _inclCrvs != null)
        inclCrvs = _inclCrvs.Select(x => (Curve)x)
          .ToList();
      if (inclPts == null && _inclPts != null)
        inclPts = _inclPts.ToList();

      var dup = new GsaMember2d(brep, inclCrvs, inclPts) {
        Id = Id,
        ApiMember = ApiMember,
        Property = Property.Duplicate(),
      };

      return dup;
    }

    public GsaMember2d Transform(Transform xform) {
      GsaMember2d dup = Duplicate(true);
      dup.Id = 0;

      dup._brep?.Transform(xform);
      dup._edgeCrv?.Transform(xform);
      dup._edgeCrvTopo = xform.TransformList(dup._edgeCrvTopo)
        .ToList();
      for (int i = 0; i < _voidCrvs.Count; i++) {
        dup._voidCrvs[i]
          ?.Transform(xform);
        dup._voidCrvsTopo[i] = xform.TransformList(dup._voidCrvsTopo[i])
          .ToList();
      }

      for (int i = 0; i < _inclCrvs.Count; i++) {
        dup._inclCrvs[i]
          ?.Transform(xform);
        dup._inclCrvsTopo[i] = xform.TransformList(dup._inclCrvsTopo[i])
          .ToList();
      }

      dup._inclPts = xform.TransformList(dup._inclPts)
        .ToList();

      return dup;
    }

    public GsaMember2d Morph(SpaceMorph xmorph) {
      GsaMember2d dup = Duplicate(true);
      dup.Id = 0;

      if (dup._brep != null)
        xmorph.Morph(dup._brep);

      if (dup._edgeCrv != null)
        xmorph.Morph(_edgeCrv);
      for (int i = 0; i < dup._edgeCrvTopo.Count; i++)
        dup._edgeCrvTopo[i] = xmorph.MorphPoint(dup._edgeCrvTopo[i]);

      for (int i = 0; i < _voidCrvs.Count; i++) {
        if (dup._voidCrvs[i] != null)
          xmorph.Morph(_voidCrvs[i]);
        for (int j = 0; j < dup._voidCrvsTopo[i].Count; j++)
          dup._voidCrvsTopo[i][j] = xmorph.MorphPoint(dup._voidCrvsTopo[i][j]);
      }

      for (int i = 0; i < _inclCrvs.Count; i++) {
        if (dup._inclCrvs[i] != null)
          xmorph.Morph(_inclCrvs[i]);
        for (int j = 0; j < dup._inclCrvsTopo[i].Count; j++)
          dup._inclCrvsTopo[i][j] = xmorph.MorphPoint(dup._inclCrvsTopo[i][j]);
      }

      for (int i = 0; i < dup._inclPts.Count; i++)
        dup._inclPts[i] = xmorph.MorphPoint(dup._inclPts[i]);

      return dup;
    }

    public override string ToString() {
      string incl = "";
      if (_inclCrvs != null)
        if (_inclCrvs.Count > 0)
          incl = " Incl.Crv:" + _inclCrvs.Count;

      if (_inclPts != null) {
        if (_inclPts != null && _inclPts.Count > 0)
          incl += " &";
        if (_inclPts.Count > 0)
          incl += " Incl.Pt:" + _inclPts.Count;
      }

      string idd = Id == 0
        ? ""
        : "ID:" + Id + " ";
      string type = Mappings.s_memberTypeMapping.FirstOrDefault(x => x.Value == Type)
          .Key
        + " ";
      return string.Join(" ", idd.Trim(), type.Trim(), incl.Trim())
        .Trim()
        .Replace("  ", " ");
    }

    #endregion
  }
}
