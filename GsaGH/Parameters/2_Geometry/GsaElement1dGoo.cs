using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using Rhino.Geometry;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaElement1d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaElement1dGoo : GH_OasysGeometricGoo<GsaElement1d>
  {
    public static string Name => "Element1D";
    public static string NickName => "E1D";
    public static string Description => "GSA 1D Element";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement1dGoo(GsaElement1d item) : base(item) { }
    internal GsaElement1dGoo(GsaElement1d item, bool duplicate) : base(null)
    {
      if (duplicate)
        this.Value = item.Duplicate();
      else
        this.Value = item;
    }

    public override IGH_GeometricGoo Duplicate() => new GsaElement1dGoo(this.Value);
    public override GeometryBase GetGeometry() => this.Value.Line;

    #region casting
    public override bool CastTo<Q>(ref Q target)
    {
      if (base.CastTo<Q>(ref target))
        return true;

      // Cast to Curve
      if (typeof(Q).IsAssignableFrom(typeof(Line)))
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
          if (GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      target = default;
      return false;
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      // Cast from Curve
      Line ln = new Line();
      if (GH_Convert.ToLine(source, ref ln, GH_Conversion.Both))
      {
        LineCurve crv = new LineCurve(ln);
        GsaElement1d elem = new GsaElement1d(crv);
        this.Value = elem;
        return true;
      }

      return false;
    }
    #endregion

    #region drawing methods
    public override void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      //no meshes to be drawn
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      //Draw lines
      if (Value.Line != null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, Helpers.Graphics.Colours.Dummy1D);
          else
          {
            if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
              args.Pipeline.DrawCurve(Value.Line, Value.Colour, 2);
            else
            {
              System.Drawing.Color col = Helpers.Graphics.Colours.ElementType(Value.Type);
              args.Pipeline.DrawCurve(Value.Line, col, 2);
            }
            //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
            //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
          }
        }
        else
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, Helpers.Graphics.Colours.Element1dSelected);
          else
          {
            args.Pipeline.DrawCurve(Value.Line, Helpers.Graphics.Colours.Element1dSelected, 2);
            //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
            //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
          }
        }
      }
      //Draw releases
      if (!Value.IsDummy)
      {
        if (Value.PreviewGreenLines != null)
        {
          foreach (Line ln1 in Value.PreviewGreenLines)
            args.Pipeline.DrawLine(ln1, Helpers.Graphics.Colours.Support);
          foreach (Line ln2 in Value.PreviewRedLines)
            args.Pipeline.DrawLine(ln2, Helpers.Graphics.Colours.Release);
        }
      }
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      return new GsaElement1dGoo(Value.Transform(xform));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      return new GsaElement1dGoo(Value.Morph(xmorph));
    }
    #endregion
  }
}
