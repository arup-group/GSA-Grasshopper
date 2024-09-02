using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Graphics;

using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;

using Rhino.Display;
using Rhino.Geometry;

using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaNode" /> can be used in Grasshopper.
  /// </summary>
  public class GsaNodeGoo : GH_OasysGeometricGoo<GsaNode>, IGH_PreviewData {
    public static string Description => "GSA Node";
    public static string Name => "Node";
    public static string NickName => "No";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaNodeGoo(GsaNode item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Point))) {
        target = Value == null ? default : (TQ)(object)new GH_Point(Value.Point);
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

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null || !Value.Point.IsValid) {
        return;
      }

      Value.SupportPreview?.DrawViewportWires(args);

      if (args.Color == Colours.EntityIsNotSelected) {
        // this is a workaround to change colour between selected and not
        if ((Color)Value.ApiNode.Colour != Color.FromArgb(0, 0, 0)) {
          args.Pipeline.DrawPoint(
            Value.Point, PointStyle.RoundSimple, 3, (Color)Value.ApiNode.Colour);
        } else {
          args.Pipeline.DrawPoint(Value.Point, PointStyle.RoundSimple, 3, Colours.Node);
        }

      } else {
        args.Pipeline.DrawPoint(Value.Point, PointStyle.RoundControlPoint, 3, Colours.NodeSelected);
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaNodeGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      if (Value?.Point == null) {
        return null;
      }

      if (Value.SupportPreview != null && Value.SupportPreview.SupportSymbol != null) {
        return Value.SupportPreview.SupportSymbol;
      }

      Point3d pt1 = Value.Point;
      pt1.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / 2;
      Point3d pt2 = Value.Point;
      pt2.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / -2;
      var ln = new Line(pt1, pt2);
      return new LineCurve(ln);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var node = new GsaNode(Value) {
        Id = 0
      };
      var pt = new Point3d(node.Point);
      xmorph.MorphPoint(pt);
      node.Point = pt;
      node.UpdatePreview();
      return new GsaNodeGoo(node);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var node = new GsaNode(Value) {
        Id = 0
      };
      var pt = new Point3d(node.Point);
      pt.Transform(xform);
      node.Point = pt;
      node.UpdatePreview();
      return new GsaNodeGoo(node);
    }
  }
}
