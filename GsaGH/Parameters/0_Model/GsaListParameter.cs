using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaListGoo"/> type.
  /// </summary>
  public class GsaListParameter : GH_OasysPersistentParam<GsaListGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaListGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaListGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("ce4c0131-22bd-4046-8b69-c42832cf7a53");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ListParam;

    public GsaListParameter() : base(new GH_InstanceDescription(
      GsaListGoo.Name,
      GsaListGoo.NickName,
      GsaListGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaListGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaList))
        return new GsaListGoo((GsaList)data);
      return base.PreferredCast(data);
    }
  }
}
