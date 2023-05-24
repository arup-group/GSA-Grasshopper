using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.Geometry;
using System;
using System.Drawing;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaGridPlaneSurfaceGoo" /> type.
  /// </summary>
  public class GsaGridPlaneSurfaceParameter : GH_OasysPersistentGeometryParam<GsaGridPlaneSurfaceGoo> {
    public override Guid ComponentGuid => new Guid("161e2439-83b6-4fda-abb9-2ed938612530");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaGridPlaneSurfaceGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaGridPlaneSurfaceGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.GridPlaneParam;

    public GsaGridPlaneSurfaceParameter() : base(new GH_InstanceDescription(GsaGridPlaneSurfaceGoo.Name,
      GsaGridPlaneSurfaceGoo.NickName, GsaGridPlaneSurfaceGoo.Description + " parameter",
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    protected override GsaGridPlaneSurfaceGoo PreferredCast(object data) {
      var pln = new Plane();
      if (GH_Convert.ToPlane(data, ref pln, GH_Conversion.Both)) {
        return new GsaGridPlaneSurfaceGoo(new GsaGridPlaneSurface(pln));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to GridPlaneSurface");
      return new GsaGridPlaneSurfaceGoo(null);
    }
  }
}
