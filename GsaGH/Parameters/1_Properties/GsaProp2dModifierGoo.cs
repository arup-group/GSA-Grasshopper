using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaProp2dModifier" /> can be used in Grasshopper.
  /// </summary>
  public class GsaProp2dModifierGoo : GH_OasysGoo<GsaProp2dModifier> {
    public static string Description => 
      "A 2D Property Modifier is part of a 2D Property and can be used \n" +
      "to modify property's analytical properties without changing the \n" +
      "thickness. By default the 2D Property Modifier is unmodified.";
    public static string Name => "Property 2D Modifier";
    public static string NickName => "P2M";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp2dModifierGoo(GsaProp2dModifier item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaProp2dModifierGoo(Value);
    }
  }
}
