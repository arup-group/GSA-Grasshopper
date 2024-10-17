using System;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Graphics;

using OasysGH.Parameters;

using OasysUnits;

using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaAnnotation3d : GH_GeometricGoo<Text3d>, IGsaAnnotation, IGH_PreviewData {
    public override BoundingBox Boundingbox => Value.BoundingBox;
    public override string TypeDescription => "A GSA 3D Annotation.";
    public override string TypeName => "Annotation3D";
    public BoundingBox ClippingBox => Boundingbox;
    public Color Color { get; private set; } = Colours.GsaDarkBlue;
    public string Text => Value.Text;
    public Point3d Location => Value.TextPlane.Origin;

    internal GsaAnnotation3d(Plane plane, Color color, string text, double height)
      : base(new Text3d(text, plane, height)) {
      Color = color;
      Value.HorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Center;
      Value.VerticalAlignment = Rhino.DocObjects.TextVerticalAlignment.Top;
    }

    private GsaAnnotation3d() { }

    public override bool CastTo<TQ>(ref TQ target) {
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
    public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (Value != null) {
        args.Viewport.GetCameraFrame(out Plane pln);
        if (Vector3d.VectorAngle(pln.Normal, Value.TextPlane.Normal) > Math.PI / 2) {
          var newPlane = new Plane(Value.TextPlane);
          newPlane.Rotate(Math.PI, Vector3d.ZAxis);
          args.Pipeline.Draw3dText(Value, Color, newPlane);
        } else {
          args.Pipeline.Draw3dText(Value, Color);
        }
      }
    }
    public void DrawViewportWires(GH_PreviewWireArgs args) { }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new GsaAnnotation3d() {
        Value = Value,
        Color = Color,
      };
    }

    public override string ToString() {
      if (Value == null) {
        return "Null";
      }

      var pt = new GH_Point(Value.TextPlane.Origin);
      return $"{pt}, Value: {Value.Text}";
    }

    public GeometryBase GetGeometry() {
      if (Value?.TextPlane.Origin == null) {
        return null;
      }

      return new LineCurve(Value.BoundingBox.Min, Value.BoundingBox.Max);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      Plane pln = Value.TextPlane.Clone();
      pln.Transform(xform);
      return new GsaAnnotationGoo(new GsaAnnotation3d(pln, Color, Value.Text, Value.Height));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Plane pln = Value.TextPlane.Clone();
      xmorph.Morph(ref pln);
      return new GsaAnnotationGoo(new GsaAnnotation3d(pln, Color, Value.Text, Value.Height));
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      BoundingBox box = Value.BoundingBox;
      box.Transform(xform);
      return box;
    }
  }
}
