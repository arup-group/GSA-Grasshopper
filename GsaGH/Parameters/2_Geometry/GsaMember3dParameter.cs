using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaMember3dGoo"/> type.
  /// </summary>
  public class GsaMember3dParameter : GH_OasysPersistentGeometryParam<GsaMember3dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaMember3dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaMember3dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("7608a5a0-7762-4214-8c30-fb395365056e");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Mem3dParam;

    public GsaMember3dParameter() : base(new GH_InstanceDescription(
      GsaMember3dGoo.Name,
      GsaMember3dGoo.NickName,
      GsaMember3dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }
  }
}
