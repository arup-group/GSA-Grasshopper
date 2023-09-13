using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="IGsaLoad" /> can be used in Grasshopper.
  /// </summary>
  public class GsaLoadGoo : GH_OasysGoo<IGsaLoad> {
    public static string Description => "GSA Load";
    public static string Name => "Load";
    public static string NickName => "Ld";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaLoadGoo(IGsaLoad item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Plane))) {
        if (Value != null) {
          switch (Value.LoadType) {
            case LoadType.GridArea: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(((GsaGridAreaLoad)Value).GridPlaneSurface.Plane, GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
            case LoadType.GridLine: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(((GsaGridLineLoad)Value).GridPlaneSurface.Plane, GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
            case LoadType.GridPoint: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(((GsaGridPointLoad)Value).GridPlaneSurface.Plane, GH_Conversion.Both,
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
          if (Value.LoadType != LoadType.GridPoint) {
            return false;
          }

          var point = new Point3d {
            X = ((GsaGridPointLoad)Value).GridPointLoad.X,
            Y = ((GsaGridPointLoad)Value).GridPointLoad.Y,
            Z = ((GsaGridPointLoad)Value).GridPlaneSurface.Plane.OriginZ,
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
          if (Value.LoadType == LoadType.GridLine && ((GsaGridLineLoad)Value).Points.Count > 0) {
            var crv = new Polyline(((GsaGridLineLoad)Value).Points);
            target = (TQ)(object)new GH_Curve(crv.ToPolylineCurve());
            return true;
          } else if (Value.LoadType == LoadType.GridArea
            && ((GsaGridAreaLoad)Value).Points.Count > 0) {
            var crv = new Polyline(((GsaGridAreaLoad)Value).Points);
            target = (TQ)(object)new GH_Curve(crv.ToPolylineCurve());
            return true;
          }
        }
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Surface))) {
        if (Value == null) {
          target = default;
        } else {
          if (Value.LoadType == LoadType.GridArea) {
            var crv = new Polyline(((GsaGridAreaLoad)Value).Points);
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
          if (Value.LoadType == LoadType.GridArea) {
            var crv = new Polyline(((GsaGridAreaLoad)Value).Points);
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

    public override string ToString() {
      if (Value == null) {
        return "Null";
      }

      string caseid = $"LC{Value.CaseId}";
      string type = Value.LoadType.ToString();
      string name = Value.Name;
      string value = string.Join(" ", caseid, type, name).TrimSpaces();
      return $"{PluginInfo.ProductName} {TypeName} ({value})";
    }
  }
}
