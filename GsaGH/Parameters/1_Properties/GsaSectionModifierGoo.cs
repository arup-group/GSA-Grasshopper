using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaSectionModifier" /> can be used in Grasshopper.
  /// </summary>
  public class GsaSectionModifierGoo : GH_OasysGoo<GsaSectionModifier> {
    public static string Description => "GSA Section Modifier";
    public static string Name => "Section Modifier";
    public static string NickName => "PBM";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaSectionModifierGoo(GsaSectionModifier item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaSectionModifierGoo(Value);
    }
  }
}
