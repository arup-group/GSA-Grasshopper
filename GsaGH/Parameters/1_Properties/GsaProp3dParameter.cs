using System;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaProp3dGoo"/> type.
  /// </summary>
  public class GsaProp3dParameter : GH_OasysPersistentParam<GsaProp3dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaProp3dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaProp3dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("277c96bb-8ea4-4d95-ab02-2954f14203f3");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Prop3dParam;

    public GsaProp3dParameter() : base(new GH_InstanceDescription(
      GsaProp3dGoo.Name,
      GsaProp3dGoo.NickName,
      GsaProp3dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GsaProp3dGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaProp3d))
        return new GsaProp3dGoo((GsaProp3d)data);

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both))
      {
        GsaProp3d prop = new GsaProp3d(id);
        return new GsaProp3dGoo(prop);
      }
      return base.PreferredCast(data);
    }
  }
}
