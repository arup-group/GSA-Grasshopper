using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Graphics;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  public enum ArrowMode {
    NoArrow,
    OneArrow,
    DoubleArrow,
  }

  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GH_Vector" /> can be used in Grasshopper.
  /// </summary>
  public class DiagramGoo : GH_GeometricGoo<GH_Vector>, IGH_PreviewData {
    public override BoundingBox Boundingbox
      => new BoundingBox(new List<Point3d>() {
        _reactionForceLine.From,
        _reactionForceLine.To,
      });
    public Vector3d Direction { get; private set; }
    public override string TypeDescription => "A GSA result diagram type.";
    public override string TypeName => "Diagram Vector";
    public Color Color { get; private set; } = Colours.GsaDarkPurple;
    public readonly ArrowMode ArrowMode;
    public readonly Point3d StartingPoint;
    private Mesh _arrowHead;
    private List<Color> _arrowHeadOutlineColors;
    private List<Polyline> _arrowHeadOutlines;
    private Line _reactionForceLine;

    /// <summary>
    ///   Goo wrapper GH_Vector class for reaction force vectors.
    ///   Default color: Gsa_Purple
    /// </summary>
    public DiagramGoo(Point3d startingPoint, Vector3d direction, ArrowMode arrowMode) {
      StartingPoint = startingPoint == Point3d.Unset ? Point3d.Origin : startingPoint;
      Direction = direction == Vector3d.Unset ? Vector3d.Zero : direction;
      ArrowMode = arrowMode;
      _reactionForceLine = CreateReactionForceLine(Direction);
      Value = GetGhVector();
    }

    internal DiagramGoo(
      ReadOnlyCollection<Triangle> triangles, ReadOnlyCollection<Polygon> polygons,
      double scalingFactor) {
      _arrowHead = CreateMeshFromTriangles(triangles);
      scalingFactor *= 0.1;
      var scalar = Rhino.Geometry.Transform.Scale(new Point3d(0, 0, 0), scalingFactor);
      _arrowHead.Transform(scalar);

      _arrowHeadOutlines = CreatePolylinesFromPolygon(polygons);
      _arrowHeadOutlines.ForEach(item => item.Transform(scalar));

      _arrowHeadOutlineColors = polygons.Select(item => (Color)item.Colour).ToList();
    }

    private DiagramGoo() { }

    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (_arrowHead != null) {
        args.Pipeline.DrawMeshFalseColors(_arrowHead);
      }
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) {

      switch (ArrowMode) {
        case ArrowMode.NoArrow:
          args.Pipeline.DrawLine(_reactionForceLine, Color);
          break;
        case ArrowMode.OneArrow:
          args.Pipeline.DrawArrow(_reactionForceLine, Color);
          break;
        case ArrowMode.DoubleArrow: {
            args.Viewport.GetWorldToScreenScale(_reactionForceLine.To, out double pixelsPerUnit);
            args.Pipeline.DrawArrow(_reactionForceLine, Color);
            const int arrowHeadScreenSize = 20;
            Point3d point = CalculateExtraStartOffsetPoint(pixelsPerUnit, arrowHeadScreenSize);
            args.Pipeline.DrawArrowHead(point, Direction, Color, arrowHeadScreenSize, 0);
            break;
          }
      }

      if (_arrowHeadOutlines == null) {
        return;
      }

      for (int i = 0; i < _arrowHeadOutlines.Count; i++) {
        args.Pipeline.DrawPolyline(_arrowHeadOutlines[i], _arrowHeadOutlineColors[i]);
      }
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
      if (_arrowHeadOutlines != null) {
        return new DiagramGoo() {
          _arrowHead = _arrowHead,
          _arrowHeadOutlines = _arrowHeadOutlines,
          _arrowHeadOutlineColors = _arrowHeadOutlineColors,
        };
      }

      return new DiagramGoo(StartingPoint, Direction, ArrowMode);
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      GH_Vector vector = Value;
      vector.Value.Transform(xform);
      Line line = CreateReactionForceLine(vector.Value);
      return new BoundingBox(line.From, line.To);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      if (_arrowHeadOutlines != null) {
        Point3d sPoint = xmorph.MorphPoint(StartingPoint);
        return new DiagramGoo(sPoint, Direction, ArrowMode);
      }

      Mesh mesh = _arrowHead.DuplicateMesh();
      xmorph.Morph(mesh);

      var polylines = new List<Polyline>();
      _arrowHeadOutlines?.ForEach(item => {
        var polyline = new Polyline();
        foreach (Point3d point in item) {
          polyline.Add(xmorph.MorphPoint(point));
        }

        polylines.Add(polyline);
      });

      return new DiagramGoo() {
        _arrowHead = mesh,
        _arrowHeadOutlines = polylines,
        _arrowHeadOutlineColors = _arrowHeadOutlineColors,
      };
    }

    public override object ScriptVariable() {
      return Value;
    }

    public DiagramGoo SetColor(Color value) {
      Color = value;
      return this;
    }

    public override string ToString() {
      var pt = new GH_Point(StartingPoint);
      var vec = new GH_Vector(Direction);
      return $"Diagram Result: Starting point: {pt}, Direction:{vec}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      if (_arrowHeadOutlines != null) {
        Point3d sPoint = StartingPoint;
        sPoint.Transform(xform);
        return new DiagramGoo(sPoint, Direction, ArrowMode);
      }

      Mesh mesh = _arrowHead.DuplicateMesh();
      mesh.Transform(xform);

      var polylines = _arrowHeadOutlines?.ToList();
      polylines?.ForEach(item => item.Transform(xform));

      return new DiagramGoo() {
        _arrowHead = mesh,
        _arrowHeadOutlines = polylines,
        _arrowHeadOutlineColors = _arrowHeadOutlineColors,
      };
    }

    private Point3d CalculateExtraStartOffsetPoint(double pixelsPerUnit, int offset) {
      var point = new Point3d(_reactionForceLine.To);

      return TransformPoint(point, pixelsPerUnit, offset);
    }

    private Line CreateReactionForceLine(Vector3d direction) {
      var line = new Line(StartingPoint, direction);
      line.Flip();
      line.Transform(Rhino.Geometry.Transform.Scale(StartingPoint, -1));
      return line;
    }

    private GH_Vector GetGhVector() {
      return new GH_Vector(new Vector3d(_reactionForceLine.ToX - _reactionForceLine.FromX,
        _reactionForceLine.ToY - _reactionForceLine.FromY,
        _reactionForceLine.ToZ - _reactionForceLine.FromZ));
    }

    private Point3d TransformPoint(Point3d point, double pixelsPerUnit, int offset) {
      if (pixelsPerUnit == 0 || offset == 0) {
        return point;
      }

      Vector3d direction = _reactionForceLine.Direction;

      direction.Unitize();
      var t = Rhino.Geometry.Transform.Translation(direction * -1 * offset / pixelsPerUnit);
      point.Transform(t);

      return point;
    }

    private static Mesh CreateMeshFromTriangles(ReadOnlyCollection<Triangle> triangles) {
      var faces = new ConcurrentBag<Mesh>();
      Parallel.ForEach(triangles, tri => {
        var face = new Mesh();
        var col = (Color)tri.Colour;

        foreach (Double3 verticy in tri.Vertices) {
          face.Vertices.Add(verticy.X, verticy.Y, verticy.Z);
          face.VertexColors.Add(col);
        }

        face.Faces.AddFace(0, 1, 2);
        faces.Add(face);
      });
      var mesh = new Mesh();
      mesh.Append(faces);
      mesh.Vertices.CombineIdentical(true, false);
      return mesh;
    }

    private static List<Polyline> CreatePolylinesFromPolygon(ReadOnlyCollection<Polygon> polygons) {
      var polylines = new List<Polyline>();

      foreach (Polygon polygon in polygons) {
        var polyline = new Polyline();
        foreach (Vector3 polygonPoint in polygon.Points) {
          polyline.Add(polygonPoint.X, polygonPoint.Y, polygonPoint.Z);
        }

        if (!polyline.IsClosed) {
          Vector3 point = polygon.Points[0];
          polyline.Add(point.X, point.Y, point.Z);
        }

        polylines.Add(polyline);
      }

      return polylines;
    }
  }
}
