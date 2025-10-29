using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;


namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaModalDynamic" /> can be used in Grasshopper.
  /// </summary>
  public class GsaModalDynamicGoo : GH_OasysGoo<GsaModalDynamic> {
    public static string Description => "GSA modal dynamic analysis parameters";
    public static string Name => "Modal Dynamic Parameter";
    public static string NickName => "Mdp";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaModalDynamicGoo(GsaModalDynamic item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaModalDynamicGoo(Value);
    }
  }
}
