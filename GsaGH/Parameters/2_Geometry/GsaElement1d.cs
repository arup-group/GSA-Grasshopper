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
using GsaGH.Helpers.GsaApi;
using GsaGH.Components;
using GH_IO.Serialization;
using System.Collections.Concurrent;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Element1d class, this class defines the basic properties and methods for any Gsa Element 1d
  /// </summary>
  public class GsaElement1d
  {
    #region fields
    internal List<Line> PreviewGreenLines;
    internal List<Line> PreviewRedLines;

    private int _id = 0;
    private Guid _guid = Guid.NewGuid();
    private LineCurve _line = new LineCurve();
    private GsaBool6 _rel1;
    private GsaBool6 _rel2;
    private GsaNode _orientationNode;
    #endregion

    #region properties
    public int Id
    {
      get
      {
        return _id;
      }
      set
      {
        this.CloneApiObject();
        _id = value;
      }
    }
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
        this._guid = Guid.NewGuid();
        this.UpdatePreview();
      }
    }
    public GsaBool6 ReleaseStart
    {
      get
      {
        return new GsaBool6(this.ApiElement.GetEndRelease(0).Releases);
      }
      set
      {
        this._rel1 = value;
        if (this._rel1 == null)
        {
          this._rel1 = new GsaBool6();
        }
        this.CloneApiObject();
        this.ApiElement.SetEndRelease(0, new EndRelease(this._rel1._bool6));
        this.UpdatePreview();
      }
    }
    public GsaBool6 ReleaseEnd
    {
      get
      {
        return new GsaBool6(this.ApiElement.GetEndRelease(1).Releases);
      }
      set
      {
        this._rel2 = value;
        if (this._rel2 == null)
        {
          this._rel2 = new GsaBool6();
        }
        this.CloneApiObject();
        this.ApiElement.SetEndRelease(1, new EndRelease(this._rel2._bool6));
        this.UpdatePreview();
      }
    }
    public GsaSection Section { get; set; } = new GsaSection();
    internal Element ApiElement { get; set; } = new Element();

    public Color Colour
    {
      get
      {
        return (Color)this.ApiElement.Colour;
      }
      set
      {
        this.CloneApiObject();
        this.ApiElement.Colour = value;
      }
    }
    public int Group
    {
      get
      {
        return this.ApiElement.Group;
      }
      set
      {
        this.CloneApiObject();
        this.ApiElement.Group = value;
      }
    }
    public bool IsDummy
    {
      get
      {
        return this.ApiElement.IsDummy;
      }
      set
      {
        this.CloneApiObject();
        this.ApiElement.IsDummy = value;
      }
    }
    public string Name
    {
      get
      {
        return this.ApiElement.Name;
      }
      set
      {
        this.CloneApiObject();
        this.ApiElement.Name = value;
      }
    }
    public GsaOffset Offset
    {
      get
      {
        return new GsaOffset(this.ApiElement.Offset.X1, this.ApiElement.Offset.X2, this.ApiElement.Offset.Y, this.ApiElement.Offset.Z);
      }
      set
      {
        this.CloneApiObject();
        this.ApiElement.Offset.X1 = value.X1.Meters;
        this.ApiElement.Offset.X2 = value.X2.Meters;
        this.ApiElement.Offset.Y = value.Y.Meters;
        this.ApiElement.Offset.Z = value.Z.Meters;
      }
    }
    public Angle OrientationAngle
    {
      get
      {
        return new Angle(this.ApiElement.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian);
      }
      set
      {
        this.CloneApiObject();
        this.ApiElement.OrientationAngle = value.Degrees;
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
        return this.ApiElement.ParentMember.Member;
      }
    }
    public ElementType Type
    {
      get
      {
        return this.ApiElement.Type;
      }
      set
      {
        this.CloneApiObject();
        this.ApiElement.Type = value;
      }
    }
    public Guid Guid
    {
      get
      {
        return this._guid;
      }
    }
    #endregion

    #region constructors
    public GsaElement1d()
    {
    }

    public GsaElement1d(LineCurve line, int prop = 0, int id = 0, GsaNode orientationNode = null)
    {
      this.ApiElement = new Element
      {
        Type = ElementType.BEAM,
      };
      this._line = line;
      this.Id = Id;
      this.Section.Id = prop;
      this._orientationNode = orientationNode;
      this.UpdatePreview();
    }

    internal GsaElement1d(Element elem, LineCurve line, int id, GsaSection section, GsaNode orientationNode)
    {
      this.ApiElement = elem;
      this._line = line;
      this._rel1 = new GsaBool6(ApiElement.GetEndRelease(0).Releases);
      this._rel2 = new GsaBool6(ApiElement.GetEndRelease(1).Releases);
      this.Id = id;
      this.Section = section;
      this._orientationNode = orientationNode;
      this.UpdatePreview();
    }

    internal GsaElement1d(ReadOnlyDictionary<int, Element> eDict, int id, ReadOnlyDictionary<int, Node> nDict,
      ReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, SectionModifier> modDict, ReadOnlyDictionary<int, AnalysisMaterial> matDict,
      Dictionary<int, ReadOnlyCollection<double>> localAxesDict, LengthUnit modelUnit)
    {
      this.Id = id;
      this.ApiElement = eDict[id];
      this._rel1 = new GsaBool6(ApiElement.GetEndRelease(0).Releases);
      this._rel2 = new GsaBool6(ApiElement.GetEndRelease(1).Releases);
      if (this.ApiElement.OrientationNode > 0)
        this._orientationNode = new GsaNode(Helpers.Import.Nodes.Point3dFromNode(nDict[this.ApiElement.OrientationNode], modelUnit));
      this._line = new LineCurve(new Line(
        Helpers.Import.Nodes.Point3dFromNode(nDict[this.ApiElement.Topology[0]], modelUnit),
        Helpers.Import.Nodes.Point3dFromNode(nDict[this.ApiElement.Topology[1]], modelUnit)));
      this.LocalAxes = new GsaLocalAxes(localAxesDict[id]);
      this.Section = new GsaSection(sDict, this.ApiElement.Property, modDict, matDict);
      this.UpdatePreview();
    }
    #endregion

    #region methods
    public GsaElement1d Duplicate(bool cloneApiElement = false)
    {
      GsaElement1d dup = new GsaElement1d();
      dup.Id = this.Id;
      dup.ApiElement = this.ApiElement;
      dup.LocalAxes = this.LocalAxes;
      dup._guid = new Guid(_guid.ToString());
      if (cloneApiElement)
        dup.CloneApiObject();
      dup._line = (LineCurve)this._line.DuplicateShallow();
      if (_rel1 != null)
        dup._rel1 = this._rel1.Duplicate();
      if (_rel2 != null)
        dup._rel2 = this._rel2.Duplicate();
      dup.Section = this.Section.Duplicate();
      if (this._orientationNode != null)
        dup._orientationNode = this._orientationNode.Duplicate();
      this.UpdatePreview();
      return dup;
    }

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

    public override string ToString()
    {
      string idd = this.Id == 0 ? "" : "ID:" + Id + " ";
      string type = Mappings.ElementTypeMapping.FirstOrDefault(x => x.Value == this.Type).Key + " ";
      string pb = this.Section.Id > 0 ? "PB" + this.Section.Id : this.Section.Profile;
      return string.Join(" ", idd.Trim(), type.Trim(), pb.Trim()).Trim().Replace("  ", " ");
    }

    internal void CloneApiObject()
    {
      this.ApiElement = this.GetApiElementClone();
      this._guid = Guid.NewGuid();
    }

    internal Element GetApiElementClone()
    {
      Element elem = new Element()
      {
        Group = this.ApiElement.Group,
        IsDummy = this.ApiElement.IsDummy,
        Name = this.ApiElement.Name.ToString(),
        Offset = this.ApiElement.Offset,
        OrientationAngle = this.ApiElement.OrientationAngle,
        OrientationNode = this.ApiElement.OrientationNode,
        ParentMember = this.ApiElement.ParentMember,
        Property = this.ApiElement.Property,
        Type = this.ApiElement.Type //GsaToModel.Element1dType((int)Element.Type)
      };
      elem.Topology = new ReadOnlyCollection<int>(this.ApiElement.Topology.ToList());
      elem.SetEndRelease(0, this.ApiElement.GetEndRelease(0));
      elem.SetEndRelease(1, this.ApiElement.GetEndRelease(1));
      if ((Color)ApiElement.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        elem.Colour = this.ApiElement.Colour;
      return elem;
    }

    internal void UpdatePreview()
    {
      if (this._rel1 != null & this._rel2 != null)
      {
        if (this._rel1.X || this._rel1.Y || _rel1.Z || _rel1.XX || this._rel1.YY || this._rel1.ZZ || this._rel2.X || this._rel2.Y || this._rel2.Z || this._rel2.XX || this._rel2.YY || this._rel2.ZZ)
        {
          PolyCurve crv = new PolyCurve();
          crv.Append(this._line);
          Tuple<List<Line>, List<Line>> previewCurves = Helpers.Graphics.Display.Preview1D(crv, this.ApiElement.OrientationAngle * Math.PI / 180.0, this._rel1, this._rel2);
          PreviewGreenLines = previewCurves.Item1;
          PreviewRedLines = previewCurves.Item2;
        }
        else
          PreviewGreenLines = null;
      }
    }
    #endregion
  }
}
