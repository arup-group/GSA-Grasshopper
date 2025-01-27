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
  ///   This class provides a parameter interface for the <see cref="GsaModalDynamicGoo" /> type.
  /// </summary>
  public class GsaModalDynamicParameter : GH_OasysPersistentParam<GsaModalDynamicGoo> {
    public override Guid ComponentGuid => new Guid("6b99a192-bdbd-41bf-8efa-1bc146d3d224");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override string InstanceDescription => GsaModalDynamicGoo.Name;
    public override string TypeName => GsaModalDynamicGoo.Name;
    protected override Bitmap Icon => Resources.ModalDynamicParam;

    public GsaModalDynamicParameter() : base(new GH_InstanceDescription(
      GsaModalDynamicGoo.Name, GsaModalDynamicGoo.NickName,
      GsaModalDynamicGoo.Description,
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    protected override GsaModalDynamicGoo PreferredCast(object data) {
      switch (data) {
        case GsaAnalysisTaskGoo analysisTask:
          return new GsaModalDynamicGoo(new GsaModalDynamic(analysisTask.Value.ApiTask));
        case GsaModalDynamicGoo dynamicAnalysis:
          return new GsaModalDynamicGoo(dynamicAnalysis.Value);
        default:
          this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to ModalDynamicAnalysis parameter");
          return new GsaModalDynamicGoo(null);
      }
    }
  }
}
