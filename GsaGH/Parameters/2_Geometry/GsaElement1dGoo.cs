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
    public static string Name => "Element 1D";
    public static string NickName => "E1D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement1dGoo(GsaElement1d item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Line))) {
        if (Value != null) {
          var ghLine = new GH_Line();
          GH_Convert.ToGHLine(Value.Line, GH_Conversion.Both, ref ghLine);
          target = (TQ)(object)ghLine;
          return true;
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        target = Value == null ? default : (TQ)(object)new GH_Curve(Value.Line);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh)) && Value.Section3dPreview != null) {
        target = Value == null ? default : (TQ)(object)new GH_Mesh(Value.Section3dPreview.Mesh);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Integer(Value.Id);
          return true;
        }
      }

      target = default;
      return false;
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      Value?.Section3dPreview?.DrawViewportMeshes(args);
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null) {
        return;
      }

      Value.Section3dPreview?.DrawViewportWires(args);

      if (Value.Line != null) {
        // this is a workaround to change colour between selected and not
        if (args.Color == Colours.EntityIsNotSelected) {
          if (Value.ApiElement.IsDummy) {
            args.Pipeline.DrawDottedLine(Value.Line.PointAtStart, Value.Line.PointAtEnd,
              Colours.Dummy1D);
          } else {
            if ((Color)Value.ApiElement.Colour != Color.FromArgb(0, 0, 0)) {
              args.Pipeline.DrawCurve(Value.Line, (Color)Value.ApiElement.Colour, 2);
            } else {
              Color col = Colours.ElementType(Value.ApiElement.Type);
              args.Pipeline.DrawCurve(Value.Line, col, 2);
            }
          }
        } else {
          if (Value.ApiElement.IsDummy) {
            args.Pipeline.DrawDottedLine(Value.Line.PointAtStart, Value.Line.PointAtEnd,
              Colours.Element1dSelected);
          } else {
            args.Pipeline.DrawCurve(Value.Line, Colours.Element1dSelected, 2);
          }
        }
      }

      if (!Value.ApiElement.IsDummy) {
        Value.ReleasePreview?.DrawViewportWires(args);
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaElement1dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      if (Value == null) {
        return null;
      }

      if (Value.Section3dPreview != null && Value.Section3dPreview.Mesh != null) {
        return Value.Section3dPreview.Mesh;
      }

      return Value.Line;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var elem = new GsaElement1d(Value) {
        Id = 0,
      };
      LineCurve xLn = elem.Line;
      xmorph.Morph(xLn);
      elem.Line = xLn;
      elem.UpdateReleasesPreview();
      elem.Section3dPreview?.Morph(xmorph);
      return new GsaElement1dGoo(elem);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var elem = new GsaElement1d(Value) {
        Id = 0,
      };
      LineCurve xLn = elem.Line;
      xLn.Transform(xform);
      elem.Line = xLn;
      elem.UpdateReleasesPreview();
      elem.Section3dPreview?.Transform(xform);
      return new GsaElement1dGoo(elem);
    }
  }
}
