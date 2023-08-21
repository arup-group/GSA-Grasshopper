using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaSectionModifier" /> can be used in Grasshopper.
  /// </summary>
  public class GsaSectionModifierGoo : GH_OasysGoo<GsaSectionModifier> {
    public static string Description => 
      "A Section Modifier is part of a Section and can be used to \n" +
      "modify section's analytical properties without changing the \n" +
      "profile. By default the Section Modifier is unmodified.";
    public static string Name => "Section Modifier";
    public static string NickName => "PBM";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaSectionModifierGoo(GsaSectionModifier item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaSectionModifierGoo(Value);
    }
  }
}
