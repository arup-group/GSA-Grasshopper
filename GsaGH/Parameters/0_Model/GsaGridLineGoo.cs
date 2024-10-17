using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Graphics;

using OasysGH;
using OasysGH.Parameters;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaGridLine" /> can be used in Grasshopper.
  /// </summary>
  public class GsaGridLineGoo : GH_OasysGeometricGoo<GsaGridLine> {
    public static string Description => "GSA Grid Line";
    public static string Name => "Grid Line";
    public static string NickName => "GL";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaGridLineGoo(GsaGridLine item) : base(item) { }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaGridLineGoo(Value);
    }

    public override bool CastTo<Q>(ref Q target) {
      if (Value != null) {
        if (typeof(Q).IsAssignableFrom(typeof(GH_Curve))) {
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Line) {
            var line = GsaGridLine.ToLine(Value.GridLine);
            var ghLine = new GH_Curve();
            GH_Convert.ToGHCurve(line, GH_Conversion.Both, ref ghLine);
            target = (Q)(object)ghLine;
            return true;
          }
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Arc) {
            var arc = GsaGridLine.ToArc(Value.GridLine);
            var ghArc = new GH_Curve();
            GH_Convert.ToGHCurve(arc, GH_Conversion.Both, ref ghArc);
            target = (Q)(object)ghArc;
            return true;
          } else {
            return false;
          }
        }
        if (typeof(Q).IsAssignableFrom(typeof(GH_Line))) {
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Line) {
            var line = GsaGridLine.ToLine(Value.GridLine);
            var ghLine = new GH_Line();
            GH_Convert.ToGHLine(line, GH_Conversion.Both, ref ghLine);
            target = (Q)(object)ghLine;
            return true;
          } else {
            return false;
          }
        }
        if (typeof(Q).IsAssignableFrom(typeof(GH_Arc))) {
          if (Value.GridLine.Shape == GsaAPI.GridLineShape.Arc) {
            var arc = GsaGridLine.ToArc(Value.GridLine);
            var ghArc = new GH_Arc();
            GH_Convert.ToGHArc(arc, GH_Conversion.Both, ref ghArc);
            target = (Q)(object)ghArc;
            return true;
          } else {
            return false;
          }
        }
      }

      return base.CastTo(ref target);
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null || Value.Curve == null || !Value.Curve.IsValid) {
        return;
      }

      int thickness = 1;
      // this is a workaround to change colour between selected and not
      if (args.Color != Colours.EntityIsNotSelected) {
        thickness = 3;
      }


      if (Value.Points != null) {
        Color color = Color.Black;
        int pattern = 999999;
        args.Pipeline.DrawPatternedPolyline(Value.Points, color, pattern, thickness, false);

        if (args.Color != Colours.EntityIsNotSelected) {
          Value.Text.Bold = true;
        }

        args.Pipeline.Draw3dText(Value.Text, color);

        args.Pipeline.DrawCircle(Value.Circle, color, thickness);
      }
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value.Curve;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var gridline = new GsaGridLine(Value);
      xmorph.Morph(gridline.Curve);
      gridline.UpdatePreview();
      return new GsaGridLineGoo(gridline);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var gridline = new GsaGridLine(Value);
      gridline.Curve.Transform(xform);
      gridline.UpdatePreview();
      return new GsaGridLineGoo(gridline);
    }
  }
}
