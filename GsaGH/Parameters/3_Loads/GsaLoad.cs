﻿using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public class GsaBeamLoad {
    public BeamLoad BeamLoad { get; set; }
    internal ReferenceType _referenceType = ReferenceType.None;
    internal GsaList _refList;
    internal Guid _refObjectGuid;

    public GsaBeamLoad() {
      BeamLoad = new BeamLoad {
        Type = BeamLoadType.UNIFORM,
      };
    }

    public GsaBeamLoad Duplicate() {
      var dup = new GsaBeamLoad {
        BeamLoad = {
          AxisProperty = BeamLoad.AxisProperty,
          Case = BeamLoad.Case,
          Direction = BeamLoad.Direction,
          Elements = BeamLoad.Elements.ToString(),
          Name = BeamLoad.Name.ToString(),
          IsProjected = BeamLoad.IsProjected,
          Type = BeamLoad.Type,
        },
      };
      switch (BeamLoad.Type) {
        case BeamLoadType.POINT:
          dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          break;

        case BeamLoadType.UNIFORM:
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          break;

        case BeamLoadType.LINEAR:
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
          break;

        case BeamLoadType.PATCH:
          dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
          dup.BeamLoad.SetPosition(1, BeamLoad.Position(1));
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
          break;

        case BeamLoadType.TRILINEAR:
          dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
          dup.BeamLoad.SetPosition(1, BeamLoad.Position(1));
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
          break;
      }

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
  }

  public class GsaFaceLoad {
    public FaceLoad FaceLoad { get; set; }
    internal ReferenceType _referenceType = ReferenceType.None;
    internal GsaList _refList;
    internal Guid _refObjectGuid;

    public GsaFaceLoad() {
      FaceLoad = new FaceLoad {
        Type = FaceLoadType.CONSTANT,
      };
    }

    public GsaFaceLoad Duplicate() {
      var dup = new GsaFaceLoad {
        FaceLoad = {
          AxisProperty = FaceLoad.AxisProperty,
          Case = FaceLoad.Case,
          Direction = FaceLoad.Direction,
          Elements = FaceLoad.Elements.ToString(),
          Name = FaceLoad.Name.ToString(),
          Type = FaceLoad.Type,
        },
      };
      switch (FaceLoad.Type) {
        case FaceLoadType.CONSTANT:
          dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
          dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
          break;

        case FaceLoadType.GENERAL:
          dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
          dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
          dup.FaceLoad.SetValue(1, FaceLoad.Value(1));
          dup.FaceLoad.SetValue(2, FaceLoad.Value(2));
          dup.FaceLoad.SetValue(3, FaceLoad.Value(3));
          break;

        case FaceLoadType.POINT:
          dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
          dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
          dup.FaceLoad.Position = FaceLoad.Position; // todo
          //note Vector2 currently only get in GsaAPI
          // duplicate Position.X and Position.Y when fixed
          break;
      }

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
  }

  /// <summary>
  ///   Individual load type classes holding GsaAPI load type along with any required geometry objects
  /// </summary>
  public class GsaGravityLoad {
    public GravityLoad GravityLoad { get; set; } = new GravityLoad();
    internal ReferenceType _referenceType = ReferenceType.None;
    internal GsaList _refList;
    internal Guid _refObjectGuid;

    public GsaGravityLoad() {
      GravityLoad.Factor = new Vector3() {
        X = 0,
        Y = 0,
        Z = -1,
      };
      GravityLoad.Case = 1;
      GravityLoad.Elements = "all";
      GravityLoad.Nodes = "all";
    }

    public GsaGravityLoad Duplicate() {
      var dup = new GsaGravityLoad {
        GravityLoad = {
          Case = GravityLoad.Case,
          Elements = GravityLoad.Elements.ToString(),
          Nodes = GravityLoad.Nodes.ToString(),
          Name = GravityLoad.Name.ToString(),
          Factor = GravityLoad.Factor,
        },
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
  }

  public class GsaGridAreaLoad {
    public GridAreaLoad GridAreaLoad { get; set; } = new GridAreaLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    internal List<Point3d> Points { get; set; } = new List<Point3d>();

    public GsaGridAreaLoad() {
      GridAreaLoad.Type = GridAreaPolyLineType.PLANE;
    }
    public GsaGridAreaLoad Duplicate() {
      var dup = new GsaGridAreaLoad {
        GridAreaLoad = {
          AxisProperty = GridAreaLoad.AxisProperty,
          Case = GridAreaLoad.Case,
          Direction = GridAreaLoad.Direction,
          GridSurface = GridAreaLoad.GridSurface,
          IsProjected = GridAreaLoad.IsProjected,
          Name = GridAreaLoad.Name.ToString(),
          PolyLineDefinition = GridAreaLoad.PolyLineDefinition.ToString(),
          PolyLineReference = GridAreaLoad.PolyLineReference,
          Type = GridAreaLoad.Type,
          Value = GridAreaLoad.Value,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
        Points = Points.ToList(),
      };
      return dup;
    }
  }

  public class GsaGridLineLoad {
    public GridLineLoad GridLineLoad { get; set; } = new GridLineLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    internal List<Point3d> Points { get; set; } = new List<Point3d>();

    public GsaGridLineLoad() {
      GridLineLoad.PolyLineReference = 0;
    }
    public GsaGridLineLoad Duplicate() {
      var dup = new GsaGridLineLoad {
        GridLineLoad = {
          AxisProperty = GridLineLoad.AxisProperty,
          Case = GridLineLoad.Case,
          Direction = GridLineLoad.Direction,
          GridSurface = GridLineLoad.GridSurface,
          IsProjected = GridLineLoad.IsProjected,
          Name = GridLineLoad.Name.ToString(),
          PolyLineDefinition = GridLineLoad.PolyLineDefinition.ToString(),
          PolyLineReference = GridLineLoad.PolyLineReference,
          Type = GridLineLoad.Type,
          ValueAtStart = GridLineLoad.ValueAtStart,
          ValueAtEnd = GridLineLoad.ValueAtEnd,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
        Points = Points.ToList(),
      };
      return dup;
    }
  }

  public class GsaGridPointLoad {
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GridPointLoad GridPointLoad { get; set; } = new GridPointLoad();
    public GsaGridPointLoad() { }

    public GsaGridPointLoad Duplicate() {
      var dup = new GsaGridPointLoad {
        GridPointLoad = {
          AxisProperty = GridPointLoad.AxisProperty,
          Case = GridPointLoad.Case,
          Direction = GridPointLoad.Direction,
          GridSurface = GridPointLoad.GridSurface,
          Name = GridPointLoad.Name.ToString(),
          X = GridPointLoad.X,
          Y = GridPointLoad.Y,
          Value = GridPointLoad.Value,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
      };
      return dup;
    }

    internal Point3d GetPoint(LengthUnit unit) {
      LengthUnit m = LengthUnit.Meter;
      return new Point3d(
              new Length(GridPointLoad.X, m).As(unit),
              new Length(GridPointLoad.Y, m).As(unit),
              new Length(GridPlaneSurface.Axis.OriginZ, m).As(unit));
    }
  }

  internal static class GridLoadHelper {
    internal static (List<Point3d>, string) CreateDefinition(List<Point3d> controlPoints, Plane plane) {
      string desc = string.Empty;
      var points = new List<Point3d>();
      for (int i = 0; i < controlPoints.Count; i++) {
        if (i > 0) {
          desc += " ";
        }

        plane.RemapToPlaneSpace(controlPoints[i], out Point3d temppt);
        // format accepted by GSA: (0,0) (0,1) (1,2) (3,4) (4,0)(m)
        desc += $"({temppt.X},{temppt.Y})";
        points.Add(temppt);
      }

      return (points, desc);
    }

    internal static string ClearDefinitionForUnit(string definition) {
      return ClearDefGetUnit(definition).def;
    }

    internal static List<Point3d> ConvertPoints(string definition, LengthUnit desiredUnit, Plane localPlane) {
      (LengthUnit lengthUnit, string def) = ClearDefGetUnit(definition);
      var points = new List<Point3d>();
      string[] pts = def.Split(')');
      var map = Transform.ChangeBasis(localPlane, Plane.WorldXY);
      foreach (string ptStr in pts) {
        if (ptStr != string.Empty) {
          string pt = ptStr.Replace("(", string.Empty).Trim();
          var x = new Length(double.Parse(pt.Split(',')[0]), lengthUnit);
          var y = new Length(double.Parse(pt.Split(',')[1]), lengthUnit);
          var point = new Point3d(x.As(desiredUnit), y.As(desiredUnit), 0);
          point.Transform(map);
          points.Add(point);
        }
      }
      return points;
    }

    private static (LengthUnit lengthUnit, string def) ClearDefGetUnit(string definition) {
      LengthUnit lengthUnit = LengthUnit.Meter;
      if (definition.EndsWith("(mm)")) {
        lengthUnit = LengthUnit.Millimeter;
        definition = definition.Replace("(mm)", string.Empty);
      }
      if (definition.EndsWith("(cm)")) {
        lengthUnit = LengthUnit.Centimeter;
        definition = definition.Replace("(cm)", string.Empty);
      }
      if (definition.EndsWith("(ft)")) {
        lengthUnit = LengthUnit.Foot;
        definition = definition.Replace("(ft)", string.Empty);
      }
      if (definition.EndsWith("(in)")) {
        lengthUnit = LengthUnit.Inch;
        definition = definition.Replace("(in)", string.Empty);
      }
      definition = definition.Replace("(m)", string.Empty);
      return (lengthUnit, definition);
    }
  }
  internal enum ReferenceType {
    None,
    Property,
    Element,
    MemberChildElements,
    Member,
    List,
  }
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

  public class GsaNodeLoad {
    public enum NodeLoadTypes // direct copy from GSA API enums
    {
      NodeLoad = 0,
      AppliedDisp = 1,
      Settlement = 2,
      Gravity = 3,
      NumTypes = 4,
    }

    public NodeLoad NodeLoad { get; set; } = new NodeLoad();
    public NodeLoadTypes Type;
    internal GsaList _refList;
    internal Point3d _refPoint = Point3d.Unset;

    public GsaNodeLoad() {
      Type = NodeLoadTypes.NodeLoad;
    }

    public GsaNodeLoad Duplicate() {
      var dup = new GsaNodeLoad {
        NodeLoad = {
          AxisProperty = NodeLoad.AxisProperty,
          Case = NodeLoad.Case,
          Direction = NodeLoad.Direction,
          Nodes = NodeLoad.Nodes.ToString(),
          Name = NodeLoad.Name.ToString(),
          Value = NodeLoad.Value,
        },
        Type = Type,
      };
      if (_refPoint != Point3d.Unset) {
        dup._refPoint = new Point3d(_refPoint);
      }

      if (_refList != null) {
        dup._refList = _refList.Duplicate();
      }

      return dup;
    }
  }
}
