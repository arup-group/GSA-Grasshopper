using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaCombinationCaseGoo"/> type.
  /// </summary>
  public class GsaCombinationCaseParameter : GH_OasysPersistentParam<GsaCombinationCaseGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaCombinationCaseGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaCombinationCaseGoo.Name : base.TypeName;

    public override Guid ComponentGuid => new Guid("f8d0651f-b235-473f-ba71-30c97b9497cd");

    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CombinationCaseParam;

    public GsaCombinationCaseParameter() : base(new GH_InstanceDescription(
     GsaCombinationCaseGoo.Name,
     GsaCombinationCaseGoo.NickName,
     GsaCombinationCaseGoo.Description + " parameter",
     GsaGH.Components.Ribbon.CategoryName.Name(),
     GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }
  }
}
