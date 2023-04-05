using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaGridPlaneSurfaceGoo" /> type.
  /// </summary>
  public class GsaGridPlaneParameter : GH_OasysPersistentGeometryParam<GsaGridPlaneSurfaceGoo> {
    public GsaGridPlaneParameter() : base(new GH_InstanceDescription(GsaGridPlaneSurfaceGoo.Name,
      GsaGridPlaneSurfaceGoo.NickName,
      GsaGridPlaneSurfaceGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaGridPlaneSurfaceGoo.Name + " parameter"
        : base.InstanceDescription;

    public override string TypeName
      => SourceCount == 0
        ? GsaGridPlaneSurfaceGoo.Name
        : base.TypeName;

    public override Guid ComponentGuid => new Guid("161e2439-83b6-4fda-abb9-2ed938612530");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    protected override Bitmap Icon => Resources.GridPlaneParam;

    protected override GsaGridPlaneSurfaceGoo PreferredCast(object data) => data.GetType() == typeof(GsaGridPlaneSurface)
        ? new GsaGridPlaneSurfaceGoo((GsaGridPlaneSurface)data)
        : base.PreferredCast(data);

    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
    }
  }
}
