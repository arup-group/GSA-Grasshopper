using System;
using GH_IO.Types;
using GsaAPI;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Grid Plane Surface class, this class defines the basic properties and methods for any Gsa Grid Plane Surface
  /// </summary>
  public class GsaGridPlaneSurface
  {
    #region fields
    private int _axisID = 0;

    private int _gridSrfID = 0;
    private Guid _gridSrfGuid = Guid.NewGuid();
    private GridSurface _gridSrf = new GridSurface();

    private int _gridPlnID = 0;
    private Guid _gridPlnGuid = Guid.NewGuid();
    private GridPlane _gridPln = new GridPlane();

    private Plane _pln = Plane.WorldXY;  // plane at display point (Axis + elevation) 
    #endregion

    #region properties
    public double Elevation { get; set; } = 0;
    public string Name { get; set; } = "";

    public int AxisId
    {
      get
      {
        return _axisID;
      }
      set
      {
        _gridPlnGuid = Guid.NewGuid();
        _axisID = value;
      }
    }

    public GridSurface GridSurface
    {
      get
      {
        return _gridSrf;
      }
      set
      {
        _gridSrfGuid = Guid.NewGuid();
        _gridSrf = value;
      }
    }

    public int GridSurfaceID
    {
      get
      {
        return _gridSrfID;
      }
      set
      {
        _gridSrfGuid = Guid.NewGuid();
        _gridSrfID = value;
      }
    }
    public Guid GridSurfaceGUID
    {
      get
      {
        return _gridSrfGuid;
      }
    }

    public GridPlane GridPlane
    {
      get
      {
        return _gridPln;
      }
      set
      {
        _gridPlnGuid = Guid.NewGuid();
        _gridPln = value;
      }
    }

    public int GridPlaneID
    {
      get
      {
        return _gridPlnID;
      }
      set
      {
        _gridPlnGuid = Guid.NewGuid();
        _gridPlnID = value;
      }
    }

    public Guid GridPlaneGUID
    {
      get
      {
        return _gridPlnGuid;
      }
    }

    public Plane Plane
    {
      get
      {
        return _pln;
      }
      set
      {
        _gridPlnGuid = Guid.NewGuid();
        _pln = value;
      }
    }
    #endregion

    #region constructors
    public GsaGridPlaneSurface()
    {
    }

    public GsaGridPlaneSurface(Plane plane, bool tryUseExisting = false)
    {
      _pln = plane;
      _gridPln = new GridPlane();
      if (tryUseExisting)
        _gridPlnGuid = new Guid(); // will create 0000-00000-00000-00000
      else
        _gridPlnGuid = Guid.NewGuid(); // will create random guid

      _gridSrf = new GridSurface
      {
        Direction = 0,
        Elements = "all",
        ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL,
        ExpansionType = GridSurfaceExpansionType.UNDEF,
        SpanType = GridSurface.Span_Type.ONE_WAY
      };
      if (tryUseExisting)
        _gridSrfGuid = new Guid(); // will create 0000-00000-00000-00000
      else
        _gridSrfGuid = Guid.NewGuid(); // will create random guid
    }

    public GsaGridPlaneSurface Duplicate()
    {
      GsaGridPlaneSurface dup = new GsaGridPlaneSurface
      {
        Plane = (_gridPln == null) ? Plane.WorldXY : this._pln.Clone(),
        GridPlane = this._gridPln == null ? null : new GridPlane
        {
          AxisProperty = this._gridPln.AxisProperty,
          Elevation = this._gridPln.Elevation,
          IsStoreyType = this._gridPln.IsStoreyType,
          Name = this._gridPln.Name.ToString(),
          ToleranceAbove = this._gridPln.ToleranceAbove,
          ToleranceBelow = this._gridPln.ToleranceBelow
        },
        GridSurface = new GridSurface
        {
          Direction = this._gridSrf.Direction,
          Elements = this._gridSrf.Elements.ToString(),
          ElementType = this._gridSrf.ElementType,
          ExpansionType = this._gridSrf.ExpansionType,
          GridPlane = this._gridSrf.GridPlane,
          Name = this._gridSrf.Name.ToString(),
          SpanType = this._gridSrf.SpanType,
          Tolerance = this._gridSrf.Tolerance
        },
        Name = this.Name,
        //Axis = this._gridPln == null ? null : new Axis
        //{
        //  Name = _axis.Name.ToString(),
        //  Origin = new Vector3 { X = _axis.Origin.X, Y = _axis.Origin.Y, Z = _axis.Origin.Z },
        //  Type = _axis.Type,
        //  XVector = new Vector3 { X = _axis.XVector.X, Y = _axis.XVector.Y, Z = _axis.XVector.Z },
        //  XYPlane = new Vector3 { X = _axis.XYPlane.X, Y = _axis.XYPlane.Y, Z = _axis.XYPlane.Z }
        //},
        GridPlaneID = _gridPlnID,
        GridSurfaceID = _gridSrfID
      };
      dup._gridSrfGuid = new Guid(_gridSrfGuid.ToString());
      dup._gridPlnGuid = new Guid(_gridPlnGuid.ToString());
      return dup;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      if (GridPlane == null && GridSurface == null)
      {
        return "Null";
      }
      string ax = (this.AxisId == 0) ? "" : "Ax:" + this.AxisId.ToString() + " ";
      bool global = false;
      if (this.Plane.Origin.X == 0 && this.Plane.Origin.Y == 0 && this.Plane.Origin.Z == 0)
        if (this.Plane.XAxis.X == 1 && this.Plane.XAxis.Y == 0 && this.Plane.XAxis.Z == 0)
          if (this.Plane.YAxis.X == 0 && this.Plane.YAxis.Y == 1 && this.Plane.YAxis.Z == 0)
            global = true;

      string gp = (this.GridPlaneID == 0) ? "" : "GPln:" + this.GridPlaneID.ToString() + " ";
      string gpName = this.GridPlane == null ? "" : this.GridPlane.Name;
      gp += gpName == "" ? "" : "'" + gpName + "' ";

      if (global)
        gp += "Global grid ";
      else
        gp += $"O:{this._pln.Origin}, X:{this._pln.XAxis}, Y:{this._pln.YAxis}";
      if (this.GridPlane.Elevation != 0)
        gp += "E:" + new Length(this.GridPlane.Elevation, LengthUnit.Meter).ToUnit(DefaultUnits.LengthUnitGeometry).ToString("g").Replace(" ", string.Empty) + " ";
      if (this.GridPlane.IsStoreyType)
        gp += "Storey ";

      string gs = (this.GridSurfaceID == 0) ? "" : "GSrf:" + this.GridSurfaceID.ToString() + " ";
      string gsName = this.GridSurface == null ? "" : this.GridSurface.Name;
      gs += gsName == "" ? "" : "'" + gsName + "' ";
      if (this.GridSurface.SpanType == GridSurface.Span_Type.ONE_WAY)
      {
        if (this.GridSurface.SpanType == GridSurface.Span_Type.ONE_WAY)
          gs += "1D, One-way ";
        else
          gs += "1D, Two-way ";
        if (this.GridSurface.SpanType == GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS)
          gs += "simplified ";
      }
      else
        gs += "2D ";
      if (this.GridSurface.Direction != 0)
        gs += new Angle(this.GridSurface.Direction, AngleUnit.Degree).ToString("g").Replace(" ", string.Empty) + " ";
      gs += this.GridSurface.Elements == "all" ? "" : this.GridSurface.Elements;

      return (ax + gp + gs).Replace("''", string.Empty).Trim();
    }

    internal Axis GetAxis(LengthUnit modelUnit)
    {
      Axis axis = new Axis();
      axis.Origin.X = new Length(this.Plane.Origin.X, modelUnit).Meters;
      axis.Origin.Y = new Length(this.Plane.Origin.Y, modelUnit).Meters;
      axis.Origin.Z = new Length(this.Plane.Origin.Z - this.Elevation, modelUnit).Meters;
      axis.XVector.X = new Length(this.Plane.XAxis.X, modelUnit).Meters;
      axis.XVector.Y = new Length(this.Plane.XAxis.Y, modelUnit).Meters;
      axis.XVector.Z = new Length(this.Plane.XAxis.Z, modelUnit).Meters;
      axis.XYPlane.X = new Length(this.Plane.YAxis.X, modelUnit).Meters;
      axis.XYPlane.Y = new Length(this.Plane.YAxis.Y, modelUnit).Meters;
      axis.XYPlane.Z = new Length(this.Plane.YAxis.Z, modelUnit).Meters;

      return axis;
    }
    #endregion
  }
}
