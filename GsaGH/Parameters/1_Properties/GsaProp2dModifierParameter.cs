using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaProp2dModifierGoo" /> type.
  /// </summary>
  public class GsaProp2dModifierParameter : GH_OasysPersistentParam<GsaProp2dModifierGoo> {
    public override Guid ComponentGuid => new Guid("d3a428ed-0d1a-4577-843b-4439738a306a");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaProp2dModifierGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaProp2dModifierGoo.Name : base.TypeName;
    // todo: create icons!
    protected override Bitmap Icon => Resources.Prop2dModifierParam;

    public GsaProp2dModifierParameter() : base(new GH_InstanceDescription(
      GsaProp2dModifierGoo.Name,  GsaProp2dModifierGoo.NickName,
      GsaProp2dModifierGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaProp2dModifierGoo PreferredCast(object data) {
      if (data is GsaProp2dGoo goo) {
        Prop2DModifier apiModifier = goo.Value.ApiProp2d.PropertyModifier;
        var modifier = new GsaProp2dModifier(apiModifier);
        return new GsaProp2dModifierGoo(modifier);
      } else if (data is GsaProp2dModifier modifier) {
        return new GsaProp2dModifierGoo(modifier);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Prop2dModifier");
      return new GsaProp2dModifierGoo(null);
    }
  }
}
