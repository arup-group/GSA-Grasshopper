using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaElement3dGoo"/> type.
  /// </summary>
  public class GsaElement3dParameter : GH_OasysPersistentGeometryParam<GsaElement3dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaElement3dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaElement3dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("e7326f8e-c8e5-40d9-b8e4-6912ccf80b92");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Elem3dParam;

    public GsaElement3dParameter() : base(new GH_InstanceDescription(
      GsaElement3dGoo.Name,
      GsaElement3dGoo.NickName,
      GsaElement3dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GsaElement3dGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaElement3d))
        return new GsaElement3dGoo((GsaElement3d)data);

      return base.PreferredCast(data);
    }
  }
}
