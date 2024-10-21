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
  public class GsaGridPlaneSurfaceParameter : GH_OasysPersistentGeometryParam<GsaGridPlaneSurfaceGoo> {
    public override Guid ComponentGuid => new Guid("161e2439-83b6-4fda-abb9-2ed938612530");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaGridPlaneSurfaceGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaGridPlaneSurfaceGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.GridPlaneSurfaceParam;

    public GsaGridPlaneSurfaceParameter() : base(new GH_InstanceDescription(
      GsaGridPlaneSurfaceGoo.Name, GsaGridPlaneSurfaceGoo.NickName,
      GsaGridPlaneSurfaceGoo.Description + " parameter",
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    protected override GsaGridPlaneSurfaceGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaLoadGoo)) {
        var loadGoo = (GsaLoadGoo)data;
        if (loadGoo.Value != null) {
          switch (loadGoo.Value) {
            case GsaGridPointLoad point:
              return new GsaGridPlaneSurfaceGoo(point.GridPlaneSurface);

            case GsaGridLineLoad line:
              return new GsaGridPlaneSurfaceGoo(line.GridPlaneSurface);

            case GsaGridAreaLoad area:
              return new GsaGridPlaneSurfaceGoo(area.GridPlaneSurface);

            default:
              this.AddRuntimeError(
                "Load is " + loadGoo.Value.GetType().ToString()
                .Replace("Gsa", string.Empty).Replace("Load", string.Empty) +
                $" but must be a GridLoad to convert to GridPlaneSurface");
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
