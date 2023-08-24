using System;
using GsaAPI;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  public class GsaGridLine {
    internal GridLine _gridLine;
    internal PolyCurve _curve;

    internal int _pattern = 0;

    internal GsaGridLine(GridLine gridLine, PolyCurve curve, int pattern = 0) {
      _gridLine = gridLine;
      _curve = curve;
      _pattern = pattern;
    }

    internal static GridLine FromArc(Arc arc, string label = "") {
      GridLine gridLine;
      // project onto WorldXY
      Point3d startPoint = arc.StartPoint;
      startPoint.Z = 0;
      Point3d midPoint = arc.MidPoint;
      midPoint.Z = 0;
      Point3d endPoint = arc.EndPoint;
      endPoint.Z = 0;
      var planarArc = new Arc(startPoint, midPoint, endPoint);

      if (planarArc.Plane.ZAxis.Z < 0) {
        planarArc = new Arc(endPoint, midPoint, startPoint);
      }

      double arcAngleRadians = Vector3d.VectorAngle(planarArc.Plane.XAxis, Vector3d.XAxis);
      var x = Vector3d.CrossProduct(Vector3d.XAxis, planarArc.Plane.XAxis);
      if (x.Z < 0) {
        arcAngleRadians *= -1;
      }

      double startAngleDegrees = arcAngleRadians * 180 / Math.PI;
      double endAngleDegrees = startAngleDegrees + planarArc.EndAngleDegrees;

      gridLine = new GridLine(label) {
        Shape = GridLineShape.Arc,
        X = planarArc.Center.X,
        Y = planarArc.Center.Y,
        Length = planarArc.Diameter / 2.0,
        Theta1 = startAngleDegrees,
        Theta2 = endAngleDegrees
      };
      return gridLine;
    }

    internal static GridLine FromLine(Line line, string label = "") {
      GridLine gridLine;
      var polyCurve = new PolyCurve();
      // project onto WorldXY
      line.FromZ = 0;
      line.ToZ = 0;

      gridLine = new GridLine(label) {
        Shape = GridLineShape.Line,
        X = line.From.X,
        Y = line.From.Y,
        Length = line.Length,
        Theta1 = Vector3d.VectorAngle(new Vector3d(1, 0, 0), line.UnitTangent) * 180 / Math.PI
      };
      return gridLine;
    }

    internal static Arc ToArc(GridLine gridLine) {
      var center = new Point3d(gridLine.X, gridLine.Y, 0);
      double angleRadians = (gridLine.Theta2 - gridLine.Theta1) * Math.PI / 180.0;
      double radius = gridLine.Length;
      var arc = new Arc(center, radius, angleRadians) {
        StartAngle = gridLine.Theta1 * Math.PI / 180.0,
        EndAngle = gridLine.Theta2 * Math.PI / 180.0
      };
      return arc;
    }

    internal static PolyCurve ToCurve(GridLine gridLine) {
        var curve = new PolyCurve();
      if (gridLine.Shape is GridLineShape.Line) {
        var line = GsaGridLine.ToLine(gridLine);
        curve.Append(line);
      } else if (gridLine.Shape is GridLineShape.Arc) {
        var arc = GsaGridLine.ToArc(gridLine);
        curve.Append(arc);
      }
      return curve;
    }

    internal static Line ToLine(GridLine gridLine) {
        var start = new Point3d(gridLine.X, gridLine.Y, 0);
      var end = new Point3d(gridLine.X + gridLine.Length, gridLine.Y, 0);
      var line = new Line(start, end);
      Transform.Rotation(gridLine.Theta1 * Math.PI / 180.0, start);

      return line;
    }

    public GsaGridLine Clone() {
      var gridLine = new GridLine(_gridLine.Label) {
        X = _gridLine.X,
        Y = _gridLine.Y,
        Length = _gridLine.Length,
        Shape = _gridLine.Shape,
        Theta1 = _gridLine.Theta1,
      };
      if (_gridLine.Shape == GridLineShape.Arc) {
        gridLine.Theta2 = _gridLine.Theta2;
      }
      PolyCurve curve = _curve.DuplicatePolyCurve();
      var dup = new GsaGridLine(gridLine, curve, _pattern);
      return dup;
    }

    public GsaGridLine Duplicate() {
      return this;
    }

    internal Arc ToArc() {
      return GsaGridLine.ToArc(_gridLine);
    }

    internal PolyCurve ToCurve() {
      return GsaGridLine.ToCurve(_gridLine);
    }

    internal Line ToLine() {
      return GsaGridLine.ToLine(_gridLine);
    }

    public override string ToString() {
      string label = _gridLine.Label != "" ? $"{_gridLine.Label} " : string.Empty;
      string type = _gridLine.Shape == GridLineShape.Arc ? "Shape: Arc " : string.Empty;
      string s = $"{label}{type}X:{_gridLine.X} Y:{_gridLine.Y} Length:" +
        $"{_gridLine.Length} Orientation:{_gridLine.Theta1}°";
      if (_gridLine.Shape == GridLineShape.Arc) {
        s.Replace("Orientation", "Theta1");
        s += " Theta2:" + _gridLine.Theta2 + "°";
      }

      return s;
    }
  }
}
