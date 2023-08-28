using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Helpers.GsaApi;
using OasysUnits;
using Rhino.Collections;
using Rhino.Geometry;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para><see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-member.html">Members</see> in GSA are geometrical objects used in the Design Layer. Members can automatically intersection with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. </para>
  /// <para>A Member1D is the linear geometry resembling for instance a column or a beam. They can be defined geometrically by a PolyCurve consisting of either multiple line segments or a single arc.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/explanations/members-1d.html">1D Members</see> to read more.</para>
  /// </summary>
  public class GsaMember1d {
    public bool AutomaticOffsetEnd1 {
      get => ApiMember.AutomaticOffset.End1;
      set {
        CloneApiObject();
        ApiMember.AutomaticOffset.End1 = value;
      }
    }
    public bool AutomaticOffsetEnd2 {
      get => ApiMember.AutomaticOffset.End2;
      set {
        CloneApiObject();
        ApiMember.AutomaticOffset.End2 = value;
      }
    }
    public double AutomaticOffsetLength1 => ApiMember.AutomaticOffset.X1;
    public double AutomaticOffsetLength2 => ApiMember.AutomaticOffset.X2;
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
    public GsaNode OrientationNode {
      get => _orientationNode;
      set {
        CloneApiObject();
        _orientationNode = value;
      }
    }
    public PolyCurve PolyCurve {
      get => _crv;
      set {
        Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv
          = RhinoConversions.ConvertMem1dCrv(value);
        _crv = convertCrv.Item1;
        Topology = convertCrv.Item2;
        TopologyType = convertCrv.Item3;
      }
    }
    public GsaBool6 ReleaseEnd {
      get => new GsaBool6(ApiMember.GetEndRelease(1).Releases);
      set {
        _rel2 = value ?? new GsaBool6();
        CloneApiObject();
        ApiMember.SetEndRelease(1, new EndRelease(_rel2._bool6));
        UpdateReleasesPreview();
      }
    }
    public GsaBool6 ReleaseStart {
      get => new GsaBool6(ApiMember.GetEndRelease(0).Releases);
      set {
        _rel1 = value ?? new GsaBool6();
        CloneApiObject();
        ApiMember.SetEndRelease(0, new EndRelease(_rel1._bool6));
        UpdateReleasesPreview();
      }
    }
    public GsaSection Section { get; set; } = new GsaSection();
    public List<Point3d> Topology { get; private set; }
    public List<string> TopologyType { get; private set; }
    public MemberType Type {
      get => ApiMember.Type;
      set {
        CloneApiObject();
        ApiMember.Type = value;
      }
    }
    public ElementType Type1D {
      get => ApiMember.Type1D;
      set {
        CloneApiObject();
        ApiMember.Type1D = value;
      }
    }
    internal Member ApiMember { get; set; } = new Member();
    internal GsaSection3dPreview Section3dPreview { get; set; }
    internal GsaLocalAxes LocalAxes { get; set; } = null;
    internal List<Line> _previewGreenLines;
    internal List<Line> _previewRedLines;
    private PolyCurve _crv = new PolyCurve();
    // Polyline for visualisation /member1d/member2d
    private Guid _guid = Guid.NewGuid();
    private int _id = 0;
    private GsaNode _orientationNode;
    private GsaBool6 _rel1;
    private GsaBool6 _rel2;

    public GsaMember1d() { }

    public GsaMember1d(Curve crv, int prop = 0) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_1D,
        Property = prop,
      };
      Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv
        = RhinoConversions.ConvertMem1dCrv(crv);
      _crv = convertCrv.Item1;
      Topology = convertCrv.Item2;
      TopologyType = convertCrv.Item3;
      Section = new GsaSection(prop);
      UpdateReleasesPreview();
    }

    internal GsaMember1d(
      KeyValuePair<int, Member> mem,
      List<Point3d> topology,
      List<string> topoType,
      ReadOnlyCollection<double> localAxis,
      GsaSection section,
      LengthUnit modelUnit) {
      ApiMember = mem.Value;
      MeshSize = new Length(mem.Value.MeshSize, LengthUnit.Meter).As(modelUnit);
      _id = mem.Key;
      _crv = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topology, topoType);
      Topology = topology;
      TopologyType = topoType;
      _rel1 = new GsaBool6(ApiMember.GetEndRelease(0).Releases);
      _rel2 = new GsaBool6(ApiMember.GetEndRelease(1).Releases);
      LocalAxes = new GsaLocalAxes(localAxis);
      Section = section;
      UpdateReleasesPreview();
    }

    public GsaMember1d Clone() {
      var dup = new GsaMember1d {
        Id = Id,
        MeshSize = MeshSize,
        ApiMember = ApiMember,
        LocalAxes = LocalAxes,
      };
      dup.CloneApiObject();

      dup._crv = (PolyCurve)_crv.DuplicateShallow();
      if (_rel1 != null) {
        dup._rel1 = _rel1.Duplicate();
      }

      if (_rel2 != null) {
        dup._rel2 = _rel2.Duplicate();
      }

      dup.Section = Section.Duplicate();
      dup.Topology = Topology;
      dup.TopologyType = TopologyType;
      if (_orientationNode != null) {
        dup._orientationNode = _orientationNode.Duplicate();
      }

      if (Section3dPreview != null) {
        dup.Section3dPreview = Section3dPreview;
      }

      if (_previewGreenLines != null) {
        dup._previewGreenLines = _previewGreenLines.ToList();
      }

      if (_previewRedLines != null) {
        dup._previewRedLines = _previewRedLines.ToList();
      }

      return dup;
    }

    public GsaMember1d Duplicate() {
      return this;
    }

    public GsaMember1d Morph(SpaceMorph xmorph) {
      GsaMember1d dup = Clone();
      dup.Id = 0;
      dup.LocalAxes = null;

      var pts = Topology.ToList();
      for (int i = 0; i < pts.Count; i++) {
        pts[i] = xmorph.MorphPoint(pts[i]);
      }

      dup.Topology = pts;

      if (_crv != null) {
        PolyCurve crv = _crv.DuplicatePolyCurve();
        xmorph.Morph(crv);
        dup._crv = crv;
      }

      if (Section3dPreview != null) {
        dup.Section3dPreview = Section3dPreview.Morph(xmorph);
      }

      dup.UpdateReleasesPreview();
      return dup;
    }

    public override string ToString() {
      string idd = Id == 0 ? string.Empty : "ID:" + Id + " ";
      string type = Mappings.memberTypeMapping.FirstOrDefault(x => x.Value == Type).Key + " ";
      string pb = Section.Id > 0 ? "PB" + Section.Id : Section.Profile;
      return string.Join(" ", idd.Trim(), type.Trim(), pb.Trim()).Trim().Replace("  ", " ");
    }

    public GsaMember1d Transform(Transform xform) {
      GsaMember1d dup = Clone();
      dup.Id = 0;
      dup.LocalAxes = null;

      var pts = Topology.ToList();
      var xpts = new Point3dList(pts);
      xpts.Transform(xform);
      dup.Topology = xpts.ToList();

      if (_crv != null) {
        PolyCurve crv = _crv.DuplicatePolyCurve();
        crv.Transform(xform);
        dup._crv = crv;
      }

      if (Section3dPreview != null) {
        dup.Section3dPreview = Section3dPreview.Transform(xform);
      }

      dup.UpdateReleasesPreview();
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
        EquivalentUniformMomentFactor = ApiMember.EquivalentUniformMomentFactor,
        MeshSize = ApiMember.MeshSize,
        MomentAmplificationFactorStrongAxis = ApiMember.MomentAmplificationFactorStrongAxis,
        MomentAmplificationFactorWeakAxis = ApiMember.MomentAmplificationFactorWeakAxis,
        Name = ApiMember.Name.ToString(),
        OrientationAngle = ApiMember.OrientationAngle,
        OrientationNode = ApiMember.OrientationNode,
        Property = ApiMember.Property,
        Type = ApiMember.Type,
        Type1D = ApiMember.Type1D,
        AutomaticOffset = ApiMember.AutomaticOffset,
      };
      if (ApiMember.Topology != string.Empty) {
        mem.Topology = ApiMember.Topology;
      }

      mem.Offset.X1 = ApiMember.Offset.X1;
      mem.Offset.X2 = ApiMember.Offset.X2;
      mem.Offset.Y = ApiMember.Offset.Y;
      mem.Offset.Z = ApiMember.Offset.Z;

      mem.MeshSize = MeshSize;

      mem.SetEndRelease(0, ApiMember.GetEndRelease(0));
      mem.SetEndRelease(1, ApiMember.GetEndRelease(1));

      if ((Color)ApiMember.Colour
        != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
      {
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    internal void UpdateCurveFromTopology() {
      if (_crv == null) {
        return;
      }

      _crv = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(Topology, TopologyType);
    }

    internal void UpdatePreview() {
      if (Section.Profile != string.Empty && GsaSection.ValidProfile(Section.Profile)) {
        Section3dPreview = new GsaSection3dPreview(this);
      } else {
        Section3dPreview = null;
      }

      UpdateReleasesPreview();
    }

    private void UpdateReleasesPreview() {
      if (!((_rel1 != null) & (_rel2 != null))) {
        return;
      }

      if (_rel1.X || _rel1.Y || _rel1.Z || _rel1.Xx || _rel1.Yy || _rel1.Zz || _rel2.X || _rel2.Y
        || _rel2.Z || _rel2.Xx || _rel2.Yy || _rel2.Zz) {
        Tuple<List<Line>, List<Line>> previewCurves = Display.Preview1D(_crv,
          ApiMember.OrientationAngle * Math.PI / 180.0, _rel1, _rel2);
        _previewGreenLines = previewCurves.Item1;
        _previewRedLines = previewCurves.Item2;
      } else {
        _previewGreenLines = null;
      }
    }
  }
}
