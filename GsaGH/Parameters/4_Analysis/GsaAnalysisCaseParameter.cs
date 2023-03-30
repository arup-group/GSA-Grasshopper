using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaAnalysisCaseGoo" /> type.
  /// </summary>
  public class GsaAnalysisCaseParameter : GH_OasysPersistentParam<GsaAnalysisCaseGoo> {
    public GsaAnalysisCaseParameter() : base(new GH_InstanceDescription(GsaAnalysisCaseGoo.Name,
      GsaAnalysisCaseGoo.NickName,
      GsaAnalysisCaseGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaAnalysisCaseGoo.Name + " parameter"
        : base.InstanceDescription;

    public override string TypeName
      => SourceCount == 0
        ? GsaAnalysisCaseGoo.Name
        : base.TypeName;

    public override Guid ComponentGuid => new Guid("6b99a192-bdbd-41bf-8efa-1bc146d3c224");

    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

    protected override Bitmap Icon => Resources.AnalysisCaseParam;

    protected override GsaAnalysisCaseGoo PreferredCast(object data) => data.GetType() == typeof(GsaAnalysisCase)
      ? new GsaAnalysisCaseGoo((GsaAnalysisCase)data)
      : base.PreferredCast(data);
  }
}
