using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaList" /> can be used in Grasshopper.
  /// </summary>
  public class GsaListGoo : GH_OasysGoo<GsaList> {
    public static string Description => "GSA Entity List";
    public static string Name => "List";
    public static string NickName => "L";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaListGoo(GsaList item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaListGoo(Value);
    }
  }
}
