using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.Graphics;

using OasysGH;
using OasysGH.Parameters;

using Rhino.Collections;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaMember1d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMember1dGoo : GH_OasysGeometricGoo<GsaMember1d> {
    public static string Description => "GSA 1D Member";
    public static string Name => "Member 1D";
    public static string NickName => "M1D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMember1dGoo(GsaMember1d item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Curve(Value.PolyCurve.DuplicatePolyCurve());
          return true;
        }
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

      if (Value.PolyCurve != null) {
        // this is a workaround to change colour between selected and not
        if (args.Color == Colours.EntityIsNotSelected) {
          if (Value.ApiMember.IsDummy) {
            args.Pipeline.DrawDottedPolyline(Value.Topology, Colours.Dummy1D, false);
          } else {
            if ((Color)Value.ApiMember.Colour != Color.FromArgb(0, 0, 0)) {
              args.Pipeline.DrawCurve(Value.PolyCurve, (Color)Value.ApiMember.Colour, 2);
            } else {
              Color col = Colours.ElementType(Value.ApiMember.Type1D);
              args.Pipeline.DrawCurve(Value.PolyCurve, col, 2);
            }
          }
        } else {
          if (Value.ApiMember.IsDummy) {
            args.Pipeline.DrawDottedPolyline(Value.Topology, Colours.Member1dSelected, false);
          } else {
            args.Pipeline.DrawCurve(Value.PolyCurve, Colours.Member1dSelected, 2);
          }
        }
      }

      if (Value.Topology != null) {
        if (!Value.ApiMember.IsDummy) {
          Point3dList pts = Value.Topology;
          for (int i = 0; i < pts.Count; i++) {
            // this is a workaround to change colour between selected and not
            if (args.Color == Colours.EntityIsNotSelected) {
              if (i == 0 || i == pts.Count - 1) {
                // draw first point bigger
                args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 2, Colours.Member1dNode);
              } else {
                args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 1, Colours.Member1dNode);
              }
            } else {
              if (i == 0 || i == pts.Count - 1) {
                // draw first point bigger
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

      if (!Value.ApiMember.IsDummy) {
        Value.ReleasePreview.DrawViewportWires(args);
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaMember1dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      if (Value == null) {
        return null;
      }

      if (Value.Section3dPreview != null && Value.Section3dPreview.Mesh != null) {
        return Value.Section3dPreview.Mesh;
      }

      return Value.PolyCurve;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var mem = new GsaMember1d(Value) {
        Id = 0,
      };
      mem.Topology?.Morph(xmorph);
      PolyCurve crv = Value.PolyCurve.DuplicatePolyCurve();
      xmorph.Morph(crv);
      mem.PolyCurve = crv;
      mem.UpdateReleasesPreview();
      mem.Section3dPreview?.Morph(xmorph);
      return new GsaMember1dGoo(mem);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var xpts = new Point3dList(Value.Topology);
      xpts.Transform(xform);
      var mem = new GsaMember1d(Value) {
        Id = 0,
        Topology = xpts
      };
      PolyCurve crv = Value.PolyCurve.DuplicatePolyCurve();
      crv.Transform(xform);
      mem.PolyCurve = crv;
      mem.UpdateReleasesPreview();
      mem.Section3dPreview?.Transform(xform);
      return new GsaMember1dGoo(mem);
    }
  }
}
