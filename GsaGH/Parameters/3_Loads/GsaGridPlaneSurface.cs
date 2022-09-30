using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
    private Plane m_plane;  // plane at display point (Axis + elevation) 
                            //remember to substract elevation when creating an axis from this plane!
    private GridPlane m_gridplane;
    private GridSurface m_gridsrf;
    private Axis m_axis;
    private int m_gridplnID = 0;
    private int m_gridsrfID = 0;
    private int m_axisID = 0;
    private Guid m_gp_guid;
    private Guid m_gs_guid;
    #endregion
    public Plane Plane
    {
      get { return m_plane; }
      set
      {
        m_gp_guid = Guid.NewGuid();
        m_plane = value;
      }
    }
    public GridPlane GridPlane
    {
      get { return m_gridplane; }
      set
      {
        m_gp_guid = Guid.NewGuid();
        m_gridplane = value;
      }
    }
    public int GridPlaneID
    {
      get { return m_gridplnID; }
      set
      {
        m_gp_guid = Guid.NewGuid();
        m_gridplnID = value;
      }
    }

    public GridSurface GridSurface
    {
      get { return m_gridsrf; }
      set
      {
        m_gs_guid = Guid.NewGuid();
        m_gridsrf = value;
      }
    }
    public int GridSurfaceID
    {
      get { return m_gridsrfID; }
      set
      {
        m_gs_guid = Guid.NewGuid();
        m_gridsrfID = value;
      }
    }
    public Axis Axis
    {
      get { return m_axis; }
      set
      {
        m_axis = value;
        m_gp_guid = Guid.NewGuid();
        if (value != null)
        {
          m_plane.OriginX = m_axis.Origin.X;
          m_plane.OriginY = m_axis.Origin.Y;
          if (m_gridplane != null)
          {
            if (m_gridplane.Elevation != 0)
              m_plane.OriginZ = m_axis.Origin.Z + m_gridplane.Elevation;
          }
          else
            m_plane.OriginZ = m_axis.Origin.Z;

          m_plane = new Plane(m_plane.Origin,
              new Vector3d(m_axis.XVector.X, m_axis.XVector.Y, m_axis.XVector.Z),
              new Vector3d(m_axis.XYPlane.X, m_axis.XYPlane.Y, m_axis.XYPlane.Z)
              );
        }
      }
    }
    public int AxisID
    {
      get { return m_axisID; }
      set
      {
        m_gp_guid = Guid.NewGuid();
        m_axisID = value;
      }
    }
    public Guid GridPlaneGUID
    {
      get { return m_gp_guid; }
    }
    public Guid GridSurfaceGUID
    {
      get { return m_gs_guid; }
    }
    #region constructors
    public GsaGridPlaneSurface()
    {
      m_plane = Plane.Unset;
      m_gridplane = new GridPlane();
      m_gp_guid = Guid.NewGuid();
      m_gridsrf = new GridSurface();
      m_gs_guid = Guid.NewGuid();
      m_axis = new Axis();
    }

    public GsaGridPlaneSurface(Plane plane, bool tryUseExisting = false)
    {
      m_plane = plane;
      m_gridplane = new GridPlane();
      if (tryUseExisting)
        m_gp_guid = new Guid(); // will create 0000-00000-00000-00000
      else
        m_gp_guid = Guid.NewGuid(); // will create random guid

      m_gridsrf = new GridSurface
      {
        Direction = 0,
        Elements = "all",
        ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL,
        ExpansionType = GridSurfaceExpansionType.UNDEF,
        SpanType = GridSurface.Span_Type.ONE_WAY
      };
      if (tryUseExisting)
        m_gs_guid = new Guid(); // will create 0000-00000-00000-00000
      else
        m_gs_guid = Guid.NewGuid(); // will create random guid

      m_axis = new Axis();
      m_axis.Origin.X = plane.OriginX;
      m_axis.Origin.Y = plane.OriginY;
      m_axis.Origin.Z = plane.OriginZ;

      m_axis.XVector.X = plane.XAxis.X;
      m_axis.XVector.Y = plane.XAxis.Y;
      m_axis.XVector.Z = plane.XAxis.Z;
      m_axis.XYPlane.X = plane.YAxis.X;
      m_axis.XYPlane.Y = plane.YAxis.Y;
      m_axis.XYPlane.Z = plane.YAxis.Z;
    }

    public GsaGridPlaneSurface Duplicate()
    {
      if (this == null) { return null; }
      GsaGridPlaneSurface dup = new GsaGridPlaneSurface
      {
        Plane = m_gridplane == null ? Plane.Unset : m_plane.Clone(),
        GridPlane = m_gridplane == null ? null : new GridPlane
        {
          AxisProperty = m_gridplane.AxisProperty,
          Elevation = m_gridplane.Elevation,
          IsStoreyType = m_gridplane.IsStoreyType,
          Name = m_gridplane.Name.ToString(),
          ToleranceAbove = m_gridplane.ToleranceAbove,
          ToleranceBelow = m_gridplane.ToleranceBelow
        },
        GridSurface = new GridSurface
        {
          Direction = m_gridsrf.Direction,
          Elements = m_gridsrf.Elements.ToString(),
          ElementType = m_gridsrf.ElementType,
          ExpansionType = m_gridsrf.ExpansionType,
          GridPlane = m_gridsrf.GridPlane,
          Name = m_gridsrf.Name.ToString(),
          SpanType = m_gridsrf.SpanType,
          Tolerance = m_gridsrf.Tolerance
        },
        Axis = m_gridplane == null ? null : new Axis
        {
          Name = m_axis.Name.ToString(),
          Origin = new Vector3 { X = m_axis.Origin.X, Y = m_axis.Origin.Y, Z = m_axis.Origin.Z },
          Type = m_axis.Type,
          XVector = new Vector3 { X = m_axis.XVector.X, Y = m_axis.XVector.Y, Z = m_axis.XVector.Z },
          XYPlane = new Vector3 { X = m_axis.XYPlane.X, Y = m_axis.XYPlane.Y, Z = m_axis.XYPlane.Z }
        },
        GridPlaneID = m_gridplnID,
        GridSurfaceID = m_gridsrfID
      };
      dup.m_gs_guid = new Guid(m_gs_guid.ToString());
      dup.m_gp_guid = new Guid(m_gp_guid.ToString());
      return dup;
    }


    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (Plane == null) { return false; }
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      if (this == null) { return "Null GridPlaneSurface"; }
      if (GridPlane == null && GridSurface == null) { return "Null GridPlaneSurface"; }
      string gp = GridPlane == null ? "" : GridPlane.Name + " ";
      string gs = GridSurface == null ? "" : GridSurface.Name;
      return "GridPlaneSurface " + gp + gs;
    }

    #endregion
  }
}
