using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaElement1d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaElement1dGoo : GH_OasysGeometricGoo<GsaElement1d> {
    public static string Description => "GSA 1D Element";
    public static string Name => "Element1D";
    public static string NickName => "E1D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement1dGoo(GsaElement1d item) : base(item) { }

    internal GsaElement1dGoo(GsaElement1d item, bool duplicate) : base(null) {
      Value = duplicate
                                                                                     ? item.Duplicate()
                                                                                     : item;
    }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (base.CastFrom(source)) {
        return true;
      }

      var ln = new Line();
      if (!GH_Convert.ToLine(source, ref ln, GH_Conversion.Both)) {
        return false;
      }

      var crv = new LineCurve(ln);
      var elem = new GsaElement1d(crv);
      Value = elem;
      return true;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(Line))) {
        target = Value == null
          ? default
          : (TQ)(object)Value.Line;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Line))) {
        if (Value == null) {
          target = default;
        }
        else {
          var ghLine = new GH_Line();
          GH_Convert.ToGHLine(Value.Line, GH_Conversion.Both, ref ghLine);
          target = (TQ)(object)ghLine;
        }

        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(Curve))) {
        target = Value == null
          ? default
          : (TQ)(object)Value.Line;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        target = Value == null
          ? default
          : (TQ)(object)new GH_Curve(Value.Line);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null) {
          target = default;
        }
        else {
          var ghint = new GH_Integer();
          target = GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint)
            ? (TQ)(object)ghint
            : default;
        }

        return true;
      }

      target = default;
      return false;
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null) {
        return;
      }

      if (Value.Line != null) {
        if (args.Color == Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if (Value.IsDummy) {
            args.Pipeline.DrawDottedLine(Value.Line.PointAtStart,
              Value.Line.PointAtEnd,
              Colours.Dummy1D);
          }
          else {
            if (Value.Colour != Color.FromArgb(0, 0, 0)) {
              args.Pipeline.DrawCurve(Value.Line, Value.Colour, 2);
            }
            else {
              Color col = Colours.ElementType(Value.Type);
              args.Pipeline.DrawCurve(Value.Line, col, 2);
            }
          }
        }
        else {
          if (Value.IsDummy) {
            args.Pipeline.DrawDottedLine(Value.Line.PointAtStart,
              Value.Line.PointAtEnd,
              Colours.Element1dSelected);
          }
          else {
            args.Pipeline.DrawCurve(Value.Line, Colours.Element1dSelected, 2);
          }
        }
      }

      if (Value.IsDummy || Value._previewGreenLines == null) {
        return;
      }

      foreach (Line ln1 in Value._previewGreenLines) {
        args.Pipeline.DrawLine(ln1, Colours.Support);
      }

      foreach (Line ln2 in Value._previewRedLines) {
        args.Pipeline.DrawLine(ln2, Colours.Release);
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaElement1dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      return Value.Line;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return new GsaElement1dGoo(Value.Morph(xmorph));
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      return new GsaElement1dGoo(Value.Transform(xform));
    }
  }
}
