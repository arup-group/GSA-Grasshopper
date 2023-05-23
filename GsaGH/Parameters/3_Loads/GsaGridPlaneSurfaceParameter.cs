using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using Newtonsoft.Json.Linq;
using OasysGH.Parameters;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaGridPlaneSurfaceGoo" /> type.
  /// </summary>
  public class GsaGridPlaneParameter : GH_OasysPersistentGeometryParam<GsaGridPlaneSurfaceGoo> {
    public override Guid ComponentGuid => new Guid("161e2439-83b6-4fda-abb9-2ed938612530");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaGridPlaneSurfaceGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaGridPlaneSurfaceGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.GridPlaneParam;

    public GsaGridPlaneParameter() : base(new GH_InstanceDescription(GsaGridPlaneSurfaceGoo.Name,
      GsaGridPlaneSurfaceGoo.NickName, GsaGridPlaneSurfaceGoo.Description + " parameter",
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) { }

    protected override GsaGridPlaneSurfaceGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaGridPlaneSurface)) {
        return new GsaGridPlaneSurfaceGoo((GsaGridPlaneSurface)data);
      }

      var pln = new Plane();
      if (GH_Convert.ToPlane(data, ref pln, GH_Conversion.Both)) {
        return new GsaGridPlaneSurfaceGoo(new GsaGridPlaneSurface(pln));
      }

      AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
        $"Data conversion failed from {data.GetTypeName()} to GridPlaneSurface");
      return new GsaGridPlaneSurfaceGoo(null);
    }
  }
}
