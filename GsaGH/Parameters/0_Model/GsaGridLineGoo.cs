using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits.Units;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaGridLine" /> can be used in Grasshopper.
  /// </summary>
  public class GsaGridLineGoo : GH_OasysGeometricGoo<GsaGridLine> {
    public static string Description => "GSA Grid Line";
    public static string Name => "Grid Line";
    public static string NickName => "GL";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaGridLineGoo(GsaGridLine item) : base(item) { }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaGridLineGoo(Value);
    }

    public override bool CastTo<Q>(ref Q target) {
      if (Value != null) {
        if (typeof(Q).IsAssignableFrom(typeof(GH_Curve))) {
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Line) {
            var line = GsaGridLine.ToLine(Value.GridLine);
            var ghLine = new GH_Curve();
            GH_Convert.ToGHCurve(line, GH_Conversion.Both, ref ghLine);
            target = (Q)(object)ghLine;
            return true;
          }
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Arc) {
            var arc = GsaGridLine.ToArc(Value.GridLine);
            var ghArc = new GH_Curve();
            GH_Convert.ToGHCurve(arc, GH_Conversion.Both, ref ghArc);
            target = (Q)(object)ghArc;
            return true;
          } else {
            return false;
          }
        }
        if (typeof(Q).IsAssignableFrom(typeof(GH_Line))) {
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Line) {
            var line = GsaGridLine.ToLine(Value.GridLine);
            var ghLine = new GH_Line();
            GH_Convert.ToGHLine(line, GH_Conversion.Both, ref ghLine);
            target = (Q)(object)ghLine;
            return true;
          } else {
            return false;
          }
        }
        if (typeof(Q).IsAssignableFrom(typeof(GH_Arc))) {
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Arc) {
            var arc = GsaGridLine.ToArc(Value.GridLine);
            var ghArc = new GH_Arc();
            GH_Convert.ToGHArc(arc, GH_Conversion.Both, ref ghArc);
            target = (Q)(object)ghArc;
            return true;
          } else {
            return false;
          }
        }
      }

      return base.CastTo(ref target);
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null || Value.Curve == null || !Value.Curve.IsValid) {
        return;
      }

      int thickness = 1;
      // this is a workaround to change colour between selected and not
      if (args.Color != Color.FromArgb(255, 150, 0, 0)) {
        thickness = 3;
      }

      // we need to scale grid lines according to the users unit settings
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

      Point3d[] points = null;
      Curve segment = Value.Curve.SegmentCurve(0);
      if (segment == null) {
        return;
      }
      if (Value.Curve.IsLinear()) {
        points = new Point3d[2] { segment.PointAtStart, segment.PointAtEnd };
      } else {
        points = segment.DivideEquidistant(segment.GetLength() / 360.0);
      }
      if (points != null) {
        Color color = Color.Black;
        int pattern = 999999;
        args.Pipeline.DrawPatternedPolyline(points, color, pattern, thickness, false);

        double radius = 0.618046972 * unitLength; // in golden ratio
        Plane plane = Plane.WorldXY;
        Point3d origin = points[0];
        Vector3d distance = Value.Curve.TangentAtStart;
        distance.Unitize();
        distance *= -radius;
        origin.Transform(Rhino.Geometry.Transform.Translation(distance));
        plane.Origin = origin;

        var text = new Text3d(Value.GridLine.Label, plane, 0.381982059 * unitLength) { // golden ratio
          HorizontalAlignment = TextHorizontalAlignment.Center,
          VerticalAlignment = TextVerticalAlignment.Middle
        };
        if (args.Color != Color.FromArgb(255, 150, 0, 0)) {
          text.Bold = true;
        }

        args.Pipeline.Draw3dText(text, color);

        var circle = new Circle(plane, radius);
        args.Pipeline.DrawCircle(circle, color, thickness);
      }
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value.Curve;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var gridline = new GsaGridLine(Value);
      xmorph.Morph(gridline.Curve);
      return new GsaGridLineGoo(gridline);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var gridline = new GsaGridLine(Value);
      gridline.Curve.Transform(xform);
      return new GsaGridLineGoo(gridline);
    }
  }
}
