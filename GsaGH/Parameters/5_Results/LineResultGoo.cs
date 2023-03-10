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
    public LineResultGoo(Line line, IQuantity result1, IQuantity result2, Color color1, Color color2, float size1, float size2, int id) : base(line)
    {
      _id = id;
      _result1 = result1;
      _result2 = result2;
      _size1 = size1;
      _size2 = size2;
      _color1 = color1;
      _color2 = color2;

      Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = new Grasshopper.GUI.Gradient.GH_Gradient();
      gH_Gradient.AddGrip(0, _color1);
      gH_Gradient.AddGrip(1, _color2);
      previewResultSegments = new List<Line>();
      previewResultColours = new List<Color>();
      previewResultThk = new List<int>();

      previewResultColours.Add(gH_Gradient.ColourAt(0));
      previewResultThk.Add((int)Math.Abs(((_size2 - _size1) * 0 + _size1)));
      previewResultSegments.Add(new Line(Value.PointAt(0), Value.PointAt(0.5)));

      previewResultColours.Add(gH_Gradient.ColourAt(1));
      previewResultThk.Add((int)Math.Abs(((_size2 - _size1) * 1 + _size1)));
      previewResultSegments.Add(new Line(Value.PointAt(0.5), Value.PointAt(1)));
    }

    internal int _id;
    internal IQuantity _result1;
    internal IQuantity _result2;
    internal List<Line> previewResultSegments;
    internal List<Color> previewResultColours;
    internal List<int> previewResultThk;
    private float _size1;
    private float _size2;
    private Color _color1;
    private Color _color2;

    public override string ToString() => $"LineResult: L:{Value.Length:0.0}, R1:{Result1:0.0}, R2:{Result2:0.0}";

    public override string TypeName => "Result Line";

    public override string TypeDescription => "A GSA result line type.";

    public override IGH_GeometricGoo DuplicateGeometry() => new LineResultGoo(Value, Result1, Result2, _color1, _color2, _size1, _size2);

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
      return new LineResultGoo(ln, _result1, _result2, _color1, _color2, _size1, _size2, _id);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      Point3d start = xmorph.MorphPoint(Value.From);
      Point3d end = xmorph.MorphPoint(Value.To);
      Line ln = new Line(start, end);
      return new LineResultGoo(ln, _result1, _result2, _color1, _color2, _size1, _size2, _id);
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
        target = (TQ)(object)new GH_Integer(_id);
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
      for (int i = 0; i < PreviewResultSegments.Count; i++)
        args.Pipeline.DrawLine(
            PreviewResultSegments[i],
            PreviewResultColors[i],
            PreviewResultThk[i]);
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
  }
}
