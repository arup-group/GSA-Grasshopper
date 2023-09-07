using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaApi;
using OasysUnits;
using Rhino.Display;
using Rhino.Geometry;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para><see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-member.html">Members</see> in GSA are geometrical objects used in the Design Layer. Members can automatically intersect with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. </para>
  /// <para>A Member2D is the planar/area geometry resembling for instance a slab or a wall. It can be defined geometrically from a planar Brep.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/explanations/members-2d.html">2D Members</see> to read more.</para>
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
    public GsaAPI.MeshMode2d MeshMode {
      get => ApiMember.MeshMode2d;
      set {
        CloneApiObject();
        ApiMember.MeshMode2d = value;
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
    public GsaProperty2d Prop2d { get; set; } = new GsaProperty2d();
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
    internal GsaSection3dPreview Section3dPreview { get; set; }
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

      Prop2d = new GsaProperty2d(prop);
    }

    internal GsaMember2d(
      KeyValuePair<int, Member> mem,
      List<Point3d> topology,
      List<string> topologyType,
      List<List<Point3d>> voidTopology,
      List<List<string>> voidTopologyType,
      List<List<Point3d>> inlcusionLinesTopology,
      List<List<string>> inclusionTopologyType,
      List<Point3d> includePoints,
      GsaProperty2d prop2d,
      LengthUnit modelUnit) {
      ApiMember = mem.Value;
      MeshSize = new Length(mem.Value.MeshSize, LengthUnit.Meter).As(modelUnit);
      _id = mem.Key;

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

      Brep = RhinoConversions.BuildBrep(PolyCurve, _voidCrvs, 0.001);

      Prop2d = prop2d;
    }

    public GsaMember2d Clone() {
      var dup = new GsaMember2d {
        Id = Id,
        MeshSize = MeshSize,
        ApiMember = ApiMember,
      };
      dup.CloneApiObject();

      dup.Prop2d = Prop2d;

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
      dup.IncLinesTopology = IncLinesTopology.ToList();
      dup.IncLinesTopologyType = IncLinesTopologyType.ToList();

      dup.InclusionPoints = InclusionPoints.ToList();

      if (Section3dPreview != null) {
        dup.Section3dPreview = Section3dPreview.Duplicate();
      }

      return dup;
    }

    public GsaMember2d Duplicate() {
      return this;
    }

    public GsaMember2d Morph(SpaceMorph xmorph) {
      GsaMember2d dup = Clone();
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

      if (Section3dPreview != null) {
        dup.Section3dPreview.Morph(xmorph);
      }

      return dup;
    }

    public override string ToString() {
      string incl = string.Empty;
      if (!_inclCrvs.IsNullOrEmpty()) {
        incl = "Incl.Crv:" + _inclCrvs.Count;
      }

      if (InclusionPoints != null) {
        if (!_inclCrvs.IsNullOrEmpty()) {
          incl += " & ";
        }

        if (InclusionPoints.Count > 0) {
          incl += "Incl.Pt:" + InclusionPoints.Count;
        }
      }

      string id = Id > 0 ? $"ID:{Id}" : string.Empty;
      string type = Mappings.memberTypeMapping.FirstOrDefault(x => x.Value == ApiMember.Type).Key;
      return string.Join(" ", id, type, incl).Trim().Replace("  ", " ");
    }

    public GsaMember2d Transform(Transform xform) {
      GsaMember2d dup = Clone();
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

      if (Section3dPreview != null) {
        dup.Section3dPreview.Transform(xform);
      }

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
        Prop2d = Prop2d,
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
        OrientationAngle = ApiMember.OrientationAngle,
        OrientationNode = ApiMember.OrientationNode,
        Property = ApiMember.Property,
        Type = ApiMember.Type,
        Type2D = ApiMember.Type2D,
        AutomaticOffset = ApiMember.AutomaticOffset,
        IsIntersector = ApiMember.IsIntersector,
        MeshMode2d = ApiMember.MeshMode2d,
      };
      if (ApiMember.Topology != string.Empty) {
        mem.Topology = ApiMember.Topology;
      }

      mem.Offset.X1 = ApiMember.Offset.X1;
      mem.Offset.X2 = ApiMember.Offset.X2;
      mem.Offset.Y = ApiMember.Offset.Y;
      mem.Offset.Z = ApiMember.Offset.Z;

      if ((Color)ApiMember.Colour != Color.FromArgb(0, 0, 0)) {
        // workaround to handle that System.Drawing.Color is non-nullable type
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    internal void UpdatePreview() {
      if (Prop2d != null && !Prop2d.IsReferencedById) {
        Section3dPreview = new GsaSection3dPreview(this);
      } else {
        Section3dPreview = null;
      }
    }
  }
}
