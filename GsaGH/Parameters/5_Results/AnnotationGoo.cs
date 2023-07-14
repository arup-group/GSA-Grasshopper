using System;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class AnnotationGoo : GH_OasysGeometricGoo<TextDot>, IGH_PreviewData {
    public override string TypeName => "Annotation";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public override string TypeDescription => "Annotation any GSA object for ID or result value";
    public Color Color { get; private set; } = Color.Empty;

    public AnnotationGoo(Point3d point, Color color, string text) : base(new TextDot(text, point)) {
      Color = color;
    }
    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_UnitNumber))) {
        var types = Quantity.Infos.Select(x => x.ValueType).ToList();
        foreach (Type type in types) {
          if (Quantity.TryParse(type, Value.Text, out IQuantity quantity)) {
            target = (TQ)(object)new GH_UnitNumber(quantity);
            return true;
          }
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Number))) {
        
        if (double.TryParse(Value.Text, out double number)) {
          target = (TQ)(object)new GH_Number(number);
          return true;
        } else {
          var types = Quantity.Infos.Select(x => x.ValueType).ToList();
          foreach (Type type in types) {
            if (Quantity.TryParse(type, Value.Text, out IQuantity quantity)) {
              target = (TQ)(object)new GH_Number(quantity.Value);
              return true;
            }
          }
        }

      }

      target = default;
      return false;
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Color != Color.Empty) {
        args.Pipeline.Draw2dText(Value.Text, Color, Value.Point, true);
      } else {
        args.Pipeline.Draw2dText(Value.Text, args.Color, Value.Point, true);
      }
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public override IGH_GeometricGoo Duplicate() {
      return new AnnotationGoo(Value.Point, Color, Value.Text);
    }

    public override string ToString() { 
      var pt = new GH_Point(Value.Point);
      return $"{pt}, Value: {Value.Text}"; 
    }

    public override GeometryBase GetGeometry() {
      if (Value?.Point == null) {
        return null;
      }

      Point3d pt1 = Value.Point;
      pt1.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / 2;
      Point3d pt2 = Value.Point;
      pt2.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / -2;
      var ln = new Line(pt1, pt2);
      return new LineCurve(ln);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      TextDot value = Value;
      Point3d point = Value.Point;
      point.Transform(xform);
      return new AnnotationGoo(point, Color, Value.Text);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      TextDot value = Value;
      Point3d point = xmorph.MorphPoint(Value.Point);
      return new AnnotationGoo(point, Color, Value.Text);
    }
  }
}
