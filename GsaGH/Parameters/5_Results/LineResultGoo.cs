using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="Line"/> can be used in Grasshopper.
  /// </summary>
  public class LineResultGoo : GH_GeometricGoo<Line>, IGH_PreviewData
  {
    public int ElementId;
    public IQuantity Result1;
    public IQuantity Result2;
    internal List<Line> resultLineSegments;
    internal List<Color> _previewResultColours;
    internal List<int> _previewResultThk;
    private float _size1;
    private float _size2;
    private Color _color1;
    private Color _color2;

    public LineResultGoo(Line line, IQuantity result1, IQuantity result2, Color color1, Color color2, float size1, float size2, int id) : base(line)
    {
      ElementId = id;
      Result1 = result1;
      Result2 = result2;
      _size1 = size1;
      _size2 = size2;
      _color1 = color1;
      _color2 = color2;

      Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = new Grasshopper.GUI.Gradient.GH_Gradient();
      gH_Gradient.AddGrip(0, _color1);
      gH_Gradient.AddGrip(1, _color2);
      resultLineSegments = new List<Line>();
      _previewResultColours = new List<Color>();
      _previewResultThk = new List<int>();

      _previewResultColours.Add(gH_Gradient.ColourAt(0));
      _previewResultThk.Add((int)Math.Abs(((_size2 - _size1) * 0 + _size1)));
      resultLineSegments.Add(new Line(Value.PointAt(0), Value.PointAt(0.5)));

      _previewResultColours.Add(gH_Gradient.ColourAt(1));
      _previewResultThk.Add((int)Math.Abs(((_size2 - _size1) * 1 + _size1)));
      resultLineSegments.Add(new Line(Value.PointAt(0.5), Value.PointAt(1)));
    }

    public override string ToString() => $"LineResult: L:{Value.Length:0.0}, R1:{Result1:0.0}, R2:{Result2:0.0}";

    public override string TypeName => "Result Line";

    public override string TypeDescription => "A GSA result line type.";

    public override IGH_GeometricGoo DuplicateGeometry() => new LineResultGoo(Value, Result1, Result2, _color1, _color2, _size1, _size2, ElementId);

    public override BoundingBox Boundingbox => Value.BoundingBox;

    public override BoundingBox GetBoundingBox(Transform xform)
    {
      Line ln = Value;
      ln.Transform(xform);
      return ln.BoundingBox;
    }

    public override IGH_GeometricGoo Transform(Transform xform)
    {
      Line ln = Value;
      ln.Transform(xform);
      return new LineResultGoo(ln, Result1, Result2, _color1, _color2, _size1, _size2, ElementId);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      Point3d start = xmorph.MorphPoint(Value.From);
      Point3d end = xmorph.MorphPoint(Value.To);
      Line ln = new Line(start, end);
      return new LineResultGoo(ln, Result1, Result2, _color1, _color2, _size1, _size2, ElementId);
    }

    public override object ScriptVariable() => Value;

    public override bool CastTo<TQ>(out TQ target)
    {
      if (typeof(TQ).IsAssignableFrom(typeof(Line)))
      {
        target = (TQ)(object)Value;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Line)))
      {
        target = (TQ)(object)new GH_Line(Value);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve)))
      {
        target = (TQ)(object)new GH_Curve(Value.ToNurbsCurve());
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer)))
      {
        target = (TQ)(object)new GH_Integer(ElementId);
        return true;
      }

      target = default(TQ);
      return false;
    }
    public override bool CastFrom(object source)
    {
      if (source == null) return false;
      if (source is Line)
      {
        Value = (Line)source;
        return true;
      }
      GH_Line lineGoo = source as GH_Line;
      if (lineGoo != null)
      {
        Value = lineGoo.Value;
        return true;
      }

      var line = new Line();
      if (!GH_Convert.ToLine(source, ref line, GH_Conversion.Both)) return false;
      Value = line;

      return true;
    }

    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }

    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      for (int i = 0; i < resultLineSegments.Count; i++)
        args.Pipeline.DrawLine(
            resultLineSegments[i],
            _previewResultColours[i],
            _previewResultThk[i]);
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
  }
}
