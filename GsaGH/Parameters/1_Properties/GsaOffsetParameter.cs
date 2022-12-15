using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
    /// <summary>
    /// This class provides a parameter interface for the <see cref="GsaOffsetGoo"/> type.
    /// </summary>
    public class GsaOffsetParameter : GH_OasysPersistentParam<GsaOffsetGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaOffsetGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaOffsetGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("0b14f16e-bd6a-4da7-991a-359f64aa28fd");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.OffsetParam;
    
    public GsaOffsetParameter() : base(new GH_InstanceDescription(
      GsaOffsetGoo.Name,
      GsaOffsetGoo.NickName,
      GsaOffsetGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaOffsetGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaOffset))
        return new GsaOffsetGoo((GsaOffset)data);

      return base.PreferredCast(data);
    }
  }
}
