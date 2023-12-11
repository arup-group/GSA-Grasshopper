using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaEffectiveLength" /> can be used in Grasshopper.
  /// </summary>
  public class GsaEffectiveLengthGoo : GH_OasysGoo<GsaEffectiveLength> {
    public static string Description => "GSA 1D Member Design Properties for Effective Length, " +
      "Restraints and Buckling Factors";
    public static string Name => "Effective Length";
    public static string NickName => "Le";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaEffectiveLengthGoo(GsaEffectiveLength item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaEffectiveLengthGoo(Value);
    }
  }
}
