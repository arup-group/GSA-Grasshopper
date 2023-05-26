using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using OasysGH.Parameters;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GH_Vector" /> can be used in Grasshopper.
  /// </summary>
  public class VectorResultGoo : GH_GeometricGoo<GH_Vector>, IGH_PreviewData {
    public override BoundingBox Boundingbox
      => new BoundingBox(new List<Point3d>() {
        _reactionForceLine.From,
        _reactionForceLine.To,
      });
    public BoundingBox ClippingBox => Boundingbox;
    public Vector3d Direction { get; private set; }
    public override string TypeDescription => "A GSA result vector type.";
    public override string TypeName => "Result Vector";
    public readonly IQuantity ForceValue;
    public readonly int NodeId;
    public readonly Point3d StartingPoint;
    private Color _color = Colours.GsaDarkPurple;
    private bool _drawArrowHead;
    private Line _reactionForceLine;
    private bool _showText;

    /// <summary>
    ///   Goo wrapper GH_Vector class for reaction force vectors.
    ///   Default color: Gsa_Purple
    /// </summary>
    public VectorResultGoo(
      Point3d startingPoint, Vector3d direction, IQuantity forceValue, int id) {
      StartingPoint = startingPoint == Point3d.Unset ? Point3d.Origin : startingPoint;
      Direction = direction == Vector3d.Unset ? Vector3d.Zero : direction;
      ForceValue = forceValue;
      NodeId = id;
      _reactionForceLine = CreateReactionForceLine(Direction);
      Value = GetGhVector();
    }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Vector))) {
        target = (TQ)(object)new GH_Vector(Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Number))) {
        target = (TQ)(object)new GH_Number(ForceValue.Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        target = (TQ)(object)new GH_Integer(NodeId);
        return true;
      }

      target = default;
      return false;
    }

    public VectorResultGoo DrawArrowHead(bool drawArrow) {
      _drawArrowHead = drawArrow;
      return this;
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      args.Viewport.GetWorldToScreenScale(_reactionForceLine.To, out double pixelsPerUnit);

      args.Pipeline.DrawArrow(_reactionForceLine, _color);
      if (_drawArrowHead) {
        const int arrowHeadScreenSize = 20;
        Point3d point = CalculateExtraStartOffsetPoint(pixelsPerUnit, arrowHeadScreenSize);
        args.Pipeline.DrawArrowHead(point, Direction, _color, arrowHeadScreenSize, 0);
      }

      if (!_showText) {
        return;
      }

      const int offset = 30;
      Point3d endOffsetPoint = CalculateExtraEndOffsetPoint(pixelsPerUnit, offset);
      Point2d positionOnTheScreen = args.Pipeline.Viewport.WorldToClient(endOffsetPoint);

      args.Pipeline.Draw2dText(ForceValue.ToString(), _color, positionOnTheScreen, true);
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new VectorResultGoo(StartingPoint, Direction, ForceValue, NodeId);
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      GH_Vector vector = Value;
      vector.Value.Transform(xform);
      Line line = CreateReactionForceLine(vector.Value);
      return new BoundingBox(line.From, line.To);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Point3d sPoint = xmorph.MorphPoint(StartingPoint);
      return new VectorResultGoo(sPoint, Direction, ForceValue, NodeId);
    }

    public override object ScriptVariable() {
      return Value;
    }

    public VectorResultGoo SetColor(Color value) {
      _color = value;
      return this;
    }

    public void ShowText(bool showText) {
      _showText = showText;
    }

    public override string ToString() {
      return
        $"VectorResult: Starting point: {StartingPoint}, Direction:{Direction}, Force:{ForceValue:0.0}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      Point3d sPoint = StartingPoint;
      sPoint.Transform(xform);
      return new VectorResultGoo(sPoint, Direction, ForceValue, NodeId);
    }

    private Point3d CalculateExtraEndOffsetPoint(double pixelsPerUnit, int offset) {
      var point = new Point3d(_reactionForceLine.From);

      return TransformPoint(point, pixelsPerUnit, offset);
    }

    private Point3d CalculateExtraStartOffsetPoint(double pixelsPerUnit, int offset) {
      var point = new Point3d(_reactionForceLine.To);

      return TransformPoint(point, pixelsPerUnit, offset);
    }

    private Line CreateReactionForceLine(Vector3d direction) {
      var line = new Line(StartingPoint, direction);
      line.Flip();
      line.Transform(Rhino.Geometry.Transform.Scale(StartingPoint, -1));
      return line;
    }

    private GH_Vector GetGhVector() {
      return new GH_Vector(new Vector3d(_reactionForceLine.ToX - _reactionForceLine.FromX,
        _reactionForceLine.ToY - _reactionForceLine.FromY,
        _reactionForceLine.ToZ - _reactionForceLine.FromZ));
    }

    private Point3d TransformPoint(Point3d point, double pixelsPerUnit, int offset) {
      if (pixelsPerUnit == 0 || offset == 0) {
        return point;
      }

      Vector3d direction = _reactionForceLine.Direction;

      direction.Unitize();
      var t = Rhino.Geometry.Transform.Translation(direction * -1 * offset / pixelsPerUnit);
      point.Transform(t);

      return point;
    }
  }
}
