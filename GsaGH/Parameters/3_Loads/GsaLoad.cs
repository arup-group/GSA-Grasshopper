﻿using System.Collections.Generic;
using System.ComponentModel;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Individual load type classes holding GsaAPI load type along with any required geometry objects
  /// </summary>
  /// 

  #region internal load classes
  public class GsaGravityLoad
  {
    public GravityLoad GravityLoad
    {
      get { return m_gravityload; }
      set { m_gravityload = value; }
    }
    #region fields
    private GravityLoad m_gravityload;
    #endregion
    #region constructor
    public GsaGravityLoad()
    {
      m_gravityload = new GravityLoad();
    }
    public GsaGravityLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaGravityLoad dup = new GsaGravityLoad();
      dup.m_gravityload.Case = m_gravityload.Case;
      dup.m_gravityload.Elements = m_gravityload.Elements.ToString();
      dup.m_gravityload.Nodes = m_gravityload.Nodes.ToString();
      dup.m_gravityload.Name = m_gravityload.Name.ToString();
      dup.m_gravityload.Factor = m_gravityload.Factor;
      return dup;
    }
    #endregion
  }
  public class GsaNodeLoad
  {
    public NodeLoad NodeLoad
    {
      get { return m_nodeload; }
      set { m_nodeload = value; }
    }

    public NodeLoadTypes NodeLoadType;

    public enum NodeLoadTypes // direct copy from GSA API enums
    {
      NODE_LOAD = 0,
      APPLIED_DISP = 1,
      SETTLEMENT = 2,
      GRAVITY = 3,
      NUM_TYPES = 4
    }

    #region fields
    private NodeLoad m_nodeload;
    #endregion
    #region constructor
    public GsaNodeLoad()
    {
      m_nodeload = new NodeLoad();
      NodeLoadType = NodeLoadTypes.NODE_LOAD;
    }
    public GsaNodeLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaNodeLoad dup = new GsaNodeLoad();
      dup.m_nodeload.AxisProperty = m_nodeload.AxisProperty;
      dup.m_nodeload.Case = m_nodeload.Case;
      dup.m_nodeload.Direction = m_nodeload.Direction;
      dup.m_nodeload.Nodes = m_nodeload.Nodes.ToString();
      dup.m_nodeload.Name = m_nodeload.Name.ToString();
      dup.m_nodeload.Value = m_nodeload.Value;
      dup.NodeLoadType = NodeLoadType;
      return dup;
    }
    #endregion
  }
  public class GsaBeamLoad
  {
    public BeamLoad BeamLoad
    {
      get { return m_beamload; }
      set { m_beamload = value; }
    }
    #region fields
    private BeamLoad m_beamload;
    #endregion
    #region constructor
    public GsaBeamLoad()
    {
      m_beamload = new BeamLoad
      {
        Type = GsaAPI.BeamLoadType.UNIFORM
      };
    }
    public GsaBeamLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaBeamLoad dup = new GsaBeamLoad();
      dup.m_beamload.AxisProperty = m_beamload.AxisProperty;
      dup.m_beamload.Case = m_beamload.Case;
      dup.m_beamload.Direction = m_beamload.Direction;
      dup.m_beamload.Elements = m_beamload.Elements.ToString();
      dup.m_beamload.Name = m_beamload.Name.ToString();
      dup.m_beamload.IsProjected = m_beamload.IsProjected;
      dup.m_beamload.Type = m_beamload.Type;
      if (m_beamload.Type == BeamLoadType.POINT)
      {
        dup.m_beamload.SetPosition(0, m_beamload.Position(0));
        dup.m_beamload.SetValue(0, m_beamload.Value(0));
      }
      if (m_beamload.Type == BeamLoadType.UNIFORM)
      {
        dup.m_beamload.SetValue(0, m_beamload.Value(0));
      }
      if (m_beamload.Type == BeamLoadType.LINEAR)
      {
        dup.m_beamload.SetValue(0, m_beamload.Value(0));
        dup.m_beamload.SetValue(1, m_beamload.Value(1));
      }
      if (m_beamload.Type == BeamLoadType.PATCH)
      {
        dup.m_beamload.SetPosition(0, m_beamload.Position(0));
        dup.m_beamload.SetPosition(1, m_beamload.Position(1));
        dup.m_beamload.SetValue(0, m_beamload.Value(0));
        dup.m_beamload.SetValue(1, m_beamload.Value(1));
      }
      if (m_beamload.Type == BeamLoadType.TRILINEAR)
      {
        dup.m_beamload.SetPosition(0, m_beamload.Position(0));
        dup.m_beamload.SetPosition(1, m_beamload.Position(1));
        dup.m_beamload.SetValue(0, m_beamload.Value(0));
        dup.m_beamload.SetValue(1, m_beamload.Value(1));
      }
      return dup;
    }
    #endregion
  }
  public class GsaFaceLoad
  {
    public FaceLoad FaceLoad
    {
      get { return m_faceload; }
      set { m_faceload = value; }
    }

    #region fields
    private FaceLoad m_faceload;
    #endregion
    #region constructor
    public GsaFaceLoad()
    {
      m_faceload = new FaceLoad
      {
        Type = FaceLoadType.CONSTANT
      };
    }
    public GsaFaceLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaFaceLoad dup = new GsaFaceLoad();
      dup.m_faceload.AxisProperty = m_faceload.AxisProperty;
      dup.m_faceload.Case = m_faceload.Case;
      dup.m_faceload.Direction = m_faceload.Direction;
      dup.m_faceload.Elements = m_faceload.Elements.ToString();
      dup.m_faceload.Name = m_faceload.Name.ToString();
      dup.m_faceload.Type = m_faceload.Type;
      if (m_faceload.Type == FaceLoadType.CONSTANT)
      {
        dup.m_faceload.IsProjected = m_faceload.IsProjected;
        dup.m_faceload.SetValue(0, m_faceload.Value(0));
      }
      if (m_faceload.Type == FaceLoadType.GENERAL)
      {
        dup.m_faceload.IsProjected = m_faceload.IsProjected;
        dup.m_faceload.SetValue(0, m_faceload.Value(0));
        dup.m_faceload.SetValue(1, m_faceload.Value(1));
        dup.m_faceload.SetValue(2, m_faceload.Value(2));
        dup.m_faceload.SetValue(3, m_faceload.Value(3));
      }
      if (m_faceload.Type == FaceLoadType.POINT)
      {
        dup.m_faceload.IsProjected = m_faceload.IsProjected;
        dup.m_faceload.SetValue(0, m_faceload.Value(0));
        dup.m_faceload.Position = m_faceload.Position; // 
                                                       //note Vector2 currently only get in GsaAPI
                                                       // duplicate Position.X and Position.Y when fixed
      }
      return dup;
    }
    #endregion
  }
  public class GsaGridPointLoad
  {
    public GridPointLoad GridPointLoad
    {
      get { return m_gridpointload; }
      set { m_gridpointload = value; }
    }
    public GsaGridPlaneSurface GridPlaneSurface
    {
      get { return m_gridplanesrf; }
      set { m_gridplanesrf = value; }
    }
    #region fields
    private GridPointLoad m_gridpointload;
    private GsaGridPlaneSurface m_gridplanesrf;
    #endregion
    #region constructor
    public GsaGridPointLoad()
    {
      m_gridpointload = new GridPointLoad();
      m_gridplanesrf = new GsaGridPlaneSurface();
    }
    public GsaGridPointLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaGridPointLoad dup = new GsaGridPointLoad();
      dup.m_gridpointload.AxisProperty = m_gridpointload.AxisProperty;
      dup.m_gridpointload.Case = m_gridpointload.Case;
      dup.m_gridpointload.Direction = m_gridpointload.Direction;
      dup.m_gridpointload.GridSurface = m_gridpointload.GridSurface;
      dup.m_gridpointload.Name = m_gridpointload.Name.ToString();
      dup.m_gridpointload.X = m_gridpointload.X;
      dup.m_gridpointload.Y = m_gridpointload.Y;
      dup.m_gridpointload.Value = m_gridpointload.Value;
      if (m_gridplanesrf != null)
        dup.m_gridplanesrf = m_gridplanesrf.Duplicate();
      else
        dup.m_gridplanesrf = null;
      return dup;
    }
    #endregion
  }
  public class GsaGridLineLoad
  {
    public GridLineLoad GridLineLoad
    {
      get { return m_gridlineload; }
      set { m_gridlineload = value; }
    }

    public GsaGridPlaneSurface GridPlaneSurface
    {
      get { return m_gridplanesrf; }
      set { m_gridplanesrf = value; }
    }

    #region fields
    private GridLineLoad m_gridlineload;
    private GsaGridPlaneSurface m_gridplanesrf;
    #endregion
    #region constructor
    public GsaGridLineLoad()
    {
      m_gridlineload = new GridLineLoad();
      m_gridlineload.PolyLineReference = 0; // explicit type = 0
      m_gridplanesrf = new GsaGridPlaneSurface();
    }
    public GsaGridLineLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaGridLineLoad dup = new GsaGridLineLoad();
      dup.m_gridlineload.AxisProperty = m_gridlineload.AxisProperty;
      dup.m_gridlineload.Case = m_gridlineload.Case;
      dup.m_gridlineload.Direction = m_gridlineload.Direction;
      dup.m_gridlineload.GridSurface = m_gridlineload.GridSurface;
      dup.m_gridlineload.IsProjected = m_gridlineload.IsProjected;
      dup.m_gridlineload.Name = m_gridlineload.Name.ToString();
      dup.m_gridlineload.PolyLineDefinition = m_gridlineload.PolyLineDefinition.ToString();
      dup.m_gridlineload.PolyLineReference = m_gridlineload.PolyLineReference;
      dup.m_gridlineload.Type = m_gridlineload.Type;
      dup.m_gridlineload.ValueAtStart = m_gridlineload.ValueAtStart;
      dup.m_gridlineload.ValueAtEnd = m_gridlineload.ValueAtEnd;
      if (m_gridplanesrf != null)
        dup.m_gridplanesrf = m_gridplanesrf.Duplicate();
      else
        dup.m_gridplanesrf = null;
      return dup;
    }
    #endregion
  }
  public class GsaGridAreaLoad
  {
    public GridAreaLoad GridAreaLoad
    {
      get { return m_gridareaload; }
      set { m_gridareaload = value; }
    }
    public GsaGridPlaneSurface GridPlaneSurface
    {
      get { return m_gridplanesrf; }
      set { m_gridplanesrf = value; }
    }
    #region fields
    private GridAreaLoad m_gridareaload;
    private GsaGridPlaneSurface m_gridplanesrf;
    #endregion
    #region constructor
    public GsaGridAreaLoad()
    {
      m_gridareaload = new GridAreaLoad();
      m_gridareaload.Type = GsaAPI.GridAreaPolyLineType.PLANE;
      m_gridplanesrf = new GsaGridPlaneSurface();
    }
    public GsaGridAreaLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaGridAreaLoad dup = new GsaGridAreaLoad();
      dup.m_gridareaload.AxisProperty = m_gridareaload.AxisProperty;
      dup.m_gridareaload.Case = m_gridareaload.Case;
      dup.m_gridareaload.Direction = m_gridareaload.Direction;
      dup.m_gridareaload.GridSurface = m_gridareaload.GridSurface;
      dup.m_gridareaload.IsProjected = m_gridareaload.IsProjected;
      dup.m_gridareaload.Name = m_gridareaload.Name.ToString();
      dup.m_gridareaload.PolyLineDefinition = m_gridareaload.PolyLineDefinition.ToString();
      dup.m_gridareaload.PolyLineReference = m_gridareaload.PolyLineReference;
      dup.m_gridareaload.Type = m_gridareaload.Type;
      dup.m_gridareaload.Value = m_gridareaload.Value;
      if (m_gridplanesrf != null)
        dup.m_gridplanesrf = m_gridplanesrf.Duplicate();
      else
        dup.m_gridplanesrf = null;
      return dup;
    }
    #endregion
  }
  #endregion
  /// <summary>
  /// GsaLoad class holding all load types
  /// </summary>
  public class GsaLoad
  {
    public enum LoadTypes
    {
      [Description("Gravity Load")] Gravity,
      [Description("Node Load")] Node,
      [Description("Beam Load")] Beam,
      [Description("Face Load")] Face,
      [Description("Grid Point Load")] GridPoint,
      [Description("Grid Line Load")] GridLine,
      [Description("Grid Area Load")] GridArea,
    }
    public LoadTypes LoadType;

    public GsaGravityLoad GravityLoad
    {
      get { return m_gravity; }
      set { m_gravity = value; }
    }
    public GsaNodeLoad NodeLoad
    {
      get { return m_node; }
      set { m_node = value; }
    }
    public GsaBeamLoad BeamLoad
    {
      get { return m_beam; }
      set { m_beam = value; }
    }
    public GsaFaceLoad FaceLoad
    {
      get { return m_face; }
      set { m_face = value; }
    }
    public GsaGridPointLoad PointLoad
    {
      get { return m_gridpoint; }
      set { m_gridpoint = value; }
    }
    public GsaGridLineLoad LineLoad
    {
      get { return m_gridline; }
      set { m_gridline = value; }
    }
    public GsaGridAreaLoad AreaLoad
    {
      get { return m_gridarea; }
      set { m_gridarea = value; }
    }
    #region fields
    private GsaGravityLoad m_gravity;
    private GsaNodeLoad m_node;
    private GsaBeamLoad m_beam;
    private GsaFaceLoad m_face;
    private GsaGridPointLoad m_gridpoint;
    private GsaGridLineLoad m_gridline;
    private GsaGridAreaLoad m_gridarea;
    #endregion

    #region constructors
    public GsaLoad()
    {
      // empty constructor
    }

    public GsaLoad(GsaGravityLoad gravityload)
    {
      GravityLoad = gravityload;
      LoadType = LoadTypes.Gravity;
    }

    public GsaLoad(GsaNodeLoad nodeload)
    {
      NodeLoad = nodeload;
      LoadType = LoadTypes.Node;
    }

    public GsaLoad(GsaBeamLoad beamload)
    {
      BeamLoad = beamload;
      LoadType = LoadTypes.Beam;
    }

    public GsaLoad(GsaFaceLoad faceload)
    {
      FaceLoad = faceload;
      LoadType = LoadTypes.Face;
    }

    public GsaLoad(GsaGridPointLoad gridpointload)
    {
      PointLoad = gridpointload;
      LoadType = LoadTypes.GridPoint;
    }

    public GsaLoad(GsaGridLineLoad gridlineload)
    {
      LineLoad = gridlineload;
      LoadType = LoadTypes.GridLine;
    }

    public GsaLoad(GsaGridAreaLoad gridareaload)
    {
      AreaLoad = gridareaload;
      LoadType = LoadTypes.GridArea;
    }

    public GsaLoad Duplicate()
    {
      if (this == null) { return null; }
      GsaLoad dup;
      switch (this.LoadType)
      {
        case LoadTypes.Gravity:
          dup = new GsaLoad(m_gravity.Duplicate());
          return dup;
        case LoadTypes.Node:
          dup = new GsaLoad(m_node.Duplicate());
          return dup;
        case LoadTypes.Beam:
          dup = new GsaLoad(m_beam.Duplicate());
          return dup;
        case LoadTypes.Face:
          dup = new GsaLoad(m_face.Duplicate());
          return dup;
        case LoadTypes.GridPoint:
          dup = new GsaLoad(m_gridpoint.Duplicate());
          if (m_gridpoint.GridPlaneSurface != null)
            dup.PointLoad.GridPlaneSurface = m_gridpoint.GridPlaneSurface.Duplicate();
          return dup;
        case LoadTypes.GridLine:
          dup = new GsaLoad(m_gridline.Duplicate());
          if (m_gridline.GridPlaneSurface != null)
            dup.LineLoad.GridPlaneSurface = m_gridline.GridPlaneSurface.Duplicate();
          return dup;
        case LoadTypes.GridArea:
          dup = new GsaLoad(m_gridarea.Duplicate());
          if (m_gridarea.GridPlaneSurface != null)
            dup.AreaLoad.GridPlaneSurface = m_gridarea.GridPlaneSurface.Duplicate();
          return dup;
      }
      return default;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      if (LoadType == LoadTypes.Gravity && this.GravityLoad == null) { return "Null Load"; }
      string name = "";
      switch (LoadType)
      {
        case LoadTypes.Gravity:
          name = GravityLoad.GravityLoad.Name;
          break;
        case LoadTypes.Node:
          name = NodeLoad.NodeLoad.Name;
          break;
        case LoadTypes.Beam:
          name = BeamLoad.BeamLoad.Name;
          break;
        case LoadTypes.Face:
          name = FaceLoad.FaceLoad.Name;
          break;
        case LoadTypes.GridPoint:
          name = PointLoad.GridPointLoad.Name;
          break;
        case LoadTypes.GridLine:
          name = LineLoad.GridLineLoad.Name;
          break;
        case LoadTypes.GridArea:
          name = AreaLoad.GridAreaLoad.Name;
          break;
      }
      if (name == "")
        name = " Load";
      else
        name = " " + name;

      return "GSA " + LoadType.ToString() + name;
    }

    #endregion
  }
}
