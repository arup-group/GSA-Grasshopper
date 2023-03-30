using System.Collections.Generic;
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

    #region Properties + Fields
    public static string Description => "GSA Load";
    public static string Name => "Load";
    public static string NickName => "Ld";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaLoadGoo(GsaLoad item) : base(item) {
    }

    #endregion Public Constructors

    #region Public Methods
    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo<TQ>(ref target))
        return true;

      if (typeof(TQ).IsAssignableFrom(typeof(GsaGridPlaneSurfaceGoo))) {
        if (Value == null)
          target = default;
        else {
          if (Value.AreaLoad != null) {
            GsaGridPlaneSurface gridplane = Value.AreaLoad.GridPlaneSurface;
            var gpgoo = new GsaGridPlaneSurfaceGoo(gridplane);
            target = (TQ)(object)gpgoo;
            return true;
          }

          if (Value.LineLoad != null) {
            GsaGridPlaneSurface gridplane = Value.LineLoad.GridPlaneSurface;
            var gpgoo = new GsaGridPlaneSurfaceGoo(gridplane);
            target = (TQ)(object)gpgoo;
            return true;
          }

          if (Value.PointLoad == null)
            return true;
          {
            GsaGridPlaneSurface gridplane = Value.PointLoad.GridPlaneSurface;
            var gpgoo = new GsaGridPlaneSurfaceGoo(gridplane);
            target = (TQ)(object)gpgoo;
            return true;
          }
        }

        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Plane))) {
        if (Value == null)
          target = default;
        else {
          switch (Value.LoadType) {
            case GsaLoad.LoadTypes.GridArea: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(Value.AreaLoad.GridPlaneSurface.Plane,
                  GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
            case GsaLoad.LoadTypes.GridLine: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(Value.LineLoad.GridPlaneSurface.Plane,
                  GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
            case GsaLoad.LoadTypes.GridPoint: {
                var ghpln = new GH_Plane();
                GH_Convert.ToGHPlane(Value.PointLoad.GridPlaneSurface.Plane,
                  GH_Conversion.Both,
                  ref ghpln);
                target = (TQ)(object)ghpln;
                return true;
              }
          }
        }

        return false;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Point))) {
        if (Value == null)
          target = default;
        else {
          if (Value.LoadType != GsaLoad.LoadTypes.GridPoint)
            return false;
          var point = new Point3d {
            X = Value.PointLoad.GridPointLoad.X,
            Y = Value.PointLoad.GridPointLoad.Y,
            Z = Value.PointLoad.GridPlaneSurface.Plane.OriginZ,
          };
          var ghpt = new GH_Point();
          GH_Convert.ToGHPoint(point, GH_Conversion.Both, ref ghpt);
          target = (TQ)(object)ghpt;
          return true;
        }

        return false;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
        if (Value == null)
          target = default;
        else {
          if (Value.LoadType != GsaLoad.LoadTypes.GridLine)
            return false;
          var pts = new List<Point3d>();
          string def = Value.LineLoad.GridLineLoad.PolyLineDefinition; //implement converter
          // to be done
          //target = (Q)(object)ghpt;
          //return true;
        }

        return false;
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() => new GsaLoadGoo(Value);

    #endregion Public Methods
  }
}
