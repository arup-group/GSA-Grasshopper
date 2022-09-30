using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Element1d class, this class defines the basic properties and methods for any Gsa Element 1d
  /// </summary>
  public class GsaElement1d
  {
    public LineCurve Line
    {
      get { return m_line; }
      set
      {
        m_line = value;
        UpdatePreview();
      }
    }
    public int ID
    {
      get { return m_id; }
      set { m_id = value; }
    }
    public GsaBool6 ReleaseStart
    {
      get
      {
        return new GsaBool6(m_element.GetEndRelease(0).Releases);
      }
      set
      {
        m_rel1 = value;
        if (m_rel1 == null) { m_rel1 = new GsaBool6(); }
        CloneApiElement();
        m_element.SetEndRelease(0, new EndRelease(m_rel1.API_Bool6));
        UpdatePreview();
      }
    }
    public GsaBool6 ReleaseEnd
    {
      get
      {
        return new GsaBool6(m_element.GetEndRelease(1).Releases);
      }
      set
      {
        m_rel2 = value;
        if (m_rel2 == null) { m_rel2 = new GsaBool6(); }
        CloneApiElement();
        m_element.SetEndRelease(1, new EndRelease(m_rel2.API_Bool6));
        UpdatePreview();
      }
    }

    public GsaSection Section
    {
      get { return m_section; }
      set { m_section = value; }
    }

    #region GsaAPI.Element members
    internal Element API_Element
    {
      get { return m_element; }
      set { m_element = value; }
    }

    internal Element GetAPI_ElementClone()
    {
      Element elem = new Element()
      {
        Group = m_element.Group,
        IsDummy = m_element.IsDummy,
        Name = m_element.Name.ToString(),
        Offset = m_element.Offset,
        OrientationAngle = m_element.OrientationAngle,
        OrientationNode = m_element.OrientationNode,
        ParentMember = m_element.ParentMember,
        Property = m_element.Property,
        Topology = new ReadOnlyCollection<int>(m_element.Topology.ToList()),
        Type = m_element.Type //GsaToModel.Element1dType((int)Element.Type)
      };
      elem.SetEndRelease(0, m_element.GetEndRelease(0));
      elem.SetEndRelease(1, m_element.GetEndRelease(1));
      if ((System.Drawing.Color)m_element.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        elem.Colour = m_element.Colour;
      return elem;
    }

    public System.Drawing.Color Colour
    {
      get
      {
        return (System.Drawing.Color)m_element.Colour;
      }
      set
      {
        CloneApiElement();
        m_element.Colour = value;
      }
    }

    public int Group
    {
      get { return m_element.Group; }
      set
      {
        CloneApiElement();
        m_element.Group = value;
      }
    }

    public bool IsDummy
    {
      get { return m_element.IsDummy; }
      set
      {
        CloneApiElement();
        m_element.IsDummy = value;
      }
    }

    public string Name
    {
      get { return m_element.Name; }
      set
      {
        CloneApiElement();
        m_element.Name = value;
      }
    }
    public GsaOffset Offset
    {
      get
      {
        return new GsaOffset(
            m_element.Offset.X1,
            m_element.Offset.X2,
            m_element.Offset.Y,
            m_element.Offset.Z);
      }
      set
      {
        CloneApiElement();
        m_element.Offset.X1 = value.X1.Meters;
        m_element.Offset.X2 = value.X2.Meters;
        m_element.Offset.Y = value.Y.Meters;
        m_element.Offset.Z = value.Z.Meters;
      }
    }

    public double OrientationAngle
    {
      get { return m_element.OrientationAngle; }
      set
      {
        CloneApiElement();
        m_element.OrientationAngle = value;
      }
    }

    public GsaNode OrientationNode
    {
      get { return m_orientationNode; }
      set
      {
        CloneApiElement();
        m_orientationNode = value;
      }
    }

    public int ParentMember
    {
      get { return m_element.ParentMember.Member; }
    }

    public ElementType Type
    {
      get { return m_element.Type; }
      set
      {
        CloneApiElement();
        m_element.Type = value;
      }
    }

    internal void CloneApiElement()
    {
      m_element = this.GetAPI_ElementClone();
    }
    #endregion

    #region preview
    internal Point3d previewPointStart;
    internal Point3d previewPointEnd;
    #region preview lines
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

    internal List<Line> previewGreenLines;
    internal List<Line> previewRedLines;
    #endregion
    internal void UpdatePreview()
    {
      if (m_rel1 != null & m_rel2 != null)
      {
        if (m_rel1.X || m_rel1.Y || m_rel1.Z || m_rel1.XX || m_rel1.YY || m_rel1.ZZ ||
        m_rel2.X || m_rel2.Y || m_rel2.Z || m_rel2.XX || m_rel2.YY || m_rel2.ZZ)
        {
          #region add lines
          previewGreenLines = new List<Line>();
          previewGreenLines.Add(previewSX1);
          previewGreenLines.Add(previewSX2);
          previewGreenLines.Add(previewSY1);
          previewGreenLines.Add(previewSY2);
          previewGreenLines.Add(previewSY3);
          previewGreenLines.Add(previewSY4);
          previewGreenLines.Add(previewSZ1);
          previewGreenLines.Add(previewSZ2);
          previewGreenLines.Add(previewSZ3);
          previewGreenLines.Add(previewSZ4);
          previewGreenLines.Add(previewEX1);
          previewGreenLines.Add(previewEX2);
          previewGreenLines.Add(previewEY1);
          previewGreenLines.Add(previewEY2);
          previewGreenLines.Add(previewEY3);
          previewGreenLines.Add(previewEY4);
          previewGreenLines.Add(previewEZ1);
          previewGreenLines.Add(previewEZ2);
          previewGreenLines.Add(previewEZ3);
          previewGreenLines.Add(previewEZ4);
          previewRedLines = new List<Line>();
          previewRedLines.Add(previewSXX);
          previewRedLines.Add(previewSYY1);
          previewRedLines.Add(previewSYY2);
          previewRedLines.Add(previewSZZ1);
          previewRedLines.Add(previewSZZ2);
          previewRedLines.Add(previewEXX);
          previewRedLines.Add(previewEYY1);
          previewRedLines.Add(previewEYY2);
          previewRedLines.Add(previewEZZ1);
          previewRedLines.Add(previewEZZ2);
          #endregion
          PolyCurve crv = new PolyCurve();
          crv.Append(m_line);
          GsaGH.UI.Display.Preview1D(crv, m_element.OrientationAngle * Math.PI / 180.0, m_rel1, m_rel2,
              ref previewGreenLines, ref previewRedLines);
        }
        else
          previewGreenLines = null;
      }

      previewPointStart = m_line.PointAtStart;
      previewPointEnd = m_line.PointAtEnd;
    }
    #endregion

    #region fields
    private Element m_element;
    private LineCurve m_line;
    private int m_id = 0;
    private GsaBool6 m_rel1;
    private GsaBool6 m_rel2;
    private GsaSection m_section;
    private GsaNode m_orientationNode;
    internal Tuple<Vector3d, Vector3d, Vector3d> LocalAxes
    {
      get
      {
        PolyCurve crv = new PolyCurve();
        crv.Append(m_line);
        return UI.Display.GetLocalPlane(crv, crv.GetLength() / 2, m_element.OrientationAngle * Math.PI / 180.0);
      }
    }
    #endregion

    #region constructors
    public GsaElement1d()
    {
      m_element = new Element();
      m_line = new LineCurve();
      m_section = new GsaSection();
    }

    public GsaElement1d(LineCurve line, int prop = 0, int ID = 0, GsaNode orientationNode = null)
    {
      this.m_element = new Element
      {
        Type = ElementType.BEAM,
      };

      this.m_line = line;
      this.m_section = new GsaSection();
      this.m_id = ID;
      this.m_section.ID = prop;
      this.m_orientationNode = orientationNode;
      UpdatePreview();
    }

    internal GsaElement1d(Element elem, LineCurve line, int ID, GsaSection section, GsaNode orientationNode)
    {
      this.m_element = elem;
      this.m_line = line;
      this.m_rel1 = new GsaBool6(m_element.GetEndRelease(0).Releases);
      this.m_rel2 = new GsaBool6(m_element.GetEndRelease(1).Releases);
      this.m_id = ID;
      this.m_section = section;
      this.m_orientationNode = orientationNode;

      UpdatePreview();
    }

    public GsaElement1d Duplicate(bool cloneApiElement = false)
    {
      GsaElement1d dup = new GsaElement1d();
      dup.m_id = this.m_id;
      dup.m_element = this.m_element;
      if (cloneApiElement)
        dup.CloneApiElement();
      dup.m_line = (LineCurve)this.m_line.DuplicateShallow();
      if (m_rel1 != null)
        dup.m_rel1 = this.m_rel1.Duplicate();
      if (m_rel2 != null)
        dup.m_rel2 = this.m_rel2.Duplicate();
      dup.m_section = this.m_section.Duplicate();
      if (this.m_orientationNode != null)
        dup.m_orientationNode = this.m_orientationNode.Duplicate();
      UpdatePreview();
      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (m_line == null)
          return false;
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string idd = " " + ID.ToString();
      if (ID == 0) { idd = ""; }
      string valid = (this.IsValid) ? "" : "Invalid ";
      if (this.Line != null)
      {
        if (this.Line.GetLength() == 0) { valid = "Invalid "; }
      }
      return valid + "GSA 1D Element" + idd;
    }
    #endregion
  }
}
