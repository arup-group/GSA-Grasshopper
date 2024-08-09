using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaAnalysisTaskGoo" /> type.
  /// </summary>
  public class GsaAnalysisTaskParameter : GH_OasysPersistentParam<GsaAnalysisTaskGoo> {
    public override Guid ComponentGuid => new Guid("51048d67-3652-45d0-9eec-0f9ef339c1a5");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaAnalysisTaskGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaAnalysisTaskGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.AnalysisTaskParam;

    public GsaAnalysisTaskParameter() : base(new GH_InstanceDescription(
      GsaAnalysisTaskGoo.Name, GsaAnalysisTaskGoo.NickName,
      GsaAnalysisTaskGoo.Description + " parameter",
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    protected override GsaAnalysisTaskGoo PreferredCast(object data) {
      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to AnalysisTask");
      return new GsaAnalysisTaskGoo(null);
    }
  }
}
