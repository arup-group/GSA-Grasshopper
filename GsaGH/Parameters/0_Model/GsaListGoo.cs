using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaGridLine" /> can be used in Grasshopper.
  /// </summary>
  public class GsaGridLineGoo : GH_OasysGoo<GsaGridLine> {
    public static string Description => "GSA Grid Line";
    public static string Name => "GridLine";
    public static string NickName => "GL";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaGridLineGoo(GsaGridLine item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaGridLineGoo(Value);
    }
  }
}
