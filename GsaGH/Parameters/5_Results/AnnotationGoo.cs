using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  internal class AnnotationGoo : GH_OasysGeometricGoo<TextDot>, IGH_PreviewData {
    public override string TypeName => "Annotation";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public override string TypeDescription => "Annotation for diagram result";

    public Vector3d Position { get; private set; }
    public Color Color { get; private set; }
    public string Text { get; private set; }

    public AnnotationGoo(Vector3d position, Color color, string text) : base(new TextDot(text,
      new Point3d(position))) {
      Position = position;
      Color = color;
      Text = text;
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      var coordinate = new Point3d(Position.X, Position.Y, Position.Z);
      args.Pipeline.Draw2dText(Text, Color, coordinate, true);
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public override IGH_GeometricGoo Duplicate() {
      return new AnnotationGoo(Position, Color, Text);
    }

    public override string ToString() { return $"Position {Position}, Value: {Text}"; }

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
      value.Transform(xform);
      return new AnnotationGoo(new Vector3d(value.Point), Color, Text);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Point3d point = xmorph.MorphPoint(new Point3d(Position));
      return new AnnotationGoo(new Vector3d(point), Color, Text);
    }
  }
}
