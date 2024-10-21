using System;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters.Enums;

using OasysUnits;

using Rhino.Geometry;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Grid Plane Surface is used by <see href="https://docs.oasys-software.com/structural/gsa/references-theory/grid-loads.html">Grid Loads</see>.</para>
  /// <para>A grid plane defines the geometry of a surface, and the load behaviour of the grid plane is defined by a grid surface.</para>
  /// <para>In Grasshopper, a Grid Plane Surface contains both the information of what in GSA is known as a <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-grid-plane.html">Grid Plane</see> and a <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-grid-surface.html">Grid Surface</see></para>
  /// <para>The Grasshopper plugin will automatically create a fitting Grid Plane Surface when using the <see cref="Components.CreateGridPointLoad"/>, <see cref="Components.CreateGridLineLoad"/> or <see cref="Components.CreateGridAreaLoad"/> components. </para>
  /// <para>Grid Plane Surfaces can also be created independently using the <see cref="Components.CreateGridPlane"/> and <see cref="Components.CreateGridSurface"/> components.</para>
  /// </summary>
  public class GsaGridPlaneSurface {
    public int AxisId {
      get => _axisId;
      set {
        _gridPlnGuid = Guid.NewGuid();
        _axisId = value;
      }
    }
    public string AxisName { get; set; } = string.Empty;
    /// <summary>
    ///   String either in the format of a double (will be converted into Model Units)
    ///   or in the format of a OasysUnits.Length ('5 m')
    /// </summary>
    public string Elevation { get; private set; } = "0";

    public GridPlane GridPlane {
      get => _gridPln;
      set {
        _gridPlnGuid = Guid.NewGuid();
        _gridPln = value;
      }
    }
    public Guid GridPlaneGuid => _gridPlnGuid;
    public int GridPlaneId {
      get => _gridPlnId;
      set {
        _gridPlnGuid = Guid.NewGuid();
        _gridPlnId = value;
      }
    }
    public GridSurface GridSurface {
      get => _gridSrf;
      set {
        _gridSrfGuid = Guid.NewGuid();
        _gridSrf = value;
      }
    }
    public Guid GridSurfaceGuid => _gridSrfGuid;
    public int GridSurfaceId {
      get => _gridSrfId;
      set {
        _gridSrfGuid = Guid.NewGuid();
        _gridSrfId = value;
      }
    }
    public Plane Plane {
      get => _pln;
      set {
        _gridPlnGuid = Guid.NewGuid();
        double elevation = _pln.Origin.DistanceTo(PreviewPlane.Origin);
        _pln = value;
        UpdatePreviewPlane(elevation);
      }
    }

    internal Plane PreviewPlane = Plane.WorldXY;

    /// <summary>
    ///   String either in the format of a double (will be converted into Model Units)
    ///   or in the format of a OasysUnits.Length ('5 m'). '0' equals 'auto'.
    /// </summary>
    public string StoreyToleranceAbove { get; set; } = "auto";
    /// <summary>
    ///   String either in the format of a double (will be converted into Model Units)
    ///   or in the format of a OasysUnits.Length ('5 m'). '0' equals 'auto'.
    /// </summary>
    public string StoreyToleranceBelow { get; set; } = "auto";
    /// <summary>
    ///   String either in the format of a double (will be converted into Model Units)
    ///   or in the format of a OasysUnits.Length ('5 m')
    /// </summary>
    public string Tolerance { get; set; } = "10 mm";
    internal ReferenceType _referenceType = ReferenceType.None;
    internal GsaList _refList;
    internal Guid _refObjectGuid;
    private int _axisId;
    private GridPlane _gridPln = new GridPlane();
    private Guid _gridPlnGuid = Guid.NewGuid();
    private int _gridPlnId = 0;
    private GridSurface _gridSrf = new GridSurface("");
    private Guid _gridSrfGuid = Guid.NewGuid();
    private int _gridSrfId;
    private Plane _pln = Plane.WorldXY;

    public GsaGridPlaneSurface() { }

    public GsaGridPlaneSurface(Plane plane, bool tryUseExisting = false) {
      _pln = plane;
      _gridPlnGuid = tryUseExisting ? new Guid() // will create 0000-00000-00000-00000
        : Guid.NewGuid(); // will create random guid

      _gridSrf = new GridSurface("") {
        Direction = 0,
        Elements = "all",
        ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL,
        ExpansionType = GridSurfaceExpansionType.UNDEF,
        SpanType = GridSurface.Span_Type.ONE_WAY,
      };
      _gridSrfGuid = tryUseExisting ? new Guid() // will create 0000-00000-00000-00000
        : Guid.NewGuid(); // will create random guid
    }

    public GsaGridPlaneSurface Duplicate() {
      var dup = new GsaGridPlaneSurface {
        Plane = (_gridPln == null) ? Plane.WorldXY : _pln.Clone(),
        GridPlane = _gridPln == null ? null : new GridPlane {
          AxisProperty = _gridPln.AxisProperty,
          IsStoreyType = _gridPln.IsStoreyType,
          Name = _gridPln.Name.ToString(),
          ToleranceAbove = _gridPln.ToleranceAbove,
          ToleranceBelow = _gridPln.ToleranceBelow,
        },
        GridSurface = new GridSurface(_gridSrf.Name) {
          Direction = _gridSrf.Direction,
          Elements = _gridSrf.Elements.ToString(),
          ElementType = _gridSrf.ElementType,
          ExpansionType = _gridSrf.ExpansionType,
          GridPlane = _gridSrf.GridPlane,
          SpanType = _gridSrf.SpanType,
          Tolerance = _gridSrf.Tolerance
        },
        Elevation = Elevation,
        Tolerance = Tolerance,
        StoreyToleranceAbove = StoreyToleranceAbove,
        StoreyToleranceBelow = StoreyToleranceBelow,
        AxisName = AxisName,
        GridPlaneId = _gridPlnId,
        GridSurfaceId = _gridSrfId,
        _gridSrfGuid = new Guid(_gridSrfGuid.ToString()),
        _gridPlnGuid = new Guid(_gridPlnGuid.ToString()),
        PreviewPlane = new Plane(PreviewPlane),
      };
      if (_referenceType == ReferenceType.None) {
        return dup;
      }

      if (_referenceType == ReferenceType.List) {
        dup._referenceType = ReferenceType.List;
        dup._refList = _refList.Duplicate();
      } else {
        dup._refObjectGuid = new Guid(_refObjectGuid.ToString());
        dup._referenceType = _referenceType;
      }

      return dup;
    }

    public override string ToString() {
      if (GridPlane == null && GridSurface == null) {
        return "Null";
      }

      string ax = (AxisId == 0) ? string.Empty : "Ax:" + AxisId.ToString() + " ";
      bool global = false;
      if (Plane.Origin.X == 0 && Plane.Origin.Y == 0 && Plane.Origin.Z == 0) {
        if (Plane.XAxis.X == 1 && Plane.XAxis.Y == 0 && Plane.XAxis.Z == 0) {
          if (Plane.YAxis.X == 0 && Plane.YAxis.Y == 1 && Plane.YAxis.Z == 0) {
            global = true;
          }
        }
      }

      string gp = (GridPlaneId == 0) ? string.Empty : "GPln:" + GridPlaneId.ToString() + " ";
      string gpName = GridPlane == null ? string.Empty : GridPlane.Name;
      gp += gpName == string.Empty ? string.Empty : "'" + gpName + "' ";

      if (global) {
        gp += "Global grid ";
      } else {
        gp += $"O:{_pln.Origin}, X:{_pln.XAxis}, Y:{_pln.YAxis}";
      }

      if (Elevation != "0") {
        gp += " E:" + Elevation.Replace(" ", string.Empty).Replace(",", string.Empty) + " ";
      }

      if (GridPlane.IsStoreyType) {
        gp += "Storey ";
      }

      string gs = (GridSurfaceId == 0) ? string.Empty : "GSrf:" + GridSurfaceId.ToString() + " ";
      string gsName = GridSurface == null ? string.Empty : GridSurface.Name;
      gs += gsName == string.Empty ? string.Empty : "'" + gsName + "' ";
      if (GridSurface.SpanType == GridSurface.Span_Type.ONE_WAY) {
        if (GridSurface.SpanType == GridSurface.Span_Type.ONE_WAY) {
          gs += "1D, One-way ";
        } else {
          gs += "1D, Two-way ";
        }

        if (GridSurface.SpanType == GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS) {
          gs += "simplified ";
        }
      } else {
        gs += "2D ";
      }

      if (GridSurface.Direction != 0) {
        gs += new Angle(GridSurface.Direction, AngleUnit.Degree).ToString("g")
         .Replace(" ", string.Empty) + " ";
      }

      gs += GridSurface.Elements == "all" ? string.Empty : GridSurface.Elements;

      return string.Join(" ", ax, gp, gs).Replace("''", string.Empty).TrimSpaces();
    }

    internal Axis GetAxis(LengthUnit modelUnit) {
      var axis = new Axis();
      axis.Origin.X = new Length(Plane.Origin.X, modelUnit).Meters;
      axis.Origin.Y = new Length(Plane.Origin.Y, modelUnit).Meters;
      axis.Origin.Z = new Length(Plane.Origin.Z, modelUnit).Meters;

      axis.XVector.X = Plane.XAxis.X;
      axis.XVector.Y = Plane.XAxis.Y;
      axis.XVector.Z = Plane.XAxis.Z;
      axis.XYPlane.X = Plane.YAxis.X;
      axis.XYPlane.Y = Plane.YAxis.Y;
      axis.XYPlane.Z = Plane.YAxis.Z;

      return axis;
    }

    internal void SetElevation(double elevation) {
      Elevation = elevation.ToString();
      UpdatePreviewPlane(elevation);
    }
    internal void SetElevation(Length elevation) {
      Elevation = elevation.ToString().Replace(" ", string.Empty).Replace(",", string.Empty);
      UpdatePreviewPlane(elevation.Value);
    }

    private void UpdatePreviewPlane(double elevation) {
      // if elevation is set we want to move the plane in it's normal direction
      Vector3d vec = _pln.Normal;
      vec.Unitize();
      vec.X *= elevation;
      vec.Y *= elevation;
      vec.Z *= elevation;
      var xform = Transform.Translation(vec);
      var pln = new Plane(_pln);
      pln.Transform(xform);
      PreviewPlane = pln;
    }
  }
}
