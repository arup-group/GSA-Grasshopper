using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using OasysUnits;

using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class PointResultGoo : GH_GeometricGoo<Point3d>, IGH_PreviewData {
    public override BoundingBox Boundingbox {
      get {
        var box = new BoundingBox(Value, Value);
        box.Inflate(1);
        return box;
      }
    }
    public BoundingBox ClippingBox => Boundingbox;
    public override string TypeDescription => "A GSA result point type.";
    public override string TypeName => "Result Point";
    public readonly int NodeId;
    public readonly IQuantity Result;
    private readonly Color _color;
    private readonly float _size;

    public PointResultGoo(Point3d point, IQuantity result, Color color, float size, int id)
      : base(point) {
      Result = result;
      _size = size;
      _color = color;
      NodeId = id;
    }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Point))) {
        target = (TQ)(object)new GH_Point(Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Number))) {
        target = (TQ)(object)new GH_Number(Result.Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        target = (TQ)(object)new GH_Integer(NodeId);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Colour))) {
        target = (TQ)(object)new GH_Colour(_color);
        return true;
      }

      target = default;
      return false;
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      args.Pipeline.DrawPoint(Value, PointStyle.RoundSimple, _size, _color);
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new PointResultGoo(Value, Result, _color, _size, NodeId);
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      Point3d point = Value;
      point.Transform(xform);
      var box = new BoundingBox(point, point);
      box.Inflate(1);
      return box;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Point3d point = xmorph.MorphPoint(Value);
      return new PointResultGoo(point, Result, _color, _size, NodeId);
    }

    public override string ToString() {
      return $"PointResult: P {NodeId}:({Value.X:0.0},{Value.Y:0.0},{Value.Z:0.0}) R:{Result:0.0}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      Point3d point = Value;
      point.Transform(xform);
      return new PointResultGoo(point, Result, _color, _size, NodeId);
    }
  }
}
