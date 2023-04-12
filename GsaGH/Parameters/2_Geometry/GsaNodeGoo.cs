using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using Rhino.Display;
using Rhino.Geometry;
using Point = Rhino.Geometry.Point;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaNode" /> can be used in Grasshopper.
  /// </summary>
  public class GsaNodeGoo : GH_OasysGeometricGoo<GsaNode>,
    IGH_PreviewData {
    public static string Description => "GSA Node";
    public static string Name => "Node";
    public static string NickName => "No";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaNodeGoo(GsaNode item) : base(item) { }

    internal GsaNodeGoo(GsaNode item, bool duplicate) : base(null)
                                      => Value = duplicate
        ? item.Duplicate()
        : item;

    public override bool CastFrom(object source) {
      // This function is called when Grasshopper needs to convert other data
      // into GsaNode.
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      if (typeof(Node).IsAssignableFrom(source.GetType())) {
        Value = new GsaNode {
          ApiNode = (Node)source,
        };
        return true;
      }

      var pt = new Point3d();
      if (!GH_Convert.ToPoint3d(source, ref pt, GH_Conversion.Both))
        return false;
      Value = new GsaNode(pt);
      return true;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      // This function is called when Grasshopper needs to convert this
      // instance of GsaNode into some other type Q.
      if (base.CastTo(ref target))
        return true;

      //Cast to Point3d
      if (typeof(TQ).IsAssignableFrom(typeof(Point3d))) {
        target = Value == null
          ? default
          : (TQ)(object)new Point3d(Value.Point);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Point))) {
        target = Value == null
          ? default
          : (TQ)(object)new GH_Point(Value.Point);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(Point))) {
        target = Value == null
          ? default
          : (TQ)(object)new Point(Value.Point);
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null)
          target = default;
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
      if (Value == null
        || !Value.Point.IsValid)
        return;
      if (args.Color == Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
      {
        if (Value.Colour != Color.FromArgb(0, 0, 0))
          args.Pipeline.DrawPoint(Value.Point, PointStyle.RoundSimple, 3, Value.Colour);
        else {
          Color col = Colours.Node;
          args.Pipeline.DrawPoint(Value.Point, PointStyle.RoundSimple, 3, col);
        }

        if (Value._previewSupportSymbol != null)
          args.Pipeline.DrawBrepShaded(Value._previewSupportSymbol, Colours.SupportSymbol);
        if (Value._previewText != null)
          args.Pipeline.Draw3dText(Value._previewText, Colours.Support);
      }
      else {
        args.Pipeline.DrawPoint(Value.Point,
          PointStyle.RoundControlPoint,
          3,
          Colours.NodeSelected);
        if (Value._previewSupportSymbol != null)
          args.Pipeline.DrawBrepShaded(Value._previewSupportSymbol,
            Colours.SupportSymbolSelected);
        if (Value._previewText != null)
          args.Pipeline.Draw3dText(Value._previewText, Colours.NodeSelected);
      }

      if (!Value.IsGlobalAxis()) {
        args.Pipeline.DrawLine(Value._previewXaxis, Color.FromArgb(255, 244, 96, 96), 1);
        args.Pipeline.DrawLine(Value._previewYaxis, Color.FromArgb(255, 96, 244, 96), 1);
        args.Pipeline.DrawLine(Value._previewZaxis, Color.FromArgb(255, 96, 96, 234), 1);
      }
    }

    public override IGH_GeometricGoo Duplicate() => new GsaNodeGoo(Value);

    public override GeometryBase GetGeometry() {
      if (Value?.Point == null)
        return null;
      Point3d pt1 = Value.Point;
      pt1.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / 2;
      Point3d pt2 = Value.Point;
      pt2.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / -2;
      var ln = new Line(pt1, pt2);
      return new LineCurve(ln);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
      => new GsaNodeGoo(Value.Morph(xmorph));

    public override IGH_GeometricGoo Transform(Transform xform)
      => new GsaNodeGoo(Value.Transform(xform));
  }
}
