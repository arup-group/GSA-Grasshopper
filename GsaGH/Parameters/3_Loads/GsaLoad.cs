namespace GsaGH.Parameters {

  /// <summary>
  ///   GsaLoad class holding all load types
  /// </summary>
  public class GsaLoad {
    /// <summary>
    /// When referencing load by GsaGH object through Guid, use this to set the type of object
    /// </summary>

    public enum LoadTypes {
      Gravity,
      Node,
      Beam,
      Face,
      GridPoint,
      GridLine,
      GridArea,
    }

    public GsaGridAreaLoad AreaLoad { get; set; }
    public GsaBeamLoad BeamLoad { get; set; }
    public GsaFaceLoad FaceLoad { get; set; }
    public GsaGravityLoad GravityLoad { get; set; }
    public GsaGridLineLoad LineLoad { get; set; }
    public GsaNodeLoad NodeLoad { get; set; }
    public GsaGridPointLoad PointLoad { get; set; }
    public LoadTypes LoadType = LoadTypes.Gravity;
    internal int CaseId {
      get {
        switch (LoadType) {
          case LoadTypes.Node:
            return NodeLoad.NodeLoad.Case;

          case LoadTypes.Beam:
            return BeamLoad.BeamLoad.Case;

          case LoadTypes.Face:
            return FaceLoad.FaceLoad.Case;

          case LoadTypes.GridPoint:
            return PointLoad.GridPointLoad.Case;

          case LoadTypes.GridLine:
            return LineLoad.GridLineLoad.Case;

          case LoadTypes.GridArea:
            return AreaLoad.GridAreaLoad.Case;

          case LoadTypes.Gravity:
          default:
            return GravityLoad.GravityLoad.Case;
        }
      }
    }

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
          if (PointLoad.GridPlaneSurface != null) {
            dup.PointLoad.GridPlaneSurface = PointLoad.GridPlaneSurface.Duplicate();
          }

          return dup;

        case LoadTypes.GridLine:
          dup = new GsaLoad(LineLoad.Duplicate());
          if (LineLoad.GridPlaneSurface != null) {
            dup.LineLoad.GridPlaneSurface = LineLoad.GridPlaneSurface.Duplicate();
          }

          return dup;

        case LoadTypes.GridArea:
          dup = new GsaLoad(AreaLoad.Duplicate());
          if (AreaLoad.GridPlaneSurface != null) {
            dup.AreaLoad.GridPlaneSurface = AreaLoad.GridPlaneSurface.Duplicate();
          }

          return dup;
      }

      return default;
    }

    public override string ToString() {
      if (LoadType == LoadTypes.Gravity && GravityLoad == null) {
        return "Null";
      }

      string name = string.Empty;
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
  }
}
