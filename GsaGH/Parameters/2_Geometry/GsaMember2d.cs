using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using OasysGH.Units;
using OasysUnits;
using Rhino.Geometry;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Member2d class, this class defines the basic properties and methods for any Gsa Member 2d
  /// </summary>
  public class GsaMember2d {
    public bool AutomaticInternalOffset {
      get => ApiMember.AutomaticOffset.Internal;
      set {
        CloneApiObject();
        ApiMember.AutomaticOffset.Internal = value;
      }
    }
    public Brep Brep { get; private set; }
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
    public List<List<Point3d>> IncLinesTopology { get; private set; }
    public List<List<string>> IncLinesTopologyType { get; private set; }
    public List<PolyCurve> InclusionLines => _inclCrvs ?? new List<PolyCurve>();
    public List<Point3d> InclusionPoints { get; private set; }
    public bool IsDummy {
      get => ApiMember.IsDummy;
      set {
        CloneApiObject();
        ApiMember.IsDummy = value;
      }
    }
    // mesh size in Rhino/Grasshopper world, might be different to internal GSA mesh size
    public double MeshSize { get; set; } = 0;
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
    public GsaOffset Offset {
      get
        => new GsaOffset(ApiMember.Offset.X1, ApiMember.Offset.X2, ApiMember.Offset.Y,
          ApiMember.Offset.Z);
      set {
        CloneApiObject();
        ApiMember.Offset.X1 = value.X1.Meters;
        ApiMember.Offset.X2 = value.X2.Meters;
        ApiMember.Offset.Y = value.Y.Meters;
        ApiMember.Offset.Z = value.Z.Meters;
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
    public PolyCurve PolyCurve { get; private set; }
    public GsaProp2d Prop2d { get; set; } = new GsaProp2d();
    public List<Point3d> Topology { get; private set; }
    public List<string> TopologyType { get; private set; }
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
    public List<List<Point3d>> VoidTopology { get; private set; }
    public List<List<string>> VoidTopologyType { get; private set; }
    internal Member ApiMember { get; set; } = new Member() {
      Type = MemberType.GENERIC_2D,
    };

    // list of polyline curve type (arch or line) for member1d/2d
    private Guid _guid = Guid.NewGuid();
    private int _id = 0;
    private List<PolyCurve> _inclCrvs;
    private List<PolyCurve> _voidCrvs;

    public GsaMember2d() { }

    public GsaMember2d(
      Brep brep, List<Curve> includeCurves = null, List<Point3d> includePoints = null,
      int prop = 0) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_2D,
        Property = prop,
      };

      (Tuple<PolyCurve, List<Point3d>, List<string>> edgeTuple,
          Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>> voidTuple,
          Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>> inclTuple)
        = RhinoConversions.ConvertPolyBrepInclusion(brep, includeCurves, includePoints);

      PolyCurve = edgeTuple.Item1;
      Topology = edgeTuple.Item2;
      TopologyType = edgeTuple.Item3;
      _voidCrvs = voidTuple.Item1;
      VoidTopology = voidTuple.Item2;
      VoidTopologyType = voidTuple.Item3;
      _inclCrvs = inclTuple.Item1;
      IncLinesTopology = inclTuple.Item2;
      IncLinesTopologyType = inclTuple.Item3;
      InclusionPoints = inclTuple.Item4;

      Brep = RhinoConversions.BuildBrep(PolyCurve, _voidCrvs);
      if (Brep == null) {
        throw new Exception(" Error with Mem2D: Unable to build Brep, "
          + "please verify input geometry is valid and tolerance "
          + "is set accordingly with your geometry under GSA Plugin Unit "
          + "Settings or if unset under Rhino unit settings");
      }
    }

    internal GsaMember2d(
      Member member, int id, List<Point3d> topology, List<string> topologyType,
      List<List<Point3d>> voidTopology, List<List<string>> voidTopologyType,
      List<List<Point3d>> inlcusionLinesTopology, List<List<string>> inclusionTopologyType,
      List<Point3d> includePoints, IReadOnlyDictionary<int, Prop2D> properties,
      IReadOnlyDictionary<int, AnalysisMaterial> materials, IReadOnlyDictionary<int, Axis> axDict,
      LengthUnit modelUnit) {
      ApiMember = member;
      MeshSize = new Length(member.MeshSize, LengthUnit.Meter).As(modelUnit);
      _id = id;

      if (topology[0] != topology[topology.Count - 1]) // add last point to close boundary
      {
        topology.Add(topology[0]);
        topologyType.Add(string.Empty);
      }

      PolyCurve = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topology, topologyType);
      Topology = topology;
      TopologyType = topologyType;

      if (voidTopology != null) {
        if (_voidCrvs == null) {
          _voidCrvs = new List<PolyCurve>();
        }

        for (int i = 0; i < voidTopology.Count; i++) {
          if (voidTopology[i][0] != voidTopology[i][voidTopology[i].Count - 1]) {
            voidTopology[i].Add(voidTopology[i][0]);
            voidTopologyType[i].Add(string.Empty);
          }

          if (voidTopologyType != null) {
            _voidCrvs.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i],
                voidTopologyType[i]));
          } else {
            _voidCrvs.Add(RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i]));
          }
        }
      }

      VoidTopology = voidTopology;
      VoidTopologyType = voidTopologyType;

      if (inlcusionLinesTopology != null) {
        if (_inclCrvs == null) {
          _inclCrvs = new List<PolyCurve>();
        }

        for (int i = 0; i < inlcusionLinesTopology.Count; i++) {
          if (inclusionTopologyType != null) {
            _inclCrvs.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i],
                inclusionTopologyType[i]));
          } else {
            _inclCrvs.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i]));
          }
        }
      }

      IncLinesTopology = inlcusionLinesTopology;
      IncLinesTopologyType = inclusionTopologyType;
      InclusionPoints = includePoints;

      Brep = RhinoConversions.BuildBrep(PolyCurve, _voidCrvs,
        new Length(0.001, LengthUnit.Meter).As(DefaultUnits.LengthUnitGeometry));

      Prop2d = new GsaProp2d(properties, ApiMember.Property, materials, axDict, modelUnit);
    }

    public GsaMember2d Duplicate(bool cloneApiMember = false) {
      var dup = new GsaMember2d {
        Id = Id,
        MeshSize = MeshSize,
        _guid = new Guid(_guid.ToString()),
        ApiMember = ApiMember,
      };
      if (cloneApiMember) {
        dup.CloneApiObject();
      }

      dup.Prop2d = Prop2d.Duplicate();

      if (Brep == null) {
        return dup;
      }

      dup.Brep = (Brep)Brep.DuplicateShallow();

      dup.PolyCurve = (PolyCurve)PolyCurve.DuplicateShallow();
      dup.Topology = Topology;
      dup.TopologyType = TopologyType;

      var dupVoidCrvs = _voidCrvs.Select(t => (PolyCurve)t.DuplicateShallow()).ToList();
      dup._voidCrvs = dupVoidCrvs;
      dup.VoidTopology = VoidTopology;
      dup.VoidTopologyType = VoidTopologyType;

      var dupInclCrvs = _inclCrvs.Select(t => (PolyCurve)t.DuplicateShallow()).ToList();
      dup._inclCrvs = dupInclCrvs;
      dup.IncLinesTopology = IncLinesTopology;
      dup.IncLinesTopologyType = IncLinesTopologyType;

      dup.InclusionPoints = InclusionPoints;

      return dup;
    }

    public GsaMember2d Morph(SpaceMorph xmorph) {
      GsaMember2d dup = Duplicate(true);
      dup.Id = 0;

      if (dup.Brep != null) {
        xmorph.Morph(dup.Brep);
      }

      if (dup.PolyCurve != null) {
        xmorph.Morph(PolyCurve);
      }

      for (int i = 0; i < dup.Topology.Count; i++) {
        dup.Topology[i] = xmorph.MorphPoint(dup.Topology[i]);
      }

      for (int i = 0; i < _voidCrvs.Count; i++) {
        if (dup._voidCrvs[i] != null) {
          xmorph.Morph(_voidCrvs[i]);
        }

        for (int j = 0; j < dup.VoidTopology[i].Count; j++) {
          dup.VoidTopology[i][j] = xmorph.MorphPoint(dup.VoidTopology[i][j]);
        }
      }

      for (int i = 0; i < _inclCrvs.Count; i++) {
        if (dup._inclCrvs[i] != null) {
          xmorph.Morph(_inclCrvs[i]);
        }

        for (int j = 0; j < dup.IncLinesTopology[i].Count; j++) {
          dup.IncLinesTopology[i][j] = xmorph.MorphPoint(dup.IncLinesTopology[i][j]);
        }
      }

      for (int i = 0; i < dup.InclusionPoints.Count; i++) {
        dup.InclusionPoints[i] = xmorph.MorphPoint(dup.InclusionPoints[i]);
      }

      return dup;
    }

    public override string ToString() {
      string incl = string.Empty;
      if (_inclCrvs != null) {
        if (_inclCrvs.Count > 0) {
          incl = " Incl.Crv:" + _inclCrvs.Count;
        }
      }

      if (InclusionPoints != null) {
        if (InclusionPoints != null && InclusionPoints.Count > 0) {
          incl += " &";
        }

        if (InclusionPoints.Count > 0) {
          incl += " Incl.Pt:" + InclusionPoints.Count;
        }
      }

      string idd = Id == 0 ? string.Empty : "ID:" + Id + " ";
      string type = Mappings.memberTypeMapping.FirstOrDefault(x => x.Value == Type).Key + " ";
      return string.Join(" ", idd.Trim(), type.Trim(), incl.Trim()).Trim().Replace("  ", " ");
    }

    public GsaMember2d Transform(Transform xform) {
      GsaMember2d dup = Duplicate(true);
      dup.Id = 0;

      dup.Brep?.Transform(xform);
      dup.PolyCurve?.Transform(xform);
      dup.Topology = xform.TransformList(dup.Topology).ToList();
      for (int i = 0; i < _voidCrvs.Count; i++) {
        dup._voidCrvs[i]?.Transform(xform);
        dup.VoidTopology[i] = xform.TransformList(dup.VoidTopology[i]).ToList();
      }

      for (int i = 0; i < _inclCrvs.Count; i++) {
        dup._inclCrvs[i]?.Transform(xform);
        dup.IncLinesTopology[i] = xform.TransformList(dup.IncLinesTopology[i]).ToList();
      }

      dup.InclusionPoints = xform.TransformList(dup.InclusionPoints).ToList();

      return dup;
    }

    public GsaMember2d UpdateGeometry(
      Brep brep = null, List<Curve> inclCrvs = null, List<Point3d> inclPts = null) {
      if (brep == null && Brep != null) {
        brep = Brep.DuplicateBrep();
      }

      if (inclCrvs == null && _inclCrvs != null) {
        inclCrvs = _inclCrvs.Select(x => (Curve)x).ToList();
      }

      if (inclPts == null && InclusionPoints != null) {
        inclPts = InclusionPoints.ToList();
      }

      var dup = new GsaMember2d(brep, inclCrvs, inclPts) {
        Id = Id,
        ApiMember = ApiMember,
        Prop2d = Prop2d.Duplicate(),
      };

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
      if (ApiMember.Topology != string.Empty) {
        mem.Topology = ApiMember.Topology;
      }

      if ((Color)ApiMember.Colour
        != Color.FromArgb(0, 0,
          0)) // workaround to handle that System.Drawing.Color is non-nullable type
      {
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    // list of polyline curve type (arch or line) for void /member2d
    //slist of points for inclusion /member2d
  }
}
