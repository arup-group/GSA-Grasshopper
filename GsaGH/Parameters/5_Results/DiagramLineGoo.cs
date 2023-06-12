using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="Line" /> can be used in Grasshopper.
  /// </summary>
  public class DiagramLineGoo : GH_GeometricGoo<Line>, IGH_PreviewData {
    public override BoundingBox Boundingbox => Value.BoundingBox;
    public BoundingBox ClippingBox => Boundingbox;
    public override string TypeDescription => "A GSA diagram line type.";
    public override string TypeName => "Diagram Line";
    public Color Color { get; private set; }

    public DiagramLineGoo(Point3d startPoint, Point3d endPoint, Color color) {
      Value = new Line(startPoint, endPoint);
      Color = color;
    }

    public override bool CastFrom(object source) {
      return false;
    }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(Line))) {
        target = (TQ)(object)Value;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Line))) {
        target = (TQ)(object)new GH_Line(Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        target = (TQ)(object)new GH_Curve(Value.ToNurbsCurve());
        return true;
      }

      target = default;
      return false;
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      args.Pipeline.DrawLine(Value, Color);
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new DiagramLineGoo(Value.From, Value.To, Color);
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      Line ln = Value;
      ln.Transform(xform);
      return ln.BoundingBox;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Point3d start = xmorph.MorphPoint(Value.From);
      Point3d end = xmorph.MorphPoint(Value.To);
      return new DiagramLineGoo(start, end, Color);
    }

    public override object ScriptVariable() {
      return Value;
    }

    public override string ToString() {
      return $"DiagramLine: L:{Value.Length:0.0}, S:{Value.From:0.0}, E:{Value.To:0.0}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      Line ln = Value;
      ln.Transform(xform);
      return new DiagramLineGoo(ln.From, ln.To, Color);
    }
  }
}
