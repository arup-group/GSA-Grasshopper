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
    public static string Name => "GridLine";
    public static string NickName => "GL";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaGridLineGoo(GsaGridLine item) : base(item) { }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaGridLineGoo(Value);
    }

    public override bool CastTo<Q>(ref Q target) {
      if (Value != null) {
        if (typeof(Q).IsAssignableFrom(typeof(GH_Line))) {
          if (Value._gridLine.Shape == GsaAPI.GridLineShape.Line) {
            var line = GsaGridLine.ToLine(Value._gridLine);
            var ghLine = new GH_Line();
            GH_Convert.ToGHLine(line, GH_Conversion.Both, ref ghLine);
            target = (Q)(object)ghLine;
            return true;
          }
        }
        if (typeof(Q).IsAssignableFrom(typeof(GH_Arc))) {
          if (Value._gridLine.Shape == GsaAPI.GridLineShape.Arc) {
            var arc = GsaGridLine.ToArc(Value._gridLine);
            var ghArc = new GH_Arc();
            GH_Convert.ToGHArc(arc, GH_Conversion.Both, ref ghArc);
            target = (Q)(object)ghArc;
            return true;
          } else {
            return false;
          }
        }
      }

      return base.CastTo<Q>(ref target);
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {

    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
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

      Point3d[] points;
      if (Value._curve.IsLinear()) {
        points = new Point3d[2] { Value._curve.SegmentCurve(0).PointAtStart, Value._curve.SegmentCurve(0).PointAtEnd };

      } else {
        points = Value._curve.SegmentCurve(0).DivideEquidistant(Value._curve.SegmentCurve(0).GetLength() / 360.0);
      }
      if (points != null) {
        int thickness = 1;
        Color color = Color.Black;
        int pattern = 999999;
        if (Value._pattern != 0) {
          pattern = Value._pattern;
        }
        args.Pipeline.DrawPatternedPolyline(points, color, pattern, thickness, false);

        double radius = 0.618046972 * unitLength; // in golden ratio
        Plane plane = Plane.WorldXY;
        Point3d origin = points[0];
        Vector3d distance = Value._curve.TangentAtStart;
        distance.Unitize();
        distance *= -radius;
        origin.Transform(Rhino.Geometry.Transform.Translation(distance));
        plane.Origin = origin;

        // we need to shorten text if it´s too long
        var text = new Text3d(Value._gridLine.Label.Substring(0, 4), plane, 0.381982059 * unitLength) { // golden ratio
          HorizontalAlignment = TextHorizontalAlignment.Center,
          VerticalAlignment = TextVerticalAlignment.Middle
        };
        args.Pipeline.Draw3dText(text, color);

        var circle = new Circle(plane, radius);
        args.Pipeline.DrawCircle(circle, color);
      }
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value._curve;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var duplicateCurve = (PolyCurve)Value._curve.Duplicate();
      xmorph.Morph(duplicateCurve);
      GsaGridLine duplicate = Value.Clone();
      duplicate._curve = duplicateCurve;
      return new GsaGridLineGoo(duplicate);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var duplicateCurve = (PolyCurve)Value._curve.Duplicate();
      duplicateCurve.Transform(xform);
      GsaGridLine duplicate = Value.Clone();
      duplicate._curve = duplicateCurve;
      return new GsaGridLineGoo(duplicate);
    }
  }
}
