using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaElement2dGoo" /> type.
  /// </summary>
  public class GsaElement2dParameter : GH_OasysPersistentGeometryParam<GsaElement2dGoo> {
    public override Guid ComponentGuid => new Guid("bfaa6912-77b0-40b1-aa78-54e2b28614d0");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaElement2dGoo.Name + " parameter"
        : base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0
        ? GsaElement2dGoo.Name
        : base.TypeName;
    protected override Bitmap Icon => Resources.Elem2dParam;

    public GsaElement2dParameter() : base(new GH_InstanceDescription(GsaElement2dGoo.Name,
                          GsaElement2dGoo.NickName,
      GsaElement2dGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaElement2dGoo PreferredCast(object data) {
      return data.GetType() == typeof(GsaElement2d) ? new GsaElement2dGoo((GsaElement2d)data) : base.PreferredCast(data);
    }
  }
}
