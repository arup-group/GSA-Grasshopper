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
  ///   Goo wrapper class, makes sure <see cref="GsaGridPlaneSurface" /> can be used in Grasshopper.
  /// </summary>
  public class GsaGridPlaneSurfaceGoo : GH_OasysGeometricGoo<GsaGridPlaneSurface>, IGH_PreviewData {
    public static string Description => "GSA Grid Plane Surface";
    public static string Name => "Grid Plane Surface";
    public static string NickName => "GPS";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaGridPlaneSurfaceGoo(GsaGridPlaneSurface item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Plane))) {
        if (Value != null) {
          var pln = new GH_Plane();
          GH_Convert.ToGHPlane(Value.Plane, GH_Conversion.Both, ref pln);
          target = (TQ)(object)pln;
          return true;
        }
      }

      target = default;
      return false;
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null || !Value.Plane.IsValid) {
        return;
      }

      if (args.Color
        == Color.FromArgb(255, 150, 0,
          0)) // this is a workaround to change colour between selected and not
      {
        GH_Plane.DrawPlane(args.Pipeline, Value.PreviewPlane, 16, 16, Color.Gray, Color.Red, Color.Green);
        args.Pipeline.DrawPoint(Value.PreviewPlane.Origin, PointStyle.RoundSimple, 3, Colours.Node);
      } else {
        GH_Plane.DrawPlane(args.Pipeline, Value.PreviewPlane, 16, 16, Color.LightGray, Color.Red,
          Color.Green);
        args.Pipeline.DrawPoint(Value.PreviewPlane.Origin, PointStyle.RoundControlPoint, 3,
          Colours.NodeSelected);
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaGridPlaneSurfaceGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      if (Value?.Plane.Origin == null) {
        return null;
      }

      Point3d pt1 = Value.Plane.Origin;
      pt1.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / 2;
      Point3d pt2 = Value.Plane.Origin;
      pt2.Z += DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry) / -2;
      var ln = new Line(pt1, pt2);
      return new LineCurve(ln);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      if (Value?.Plane == null) {
        return null;
      }

      GsaGridPlaneSurface dup = Value.Duplicate();
      Plane pln = dup.Plane;
      xmorph.Morph(ref pln);
      var gridplane = new GsaGridPlaneSurface(pln);
      return new GsaGridPlaneSurfaceGoo(gridplane);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      if (Value?.Plane == null) {
        return null;
      }

      GsaGridPlaneSurface dup = Value.Duplicate();
      Plane pln = dup.Plane;
      pln.Transform(xform);
      var gridplane = new GsaGridPlaneSurface(pln);
      return new GsaGridPlaneSurfaceGoo(gridplane);
    }
  }
}
