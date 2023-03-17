using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;

namespace GsaGH.Parameters
{
  public class PointResultGoo : GH_GeometricGoo<Point3d>, IGH_PreviewData
  {
    public readonly int NodeId;
    public readonly IQuantity Result;
    private float _size;
    private Color _color;

    public PointResultGoo(Point3d point, IQuantity result, Color color, float size, int id)
    : base(point)
    {
      Result = result;
      _size = size;
      _color = color;
      NodeId = id;
    }

    public override string ToString()
    {
      return string.Format("PointResult: P:({0:0.0},{1:0.0},{2:0.0}) R:{3:0.0}", Value.X, Value.Y, Value.Z, Result);
    }
    public override string TypeName
    {
      get { return "Result Point"; }
    }
    public override string TypeDescription
    {
      get { return "A GSA result point type."; }
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return new PointResultGoo(Value, Result, _color, _size, NodeId);
    }
    public override BoundingBox Boundingbox
    {
      get
      {
        var box = new BoundingBox(Value, Value);
        box.Inflate(1);
        return box;
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      Point3d point = Value;
      point.Transform(xform);
      var box = new BoundingBox(point, point);
      box.Inflate(1);
      return box;
    }
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      Point3d point = Value;
      point.Transform(xform);
      return new PointResultGoo(point, Result, _color, _size, NodeId);
    }
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      Point3d point = xmorph.MorphPoint(Value);
      return new PointResultGoo(point, Result, _color, _size, NodeId);
    }

    public override object ScriptVariable() => Value;
    
    public override bool CastTo<TQ>(out TQ target)
    {
      if (typeof(TQ).IsAssignableFrom(typeof(Point3d)))
      {
        target = (TQ)(object)Value;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Point)))
      {
        target = (TQ)(object)new GH_Point(Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Number)))
      {
        target = (TQ)(object)new GH_Number(Result.Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer)))
      {
        target = (TQ)(object)new GH_Integer(NodeId);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Colour)))
      {
        target = (TQ)(object)new GH_Colour(_color);
        return true;
      }

      target = default(TQ);
      return false;
    }
    public override bool CastFrom(object source)
    {
      if (source == null) return false;
      if (source is Point3d)
      {
        Value = (Point3d)source;
        return true;
      }
      GH_Point pointGoo = source as GH_Point;
      if (pointGoo != null)
      {
        Value = pointGoo.Value;
        return true;
      }

      Point3d point = Point3d.Unset;
      if (GH_Convert.ToPoint3d(source, ref point, GH_Conversion.Both))
      {
        Value = point;
        return true;
      }

      return false;
    }

    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      args.Pipeline.DrawPoint(Value, PointStyle.RoundSimple, _size, _color);
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
  }
}
