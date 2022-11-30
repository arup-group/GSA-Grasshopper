using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Member1d class, this class defines the basic properties and methods for any Gsa Member 1d
  /// </summary>
  public class GsaMember1d
  {
    #region fields
    internal List<Line> previewGreenLines;
    internal List<Line> previewRedLines;

    private PolyCurve _crv = new PolyCurve(); // Polyline for visualisation /member1d/member2d
    private List<Point3d> _topo; // list of topology points for visualisation /member1d/member2d
    private List<string> _topoType; //list of polyline curve type (arch or line) for member1d/2d
    private GsaBool6 _rel1;
    private GsaBool6 _rel2;
    private GsaNode _orientationNode;

    private Line previewSX1;
    private Line previewSX2;
    private Line previewSY1;
    private Line previewSY2;
    private Line previewSY3;
    private Line previewSY4;
    private Line previewSZ1;
    private Line previewSZ2;
    private Line previewSZ3;
    private Line previewSZ4;
    private Line previewEX1;
    private Line previewEX2;
    private Line previewEY1;
    private Line previewEY2;
    private Line previewEY3;
    private Line previewEY4;
    private Line previewEZ1;
    private Line previewEZ2;
    private Line previewEZ3;
    private Line previewEZ4;
    private Line previewSXX;
    private Line previewSYY1;
    private Line previewSYY2;
    private Line previewSZZ1;
    private Line previewSZZ2;
    private Line previewEXX;
    private Line previewEYY1;
    private Line previewEYY2;
    private Line previewEZZ1;
    private Line previewEZZ2;
    #endregion

    #region properties
    public int Id { get; set; } = 0;
    internal Member ApiMember { get; set; } = new Member();
    public double MeshSize { get; set; } = 0;
    public GsaSection Section { get; set; } = new GsaSection();
    internal GsaLocalAxes LocalAxes { get; set; } = null;
    public List<Point3d> Topology => this._topo;
    public List<string> TopologyType => this._topoType;
    public PolyCurve PolyCurve
    {
      get
      {
        return this._crv;
      }
      set
      {
        Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Util.GH.Convert.ConvertMem1dCrv(value);
        this._crv = convertCrv.Item1;
        this._topo = convertCrv.Item2;
        this._topoType = convertCrv.Item3;
        this.UpdatePreview();
      }
    }
    public GsaBool6 ReleaseStart
    {
      get
      {
        return new GsaBool6(this.ApiMember.GetEndRelease(0).Releases);
      }
      set
      {
        this._rel1 = value;
        if (this._rel1 == null)
        {
          this._rel1 = new GsaBool6();
        }
        this.CloneApiObject();
        this.ApiMember.SetEndRelease(0, new EndRelease(this._rel1._bool6));
        this.UpdatePreview();
      }
    }
    public GsaBool6 ReleaseEnd
    {
      get
      {
        return new GsaBool6(this.ApiMember.GetEndRelease(1).Releases);
      }
      set
      {
        this._rel2 = value;
        if (this._rel2 == null)
        {
          this._rel2 = new GsaBool6();
        }
        this.CloneApiObject();
        this.ApiMember.SetEndRelease(1, new EndRelease(this._rel2._bool6));
        this.UpdatePreview();
      }
    }
    internal Member GetAPI_MemberClone()
    {
      Member mem = new Member
      {
        Group = this.ApiMember.Group,
        IsDummy = this.ApiMember.IsDummy,
        IsIntersector = this.ApiMember.IsIntersector,
        LateralTorsionalBucklingFactor = this.ApiMember.LateralTorsionalBucklingFactor,
        MeshSize = this.ApiMember.MeshSize,
        MomentAmplificationFactorStrongAxis = this.ApiMember.MomentAmplificationFactorStrongAxis,
        MomentAmplificationFactorWeakAxis = this.ApiMember.MomentAmplificationFactorWeakAxis,
        Name = this.ApiMember.Name.ToString(),
        Offset = this.ApiMember.Offset,
        OrientationAngle = this.ApiMember.OrientationAngle,
        OrientationNode = this.ApiMember.OrientationNode,
        Property = this.ApiMember.Property,
        Type = this.ApiMember.Type,
        Type1D = this.ApiMember.Type1D
      };
      if (this.ApiMember.Topology != String.Empty)
        mem.Topology = this.ApiMember.Topology;

      mem.MeshSize = this.MeshSize;

      mem.SetEndRelease(0, this.ApiMember.GetEndRelease(0));
      mem.SetEndRelease(1, this.ApiMember.GetEndRelease(1));

      if ((Color)this.ApiMember.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        mem.Colour = this.ApiMember.Colour;

      return mem;
    }
    public Color Colour
    {
      get
      {
        return (Color)this.ApiMember.Colour;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Colour = value;
        this.UpdatePreview();
      }
    }
    public int Group
    {
      get
      {
        return this.ApiMember.Group;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Group = value;
      }
    }
    public bool IsDummy
    {
      get
      {
        return this.ApiMember.IsDummy;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.IsDummy = value;
        this.UpdatePreview();
      }
    }
    public string Name
    {
      get
      {
        return this.ApiMember.Name;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Name = value;
      }
    }
    public bool MeshWithOthers
    {
      get
      {
        return this.ApiMember.IsIntersector;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.IsIntersector = value;
      }
    }
    public GsaOffset Offset
    {
      get
      {
        return new GsaOffset(this.ApiMember.Offset.X1, this.ApiMember.Offset.X2, this.ApiMember.Offset.Y, this.ApiMember.Offset.Z);
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Offset.X1 = value.X1.Meters;
        this.ApiMember.Offset.X2 = value.X2.Meters;
        this.ApiMember.Offset.Y = value.Y.Meters;
        this.ApiMember.Offset.Z = value.Z.Meters;
      }
    }
    public Angle OrientationAngle
    {
      get
      {
        return new Angle(this.ApiMember.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.OrientationAngle = value.Degrees;
      }
    }
    public GsaNode OrientationNode
    {
      get
      {
        return this._orientationNode;
      }
      set
      {
        this.CloneApiObject();
        this._orientationNode = value;
      }
    }
    public MemberType Type
    {
      get
      {
        return this.ApiMember.Type;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Type = value;
      }
    }
    public ElementType Type1D
    {
      get
      {
        return this.ApiMember.Type1D;
      }
      set
      {
        this.CloneApiObject();
        this.ApiMember.Type1D = value;
      }
    }
    #endregion

    #region constructors
    public GsaMember1d()
    {
    }

    internal GsaMember1d(Member member, LengthUnit modelUnit, int id, List<Point3d> topology, List<string> topo_type, GsaSection section, GsaNode orientationNode)
    {
      this.ApiMember = member;
      // scale mesh size to model units
      this.MeshSize = new Length(member.MeshSize, LengthUnit.Meter).As(modelUnit);
      this.Id = id;
      this._crv = Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(topology, topo_type);
      this._topo = topology;
      this._topoType = topo_type;
      this.Section = section;
      this._rel1 = new GsaBool6(this.ApiMember.GetEndRelease(0).Releases);
      this._rel2 = new GsaBool6(this.ApiMember.GetEndRelease(1).Releases);
      // is this check necessary?
      if (orientationNode != null)
        this._orientationNode = orientationNode;
      this.UpdatePreview();
    }

    public GsaMember1d(Curve crv, int prop = 0)
    {
      this.ApiMember = new Member
      {
        Type = MemberType.GENERIC_1D,
        Property = prop
      };
      Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Util.GH.Convert.ConvertMem1dCrv(crv);
      this._crv = convertCrv.Item1;
      this._topo = convertCrv.Item2;
      this._topoType = convertCrv.Item3;

      this.UpdatePreview();
    }
    #endregion

    #region methods
    public GsaMember1d Duplicate(bool cloneApiMember = false)
    {
      GsaMember1d dup = new GsaMember1d();
      dup.Id = this.Id;
      dup.MeshSize = this.MeshSize;
      dup.ApiMember = this.ApiMember;
      dup._localAxes = this._localAxes;
      if (cloneApiMember)
        dup.CloneApiObject();
      dup._crv = (PolyCurve)this._crv.DuplicateShallow();
      if (this._rel1 != null)
        dup._rel1 = this._rel1.Duplicate();
      if (this._rel2 != null)
        dup._rel2 = this._rel2.Duplicate();
      dup.Section = this.Section.Duplicate();
      dup._topo = this._topo;
      dup._topoType = this._topoType;
      if (this._orientationNode != null)
        dup._orientationNode = this._orientationNode.Duplicate(cloneApiMember);
      dup.UpdatePreview();
      return dup;
    }

    public GsaMember1d Transform(Transform xform)
    {
      GsaMember1d dup = this.Duplicate(true);
      dup.Id = 0;
      dup.LocalAxes = null;

      List<Point3d> pts = this._topo.ToList();
      Point3dList xpts = new Point3dList(pts);
      xpts.Transform(xform);
      dup._topo = xpts.ToList();

      if (this._crv != null)
      {
        PolyCurve crv = this._crv.DuplicatePolyCurve();
        crv.Transform(xform);
        dup._crv = crv;
      }
      dup.UpdatePreview();
      return dup;
    }

    public GsaMember1d Morph(SpaceMorph xmorph)
    {
      GsaMember1d dup = this.Duplicate(true);
      dup.Id = 0;
      dup.LocalAxes = null;

      List<Point3d> pts = this._topo.ToList();
      for (int i = 0; i < pts.Count; i++)
        pts[i] = xmorph.MorphPoint(pts[i]);
      dup._topo = pts;

      if (this._crv != null)
      {
        PolyCurve crv = this._crv.DuplicatePolyCurve();
        xmorph.Morph(crv);
        dup._crv = crv;
      }
      dup.UpdatePreview();
      return dup;
    }

    public override string ToString()
    {
      string idd = this.Id == 0 ? "" : "ID:" + Id + " ";
      string type = Helpers.Mappings.MemberTypeMapping.FirstOrDefault(x => x.Value == this.Type).Key + " ";
      string pb = this.Section.Id > 0 ? "PB" + this.Section.Id : this.Section.Profile;
      return string.Join(" ", idd.Trim(), type.Trim(), pb.Trim()).Trim().Replace("  ", " ");
    }

    internal void CloneApiObject()
    {
      this.ApiMember = this.GetAPI_MemberClone();
    }

    private void UpdatePreview()
    {
      if (this._rel1 != null & this._rel2 != null)
      {
        if (this._rel1.X || this._rel1.Y || this._rel1.Z || this._rel1.XX || this._rel1.YY || this._rel1.ZZ || this._rel2.X || this._rel2.Y || this._rel2.Z || this._rel2.XX || this._rel2.YY || this._rel2.ZZ)
        {
          previewGreenLines = new List<Line>
          {
            previewSX1,
            previewSX2,
            previewSY1,
            previewSY2,
            previewSY3,
            previewSY4,
            previewSZ1,
            previewSZ2,
            previewSZ3,
            previewSZ4,
            previewEX1,
            previewEX2,
            previewEY1,
            previewEY2,
            previewEY3,
            previewEY4,
            previewEZ1,
            previewEZ2,
            previewEZ3,
            previewEZ4
          };
          previewRedLines = new List<Line>
          {
            previewSXX,
            previewSYY1,
            previewSYY2,
            previewSZZ1,
            previewSZZ2,
            previewEXX,
            previewEYY1,
            previewEYY2,
            previewEZZ1,
            previewEZZ2
          };
          GsaGH.UI.Display.Preview1D(this._crv, this.ApiMember.OrientationAngle * Math.PI / 180.0, this._rel1, this._rel2,
          ref previewGreenLines, ref previewRedLines);
        }
        else
          previewGreenLines = null;
      }
    }
    #endregion
  }
}
