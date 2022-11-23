using System;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaGridPlaneSurfaceGoo"/> type.
  /// </summary>
  public class GsaGridPlaneParameter : GH_OasysPersistentGeometryParam<GsaGridPlaneSurfaceGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaGridPlaneSurfaceGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaGridPlaneSurfaceGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("161e2439-83b6-4fda-abb9-2ed938612530");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GridPlaneParam;

    public GsaGridPlaneParameter() : base(new GH_InstanceDescription(
      GsaGridPlaneSurfaceGoo.Name,
      GsaGridPlaneSurfaceGoo.NickName,
      GsaGridPlaneSurfaceGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GsaGridPlaneSurfaceGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaGridPlaneSurface))
        return new GsaGridPlaneSurfaceGoo((GsaGridPlaneSurface)data);

      return base.PreferredCast(data);
    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args)
    {
      //Meshes aren't drawn.
    }
  }
}
