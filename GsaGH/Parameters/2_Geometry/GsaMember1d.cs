using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  /// <para><see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-member.html">Members</see> in GSA are geometrical objects used in the Design Layer. Members can automatically intersection with other members. Members are as such more closely related to building objects, like a beam, column, slab or wall. Elements can automatically be created from Members used for analysis. </para>
  /// <para>A Member1D is the linear geometry resembling for instance a column or a beam. They can be defined geometrically by a PolyCurve consisting of either multiple line segments or a single arc.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/explanations/members-1d.html">1D Members</see> to read more.</para>
  /// </summary>
  public class GsaMember1d {
    public Member ApiMember { get; internal set; }
    public int Id { get; set; } = 0;
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public PolyCurve PolyCurve { get; internal set; } = new PolyCurve();
    public Point3dList Topology { get; internal set; }
    public List<string> TopologyType { get; internal set; }
    public GsaNode OrientationNode { get; set; }
    public GsaSection Section { get; set; }
    public GsaSpringProperty SpringProperty { get; set; }
    public LocalAxes LocalAxes { get; private set; }
    public Section3dPreview Section3dPreview { get; private set; }
    public ReleasePreview ReleasePreview { get; private set; }

    public GsaOffset Offset {
      get => GetOffSetFromApiMember();
      set => SetOffsetInApiElement(value);
    }

    public Angle OrientationAngle {
      get => new Angle(ApiMember.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      set => ApiMember.OrientationAngle = value.Degrees;
    }
    public GsaBool6 ReleaseEnd {
      get => new GsaBool6(ApiMember.GetEndRelease(1).Releases);
      set => SetRelease(value, 1);
    }

    public GsaBool6 ReleaseStart {
      get => new GsaBool6(ApiMember.GetEndRelease(0).Releases);
      set => SetRelease(value, 0);
    }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaMember1d() {
      ApiMember = new Member() {
        Type = MemberType.GENERIC_1D,
        Type1D = ElementType.BEAM,
        Group = GsaMember.DefaultGroupValue,
      };
    }

    /// <summary>
    /// Create new instance by casting from a Curve
    /// </summary>
    /// <param name="crv"></param>
    public GsaMember1d(Curve crv) {
      ApiMember = new Member {
        Type = MemberType.GENERIC_1D,
        Type1D = ElementType.BEAM,
        Group = GsaMember.DefaultGroupValue,
      };
      UpdateGeometry(crv);
      UpdateReleasesPreview();
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaMember1d(GsaMember1d other) {
      Id = other.Id;
      ApiMember = other.DuplicateApiObject();
      LocalAxes = other.LocalAxes;
      PolyCurve = (PolyCurve)other.PolyCurve.DuplicateShallow();
      Topology = other.Topology?.Duplicate();
      TopologyType = other.TopologyType;
      OrientationNode = other.OrientationNode;
      Section = other.Section;
      SpringProperty = other.SpringProperty;
      Section3dPreview = other.Section3dPreview?.Duplicate();
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaMember1d(KeyValuePair<int, Member> mem, Point3dList topology,
      List<string> topoType, ReadOnlyCollection<double> localAxis, GsaNode orientationNode,
      LengthUnit modelUnit, GsaSpringProperty springProperty = null) {
      Id = mem.Key;
      ApiMember = mem.Value;
      ApiMember.MeshSize = new Length(mem.Value.MeshSize, LengthUnit.Meter).As(modelUnit);
      ApiMember.Group = mem.Value.Group;

      AdjustToModelUnit(modelUnit);

      PolyCurve = RhinoConversions.BuildArcLineCurveFromPtsAndTopoType(topology, topoType);
      Topology = topology;
      TopologyType = topoType;
      OrientationNode = orientationNode;
      SpringProperty = springProperty;
      LocalAxes = new LocalAxes(localAxis);
      UpdateReleasesPreview();
    }

    public void CreateSection3dPreview() {
      Section3dPreview = new Section3dPreview(this);
    }

    public Member DuplicateApiObject() {
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
        EffectiveLength = ApiMember.EffectiveLength,
      };
      if (ApiMember.Topology != string.Empty) {
        mem.Topology = ApiMember.Topology;
      }

      mem.Offset.X1 = ApiMember.Offset.X1;
      mem.Offset.X2 = ApiMember.Offset.X2;
      mem.Offset.Y = ApiMember.Offset.Y;
      mem.Offset.Z = ApiMember.Offset.Z;

      mem.SetEndRelease(0, ApiMember.GetEndRelease(0));
      mem.SetEndRelease(1, ApiMember.GetEndRelease(1));

      // workaround to handle that Color is non-nullable type
      if ((Color)ApiMember.Colour != Color.FromArgb(0, 0, 0)) {
        mem.Colour = ApiMember.Colour;
      }

      return mem;
    }

    public override string ToString() {
      string id = Id > 0 ? $"ID:{Id}" : string.Empty;
      string type = Mappings._memberTypeMapping.FirstOrDefault(x => x.Value == ApiMember.Type).Key;
      string property = string.Empty;
      if (Section != null) {
        property = Section.Id > 0 ? $"PB{Section.Id}"
        : Section.ApiSection != null ? Section.ApiSection.Profile : string.Empty;
      } else if (SpringProperty != null) {
        property = SpringProperty.Id > 0 ? $"SP{SpringProperty.Id}"
        : SpringProperty.ApiProperty != null ? SpringProperty.ApiProperty.Name : string.Empty;
      }
      return string.Join(" ", id, type, property).TrimSpaces();
    }

    public void UpdateGeometry(Curve crv) {
      Tuple<PolyCurve, Point3dList, List<string>> convertCrv
        = RhinoConversions.ConvertMem1dCrv(crv);
      PolyCurve = convertCrv.Item1;
      Topology = convertCrv.Item2;
      TopologyType = convertCrv.Item3;
    }

    private void AdjustToModelUnit(LengthUnit modelUnit) {
      ApiMember.EffectiveLength.DestablisingLoad
        = new Length(ApiMember.EffectiveLength.DestablisingLoad, LengthUnit.Meter).As(modelUnit);
      if (modelUnit != LengthUnit.Meter
        && ApiMember.EffectiveLength is EffectiveLengthFromUserSpecifiedValue user) {
        if (user.EffectiveLengthAboutY.Option == EffectiveLengthOptionType.Absolute) {
          user.EffectiveLengthAboutY = new EffectiveLengthAttribute(EffectiveLengthOptionType.Absolute,
            new Length(user.EffectiveLengthAboutY.Value, LengthUnit.Meter).As(modelUnit));
        }

        if (user.EffectiveLengthAboutZ.Option == EffectiveLengthOptionType.Absolute) {
          user.EffectiveLengthAboutZ = new EffectiveLengthAttribute(EffectiveLengthOptionType.Absolute,
            new Length(user.EffectiveLengthAboutZ.Value, LengthUnit.Meter).As(modelUnit));
        }

        if (user.EffectiveLengthLaterialTorsional.Option == EffectiveLengthOptionType.Absolute) {
          user.EffectiveLengthLaterialTorsional = new EffectiveLengthAttribute(
            EffectiveLengthOptionType.Absolute,
            new Length(user.EffectiveLengthLaterialTorsional.Value, LengthUnit.Meter).As(modelUnit));
        }
      }
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

    private void SetRelease(GsaBool6 bool6, int pos) {
      ApiMember.SetEndRelease(pos, new EndRelease(bool6.ApiBool6));
      UpdateReleasesPreview();
    }

    public void UpdateReleasesPreview() {
      Bool6 s = ApiMember.GetEndRelease(0).Releases;
      Bool6 e = ApiMember.GetEndRelease(1).Releases;

      ReleasePreview = s.X || s.Y || s.Z || s.XX || s.YY || s.ZZ
        || e.X || e.Y || e.Z || e.XX || e.YY || e.ZZ
        ? new ReleasePreview(PolyCurve,
          ApiMember.OrientationAngle * Math.PI / 180.0, s, e)
        : new ReleasePreview();
    }
  }

  public struct GsaMember {
    public const int DefaultGroupValue = 1;
  }
}
