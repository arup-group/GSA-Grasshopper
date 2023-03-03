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
    public PointResultGoo(Point3d point, IQuantity result, Color color, float size)
    : base(point)
    {
      MResult = result;
      _mSize = size;
      _mColor = color;
    }

    private readonly float _mSize;
    private readonly Color _mColor;

    internal IQuantity MResult;

    public override string ToString() => $"PointResult: P:({Value.X:0.0},{Value.Y:0.0},{Value.Z:0.0}) R:{MResult:0.0}";
    
    public override string TypeName => "Result Point";

    public override string TypeDescription => "A GSA result point type.";

    public override IGH_GeometricGoo DuplicateGeometry() => new PointResultGoo(Value, MResult, _mColor, _mSize);
    
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
      return new PointResultGoo(point, MResult, _mColor, _mSize);
    }
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      Point3d point = xmorph.MorphPoint(Value);
      return new PointResultGoo(point, MResult, _mColor, _mSize);
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
        target = (TQ)(object)new GH_Number(MResult.Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Colour)))
      {
        target = (TQ)(object)new GH_Colour(_mColor);
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

      var point = Point3d.Unset;
      if (!GH_Convert.ToPoint3d(source, ref point, GH_Conversion.Both)) return false;
      
      Value = point;
      
      return true;
    }

    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      args.Pipeline.DrawPoint(Value, PointStyle.RoundSimple, _mSize, _mColor);
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
  }
}
