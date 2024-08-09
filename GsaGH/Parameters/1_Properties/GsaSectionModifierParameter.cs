using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaSectionModifierGoo" /> type.
  /// </summary>
  public class GsaSectionModifierParameter : GH_OasysPersistentParam<GsaSectionModifierGoo> {
    public override Guid ComponentGuid => new Guid("19b3bec4-e021-493e-a847-cd30476b5322");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaSectionModifierGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaSectionModifierGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.SectionModifierParam;

    public GsaSectionModifierParameter() : base(new GH_InstanceDescription(
      GsaSectionModifierGoo.Name, GsaSectionModifierGoo.NickName,
      GsaSectionModifierGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaSectionModifierGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaSectionGoo)) {
        GsaSection section = ((GsaSectionGoo)data).Value;
        var modifier = new GsaSectionModifier();
        if (section.Modifier != null) {
          modifier = section.Modifier;
        }

        return new GsaSectionModifierGoo(modifier);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to SectionModifier");
      return new GsaSectionModifierGoo(null);
    }
  }
}
