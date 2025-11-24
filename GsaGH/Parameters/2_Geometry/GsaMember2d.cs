using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  ///   <para>
  ///     <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-member.html">Members</see> in
  ///     GSA are geometrical objects used in the Design Layer. Members can automatically intersect with other members.
  ///     Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can
  ///     automatically be created from Members used for analysis.
  ///   </para>
  ///   <para>
  ///     A Member2D is the planar/area geometry resembling for instance a slab or a wall. It can be defined
  ///     geometrically from a planar Brep.
  ///   </para>
  ///   <para>
  ///     Refer to
  ///     <see href="https://docs.oasys-software.com/structural/gsa/explanations/members-2d.html">2D Members</see> to read
  ///     more.
  ///   </para>
  /// </summary>
  public class GsaMember2d : GsaGeometryBase {
    public Member ApiMember { get; internal set; }
    public int Id { get; set; } = 0;
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public Brep Brep { get; internal set; }
    public PolyCurve PolyCurve { get; internal set; }
    public Point3dList Topology { get; internal set; }
    public List<string> TopologyType { get; internal set; }
    public List<PolyCurve> VoidCurves { get; internal set; }
    public List<Point3dList> VoidTopology { get; internal set; }
    public List<List<string>> VoidTopologyType { get; internal set; }
    public List<PolyCurve> InclusionLines { get; internal set; }
    public List<Point3dList> InclusionLinesTopology { get; internal set; }
    public List<List<string>> InclusionLinesTopologyType { get; internal set; }
    public Point3dList InclusionPoints { get; internal set; }
    public GsaProperty2d Prop2d { get; set; }
    public Section3dPreview Section3dPreview { get; private set; }

    public GsaOffset Offset {
      get => ApiMember.GetOffsetFromMember();
      set => ApiMember.SetOffsetForMember(value);
    }

    public Angle OrientationAngle {
      get => new Angle(ApiMember.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      set => ApiMember.OrientationAngle = value.Degrees;
    }

    /// <summary>
    ///   Empty constructor instantiating a new API object
    /// </summary>
    public GsaMember2d() { ApiMember = MemberHelper.CreateDefaultApiMember(MemberType.GENERIC_2D); }

    /// <summary>
    ///   Create new instance by casting from a Brep with optional inclusion geometry
    /// </summary>
    public GsaMember2d(
      Brep brep, List<Curve> includeCurves = null, Point3dList includePoints = null) {
      ApiMember = MemberHelper.CreateDefaultApiMember(MemberType.GENERIC_2D);

      (Tuple<PolyCurve, Point3dList, List<string>> edgeTuple,
          Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>> voidTuple,
          Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>, Point3dList> inclTuple)
        = RhinoConversions.ConvertPolyBrepInclusion(brep, includeCurves, includePoints);

      PolyCurve = edgeTuple.Item1;
      Topology = edgeTuple.Item2;
      TopologyType = edgeTuple.Item3;
      VoidCurves = voidTuple.Item1;
      VoidTopology = voidTuple.Item2;
      VoidTopologyType = voidTuple.Item3;
      InclusionLines = inclTuple.Item1;
      InclusionLinesTopology = inclTuple.Item2;
      InclusionLinesTopologyType = inclTuple.Item3;
      InclusionPoints = inclTuple.Item4;

      Brep = RhinoConversions.BuildBrep(PolyCurve, VoidCurves);
      CheckBrep();
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaMember2d(GsaMember2d other) : base(other.LengthUnit) {
      Id = other.Id;
      ApiMember = other.DuplicateApiObject();

      PolyCurve = (PolyCurve)other.PolyCurve?.DuplicateShallow();
      Brep = (Brep)other.Brep?.DuplicateShallow();
      PolyCurve = (PolyCurve)other.PolyCurve?.DuplicateShallow();
      Topology = other.Topology?.Duplicate();
      TopologyType = other.TopologyType;

      VoidCurves = other.VoidCurves?.ConvertAll(t => (PolyCurve)t.DuplicateShallow());
      VoidTopology = other.VoidTopology?.Duplicate();
      VoidTopologyType = other.VoidTopologyType;

      InclusionLines = other.InclusionLines?.ConvertAll(t => (PolyCurve)t.DuplicateShallow());
      InclusionLinesTopology = other.InclusionLinesTopology?.Duplicate();
      InclusionLinesTopologyType = other.InclusionLinesTopologyType;

      InclusionPoints = other.InclusionPoints?.Duplicate();

      Prop2d = other.Prop2d;
      Section3dPreview = other.Section3dPreview?.Duplicate();
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaMember2d(
      KeyValuePair<int, Member> mem, Point3dList topology, List<string> topologyType,
      List<Point3dList> voidTopology, List<List<string>> voidTopologyType,
      List<Point3dList> inlcusionLinesTopology, List<List<string>> inclusionTopologyType,
      Point3dList includePoints, GsaProperty2d prop2d, LengthUnit modelUnit) : base(modelUnit) {
      ApiMember = mem.Value;
      ApiMember.MeshSize = new Length(mem.Value.MeshSize, LengthUnit.Meter).As(modelUnit);
      ApiMember.Group = mem.Value.Group;
      Id = mem.Key;

      InitTopology(topology, topologyType);

      InitVoidCurves(voidTopology, voidTopologyType);

      VoidTopology = voidTopology;
      VoidTopologyType = voidTopologyType;

      InitInclusionLines(inlcusionLinesTopology, inclusionTopologyType);

      InclusionLinesTopology = inlcusionLinesTopology;
      InclusionLinesTopologyType = inclusionTopologyType;
      InclusionPoints = includePoints;

      Brep = RhinoConversions.BuildBrep(PolyCurve, VoidCurves, 0.001);

      Prop2d = prop2d;
    }

    public void CreateSection3dPreview() {
      Section3dPreview = new Section3dPreview(this);
    }

    public Member DuplicateApiObject() {
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

      mem.SetOffsetsFrom(ApiMember);

      // workaround to handle that Color is non-nullable type
      if ((Color)ApiMember.Colour != Color.FromArgb(0, 0, 0)) {
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    public override string ToString() {
      string incl = string.Empty;
      if (!InclusionLines.IsNullOrEmpty()) {
        incl = "Incl.Crv:" + InclusionLines.Count;
      }

      if (InclusionPoints != null) {
        if (!InclusionLines.IsNullOrEmpty()) {
          incl += " & ";
        }

        if (InclusionPoints.Count > 0) {
          incl += "Incl.Pt:" + InclusionPoints.Count;
        }
      }

      string id = Id > 0 ? $"ID:{Id}" : string.Empty;
      string type = Mappings._memberTypeMapping.FirstOrDefault(x => x.Value == ApiMember.Type).Key;
      return string.Join(" ", id, type, incl).TrimSpaces();
    }

    public void UpdateGeometry(
      Brep brep = null, List<Curve> inclCrvs = null, Point3dList inclPts = null) {
      if (brep == null && Brep != null) {
        brep = Brep.DuplicateBrep();
      }

      if (inclCrvs == null && InclusionLines != null) {
        inclCrvs = InclusionLines.Select(x => (Curve)x).ToList();
      }

      if (inclPts == null && InclusionPoints != null) {
        inclPts = new Point3dList(InclusionPoints);
      }

      (Tuple<PolyCurve, Point3dList, List<string>> edgeTuple,
          Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>> voidTuple,
          Tuple<List<PolyCurve>, List<Point3dList>, List<List<string>>, Point3dList> inclTuple)
        = RhinoConversions.ConvertPolyBrepInclusion(brep, inclCrvs, inclPts);

      PolyCurve = edgeTuple.Item1;
      Topology = edgeTuple.Item2;
      TopologyType = edgeTuple.Item3;
      VoidCurves = voidTuple.Item1;
      VoidTopology = voidTuple.Item2;
      VoidTopologyType = voidTuple.Item3;
      InclusionLines = inclTuple.Item1;
      InclusionLinesTopology = inclTuple.Item2;
      InclusionLinesTopologyType = inclTuple.Item3;
      InclusionPoints = inclTuple.Item4;

      Brep = RhinoConversions.BuildBrep(PolyCurve, VoidCurves);
      CheckBrep();
    }

    private void CheckBrep() {
      if (Brep == null) {
        throw new Exception(" Error with Mem2D: Unable to build Brep, "
          + "please verify input geometry is valid and tolerance "
          + "is set accordingly with your geometry under GSA Plugin Unit "
          + "Settings or if unset under Rhino unit settings");
      }
    }

    public void SetProperty(GsaProperty2d property) {
      Prop2d = property;
      if (property.ApiProp2d == null) {
        return;
      }

      ApiMember.Type2D = property.ApiProp2d.Type == Property2D_Type.LOAD ?
        AnalysisOrder.LOAD_PANEL : AnalysisOrder.LINEAR;
    }

    private void InitInclusionLines(
      List<Point3dList> inlcusionLinesTopology, List<List<string>> inclusionTopologyType) {
      if (inlcusionLinesTopology == null) {
        return;
      }

      InclusionLines ??= new List<PolyCurve>();

      for (int i = 0; i < inlcusionLinesTopology.Count; i++) {
        InclusionLines.Add(inclusionTopologyType != null ?
          RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i], inclusionTopologyType[i]) :
          RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i]));
      }
    }

    private void InitTopology(Point3dList topology, List<string> topologyType) {
      if (!topology.IsClosed()) // add last point to close boundary
      {
        topology.Add(topology[0]);
        topologyType.Add(string.Empty);
      }

      PolyCurve = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topology, topologyType);
      Topology = topology;
      TopologyType = topologyType;
    }

    private void InitVoidCurves(List<Point3dList> voidTopology, List<List<string>> voidTopologyType) {
      if (voidTopology == null) {
        return;
      }

      VoidCurves ??= new List<PolyCurve>();

      for (int i = 0; i < voidTopology.Count; i++) {
        if (voidTopology[i][0] != voidTopology[i][voidTopology[i].Count - 1]) {
          voidTopology[i].Add(voidTopology[i][0]);
          voidTopologyType[i].Add(string.Empty);
        }

        VoidCurves.Add(voidTopologyType != null ?
          RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i], voidTopologyType[i]) :
          RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i]));
      }
    }
  }
}
