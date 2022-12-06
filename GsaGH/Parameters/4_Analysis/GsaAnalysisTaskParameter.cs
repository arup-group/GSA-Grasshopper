using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
    /// <summary>
    /// This class provides a parameter interface for the <see cref="GsaAnalysisTaskGoo"/> type.
    /// </summary>
    public class GsaAnalysisTaskParameter : GH_OasysPersistentParam<GsaAnalysisTaskGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaAnalysisTaskGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaAnalysisTaskGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("51048d67-3652-45d0-9eec-0f9ef339c1a5");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AnalysisTaskParam;

    public GsaAnalysisTaskParameter() : base(new GH_InstanceDescription(
      GsaAnalysisTaskGoo.Name,
      GsaAnalysisTaskGoo.NickName,
      GsaAnalysisTaskGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaAnalysisTaskGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaAnalysisTask))
        return new GsaAnalysisTaskGoo((GsaAnalysisTask)data);

      return base.PreferredCast(data);
    }
  }
}
