using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMember2dGoo" /> type.
  /// </summary>
  public class GsaMember2dParameter : GH_OasysPersistentGeometryParam<GsaMember2dGoo> {
    public GsaMember2dParameter() : base(new GH_InstanceDescription(GsaMember2dGoo.Name,
      GsaMember2dGoo.NickName,
      GsaMember2dGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaMember2dGoo.Name + " parameter"
        : base.InstanceDescription;

    public override string TypeName
      => SourceCount == 0
        ? GsaMember2dGoo.Name
        : base.TypeName;

    public override Guid ComponentGuid => new Guid("fa512c2d-4767-49f1-a574-32bf66a66568");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override Bitmap Icon => Resources.Mem2dParam;

    protected override GsaMember2dGoo PreferredCast(object data)
      => data.GetType() == typeof(GsaMember2d)
        ? new GsaMember2dGoo((GsaMember2d)data)
        : base.PreferredCast(data);
  }
}
