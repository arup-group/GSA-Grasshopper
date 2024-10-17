using System.Collections.ObjectModel;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.Import;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaArrowheadDiagram : GH_GeometricGoo<Mesh>, IGsaDiagram, IGH_PreviewData {
    public override BoundingBox Boundingbox => Value.GetBoundingBox(false);
    public override string TypeDescription => "A GSA arrowhead diagram.";
    public override string TypeName => "Arrowhead Diagram";
    public BoundingBox ClippingBox => Boundingbox;

    internal GsaArrowheadDiagram(
      ReadOnlyCollection<Triangle> faces, double scaleFactor, Color customColor) {
      Value = Diagrams.CreateMeshFromTriangles(faces, scaleFactor, customColor);
    }
    private GsaArrowheadDiagram() { }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (Value != null) {
        args.Pipeline.DrawMeshFalseColors(Value);
      }
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) { }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        target = (TQ)(object)new GH_Mesh(Value);
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new GsaArrowheadDiagram() {
        Value = Value,
      };
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      Mesh m = Value.DuplicateMesh();
      m.Transform(xform);
      return m.GetBoundingBox(false);
    }

    public GeometryBase GetGeometry() {
      return Value;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Mesh m = Value.DuplicateMesh();
      xmorph.Morph(m);
      return new GsaArrowheadDiagram() {
        Value = m,
      };
    }

    public override string ToString() {
      var m = new GH_Mesh(Value);
      return $"Diagram Arrowhead {m}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      Mesh m = Value.DuplicateMesh();
      m.Transform(xform);
      return new GsaArrowheadDiagram() {
        Value = m,
      };
    }
  }
}
