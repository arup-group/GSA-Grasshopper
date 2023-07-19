using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Graphics;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GH_Vector" /> can be used in Grasshopper.
  /// </summary>
  public class PolygonGoo : GH_GeometricGoo<GH_Vector>, IGH_PreviewData {
    public override BoundingBox Boundingbox {
      get {
        var box = new BoundingBox(_points);
        box.Inflate(1);
        return box;
      }
    }
    public override string TypeDescription => "A GSA result polygon type.";
    public override string TypeName => "Result polygon";
    private Color Color { get; set; } = Colours.GsaDarkPurple;
    private IEnumerable<Point3d> _points = new List<Point3d>();

    /// <summary>
    ///   Goo wrapper GH_Vector class for reaction force vectors.
    ///   Default color: Gsa_Purple
    /// </summary>
    public PolygonGoo(IEnumerable<Vector3> vectors, Color color) {
      _points = vectors.Select(item => new Point3d(item.X, item.Y, item.Z));
      Color = color;
    }

    public PolygonGoo(IEnumerable<Point3d> points, Color color) {
      _points = points;
      Color = color;
    }

    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      args.Pipeline.DrawPolygon(_points, Color, true);
    }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Vector))) {
        target = (TQ)(object)new GH_Vector(Value);
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new PolygonGoo(_points, Color);
    }

    public override BoundingBox GetBoundingBox(Transform xform) { return Boundingbox; }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return new PolygonGoo(_points, Color);
    }

    public override object ScriptVariable() {
      return Value;
    }

    public override string ToString() {
      return $"Polygons: {_points}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      return new PolygonGoo(_points, Color);
    }
  }
}
