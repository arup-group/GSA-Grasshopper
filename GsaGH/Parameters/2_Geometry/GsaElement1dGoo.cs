using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaElement1d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaElement1dGoo : GH_OasysGeometricGoo<GsaElement1d>, IGH_PreviewData
  {
    public static string Name => "1D Element";
    public static string NickName => "E1D";
    public static string Description => "GSA 1D Element";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement1dGoo(GsaElement1d item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaElement1dGoo(this.Value);

    public override bool IsValid
    {
      get
      {
        if (Value != null && Value.Line == null)
          return true;
        return false;
      }
    }

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaElement1d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Element)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.GetAPI_ElementClone();
        return true;
      }

      // Cast to Curve
      else if (typeof(Q).IsAssignableFrom(typeof(Line)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Line;
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Line ghLine = new GH_Line();
          GH_Convert.ToGHLine(Value.Line, GH_Conversion.Both, ref ghLine);
          target = (Q)(object)ghLine;
        }
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Line;
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Curve(Value.Line);
        }
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Integer ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from GsaElement
      else if (typeof(GsaElement1d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaElement1d)source;
        return true;
      }

      // Cast from GsaAPI Member
      // we shouldnt provide auto-convertion from GsaAPI.Element
      // as this cannot alone be used to create a line....
      //else if (typeof(Element).IsAssignableFrom(source.GetType()))
      //{
      //    Value.Element = (Element)source;
      //    return true;
      //}

      // Cast from Curve
      Line ln = new Line();
      if (GH_Convert.ToLine(source, ref ln, GH_Conversion.Both))
      {
        LineCurve crv = new LineCurve(ln);
        GsaElement1d elem = new GsaElement1d(crv);
        this.Value = elem;
        return true;
      }

      return base.CastFrom(source);
    }

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      if (Value == null) { return null; }
      if (Value.Line == null) { return null; }

      GsaElement1d elem = Value.Duplicate(true);
      elem.ID = 0;
      LineCurve xLn = elem.Line;
      xLn.Transform(xform);
      elem.Line = xLn;

      return new GsaElement1dGoo(elem);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.Line == null) { return null; }

      GsaElement1d elem = Value.Duplicate(true);
      LineCurve xLn = Value.Line;
      xmorph.Morph(xLn);
      elem.Line = xLn;

      return new GsaElement1dGoo(elem);
    }

    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      //no meshes to be drawn
    }
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      //Draw lines
      if (Value.Line != null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, UI.Colour.Dummy1D);
          else
          {
            if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
              args.Pipeline.DrawCurve(Value.Line, Value.Colour, 2);
            else
            {
              System.Drawing.Color col = UI.Colour.ElementType(Value.Type);
              args.Pipeline.DrawCurve(Value.Line, col, 2);
            }
            //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
            //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
          }
        }
        else
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, UI.Colour.Element1dSelected);
          else
          {
            args.Pipeline.DrawCurve(Value.Line, UI.Colour.Element1dSelected, 2);
            //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
            //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
          }
        }
      }
      //Draw releases
      if (!Value.IsDummy)
      {
        if (Value.previewGreenLines != null)
        {
          foreach (Line ln1 in Value.previewGreenLines)
            args.Pipeline.DrawLine(ln1, UI.Colour.Support);
          foreach (Line ln2 in Value.previewRedLines)
            args.Pipeline.DrawLine(ln2, UI.Colour.Release);
        }
      }
    }

    public override GeometryBase GetGeometry()
    {
      if (this.Value == null)
        return null;
      return this.Value.Line;
    }
    #endregion
  }
}
