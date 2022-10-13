using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaAnalysisCaseGoo"/> type.
  /// </summary>
  public class GsaAnalysisCaseParameter : GH_OasysPersistentParam<GsaAnalysisCaseGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaAnalysisCaseGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaAnalysisCaseGoo.Name : base.TypeName;

    public override Guid ComponentGuid => new Guid("6b99a192-bdbd-41bf-8efa-1bc146d3c224");

    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AnalysisCaseParam;

    public GsaAnalysisCaseParameter() : base(new GH_InstanceDescription(
     GsaAnalysisCaseGoo.Name,
     GsaAnalysisCaseGoo.NickName,
     GsaAnalysisCaseGoo.Description + " parameter",
     GsaGH.Components.Ribbon.CategoryName.Name(),
     GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }
  }
}
