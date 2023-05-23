using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaProp3dGoo" /> type.
  /// </summary>
  public class GsaProp3dParameter : GH_OasysPersistentParam<GsaProp3dGoo> {
    public override Guid ComponentGuid => new Guid("277c96bb-8ea4-4d95-ab02-2954f14203f3");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaProp3dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaProp3dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Prop3dParam;

    public GsaProp3dParameter() : base(new GH_InstanceDescription(GsaProp3dGoo.Name,
      GsaProp3dGoo.NickName, GsaProp3dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaProp3dGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaProp3d)) {
        return new GsaProp3dGoo((GsaProp3d)data);
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        var prop = new GsaProp3d(id);
        return new GsaProp3dGoo(prop);
      }

      if (data.GetType() != typeof(GsaMaterialGoo)) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
        $"Data conversion failed from {data.GetTypeName()} to Prop3d");
        return new GsaProp3dGoo(null);
      }

      {
        var prop = new GsaProp3d(((GsaMaterialGoo)data).Value);
        return new GsaProp3dGoo(prop);
      }
    }
  }
}
