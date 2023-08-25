using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaMember2d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMember2dGoo : GH_OasysGeometricGoo<GsaMember2d>, IGH_PreviewData {
    public static string Description => "GSA 2D Member";
    public static string Name => "Member 2D";
    public static string NickName => "M2D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMember2dGoo(GsaMember2d item) : base(item) { }

    internal GsaMember2dGoo(GsaMember2d item, bool duplicate) : base(null) {
      Value = duplicate ? item.Duplicate() : item;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Brep))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Brep(Value.Brep.DuplicateBrep());
          return true;
        }
      }

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
      if (Value == null || Value.Brep == null) {
        return;
      }

      if (Value.Type == MemberType.VOID_CUTTER_2D) {
        if (args.Material.Diffuse
          == Color.FromArgb(255, 150, 0,
            0)) // this is a workaround to change colour between selected and not
        {
          args.Pipeline.DrawBrepShaded(Value.Brep,
            Colours.Member2dVoidCutterFace); //UI.Colour.Member2dFace
        }
      } else {
        args.Pipeline.DrawBrepShaded(Value.Brep,
          args.Material.Diffuse
          == Color.FromArgb(255, 150, 0,
            0) // this is a workaround to change colour between selected and not
            ? Colours.Member2dFace : Colours.Member2dFaceSelected); //UI.Colour.Member2dFace
      }

      if (Value.Section3dPreview != null) {
        args.Pipeline.DrawMeshFalseColors(Value.Section3dPreview.Mesh);
      }
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null) {
        return;
      }

      if (Value.Brep != null) {
        if (args.Color
          == Color.FromArgb(255, 150, 0,
            0)) // this is a workaround to change colour between selected and not
        {
          if (Value.Type == MemberType.VOID_CUTTER_2D) {
            args.Pipeline.DrawBrepWires(Value.Brep, Colours.VoidCutter, -1);
          } else if (!Value.IsDummy) {
            args.Pipeline.DrawBrepWires(Value.Brep, Colours.Member2dEdge, -1);
          }
        } else {
          args.Pipeline.DrawBrepWires(Value.Brep, Colours.Member2dEdgeSelected, -1);
        }
      }

      if (Value.PolyCurve != null & Value.Brep == null) {
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
              Color col = Colours.Member2dEdge;
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

      if (Value.InclusionLines != null) {
        for (int i = 0; i < Value.InclusionLines.Count; i++) {
          if (Value.IsDummy) {
            args.Pipeline.DrawDottedPolyline(Value.IncLinesTopology[i], Colours.Member1dSelected,
              false);
          } else {
            args.Pipeline.DrawCurve(Value.InclusionLines[i], Colours.Member2dInclLn, 2);
          }
        }
      }

      if (Value.Topology != null) {
        List<Point3d> pts = Value.Topology;
        for (int i = 0; i < pts.Count; i++) {
          if (args.Color
            == Color.FromArgb(255, 150, 0,
              0)) // this is a workaround to change colour between selected and not
          {
            if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
            {
              args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 3,
                Value.IsDummy ? Colours.Dummy1D : Colours.Member1dNode);
            } else {
              args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 2,
                Value.IsDummy ? Colours.Dummy1D : Colours.Member1dNode);
            }
          } else {
            if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
            {
              args.Pipeline.DrawPoint(pts[i], PointStyle.RoundControlPoint, 3,
                Colours.Member1dNodeSelected);
            } else {
              args.Pipeline.DrawPoint(pts[i], PointStyle.RoundControlPoint, 2,
                Colours.Member1dNodeSelected);
            }
          }
        }
      }

      if (Value.InclusionPoints != null) {
        foreach (Point3d point3d in Value.InclusionPoints) {
          args.Pipeline.DrawPoint(point3d, PointStyle.RoundSimple, 3,
            Value.IsDummy ? Colours.Dummy1D : Colours.Member2dInclPt);
        }

      }

      if (Value.Section3dPreview != null) {
        if (args.Color == Color.FromArgb(255, 150, 0, 0)) {
          args.Pipeline.DrawLines(Value.Section3dPreview.Outlines, Colours.Member2dEdge);
        } else {
          args.Pipeline.DrawLines(Value.Section3dPreview.Outlines, Colours.Member2dEdgeSelected);
        }
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaMember2dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value.Brep;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return new GsaMember2dGoo(Value.Morph(xmorph));
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      return new GsaMember2dGoo(Value.Transform(xform));
    }
  }
}
