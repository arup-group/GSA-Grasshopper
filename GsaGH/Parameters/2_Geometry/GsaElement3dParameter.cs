using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaElement3dGoo" /> type.
  /// </summary>
  public class GsaElement3dParameter : GH_OasysPersistentGeometryParam<GsaElement3dGoo> {
    public override Guid ComponentGuid => new Guid("e7326f8e-c8e5-40d9-b8e4-6912ccf80b92");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaElement3dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaElement3dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Elem3dParam;

    public GsaElement3dParameter() : base(new GH_InstanceDescription(GsaElement3dGoo.Name,
      GsaElement3dGoo.NickName, GsaElement3dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaElement3dGoo PreferredCast(object data) {
      return data.GetType() == typeof(GsaElement3d) ? new GsaElement3dGoo((GsaElement3d)data) :
        base.PreferredCast(data);
    }
  }
}
