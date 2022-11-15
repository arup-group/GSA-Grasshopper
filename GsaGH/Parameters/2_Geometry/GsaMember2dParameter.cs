using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaMember2dGoo"/> type.
  /// </summary>
  public class GsaMember2dParameter : GH_OasysPersistentGeometryParam<GsaMember2dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaMember2dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaMember2dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("fa512c2d-4767-49f1-a574-32bf66a66568");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Mem2dParam;

    public GsaMember2dParameter() : base(new GH_InstanceDescription(
      GsaMember2dGoo.Name,
      GsaMember2dGoo.NickName,
      GsaMember2dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GsaMember2dGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaMember2d))
        return new GsaMember2dGoo((GsaMember2d)data);

      return base.PreferredCast(data);
    }
  }
}
