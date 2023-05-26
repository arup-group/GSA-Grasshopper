using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaBool6" /> can be used in Grasshopper.
  /// </summary>
  public class GsaBool6Goo : GH_OasysGoo<GsaBool6> {
    public static string Description
      => "GSA Bool6 (A 6-character string to describe the restraint condition (F = Fixed, R = Released) for each degree of freedom)";
    public static string Name => "Bool6";
    public static string NickName => "B6";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBool6Goo(GsaBool6 item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaBool6Goo(Value);
    }
  }
}
