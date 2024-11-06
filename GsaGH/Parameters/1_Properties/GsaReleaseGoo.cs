using System.Linq;

using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaReleaseGoo" /> can be used in Grasshopper.
  /// </summary>
  public class GsaReleaseGoo : GH_OasysGoo<GsaBool6> {
    public static string Description =>
      "GSA releases containing six booleans representing the status";
    public static string Name => "Release";
    public static string NickName => "Rel";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaReleaseGoo(GsaBool6 item) {
      Value = item;
    }

    public override IGH_Goo Duplicate() {
      return new GsaReleaseGoo(Value);
    }
  }
}
