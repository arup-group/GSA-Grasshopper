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

    private int _gridSrfId = 0;
    private Guid _gridSrfGuid = Guid.NewGuid();
    private GridSurface _gridSrf = new GridSurface();

    private int _gridPlnId = 0;
    private Guid _gridPlnGuid = Guid.NewGuid();
    private GridPlane _gridPln = new GridPlane();

    private Plane _pln = Plane.WorldXY;  // plane at display point (Axis + elevation) 

    internal Guid RefObjectGuid;
    internal ReferenceType ReferenceType = ReferenceType.None;
    #endregion

    #region properties
    /// <summary>
    /// String either in the format of a double (will be converted into Model Units)
    /// or in the format of a OasysUnits.Length ('5 m')
    /// </summary>
    public string Elevation { get; set; } = "0";
    /// <summary>
    /// String either in the format of a double (will be converted into Model Units)
    /// or in the format of a OasysUnits.Length ('5 m')
    /// </summary>
    public string Tolerance { get; set; } = "10 mm";
    /// <summary>
    /// String either in the format of a double (will be converted into Model Units)
    /// or in the format of a OasysUnits.Length ('5 m'). '0' equals 'auto'.
    /// </summary>
    public string StoreyToleranceAbove { get; set; } = "auto";
    /// <summary>
    /// String either in the format of a double (will be converted into Model Units)
    /// or in the format of a OasysUnits.Length ('5 m'). '0' equals 'auto'.
    /// </summary>
    public string StoreyToleranceBelow { get; set; } = "auto";
    public string AxisName { get; set; } = "";
    public Guid GridSurfaceGUID => _gridSrfGuid;
    public Guid GridPlaneGUID => _gridPlnGuid;
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

    public int GridSurfaceId
    {
      get
      {
        return _gridSrfId;
      }
      set
      {
        _gridSrfGuid = Guid.NewGuid();
        _gridSrfId = value;
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
    public int GridPlaneId
    {
      get
      {
        return _gridPlnId;
      }
      set
      {
        _gridPlnGuid = Guid.NewGuid();
        _gridPlnId = value;
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
        Elevation = this.Elevation,
        Tolerance = this.Tolerance,
        StoreyToleranceAbove = this.StoreyToleranceAbove,
        StoreyToleranceBelow = this.StoreyToleranceBelow,
        AxisName = this.AxisName,
        GridPlaneId = _gridPlnId,
        GridSurfaceId = _gridSrfId
      };
      dup._gridSrfGuid = new Guid(_gridSrfGuid.ToString());
      dup._gridPlnGuid = new Guid(_gridPlnGuid.ToString());
      if (this.ReferenceType != ReferenceType.None)
      {
        dup.RefObjectGuid = new Guid(this.RefObjectGuid.ToString());
        dup.ReferenceType = this.ReferenceType;
      }
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

      string gp = (this.GridPlaneId == 0) ? "" : "GPln:" + this.GridPlaneId.ToString() + " ";
      string gpName = this.GridPlane == null ? "" : this.GridPlane.Name;
      gp += gpName == "" ? "" : "'" + gpName + "' ";

      if (global)
        gp += "Global grid ";
      else
        gp += $"O:{this._pln.Origin}, X:{this._pln.XAxis}, Y:{this._pln.YAxis}";
      if (this.Elevation != "0")
        gp += " E:" + this.Elevation.Replace(" ", string.Empty).Replace(",", string.Empty) + " ";
      if (this.GridPlane.IsStoreyType)
        gp += "Storey ";

      string gs = (this.GridSurfaceId == 0) ? "" : "GSrf:" + this.GridSurfaceId.ToString() + " ";
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

      return string.Join(" ", ax.Trim(), gp.Trim(), gs.Trim()).Replace("''", string.Empty).Trim().Replace("  ", " ");
    }

    internal Axis GetAxis(LengthUnit modelUnit)
    {
      Axis axis = new Axis();
      axis.Origin.X = new Length(this.Plane.Origin.X, modelUnit).Meters;
      axis.Origin.Y = new Length(this.Plane.Origin.Y, modelUnit).Meters;
      axis.Origin.Z = new Length(this.Plane.Origin.Z, modelUnit).Meters;
      if (this.Elevation != "0")
      {
        Length elevation = new Length();
        try
        {
          elevation = Length.Parse(this.Elevation);
        }
        catch (Exception)
        {
          if (double.TryParse(this.Elevation, out double elev))
            elevation = new Length(elev, modelUnit);
        }
        axis.Origin.Z -= elevation.As(modelUnit);
      }
      
      axis.XVector.X = this.Plane.XAxis.X;
      axis.XVector.Y = this.Plane.XAxis.Y;
      axis.XVector.Z = this.Plane.XAxis.Z;
      axis.XYPlane.X = this.Plane.YAxis.X;
      axis.XYPlane.Y = this.Plane.YAxis.Y;
      axis.XYPlane.Z = this.Plane.YAxis.Z;

      return axis;
    }
    #endregion
  }
}
