using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH.Units;
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

    private int _id = 0;
    private Member _member = new Member();
    private GsaSection _section = new GsaSection();

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
    public int ID
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
    public List<Point3d> Topology
    {
      get
      {
        return this._topo;
      }
    }
    public List<string> TopologyType
    {
      get
      {
        return this._topoType;
      }
    }
    public GsaBool6 ReleaseStart
    {
      get
      {
        return new GsaBool6(this._member.GetEndRelease(0).Releases);
      }
      set
      {
        this._rel1 = value;
        if (this._rel1 == null)
        {
          this._rel1 = new GsaBool6();
        }
        this.CloneApiObject();
        this._member.SetEndRelease(0, new EndRelease(this._rel1._bool6));
        this.UpdatePreview();
      }
    }
    public GsaBool6 ReleaseEnd
    {
      get
      {
        return new GsaBool6(this._member.GetEndRelease(1).Releases);
      }
      set
      {
        this._rel2 = value;
        if (this._rel2 == null)
        {
          this._rel2 = new GsaBool6();
        }
        this.CloneApiObject();
        this._member.SetEndRelease(1, new EndRelease(this._rel2._bool6));
        this.UpdatePreview();
      }
    }
    internal Tuple<Vector3d, Vector3d, Vector3d> LocalAxes
    {
      get
      {
        return UI.Display.GetLocalPlane(this._crv, this._crv.GetLength() / 2, this._member.OrientationAngle * Math.PI / 180.0);
      }
    }
    public GsaSection Section
    {
      get
      {
        return this._section;
      }
      set
      {
        this._section = value;
      }
    }
    #region GsaAPI.Member members
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
    internal Member GetAPI_MemberClone()
    {
      Member mem = new Member
      {
        Group = this._member.Group,
        IsDummy = this._member.IsDummy,
        IsIntersector = this._member.IsIntersector,
        LateralTorsionalBucklingFactor = this._member.LateralTorsionalBucklingFactor,
        MeshSize = this._member.MeshSize,
        MomentAmplificationFactorStrongAxis = this._member.MomentAmplificationFactorStrongAxis,
        MomentAmplificationFactorWeakAxis = this._member.MomentAmplificationFactorWeakAxis,
        Name = this._member.Name.ToString(),
        Offset = this._member.Offset,
        OrientationAngle = this._member.OrientationAngle,
        OrientationNode = this._member.OrientationNode,
        Property = this._member.Property,
        Type = this._member.Type,
        Type1D = this._member.Type1D
      };
      if (this._member.Topology != String.Empty)
        mem.Topology = this._member.Topology;

      mem.SetEndRelease(0, this._member.GetEndRelease(0));
      mem.SetEndRelease(1, this._member.GetEndRelease(1));

      if ((Color)this._member.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        mem.Colour = this._member.Colour;

      return mem;
    }
    public Color Colour
    {
      get
      {
        return (Color)this._member.Colour;
      }
      set
      {
        this.CloneApiObject();
        this._member.Colour = value;
        this.UpdatePreview();
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
        this.CloneApiObject();
        this._member.Group = value;
      }
    }
    public bool IsDummy
    {
      get
      {
        return this._member.IsDummy;
      }
      set
      {
        this.CloneApiObject();
        this._member.IsDummy = value;
        this.UpdatePreview();
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
        this.CloneApiObject();
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
        this.CloneApiObject();
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
        this.CloneApiObject();
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
        this.CloneApiObject();
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
        this.CloneApiObject();
        this._member.OrientationAngle = value;
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
        return this._member.Type;
      }
      set
      {
        this.CloneApiObject();
        this._member.Type = value;
      }
    }
    public ElementType Type1D
    {
      get
      {
        return this._member.Type1D;
      }
      set
      {
        this.CloneApiObject();
        this._member.Type1D = value;
      }
    }
    #endregion
    #endregion

    #region constructors
    public GsaMember1d()
    {
    }

    internal GsaMember1d(Member member, int id, List<Point3d> topology, List<string> topo_type, GsaSection section, GsaNode orientationNode)
    {
      this._member = member;
      this._id = id;
      this._crv = Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(topology, topo_type);
      this._topo = topology;
      this._topoType = topo_type;
      this._section = section;
      this._rel1 = new GsaBool6(this._member.GetEndRelease(0).Releases);
      this._rel2 = new GsaBool6(this._member.GetEndRelease(1).Releases);
      if (orientationNode != null)
        this._orientationNode = orientationNode;
      this.UpdatePreview();
    }

    public GsaMember1d(Curve crv, int prop = 0)
    {
      this._member = new Member
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
      dup._id = this._id;
      dup._member = this._member;
      if (cloneApiMember)
        dup.CloneApiObject();
      dup._crv = (PolyCurve)this._crv.DuplicateShallow();
      if (this._rel1 != null)
        dup._rel1 = this._rel1.Duplicate();
      if (this._rel2 != null)
        dup._rel2 = this._rel2.Duplicate();
      dup._section = this._section.Duplicate();
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
      dup.ID = 0;

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
      dup.ID = 0;

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
      string idd = this.ID == 0 ? "" : "ID:" + ID + " ";
      string type = Helpers.Mappings.memberTypeMapping.FirstOrDefault(x => x.Value == this.Type).Key + " ";
      return idd + type + new GH_Curve(this.PolyCurve).ToString();
    }

    internal void CloneApiObject()
    {
      this._member = this.GetAPI_MemberClone();
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
          GsaGH.UI.Display.Preview1D(this._crv, this._member.OrientationAngle * Math.PI / 180.0, this._rel1, this._rel2,
          ref previewGreenLines, ref previewRedLines);
        }
        else
          previewGreenLines = null;
      }
    }
    #endregion
  }
}
