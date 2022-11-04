using System;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaProp2dGoo"/> type.
  /// </summary>
  public class GsaProp2dParameter : GH_OasysPersistentParam<GsaProp2dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaProp2dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaProp2dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("05a034ad-683d-479b-9768-5c04379c0606");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Prop2dParam;

    public GsaProp2dParameter() : base(new GH_InstanceDescription(
      GsaProp2dGoo.Name,
      GsaProp2dGoo.NickName,
      GsaProp2dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GsaProp2dGoo PreferredCast(object data)
    {
      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both))
      {
        GsaProp2d prop = new GsaProp2d(id);
        return new GsaProp2dGoo(prop);
      }
      return base.PreferredCast(data);
    }
  }
}
