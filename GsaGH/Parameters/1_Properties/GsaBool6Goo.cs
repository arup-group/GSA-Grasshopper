using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaBool6" /> can be used in Grasshopper.
  /// </summary>
  public class GsaBool6Goo : GH_OasysGoo<GsaBool6> {
    public static string Description =>
      "GSA Bool6 containing six booleans representing a release or restriant.";
    public static string Name => "Bool6";
    public static string NickName => "B6";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBool6Goo(GsaBool6 item) {
      Value = item;
    }

    public override IGH_Goo Duplicate() {
      return new GsaBool6Goo(Value);
    }
  }
}
