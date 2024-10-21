using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.Graphics;

using OasysGH;
using OasysGH.Parameters;

using Rhino.Collections;
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

      if (Value.ApiMember.Type == MemberType.VOID_CUTTER_2D) {
        // this is a workaround to change colour between selected and not
        if (args.Material.Diffuse == Colours.EntityIsNotSelected) {
          args.Pipeline.DrawBrepShaded(Value.Brep,
            Colours.Member2dVoidCutterFace);
        }
      } else {
        args.Pipeline.DrawBrepShaded(Value.Brep,
          // this is a workaround to change colour between selected and not
          args.Material.Diffuse == Colours.EntityIsNotSelected
            ? Colours.Member2dFace : Colours.Member2dFaceSelected);
        Value?.Section3dPreview?.DrawViewportMeshes(args);
      }

      if (Value.Section3dPreview != null) {
        args.Pipeline.DrawMeshFalseColors(Value.Section3dPreview.Mesh);
      }
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null) {
        return;
      }

      Value.Section3dPreview?.DrawViewportWires(args);

      // this is a workaround to change colour between selected and not
      bool selected = args.Color != Colours.EntityIsNotSelected;
      if (Value.Brep != null) {
        if (!selected) {
          if (Value.ApiMember.Type == MemberType.VOID_CUTTER_2D) {
            args.Pipeline.DrawBrepWires(Value.Brep, Colours.VoidCutter, -1);
          } else if (!Value.ApiMember.IsDummy) {
            args.Pipeline.DrawBrepWires(Value.Brep, Colours.Member2dEdge, -1);
          }
        } else {
          args.Pipeline.DrawBrepWires(Value.Brep, Colours.Member2dEdgeSelected, -1);
        }
      }

      if (Value.PolyCurve != null && Value.Brep == null) {
        if (!selected) {
          if (Value.ApiMember.IsDummy) {
            args.Pipeline.DrawDottedPolyline(Value.Topology, Colours.Dummy1D, false);
          } else {
            if ((Color)Value.ApiMember.Colour != Color.FromArgb(0, 0, 0)) {
              args.Pipeline.DrawCurve(Value.PolyCurve, (Color)Value.ApiMember.Colour, 2);
            } else {
              Color col = Colours.Member2dEdge;
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

      if (Value.InclusionLines != null) {
        for (int i = 0; i < Value.InclusionLines.Count; i++) {
          if (Value.ApiMember.IsDummy) {
            args.Pipeline.DrawDottedPolyline(Value.InclusionLinesTopology[i], Colours.Member1dSelected,
              false);
          } else {
            args.Pipeline.DrawCurve(Value.InclusionLines[i], Colours.Member2dInclLn, 2);
          }
        }
      }

      if (Value.Topology != null) {
        Point3dList pts = Value.Topology;
        for (int i = 0; i < pts.Count; i++) {
          if (!selected) {
            if (Value.Brep == null && (i == 0 || i == pts.Count - 1)) {
              // draw first point bigger
              args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 3,
                Value.ApiMember.IsDummy ? Colours.Dummy1D : Colours.Member1dNode);
            } else {
              args.Pipeline.DrawPoint(pts[i], PointStyle.RoundSimple, 2,
                Value.ApiMember.IsDummy ? Colours.Dummy1D : Colours.Member1dNode);
            }
          } else {
            if (Value.Brep == null && (i == 0 || i == pts.Count - 1)) {
              // draw first point bigger
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
            Value.ApiMember.IsDummy ? Colours.Dummy1D : Colours.Member2dInclPt);
        }

      }

      if (Value.Section3dPreview != null) {
        if (args.Color == Colours.EntityIsNotSelected) {
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
      if (Value == null) {
        return null;
      }

      if (Value.Section3dPreview != null && Value.Section3dPreview.Mesh != null) {
        return Value.Section3dPreview.Mesh;
      }

      return Value.Brep;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var mem = new GsaMember2d(Value) {
        Id = 0
      };

      if (mem.Brep != null) {
        xmorph.Morph(mem.Brep);
      }

      if (mem.PolyCurve != null) {
        xmorph.Morph(mem.PolyCurve);
      }

      mem.Topology?.Morph(xmorph);

      for (int i = 0; i < mem.VoidCurves.Count; i++) {
        if (mem.VoidCurves[i] != null) {
          xmorph.Morph(mem.VoidCurves[i]);
        }

        mem.VoidTopology[i]?.Morph(xmorph);
      }

      for (int i = 0; i < mem.InclusionLines.Count; i++) {
        if (mem.InclusionLines[i] != null) {
          xmorph.Morph(mem.InclusionLines[i]);
        }

        mem.InclusionLinesTopology[i]?.Morph(xmorph);
      }

      mem.InclusionPoints?.Morph(xmorph);
      mem.Section3dPreview?.Morph(xmorph);

      return new GsaMember2dGoo(mem);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var mem = new GsaMember2d(Value) {
        Id = 0
      };

      mem.Brep?.Transform(xform);
      mem.PolyCurve?.Transform(xform);
      mem.Topology?.Transform(xform);
      for (int i = 0; i < mem.VoidCurves.Count; i++) {
        mem.VoidCurves[i]?.Transform(xform);
        mem.VoidTopology[i]?.Transform(xform); ;
      }

      for (int i = 0; i < mem.InclusionLines.Count; i++) {
        mem.InclusionLines[i]?.Transform(xform);
        mem.InclusionLinesTopology[i]?.Transform(xform);
      }

      mem.InclusionPoints?.Transform(xform);
      mem.Section3dPreview?.Transform(xform);
      return new GsaMember2dGoo(mem);
    }
  }
}
