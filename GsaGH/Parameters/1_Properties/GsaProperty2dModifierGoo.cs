using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaProperty2dModifier" /> can be used in Grasshopper.
  /// </summary>
  public class GsaProperty2dModifierGoo : GH_OasysGoo<GsaProperty2dModifier> {
    public static string Description => "GSA Property 2D Modifier";
    public static string Name => "Property 2D Modifier";
    public static string NickName => "P2M";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProperty2dModifierGoo(GsaProperty2dModifier item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaProperty2dModifierGoo(Value);
    }
  }
}
