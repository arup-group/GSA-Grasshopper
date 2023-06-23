using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaLoad" /> can be used in Grasshopper.
  /// </summary>
  public class GsaLoadGoo : GH_OasysGoo<GsaLoad> {
    public static string Description => "GSA Load";
    public static string Name => "Load";
    public static string NickName => "Ld";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaLoadGoo(GsaLoad item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Plane))) {
        if (Value != null) {
          switch (Value.LoadType) {
            case GsaLoad.LoadTypes.GridArea: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(Value.AreaLoad.GridPlaneSurface.Axis, GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
            case GsaLoad.LoadTypes.GridLine: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(Value.LineLoad.GridPlaneSurface.Axis, GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
            case GsaLoad.LoadTypes.GridPoint: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(Value.PointLoad.GridPlaneSurface.Axis, GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
          }
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Point))) {
        if (Value == null) {
          target = default;
        } else {
          if (Value.LoadType != GsaLoad.LoadTypes.GridPoint) {
            return false;
          }

          var point = new Point3d {
            X = Value.PointLoad.GridPointLoad.X,
            Y = Value.PointLoad.GridPointLoad.Y,
            Z = Value.PointLoad.GridPlaneSurface.Axis.OriginZ,
          };
          var ghpt = new GH_Point();
          GH_Convert.ToGHPoint(point, GH_Conversion.Both, ref ghpt);
          target = (TQ)(object)ghpt;
          return true;
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        if (Value == null) {
          target = default;
        } else {
          if (Value.LoadType == GsaLoad.LoadTypes.GridLine
            && Value.LineLoad.Points.Count > 0) {
            var crv = new Polyline(Value.LineLoad.Points);
            target = (TQ)(object)new GH_Curve(crv.ToPolylineCurve());
            return true;
          } else if (Value.LoadType == GsaLoad.LoadTypes.GridArea
            && Value.AreaLoad.Points.Count > 0) {
            var crv = new Polyline(Value.AreaLoad.Points);
            target = (TQ)(object)new GH_Curve(crv.ToPolylineCurve());
            return true;
          }
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Surface))) {
        if (Value == null) {
          target = default;
        } else {
          if (Value.LoadType == GsaLoad.LoadTypes.GridArea) {
            var crv = new Polyline(Value.AreaLoad.Points);
            Brep brp = Brep.CreatePlanarBreps(crv.ToPolylineCurve(), 0.000001)[0];
            target = (TQ)(object)new GH_Surface(brp);
            return true;
          }
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Brep))) {
        if (Value == null) {
          target = default;
        } else {
          if (Value.LoadType == GsaLoad.LoadTypes.GridArea) {
            var crv = new Polyline(Value.AreaLoad.Points);
            Brep brp = Brep.CreatePlanarBreps(crv.ToPolylineCurve(), 0.000001)[0];
            target = (TQ)(object)new GH_Brep(brp);
            return true;
          }
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Integer(Value.CaseId);
          return true;
        }
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() {
      return new GsaLoadGoo(Value);
    }
  }
}
