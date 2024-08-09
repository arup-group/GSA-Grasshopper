using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaProperty2dModifierGoo" /> type.
  /// </summary>
  public class GsaProperty2dModifierParameter : GH_OasysPersistentParam<GsaProperty2dModifierGoo> {
    public override Guid ComponentGuid => new Guid("d3a428ed-0d1a-4577-843b-4439738a306a");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaProperty2dModifierGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaProperty2dModifierGoo.Name : base.TypeName;
    // todo: create icons!
    protected override Bitmap Icon => Resources.Property2dModifierParam;

    public GsaProperty2dModifierParameter() : base(new GH_InstanceDescription(
      GsaProperty2dModifierGoo.Name, GsaProperty2dModifierGoo.NickName,
      GsaProperty2dModifierGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaProperty2dModifierGoo PreferredCast(object data) {
      if (data is GsaProperty2dGoo goo && goo.Value?.ApiProp2d != null) {
        return new GsaProperty2dModifierGoo(
          new GsaProperty2dModifier(goo.Value.ApiProp2d.PropertyModifier));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Prop2dModifier");
      return new GsaProperty2dModifierGoo(null);
    }
  }
}
