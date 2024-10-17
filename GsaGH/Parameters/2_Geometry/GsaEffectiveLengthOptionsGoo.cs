using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaEffectiveLengthOptions" /> can be used in Grasshopper.
  /// </summary>
  public class GsaEffectiveLengthOptionsGoo : GH_OasysGoo<GsaEffectiveLengthOptions> {
    public static string Description => "GSA 1D Member Design Options for Effective Length, " +
      "Restraints and Buckling Factors";
    public static string Name => "Effective Length Options";
    public static string NickName => "Le";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaEffectiveLengthOptionsGoo(GsaEffectiveLengthOptions item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaEffectiveLengthOptionsGoo(Value);
    }
  }
}
