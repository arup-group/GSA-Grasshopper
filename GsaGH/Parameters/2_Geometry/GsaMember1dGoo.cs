using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
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

    public override bool CastFrom(object source) {
      // This function is called when Grasshopper needs to convert other data
      // into GsaMember.
      if (source == null) {
        return false;
      }

      if (base.CastFrom(source)) {
        return true;
      }

      Curve crv = null;
      if (!GH_Convert.ToCurve(source, ref crv, GH_Conversion.Both)) {
        return false;
      }

      var member = new GsaMember1d(crv);
      Value = member;
      return true;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      // This function is called when Grasshopper needs to convert this
      // instance of GsaMember into some other type Q.
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(Curve))) {
        target = Value == null ? default : (TQ)(object)Value.PolyCurve.DuplicatePolyCurve();
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        if (Value == null) {
          target = default;
        } else {
          target = (TQ)(object)new GH_Curve(Value.PolyCurve.DuplicatePolyCurve());
          if (Value.PolyCurve == null) {
            return false;
          }
        }

        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(PolyCurve))) {
        if (Value == null) {
          target = default;
        } else {
          target = (TQ)(object)Value.PolyCurve.DuplicatePolyCurve();
          if (Value.PolyCurve == null) {
            return false;
          }
        }

        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(Polyline))) {
        if (Value == null) {
          target = default;
        } else {
          target = (TQ)(object)Value.PolyCurve.DuplicatePolyCurve();
          if (Value.PolyCurve == null) {
            return false;
          }
        }

        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(Line))) {
        if (Value == null) {
          target = default;
        } else {
          target = (TQ)(object)Value.PolyCurve.ToPolyline(
            DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry), 2, 0, 0);
          if (Value.PolyCurve == null) {
            return false;
          }
        }

        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null) {
          target = default;
        } else {
          var ghint = new GH_Integer();
          target = GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint) ?
            (TQ)(object)ghint : default;
        }

        return true;
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
      return Value.PolyCurve;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return new GsaMember1dGoo(Value.Morph(xmorph));
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      return new GsaMember1dGoo(Value.Transform(xform));
    }
  }
}
