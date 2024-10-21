using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Graphics;

using Rhino.Collections;
using Rhino.Geometry;

using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="Vector3d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaVectorDiagram : GH_GeometricGoo<Vector3d>, IGsaDiagram, IGH_PreviewData {
    public override BoundingBox Boundingbox
      => new BoundingBox(new Point3dList() {
        DisplayLine.From,
        DisplayLine.To,
      });
    public Vector3d Direction { get; private set; }
    public override string TypeDescription => "A GSA vector diagram.";
    public override string TypeName => "Vector Diagram";
    public BoundingBox ClippingBox => Boundingbox;
    public Color Color { get; private set; } = Colours.GsaDarkPurple;
    public readonly Point3d AnchorPoint;
    internal Line DisplayLine;
    private bool _doubleArrow = false;

    internal GsaVectorDiagram(Point3d anchor, Vector3d direction, bool doubleArrow, Color color) {
      AnchorPoint = anchor;
      Direction = direction;
      _doubleArrow = doubleArrow;

      DisplayLine = CreateReactionForceLine(anchor, direction);
      Value = direction;

      if (color != Color.Empty) {
        Color = color;
        return;
      }

      if (doubleArrow) {
        Color = Colours.GsaGold;
      } else {
        Color = Colours.GsaDarkPurple;
      }
    }

    private GsaVectorDiagram() { }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      if (_doubleArrow) {
        args.Viewport.GetWorldToScreenScale(DisplayLine.To, out double pixelsPerUnit);
        args.Pipeline.DrawArrow(DisplayLine, Color);
        const int arrowHeadScreenSize = 20;
        Point3d point = CalculateExtraStartOffsetPoint(pixelsPerUnit, arrowHeadScreenSize);
        args.Pipeline.DrawArrowHead(point, Direction, Color, arrowHeadScreenSize, 0);
      } else {
        args.Pipeline.DrawArrow(DisplayLine, Color);
      }
    }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Vector))) {
        target = (TQ)(object)new GH_Vector(Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Line))) {
        target = (TQ)(object)new GH_Line(DisplayLine);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Point))) {
        target = (TQ)(object)new GH_Point(AnchorPoint);
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new GsaVectorDiagram(AnchorPoint, Direction, _doubleArrow, Color);
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      var ln = new Line(DisplayLine.From, DisplayLine.To);
      ln.Transform(xform);
      return ln.BoundingBox;
    }

    public GeometryBase GetGeometry() {
      return new LineCurve(DisplayLine);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Point3d anchor = xmorph.MorphPoint(AnchorPoint);
      var fakeVector = new Point3d(Value);
      var vec = new Vector3d(xmorph.MorphPoint(fakeVector));
      return new GsaVectorDiagram(anchor, vec, _doubleArrow, Color);
    }

    public override string ToString() {
      var pt = new GH_Point(AnchorPoint);
      var vec = new GH_Vector(Direction);
      return $"Diagram Vector: Anchor {pt}, Direction {vec}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var anchor = new Point3d(AnchorPoint);
      anchor.Transform(xform);
      var vec = new Vector3d(Value);
      vec.Transform(xform);
      return new GsaVectorDiagram(anchor, vec, _doubleArrow, Color);
    }

    private Point3d CalculateExtraStartOffsetPoint(double pixelsPerUnit, int offset) {
      var point = new Point3d(DisplayLine.To);

      return TransformPoint(point, pixelsPerUnit, offset);
    }

    private Line CreateReactionForceLine(Point3d anchor, Vector3d direction) {
      var line = new Line(anchor, direction);
      line.Flip();
      line.Transform(Rhino.Geometry.Transform.Scale(anchor, -1));
      return line;
    }

    private Point3d TransformPoint(Point3d point, double pixelsPerUnit, int offset) {
      if (pixelsPerUnit == 0 || offset == 0) {
        return point;
      }

      Vector3d direction = DisplayLine.Direction;

      direction.Unitize();
      var t = Rhino.Geometry.Transform.Translation(direction * -1 * offset / pixelsPerUnit);
      point.Transform(t);

      return point;
    }
  }
}
