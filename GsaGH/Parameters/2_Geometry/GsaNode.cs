using System.Drawing;
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

    private Plane _plane;
    private Node _node = new Node();
    #endregion

    #region properties
    public int Id { get; set; }
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
    public bool IsSupport => _node.Restraint.X || _node.Restraint.Y || _node.Restraint.Z || _node.Restraint.XX || _node.Restraint.YY || _node.Restraint.ZZ;

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
        this.CloneApiObject();
        Id = 0;
        _node.Position.X = value.X;
        _node.Position.Y = value.Y;
        _node.Position.Z = value.Z;
        this.UpdatePreview();
      }
    }
    #region GsaAPI.Node members
    internal Node ApiNode
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
        CloneApiObject();
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
        CloneApiObject();
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
        CloneApiObject();
        _node.Name = value;
      }
    }

    public int DamperProperty
    {
      get
      {
        return _node.DamperProperty;
      }
      set
      {
        CloneApiObject();
        _node.DamperProperty = value;
      }
    }

    public int MassProperty
    {
      get
      {
        return _node.MassProperty;
      }
      set
      {
        CloneApiObject();
        _node.MassProperty = value;
      }
    }

    public int SpringProperty
    {
      get
      {
        return _node.SpringProperty;
      }
      set
      {
        CloneApiObject();
        _node.SpringProperty = value;
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
      CloneApiObject();
      if (unit != LengthUnit.Meter)
      {
        _node.Position.X = new Length(node.Position.X, LengthUnit.Meter).As(unit);
        _node.Position.Y = new Length(node.Position.Y, LengthUnit.Meter).As(unit);
        _node.Position.Z = new Length(node.Position.Z, LengthUnit.Meter).As(unit);
      }
      Id = id;
      _plane = localAxis;
      this.UpdatePreview();
    }

    public GsaNode(Point3d position, int id = 0)
    {
      Point = position;
      Id = id;
      this.UpdatePreview();
    }
    #endregion

    #region methods
    public GsaNode Duplicate(bool cloneApiNode = false)
    {
      GsaNode dup = new GsaNode();
      dup.Id = this.Id;
      dup._node = _node;
      if (cloneApiNode)
        dup.CloneApiObject();
      dup._plane = _plane;
      dup.UpdatePreview();
      return dup;
    }

    public override string ToString()
    {
      if (ApiNode == null) { return "Null"; }
      string idd = this.Id == 0 ? "" : "ID:" + Id + " ";

      string sptTxt;
      if (ApiNode.Restraint.X == false && ApiNode.Restraint.Y == false && ApiNode.Restraint.Z == false &&
          ApiNode.Restraint.XX == false && ApiNode.Restraint.YY == false && ApiNode.Restraint.ZZ == false)
        sptTxt = "";
      else if (ApiNode.Restraint.X & ApiNode.Restraint.Y & ApiNode.Restraint.Z &
            !ApiNode.Restraint.XX & !ApiNode.Restraint.YY & !ApiNode.Restraint.ZZ)
        sptTxt = " Pin";
      else if (ApiNode.Restraint.X & ApiNode.Restraint.Y & ApiNode.Restraint.Z &
          ApiNode.Restraint.XX & ApiNode.Restraint.YY & ApiNode.Restraint.ZZ)
        sptTxt = " Fix";
      else
      {
        sptTxt = " " + "X:" + (ApiNode.Restraint.X ? "\u2713" : "\u2610") +
         " Y:" + (ApiNode.Restraint.Y ? "\u2713" : "\u2610") +
         " Z:" + (ApiNode.Restraint.Z ? "\u2713" : "\u2610") +
         " XX:" + (ApiNode.Restraint.XX ? "\u2713" : "\u2610") +
         " YY:" + (ApiNode.Restraint.YY ? "\u2713" : "\u2610") +
         " ZZ:" + (ApiNode.Restraint.ZZ ? "\u2713" : "\u2610");
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

      return string.Join(" ", idd.Trim(), sptTxt.Trim(), localTxt.Trim(), ("Pos:" + new GH_Point(this.Point).ToString()).Trim()).Trim().Replace("  ", " ");
    }

    internal void CloneApiObject()
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
        this.ApiNode.Position.X = new Length(this.ApiNode.Position.X, LengthUnit.Meter).As(unit);
        this.ApiNode.Position.Y = new Length(this.ApiNode.Position.Y, LengthUnit.Meter).As(unit);
        this.ApiNode.Position.Z = new Length(this.ApiNode.Position.Z, LengthUnit.Meter).As(unit);
      }
    }

    internal void UpdatePreview()
    {
      if (_node.Restraint.X || _node.Restraint.Y || _node.Restraint.Z || _node.Restraint.XX || _node.Restraint.YY || _node.Restraint.ZZ)
      {
        Helpers.Graphics.Display.PreviewRestraint(Restraint, _plane, Point, ref this.previewSupportSymbol, ref this.previewText);
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
