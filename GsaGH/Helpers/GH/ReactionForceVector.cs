using GsaAPI;
using OasysUnits;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Helpers.GH
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
      this.Id = id;
      this.StartingPoint = startingPoint;
      this.Direction = direction;
      this.ForceValue = forceValue;
      this._forceType = forceType;
      this._reactionForceLine = CreateReactionForceLine();
    }

    public Line GetReactionForceLine() => this._reactionForceLine;
    public ForceType GetForceType() => this._forceType;

    public Point3d CalculateExtraStartOffsetPoint(double pixelsPerUnit, int offset)
    {
      var point = new Point3d(this._reactionForceLine.To);

      return this.TransformPoint(point, pixelsPerUnit, offset);
    }

    public Point3d CalculateExtraEndOffsetPoint(double pixelsPerUnit, int offset)
    {
      var point = new Point3d(this._reactionForceLine.From);

      return this.TransformPoint(point, pixelsPerUnit, offset);
    }

    private Point3d TransformPoint(Point3d point, double pixelsPerUnit, int offset)
    {
      var direction = this._reactionForceLine.Direction;

      direction.Unitize();
      var t = Transform.Translation(direction * -1 * offset / pixelsPerUnit);
      point.Transform(t);

      return point;
    }

    private Line CreateReactionForceLine()
    {
      var line = new Line(this.StartingPoint, this.Direction);
      line.Flip();
      line.Transform(Transform.Scale(this.StartingPoint, -1));
      return line;
    }
  }
}
