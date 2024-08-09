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
    public override Guid ComponentGuid => new Guid("6b99a192-bdbd-41bf-8efa-1bc146d3c224");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaAnalysisCaseGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaAnalysisCaseGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.AnalysisCaseParam;

    public GsaAnalysisCaseParameter() : base(new GH_InstanceDescription(
      GsaAnalysisCaseGoo.Name, GsaAnalysisCaseGoo.NickName,
      GsaAnalysisCaseGoo.Description + " parameter",
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    protected override GsaAnalysisCaseGoo PreferredCast(object data) {
      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        return new GsaAnalysisCaseGoo(
          new GsaAnalysisCase(id, "Analysis Case " + id, string.Empty));
      }

      if (GH_Convert.ToString(data, out string name, GH_Conversion.Both)) {
        return new GsaAnalysisCaseGoo(new GsaAnalysisCase(0, "Analysis Case", name));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to AnalysisCase");
      return new GsaAnalysisCaseGoo(null);
    }
  }
}
