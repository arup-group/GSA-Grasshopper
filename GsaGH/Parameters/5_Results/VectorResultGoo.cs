using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GH_Vector"/> can be used in Grasshopper.
  /// </summary>
  public class VectorResultGoo : GH_GeometricGoo<GH_Vector>, IGH_PreviewData
  {
    public readonly Point3d StartingPoint;
    public Vector3d Direction { get; private set; }
    public readonly IQuantity ForceValue;

    private Line _reactionForceLine;
    private Color _color = Helpers.Graphics.Colours.GsaDarkPurple;
    private bool _showText;
    private bool _drawArrowHead;

    /// <summary>
    /// Goo wrapper GH_Vector class for reaction force vectors.
    /// Default color: Gsa_Purple
    /// </summary>
    public VectorResultGoo(Point3d startingPoint, Vector3d direction, IQuantity forceValue)
    {
      StartingPoint = startingPoint == Point3d.Unset ? Point3d.Origin : startingPoint;
      Direction = direction == Vector3d.Unset ? Vector3d.Zero : direction;
      ForceValue = forceValue;

      _reactionForceLine = this.CreateReactionForceLine(Direction);
      Value = this.GetGhVector();
    }

    public override string ToString() => $"VectorResult: Starting point: {StartingPoint}, Direction:{Direction}, Force:{ForceValue:0.0}";

    public override string TypeName => "Result Vector3d";

    public override string TypeDescription => "A GSA result vector3d type.";

    public override IGH_GeometricGoo DuplicateGeometry() =>
      new VectorResultGoo(StartingPoint, Direction, ForceValue);

    public override BoundingBox Boundingbox => new BoundingBox( new List<Point3d>(){ _reactionForceLine.From, _reactionForceLine.To});

    public override BoundingBox GetBoundingBox(Transform xform)
    {
      var vector = Value;
      vector.Value.Transform(xform);
      var line = CreateReactionForceLine(vector.Value);
      return new BoundingBox(line.From, line.To);
    }

    public override IGH_GeometricGoo Transform(Transform xform)
    {
      Direction.Transform(xform);
      this._reactionForceLine = CreateReactionForceLine(Direction);
      return this;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      var start = xmorph.MorphPoint(_reactionForceLine.From);
      var end = xmorph.MorphPoint(_reactionForceLine.To);
      _reactionForceLine = new Line(start, end);
      return this;
    }

    public override object ScriptVariable() => Value;

    public override bool CastTo<TQ>(out TQ target)
    {
      if (typeof(TQ).IsAssignableFrom(typeof(Vector3d)))
      {
        target = (TQ)(object)Value;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Vector)))
      {
        target = (TQ)(object)new GH_Vector(Value);
        return true;
      }

      target = default(TQ);
      return false;
    }
    public override bool CastFrom(object source)
    {
      if (source == null) return false;
      if (source is Vector3d vector)
      {
        Value = new GH_Vector(vector);
        return true;
      }

      if (source is GH_Vector lineGoo)
      {
        Value = lineGoo;
        return true;
      }

      var vector3d = new Vector3d();
      if (!GH_Convert.ToVector3d(source, ref vector3d, GH_Conversion.Both)) return false;
      Value = new GH_Vector(vector3d);

      return true;
    }

    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      args.Viewport.GetWorldToScreenScale(_reactionForceLine.To, out double pixelsPerUnit);
      
      args.Pipeline.DrawArrow(_reactionForceLine, _color);
      if (_drawArrowHead)
      {
        const int arrowHeadScreenSize = 20;
        var point = this.CalculateExtraStartOffsetPoint(pixelsPerUnit, arrowHeadScreenSize);
        args.Pipeline.DrawArrowHead(point, Direction, _color, arrowHeadScreenSize, 0);
      }

      if (!_showText) return;

      const int offset = 30;
      var endOffsetPoint = this.CalculateExtraEndOffsetPoint(pixelsPerUnit, offset);
      var positionOnTheScreen = args.Pipeline.Viewport.WorldToClient(endOffsetPoint);

      args.Pipeline.Draw2dText(ForceValue.ToString(), _color, positionOnTheScreen, true);
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public VectorResultGoo SetColor(Color value)
    {
      _color = value;
      return this;
    }

    public void ShowText(bool showText)
    {
      _showText = showText;
    }

    public VectorResultGoo DrawArrowHead(bool drawArrow)
    {
      _drawArrowHead = drawArrow;
      return this;
    }

    private Point3d CalculateExtraStartOffsetPoint(double pixelsPerUnit, int offset)
    {
      var point = new Point3d(_reactionForceLine.To);

      return TransformPoint(point, pixelsPerUnit, offset);
    }

    private Point3d CalculateExtraEndOffsetPoint(double pixelsPerUnit, int offset)
    {
      var point = new Point3d(_reactionForceLine.From);

      return TransformPoint(point, pixelsPerUnit, offset);
    }

    private Point3d TransformPoint(Point3d point, double pixelsPerUnit, int offset)
    {
      if (pixelsPerUnit == 0 || offset == 0) return point;

      var direction = _reactionForceLine.Direction;

      direction.Unitize();
      var t = Rhino.Geometry.Transform.Translation(direction * -1 * offset / pixelsPerUnit);
      point.Transform(t);

      return point;
    }

    private Line CreateReactionForceLine(Vector3d direction)
    {
      var line = new Line(StartingPoint, direction);
      line.Flip();
      line.Transform(Rhino.Geometry.Transform.Scale(StartingPoint, -1));
      return line;
    }

    private GH_Vector GetGhVector() => new GH_Vector(new Vector3d(
      _reactionForceLine.ToX - _reactionForceLine.FromX,
      _reactionForceLine.ToY - _reactionForceLine.FromY,
      _reactionForceLine.ToZ - _reactionForceLine.FromZ));
  }
}
