using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Helpers.Graphics;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaModalDynamicAnalysisGoo" /> type.
  /// </summary>
  public class ModalDynamicAnalysisParameter : GH_OasysPersistentParam<GsaModalDynamicAnalysisGoo> {
    public override Guid ComponentGuid => new Guid("6b99a192-bdbd-41bf-8efa-1bc146d3d224");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override string InstanceDescription => GsaAnalysisCaseGoo.Name + " parameter";
    public override string TypeName => GsaAnalysisCaseGoo.Name;
    protected override Bitmap Icon => Resources.AnalysisCaseParam;

    public ModalDynamicAnalysisParameter() : base(new GH_InstanceDescription(
      GsaAnalysisCaseGoo.Name, GsaAnalysisCaseGoo.NickName,
      GsaAnalysisCaseGoo.Description + " parameter",
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    protected override GsaModalDynamicAnalysisGoo PreferredCast(object data) {
      switch (data) {
        case GsaAnalysisTaskGoo analysisTask:
          return new GsaModalDynamicAnalysisGoo(new GsaModalDynamicAnalysis(analysisTask.Value));
        case GsaModalDynamicAnalysisGoo dynamicAnalysis:
          return new GsaModalDynamicAnalysisGoo(dynamicAnalysis.Value);
        default:
          this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to ModalDynamicAnalysis parameter");
          return new GsaModalDynamicAnalysisGoo(null);
      }
    }
  }
}
