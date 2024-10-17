using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaAssembly" /> can be used in Grasshopper.
  /// </summary>
  public class GsaAssemblyGoo : GH_OasysGoo<GsaAssembly> {
    public static string Description => "GSA Assembly";
    public static string Name => "Assembly";
    public static string NickName => "As";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAssemblyGoo(GsaAssembly item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaAssemblyGoo(Value);
    }
  }
}
