using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaResult" /> can be used in Grasshopper.
  /// </summary>
  public class GsaResultGoo : GH_OasysGoo<GsaResult> {
    public GsaResultGoo(GsaResult item) : base(item) { }
    public static string Name => "Result";
    public static string NickName => "Res";
    public static string Description => "GSA Result";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public override IGH_Goo Duplicate() => this;
  }
}
