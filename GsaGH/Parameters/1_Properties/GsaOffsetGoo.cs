using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaOffset" /> can be used in Grasshopper.
  /// </summary>
  public class GsaOffsetGoo : GH_OasysGoo<GsaOffset> {
    public static string Description => "GSA Offset";
    public static string Name => "Offset";
    public static string NickName => "Off";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaOffsetGoo(GsaOffset item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaOffsetGoo(Value);
    }
  }
}
