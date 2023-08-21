using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="IGsaLoad" /> can be used in Grasshopper.
  /// </summary>
  public class GsaLoadGoo : GH_OasysGoo<IGsaLoad> {
    public static string Description =>
      "GSA provides a number of different ways to apply loads to a model. \n" +
      "The simplest option is nodal loading where forces are applied \n" +
      "directly to nodes. This is not recommended for 2D and 3D elements. \n" +
      "The next level of loading applies loads to the elements, either beam \n" +
      "loading, 2D element loading or 3D element loading. In the solver these \n" +
      "use shape functions to give loading on the nodes compatible with the \n" +
      "elements to which the load is applied. Grid loading is a different type \n" +
      "of loading which is applied to a grid surface. An algorithm then \n" +
      "distributes this loading from the grid surface to the surrounding beam \n" +
      "elements. This is useful for models where floor slabs are not modelled \n" +
      "explicitly.\n" +
      "Gravity is the final load type. This is different from the other load \n" +
      "types as it is specified as an acceleration (in g). This is normally \n" +
      "used to model the dead weight of the structure by specifying a gravity \n" +
      "load of −1 × g in the z direction.";
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
      string type = Value.LoadType.ToString().Trim();
      string name = Value.Name.Trim();
      string value = string.Join(" ", caseid, type, name).Trim().Replace("  ", " ");
      return PluginInfo.ProductName + " " + TypeName + " (" + value + ")";
    }
  }
}
