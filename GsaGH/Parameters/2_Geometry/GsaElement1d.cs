﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.Import;
using OasysUnits;
using Rhino.Geometry;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Elements in GSA are geometrical objects used for Analysis. Elements must be split at intersections with other elements to connect to each other or 'node out'. </para>
  /// <para>Element1Ds are one-dimensional stick elements (representing <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#element-types">1D Element Types</see>) used by the solver for analysis.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-element.html">Elements</see> to read more.</para>
  /// </summary>
  public class GsaElement1d {
    public Element ApiElement { get; internal set; }
    public int Id { get; set; } = 0;
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public LineCurve Line { get; internal set; } = new LineCurve();
    public GsaNode OrientationNode { get; set; }
    public GsaSection Section { get; set; }
    public LocalAxes LocalAxes { get; private set; }
    public Section3dPreview Section3dPreview { get; private set; }
    public ReleasePreview ReleasePreview { get; private set; }

    public GsaOffset Offset {
      get => GetOffSetFromApiElement();
      set => SetOffsetInApiElement(value);
    }

    public Angle OrientationAngle {
      get => new Angle(ApiElement.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      set => ApiElement.OrientationAngle = value.Degrees;
    }
    public GsaBool6 ReleaseEnd {
      get => new GsaBool6(ApiElement.GetEndRelease(1).Releases);
      set => SetRelease(value, 1);
    }

    public GsaBool6 ReleaseStart {
      get => new GsaBool6(ApiElement.GetEndRelease(0).Releases);
      set => SetRelease(value, 0);
    }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaElement1d() {
      ApiElement = new Element {
        Type = ElementType.BEAM,
      };
    }

    /// <summary>
    /// Create new instance by casting from a Line
    /// </summary>
    /// <param name="line"></param>
    public GsaElement1d(LineCurve line) {
      Id = Id;
      ApiElement = new Element {
        Type = ElementType.BEAM,
      };
      Line = line;
      UpdateReleasesPreview();
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaElement1d(GsaElement1d other) {
      Id = other.Id;
      ApiElement = other.DuplicateApiObject();
      LocalAxes = other.LocalAxes;
      Line = (LineCurve)other.Line.DuplicateShallow();
      OrientationNode = other.OrientationNode;
      Section = other.Section;
      Section3dPreview = other.Section3dPreview;
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaElement1d(KeyValuePair<int, Element> element, IReadOnlyDictionary<int, Node> nodes,
      GsaSection section, ReadOnlyCollection<double> localAxes, LengthUnit modelUnit) {
      Id = element.Key;
      ApiElement = element.Value;
      if (nodes.Keys.Contains(ApiElement.OrientationNode)) {
        OrientationNode
          = new GsaNode(Nodes.Point3dFromNode(nodes[ApiElement.OrientationNode], modelUnit));
      }

      Line = new LineCurve(new Line(
        Nodes.Point3dFromNode(nodes[ApiElement.Topology[0]], modelUnit),
        Nodes.Point3dFromNode(nodes[ApiElement.Topology[1]], modelUnit)));
      LocalAxes = new LocalAxes(localAxes);
      Section = section;
    }

    public void CreateSection3dPreview() {
      Section3dPreview = new Section3dPreview(this);
    }

    public Element DuplicateApiObject() {
      var elem = new Element {
        Group = ApiElement.Group,
        IsDummy = ApiElement.IsDummy,
        Name = ApiElement.Name.ToString(),
        OrientationAngle = ApiElement.OrientationAngle,
        OrientationNode = ApiElement.OrientationNode,
        ParentMember = ApiElement.ParentMember,
        Property = ApiElement.Property,
        Type = ApiElement.Type,
        Topology = new ReadOnlyCollection<int>(ApiElement.Topology.ToList()),
      };
      elem.SetEndRelease(0, ApiElement.GetEndRelease(0));
      elem.SetEndRelease(1, ApiElement.GetEndRelease(1));

      elem.Offset.X1 = ApiElement.Offset.X1;
      elem.Offset.X2 = ApiElement.Offset.X2;
      elem.Offset.Y = ApiElement.Offset.Y;
      elem.Offset.Z = ApiElement.Offset.Z;

      // workaround to handle that System.Drawing.Color is non-nullable type
      if ((Color)ApiElement.Colour != Color.FromArgb(0, 0, 0)) {
        elem.Colour = ApiElement.Colour;
      }

      return elem;
    }

    public override string ToString() {
      string id = Id > 0 ? $"ID:{Id}" : string.Empty;
      string type = Mappings.elementTypeMapping.FirstOrDefault(x => x.Value == ApiElement.Type).Key;
      string pb = string.Empty;
      if (Section != null) {
        pb = Section.Id > 0 ? $"PB{Section.Id}"
        : Section.ApiSection != null ? Section.ApiSection.Profile : string.Empty;
      }

      return string.Join(" ", id, type, pb).TrimSpaces();
    }

    public void UpdateReleasesPreview() {
      Bool6 s = ApiElement.GetEndRelease(0).Releases;
      Bool6 e = ApiElement.GetEndRelease(1).Releases;

      if (s.X || s.Y || s.Z || s.XX || s.YY || s.ZZ
        || e.X || e.Y || e.Z || e.XX || e.YY || e.ZZ) {
        var crv = new PolyCurve();
        crv.Append(Line);
        ReleasePreview = new ReleasePreview(crv,
          ApiElement.OrientationAngle * Math.PI / 180.0, s, e);
      } else {
        ReleasePreview = new ReleasePreview();
      }
    }

    private GsaOffset GetOffSetFromApiElement() {
      return new GsaOffset(
        ApiElement.Offset.X1, ApiElement.Offset.X2, ApiElement.Offset.Y, ApiElement.Offset.Z);
    }

    private void SetOffsetInApiElement(GsaOffset offset) {
      ApiElement.Offset.X1 = offset.X1.Meters;
      ApiElement.Offset.X2 = offset.X2.Meters;
      ApiElement.Offset.Y = offset.Y.Meters;
      ApiElement.Offset.Z = offset.Z.Meters;
    }

    private void SetRelease(GsaBool6 bool6, int pos) {
      ApiElement.SetEndRelease(pos, new EndRelease(bool6.ApiBool6));
      UpdateReleasesPreview();
    }
  }
}
