using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System.Drawing;

namespace GsaGH.Parameters {
  internal class AnnotationGoo : GH_GeometricGoo<Point3d>, IGH_PreviewData {
    public override string TypeName => "Annotation";
    public override string TypeDescription => "Annotation for diagram result";
    public override BoundingBox Boundingbox {
      get {
        var box = new BoundingBox(Value, Value);
        box.Inflate(1);
        return box;
      }
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      args.Pipeline.Draw2dText(Text, Color, new Point3d(Position.X, Position.Y, Position.Z), true);
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
    }

    public BoundingBox ClippingBox => Boundingbox;

    public Vector3d Position { get; private set; }
    public Color Color { get; private set; }
    public string Text { get; private set; }

    public AnnotationGoo(Vector3d position, Color color, string text) {
      Position = position;
      Color = color;
      Text = text;
    }

    public override string ToString() { return $"Position {Position}, Value: {Text}"; }
    public override IGH_GeometricGoo DuplicateGeometry() { return new AnnotationGoo(Position, Color, Text); }

    public override BoundingBox GetBoundingBox(Transform xform) {
      Point3d point = Value;
      point.Transform(xform);
      var box = new BoundingBox(point, point);
      box.Inflate(1);
      return box;
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      Point3d point = Value;
      point.Transform(xform);
      return new AnnotationGoo(new Vector3d(point), Color, Text);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Point3d point = xmorph.MorphPoint(new Point3d(Position));
      return new AnnotationGoo(new Vector3d(point), Color, Text);
    }
  }
}