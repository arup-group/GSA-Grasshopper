using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Import;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaLineDiagram : GH_GeometricGoo<Line>, IGsaDiagram, IGH_PreviewData {
    public override BoundingBox Boundingbox => Value.BoundingBox;
    public override string TypeDescription => "A GSA line diagram.";
    public override string TypeName => "Line Diagram";
    public BoundingBox ClippingBox => Boundingbox;
    public Color Color { get; private set; }

    internal GsaLineDiagram(GsaAPI.Line line, double scaleFactor, Color customColor) {
      Value = Diagrams.ConvertLine(line, scaleFactor);
      if (customColor.IsEmpty) {
        Color = (Color)line.Colour;
      } else {
        Color = customColor;
      }
    }

    private GsaLineDiagram() { }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value != null && Value.IsValid) {
        args.Pipeline.DrawLine(Value, Color);
      }
    }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Line))) {
        target = (TQ)(object)new GH_Line(Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Colour))) {
        target = (TQ)(object)new GH_Colour(Color);
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new GsaLineDiagram() {
        Value = Value,
        Color = Color
      };
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      var ln = new Line(Value.From, Value.To);
      ln.Transform(xform);
      return ln.BoundingBox;
    }

    public GeometryBase GetGeometry() {
      return new LineCurve(Value);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var ln = new Line(xmorph.MorphPoint(Value.From), xmorph.MorphPoint(Value.To));
      return new GsaLineDiagram() {
        Value = ln,
        Color = Color
      };
    }

    public override string ToString() {
      var ln = new GH_Line(Value);
      return $"Diagram Line {ln}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var ln = new Line(Value.From, Value.To);
      ln.Transform(xform);
      return new GsaLineDiagram() {
        Value = ln,
        Color = Color
      };
    }
  }
}
