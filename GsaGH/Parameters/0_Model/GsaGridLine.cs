using System;

using GsaAPI;

using OasysGH.Units;

using OasysUnits;

using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  /// Grid Lines can be used to aid visual representation of a <see cref="GsaModel"/>.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-grid-line.html">Grid lines</see> to read more.</para>
  /// </summary>
  public class GsaGridLine {
    public GridLine GridLine { get; internal set; }
    public PolyCurve Curve { get; internal set; }

    internal Point3d[] Points { get; private set; }
    internal Text3d Text { get; private set; }
    internal Circle Circle { get; private set; }

    internal GsaGridLine(GridLine gridLine, PolyCurve curve) {
      GridLine = gridLine;
      Curve = curve;
    }

    public GsaGridLine(Arc arc, string label = "") {
      if (arc.Plane.ZAxis.Z < 0) {
        arc = new Arc(arc.EndPoint, arc.MidPoint, arc.StartPoint);
      }
      Curve = new PolyCurve();
      Curve.Append(arc);

      double arcAngleRadians = Vector3d.VectorAngle(arc.Plane.XAxis, Vector3d.XAxis);
      var x = Vector3d.CrossProduct(Vector3d.XAxis, arc.Plane.XAxis);
      if (x.Z < 0) {
        arcAngleRadians *= -1;
      }

      double startAngleDegrees = arcAngleRadians * 180 / Math.PI;
      double endAngleDegrees = startAngleDegrees + arc.EndAngleDegrees;

      GridLine = new GridLine(label) {
        Shape = GridLineShape.Arc,
        X = arc.Center.X,
        Y = arc.Center.Y,
        Length = arc.Diameter / 2.0,
        Theta1 = startAngleDegrees,
        Theta2 = endAngleDegrees
      };

      UpdatePreview();
    }

    public GsaGridLine(Line line, string label = "") {
      GridLine = new GridLine(label) {
        Shape = GridLineShape.Line,
        X = line.From.X,
        Y = line.From.Y,
        Length = line.Length,
        Theta1 = Vector3d.VectorAngle(new Vector3d(1, 0, 0), line.UnitTangent) * 180 / Math.PI
      };
      Curve = new PolyCurve();
      Curve.Append(line);

      UpdatePreview();
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
        Line line = ToLine(gridLine);
        curve.Append(line);
      } else if (gridLine.Shape is GridLineShape.Arc) {
        Arc arc = ToArc(gridLine);
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

    public GsaGridLine(GsaGridLine other) {
      GridLine = new GridLine(other.GridLine.Label) {
        Shape = other.GridLine.Shape,
        Theta1 = other.GridLine.Theta1,
        X = other.GridLine.X,
        Y = other.GridLine.Y,
        Length = other.GridLine.Length,
      };

      if (GridLine.Shape == GridLineShape.Arc) {
        GridLine.Theta2 = other.GridLine.Theta2;
      }

      Curve = other.Curve.DuplicatePolyCurve();

      UpdatePreview();
    }

    public override string ToString() {
      string label = GridLine.Label != "" ? $"{GridLine.Label} " : string.Empty;
      string type = GridLine.Shape == GridLineShape.Arc ? "Shape: Arc " : string.Empty;
      string s = $"{label}{type}X:{GridLine.X} Y:{GridLine.Y} Length:" +
        $"{GridLine.Length} Orientation:{GridLine.Theta1}°";
      if (GridLine.Shape == GridLineShape.Arc) {
        s.Replace("Orientation", "Theta1");
        s += " Theta2:" + GridLine.Theta2 + "°";
      }

      return s;
    }

    internal void UpdatePreview() {
      // we want to scale grid lines according to the users unit settings
      double unitLength = 1;
      LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
      switch (DefaultUnits.LengthUnitGeometry) {
        case LengthUnit.Millimeter:
          unitLength = 1000;
          break;
        case LengthUnit.Centimeter:
          unitLength = 100;
          break;
        case LengthUnit.Inch:
          unitLength = 39.3701;
          break;
        case LengthUnit.Foot:
          unitLength = 3.28084;
          break;
      }

      Points = null;
      Curve segment = Curve.SegmentCurve(0);
      if (segment == null) {
        return;
      }
      if (Curve.IsLinear()) {
        Points = new Point3d[2] { segment.PointAtStart, segment.PointAtEnd };
      } else {
        Points = segment.DivideEquidistant(segment.GetLength() / 360.0);
      }

      Text = null;
      Circle = Circle.Unset;

      if (Points != null) {
        double radius = 0.618046972 * unitLength; // in golden ratio
        Plane plane = Plane.WorldXY;
        Point3d origin = Points[0];
        Vector3d distance = Curve.TangentAtStart;
        distance.Unitize();
        distance *= -radius;
        origin.Transform(Transform.Translation(distance));
        plane.Origin = origin;

        Text = new Text3d(GridLine.Label, plane, 0.381982059 * unitLength) { // golden ratio
          HorizontalAlignment = TextHorizontalAlignment.Center,
          VerticalAlignment = TextVerticalAlignment.Middle
        };

        Circle = new Circle(plane, radius);
      }
    }

    public GridLine GetApiGridLineToUnit(LengthUnit unit) {
      GridLine gridLine = DuplicateApiObject();
      gridLine.X = unit == LengthUnit.Meter ? GridLine.X : new Length(GridLine.X, unit).Value;
      gridLine.Y = unit == LengthUnit.Meter ? GridLine.Y : new Length(GridLine.Y, unit).Value;
      gridLine.Length = unit == LengthUnit.Meter ? GridLine.Length : new Length(GridLine.Length, unit).Value;

      return gridLine;
    }

    public GridLine DuplicateApiObject() {
      var gsaLine = new GridLine(GridLine.Label) {
        Theta1 = GridLine.Theta1,
        X = GridLine.X,
        Y = GridLine.Y,
        Length = GridLine.Length,
        Shape = GridLine.Shape,
      };
      if (GridLine.Shape == GridLineShape.Arc) {
        gsaLine.Theta2 = GridLine.Theta2;
      }

      return gsaLine;
    }
  }
}
