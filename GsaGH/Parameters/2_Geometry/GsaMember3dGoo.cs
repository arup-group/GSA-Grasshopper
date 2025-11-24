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
  ///   Goo wrapper class, makes sure <see cref="GsaMember3D" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMember3dGoo : GH_OasysGeometricGoo<GsaMember3D>, IGH_PreviewData {
    public static string Description => "GSA 3D Member";
    public static string Name => "Member 3D";
    public static string NickName => "M3D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMember3dGoo(GsaMember3D item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        if (Value == null) {
          target = default;
        } else {
          target = (TQ)(object)new GH_Mesh(Value.SolidMesh);
          if (Value.SolidMesh == null) {
            return false;
          }
        }

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
      if (Value?.SolidMesh == null) {
        return;
      }

      if (!Value.ApiMember.IsDummy) {
        args.Pipeline.DrawMeshShaded(Value.SolidMesh,
          args.Material.Diffuse
          == Color.FromArgb(255, 150, 0,
            0) // this is a workaround to change colour between selected and not
            ? Colours.Element2dFace : Colours.Element2dFaceSelected); //UI.Colour.Member2dFace
      } else {
        args.Pipeline.DrawMeshShaded(Value.SolidMesh, Colours.Dummy2D);
      }
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value?.SolidMesh == null) {
        return;
      }

      if (Value.ApiMember.IsDummy) {
        foreach (Line line in Value.PreviewEdgeLines) {
          args.Pipeline.DrawDottedLine(line,
            args.Color
            == Color.FromArgb(255, 150, 0,
              0) // this is a workaround to change colour between selected and not
              ? Colours.Dummy1D : Colours.Member2dEdgeSelected);
        }
      } else {
        foreach (Line line in Value.PreviewEdgeLines) {
          if (args.Color
            == Color.FromArgb(255, 150, 0,
              0)) // this is a workaround to change colour between selected and not
          {
            if ((Color)Value.ApiMember.Colour != Color.FromArgb(0, 0, 0)) {
              args.Pipeline.DrawLine(line, (Color)Value.ApiMember.Colour, 2);
            } else {
              Color col = Colours.Member2dEdge;
              args.Pipeline.DrawLine(line, col, 2);
            }
          } else {
            args.Pipeline.DrawLine(line, Colours.Element2dEdgeSelected, 2);
          }
        }

        foreach (Polyline line in Value.PreviewHiddenLines) {
          args.Pipeline.DrawDottedPolyline(line, Colours.Dummy1D, false);
        }
      }

      foreach (Point3d point3d in Value.PreviewPts) {
        if (args.Color
          == Color.FromArgb(255, 150, 0,
            0)) // this is a workaround to change colour between selected and not
        {
          args.Pipeline.DrawPoint(point3d, PointStyle.RoundSimple, 2,
            Value.ApiMember.IsDummy ? Colours.Dummy1D : Colours.Member1dNode);
        } else {
          args.Pipeline.DrawPoint(point3d, PointStyle.RoundControlPoint, 3,
            Colours.Member1dNodeSelected);
        }
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaMember3dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value.SolidMesh;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var mem = new GsaMember3D(Value) {
        Id = 0
      };

      xmorph.Morph(mem.SolidMesh);
      mem.UpdatePreview();
      return new GsaMember3dGoo(mem);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var mem = new GsaMember3D(Value) {
        Id = 0
      };

      mem.SolidMesh.Transform(xform);
      mem.UpdatePreview();
      return new GsaMember3dGoo(mem);
    }
  }
}
