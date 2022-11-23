using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
    /// <summary>
    /// This class provides a parameter interface for the <see cref="GsaLoadGoo"/> type.
    /// </summary>
    public class GsaLoadParameter : GH_OasysPersistentParam<GsaLoadGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaLoadGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaLoadGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("2833ef04-c595-4b05-8db3-622c75fa9a25");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.LoadParam;

    public GsaLoadParameter() : base(new GH_InstanceDescription(
      GsaLoadGoo.Name,
      GsaLoadGoo.NickName,
      GsaLoadGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaLoadGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaLoad))
        return new GsaLoadGoo((GsaLoad)data);

      return base.PreferredCast(data);
    }
  }
}
