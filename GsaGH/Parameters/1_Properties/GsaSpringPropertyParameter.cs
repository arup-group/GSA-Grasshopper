using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaSpringPropertyGoo" /> type.
  /// </summary>
  public class GsaSpringPropertyParameter : GH_OasysPersistentParam<GsaSpringPropertyGoo> {
    public override Guid ComponentGuid => new Guid("3ac6c404-b3ac-4e31-875f-36e27dcf5cc7");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaSpringPropertyGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaSpringPropertyGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.SpringPropertyParam;

    public GsaSpringPropertyParameter() : base(new GH_InstanceDescription(GsaSpringPropertyGoo.Name,
      GsaSpringPropertyGoo.NickName, GsaSpringPropertyGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaSpringPropertyGoo PreferredCast(object data) {
      string mes = string.Empty;
      string defaultText = $"{data.GetTypeName()} does not contain a Spring Property";

      if (data is GsaPropertyGoo prop) {
        if (prop.Value is GsaSpringProperty spring) {
          return new GsaSpringPropertyGoo(spring);
        } else {
          this.AddRuntimeError($"Data conversion failed from Section to Spring" + mes);
          return new GsaSpringPropertyGoo(null);
        }
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        GH_Convert.ToString(data, out string val, GH_Conversion.Both);
        if (!val.Contains(".")) {
          return new GsaSpringPropertyGoo(new GsaSpringProperty(id));
        }
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Spring Property");
      return new GsaSpringPropertyGoo(null);
    }
  }
}
