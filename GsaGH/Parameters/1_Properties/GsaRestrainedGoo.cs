using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaRestrainedGoo" /> can be used in Grasshopper.
  /// </summary>
  public class GsaRestrainedGoo : GH_OasysGoo<GsaBool6> {
    public static string Description =>
      "GSA Bool6 containing six booleans representing a node restraint";
    public static string Name => "Restraint";
    public static string NickName => "Res";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaRestrainedGoo(GsaBool6 item) {
      Value = item;
    }

    public override IGH_Goo Duplicate() {
      return new GsaRestrainedGoo(Value);
    }
  }
}
