using System;
using System.ComponentModel;
using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  #region internal load classes
  /// <summary>
  /// When referencing load by GsaGH object through Guid, use this to set the type of object
  /// </summary>
  internal enum ReferenceType
  {
    None,
    Section,
    Prop2d,
    Prop3d,
    Element,
    Member
  }

  /// <summary>
  /// Individual load type classes holding GsaAPI load type along with any required geometry objects
  /// </summary>
  /// 
  public class GsaGravityLoad
  {
    public GravityLoad GravityLoad { get; set; } = new GravityLoad();
    internal Guid RefObjectGuid;
    internal ReferenceType ReferenceType = ReferenceType.None;
    public GsaGravityLoad()
    {
      this.GravityLoad.Factor = new Vector3() { X = 0, Y = 0, Z = -1 };
      this.GravityLoad.Case = 1;
      this.GravityLoad.Elements = "all";
      this.GravityLoad.Nodes = "all";
    }

    public GsaGravityLoad Duplicate()
    {
      GsaGravityLoad dup = new GsaGravityLoad();
      dup.GravityLoad.Case = this.GravityLoad.Case;
      dup.GravityLoad.Elements = this.GravityLoad.Elements.ToString();
      dup.GravityLoad.Nodes = this.GravityLoad.Nodes.ToString();
      dup.GravityLoad.Name = this.GravityLoad.Name.ToString();
      dup.GravityLoad.Factor = this.GravityLoad.Factor;
      if (this.ReferenceType != ReferenceType.None)
      {
        dup.RefObjectGuid = new Guid(this.RefObjectGuid.ToString());
        dup.ReferenceType = this.ReferenceType;
      }
      return dup;
    }
  }

  public class GsaNodeLoad
  {
    public enum NodeLoadTypes // direct copy from GSA API enums
    {
      NODE_LOAD = 0,
      APPLIED_DISP = 1,
      SETTLEMENT = 2,
      GRAVITY = 3,
      NUM_TYPES = 4
    }

    public NodeLoadTypes Type;

    public NodeLoad NodeLoad { get; set; } = new NodeLoad();
    internal Point3d RefPoint;

    public GsaNodeLoad()
    {
      this.Type = NodeLoadTypes.NODE_LOAD;
    }

    public GsaNodeLoad Duplicate()
    {
      GsaNodeLoad dup = new GsaNodeLoad();
      dup.NodeLoad.AxisProperty = this.NodeLoad.AxisProperty;
      dup.NodeLoad.Case = this.NodeLoad.Case;
      dup.NodeLoad.Direction = this.NodeLoad.Direction;
      dup.NodeLoad.Nodes = this.NodeLoad.Nodes.ToString();
      dup.NodeLoad.Name = this.NodeLoad.Name.ToString();
      dup.NodeLoad.Value = this.NodeLoad.Value;
      dup.Type = Type;
      if (this.RefPoint != null)
        dup.RefPoint = new Point3d(this.RefPoint);
      return dup;
    }
  }

  public class GsaBeamLoad
  {
    public BeamLoad BeamLoad { get; set; }
    internal Guid RefObjectGuid;
    internal ReferenceType ReferenceType = ReferenceType.None;

    public GsaBeamLoad()
    {
      this.BeamLoad = new BeamLoad
      {
        Type = BeamLoadType.UNIFORM
      };
    }

    public GsaBeamLoad Duplicate()
    {
      GsaBeamLoad dup = new GsaBeamLoad();
      dup.BeamLoad.AxisProperty = this.BeamLoad.AxisProperty;
      dup.BeamLoad.Case = this.BeamLoad.Case;
      dup.BeamLoad.Direction = this.BeamLoad.Direction;
      dup.BeamLoad.Elements = this.BeamLoad.Elements.ToString();
      dup.BeamLoad.Name = this.BeamLoad.Name.ToString();
      dup.BeamLoad.IsProjected = this.BeamLoad.IsProjected;
      dup.BeamLoad.Type = this.BeamLoad.Type;
      if (BeamLoad.Type == BeamLoadType.POINT)
      {
        dup.BeamLoad.SetPosition(0, this.BeamLoad.Position(0));
        dup.BeamLoad.SetValue(0, this.BeamLoad.Value(0));
      }
      else if (BeamLoad.Type == BeamLoadType.UNIFORM)
      {
        dup.BeamLoad.SetValue(0, this.BeamLoad.Value(0));
      }
      else if (BeamLoad.Type == BeamLoadType.LINEAR)
      {
        dup.BeamLoad.SetValue(0, this.BeamLoad.Value(0));
        dup.BeamLoad.SetValue(1, this.BeamLoad.Value(1));
      }
      else if (BeamLoad.Type == BeamLoadType.PATCH)
      {
        dup.BeamLoad.SetPosition(0, this.BeamLoad.Position(0));
        dup.BeamLoad.SetPosition(1, this.BeamLoad.Position(1));
        dup.BeamLoad.SetValue(0, this.BeamLoad.Value(0));
        dup.BeamLoad.SetValue(1, this.BeamLoad.Value(1));
      }
      else if (BeamLoad.Type == BeamLoadType.TRILINEAR)
      {
        dup.BeamLoad.SetPosition(0, this.BeamLoad.Position(0));
        dup.BeamLoad.SetPosition(1, this.BeamLoad.Position(1));
        dup.BeamLoad.SetValue(0, this.BeamLoad.Value(0));
        dup.BeamLoad.SetValue(1, this.BeamLoad.Value(1));
      }
      if (this.ReferenceType != ReferenceType.None)
      {
        dup.RefObjectGuid = new Guid(this.RefObjectGuid.ToString());
        dup.ReferenceType = this.ReferenceType;
      }
      return dup;
    }
  }

  public class GsaFaceLoad
  {
    public FaceLoad FaceLoad { get; set; }
    internal Guid RefObjectGuid;
    internal ReferenceType ReferenceType = ReferenceType.None;
    public GsaFaceLoad()
    {
      this.FaceLoad = new FaceLoad
      {
        Type = FaceLoadType.CONSTANT
      };
    }

    public GsaFaceLoad Duplicate()
    {
      GsaFaceLoad dup = new GsaFaceLoad();
      dup.FaceLoad.AxisProperty = this.FaceLoad.AxisProperty;
      dup.FaceLoad.Case = this.FaceLoad.Case;
      dup.FaceLoad.Direction = this.FaceLoad.Direction;
      dup.FaceLoad.Elements = this.FaceLoad.Elements.ToString();
      dup.FaceLoad.Name = this.FaceLoad.Name.ToString();
      dup.FaceLoad.Type = this.FaceLoad.Type;
      if (this.FaceLoad.Type == FaceLoadType.CONSTANT)
      {
        dup.FaceLoad.IsProjected = this.FaceLoad.IsProjected;
        dup.FaceLoad.SetValue(0, this.FaceLoad.Value(0));
      }
      else if (this.FaceLoad.Type == FaceLoadType.GENERAL)
      {
        dup.FaceLoad.IsProjected = this.FaceLoad.IsProjected;
        dup.FaceLoad.SetValue(0, this.FaceLoad.Value(0));
        dup.FaceLoad.SetValue(1, this.FaceLoad.Value(1));
        dup.FaceLoad.SetValue(2, this.FaceLoad.Value(2));
        dup.FaceLoad.SetValue(3, this.FaceLoad.Value(3));
      }
      else if (this.FaceLoad.Type == FaceLoadType.POINT)
      {
        dup.FaceLoad.IsProjected = this.FaceLoad.IsProjected;
        dup.FaceLoad.SetValue(0, this.FaceLoad.Value(0));
        dup.FaceLoad.Position = this.FaceLoad.Position; // todo
                                                        //note Vector2 currently only get in GsaAPI
                                                        // duplicate Position.X and Position.Y when fixed
      }
      if (this.ReferenceType != ReferenceType.None)
      {
        dup.RefObjectGuid = new Guid(this.RefObjectGuid.ToString());
        dup.ReferenceType = this.ReferenceType;
      }
      return dup;
    }
  }

  public class GsaGridPointLoad
  {
    public GridPointLoad GridPointLoad { get; set; } = new GridPointLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    internal Point3d RefPoint;
    public GsaGridPointLoad()
    {
    }

    public GsaGridPointLoad Duplicate()
    {
      GsaGridPointLoad dup = new GsaGridPointLoad();
      dup.GridPointLoad.AxisProperty = this.GridPointLoad.AxisProperty;
      dup.GridPointLoad.Case = this.GridPointLoad.Case;
      dup.GridPointLoad.Direction = this.GridPointLoad.Direction;
      dup.GridPointLoad.GridSurface = this.GridPointLoad.GridSurface;
      dup.GridPointLoad.Name = this.GridPointLoad.Name.ToString();
      dup.GridPointLoad.X = this.GridPointLoad.X;
      dup.GridPointLoad.Y = this.GridPointLoad.Y;
      dup.GridPointLoad.Value = this.GridPointLoad.Value;
      dup.GridPlaneSurface = this.GridPlaneSurface.Duplicate();
      return dup;
    }
  }

  public class GsaGridLineLoad
  {
    public GridLineLoad GridLineLoad { get; set; } = new GridLineLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GsaGridLineLoad()
    {
      GridLineLoad.PolyLineReference = 0; // explicit type = 0
    }

    public GsaGridLineLoad Duplicate()
    {
      GsaGridLineLoad dup = new GsaGridLineLoad();
      dup.GridLineLoad.AxisProperty = this.GridLineLoad.AxisProperty;
      dup.GridLineLoad.Case = this.GridLineLoad.Case;
      dup.GridLineLoad.Direction = this.GridLineLoad.Direction;
      dup.GridLineLoad.GridSurface = this.GridLineLoad.GridSurface;
      dup.GridLineLoad.IsProjected = this.GridLineLoad.IsProjected;
      dup.GridLineLoad.Name = this.GridLineLoad.Name.ToString();
      dup.GridLineLoad.PolyLineDefinition = this.GridLineLoad.PolyLineDefinition.ToString();
      dup.GridLineLoad.PolyLineReference = this.GridLineLoad.PolyLineReference;
      dup.GridLineLoad.Type = this.GridLineLoad.Type;
      dup.GridLineLoad.ValueAtStart = this.GridLineLoad.ValueAtStart;
      dup.GridLineLoad.ValueAtEnd = this.GridLineLoad.ValueAtEnd;
      dup.GridPlaneSurface = this.GridPlaneSurface.Duplicate();
      return dup;
    }
  }

  public class GsaGridAreaLoad
  {
    public GridAreaLoad GridAreaLoad { get; set; } = new GridAreaLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GsaGridAreaLoad()
    {
      GridAreaLoad.Type = GridAreaPolyLineType.PLANE;
    }

    public GsaGridAreaLoad Duplicate()
    {
      GsaGridAreaLoad dup = new GsaGridAreaLoad();
      dup.GridAreaLoad.AxisProperty = this.GridAreaLoad.AxisProperty;
      dup.GridAreaLoad.Case = this.GridAreaLoad.Case;
      dup.GridAreaLoad.Direction = this.GridAreaLoad.Direction;
      dup.GridAreaLoad.GridSurface = this.GridAreaLoad.GridSurface;
      dup.GridAreaLoad.IsProjected = this.GridAreaLoad.IsProjected;
      dup.GridAreaLoad.Name = this.GridAreaLoad.Name.ToString();
      dup.GridAreaLoad.PolyLineDefinition = this.GridAreaLoad.PolyLineDefinition.ToString();
      dup.GridAreaLoad.PolyLineReference = this.GridAreaLoad.PolyLineReference;
      dup.GridAreaLoad.Type = this.GridAreaLoad.Type;
      dup.GridAreaLoad.Value = this.GridAreaLoad.Value;
      dup.GridPlaneSurface = this.GridPlaneSurface.Duplicate();
      return dup;
    }
  }
  #endregion

  /// <summary>
  /// GsaLoad class holding all load types
  /// </summary>
  public class GsaLoad
  {
    public enum LoadTypes
    {
      Gravity,
      Node,
      Beam,
      Face,
      GridPoint,
      GridLine,
      GridArea,
    }

    #region fields
    public LoadTypes LoadType = LoadTypes.Gravity;
    #endregion

    #region properties
    public GsaGravityLoad GravityLoad { get; set; }
    public GsaNodeLoad NodeLoad { get; set; }
    public GsaBeamLoad BeamLoad { get; set; }
    public GsaFaceLoad FaceLoad { get; set; }
    public GsaGridPointLoad PointLoad { get; set; }
    public GsaGridLineLoad LineLoad { get; set; }
    public GsaGridAreaLoad AreaLoad { get; set; }
    #endregion

    #region constructors
    public GsaLoad()
    {
      this.GravityLoad = new GsaGravityLoad();
      this.LoadType = LoadTypes.Gravity;
    }

    public GsaLoad(GsaGravityLoad gravityLoad)
    {
      this.GravityLoad = gravityLoad;
      this.LoadType = LoadTypes.Gravity;
    }

    public GsaLoad(GsaNodeLoad nodeLoad)
    {
      this.NodeLoad = nodeLoad;
      this.LoadType = LoadTypes.Node;
    }

    public GsaLoad(GsaBeamLoad beamLoad)
    {
      this.BeamLoad = beamLoad;
      this.LoadType = LoadTypes.Beam;
    }

    public GsaLoad(GsaFaceLoad faceLoad)
    {
      this.FaceLoad = faceLoad;
      this.LoadType = LoadTypes.Face;
    }

    public GsaLoad(GsaGridPointLoad gridPointLoad)
    {
      this.PointLoad = gridPointLoad;
      this.LoadType = LoadTypes.GridPoint;
    }

    public GsaLoad(GsaGridLineLoad gridLineLoad)
    {
      this.LineLoad = gridLineLoad;
      this.LoadType = LoadTypes.GridLine;
    }

    public GsaLoad(GsaGridAreaLoad gridAreaLoad)
    {
      this.AreaLoad = gridAreaLoad;
      this.LoadType = LoadTypes.GridArea;
    }
    #endregion

    #region methods
    public GsaLoad Duplicate()
    {
      GsaLoad dup;
      switch (this.LoadType)
      {
        case LoadTypes.Gravity:
          dup = new GsaLoad(this.GravityLoad.Duplicate());
          return dup;
        case LoadTypes.Node:
          dup = new GsaLoad(this.NodeLoad.Duplicate());
          return dup;
        case LoadTypes.Beam:
          dup = new GsaLoad(this.BeamLoad.Duplicate());
          return dup;
        case LoadTypes.Face:
          dup = new GsaLoad(this.FaceLoad.Duplicate());
          return dup;
        case LoadTypes.GridPoint:
          dup = new GsaLoad(this.PointLoad.Duplicate());
          if (this.PointLoad.GridPlaneSurface != null)
            dup.PointLoad.GridPlaneSurface = this.PointLoad.GridPlaneSurface.Duplicate();
          return dup;
        case LoadTypes.GridLine:
          dup = new GsaLoad(this.LineLoad.Duplicate());
          if (this.LineLoad.GridPlaneSurface != null)
            dup.LineLoad.GridPlaneSurface = this.LineLoad.GridPlaneSurface.Duplicate();
          return dup;
        case LoadTypes.GridArea:
          dup = new GsaLoad(this.AreaLoad.Duplicate());
          if (this.AreaLoad.GridPlaneSurface != null)
            dup.AreaLoad.GridPlaneSurface = this.AreaLoad.GridPlaneSurface.Duplicate();
          return dup;
      }
      return default;
    }

    public override string ToString()
    {
      if (this.LoadType == LoadTypes.Gravity && this.GravityLoad == null)
      {
        return "Null";
      }
      string name = "";
      switch (this.LoadType)
      {
        case LoadTypes.Gravity:
          name = this.GravityLoad.GravityLoad.Name;
          break;
        case LoadTypes.Node:
          name = this.NodeLoad.NodeLoad.Name;
          break;
        case LoadTypes.Beam:
          name = this.BeamLoad.BeamLoad.Name;
          break;
        case LoadTypes.Face:
          name = this.FaceLoad.FaceLoad.Name;
          break;
        case LoadTypes.GridPoint:
          name = this.PointLoad.GridPointLoad.Name;
          break;
        case LoadTypes.GridLine:
          name = this.LineLoad.GridLineLoad.Name;
          break;
        case LoadTypes.GridArea:
          name = this.AreaLoad.GridAreaLoad.Name;
          break;
      }

      return string.Join(" ", this.LoadType.ToString().Trim(), name.Trim()).Trim().Replace("  ", " ");
    }
    #endregion
  }
}
