using System.Drawing;
using GH_IO.Serialization;
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
    #region fields
    internal Line previewXaxis;
    internal Line previewYaxis;
    internal Line previewZaxis;
    internal Brep previewSupportSymbol;
    internal Text3d previewText;

    private int _id;
    private Plane _plane;
    private Node _node = new Node();
    #endregion

    #region properties
    //public GsaSpring Spring
    //{
    //    get { return m_spring; }
    //    set { m_spring = value; }
    //}
    public int ID
    {
      get
      {
        return _id;
      }
      set
      {
        _id = value;
      }
    }
    public Plane LocalAxis
    {
      get
      {
        return _plane;
      }
      set
      {
        _plane = value;
        this.UpdatePreview();
      }
    }
    public bool IsSupport
    {
      get
      {
        return _node.Restraint.X || _node.Restraint.Y || _node.Restraint.Z || _node.Restraint.XX || _node.Restraint.YY || _node.Restraint.ZZ;
      }
    }
    public Point3d Point
    {
      get
      {
        if (_node == null)
        {
          return Point3d.Unset;
        }
        return new Point3d(_node.Position.X, _node.Position.Y, _node.Position.Z);
      }
      set
      {
        CloneApiNode();
        _id = 0;
        _node.Position.X = value.X;
        _node.Position.Y = value.Y;
        _node.Position.Z = value.Z;
        this.UpdatePreview();
      }
    }
    #region GsaAPI.Node members
    internal Node API_Node
    {
      get
      {
        return _node;
      }
      set
      {
        _node = value;
        this.UpdatePreview();
      }
    }
    public Color Colour
    {
      get
      {
        return (Color)_node.Colour;
      }
      set
      {
        CloneApiNode();
        _node.Colour = value;
      }
    }
    public GsaBool6 Restraint
    {
      get
      {
        return new GsaBool6(_node.Restraint.X, _node.Restraint.Y, _node.Restraint.Z, _node.Restraint.XX, _node.Restraint.YY, _node.Restraint.ZZ);
      }
      set
      {
        CloneApiNode();
        _node.Restraint = new NodalRestraint
        {
          X = value.X,
          Y = value.Y,
          Z = value.Z,
          XX = value.XX,
          YY = value.YY,
          ZZ = value.ZZ,
        };
        this.UpdatePreview();
      }
    }
    public string Name
    {
      get
      {
        return _node.Name;
      }
      set
      {
        CloneApiNode();
        _node.Name = value;
      }
    }
    #endregion
    #endregion

    #region constructors
    public GsaNode()
    {
    }

    internal GsaNode(Node node, int id, LengthUnit unit, Plane localAxis = new Plane())
    {
      _node = node;
      CloneApiNode();
      if (unit != LengthUnit.Meter)
      {
        _node.Position.X = new Length(node.Position.X, LengthUnit.Meter).As(unit);
        _node.Position.Y = new Length(node.Position.Y, LengthUnit.Meter).As(unit);
        _node.Position.Z = new Length(node.Position.Z, LengthUnit.Meter).As(unit);
      }
      _id = id;
      _plane = localAxis;
      this.UpdatePreview();
    }

    public GsaNode(Point3d position, int id = 0)
    {
      Point = position;
      _id = id;
      this.UpdatePreview();
    }
    #endregion

    #region methods
    public GsaNode Duplicate(bool cloneApiNode = false)
    {
      GsaNode dup = new GsaNode();
      dup._id = _id;
      dup._node = _node;
      if (cloneApiNode)
        dup.CloneApiNode();
      dup._plane = _plane;
      dup.UpdatePreview();
      return dup;
    }

    public override string ToString()
    {
      if (API_Node == null) { return "Null"; }
      string idd = this.ID == 0 ? "" : "ID:" + ID + " ";

      string sptTxt;
      if (API_Node.Restraint.X == false && API_Node.Restraint.Y == false && API_Node.Restraint.Z == false &&
          API_Node.Restraint.XX == false && API_Node.Restraint.YY == false && API_Node.Restraint.ZZ == false)
        sptTxt = "";
      else if (API_Node.Restraint.X & API_Node.Restraint.Y & API_Node.Restraint.Z &
            !API_Node.Restraint.XX & !API_Node.Restraint.YY & !API_Node.Restraint.ZZ)
        sptTxt = " Pin";
      else if (API_Node.Restraint.X & API_Node.Restraint.Y & API_Node.Restraint.Z &
          API_Node.Restraint.XX & API_Node.Restraint.YY & API_Node.Restraint.ZZ)
        sptTxt = " Fix";
      else
      {
        sptTxt = " " + "X:" + (API_Node.Restraint.X ? "\u2713" : "\u2610") +
         " Y:" + (API_Node.Restraint.Y ? "\u2713" : "\u2610") +
         " Z:" + (API_Node.Restraint.Z ? "\u2713" : "\u2610") +
         " XX:" + (API_Node.Restraint.XX ? "\u2713" : "\u2610") +
         " YY:" + (API_Node.Restraint.YY ? "\u2713" : "\u2610") +
         " ZZ:" + (API_Node.Restraint.ZZ ? "\u2713" : "\u2610");
      }

      string localTxt = "";
      Plane noPlane = new Plane() { Origin = new Point3d(0, 0, 0), XAxis = new Vector3d(0, 0, 0), YAxis = new Vector3d(0, 0, 0), ZAxis = new Vector3d(0, 0, 0) };
      if (LocalAxis != noPlane)
      {
        if (LocalAxis != Plane.WorldXY)
        {
          GH_Plane gH_Plane = new GH_Plane(LocalAxis);
          localTxt = " Axis:{" + gH_Plane.ToString() + "}";
        }
      }

      return idd + sptTxt + localTxt + " Pos:" + new GH_Point(this.Point).ToString();
    }

    internal void CloneApiNode()
    {
      Node node = new Node
      {
        AxisProperty = _node.AxisProperty,
        DamperProperty = _node.DamperProperty,
        MassProperty = _node.MassProperty,
        Name = _node.Name.ToString(),
        Restraint = new NodalRestraint
        {
          X = _node.Restraint.X,
          Y = _node.Restraint.Y,
          Z = _node.Restraint.Z,
          XX = _node.Restraint.XX,
          YY = _node.Restraint.YY,
          ZZ = _node.Restraint.ZZ,
        },
        SpringProperty = _node.SpringProperty,
        Position = new Vector3
        {
          X = _node.Position.X,
          Y = _node.Position.Y,
          Z = _node.Position.Z
        }
      };

      if ((Color)_node.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        node.Colour = _node.Colour;

      _node = node;
    }

    internal Node GetApiNodeToUnit(LengthUnit unit = LengthUnit.Meter)
    {
      Node node = new Node
      {
        AxisProperty = _node.AxisProperty,
        DamperProperty = _node.DamperProperty,
        MassProperty = _node.MassProperty,
        Name = _node.Name.ToString(),
        Restraint = new NodalRestraint
        {
          X = _node.Restraint.X,
          Y = _node.Restraint.Y,
          Z = _node.Restraint.Z,
          XX = _node.Restraint.XX,
          YY = _node.Restraint.YY,
          ZZ = _node.Restraint.ZZ,
        },
        SpringProperty = _node.SpringProperty,
        Position = new Vector3
        {
          X = (unit == LengthUnit.Meter) ? _node.Position.X : new Length(_node.Position.X, unit).Meters,
          Y = (unit == LengthUnit.Meter) ? _node.Position.Y : new Length(_node.Position.Y, unit).Meters,
          Z = (unit == LengthUnit.Meter) ? _node.Position.Z : new Length(_node.Position.Z, unit).Meters
        }
      };

      if ((Color)_node.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        node.Colour = _node.Colour;

      return node;
    }

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

    internal void UpdatePreview()
    {
      if (_node.Restraint.X || _node.Restraint.Y || _node.Restraint.Z || _node.Restraint.XX || _node.Restraint.YY || _node.Restraint.ZZ)
      {
        GsaGH.UI.Display.PreviewRestraint(Restraint, _plane, Point, ref this.previewSupportSymbol, ref this.previewText);
      }
      else
      {
        this.previewSupportSymbol = null;
        this.previewText = null;
      }

      if (_plane != null)
      {
        if (_plane != Plane.WorldXY & _plane != new Plane())
        {
          Plane local = _plane.Clone();
          local.Origin = Point;

          this.previewXaxis = new Line(Point, local.XAxis, 0.5);
          this.previewYaxis = new Line(Point, local.YAxis, 0.5);
          this.previewZaxis = new Line(Point, local.ZAxis, 0.5);
        }
      }
    }
    #endregion
  }
}
