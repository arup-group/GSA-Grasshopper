using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaProperty3dGoo" /> type.
  /// </summary>
  public class GsaProperty3dParameter : GH_OasysPersistentParam<GsaProperty3dGoo> {
    public override Guid ComponentGuid => new Guid("277c96bb-8ea4-4d95-ab02-2954f14203f3");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaProperty3dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaProperty3dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Property3dParam;

    public GsaProperty3dParameter() : base(new GH_InstanceDescription(GsaProperty3dGoo.Name,
      GsaProperty3dGoo.NickName, GsaProperty3dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaProperty3dGoo PreferredCast(object data) {
      string mes = string.Empty;
      string defaultText = $"{data.GetTypeName()} does not contain a 3D Property";
      switch (data) {
        case GsaElement3dGoo elem3d:
          if (elem3d.Value.Prop3ds.IsNullOrEmpty()) {
            mes = defaultText;
            break;
          }

          return new GsaProperty3dGoo(elem3d.Value.Prop3ds[0]);

        case GsaMember3dGoo mem3d:
          if (mem3d.Value.Prop3d == null) {
            mes = defaultText;
            break;
          }

          return new GsaProperty3dGoo(mem3d.Value.Prop3d);
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        var prop = new GsaProperty3d(id);
        return new GsaProperty3dGoo(prop);
      }

      if (data.GetType() == typeof(GsaMaterialGoo)) {
        var prop = new GsaProperty3d(((GsaMaterialGoo)data).Value);
        return new GsaProperty3dGoo(prop);
      }

      if (!string.IsNullOrEmpty(mes)) {
        mes = "." + Environment.NewLine + mes;
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Prop3d" + mes);
      return new GsaProperty3dGoo(null);
    }
  }
}
