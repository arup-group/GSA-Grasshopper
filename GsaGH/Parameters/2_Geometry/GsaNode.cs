using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Node class, this class defines the basic properties and methods for any Gsa Node
  /// </summary>
  public class GsaNode
  {
    #region properties
    public bool IsValid
    {
      get
      {
        if (API_Node == null) { return false; }
        return true;
      }
    }
    //public GsaSpring Spring
    //{
    //    get { return m_spring; }
    //    set { m_spring = value; }
    //}
    public int Id
    {
      get { return m_id; }
      set { m_id = value; }
    }
    public Plane LocalAxis
    {
      get { return m_plane; }
      set
      {
        m_plane = value;
        UpdatePreview();
      }
    }
    public bool isSupport
    {
      get
      {
        return m_node.Restraint.X || m_node.Restraint.Y || m_node.Restraint.Z ||
              m_node.Restraint.XX || m_node.Restraint.YY || m_node.Restraint.ZZ;
      }
    }
    public Point3d Point
    {
      get
      {
        if (m_node == null) { return Point3d.Unset; }
        return new Point3d(m_node.Position.X, m_node.Position.Y, m_node.Position.Z);
      }
      set
      {
        CloneApiNode();
        m_id = 0;
        m_node.Position.X = value.X;
        m_node.Position.Y = value.Y;
        m_node.Position.Z = value.Z;
        UpdatePreview();
      }
    }

    #endregion

    #region GsaAPI.Node members
    internal Node API_Node
    {
      get { return m_node; }
      set
      {
        m_node = value;
        UpdatePreview();
      }
    }
    public System.Drawing.Color Colour
    {
      get
      {
        return (System.Drawing.Color)m_node.Colour;
      }
      set
      {
        CloneApiNode();
        m_node.Colour = value;
      }
    }
    public GsaBool6 Restraint
    {
      get
      {
        return new GsaBool6(m_node.Restraint.X, m_node.Restraint.Y, m_node.Restraint.Z,
            m_node.Restraint.XX, m_node.Restraint.YY, m_node.Restraint.ZZ);
      }
      set
      {
        CloneApiNode();
        m_node.Restraint = new NodalRestraint
        {
          X = value.X,
          Y = value.Y,
          Z = value.Z,
          XX = value.XX,
          YY = value.YY,
          ZZ = value.ZZ,
        };
        UpdatePreview();
      }
    }
    public string Name
    {
      get { return m_node.Name; }
      set
      {
        CloneApiNode();
        m_node.Name = value;
      }
    }
    internal void CloneApiNode()
    {
      Node node = new Node
      {
        AxisProperty = m_node.AxisProperty,
        DamperProperty = m_node.DamperProperty,
        MassProperty = m_node.MassProperty,
        Name = m_node.Name.ToString(),
        Restraint = new NodalRestraint
        {
          X = m_node.Restraint.X,
          Y = m_node.Restraint.Y,
          Z = m_node.Restraint.Z,
          XX = m_node.Restraint.XX,
          YY = m_node.Restraint.YY,
          ZZ = m_node.Restraint.ZZ,
        },
        SpringProperty = m_node.SpringProperty,
        Position = new Vector3
        {
          X = m_node.Position.X,
          Y = m_node.Position.Y,
          Z = m_node.Position.Z
        }
      };

      if ((System.Drawing.Color)m_node.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        node.Colour = m_node.Colour;

      m_node = node;
    }
    internal Node GetApiNodeToUnit(LengthUnit unit = LengthUnit.Meter)
    {
      Node node = new Node
      {
        AxisProperty = m_node.AxisProperty,
        DamperProperty = m_node.DamperProperty,
        MassProperty = m_node.MassProperty,
        Name = m_node.Name.ToString(),
        Restraint = new NodalRestraint
        {
          X = m_node.Restraint.X,
          Y = m_node.Restraint.Y,
          Z = m_node.Restraint.Z,
          XX = m_node.Restraint.XX,
          YY = m_node.Restraint.YY,
          ZZ = m_node.Restraint.ZZ,
        },
        SpringProperty = m_node.SpringProperty,
        Position = new Vector3
        {
          X = (unit == LengthUnit.Meter) ? m_node.Position.X : new Length(m_node.Position.X, unit).Meters,
          Y = (unit == LengthUnit.Meter) ? m_node.Position.Y : new Length(m_node.Position.Y, unit).Meters,
          Z = (unit == LengthUnit.Meter) ? m_node.Position.Z : new Length(m_node.Position.Z, unit).Meters
        }
      };

      if ((System.Drawing.Color)m_node.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        node.Colour = m_node.Colour;

      return node;
    }
    #endregion

    #region preview
    internal Line previewXaxis;
    internal Line previewYaxis;
    internal Line previewZaxis;
    internal Brep previewSupportSymbol;
    internal Text3d previewText;

    internal void UpdatePreview()
    {
      if (m_node.Restraint.X || m_node.Restraint.Y || m_node.Restraint.Z ||
          m_node.Restraint.XX || m_node.Restraint.YY || m_node.Restraint.ZZ)
      {
        GsaGH.UI.Display.PreviewRestraint(Restraint, m_plane, Point,
            ref previewSupportSymbol, ref previewText);
      }
      else
      {
        previewSupportSymbol = null;
        previewText = null;
      }

      if (m_plane != null)
      {
        if (m_plane != Plane.WorldXY & m_plane != new Plane())
        {
          Plane local = m_plane.Clone();
          local.Origin = Point;

          previewXaxis = new Line(Point, local.XAxis, 0.5);
          previewYaxis = new Line(Point, local.YAxis, 0.5);
          previewZaxis = new Line(Point, local.ZAxis, 0.5);
        }
      }
    }
    #endregion
    #region fields
    private Plane m_plane;
    private int m_id;
    private Node m_node;
    #endregion

    #region constructors
    public GsaNode()
    {
      m_node = new Node();
    }

    internal GsaNode(Node node, int ID, LengthUnit unit, Plane localAxis = new Plane())
    {
      m_node = node;
      CloneApiNode();
      if (unit != LengthUnit.Meter)
      {
        m_node.Position.X = new Length(node.Position.X, LengthUnit.Meter).As(unit);
        m_node.Position.Y = new Length(node.Position.Y, LengthUnit.Meter).As(unit);
        m_node.Position.Z = new Length(node.Position.Z, LengthUnit.Meter).As(unit);
      }
      m_id = ID;
      m_plane = localAxis;
      UpdatePreview();
    }
    public GsaNode(Point3d position, int ID = 0)
    {
      m_node = new Node();
      Point = position;
      m_id = ID;
      UpdatePreview();
    }
    public GsaNode Duplicate(bool cloneApiNode = false)
    {
      if (m_node == null) { return null; }
      GsaNode dup = new GsaNode();
      dup.m_id = m_id;
      dup.m_node = m_node;
      if (cloneApiNode)
        dup.CloneApiNode();
      dup.m_plane = m_plane;
      //dup.m_spring = m_spring;
      dup.UpdatePreview();
      return dup;
    }
    #endregion

    #region methods
    public void UpdateUnit(LengthUnit unit)
    {
      if (unit != LengthUnit.Meter) // convert from meter to input unit if not meter
      {
        Vector3 pos = new Vector3();
        this.API_Node.Position.X = new Length(this.API_Node.Position.X, LengthUnit.Meter).As(unit);
        this.API_Node.Position.Y = new Length(this.API_Node.Position.Y, LengthUnit.Meter).As(unit);
        this.API_Node.Position.Z = new Length(this.API_Node.Position.Z, LengthUnit.Meter).As(unit);
      }
    }
    public override string ToString()
    {
      if (API_Node == null) { return "Null Node"; }
      string idd = " " + Id.ToString() + " ";
      if (Id == 0) { idd = " "; }
      GH_Point gH_Point = new GH_Point(Point);
      string nodeTxt = "GSA Node" + idd + gH_Point.ToString();

      string localTxt = "";
      Plane noPlane = new Plane() { Origin = new Point3d(0, 0, 0), XAxis = new Vector3d(0, 0, 0), YAxis = new Vector3d(0, 0, 0), ZAxis = new Vector3d(0, 0, 0) };
      if (LocalAxis != noPlane)
      {
        if (LocalAxis != Plane.WorldXY)
        {
          GH_Plane gH_Plane = new GH_Plane(LocalAxis);
          localTxt = " Local axis: {" + gH_Plane.ToString() + "}";
        }
      }

      string sptTxt;
      if (API_Node.Restraint.X == false && API_Node.Restraint.Y == false && API_Node.Restraint.Z == false &&
          API_Node.Restraint.XX == false && API_Node.Restraint.YY == false && API_Node.Restraint.ZZ == false)
        sptTxt = "";
      else
      {
        sptTxt = " Restraint: " + "X: " + (API_Node.Restraint.X ? "\u2713" : "\u2610") +
           ", Y: " + (API_Node.Restraint.Y ? "\u2713" : "\u2610") +
           ", Z: " + (API_Node.Restraint.Z ? "\u2713" : "\u2610") +
           ", XX: " + (API_Node.Restraint.XX ? "\u2713" : "\u2610") +
           ", YY: " + (API_Node.Restraint.YY ? "\u2713" : "\u2610") +
           ", ZZ: " + (API_Node.Restraint.ZZ ? "\u2713" : "\u2610");
        if (!API_Node.Restraint.X & !API_Node.Restraint.Y & !API_Node.Restraint.Z &
            !API_Node.Restraint.XX & !API_Node.Restraint.YY & !API_Node.Restraint.ZZ)
          sptTxt = "";
        if (API_Node.Restraint.X & API_Node.Restraint.Y & API_Node.Restraint.Z &
            !API_Node.Restraint.XX & !API_Node.Restraint.YY & !API_Node.Restraint.ZZ)
          sptTxt = " Restraint: Pinned";
        if (API_Node.Restraint.X & API_Node.Restraint.Y & API_Node.Restraint.Z &
            API_Node.Restraint.XX & API_Node.Restraint.YY & API_Node.Restraint.ZZ)
          sptTxt = " Restraint: Fixed";
      }

      return nodeTxt + sptTxt + localTxt;
    }
    #endregion
  }
}
