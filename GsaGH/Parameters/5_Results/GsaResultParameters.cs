using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaResultGoo"/> type.
  /// </summary>
  public class GsaResultsParameter : GH_OasysPersistentParam<GsaResultGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaResultGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaResultGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("81f6f103-cb53-414c-908b-6adf46c3260d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ResultParam;

    public GsaResultsParameter() : base(new GH_InstanceDescription(
      GsaResultGoo.Name,
      GsaResultGoo.NickName,
      GsaResultGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GsaResultGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaResult))
        return new GsaResultGoo((GsaResult)data);

      return base.PreferredCast(data);
    }
  }
}
