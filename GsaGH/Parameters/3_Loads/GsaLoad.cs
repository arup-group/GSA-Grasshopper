using System;
using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  #region internal load classes
  /// <summary>
  /// When referencing load by GsaGH object through Guid, use this to set the type of object
  /// </summary>
  internal enum ReferenceType {
    None,
    Section,
    Prop2d,
    Prop3d,
    Element,
    Member,
  }

  /// <summary>
  /// Individual load type classes holding GsaAPI load type along with any required geometry objects
  /// </summary>
  /// 
  public class GsaGravityLoad {
    public GravityLoad GravityLoad { get; set; } = new GravityLoad();
    internal Guid RefObjectGuid;
    internal ReferenceType ReferenceType = ReferenceType.None;
    public GsaGravityLoad() {
      GravityLoad.Factor = new Vector3() { X = 0, Y = 0, Z = -1 };
      GravityLoad.Case = 1;
      GravityLoad.Elements = "all";
      GravityLoad.Nodes = "all";
    }

    public GsaGravityLoad Duplicate() {
      var dup = new GsaGravityLoad();
      dup.GravityLoad.Case = GravityLoad.Case;
      dup.GravityLoad.Elements = GravityLoad.Elements.ToString();
      dup.GravityLoad.Nodes = GravityLoad.Nodes.ToString();
      dup.GravityLoad.Name = GravityLoad.Name.ToString();
      dup.GravityLoad.Factor = GravityLoad.Factor;
      if (ReferenceType != ReferenceType.None) {
        dup.RefObjectGuid = new Guid(RefObjectGuid.ToString());
        dup.ReferenceType = ReferenceType;
      }
      return dup;
    }
  }

  public class GsaNodeLoad {
    public enum NodeLoadTypes // direct copy from GSA API enums
    {
      NodeLoad = 0,
      AppliedDisp = 1,
      Settlement = 2,
      Gravity = 3,
      NumTypes = 4,
    }

    public NodeLoadTypes Type;

    public NodeLoad NodeLoad { get; set; } = new NodeLoad();
    internal Point3d RefPoint = Point3d.Unset;

    public GsaNodeLoad() {
      Type = NodeLoadTypes.NodeLoad;
    }

    public GsaNodeLoad Duplicate() {
      var dup = new GsaNodeLoad();
      dup.NodeLoad.AxisProperty = NodeLoad.AxisProperty;
      dup.NodeLoad.Case = NodeLoad.Case;
      dup.NodeLoad.Direction = NodeLoad.Direction;
      dup.NodeLoad.Nodes = NodeLoad.Nodes.ToString();
      dup.NodeLoad.Name = NodeLoad.Name.ToString();
      dup.NodeLoad.Value = NodeLoad.Value;
      dup.Type = Type;
      if (RefPoint != Point3d.Unset)
        dup.RefPoint = new Point3d(RefPoint);
      return dup;
    }
  }

  public class GsaBeamLoad {
    public BeamLoad BeamLoad {
      get; set;
    }
    internal Guid RefObjectGuid;
    internal ReferenceType ReferenceType = ReferenceType.None;

    public GsaBeamLoad() {
      BeamLoad = new BeamLoad {
        Type = BeamLoadType.UNIFORM
      };
    }

    public GsaBeamLoad Duplicate() {
      var dup = new GsaBeamLoad();
      dup.BeamLoad.AxisProperty = BeamLoad.AxisProperty;
      dup.BeamLoad.Case = BeamLoad.Case;
      dup.BeamLoad.Direction = BeamLoad.Direction;
      dup.BeamLoad.Elements = BeamLoad.Elements.ToString();
      dup.BeamLoad.Name = BeamLoad.Name.ToString();
      dup.BeamLoad.IsProjected = BeamLoad.IsProjected;
      dup.BeamLoad.Type = BeamLoad.Type;
      if (BeamLoad.Type == BeamLoadType.POINT) {
        dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
        dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
      }
      else if (BeamLoad.Type == BeamLoadType.UNIFORM) {
        dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
      }
      else if (BeamLoad.Type == BeamLoadType.LINEAR) {
        dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
        dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
      }
      else if (BeamLoad.Type == BeamLoadType.PATCH) {
        dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
        dup.BeamLoad.SetPosition(1, BeamLoad.Position(1));
        dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
        dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
      }
      else if (BeamLoad.Type == BeamLoadType.TRILINEAR) {
        dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
        dup.BeamLoad.SetPosition(1, BeamLoad.Position(1));
        dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
        dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
      }
      if (ReferenceType != ReferenceType.None) {
        dup.RefObjectGuid = new Guid(RefObjectGuid.ToString());
        dup.ReferenceType = ReferenceType;
      }
      return dup;
    }
  }

  public class GsaFaceLoad {
    public FaceLoad FaceLoad {
      get; set;
    }
    internal Guid RefObjectGuid;
    internal ReferenceType ReferenceType = ReferenceType.None;
    public GsaFaceLoad() {
      FaceLoad = new FaceLoad {
        Type = FaceLoadType.CONSTANT
      };
    }

    public GsaFaceLoad Duplicate() {
      var dup = new GsaFaceLoad();
      dup.FaceLoad.AxisProperty = FaceLoad.AxisProperty;
      dup.FaceLoad.Case = FaceLoad.Case;
      dup.FaceLoad.Direction = FaceLoad.Direction;
      dup.FaceLoad.Elements = FaceLoad.Elements.ToString();
      dup.FaceLoad.Name = FaceLoad.Name.ToString();
      dup.FaceLoad.Type = FaceLoad.Type;
      if (FaceLoad.Type == FaceLoadType.CONSTANT) {
        dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
        dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
      }
      else if (FaceLoad.Type == FaceLoadType.GENERAL) {
        dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
        dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
        dup.FaceLoad.SetValue(1, FaceLoad.Value(1));
        dup.FaceLoad.SetValue(2, FaceLoad.Value(2));
        dup.FaceLoad.SetValue(3, FaceLoad.Value(3));
      }
      else if (FaceLoad.Type == FaceLoadType.POINT) {
        dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
        dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
        dup.FaceLoad.Position = FaceLoad.Position; // todo
                                                   //note Vector2 currently only get in GsaAPI
                                                   // duplicate Position.X and Position.Y when fixed
      }
      if (ReferenceType != ReferenceType.None) {
        dup.RefObjectGuid = new Guid(RefObjectGuid.ToString());
        dup.ReferenceType = ReferenceType;
      }
      return dup;
    }
  }

  public class GsaGridPointLoad {
    public GridPointLoad GridPointLoad { get; set; } = new GridPointLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    internal Point3d RefPoint;
    public GsaGridPointLoad() {
    }

    public GsaGridPointLoad Duplicate() {
      var dup = new GsaGridPointLoad();
      dup.GridPointLoad.AxisProperty = GridPointLoad.AxisProperty;
      dup.GridPointLoad.Case = GridPointLoad.Case;
      dup.GridPointLoad.Direction = GridPointLoad.Direction;
      dup.GridPointLoad.GridSurface = GridPointLoad.GridSurface;
      dup.GridPointLoad.Name = GridPointLoad.Name.ToString();
      dup.GridPointLoad.X = GridPointLoad.X;
      dup.GridPointLoad.Y = GridPointLoad.Y;
      dup.GridPointLoad.Value = GridPointLoad.Value;
      dup.GridPlaneSurface = GridPlaneSurface.Duplicate();
      return dup;
    }
  }

  public class GsaGridLineLoad {
    public GridLineLoad GridLineLoad { get; set; } = new GridLineLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GsaGridLineLoad() {
      GridLineLoad.PolyLineReference = 0; // explicit type = 0
    }

    public GsaGridLineLoad Duplicate() {
      var dup = new GsaGridLineLoad();
      dup.GridLineLoad.AxisProperty = GridLineLoad.AxisProperty;
      dup.GridLineLoad.Case = GridLineLoad.Case;
      dup.GridLineLoad.Direction = GridLineLoad.Direction;
      dup.GridLineLoad.GridSurface = GridLineLoad.GridSurface;
      dup.GridLineLoad.IsProjected = GridLineLoad.IsProjected;
      dup.GridLineLoad.Name = GridLineLoad.Name.ToString();
      dup.GridLineLoad.PolyLineDefinition = GridLineLoad.PolyLineDefinition.ToString();
      dup.GridLineLoad.PolyLineReference = GridLineLoad.PolyLineReference;
      dup.GridLineLoad.Type = GridLineLoad.Type;
      dup.GridLineLoad.ValueAtStart = GridLineLoad.ValueAtStart;
      dup.GridLineLoad.ValueAtEnd = GridLineLoad.ValueAtEnd;
      dup.GridPlaneSurface = GridPlaneSurface.Duplicate();
      return dup;
    }
  }

  public class GsaGridAreaLoad {
    public GridAreaLoad GridAreaLoad { get; set; } = new GridAreaLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GsaGridAreaLoad() {
      GridAreaLoad.Type = GridAreaPolyLineType.PLANE;
    }

    public GsaGridAreaLoad Duplicate() {
      var dup = new GsaGridAreaLoad();
      dup.GridAreaLoad.AxisProperty = GridAreaLoad.AxisProperty;
      dup.GridAreaLoad.Case = GridAreaLoad.Case;
      dup.GridAreaLoad.Direction = GridAreaLoad.Direction;
      dup.GridAreaLoad.GridSurface = GridAreaLoad.GridSurface;
      dup.GridAreaLoad.IsProjected = GridAreaLoad.IsProjected;
      dup.GridAreaLoad.Name = GridAreaLoad.Name.ToString();
      dup.GridAreaLoad.PolyLineDefinition = GridAreaLoad.PolyLineDefinition.ToString();
      dup.GridAreaLoad.PolyLineReference = GridAreaLoad.PolyLineReference;
      dup.GridAreaLoad.Type = GridAreaLoad.Type;
      dup.GridAreaLoad.Value = GridAreaLoad.Value;
      dup.GridPlaneSurface = GridPlaneSurface.Duplicate();
      return dup;
    }
  }
  #endregion

  /// <summary>
  /// GsaLoad class holding all load types
  /// </summary>
  public class GsaLoad {
    public enum LoadTypes {
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
    public GsaGravityLoad GravityLoad {
      get; set;
    }
    public GsaNodeLoad NodeLoad {
      get; set;
    }
    public GsaBeamLoad BeamLoad {
      get; set;
    }
    public GsaFaceLoad FaceLoad {
      get; set;
    }
    public GsaGridPointLoad PointLoad {
      get; set;
    }
    public GsaGridLineLoad LineLoad {
      get; set;
    }
    public GsaGridAreaLoad AreaLoad {
      get; set;
    }
    #endregion

    #region constructors
    public GsaLoad() {
      GravityLoad = new GsaGravityLoad();
      LoadType = LoadTypes.Gravity;
    }

    public GsaLoad(GsaGravityLoad gravityLoad) {
      GravityLoad = gravityLoad;
      LoadType = LoadTypes.Gravity;
    }

    public GsaLoad(GsaNodeLoad nodeLoad) {
      NodeLoad = nodeLoad;
      LoadType = LoadTypes.Node;
    }

    public GsaLoad(GsaBeamLoad beamLoad) {
      BeamLoad = beamLoad;
      LoadType = LoadTypes.Beam;
    }

    public GsaLoad(GsaFaceLoad faceLoad) {
      FaceLoad = faceLoad;
      LoadType = LoadTypes.Face;
    }

    public GsaLoad(GsaGridPointLoad gridPointLoad) {
      PointLoad = gridPointLoad;
      LoadType = LoadTypes.GridPoint;
    }

    public GsaLoad(GsaGridLineLoad gridLineLoad) {
      LineLoad = gridLineLoad;
      LoadType = LoadTypes.GridLine;
    }

    public GsaLoad(GsaGridAreaLoad gridAreaLoad) {
      AreaLoad = gridAreaLoad;
      LoadType = LoadTypes.GridArea;
    }
    #endregion

    #region methods
    public GsaLoad Duplicate() {
      GsaLoad dup;
      switch (LoadType) {
        case LoadTypes.Gravity:
          dup = new GsaLoad(GravityLoad.Duplicate());
          return dup;
        case LoadTypes.Node:
          dup = new GsaLoad(NodeLoad.Duplicate());
          return dup;
        case LoadTypes.Beam:
          dup = new GsaLoad(BeamLoad.Duplicate());
          return dup;
        case LoadTypes.Face:
          dup = new GsaLoad(FaceLoad.Duplicate());
          return dup;
        case LoadTypes.GridPoint:
          dup = new GsaLoad(PointLoad.Duplicate());
          if (PointLoad.GridPlaneSurface != null)
            dup.PointLoad.GridPlaneSurface = PointLoad.GridPlaneSurface.Duplicate();
          return dup;
        case LoadTypes.GridLine:
          dup = new GsaLoad(LineLoad.Duplicate());
          if (LineLoad.GridPlaneSurface != null)
            dup.LineLoad.GridPlaneSurface = LineLoad.GridPlaneSurface.Duplicate();
          return dup;
        case LoadTypes.GridArea:
          dup = new GsaLoad(AreaLoad.Duplicate());
          if (AreaLoad.GridPlaneSurface != null)
            dup.AreaLoad.GridPlaneSurface = AreaLoad.GridPlaneSurface.Duplicate();
          return dup;
      }
      return default;
    }

    public override string ToString() {
      if (LoadType == LoadTypes.Gravity && GravityLoad == null) {
        return "Null";
      }
      string name = "";
      switch (LoadType) {
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

      return string.Join(" ", LoadType.ToString().Trim(), name.Trim()).Trim().Replace("  ", " ");
    }
    #endregion
  }
}
