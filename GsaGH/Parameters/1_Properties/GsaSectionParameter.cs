using System;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaSectionGoo"/> type.
  /// </summary>
  public class GsaSectionParameter : GH_OasysPersistentParam<GsaSectionGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaSectionGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaSectionGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("8500f335-fad7-46a0-b1be-bdad22ab1474");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SectionParam;

    public GsaSectionParameter() : base(new GH_InstanceDescription(
      GsaSectionGoo.Name,
      GsaSectionGoo.NickName,
      GsaSectionGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }
  }
}
