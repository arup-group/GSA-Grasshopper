using System;
using GsaAPI;
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
    private Axis _axis = new Axis();

    private int _gridSrfID = 0;
    private Guid _gridSrfGuid = Guid.NewGuid();
    private GridSurface _gridSrf = new GridSurface();
    
    private int _gridPlnID = 0;
    private Guid _gridPlnGuid = Guid.NewGuid();
    private GridPlane _gridPln = new GridPlane();

    private Plane _pln = Plane.WorldXY;  // plane at display point (Axis + elevation) 
    #endregion

    #region properties
    public Axis Axis
    {
      get { 
        return _axis;
      }
      set
      {
        _axis = value;
        _gridPlnGuid = Guid.NewGuid();
        if (value != null)
        {
          _pln.OriginX = _axis.Origin.X;
          _pln.OriginY = _axis.Origin.Y;
          if (_gridPln != null)
          {
            if (_gridPln.Elevation != 0)
              _pln.OriginZ = _axis.Origin.Z + _gridPln.Elevation;
          }
          else
            _pln.OriginZ = _axis.Origin.Z;

          _pln = new Plane(_pln.Origin,
              new Vector3d(_axis.XVector.X, _axis.XVector.Y, _axis.XVector.Z),
              new Vector3d(_axis.XYPlane.X, _axis.XYPlane.Y, _axis.XYPlane.Z)
              );
        }
      }
    }

    public int AxisID
    {
      get {
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
      get { 
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
      get { 
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
      get { 
        return _gridSrfGuid;
      }
    }

    public GridPlane GridPlane
    {
      get {
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
      get { return _gridPlnID; 
      }
      set
      {
        _gridPlnGuid = Guid.NewGuid();
        _gridPlnID = value;
      }
    }

    public Guid GridPlaneGUID
    {
      get { 
        return _gridPlnGuid; 
      }
    }

    public Plane Plane
    {
      get { 
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

      _axis = new Axis();
      _axis.Origin.X = plane.OriginX;
      _axis.Origin.Y = plane.OriginY;
      _axis.Origin.Z = plane.OriginZ;

      _axis.XVector.X = plane.XAxis.X;
      _axis.XVector.Y = plane.XAxis.Y;
      _axis.XVector.Z = plane.XAxis.Z;
      _axis.XYPlane.X = plane.YAxis.X;
      _axis.XYPlane.Y = plane.YAxis.Y;
      _axis.XYPlane.Z = plane.YAxis.Z;
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
        Axis = this._gridPln == null ? null : new Axis
        {
          Name = _axis.Name.ToString(),
          Origin = new Vector3 { X = _axis.Origin.X, Y = _axis.Origin.Y, Z = _axis.Origin.Z },
          Type = _axis.Type,
          XVector = new Vector3 { X = _axis.XVector.X, Y = _axis.XVector.Y, Z = _axis.XVector.Z },
          XYPlane = new Vector3 { X = _axis.XYPlane.X, Y = _axis.XYPlane.Y, Z = _axis.XYPlane.Z }
        },
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
      if (GridPlane == null && GridSurface == null) { 
        return "Null GridPlaneSurface";
      }
      string gp = GridPlane == null ? "" : GridPlane.Name + " ";
      string gs = GridSurface == null ? "" : GridSurface.Name;
      return "GSA Grid Plane Surface " + gp + gs;
    }
    #endregion
  }
}
