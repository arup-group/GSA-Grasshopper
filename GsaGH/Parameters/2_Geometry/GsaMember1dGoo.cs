using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaMember1d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMember1dGoo : GH_OasysGeometricGoo<GsaMember1d> {
    public static string Description => "GSA 1D Member";
    public static string Name => "Member1D";
    public static string NickName => "M1D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMember1dGoo(GsaMember1d item) : base(item) { }

    internal GsaMember1dGoo(GsaMember1d item, bool duplicate) : base(null) {
      Value = duplicate ? item.Duplicate() : item;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Curve(Value.PolyCurve.DuplicatePolyCurve());
          return true;
        }
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

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null) {
        return;
      }

      if (Value.PolyCurve != null) {
        if (args.Color
          == Color.FromArgb(255, 150, 0,
            0)) // this is a workaround to change colour between selected and not
        {
          if (Value.IsDummy) {
            args.Pipeline.DrawDottedPolyline(Value.Topology, Colours.Dummy1D, false);
          } else {
            if (Value.Colour != Color.FromArgb(0, 0, 0)) {
              args.Pipeline.DrawCurve(Value.PolyCurve, Value.Colour, 2);
            } else {
              Color col = Colours.ElementType(Value.Type1D);
              args.Pipeline.DrawCurve(Value.PolyCurve, col, 2);
            }
          }
        } else {
          if (Value.IsDummy) {
            args.Pipeline.DrawDottedPolyline(Value.Topology, Colours.Member1dSelected, false);
          } else {
            args.Pipeline.DrawCurve(Value.PolyCurve, Colours.Member1dSelected, 2);
          }
        }
      }

      if (Value.Topology != null) {
        if (!Value.IsDummy) {
          List<Point3d> pts = Value.Topology;
          for (int i = 0; i < pts.Count; i++) {
            if (args.Color
              == Color.FromArgb(255, 150, 0,
                0)) // this is a workaround to change colour between selected and not
            {
              if (i == 0 | i == pts.Count - 1) // draw first point bigger
              {
                args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 2, Colours.Member1dNode);
              } else {
                args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 1, Colours.Member1dNode);
              }
            } else {
              if (i == 0 | i == pts.Count - 1) // draw first point bigger
              {
                args.Pipeline.DrawPoint(pts[i], PointStyle.RoundControlPoint, 2,
                  Colours.Member1dNodeSelected);
              } else {
                args.Pipeline.DrawPoint(pts[i], PointStyle.RoundControlPoint, 1,
                  Colours.Member1dNodeSelected);
              }
            }
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
      return new GsaMember1dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value.PolyCurve;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return new GsaMember1dGoo(Value.Morph(xmorph));
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      return new GsaMember1dGoo(Value.Transform(xform));
    }
  }
}
