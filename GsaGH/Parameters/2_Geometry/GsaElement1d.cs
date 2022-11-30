using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using OasysUnits.Units;
using OasysUnits;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Element1d class, this class defines the basic properties and methods for any Gsa Element 1d
  /// </summary>
  public class GsaElement1d
  {
    #region fields
    internal Point3d previewPointStart;
    internal Point3d previewPointEnd;
    internal List<Line> previewGreenLines;
    internal List<Line> previewRedLines;

    private Element _element = new Element();
    private LineCurve _line = new LineCurve();
    private GsaBool6 _rel1;
    private GsaBool6 _rel2;
    private GsaSection _section = new GsaSection();
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
    internal GsaLocalAxes LocalAxes { get; set; } = null;
    public LineCurve Line
    {
      get
      {
        return this._line;
      }
      set
      {
        this._line = value;
        this.UpdatePreview();
      }
    }
    public GsaBool6 ReleaseStart
    {
      get
      {
        return new GsaBool6(this._element.GetEndRelease(0).Releases);
      }
      set
      {
        this._rel1 = value;
        if (this._rel1 == null)
        {
          this._rel1 = new GsaBool6();
        }
        this.CloneApiObject();
        this._element.SetEndRelease(0, new EndRelease(this._rel1._bool6));
        this.UpdatePreview();
      }
    }
    public GsaBool6 ReleaseEnd
    {
      get
      {
        return new GsaBool6(this._element.GetEndRelease(1).Releases);
      }
      set
      {
        this._rel2 = value;
        if (this._rel2 == null)
        {
          this._rel2 = new GsaBool6();
        }
        this.CloneApiObject();
        this._element.SetEndRelease(1, new EndRelease(this._rel2._bool6));
        this.UpdatePreview();
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
    internal Element API_Element
    {
      get
      {
        return this._element;
      }
      set
      {
        this._element = value;
      }
    }
    public Color Colour
    {
      get
      {
        return (Color)this._element.Colour;
      }
      set
      {
        this.CloneApiObject();
        this._element.Colour = value;
      }
    }
    public int Group
    {
      get
      {
        return this._element.Group;
      }
      set
      {
        this.CloneApiObject();
        this._element.Group = value;
      }
    }
    public bool IsDummy
    {
      get
      {
        return this._element.IsDummy;
      }
      set
      {
        this.CloneApiObject();
        this._element.IsDummy = value;
      }
    }
    public string Name
    {
      get
      {
        return this._element.Name;
      }
      set
      {
        this.CloneApiObject();
        this._element.Name = value;
      }
    }
    public GsaOffset Offset
    {
      get
      {
        return new GsaOffset(this._element.Offset.X1, this._element.Offset.X2, this._element.Offset.Y, this._element.Offset.Z);
      }
      set
      {
        this.CloneApiObject();
        this._element.Offset.X1 = value.X1.Meters;
        this._element.Offset.X2 = value.X2.Meters;
        this._element.Offset.Y = value.Y.Meters;
        this._element.Offset.Z = value.Z.Meters;
      }
    }
    public Angle OrientationAngle
    {
      get
      {
        return new Angle(this._element.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      }
      set
      {
        this.CloneApiObject();
        this._element.OrientationAngle = value.Degrees;
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
    public int ParentMember
    {
      get
      {
        return this._element.ParentMember.Member;
      }
    }
    public ElementType Type
    {
      get
      {
        return this._element.Type;
      }
      set
      {
        this.CloneApiObject();
        this._element.Type = value;
      }
    }
    #endregion

    #region constructors
    public GsaElement1d()
    {
    }

    public GsaElement1d(LineCurve line, int prop = 0, int id = 0, GsaNode orientationNode = null)
    {
      this._element = new Element
      {
        Type = ElementType.BEAM,
      };
      this._line = line;
      this.Id = Id;
      this._section.Id = prop;
      this._orientationNode = orientationNode;
      this.UpdatePreview();
    }

    internal GsaElement1d(Element elem, LineCurve line, int id, GsaSection section, GsaNode orientationNode)
    {
      this._element = elem;
      this._line = line;
      this._rel1 = new GsaBool6(_element.GetEndRelease(0).Releases);
      this._rel2 = new GsaBool6(_element.GetEndRelease(1).Releases);
      this.Id = Id;
      this._section = section;
      this._orientationNode = orientationNode;
      this.UpdatePreview();
    }
    #endregion

    #region methods
    public GsaElement1d Duplicate(bool cloneApiElement = false)
    {
      GsaElement1d dup = new GsaElement1d();
      dup.Id = this.Id;
      dup._element = this._element;
      dup.LocalAxes = this.LocalAxes;
      if (cloneApiElement)
        dup.CloneApiObject();
      dup._line = (LineCurve)this._line.DuplicateShallow();
      if (_rel1 != null)
        dup._rel1 = this._rel1.Duplicate();
      if (_rel2 != null)
        dup._rel2 = this._rel2.Duplicate();
      dup._section = this._section.Duplicate();
      if (this._orientationNode != null)
        dup._orientationNode = this._orientationNode.Duplicate();
      this.UpdatePreview();
      return dup;
    }

    public override string ToString()
    {
      string idd = this.Id == 0 ? "" : "ID:" + Id + " ";
      string type = Helpers.Mappings.ElementTypeMapping.FirstOrDefault(x => x.Value == this.Type).Key + " ";
      string pb = this._section.Id > 0 ? "PB" + this._section.Id : this._section.Profile;
      return string.Join(" ", idd.Trim(), type.Trim(), pb.Trim()).Trim().Replace("  ", " ");
    }

    internal void CloneApiObject()
    {
      this._element = this.GetAPI_ElementClone();
    }

    internal Element GetAPI_ElementClone()
    {
      Element elem = new Element()
      {
        Group = this._element.Group,
        IsDummy = this._element.IsDummy,
        Name = this._element.Name.ToString(),
        Offset = this._element.Offset,
        OrientationAngle = this._element.OrientationAngle,
        OrientationNode = this._element.OrientationNode,
        ParentMember = this._element.ParentMember,
        Property = this._element.Property,
        Topology = new ReadOnlyCollection<int>(this._element.Topology.ToList()),
        Type = this._element.Type //GsaToModel.Element1dType((int)Element.Type)
      };
      elem.SetEndRelease(0, this._element.GetEndRelease(0));
      elem.SetEndRelease(1, this._element.GetEndRelease(1));
      if ((Color)_element.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        elem.Colour = this._element.Colour;
      return elem;
    }

    internal void UpdatePreview()
    {
      if (this._rel1 != null & this._rel2 != null)
      {
        if (this._rel1.X || this._rel1.Y || _rel1.Z || _rel1.XX || this._rel1.YY || this._rel1.ZZ || this._rel2.X || this._rel2.Y || this._rel2.Z || this._rel2.XX || this._rel2.YY || this._rel2.ZZ)
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
          PolyCurve crv = new PolyCurve();
          crv.Append(this._line);
          GsaGH.UI.Display.Preview1D(crv, _element.OrientationAngle * Math.PI / 180.0, this._rel1, this._rel2, ref previewGreenLines, ref previewRedLines);
        }
        else
          previewGreenLines = null;
      }

      previewPointStart = this._line.PointAtStart;
      previewPointEnd = this._line.PointAtEnd;
    }
    #endregion

    #region transformation methods
    public GsaElement1d Transform(Transform xform)
    {
      GsaElement1d elem = this.Duplicate(true);
      elem.Id = 0;
      elem.LocalAxes = null;

      LineCurve xLn = elem.Line;
      xLn.Transform(xform);
      elem.Line = xLn;

      return elem;
    }

    public GsaElement1d Morph(SpaceMorph xmorph)
    {
      GsaElement1d elem = this.Duplicate(true);
      elem.Id = 0;
      elem.LocalAxes = null;

      LineCurve xLn = this.Line;
      xmorph.Morph(xLn);
      elem.Line = xLn;

      return elem;
    }
    #endregion
  }
}
