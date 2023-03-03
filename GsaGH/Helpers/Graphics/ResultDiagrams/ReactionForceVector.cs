using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Helpers.Graphics.ResultDiagrams
{
    public enum ForceType
    {
        Force,
        Moment
    }

    public class ReactionForceVector
    {
        public readonly int Id;
        public readonly Point3d StartingPoint;
        public readonly Vector3d Direction;
        public readonly IQuantity ForceValue;

        private readonly ForceType _forceType;
        private readonly Line _reactionForceLine;

        public ReactionForceVector(int id, Point3d startingPoint, Vector3d direction, IQuantity forceValue, ForceType forceType)
        {
            Id = id;
            StartingPoint = startingPoint == Point3d.Unset ? Point3d.Origin : startingPoint;
            Direction = direction == Vector3d.Unset ? Vector3d.Zero : direction;
            ForceValue = forceValue;
            _forceType = forceType;
            _reactionForceLine = CreateReactionForceLine();
        }

        public Line GetReactionForceLine() => _reactionForceLine;
        public ForceType GetForceType() => _forceType;

        public Point3d CalculateExtraStartOffsetPoint(double pixelsPerUnit, int offset)
        {
            var point = new Point3d(_reactionForceLine.To);

            return TransformPoint(point, pixelsPerUnit, offset);
        }

        public Point3d CalculateExtraEndOffsetPoint(double pixelsPerUnit, int offset)
        {
            var point = new Point3d(_reactionForceLine.From);

            return TransformPoint(point, pixelsPerUnit, offset);
        }

        private Point3d TransformPoint(Point3d point, double pixelsPerUnit, int offset)
        {
            if (pixelsPerUnit == 0 || offset == 0) return point;

            var direction = _reactionForceLine.Direction;

            direction.Unitize();
            var t = Transform.Translation(direction * -1 * offset / pixelsPerUnit);
            point.Transform(t);

            return point;
        }

        private Line CreateReactionForceLine()
        {
            var line = new Line(StartingPoint, Direction);
            line.Flip();
            line.Transform(Transform.Scale(StartingPoint, -1));
            return line;
        }
    }
}
