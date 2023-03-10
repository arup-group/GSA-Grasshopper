using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System.Drawing;
using Rhino.Display;
using OasysUnits;

namespace GsaGH.Parameters
{
  public class PointResultGoo : GH_GeometricGoo<Point3d>, IGH_PreviewData
  {
    public PointResultGoo(Point3d point, IQuantity result, Color color, float size, int id)
    : base(point)
    {
      _result = result;
      _size = size;
      _color = color;
      _id = id;
    }

    internal IQuantity _result;
    private float _size;
    private Color _color;
    private int _id;

    public override string ToString()
    {
      return string.Format("PointResult: P:({0:0.0},{1:0.0},{2:0.0}) R:{3:0.0}", Value.X, Value.Y, Value.Z, _result);
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
      return new PointResultGoo(Value, _result, _color, _size, _id);
    }
    public override BoundingBox Boundingbox
    {
      get
      {
        BoundingBox box = new BoundingBox(Value, Value);
        box.Inflate(1);
        return box;
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      Point3d point = Value;
      point.Transform(xform);
      BoundingBox box = new BoundingBox(point, point);
      box.Inflate(1);
      return box;
    }
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      Point3d point = Value;
      point.Transform(xform);
      return new PointResultGoo(point, _result, _color, _size, _id);
    }
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      Point3d point = xmorph.MorphPoint(Value);
      return new PointResultGoo(point, _result, _color, _size, _id);
    }

    public override object ScriptVariable()
    {
      return Value;
    }
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
        target = (TQ)(object)new GH_Number(_result.Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer)))
      {
        target = (TQ)(object)new GH_Integer(_id);
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

    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      args.Pipeline.DrawPoint(Value, PointStyle.RoundSimple, _size, _color);
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
  }
}
