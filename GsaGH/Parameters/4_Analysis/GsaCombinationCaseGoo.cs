using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaCombinationCase"/> can be used in Grasshopper.
  /// </summary>
  public class GsaCombinationCaseGoo : GH_OasysGoo<GsaCombinationCase>
  {
    public static string Name => "Combination Case";
    public static string NickName => "CC";
    public static string Description => "GSA Combination Case";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaCombinationCaseGoo(GsaCombinationCase item) : base(item) { }
    public override IGH_Goo Duplicate() => new GsaCombinationCaseGoo(this.Value);
  }
}
