using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaProp2dGoo"/> type.
  /// </summary>
  public class GsaProp2dParameter : GH_OasysPersistentParam<GsaProp2dGoo> {
    public override Guid ComponentGuid => new Guid("05a034ad-683d-479b-9768-5c04379c0606");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override string InstanceDescription => m_data.DataCount == 0
              ? "Empty " + GsaProp2dGoo.Name + " parameter"
      : base.InstanceDescription;
    public override string TypeName => SourceCount == 0
      ? GsaProp2dGoo.Name
      : base.TypeName;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Prop2dParam;

    public GsaProp2dParameter() : base(new GH_InstanceDescription(
          GsaProp2dGoo.Name,
      GsaProp2dGoo.NickName,
      GsaProp2dGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaProp2dGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaProp2d)) {
        return new GsaProp2dGoo((GsaProp2d)data);
      }

      if (!GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        return base.PreferredCast(data);
      }

      var prop = new GsaProp2d(id);
      return new GsaProp2dGoo(prop);
    }
  }
}
