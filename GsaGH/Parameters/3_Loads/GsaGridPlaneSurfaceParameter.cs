using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaGridPlaneSurfaceGoo" /> type.
  /// </summary>
  public class
    GsaGridPlaneSurfaceParameter : GH_OasysPersistentGeometryParam<GsaGridPlaneSurfaceGoo> {
    public override Guid ComponentGuid => new Guid("161e2439-83b6-4fda-abb9-2ed938612530");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaGridPlaneSurfaceGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaGridPlaneSurfaceGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.GridPlaneParam;

    public GsaGridPlaneSurfaceParameter() : base(new GH_InstanceDescription(
      GsaGridPlaneSurfaceGoo.Name, GsaGridPlaneSurfaceGoo.NickName,
      GsaGridPlaneSurfaceGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaGridPlaneSurfaceGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaLoadGoo)) {
        var loadGoo = (GsaLoadGoo)data;
        if (loadGoo.Value != null) {
          switch (loadGoo.Value.LoadType) {
            case GsaLoad.LoadTypes.GridPoint:
              return new GsaGridPlaneSurfaceGoo(loadGoo.Value.PointLoad.GridPlaneSurface);

            case GsaLoad.LoadTypes.GridLine:
              return new GsaGridPlaneSurfaceGoo(loadGoo.Value.LineLoad.GridPlaneSurface);

            case GsaLoad.LoadTypes.GridArea:
              return new GsaGridPlaneSurfaceGoo(loadGoo.Value.AreaLoad.GridPlaneSurface);

            default:
              this.AddRuntimeError(
                $"Load is {loadGoo.Value.LoadType} but must be a GridLoad to convert to GridPlaneSurface");
              return new GsaGridPlaneSurfaceGoo(null);
          }
        }
      }

      var pln = new Plane();
      if (GH_Convert.ToPlane(data, ref pln, GH_Conversion.Both)) {
        return new GsaGridPlaneSurfaceGoo(new GsaGridPlaneSurface(pln));
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        return new GsaGridPlaneSurfaceGoo(new GsaGridPlaneSurface {
          GridSurface = {
            GridPlane = id,
          },
          GridPlane = null,
        });
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to GridPlaneSurface");
      return new GsaGridPlaneSurfaceGoo(null);
    }
  }
}
