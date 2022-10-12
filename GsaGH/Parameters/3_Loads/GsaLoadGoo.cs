using System.Collections.Generic;
using System.ComponentModel;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaLoad"/> can be used in Grasshopper.
  /// </summary>
  public class GsaLoadGoo : GH_OasysGoo<GsaLoad>
  {
    public static string Name => "Load";
    public static string NickName => "Ld";
    public static string Description => "GSA Load";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaLoadGoo(GsaLoad item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaLoadGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaLoad)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GsaGridPlaneSurfaceGoo)))
      {
        if (Value == null)
          target = default;
        else
        {
          if (Value.AreaLoad != null)
          {
            GsaGridPlaneSurface gridplane = Value.AreaLoad.GridPlaneSurface;
            GsaGridPlaneSurfaceGoo gpgoo = new GsaGridPlaneSurfaceGoo(gridplane);
            target = (Q)(object)gpgoo;
            return true;
          }
          if (Value.LineLoad != null)
          {
            GsaGridPlaneSurface gridplane = Value.LineLoad.GridPlaneSurface;
            GsaGridPlaneSurfaceGoo gpgoo = new GsaGridPlaneSurfaceGoo(gridplane);
            target = (Q)(object)gpgoo;
            return true;
          }
          if (Value.PointLoad != null)
          {
            GsaGridPlaneSurface gridplane = Value.PointLoad.GridPlaneSurface;
            GsaGridPlaneSurfaceGoo gpgoo = new GsaGridPlaneSurfaceGoo(gridplane);
            target = (Q)(object)gpgoo;
            return true;
          }
        }
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
      {
        if (Value == null)
          target = default;
        else
        {
          if (Value.LoadType == GsaLoad.LoadTypes.GridArea)
          {
            GH_Plane ghpln = new GH_Plane();
            GH_Convert.ToGHPlane(Value.AreaLoad.GridPlaneSurface.Plane, GH_Conversion.Both, ref ghpln);
            target = (Q)(object)ghpln;
            return true;
          }
          if (Value.LoadType == GsaLoad.LoadTypes.GridLine)
          {
            GH_Plane ghpln = new GH_Plane();
            GH_Convert.ToGHPlane(Value.LineLoad.GridPlaneSurface.Plane, GH_Conversion.Both, ref ghpln);
            target = (Q)(object)ghpln;
            return true;
          }
          if (Value.LoadType == GsaLoad.LoadTypes.GridPoint)
          {
            GH_Plane ghpln = new GH_Plane();
            GH_Convert.ToGHPlane(Value.PointLoad.GridPlaneSurface.Plane, GH_Conversion.Both, ref ghpln);
            target = (Q)(object)ghpln;
            return true;
          }
        }
        return false;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
      {
        if (Value == null)
          target = default;
        else
        {
          if (Value.LoadType == GsaLoad.LoadTypes.GridPoint)
          {
            Point3d point = new Point3d
            {
              X = Value.PointLoad.GridPointLoad.X,
              Y = Value.PointLoad.GridPointLoad.Y,
              Z = Value.PointLoad.GridPlaneSurface.Plane.OriginZ
            };
            GH_Point ghpt = new GH_Point();
            GH_Convert.ToGHPoint(point, GH_Conversion.Both, ref ghpt);
            target = (Q)(object)ghpt;
            return true;
          }
        }
        return false;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        if (Value == null)
          target = default;
        else
        {
          if (Value.LoadType == GsaLoad.LoadTypes.GridLine)
          {
            List<Point3d> pts = new List<Point3d>();
            string def = Value.LineLoad.GridLineLoad.PolyLineDefinition; //implement converter
                                                                         // to be done
                                                                         //target = (Q)(object)ghpt;
                                                                         //return true;
          }
        }
        return false;
      }

      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from GsaLoad
      if (typeof(GsaLoad).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaLoad)source;
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
