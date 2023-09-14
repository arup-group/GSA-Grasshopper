﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
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
  /// <para><see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-member.html">Members</see> in GSA are geometrical objects used in the Design Layer. Members can automatically intersect with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. </para>
  /// <para>A Member2D is the planar/area geometry resembling for instance a slab or a wall. It can be defined geometrically from a planar Brep.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/explanations/members-2d.html">2D Members</see> to read more.</para>
  /// </summary>
  public class GsaMember2d {
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
      get => GetOffSetFromApiMember();
      set => SetOffsetInApiElement(value);
    }

    public Angle OrientationAngle {
      get => new Angle(ApiMember.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      set => ApiMember.OrientationAngle = value.Degrees;
    }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaMember2d() {
      ApiMember = new Member() {
        Type = MemberType.GENERIC_2D,
      };
    }

    /// <summary>
    /// Create new instance by casting from a Brep with optional inclusion geometry
    /// </summary>
    public GsaMember2d(
      Brep brep, List<Curve> includeCurves = null, Point3dList includePoints = null) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_2D,
      };

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
      if (Brep == null) {
        throw new Exception(" Error with Mem2D: Unable to build Brep, "
          + "please verify input geometry is valid and tolerance "
          + "is set accordingly with your geometry under GSA Plugin Unit "
          + "Settings or if unset under Rhino unit settings");
      }
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaMember2d(GsaMember2d other) {
      Id = other.Id;
      ApiMember = other.DuplicateApiObject();

      PolyCurve = (PolyCurve)other.PolyCurve?.DuplicateShallow();
      Brep = (Brep)other.Brep?.DuplicateShallow();
      PolyCurve = (PolyCurve)other.PolyCurve?.DuplicateShallow();
      Topology = other.Topology;
      TopologyType = other.TopologyType;

      VoidCurves = other.VoidCurves?.ConvertAll(t => (PolyCurve)t.DuplicateShallow());
      VoidTopology = VoidTopology;
      VoidTopologyType = VoidTopologyType;

      InclusionLines = other.InclusionLines?.ConvertAll(t => (PolyCurve)t.DuplicateShallow());
      InclusionLinesTopology = other.InclusionLinesTopology;
      InclusionLinesTopologyType = other.InclusionLinesTopologyType;

      InclusionPoints = other.InclusionPoints;

      Prop2d = other.Prop2d;
      Section3dPreview = other.Section3dPreview;
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaMember2d(
      KeyValuePair<int, Member> mem,
      Point3dList topology,
      List<string> topologyType,
      List<Point3dList> voidTopology,
      List<List<string>> voidTopologyType,
      List<Point3dList> inlcusionLinesTopology,
      List<List<string>> inclusionTopologyType,
      Point3dList includePoints,
      GsaProperty2d prop2d,
      LengthUnit modelUnit) {
      ApiMember = mem.Value;
      ApiMember.MeshSize = new Length(mem.Value.MeshSize, LengthUnit.Meter).As(modelUnit);
      Id = mem.Key;

      if (topology[0] != topology[topology.Count - 1]) // add last point to close boundary
      {
        topology.Add(topology[0]);
        topologyType.Add(string.Empty);
      }

      PolyCurve = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topology, topologyType);
      Topology = topology;
      TopologyType = topologyType;

      if (voidTopology != null) {
        VoidCurves ??= new List<PolyCurve>();

        for (int i = 0; i < voidTopology.Count; i++) {
          if (voidTopology[i][0] != voidTopology[i][voidTopology[i].Count - 1]) {
            voidTopology[i].Add(voidTopology[i][0]);
            voidTopologyType[i].Add(string.Empty);
          }

          if (voidTopologyType != null) {
            VoidCurves.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(
                voidTopology[i], voidTopologyType[i]));
          } else {
            VoidCurves.Add(RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(voidTopology[i]));
          }
        }
      }

      VoidTopology = voidTopology;
      VoidTopologyType = voidTopologyType;

      if (inlcusionLinesTopology != null) {
        InclusionLines ??= new List<PolyCurve>();

        for (int i = 0; i < inlcusionLinesTopology.Count; i++) {
          if (inclusionTopologyType != null) {
            InclusionLines.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i],
                inclusionTopologyType[i]));
          } else {
            InclusionLines.Add(
              RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(inlcusionLinesTopology[i]));
          }
        }
      }

      InclusionLinesTopology = inlcusionLinesTopology;
      InclusionLinesTopologyType = inclusionTopologyType;
      InclusionPoints = includePoints;

      Brep = RhinoConversions.BuildBrep(PolyCurve, VoidCurves, 0.001);

      Prop2d = prop2d;
    }

    public void CreateSection3dPreview() {
      Section3dPreview = new Section3dPreview(this);
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
      string type = Mappings.memberTypeMapping.FirstOrDefault(x => x.Value == ApiMember.Type).Key;
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
      if (Brep == null) {
        throw new Exception(" Error with Mem2D: Unable to build Brep, "
          + "please verify input geometry is valid and tolerance "
          + "is set accordingly with your geometry under GSA Plugin Unit "
          + "Settings or if unset under Rhino unit settings");
      }
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

      mem.Offset.X1 = ApiMember.Offset.X1;
      mem.Offset.X2 = ApiMember.Offset.X2;
      mem.Offset.Y = ApiMember.Offset.Y;
      mem.Offset.Z = ApiMember.Offset.Z;

      // workaround to handle that Color is non-nullable type
      if ((Color)ApiMember.Colour != Color.FromArgb(0, 0, 0)) {
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    private GsaOffset GetOffSetFromApiMember() {
      return new GsaOffset(
        ApiMember.Offset.X1, ApiMember.Offset.X2, ApiMember.Offset.Y, ApiMember.Offset.Z);
    }

    private void SetOffsetInApiElement(GsaOffset offset) {
      ApiMember.Offset.X1 = offset.X1.Meters;
      ApiMember.Offset.X2 = offset.X2.Meters;
      ApiMember.Offset.Y = offset.Y.Meters;
      ApiMember.Offset.Z = offset.Z.Meters;
    }
  }
}
